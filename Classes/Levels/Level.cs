using Greif;
using Greif.Classes.Cameras;
using Grief.Classes.DesignPatterns.Builder;
using Grief.Classes.DesignPatterns.Builder.Builders;
using Grief.Classes.DesignPatterns.Command;
using Grief.Classes.DesignPatterns.Command.Commands;
using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Grief.Classes.DesignPatterns.Factories.ObjectFactories.Enemy;
using Grief.Classes.Quests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System.Collections.Generic;
using System.Linq;

namespace Grief.Classes.Levels
{
    public class Level
    {
        private TiledMap map;
        private TiledMapRenderer mapRenderer;

        public List<GameObject> GameObjects { get; private set; } = new List<GameObject>();
        public List<Rectangle> CollisionRectangles { get; private set; } = new List<Rectangle>();

        public void Load(string levelName)
        {
            map = GameWorld.Instance.Content.Load<TiledMap>($"TileMaps/{levelName}");
            mapRenderer = new TiledMapRenderer(GameWorld.Instance.GraphicsDevice, map);
            InputHandler.Instance.AddButtonDownCommand(Keys.K, new ToggleColliderDrawingCommand(GameObjects));

            switch (levelName)
            {
                case "Level0":
                    //Her kan vi lave koden til en main menu
                    break;
                case "GriefMap2":


                    /* Forsøg på at instansiere tiles fra tmx filen til at finde deres collider
                    TiledMapTileLayer collisionLayer = map.GetLayer<TiledMapTileLayer>("Ground");
                    for (int x = 0; x < collisionLayer.Width; x++)
                    {
                        for (int y = 0; y < collisionLayer.Height; y++)
                        {
                            TiledMapTile? tile;
                            
                            if(collisionLayer.TryGetTile((ushort)x, (ushort)y, out tile) && tile.HasValue)
                            {
                                var tileset = map.GetTilesetByTileGlobalIdentifier(tile.Value.GlobalIdentifier);
                                var tileId = tile.Value.GlobalIdentifier - map.GetTilesetFirstGlobalIdentifier(tileset);
                                var tileProperties = tileset.Tiles.FirstOrDefault(t => t.LocalTileIdentifier == tileId);

                                if (tileProperties.Properties.ContainsKey("collidable") && tileProperties.Properties["collidable"] == "true")
                                {
                                    var tileRectangle = new Rectangle(x * map.TileWidth, y * map.TileHeight, map.TileWidth, map.TileHeight);
                                    CollisionRectangles.Add(tileRectangle);
                                }
                            }
                        }
                    }
                    */

                    AddGameObject(CreatePlayer(new Vector2(250,207)));

                    /*
                    //Tilføj enemy
                    GameObject enemyObject = EnemyFactory.Instance.Create(new Vector2(300, 400));
                    GameObjects.Add(enemyObject);
                    */

                    //Tilføj en NPC i spillet
                    /*AddGameObject(CreateNPC(
                        new Vector2(200, 400),
                        "Mor",
                        new List<string>
                        {
                            "Have you seen my daughter?",
                            "I know she is somewhere around here...",
                            "Can you help me find her?"
                        },
                        new Quest()));
                    */

                    //Vi kan tilføje flere GameObjects i Level 1 her
                    break;
            }
        }

        private GameObject CreatePlayer(Vector2 position)
        {
            PlayerBuilder playerBuilder = new PlayerBuilder();
            GameObjectDirector director = new GameObjectDirector(playerBuilder);
            var player = director.Construct("Player");
            playerBuilder.AddScriptComponent<PlayerComponent>();
            playerBuilder.SetPosition(position);
            return player;
        }

        private GameObject CreateNPC(Vector2 position, string name, List<string> dialog, Quest quest = null)
        {
            NpcBuilder npcBuilder = new NpcBuilder();
            GameObjectDirector director = new GameObjectDirector(npcBuilder);
            var npc = director.Construct($"{name}");
            npcBuilder.SetPosition(position);
            npcBuilder.SetName(name);
            npcBuilder.SetDialog(dialog);
            npcBuilder.SetQuest(quest);
            return npc;
        }

        public void AddGameObject(GameObject gameObject)
        {
            GameObjects.Add(gameObject);
            gameObject.Awake();
            gameObject.Start();
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            GameObjects.Remove(gameObject);
        }

        public void Update(GameTime gameTime)
        {
            mapRenderer.Update(gameTime);

            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Update();
            }

            var player = GameObjects.FirstOrDefault(g => g.Tag == "Player");

            if (player != null)
            {
                GameWorld.Instance.Camera.Follow(player.Transform.Position);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Matrix viewMatrix)
        {
            mapRenderer.Draw(viewMatrix);

            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Draw(spriteBatch);
            }
        }
    }
}
