using Greif;
using Microsoft.Xna.Framework.Graphics;

namespace Grief.Classes.Items.Items
{
    public class StoryItem : Item
    {
        public StoryItem(string itemName)
        {
            Texture = $"Items/StoryItems/{itemName}";
            DisplayName = itemName;
        }
    }
}