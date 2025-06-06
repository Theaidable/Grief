using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Grief.Classes.Items;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Grief.Classes.DesignPatterns.Factories.ObjectFactories
{
    /// <summary>
    /// Factory til oprettelse af items
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public class ItemFactory : Factory
    {

        //Oprettelse af Singleton af EnemyFactory
        private static ItemFactory instance;
        public static ItemFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ItemFactory();
                }
                return instance;
            }
        }

        private ItemFactory() { }

        /// <summary>
        /// Standard oprettelse af items
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override GameObject Create(Vector2 position)
        {
            return Create(null, position);
        }

        /// <summary>
        /// Oprettelse af et bestemt item
        /// </summary>
        /// <param name="item"></param> Hvilket item
        /// <param name="position"></param> Dens position
        /// <returns></returns>
        public GameObject Create(Item item, Vector2 position)
        {
            GameObject itemObject = new GameObject();
            SpriteRenderer spriteRenderer = itemObject.AddComponent<SpriteRenderer>();
            Animator animator = itemObject.AddComponent<Animator>();
            Collider collider = itemObject.AddComponent<Collider>();
            ItemComponent itemComponent = itemObject.AddComponent<ItemComponent>();

            Debug.WriteLine(itemComponent != null, "ItemComponent blev tilføjet korrekt");

            itemComponent.Item = item;
            itemObject.Transform.Position = position;
            spriteRenderer.SetSprite(item.Texture);

            return itemObject;
        }
    }
}
