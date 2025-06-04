using Greif;
using Grief.Classes.GameManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Grief.Classes.GameManager.Scenes
{
    public class PauseOverlay : Scene
    {
        // Textures + scale
        private Texture2D background;
        private Texture2D continueButton;
        private Texture2D saveButton;
        private Texture2D quitButton;
        private float buttonScale = 0.1f;

        // Button positions
        private Vector2 continueButtonPos = new Vector2(0, -30);
        private Vector2 saveButtonPos = new Vector2(0, 0);
        private Vector2 quitButtonPos = new Vector2(0, 30);

        // Box Rectangles
        private Rectangle continueRect;
        private Rectangle saveRect;
        private Rectangle quitRect;

        // Mouse state
        private MouseState currentMouse;
        private MouseState previousMouse;
        private Point mousePosition;

        // Keyboard state
        private KeyboardState previousKeyState;


        public override void LoadContent()
        {
            var content = GameWorld.Instance.Content;

            //background = content.Load<Texture2D>("TileMaps/Assets/UI/MenuBackground/MenuBG");
            continueButton = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/Continue");
            saveButton = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/SG");
            quitButton = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/Exit");

            continueRect = new Rectangle(
                (int)continueButtonPos.X,
                (int)continueButtonPos.Y,
                (int)(continueButton.Width * buttonScale),
                (int)(continueButton.Height * buttonScale)
            );
            saveRect = new Rectangle(
                (int)saveButtonPos.X,
                (int)saveButtonPos.Y,
                (int)(saveButton.Width * buttonScale),
                (int)(saveButton.Height * buttonScale)
            );
            quitRect = new Rectangle(
                (int)quitButtonPos.X,
                (int)quitButtonPos.Y,
                (int)(quitButton.Width * buttonScale),
                (int)(quitButton.Height * buttonScale)
            );

            if (continueButton == null)
                Debug.WriteLine("continueButton texture is NULL!");
            if (saveButton == null)
                Debug.WriteLine("saveButton texture is NULL!");
            if (quitButton == null)
                Debug.WriteLine("quitButton texture is NULL!");

        }

        public override void Update(GameTime gameTime)
        {
            currentMouse = Mouse.GetState();
            KeyboardState keyState = Keyboard.GetState();
            Vector2 worldMousePos = GameWorld.Instance.Camera.ScreenToWorld(currentMouse.Position.ToVector2());
            mousePosition = worldMousePos.ToPoint();

            if (IsClicked(continueRect, mousePosition) ||
                (keyState.IsKeyDown(Keys.Escape) && previousKeyState.IsKeyUp(Keys.Escape)))
            {
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.Level);
            }
            if (IsClicked(saveRect, mousePosition))
            {
                Debug.WriteLine("Save clicked! (Not implemented)");
            }
            if (IsClicked(quitRect, mousePosition))
            {
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.MainMenu);
            }

            previousMouse = currentMouse;
            previousKeyState = keyState;
        }


        // Hover-effekt
        private bool IsHovering(Rectangle rect, Point mousePosition)
        {
            return rect.Contains(mousePosition);
        }

        // Klik tjek (ligesom MainMenu)
        private bool IsClicked(Rectangle rect, Point mousePosition)
        {
            return rect.Contains(mousePosition)
                && currentMouse.LeftButton == ButtonState.Pressed
                && previousMouse.LeftButton == ButtonState.Released;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Tegn mørk overlay for at "fade" baggrunden
            spriteBatch.Draw(GameWorld.Instance.Pixel, new Rectangle(0, 0, 576, 324), Color.Black * 0.5f);

            // Tegn menu baggrund
            //spriteBatch.Draw(background, new Rectangle(-320, -135, 576, 324), Color.White);

            // Tegn knapper (med hover)
            spriteBatch.Draw(continueButton, continueButtonPos, null, IsHovering(continueRect, mousePosition) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(saveButton, saveButtonPos, null, IsHovering(saveRect, mousePosition) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(quitButton, quitButtonPos, null, IsHovering(quitRect, mousePosition) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);

            // Debug visualisering (kan fjernes)
            spriteBatch.Draw(GameWorld.Instance.Pixel, continueRect, Color.Green * 0.3f);
            spriteBatch.Draw(GameWorld.Instance.Pixel, saveRect, Color.Blue * 0.3f);
            spriteBatch.Draw(GameWorld.Instance.Pixel, quitRect, Color.Red * 0.3f);
        }
    }
}
