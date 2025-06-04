using Greif;
using System;
using Greif.Classes.Cameras;
using Grief.Classes.DesignPatterns.Command;
using Grief.Classes.DesignPatterns.Command.Commands;
using Grief.Classes.Dialog;
using Grief.Classes.GameManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Diagnostics;

namespace Greif
{
    /// <summary>
    /// Den centrale klasse for hele spillet – Singleton mønster
    /// </summary>
    public class GameWorld : Game
    {
        // Private fields
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        /// <summary>
        /// Tidsforskel mellem frames
        /// </summary>
        public float DeltaTime { get; private set; }

        /// <summary>
        /// 1x1 pixel til at tegne primitive shapes
        /// </summary>
        public Texture2D Pixel { get; private set; }

        /// <summary>
        /// Standard font til tekst i spillet
        /// </summary>
        public SpriteFont DefaultFont { get; private set; }

        /// <summary>
        /// Kamera til at flytte og zoome viewet
        /// </summary>
        public Camera Camera { get; private set; }

        /// <summary>
        /// Dialogsystem – håndterer dialogbokse og tekst
        /// </summary>
        public DialogSystem Dialog { get; private set; }

        /// <summary>
        /// GameManager – håndterer alle scener (menu, level, pause, mv.)
        /// </summary>
        public GameManager GameManager { get; private set; }

        // Singleton setup
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

        /// <summary>
        /// Privat constructor – Singleton pattern
        /// </summary>
        private GameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Sæt vinduestørrelse (kan tilpasses)
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
        }

        /// <summary>
        /// Initialiserer alle systemer og scener (MainMenu, Level, Pause osv.)
        /// </summary>
        protected override void Initialize()
        {
            SoundManager.LoadContent(Content);

            GameManager = new GameManager(); // Scene- og state-system
            Camera = new Camera();
            Dialog = new DialogSystem();

            base.Initialize();
        }

        /// <summary>
        /// Loader grafik og fonts
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });

            DefaultFont = Content.Load<SpriteFont>("Fonts/Default");
        }

        /// <summary>
        /// Opdaterer spillets logik (input, scene, dialog)
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            //    Keyboard.GetState().IsKeyDown(Keys.Escape))
            //{
            //    Exit();
            //}

            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            InputHandler.Instance.Execute();

            // Opdater aktiv scene via GameManager (kan være MainMenu, Level, Pause osv.)
            GameManager.Update(gameTime);

            Dialog.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// Tegner alt i nuværende scene
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: Camera.ViewMatrix
            );

            // Tegn nuværende scene via GameManager
            GameManager.Draw(_spriteBatch);

            // Tegn dialog ovenpå scenen (fx tekstbokse)
            Dialog.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
