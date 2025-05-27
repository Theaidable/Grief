using Greif;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.Items;
using System.Linq;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    public class ItemComponent : Component
    {
        public Item Item { get; set; }

        public ItemComponent(GameObject gameObject) : base(gameObject) { }

        public override void Update()
        {
            var player = GameWorld.Instance.LevelManager.CurrentLevel.GameObjects.FirstOrDefault(gameObject => gameObject.Tag == "Player");

            if(player != null)
            {
                var playerCollider = player.GetComponent<Collider>().CollisionBox;
                var thisCollider = GameObject.GetComponent<Collider>().CollisionBox;

                if (playerCollider.Intersects(thisCollider))
                {
                    var inventory = player.GetComponent<InventoryComponent>();
                    inventory.AddItemToInventory(Item);

                    GameWorld.Instance.LevelManager.CurrentLevel.QueueRemove(GameObject);
                }
            }
        }
    }
}
