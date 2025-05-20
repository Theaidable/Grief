using Grief.Classes.DesignPatterns.Builder;
using Grief.Classes.DesignPatterns.Builder.Builders;
using Grief.Classes.DesignPatterns.Command;
using Grief.Classes.DesignPatterns.Command.Commands;
using Grief.Classes.DesignPatterns.Composite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Grief
{
    public class GameWorld : Game
    {
        //Private fields
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Public properties
        public float DeltaTime { get; private set; }
        public Texture2D Pixel { get; private set; }

        //Lister
        public List<GameObject> GameObjects { get; private set; } = new List<GameObject>();
        private List<GameObject> objectsToRemove = new List<GameObject>();

        //Oprettelse af Singleton af GameWorld
        private static GameWorld instance;
        public static GameWorld Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameWorld();
                }
                return instance;
            }
        }

        private GameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            PlayerBuilder playerBuilder = new PlayerBuilder();
            GameObjectDirector director = new GameObjectDirector(playerBuilder);
            GameObjects.Add(director.Construct("Player"));

            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Awake();
            }

            InputHandler.Instance.AddButtonDownCommand(Keys.K, new ToggleColliderDrawingCommand(GameObjects));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });

            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Start();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            InputHandler.Instance.Execute();

            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Update();
            }

            foreach (var obj in objectsToRemove)
            {
                GameObjects.Remove(obj);
            }
            objectsToRemove.Clear();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Draw(_spriteBatch);
            }

            base.Draw(gameTime);
        }
    }
}
