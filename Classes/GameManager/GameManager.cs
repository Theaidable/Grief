using Greif;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System.Collections.Generic;
using Grief.Classes.GameManager.Scenes;
using Grief.Classes.Levels;

namespace Grief.Classes.GameManager
{
    public class GameManager
    {
        public enum GameState
        {
            MainMenu,
            LoadGame,
            Level,
            Paused
        }

        public GameState CurrentState { get; private set; }

        private Scene mainMenu;
        private Scene loadGameScene;
        private PauseOverlay pauseOverlay;

        private KeyboardState previousKeyState;

        public LevelManager LevelManager { get; private set; }

        public GameManager()
        {
            mainMenu = new MainMenu();
            mainMenu.LoadContent();

            loadGameScene = new LoadGame();           
            pauseOverlay = new PauseOverlay();

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
                    // Setup for LoadGame
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
                    mainMenu.Update(gameTime);
                    break;
                case GameState.LoadGame:
                    loadGameScene.Update(gameTime);
                    break;
                case GameState.Level:
                    // TJEK for ESC kun i Level-state
                    if (keyState.IsKeyDown(Keys.Escape) && previousKeyState.IsKeyUp(Keys.Escape))
                    {
                        ChangeState(GameState.Paused);
                        // evt. return; hvis du ikke vil have LevelManager.Update kørt samme frame
                    }
                    else
                    {
                        LevelManager.Update(gameTime);
                    }
                    break;
                case GameState.Paused:
                    LevelManager.Update(gameTime); // valgfrit
                    pauseOverlay.Update(gameTime);
                    break;
            }

            previousKeyState = keyState; // Opdater state til næste frame
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
