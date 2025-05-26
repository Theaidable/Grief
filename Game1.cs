using Greif.Classes.Cameras;
using Grief.Classes.DesignPatterns.Builder;
using Grief.Classes.DesignPatterns.Builder.Builders;
using Grief.Classes.DesignPatterns.Command;
using Grief.Classes.DesignPatterns.Command.Commands;
using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System.Collections.Generic;

namespace Greif
{
    public class GameWorld : Game
    {
        //Private fields
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Public properties
        public float DeltaTime { get; private set; }
        public Texture2D Pixel { get; private set; }
        public Camera Camera { get; private set; }
        public LevelManager LevelManager { get; private set; }

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
            //Midlertidig placering for indlæsning af første level indtil der laves en GameManager når vi skal arbejde med database
            LevelManager = new LevelManager();
            LevelManager.LoadLevel("GriefMap1"); //Skal ændres til Level0 når vi laver mainmenu
            Camera = new Camera();

            InputHandler.Instance.AddButtonDownCommand(Keys.K, new ToggleColliderDrawingCommand(LevelManager.CurrentLevel.GameObjects));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            InputHandler.Instance.Execute();

            LevelManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: Camera.ViewMatrix);

            LevelManager.Draw(_spriteBatch, Camera.ViewMatrix);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
