using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Grief.Classes.Items;

namespace Grief.Classes.Quests
{
    public class FetchQuest : Quest
    {
        public string RequiredItemName { get; set; }
        public Item RewardItem { get; set; }

        public FetchQuest(string title, string description, string requiredItem, Item reward = null)
        {
            Title = title;
            Description = description;
            RequiredItemName = requiredItem;
            RewardItem = reward;
        }

        public override bool CanComplete(InventoryComponent inventory)
        {
            return inventory.HasItemByName(RequiredItemName);
        }

        public override void GrantReward(InventoryComponent inventory)
        {
            inventory.RemoveItemByName(RequiredItemName);

            if(RewardItem != null)
            {
                inventory.AddItemToInventory(RewardItem);
            }
        }
    }
}
