using Greif;
using System;
using Greif.Classes.Cameras;
using Grief.Classes.DesignPatterns.Builder;
using Grief.Classes.DesignPatterns.Builder.Builders;
using Grief.Classes.DesignPatterns.Command;
using Grief.Classes.DesignPatterns.Command.Commands;
using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.Dialog;
using Grief.Classes.Levels;
using Grief.Classes.GameManager.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Diagnostics;
using Grief.Classes.Scenes;

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

        private Scene mainMenuScene;
        private Scene loadGameScene;
        private Scene levelScene;
        private PauseOverlay pauseOverlay;

        public GameManager()
        {
            mainMenuScene = new MainMenuScene();
            loadGameScene = new LoadGame();
            levelScene = new LevelScene();
            pauseOverlay = new PauseOverlay();

            ChangeState(GameState.MainMenu);
        }

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;

            // Handle any initialization or cleanup here if needed
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
                    // Pause-specific actions
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            switch (CurrentState)
            {
                case GameState.MainMenu:
                    mainMenuScene.Update(gameTime);
                    break;
                case GameState.LoadGame:
                    loadGameScene.Update(gameTime);
                    break;
                case GameState.Level:
                    levelScene.Update(gameTime);
                    break;
                case GameState.Paused:
                    levelScene.Update(gameTime); // Optional: update background gameplay minimally or pause entirely
                    pauseOverlay.Update(gameTime);
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (CurrentState)
            {
                case GameState.MainMenu:
                    mainMenuScene.Draw(spriteBatch);
                    break;
                case GameState.LoadGame:
                    loadGameScene.Draw(spriteBatch);
                    break;
                case GameState.Level:
                    levelScene.Draw(spriteBatch);
                    break;
                case GameState.Paused:
                    levelScene.Draw(spriteBatch);
                    pauseOverlay.Draw(spriteBatch);
                    break;
            }
        }
    }
}
