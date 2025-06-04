using Greif;
using Grief.Classes.GameManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Grief.Classes.GameManager.Scenes
{
    public class MainMenu : Scene
    {
        //Fields

        //Textures + en scale
        private Texture2D background;
        private Texture2D title;
        private Texture2D startButton;
        private Texture2D loadButton;
        private Texture2D exitButton;
        private float buttonScale = 0.1f;

        //World coordinates
        private Vector2 startButtonPos = new Vector2(-40, -30);
        private Vector2 loadButtonPos = new Vector2(-40, 0);
        private Vector2 exitButtonPos = new Vector2(-40, 30);

        //Box Rectangles
        private Rectangle startRect;
        private Rectangle loadRect;
        private Rectangle exitRect;

        //MousePosition
        private MouseState currentMouse;
        private MouseState previousMouse;
        private Point mousePosition;

        /// <summary>
        /// Load Textures og knappers placering
        /// </summary>
        public override void LoadContent()
        {
            var content = GameWorld.Instance.Content;
            
            background = content.Load<Texture2D>("TileMaps/Assets/UI/MenuBackground/MenuBG");
            title = content.Load<Texture2D>("TileMaps/Assets/UI/Text/Title");

            startButton = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/NG");
            loadButton = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/LG");
            exitButton = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/Exit");

            startRect = new Rectangle
                (
                    (int)startButtonPos.X,
                    (int)startButtonPos.Y,
                    (int)(startButton.Width * buttonScale),
                    (int)(startButton.Height * buttonScale)
                );

            loadRect = new Rectangle
                (
                    (int)loadButtonPos.X,
                    (int)loadButtonPos.Y,
                    (int)(loadButton.Width * buttonScale),
                    (int)(loadButton.Height * buttonScale)
                );

            exitRect = new Rectangle
                (
                    (int)exitButtonPos.X,
                    (int)exitButtonPos.Y,
                    (int)(exitButton.Width * buttonScale),
                    (int)(exitButton.Height * buttonScale)
                );
        }

        /// <summary>
        /// Update af musens placering
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            currentMouse = Mouse.GetState();
            Vector2 worldMousePos = GameWorld.Instance.Camera.ScreenToWorld(currentMouse.Position.ToVector2());
            mousePosition = worldMousePos.ToPoint();

            if (IsClicked(startRect, mousePosition))
            {
                GameWorld.Instance.GameManager.LevelManager.LoadLevel("GriefMap1");
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.Level);
            }

            if (IsClicked(loadRect, mousePosition))
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.LoadGame);

            if (IsClicked(exitRect, mousePosition))
            {
                Debug.WriteLine("Exit button clicked!");
                GameWorld.Instance.Exit();
            }

            previousMouse = currentMouse;
        }

        /// <summary>
        /// Tjek om musen hover en knap
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        private bool IsHovering(Rectangle rect, Point mousePosition)
        {
            return rect.Contains(mousePosition);
        }
        
        /// <summary>
        /// Tjekker om man klikker på knappen
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        private bool IsClicked(Rectangle rect, Point mousePosition)
        {
            return rect.Contains(mousePosition) 
                && currentMouse.LeftButton == ButtonState.Pressed
                && previousMouse.LeftButton == ButtonState.Released;
        }

        /// <summary>
        /// Tegner main menuen
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Draw Background
            spriteBatch.Draw(background, new Rectangle(-320, -135, 576, 324), Color.White);

            //Draw Title
            spriteBatch.Draw(title, new Vector2(-118, -100), null, Color.White, 0f, Vector2.Zero, 0.3f, SpriteEffects.None, 0f);

            //Draw buttons at set positions and scale
            spriteBatch.Draw(startButton, startButtonPos, null, IsHovering(startRect, mousePosition) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(loadButton, loadButtonPos, null, IsHovering(loadRect, mousePosition) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(exitButton, exitButtonPos, null, IsHovering(exitRect, mousePosition) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);

            //Draw Color
            spriteBatch.Draw(GameWorld.Instance.Pixel, startRect, Color.Green * 0.3f);
            spriteBatch.Draw(GameWorld.Instance.Pixel, loadRect, Color.Blue * 0.3f);
            spriteBatch.Draw(GameWorld.Instance.Pixel, exitRect, Color.Red * 0.3f);
        }
    }
}
