using Grief.Classes.DesignPatterns.Composite.ObjectComponents;

namespace Grief.Classes.DesignPatterns.Command.Commands
{
    /// <summary>
    /// OpenIventoryCommand
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public class OpenInventoryCommand : ICommand
    {
        //Reference til inventory
        private readonly InventoryComponent inventory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inventory"></param>
        public OpenInventoryCommand(InventoryComponent inventory)
        {
            this.inventory = inventory;
        }

        /// <summary>
        /// Metode til at eksekverer commanden - gøre brug af en metode fra inventory
        /// </summary>
        public void Execute()
        {
            if (inventory != null)
            {
                inventory.ToggleInventory();
            }
        }
    }
}
