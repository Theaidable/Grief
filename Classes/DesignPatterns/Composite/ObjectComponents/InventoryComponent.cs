using Greif;
using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    /// <summary>
    /// Inventory component
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public class InventoryComponent : Component
    {
        //Fields
        private Texture2D backgroundTexture;
        private Rectangle backgroundSource;
        private Vector2 backgroundPosition;

        //Properties
        public bool ShowInventory { get; private set; }
        public List<Item> Items = new List<Item>();

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
            var player = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.GameObjects.FirstOrDefault(gameObject => gameObject.Tag == "Player");
            var playerX = player.Transform.Position.X;
            var playerY = player.Transform.Position.Y;

            backgroundPosition = new Vector2(playerX - 100, playerY - 175);
        }

        /// <summary>
        /// Metode til at tilføje et item til inventory
        /// </summary>
        /// <param name="item"></param>
        public void AddItemToInventory(Item item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// Metode til at finde et item i inventory
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasItemByName(string name)
        {
            return Items.Any(i => i.DisplayName == name);
        }

        /// <summary>
        /// Metode til at fjerne et item fra inventory
        /// </summary>
        /// <param name="name"></param>
        public void RemoveItemByName(string name)
        {
            var item = Items.FirstOrDefault(i => i.DisplayName == name);

            if(item != null)
            {
                Items.Remove(item);
            }
        }

        /// <summary>
        /// Tegn inventory
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            //Desired størrelse af baggrunden for Inventory
            float backgroundDesiredWidth = backgroundSource.Width * 2f;
            float backgroundDesiredHeight = backgroundSource.Height * 1f;
            Rectangle backgroundDestRect = new Rectangle((int)backgroundPosition.X, (int)backgroundPosition.Y, (int)backgroundDesiredWidth, (int)backgroundDesiredHeight);

            if (ShowInventory == true)
            {
                spriteBatch.Draw(backgroundTexture, backgroundDestRect, backgroundSource, Color.White);

                //Skriv itemets navn i inventory
                for (int i = 0; i < Items.Count; i++)
                {
                    string name = Items[i].DisplayName;
                    spriteBatch.DrawString(GameWorld.Instance.DefaultFont, name, new Vector2(backgroundPosition.X + 40, backgroundPosition.Y + 20 + i * 20), Color.White);
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
