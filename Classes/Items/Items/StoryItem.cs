using Greif;
using Microsoft.Xna.Framework.Graphics;

namespace Grief.Classes.Items.Items
{
    /// <summary>
    /// Story item
    /// </summary>
    public class StoryItem : Item
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemName"></param>
        public StoryItem(string itemName, string type)
        {
            DisplayName = itemName;
            Type = type;
            Texture = $"Items/{type}/{itemName}";
        }
    }
}