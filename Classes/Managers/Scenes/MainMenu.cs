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
        private Texture2D pixel; // For debug overlays

        private float buttonScale = 0.1f;

        // These are your "world"/menu coordinates - keep as you like
        private Vector2 startButtonPos = new Vector2(-40, -30);
        private Vector2 loadButtonPos = new Vector2(-40, 0);
        private Vector2 exitButtonPos = new Vector2(-40, 30);

        private Rectangle startRect;
        private Rectangle loadRect;
        private Rectangle exitRect;

        // THIS IS THE DRAW OFFSET YOU USE FOR YOUR MENU!
        private Point menuDrawOffset = new Point(320, 135); // Use +320,+135 to "undo" the -320,-135 in Draw()

        private MouseState currentMouse;
        private MouseState previousMouse;

        public override void Update(GameTime gameTime)
        {
            currentMouse = Mouse.GetState();

            // This is the magic! Translate screen mouse to menu space
            Point mouseMenu = currentMouse.Position - menuDrawOffset;

            if (IsClicked(startRect, mouseMenu))
            {
                GameWorld.Instance.GameManager.LevelManager.LoadLevel("GriefMap1");
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.Level);
            }

            if (IsClicked(loadRect, mouseMenu))
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.LoadGame);

            if (IsClicked(exitRect, mouseMenu))
            {
                System.Diagnostics.Debug.WriteLine("Exit button clicked!");
                GameWorld.Instance.Exit();
            }

            previousMouse = currentMouse;
        }

        private bool IsHovering(Rectangle rect, Point mouseMenu)
        {
            return rect.Contains(mouseMenu);
        }

        private bool IsClicked(Rectangle rect, Point mouseMenu)
        {
            return rect.Contains(mouseMenu) &&
                   currentMouse.LeftButton == ButtonState.Pressed &&
                   previousMouse.LeftButton == ButtonState.Released;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // You draw the background with an offset
            spriteBatch.Draw(background, new Rectangle(-320, -135, 576, 324), Color.White);

            float scale = 0.3f;
            spriteBatch.Draw(title, new Vector2(-118, -100), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // Draw buttons at set positions and scale
            spriteBatch.Draw(startButton, startButtonPos, null, IsHovering(startRect, currentMouse.Position - menuDrawOffset) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(loadButton, loadButtonPos, null, IsHovering(loadRect, currentMouse.Position - menuDrawOffset) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(exitButton, exitButtonPos, null, IsHovering(exitRect, currentMouse.Position - menuDrawOffset) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);

            // Debug overlays: show rectangles (remove/comment out for release)
            spriteBatch.Draw(pixel, startRect, Color.Green * 0.3f);
            spriteBatch.Draw(pixel, loadRect, Color.Blue * 0.3f);
            spriteBatch.Draw(pixel, exitRect, Color.Red * 0.3f);
        }

        public override void LoadContent()
        {
            var content = GameWorld.Instance.Content;

            background = content.Load<Texture2D>("TileMaps/Assets/UI/MenuBackground/MenuBG");
            title = content.Load<Texture2D>("TileMaps/Assets/UI/Text/Title");

            startButton = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/NG");
            loadButton = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/LG");
            exitButton = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/Exit");

            // For debug rectangle overlays
            pixel = new Texture2D(GameWorld.Instance.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            // Calculate hitboxes to match draw
            startRect = new Rectangle(
                (int)startButtonPos.X,
                (int)startButtonPos.Y,
                (int)(startButton.Width * buttonScale),
                (int)(startButton.Height * buttonScale)
            );

            loadRect = new Rectangle(
                (int)loadButtonPos.X,
                (int)loadButtonPos.Y,
                (int)(loadButton.Width * buttonScale),
                (int)(loadButton.Height * buttonScale)
            );

            exitRect = new Rectangle(
                (int)exitButtonPos.X,
                (int)exitButtonPos.Y,
                (int)(exitButton.Width * buttonScale),
                (int)(exitButton.Height * buttonScale)
            );
        }
    }
}
