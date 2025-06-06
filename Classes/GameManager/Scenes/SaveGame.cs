using Greif;
using Grief.Classes.GameManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Grief.Classes.GameManager.Scenes
{
    /// <summary>
    /// SaveGame Scene
    /// </summary>
    /// <author>Asbjørn Ryberg</author>
    public class SaveGame : Scene
    {
        //Fields
        private Texture2D slot1Button;
        private Texture2D slot2Button;
        private Texture2D slot3Button;
        private Texture2D backButton;
        private float buttonScale = 0.1f;

        private Vector2 slot1Pos;
        private Vector2 slot2Pos;
        private Vector2 slot3Pos;
        private Vector2 backButtonPos;

        private Rectangle slot1Rect;
        private Rectangle slot2Rect;
        private Rectangle slot3Rect;
        private Rectangle backRect;

        private MouseState currentMouse;
        private MouseState previousMouse;
        private Point mousePosition;

        private Viewport viewport;
        private Vector2 screenCenter;
        private Vector2 worldCenter;

        /// <summary>
        /// Loader textures
        /// </summary>
        public override void LoadContent()
        {
            var content = GameWorld.Instance.Content;
            slot1Button = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/S1");
            slot2Button = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/S2");
            slot3Button = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/S3");
            backButton = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/Exit");
        }

        /// <summary>
        /// Sætter position for knapperne, og tjekker om knapperne bliver trykket på
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // Opdatér positions så de er centreret på skærmen uanset vinduesstørrelse
            viewport = GameWorld.Instance.GraphicsDevice.Viewport;
            screenCenter = new Vector2(viewport.Width / 2, viewport.Height / 2);
            worldCenter = GameWorld.Instance.Camera.ScreenToWorld(screenCenter);

            slot1Pos = worldCenter + new Vector2(-slot1Button.Width * buttonScale / 2f, -40);
            slot2Pos = worldCenter + new Vector2(-slot2Button.Width * buttonScale / 2f, 0);
            slot3Pos = worldCenter + new Vector2(-slot3Button.Width * buttonScale / 2f, 40);
            backButtonPos = worldCenter + new Vector2(-backButton.Width * buttonScale / 2f, 80);

            slot1Rect = new Rectangle
                (
                    (int)slot1Pos.X,
                    (int)slot1Pos.Y,
                    (int)(slot1Button.Width * buttonScale),
                    (int)(slot1Button.Height * buttonScale)
                );
            
            slot2Rect = new Rectangle
                (
                    (int)slot2Pos.X,
                    (int)slot2Pos.Y,
                    (int)(slot2Button.Width * buttonScale),
                    (int)(slot2Button.Height * buttonScale)
                );
            
            slot3Rect = new Rectangle
                (
                    (int)slot3Pos.X,
                    (int)slot3Pos.Y,
                    (int)(slot3Button.Width * buttonScale),
                    (int)(slot3Button.Height * buttonScale)
                );
            
            backRect = new Rectangle
                (
                    (int)backButtonPos.X,
                    (int)backButtonPos.Y,
                    (int)(backButton.Width * buttonScale),
                    (int)(backButton.Height * buttonScale)
                );

            currentMouse = Mouse.GetState();
            Vector2 worldMousePos = GameWorld.Instance.Camera.ScreenToWorld(currentMouse.Position.ToVector2());
            mousePosition = worldMousePos.ToPoint();

            if (IsClicked(slot1Rect, mousePosition))
            {
                Debug.WriteLine("Save slot 1 clicked!");
                // Her indsætter du din save-funktion fx GameManager.SaveGame(1);
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.Paused);
            }
            if (IsClicked(slot2Rect, mousePosition))
            {
                Debug.WriteLine("Save slot 2 clicked!");
                // Her indsætter du din save-funktion fx GameManager.SaveGame(2);
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.Paused);
            }
            if (IsClicked(slot3Rect, mousePosition))
            {
                Debug.WriteLine("Save slot 3 clicked!");
                // Her indsætter du din save-funktion fx GameManager.SaveGame(3);
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.Paused);
            }
            if (IsClicked(backRect, mousePosition))
            {
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.Paused);
            }

            previousMouse = currentMouse;
        }

        /// <summary>
        /// Tjekker om man hover knapperne
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        private bool IsHovering(Rectangle rect, Point mousePosition)
        {
            return rect.Contains(mousePosition);
        }

        /// <summary>
        /// Tjekker om man klikker på knapperne
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
        /// Tegner SaveGame scenen
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Tegn fade overlay ligesom PauseOverlay
            Vector2 topLeft = GameWorld.Instance.Camera.ScreenToWorld(Vector2.Zero);
            Vector2 bottomRight = GameWorld.Instance.Camera.ScreenToWorld(new Vector2(viewport.Width, viewport.Height));
            Rectangle fadeRect = new Rectangle(
                (int)topLeft.X,
                (int)topLeft.Y,
                (int)bottomRight.X,
                (int)bottomRight.Y
            );
            spriteBatch.Draw(GameWorld.Instance.Pixel, fadeRect, Color.Black * 0.5f);

            // Tegn knapper centreret og med hover-effekt
            spriteBatch.Draw(slot1Button, slot1Pos, null, IsHovering(slot1Rect, mousePosition) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(slot2Button, slot2Pos, null, IsHovering(slot2Rect, mousePosition) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(slot3Button, slot3Pos, null, IsHovering(slot3Rect, mousePosition) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(backButton, backButtonPos, null, IsHovering(backRect, mousePosition) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);

            // Debug rectangles (kan fjernes)
            //spriteBatch.Draw(GameWorld.Instance.Pixel, slot1Rect, Color.Blue * 0.3f);
            //spriteBatch.Draw(GameWorld.Instance.Pixel, slot2Rect, Color.Blue * 0.3f);
            //spriteBatch.Draw(GameWorld.Instance.Pixel, slot3Rect, Color.Blue * 0.3f);
            //spriteBatch.Draw(GameWorld.Instance.Pixel, backRect, Color.Red * 0.3f);
        }
    }
}
