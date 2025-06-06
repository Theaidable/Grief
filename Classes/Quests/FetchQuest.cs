using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Grief.Classes.Items;

namespace Grief.Classes.Quests
{
    /// <summary>
    /// Fetchquest
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public class FetchQuest : Quest
    {
        //Properties
        public string RequiredItemName { get; set; }
        public Item RewardItem { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="title"></param> Questens titel
        /// <param name="description"></param> Questens beskrivelse
        /// <param name="requiredItem"></param> Hvilket item skal findes
        /// <param name="reward"></param> Gevinsten for at fuldføre questen
        public FetchQuest(string title, string description, string requiredItem, Item reward = null)
        {
            Title = title;
            Description = description;
            RequiredItemName = requiredItem;
            RewardItem = reward;
        }

        /// <summary>
        /// Hvad kræves for at complete questen
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        public override bool CanComplete(InventoryComponent inventory)
        {
            return inventory.HasItemByName(RequiredItemName);
        }

        /// <summary>
        /// Fjern item som skal bruges og giv reward
        /// </summary>
        /// <param name="inventory"></param>
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
