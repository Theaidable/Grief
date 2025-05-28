using Greif;
using Greif.Classes.Cameras;
using Grief.Classes.Algorithms;
using Grief.Classes.DesignPatterns.Builder;
using Grief.Classes.DesignPatterns.Builder.Builders;
using Grief.Classes.DesignPatterns.Command;
using Grief.Classes.DesignPatterns.Command.Commands;
using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Grief.Classes.DesignPatterns.Factories.ObjectFactories.Enemy;
using Grief.Classes.Items.Items;
using Grief.Classes.Quests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System.Collections.Generic;
using System.Linq;

namespace Grief.Classes.Levels
{
    public class Level
    {
        private TiledMapRenderer mapRenderer;
        private List<GameObject> enemies = new List<GameObject>();

        public TiledMap Map { get; private set; }
        public Dictionary<Point,Tile> TileDictionary { get; private set; }
        public AStar PathFinder { get; private set; }

        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }
        public List<GameObject> GameObjects { get; private set; } = new List<GameObject>();
        private List<GameObject> objectsToRemove = new List<GameObject>();
        public List<Rectangle> CollisionRectangles { get; private set; } = new List<Rectangle>();
        public List<Polygon> CollisionPolygons { get; private set; } = new List<Polygon>();

        public void Load(string levelName)
        {
            Map = GameWorld.Instance.Content.Load<TiledMap>($"TileMaps/{levelName}");
            mapRenderer = new TiledMapRenderer(GameWorld.Instance.GraphicsDevice, Map);

            MapWidth = Map.WidthInPixels;
            MapHeight = Map.HeightInPixels;

            var objectLayer = Map.GetLayer<TiledMapObjectLayer>("CollisionObjects");
            foreach (var rectangleObject in objectLayer.Objects.OfType<TiledMapRectangleObject>())
            {
                CollisionRectangles.Add(new Rectangle(
                (int)rectangleObject.Position.X,
                (int)rectangleObject.Position.Y,
                (int)rectangleObject.Size.Width,
                (int)rectangleObject.Size.Height));
            }

            foreach(var polygonObject in objectLayer.Objects.OfType<TiledMapPolygonObject>())
            {
                var points = polygonObject.Points.Select(p => new Vector2(polygonObject.Position.X + p.X, polygonObject.Position.Y + p.Y)).ToArray();

                CollisionPolygons.Add(new Polygon(points));
            }

            Dictionary<Point, Tile> tiles = new Dictionary<Point, Tile>();
            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    Point gridPosition = new Point(x, y);
                    Rectangle tileRectangle = new Rectangle(x * Map.TileWidth, y * Map.TileHeight, Map.TileWidth, Map.TileHeight);
                    bool walkable = !CollisionRectangles.Any(r => r.Intersects(tileRectangle));
                    tiles[gridPosition] = new Tile(gridPosition,walkable);
                }
            }

            TileDictionary = tiles;
            PathFinder = new AStar(tiles);

            switch (levelName)
            {
                case "Level0":
                    //Her kan vi lave koden til en main menu
                    break;
                case "GriefMap1":
                    //Tilføj enemy

                    enemies.Add(EnemyFactory.Instance.Create(new Vector2(500, 150), EnemyType.Enemy1, new List<Vector2> { new Vector2(550, 167), new Vector2(450, 167) }));
                    enemies.Add(EnemyFactory.Instance.Create(new Vector2(1325, 150), EnemyType.Enemy1, null, new QuestItem("Doll")));

                    foreach (var enemy in enemies)
                    {
                        AddGameObject(enemy);
                    }

                    //Tilføj en NPC i spillet
                    AddGameObject(CreateNPC(
                        new Vector2(80, 175),
                        "Dad",
                        new List<string>
                        {
                            "Have you seen my daughter?",
                            "She should be around here...",
                            "   NAME!!!",
                            "Could you help me find her?"
                        },
                        new FetchQuest
                        (
                            "Look for his Daughter",
                            "Look for my daughter and bring her back to me",
                            "Doll",
                            new StoryItem("DiaryPage #1")
                        )
                        ));


                    AddGameObject(CreatePlayer(new Vector2(100, 175)));

                    //Vi kan tilføje flere GameObjects i Level 1 her
                    break;
            }
        }

        private GameObject CreatePlayer(Vector2 position)
        {
            PlayerBuilder playerBuilder = new PlayerBuilder();
            GameObjectDirector director = new GameObjectDirector(playerBuilder);
            playerBuilder.SetPosition(position);
            playerBuilder.AddScriptComponent<InventoryComponent>();
            playerBuilder.AddScriptComponent<PlayerComponent>();
            var player = director.Construct("Player");
            return player;
        }

        private GameObject CreateNPC(Vector2 position, string name, List<string> dialog, Quest quest = null)
        {
            NpcBuilder npcBuilder = new NpcBuilder();
            GameObjectDirector director = new GameObjectDirector(npcBuilder);
            npcBuilder.SetPosition(position);
            npcBuilder.SetName(name);
            npcBuilder.SetDialog(dialog);
            npcBuilder.SetQuest(quest);
            npcBuilder.AddScriptComponent<NpcComponent>();
            var npc = director.Construct($"{name}");
            return npc;
        }

        public void AddGameObject(GameObject gameObject)
        {
            GameObjects.Add(gameObject);
            gameObject.Awake();
            gameObject.Start();
        }

        public void QueueRemove(GameObject gameObject)
        {
            objectsToRemove.Add(gameObject);
        }

        public void Update(GameTime gameTime)
        {
            mapRenderer.Update(gameTime);

            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Update();
            }

            foreach (GameObject gameObject in objectsToRemove)
            {
                GameObjects.Remove(gameObject);
            }
            objectsToRemove.Clear();


            var player = GameObjects.FirstOrDefault(g => g.Tag == "Player");

            if (player != null)
            {
                GameWorld.Instance.Camera.Follow(player, MapWidth, MapHeight);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Matrix viewMatrix)
        {
            mapRenderer.Draw(viewMatrix);

            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Draw(spriteBatch);

                var inventory = gameObject.GetComponent<InventoryComponent>();
                if (inventory != null && inventory.ShowInventory)
                {
                    inventory.Draw(spriteBatch);
                }
            }
        }
    }
}
