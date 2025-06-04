using Greif;
using Grief.Classes.GameManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Grief.Classes.GameManager.Scenes
{
    public class SaveGame : Scene
    {
        private Texture2D saveSlotButton;
        private Texture2D backButton;
        private float buttonScale = 0.1f;

        // Positions
        private Vector2 saveSlotPos;
        private Vector2 backButtonPos;

        // Rectangles
        private Rectangle saveSlotRect;
        private Rectangle backRect;

        // Mouse state
        private MouseState currentMouse;
        private MouseState previousMouse;
        private Point mousePosition;

        public override void LoadContent()
        {
            saveSlotButton = GameWorld.Instance.Content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/SG");
            backButton = GameWorld.Instance.Content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/Exit");

            // Sæt positions – midt på skærmen med lidt forskydning
            var viewport = GameWorld.Instance.GraphicsDevice.Viewport;
            var screenCenter = new Vector2(viewport.Width / 2, viewport.Height / 2);
            var worldCenter = GameWorld.Instance.Camera.ScreenToWorld(screenCenter);

            saveSlotPos = worldCenter + new Vector2(-saveSlotButton.Width * buttonScale / 2f, -30);
            backButtonPos = worldCenter + new Vector2(-backButton.Width * buttonScale / 2f, 30);

            saveSlotRect = new Rectangle(
                (int)saveSlotPos.X,
                (int)saveSlotPos.Y,
                (int)(saveSlotButton.Width * buttonScale),
                (int)(saveSlotButton.Height * buttonScale)
            );

            backRect = new Rectangle(
                (int)backButtonPos.X,
                (int)backButtonPos.Y,
                (int)(backButton.Width * buttonScale),
                (int)(backButton.Height * buttonScale)
            );
        }

        public override void Update(GameTime gameTime)
        {
            currentMouse = Mouse.GetState();
            Vector2 worldMousePos = GameWorld.Instance.Camera.ScreenToWorld(currentMouse.Position.ToVector2());
            mousePosition = worldMousePos.ToPoint();

            if (IsClicked(saveSlotRect, mousePosition))
            {
                Debug.WriteLine("Save slot clicked! (Gem her)");
                // Kald din gemmefunktion her
                // Eksempel: GameWorld.Instance.GameManager.SaveGame();
                // Gå evt. tilbage til pause menu eller main menu
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.Paused);
            }

            if (IsClicked(backRect, mousePosition))
            {
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.Paused);
            }

            previousMouse = currentMouse;
        }

        private bool IsHovering(Rectangle rect, Point mousePosition)
        {
            return rect.Contains(mousePosition);
        }

        private bool IsClicked(Rectangle rect, Point mousePosition)
        {
            return rect.Contains(mousePosition)
                && currentMouse.LeftButton == ButtonState.Pressed
                && previousMouse.LeftButton == ButtonState.Released;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Fade overlay baggrund (kan fjernes hvis ikke nødvendigt)
            var viewport = GameWorld.Instance.GraphicsDevice.Viewport;
            Vector2 topLeft = GameWorld.Instance.Camera.ScreenToWorld(Vector2.Zero);
            Vector2 bottomRight = GameWorld.Instance.Camera.ScreenToWorld(new Vector2(viewport.Width, viewport.Height));
            Rectangle fadeRect = new Rectangle(
                (int)topLeft.X,
                (int)topLeft.Y,
                (int)bottomRight.X,
                (int)bottomRight.Y
            );
            spriteBatch.Draw(GameWorld.Instance.Pixel, fadeRect, Color.Black * 0.5f);

            // Knapper
            spriteBatch.Draw(saveSlotButton, saveSlotPos, null, IsHovering(saveSlotRect, mousePosition) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(backButton, backButtonPos, null, IsHovering(backRect, mousePosition) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);

            // Debug visualisering (fjern hvis unødvendigt)
            spriteBatch.Draw(GameWorld.Instance.Pixel, saveSlotRect, Color.Blue * 0.3f);
            spriteBatch.Draw(GameWorld.Instance.Pixel, backRect, Color.Red * 0.3f);
        }
    }
}
