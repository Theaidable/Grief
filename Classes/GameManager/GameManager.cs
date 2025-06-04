using Greif;
using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Grief.Classes.DesignPatterns.Factories.ObjectFactories.Enemy;
using Grief.Classes.GameManager.Scenes;
using Grief.Classes.Items;
using Grief.Classes.Levels;
using Grief.Classes.Quests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;

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

            connectionString = "Server=DAVID\\SQLEXPRESS01;Database=Shattered Reflections;Trusted_Connection=True;TrustServerCertificate=True;";

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
                string insertLevel = "IF NOT EXISTS (SELECT * FROM Level WHERE levelNAME = @levelName " +
                    "INSERT INTO Level(levelName) VALUES (@levelName)";

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
                string deleteSave = "DELETE FROM Player WHERE playerID = @saveSlot";
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
                            INSERT INTO Item (itemName, itemType) VALUES (@itemName, @itemType)";

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
                                    INSERT INTO Item (itemName, itemType) VALUES (@itemName, 'QuestItem')";

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

        public void LoadGame(int loadSlot)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                //Get player save
                string query = @"
                    SELECT playerHealth, positionX, positionY, levelName
                    FROM Player p
                    JOIN Level l ON p.currentLevelID = l.levelID
                    WHERE playerID = @slot";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@slot", loadSlot);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int health = reader.GetInt32(0);
                            float posX = (float)reader.GetDouble(1);
                            float posY = (float)reader.GetDouble(2);
                            string levelName = reader.GetString(3);

                            //Load Level
                        }
                    }
                }
            }
        }
    }
}
