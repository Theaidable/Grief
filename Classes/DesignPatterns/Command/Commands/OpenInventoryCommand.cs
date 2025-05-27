using Greif;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using System.Linq;

namespace Grief.Classes.DesignPatterns.Command.Commands
{
    public class OpenInventoryCommand : ICommand
    {
        private readonly InventoryComponent inventory;

        public OpenInventoryCommand(InventoryComponent inventory)
        {
            this.inventory = inventory;
        }

        public void Execute()
        {
            if (inventory != null)
            {
                inventory.ToggleInventory();
            }
        }
    }
}
