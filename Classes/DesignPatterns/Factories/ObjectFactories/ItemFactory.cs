using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Grief.Classes.DesignPatterns.Factories.ObjectFactories.Enemy;
using Grief.Classes.Items;
using Microsoft.Xna.Framework;

namespace Grief.Classes.DesignPatterns.Factories.ObjectFactories
{
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

        public override GameObject Create(Vector2 position)
        {
            return Create(null, position);
        }

        public GameObject Create(Item item, Vector2 position)
        {
            GameObject itemObject = new GameObject();
            SpriteRenderer spriteRenderer = itemObject.AddComponent<SpriteRenderer>();
            Animator animator = itemObject.AddComponent<Animator>();
            Collider collider = itemObject.AddComponent<Collider>();
            var itemComp = itemObject.AddComponent<ItemComponent>();

            itemComp.Item = item;
            itemObject.Transform.Position = position;
            spriteRenderer.SetSprite(item.Texture);

            return itemObject;
        }
    }
}
