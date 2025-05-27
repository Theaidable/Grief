using Greif;
using Microsoft.Xna.Framework.Graphics;

namespace Grief.Classes.Items.Items
{
    public class QuestItem : Item
    {
        public QuestItem(string itemName)
        {
            Texture = $"Items/QuestItems/{itemName}";
            DisplayName = itemName;
        }
    }
}
