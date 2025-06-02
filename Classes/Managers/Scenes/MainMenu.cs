using Greif;
using Grief.Classes.GameManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Grief.Classes.GameManager.Scenes
{
    public class MainMenu : Scene
    {
        private Texture2D background;
        private Texture2D title;

        private Texture2D startButton;
        private Texture2D loadButton;
        private Texture2D exitButton;

        private Rectangle startRect;
        private Rectangle loadRect;
        private Rectangle exitRect;

        private MouseState currentMouse;
        private MouseState previousMouse;

        public override void Update(GameTime gameTime)
        {
            currentMouse = Mouse.GetState();

            Point mousePoint = currentMouse.Position;

            if (IsClicked(startRect))
            {
                // This will clear old objects and reload everything fresh
                GameWorld.Instance.GameManager.LevelManager.LoadLevel("GriefMap1");
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.Level);
            }

            if (IsClicked(loadRect))
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.LoadGame);

            if (IsClicked(exitRect))
            {
                System.Diagnostics.Debug.WriteLine("Exit button clicked!");
                GameWorld.Instance.Exit();
            }

            previousMouse = currentMouse;
        }

        private bool IsHovering(Rectangle rect)
        {
            return rect.Contains(currentMouse.Position);
        }

        private bool IsClicked(Rectangle rect)
        {
            return rect.Contains(currentMouse.Position) &&
                   currentMouse.LeftButton == ButtonState.Pressed &&
                   previousMouse.LeftButton == ButtonState.Released;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Rectangle(-320, -135, 576, 324), Color.White);

            float scale = 0.3f;
            spriteBatch.Draw(title, new Vector2(-118, -100), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(startButton, startRect, Color.White);
            spriteBatch.Draw(loadButton, loadRect, Color.White);
            spriteBatch.Draw(exitButton, exitRect, Color.White);

            var pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            spriteBatch.Draw(pixel, exitRect, Color.Red * 0.5f); // semi-transparent red overlay
        }

        public override void LoadContent()
        {
            var content = GameWorld.Instance.Content;

            background = content.Load<Texture2D>("TileMaps/Assets/UI/MenuBackground/MenuBG");
            title = content.Load<Texture2D>("TileMaps/Assets/UI/Text/Title");

            startButton = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/NG");
            loadButton = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/LG");
            exitButton = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/Exit");

            int screenWidth = 1280;
            int screenHeight = 720;

            float buttonScale = 0.1f; // adjust if needed
            int buttonWidth = (int)(startButton.Width * buttonScale);
            int buttonHeight = (int)(startButton.Height * buttonScale);

            Vector2 buttonOffset = new Vector2(-640, -50); // X, Y offset to align to visual background


            startRect = new Rectangle(
                screenWidth / 2 - buttonWidth / 2 + (int)buttonOffset.X,
                20 + (int)buttonOffset.Y,
                buttonWidth,
                buttonHeight
            );

            loadRect = new Rectangle(
                screenWidth / 2 - buttonWidth / 2 + (int)buttonOffset.X,
                60 + (int)buttonOffset.Y,
                buttonWidth,
                buttonHeight
            );

            exitRect = new Rectangle(
                screenWidth / 2 - buttonWidth / 2 + (int)buttonOffset.X,
                100 + (int)buttonOffset.Y,
                buttonWidth,
                buttonHeight
            );
        }
       
    }
}
