using Greif;
using Grief.Classes.GameManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Grief.Classes.GameManager.Scenes
{
    /// <summary>
    /// LoadGame Scene
    /// </summary>
    /// <author>Asbjørn Ryberg</author>
    public class LoadGame : Scene
    {
        private Texture2D background;
        private Texture2D slot1Button;
        private Texture2D slot2Button;
        private Texture2D slot3Button;
        private Texture2D backButton;
        private float buttonScale = 0.1f; // Brug 1.0f hvis dine billeder allerede er i den ønskede størrelse

        private Vector2 slot1Pos = new Vector2(-40, -60);
        private Vector2 slot2Pos = new Vector2(-40, -20);
        private Vector2 slot3Pos = new Vector2(-40, 20);
        private Vector2 backButtonPos = new Vector2(-40, 60);

        private Rectangle slot1Rect;
        private Rectangle slot2Rect;
        private Rectangle slot3Rect;
        private Rectangle backRect;

        private MouseState currentMouse;
        private MouseState previousMouse;
        private Point mousePosition;

        public override void LoadContent()
        {
            var content = GameWorld.Instance.Content;

            background = content.Load<Texture2D>("TileMaps/Assets/UI/MenuBackground/MenuBG");
            slot1Button = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/S1");
            slot2Button = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/S2");
            slot3Button = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/S3");
            backButton = content.Load<Texture2D>("TileMaps/Assets/UI/Buttons/Exit");

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
                    (int)(backButton.Width * 0.1f),
                    (int)(backButton.Height * 0.1f)
                );
        }

        public override void Update(GameTime gameTime)
        {
            currentMouse = Mouse.GetState();
            Vector2 worldMousePos = GameWorld.Instance.Camera.ScreenToWorld(currentMouse.Position.ToVector2());
            mousePosition = worldMousePos.ToPoint();

            if (IsClicked(slot1Rect, mousePosition))
            {
                Debug.WriteLine("Load slot 1 clicked!");
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.Level);
            }

            if (IsClicked(slot2Rect, mousePosition))
            {
                Debug.WriteLine("Load slot 2 clicked!");
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.Level);
            }

            if (IsClicked(slot3Rect, mousePosition))
            {
                Debug.WriteLine("Load slot 3 clicked!");
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.Level);
            }

            if (IsClicked(backRect, mousePosition))
            {
                GameWorld.Instance.GameManager.ChangeState(GameManager.GameState.MainMenu);
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
            //Draw background
            spriteBatch.Draw(background, new Rectangle(-320, -135, 576, 324), Color.White);

            //Draw each slot button
            spriteBatch.Draw(slot1Button, slot1Pos, null, IsHovering(slot1Rect, mousePosition) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(slot2Button, slot2Pos, null, IsHovering(slot2Rect, mousePosition) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(slot3Button, slot3Pos, null, IsHovering(slot3Rect, mousePosition) ? Color.LightGray : Color.White, 0f, Vector2.Zero, buttonScale, SpriteEffects.None, 0f);

            //Draw back button
            spriteBatch.Draw(backButton, backButtonPos, null, IsHovering(backRect, mousePosition) ? Color.LightGray : Color.White, 0f, Vector2.Zero, 0.1f, SpriteEffects.None, 0f);
        }
    }
}
