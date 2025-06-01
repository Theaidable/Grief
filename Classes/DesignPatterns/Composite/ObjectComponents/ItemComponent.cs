using Greif;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.Items;
using System.Linq;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    /// <summary>
    /// Item component
    /// </summary>
    public class ItemComponent : Component
    {
        //Properties
        public Item Item { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameObject"></param>
        public ItemComponent(GameObject gameObject) : base(gameObject) { }

        /// <summary>
        /// Tjekker om player og items collisionboxes rammer hinaden
        /// Hvis ja, så tilføjes itemmet til spillerens inventory
        /// </summary>
        public void PickUpItem()
        {
            var player = GameWorld.Instance.LevelManager.CurrentLevel.GameObjects.FirstOrDefault(gameObject => gameObject.Tag == "Player");

            if (player != null)
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
