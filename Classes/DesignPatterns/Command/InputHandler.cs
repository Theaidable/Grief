using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Grief.Classes.DesignPatterns.Command
{
    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    public class InputHandler
    {
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
        private InputHandler() { }

        private Dictionary<Keys, ICommand> keybindsUpdate = new Dictionary<Keys, ICommand>();
        private Dictionary<Keys, ICommand> keybindsButtonDown = new Dictionary<Keys, ICommand>();
        private Dictionary<Keys, ICommand> keybindsButtonUp = new Dictionary<Keys, ICommand>();
        private KeyboardState previousKeyState;
        private Dictionary<MouseButton, ICommand> mouseButtonDownBinds = new();
        private MouseState previousMouseState;

        public void AddUpdateCommand(Keys inputKey, ICommand command)
        {
            keybindsUpdate.Add(inputKey, command);
        }
        public void AddButtonDownCommand(Keys inputKey, ICommand command)
        {
            keybindsButtonDown.Add(inputKey, command);
        }

        public void AddButtonUpCommand(Keys inputKey, ICommand command)
        {
            keybindsButtonUp.Add(inputKey, command);
        }

        public void AddMouseButtonDownCommand(MouseButton button, ICommand command)
        {
            mouseButtonDownBinds[button] = command;
        }

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
