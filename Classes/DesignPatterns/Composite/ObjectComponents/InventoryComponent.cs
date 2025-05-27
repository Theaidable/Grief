using Greif;
using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    public class InventoryComponent : Component
    {
        private Texture2D backgroundTexture;
        private Rectangle backgroundSource;
        private Vector2 backgroundPosition;

        private List<Item> items = new List<Item>();

        public bool ShowInventory { get; private set; }

        public InventoryComponent(GameObject gameObject) : base(gameObject) { }

        public override void Start()
        {
            backgroundTexture = GameWorld.Instance.Content.Load<Texture2D>("UI/SettingsMenu");
            backgroundSource = new Rectangle(125, 0, 120, 140);
        }

        public override void Update()
        {
            var player = GameWorld.Instance.LevelManager.CurrentLevel.GameObjects.FirstOrDefault(gameObject => gameObject.Tag == "Player");
            var playerX = player.Transform.Position.X;
            var playerY = player.Transform.Position.Y;

            backgroundPosition = new Vector2(playerX - 100, playerY - 175);
        }

        public void AddItemToInventory(Item item)
        {
            items.Add(item);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Desired størrelse af baggrunden for Inventory
            float backgroundDesiredWidth = backgroundSource.Width * 2f;
            float backgroundDesiredHeight = backgroundSource.Height * 1f;
            Rectangle backgroundDestRect = new Rectangle((int)backgroundPosition.X, (int)backgroundPosition.Y, (int)backgroundDesiredWidth, (int)backgroundDesiredHeight);

            if (ShowInventory == true)
            {
                spriteBatch.Draw(backgroundTexture, backgroundDestRect, backgroundSource, Color.White);

                for (int i = 0; i < items.Count; i++)
                {
                    string name = items[i].DisplayName;
                    spriteBatch.DrawString(GameWorld.Instance.DefaultFont, name, new Vector2(backgroundPosition.X + 40, backgroundPosition.Y + 20 + i * 20), Color.White);
                }
            }
        }

        public void ToggleInventory()
        {
            Debug.WriteLine("ToggleInventory");
            Debug.WriteLine($"Inventory position: {backgroundPosition}");
            ShowInventory = !ShowInventory;
        }
    }
}
