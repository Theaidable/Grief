using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Grief.Classes.DesignPatterns.Command
{
    /// <summary>
    /// Enumaration til de forskellige knapper på musen
    /// </summary>
    /// <author>Asbjørn Ryberg</author>
    /// <author>David Gudmund Danielsen</author>
    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    /// <summary>
    /// Styrecenter for spillerens inputs
    /// </summary>
    public class InputHandler
    {
        /// <summary>
        /// Singleton
        /// </summary>
        private static InputHandler instance;
        public static InputHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InputHandler();
                }
                return instance;
            }
        }

        /// <summary>
        /// private constructor
        /// </summary>
        private InputHandler() { }

        //Dictionaries
        private Dictionary<Keys, ICommand> keybindsUpdate = new Dictionary<Keys, ICommand>();
        private Dictionary<Keys, ICommand> keybindsButtonDown = new Dictionary<Keys, ICommand>();
        private Dictionary<Keys, ICommand> keybindsButtonUp = new Dictionary<Keys, ICommand>();
        private Dictionary<MouseButton, ICommand> mouseButtonDownBinds = new();
        
        //Previousstates
        private KeyboardState previousKeyState;
        private MouseState previousMouseState;

        /// <summary>
        /// Tilføje en update command - Handling som køres kontinuert mens knap holdes nede
        /// </summary>
        /// <param name="inputKey"></param>
        /// <param name="command"></param>
        public void AddUpdateCommand(Keys inputKey, ICommand command)
        {
            keybindsUpdate[inputKey] = command;
        }

        /// <summary>
        /// Tilføje en buttondown command - Handling som udføres 1 gang når man klikker på en knap
        /// </summary>
        /// <param name="inputKey"></param>
        /// <param name="command"></param>
        public void AddButtonDownCommand(Keys inputKey, ICommand command)
        {
            keybindsButtonDown[inputKey] = command;
        }

        /// <summary>
        /// Tilføje en buttonup command - Handling som udføres 1 gang når man slipper en knap
        /// </summary>
        /// <param name="inputKey"></param>
        /// <param name="command"></param>
        public void AddButtonUpCommand(Keys inputKey, ICommand command)
        {
            keybindsButtonUp[inputKey] = command;
        }

        /// <summary>
        /// Tilføj en mouseclick command - Handling som udføres når man klikker med musen
        /// </summary>
        /// <param name="button"></param>
        /// <param name="command"></param>
        public void AddMouseButtonDownCommand(MouseButton button, ICommand command)
        {
            mouseButtonDownBinds[button] = command;
        }

        /// <summary>
        /// Eksekvere commanden
        /// </summary>
        public void Execute()
        {
            KeyboardState keyState = Keyboard.GetState();


            foreach (var key in keybindsUpdate.Keys)
            {
                if (keyState.IsKeyDown(key))
                {
                    keybindsUpdate[key].Execute();
                    //break;
                }
            }

            foreach (var key in keybindsButtonDown.Keys)
            {
                if (!previousKeyState.IsKeyDown(key) && keyState.IsKeyDown(key))
                {
                    keybindsButtonDown[key].Execute();
                }
            }
              
            foreach (var key in keybindsButtonUp.Keys)
            {
                if (previousKeyState.IsKeyDown(key) && !keyState.IsKeyDown(key))
                {
                    keybindsButtonUp[key].Execute();
                }
            }

            previousKeyState = keyState;


            MouseState mouseState = Mouse.GetState();

            // Venstre klik
            if (previousMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
            {
                if (mouseButtonDownBinds.TryGetValue(MouseButton.Left, out var cmd))
                {
                    cmd.Execute();
                }
            }

            // Tilføj evt. højre og midterklik her senere

            previousMouseState = mouseState;


        }
    }
}
