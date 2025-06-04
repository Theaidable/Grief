using Greif;
using Grief.Classes.DesignPatterns.Builder;
using Grief.Classes.DesignPatterns.Builder.Builders;
using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Grief.Classes.DesignPatterns.Factories.ObjectFactories.Enemy;
using Grief.Classes.GameManager.Scenes;
using Grief.Classes.Items;
using Grief.Classes.Items.Items;
using Grief.Classes.Levels;
using Grief.Classes.Quests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Grief.Classes.GameManager
{
    public class GameManager
    {
        public enum GameState
        {
            MainMenu,
            LoadGame,
            SaveGame,
            Level,
            Paused
        }

        private Scene mainMenu;
        private Scene loadGameScene;
        private Scene saveGameScene;
        private PauseOverlay pauseOverlay;

        private KeyboardState previousKeyState;

        private string connectionString;

        public GameState CurrentState { get; private set; }

        public LevelManager LevelManager { get; private set; }

        public GameManager()
        {
            mainMenu = new MainMenu();
            mainMenu.LoadContent();

            loadGameScene = new LoadGame();
            saveGameScene = new SaveGame();
            pauseOverlay = new PauseOverlay();

            //connectionString = "Server=DAVID\\SQLEXPRESS01;Database=Shattered Reflections;Trusted_Connection=True;TrustServerCertificate=True;";
            connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GriefGameDB;Integrated Security=True";
            
            LevelManager = new LevelManager();
            LevelManager.LoadLevel("GriefMap1");

            ChangeState(GameState.MainMenu);
        }

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;

            switch (newState)
            {
                case GameState.MainMenu:
                    // Setup for MainMenu
                    break;
                case GameState.LoadGame:
                    loadGameScene.LoadContent();// Setup for LoadGame
                    break;
                case GameState.SaveGame:
                    saveGameScene.LoadContent();// Setup for SaveGame
                    break;
                case GameState.Level:
                    // Setup gameplay
                    break;
                case GameState.Paused:
                    pauseOverlay.LoadContent(); // Pause-specific actions
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();

            switch (CurrentState)
            {
                case GameState.MainMenu:
                    GameWorld.Instance.Camera.Position = Vector2.Zero;
                    mainMenu.Update(gameTime); 
                    break;

                case GameState.LoadGame:
                    loadGameScene.Update(gameTime);
                    break;

                case GameState.SaveGame:
                    saveGameScene.Update(gameTime);
                    break;

                case GameState.Level:
                    // TJEK for ESC kun i Level-state
                    if (keyState.IsKeyDown(Keys.Escape) && previousKeyState.IsKeyUp(Keys.Escape))
                    {
                        ChangeState(GameState.Paused);
                    }
                    else
                    {
                        LevelManager.Update(gameTime);
                    }
                    break;

                case GameState.Paused:
                    pauseOverlay.Update(gameTime);
                    break;
            }

            previousKeyState = keyState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (CurrentState)
            {
                case GameState.MainMenu:
                    mainMenu.Draw(spriteBatch);
                    break;

                case GameState.LoadGame:
                    loadGameScene.Draw(spriteBatch);
                    break;

                case GameState.SaveGame:
                    LevelManager.Draw(spriteBatch, GameWorld.Instance.Camera.ViewMatrix);
                    saveGameScene.Draw(spriteBatch);
                    break;

                case GameState.Level:
                    LevelManager.Draw(spriteBatch, GameWorld.Instance.Camera.ViewMatrix);
                    break;

                case GameState.Paused:
                    LevelManager.Draw(spriteBatch, GameWorld.Instance.Camera.ViewMatrix);
                    pauseOverlay.Draw(spriteBatch);
                    break;
            }
        }

        /// <summary>
        /// Metode til at gemme spillets tilstand
        /// </summary>
        /// <param name="saveSlot"></param>
        public void SaveGame(int saveSlot)
        {
            var player = LevelManager.CurrentLevel.GameObjects.FirstOrDefault(p => p.GetComponent<PlayerComponent>() != null);
            var playerComponent = player?.GetComponent<PlayerComponent>();
            var inventoryComponent = player?.GetComponent<InventoryComponent>();
            var items = inventoryComponent?.Items;

            if(player == null || playerComponent == null)
            {
                return;
            }

            var enemies = LevelManager.CurrentLevel.GameObjects.Where(gameObject => gameObject.GetComponent<EnemyComponent>() != null);
            var npcs = LevelManager.CurrentLevel.GameObjects.Where(gameObject => gameObject.GetComponent<NpcComponent>() != null);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                //Gem level hvis ikke det findes
                string insertLevel = @"
            IF NOT EXISTS (SELECT * FROM Level WHERE levelName = @levelName)
            BEGIN
                INSERT INTO Level(levelName) VALUES (@levelName)
            END";

                using (SqlCommand cmd = new SqlCommand(insertLevel, con))
                {
                    cmd.Parameters.AddWithValue("@levelName", LevelManager.CurrentLevel.LevelName);
                    cmd.ExecuteNonQuery();
                }

                //Hent levelID
                int levelID;
                string getLevelIdQuery = "SELECT levelID FROM Level WHERE levelName = @levelName";

                using (SqlCommand cmd = new SqlCommand(getLevelIdQuery, con))
                {
                    cmd.Parameters.AddWithValue("@levelName", LevelManager.CurrentLevel.LevelName);
                    levelID = (int)cmd.ExecuteScalar();
                }

                //Slet tidligere save på samme slot
                string deleteSave = "DELETE FROM Player WHERE playerID = @slot";
                using (SqlCommand cmd = new SqlCommand(deleteSave, con))
                {
                    cmd.Parameters.AddWithValue("@slot", saveSlot);
                    cmd.ExecuteNonQuery();
                }

                //PLAYER

                //Insert Player
                string insertPlayer = @"
                    INSERT INTO Player (playerID, playerHealth, positionX, positionY, currentLevelID, saveTimeStamp)
                    VALUES (@playerID, @health, @posX, @posY, @levelID, @timestamp)";
                
                using (SqlCommand cmd = new SqlCommand(insertPlayer, con))
                {
                    cmd.Parameters.AddWithValue("@playerID", saveSlot);
                    cmd.Parameters.AddWithValue("@health", playerComponent.Health);
                    cmd.Parameters.AddWithValue("@posX", player.Transform.Position.X);
                    cmd.Parameters.AddWithValue("@posY", player.Transform.Position.Y);
                    cmd.Parameters.AddWithValue("@levelID", levelID);
                    cmd.Parameters.AddWithValue("@timestamp", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }

                //INVENTORY OG ITEMS

                //Først slet tidligere inventory
                string deleteInventory = "DELETE FROM Inventory WHERE playerID = @playerID";

                using (SqlCommand cmd = new SqlCommand(deleteInventory, con))
                {
                    cmd.Parameters.AddWithValue("@playerID", saveSlot);
                    cmd.ExecuteNonQuery();
                }

                //Indsæt nuværende items
                if(items != null)
                {
                    foreach (Item item in items)
                    {
                        string insertItem = @"
                    IF NOT EXISTS (SELECT * FROM Item WHERE itemName = @itemName)
                    BEGIN
                        INSERT INTO Item (itemName, itemType) VALUES (@itemName, @itemType)
                    END";

                        using (SqlCommand cmd = new SqlCommand(insertItem, con))
                        {
                            cmd.Parameters.AddWithValue("@itemName", item.DisplayName);
                            cmd.Parameters.AddWithValue("@itemType", item.Type);
                            cmd.ExecuteNonQuery();
                        }

                        //Find itemID
                        int itemID;
                        string getItemID = "SELECT itemID FROM Item WHERE itemName = @itemName";

                        using (SqlCommand cmd = new SqlCommand(getItemID, con))
                        {
                            cmd.Parameters.AddWithValue("itemName", item.DisplayName);
                            itemID = (int)cmd.ExecuteScalar();
                        }

                        //Gem i Inventory
                        string insertInventory = "INSERT INTO Inventory (playerID, itemID) VALUES (@playerID, @itemID)";

                        using (SqlCommand cmd = new SqlCommand(insertInventory, con))
                        {
                            cmd.Parameters.AddWithValue("@playerID", saveSlot);
                            cmd.Parameters.AddWithValue("@itemID", itemID);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                //ENEMIES

                //slet tidligere fjender
                string deleteEnemies = "DELETE FROM Enemy WHERE levelID = @levelID";

                using (SqlCommand cmd = new SqlCommand(deleteEnemies, con))
                {
                    cmd.Parameters.AddWithValue("@levelID", levelID);
                    cmd.ExecuteNonQuery();
                }

                if(enemies != null)
                {
                    //Gem fjender
                    foreach (GameObject enemy in enemies)
                    {
                        var enemyComponent = enemy.GetComponent<EnemyComponent>();
                        var enemyType = enemyComponent.EnemyType.ToString();

                        string insertEnemy = @"
                                INSERT INTO Enemy (enemyType, enemyHealth, positionX, positionY, levelID)
                                VALUES (@type, @health, @posX, @posY, @levelID)";

                        using (SqlCommand cmd = new SqlCommand(insertEnemy, con))
                        {
                            cmd.Parameters.AddWithValue("@type", enemyType);
                            cmd.Parameters.AddWithValue("@health", enemyComponent.EnemyHealth);
                            cmd.Parameters.AddWithValue("@posX", enemy.Transform.Position.X);
                            cmd.Parameters.AddWithValue("@posY", enemy.Transform.Position.Y);
                            cmd.Parameters.AddWithValue("@levelID", levelID);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                //NPCS

                // Slet quests knyttet til de NPCs i dette level
                string deleteQuests = "DELETE FROM Quest WHERE questGiver IN (SELECT npcID FROM NPC WHERE levelID = @levelID)";
                using (SqlCommand cmd = new SqlCommand(deleteQuests, con))
                {
                    cmd.Parameters.AddWithValue("@levelID", levelID);
                    cmd.ExecuteNonQuery();
                }
                //Slet tidligere NPCer
                string deleteNpcs = "DELETE FROM NPC WHERE levelID = @levelID";

                using (SqlCommand cmd = new SqlCommand(deleteNpcs, con))
                {
                    cmd.Parameters.AddWithValue("@levelID", levelID);
                    cmd.ExecuteNonQuery();
                }

                if(npcs != null)
                {
                    //Gem NPC'er
                    foreach (GameObject npc in npcs)
                    {
                        var npcComponent = npc.GetComponent<NpcComponent>();
                        string name = npcComponent.Name;

                        string insertNpc = @"
                            INSERT INTO NPC (npcName, positionX, positionY, levelID)
                            OUTPUT INSERTED.npcID
                            VALUES (@name, @x, @y, @levelID)";

                        int npcID;
                        using (SqlCommand cmd = new SqlCommand(insertNpc, con))
                        {
                            cmd.Parameters.AddWithValue("@name", name);
                            cmd.Parameters.AddWithValue("@x", npc.Transform.Position.X);
                            cmd.Parameters.AddWithValue("@y", npc.Transform.Position.Y);
                            cmd.Parameters.AddWithValue("@levelID", levelID);
                            npcID = (int)cmd.ExecuteScalar();
                        }

                        if (npcComponent.QuestToGive is FetchQuest fetch)
                        {
                            int? requiredItemID = null;

                            //Sikr at item eksisterer
                            if(string.IsNullOrEmpty(fetch.RequiredItemName) == false)
                            {
                                string insertItem = @"
                        IF NOT EXISTS (SELECT * FROM Item WHERE itemName = @itemName)
                        BEGIN
                            INSERT INTO Item (itemName, itemType) VALUES (@itemName, 'QuestItem')
                        END";

                                using (SqlCommand cmd = new SqlCommand(insertItem, con))
                                {
                                    cmd.Parameters.AddWithValue("@itemName", fetch.RequiredItemName);
                                    cmd.ExecuteNonQuery();
                                }

                                //Hent itemID
                                using (SqlCommand cmd = new SqlCommand("SELECT itemID FROM Item WHERE itemName = @itemName", con))
                                {
                                    cmd.Parameters.AddWithValue("@itemName", fetch.RequiredItemName);
                                    requiredItemID = (int)cmd.ExecuteScalar();
                                }

                                //Insert Quest
                                string insertQuest = @"
                                    INSERT INTO Quest (questName, questGiver, description, requiredItemName, isAccepted, isCompleted)
                                    VALUES (@title, @giver, @desc, @item, @accepted, @completed)";

                                using (SqlCommand cmd = new SqlCommand(insertQuest, con))
                                {
                                    cmd.Parameters.AddWithValue("@title", fetch.Title);
                                    cmd.Parameters.AddWithValue("@giver", npcID);
                                    cmd.Parameters.AddWithValue("@desc", fetch.Description);
                                    cmd.Parameters.AddWithValue("@item", requiredItemID.HasValue ? requiredItemID.Value : (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@accepted", fetch.IsAccepted);
                                    cmd.Parameters.AddWithValue("@completed", fetch.IsCompleted);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void LoadGame(int saveSlot)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // 1. Hent Player info
                string getPlayer = @"
                    SELECT playerHealth, positionX, positionY, currentLevelID 
                    FROM Player 
                    WHERE playerID = @slot";

                int playerHealth = 0;
                Vector2 playerPosition = Vector2.Zero;
                int levelID = 0;

                using (SqlCommand cmd = new SqlCommand(getPlayer, con))
                {
                    cmd.Parameters.AddWithValue("@slot", saveSlot);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            playerHealth = reader.GetInt32(0);
                            playerPosition = new Vector2((float)reader.GetDouble(1), (float)reader.GetDouble(2));
                            levelID = reader.GetInt32(3);
                        }
                    }
                }

                //Find levelName og Load Level
                string levelName = "";
                
                using (SqlCommand cmd = new SqlCommand("SELECT levelName FROM Level WHERE levelID = @id", con))
                {
                    cmd.Parameters.AddWithValue("@id", levelID);
                    levelName = (string)cmd.ExecuteScalar();
                }

                var levelManager = GameWorld.Instance.GameManager.LevelManager;
                levelManager.LoadLevel(levelName);

                //Skab Player og sæt position + liv
                var playerBuilder = new PlayerBuilder();
                var director = new GameObjectDirector(playerBuilder);
                playerBuilder.SetPosition(playerPosition);
                playerBuilder.AddScriptComponent<InventoryComponent>();
                playerBuilder.AddScriptComponent<PlayerComponent>();
                var player = director.Construct("Player");

                var playerComp = player.GetComponent<PlayerComponent>();
                playerComp.Health = playerHealth;

                levelManager.CurrentLevel.AddGameObject(player);

                //Load Inventory
                var inventory = player.GetComponent<InventoryComponent>();
                string getInventory = @"
                    SELECT I.itemName, I.itemType
                    FROM Inventory Inv
                    JOIN Item I ON Inv.itemID = I.itemID
                    WHERE Inv.playerID = @playerID";

                using (SqlCommand cmd = new SqlCommand(getInventory, con))
                {
                    cmd.Parameters.AddWithValue("@playerID", saveSlot);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader.GetString(0);
                            string type = reader.GetString(1);

                            Item item = type switch
                            {
                                "StoryItem" => new StoryItem(name, type),
                                "QuestItem" => new QuestItem(name, type),
                            };

                            inventory.AddItemToInventory(item);
                        }
                    }
                }

                //Load Enemies
                string getEnemies = @"
                    SELECT enemyType, enemyHealth, positionX, positionY 
                    FROM Enemy 
                    WHERE levelID = @levelID";

                using (SqlCommand cmd = new SqlCommand(getEnemies, con))
                {
                    cmd.Parameters.AddWithValue("@levelID", levelID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string typeStr = reader.GetString(0);
                            int health = reader.GetInt32(1);
                            float x = (float)reader.GetDouble(2);
                            float y = (float)reader.GetDouble(3);

                            if (Enum.TryParse(typeStr, out EnemyType enemyType))
                            {
                                var enemy = EnemyFactory.Instance.Create(new Vector2(x, y), enemyType);
                                enemy.GetComponent<EnemyComponent>().EnemyHealth = health;
                                levelManager.CurrentLevel.AddGameObject(enemy);
                            }
                        }
                    }
                }

                //Load NPCs og Quests
                string query = @"
                    SELECT 
                    NPC.npcID, NPC.npcName, NPC.positionX, NPC.positionY,
                    Q.questName, Q.description, I.itemName AS requiredItemName, Q.isAccepted, Q.isCompleted
                FROM NPC
                LEFT JOIN Quest Q ON Q.questGiver = NPC.npcID
                LEFT JOIN Item I ON Q.requiredItemName = I.itemID
                WHERE NPC.levelID = @levelID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@levelID", levelID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int npcID = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            float x = (float)reader.GetDouble(2);
                            float y = (float)reader.GetDouble(3);

                            // Quest-data (kan være null)
                            string questName = reader.IsDBNull(4) ? null : reader.GetString(4);
                            string questDesc = reader.IsDBNull(5) ? null : reader.GetString(5);
                            string requiredItemName = reader.IsDBNull(6) ? null : reader.GetString(6);
                            bool? accepted = reader.IsDBNull(7) ? (bool?)null : reader.GetBoolean(7);
                            bool? completed = reader.IsDBNull(8) ? (bool?)null : reader.GetBoolean(8);

                            Quest quest = null;
                            if (questName != null)
                            {
                                var reward = new StoryItem("DiaryPage", "StoryItem");
                                var fetchQuest = new FetchQuest(questName, questDesc, requiredItemName, reward);

                                if (accepted == true) fetchQuest.Accept();
                                if (completed == true) fetchQuest.Complete();

                                quest = fetchQuest;
                            }

                            // Opret NPC via builder
                            var npcBuilder = new NpcBuilder();
                            var npcDirector = new GameObjectDirector(npcBuilder);
                            npcBuilder.SetPosition(new Vector2(x, y));
                            npcBuilder.SetName(name);
                            npcBuilder.SetDialog(new List<string> { "..." }, new List<string> { "..." }, new List<string> { "..." }, new List<string> { "..." });
                            npcBuilder.SetQuest(quest);
                            npcBuilder.AddScriptComponent<NpcComponent>();
                            var npc = npcDirector.Construct(name);

                            levelManager.CurrentLevel.AddGameObject(npc);
                        }
                    }
                }
            }
        }
    }
}
