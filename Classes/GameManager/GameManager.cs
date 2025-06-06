using Greif;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Grief.Classes.GameManager.Scenes;
using Grief.Classes.Levels;

namespace Grief.Classes.GameManager
{
    /// <summary>
    /// GameManager klasse som styrer spillets tilstand
    /// </summary>
    /// <author>Asbjørn Ryberg</author>
    /// <author>David Gudmund Danielsen</author>
    public class GameManager
    {
        /// <summary>
        /// Enumartion for GameState
        /// </summary>
        public enum GameState
        {
            MainMenu,
            LoadGame,
            SaveGame,
            Level,
            Paused
        }

        //Fields
        public GameState CurrentState { get; private set; }

        private Scene mainMenu;
        private Scene loadGameScene;
        private Scene saveGameScene;
        private PauseOverlay pauseOverlay;

        private KeyboardState previousKeyState;

        public LevelManager LevelManager { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public GameManager()
        {
            mainMenu = new MainMenu();
            mainMenu.LoadContent();

            loadGameScene = new LoadGame();
            saveGameScene = new SaveGame();
            pauseOverlay = new PauseOverlay();

            LevelManager = new LevelManager();

            ChangeState(GameState.MainMenu);
        }

        /// <summary>
        /// Metode som bruges til at skifte spillets tilstand
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(GameState newState)
        {
            CurrentState = newState;

            switch (newState)
            {
                case GameState.MainMenu:
                    SoundManager.PlayMenuMusic(); // Setup for MainMenu
                    break;
                case GameState.LoadGame:
                    loadGameScene.LoadContent(); // Setup for LoadGame
                    break;
                case GameState.SaveGame:
                    saveGameScene.LoadContent(); // Setup for SaveGame
                    break;
                case GameState.Level:
                    SoundManager.PlayLevelMusic(); // Setup gameplay
                    break;
                case GameState.Paused:
                    pauseOverlay.LoadContent(); // Pause-specific actions
                    break;
            }
        }

        /// <summary>
        /// Kalder update i forhold til spillets tilstand
        /// </summary>
        /// <param name="gameTime"></param>
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

        /// <summary>
        /// Tegner det bestemte i forhold til GameState
        /// </summary>
        /// <param name="spriteBatch"></param>
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
    }
}
