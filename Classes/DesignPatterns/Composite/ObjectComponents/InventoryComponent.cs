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
    /// <summary>
    /// Inventory component
    /// </summary>
    public class InventoryComponent : Component
    {
        //Fields
        private Texture2D backgroundTexture;
        private Rectangle backgroundSource;
        private Vector2 backgroundPosition;

        private List<Item> items = new List<Item>();

        //Properties
        public bool ShowInventory { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameObject"></param>
        public InventoryComponent(GameObject gameObject) : base(gameObject) { }

        /// <summary>
        /// Sæt inventorys baggrund
        /// </summary>
        public override void Start()
        {
            backgroundTexture = GameWorld.Instance.Content.Load<Texture2D>("TileMaps/Assets/UI/SettingsMenu");
            backgroundSource = new Rectangle(125, 0, 120, 140);
        }

        /// <summary>
        /// Opdatere inventory position
        /// </summary>
        public override void Update()
        {
            var viewport = GameWorld.Instance.GraphicsDevice.Viewport;
            int screenWidth = viewport.Width;
            int screenHeight = viewport.Height;

            Vector2 screenCenter = new Vector2(screenWidth / 2, screenHeight / 2);
            Vector2 worldCenter = GameWorld.Instance.Camera.ScreenToWorld(screenCenter);

            backgroundPosition = new Vector2(worldCenter.X - 225, worldCenter.Y - 100);
        }

        /// <summary>
        /// Metode til at tilføje et item til inventory
        /// </summary>
        /// <param name="item"></param>
        public void AddItemToInventory(Item item)
        {
            items.Add(item);
        }

        /// <summary>
        /// Metode til at finde et item i inventory
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasItemByName(string name)
        {
            return items.Any(i => i.DisplayName == name);
        }

        /// <summary>
        /// Metode til at fjerne et item fra inventory
        /// </summary>
        /// <param name="name"></param>
        public void RemoveItemByName(string name)
        {
            var item = items.FirstOrDefault(i => i.DisplayName == name);

            if(item != null)
            {
                items.Remove(item);
            }
        }

        /// <summary>
        /// Tegn inventory
        /// </summary> 
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Desired størrelse af baggrunden for Inventory
            float backgroundDesiredWidth = backgroundSource.Width * 3.5f;
            float backgroundDesiredHeight = backgroundSource.Height * 1.5f;
            Rectangle backgroundDestRect = new Rectangle((int)backgroundPosition.X, (int)backgroundPosition.Y, (int)backgroundDesiredWidth, (int)backgroundDesiredHeight);

            if (ShowInventory == true)
            {
                spriteBatch.Draw(backgroundTexture, backgroundDestRect, backgroundSource, Color.White);

                //Skriv itemets navn i inventory
                for (int i = 0; i < items.Count; i++)
                {
                    string name = items[i].DisplayName;
                    spriteBatch.DrawString(GameWorld.Instance.DefaultFont, name, new Vector2(backgroundPosition.X + 75, backgroundPosition.Y + 35 + i * 20), Color.White);
                }
            }
        }

        /// <summary>
        /// Metode til at toggle inventory
        /// </summary>
        public void ToggleInventory()
        {
            ShowInventory = !ShowInventory;
        }
    }
}
