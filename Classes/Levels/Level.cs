using Grief.Classes.DesignPatterns.Builder.Builders;
using Grief.Classes.DesignPatterns.Builder;
using Grief.Classes.DesignPatterns.Composite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System.Collections.Generic;
using Grief.Classes.DesignPatterns.Command.Commands;
using Grief.Classes.DesignPatterns.Command;
using Microsoft.Xna.Framework.Input;

namespace Grief.Classes.Levels
{
    public class Level
    {
        private TiledMap map;
        private TiledMapRenderer mapRenderer;

        public List<GameObject> GameObjects { get; private set; } = new List<GameObject>();

        public void Load(string levelName)
        {
            map = GameWorld.Instance.Content.Load<TiledMap>(levelName);
            mapRenderer = new TiledMapRenderer(GameWorld.Instance.GraphicsDevice, map);
            InputHandler.Instance.AddButtonDownCommand(Keys.K, new ToggleColliderDrawingCommand(GameObjects));

            switch (levelName)
            {
                case "Level0":
                    //Her kan vi lave koden til en main menu
                    break;
                case "Level1":
                    GameObjects.Add(CreatePlayer(new Vector2(100,400))); //Sæt positionen for spilleren her
                    //Vi kan tilføje flere GameObjects i Level 1 her
                    break;
            }
        }

        private GameObject CreatePlayer(Vector2 position)
        {
            PlayerBuilder playerBuilder = new PlayerBuilder();
            GameObjectDirector director = new GameObjectDirector(playerBuilder);
            var player = director.Construct("Player");
            playerBuilder.SetPosition(position);
            return player;
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
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            mapRenderer.Draw();

            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Draw(spriteBatch);
            }
        }
    }
}
