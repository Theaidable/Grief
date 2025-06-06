using Greif;
using Microsoft.Xna.Framework.Graphics;

namespace Grief.Classes.Items.Items
{
    /// <summary>
    /// Quest item
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public class QuestItem : Item
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemName"></param>
        public QuestItem(string itemName, string type)
        {
            DisplayName = itemName;
            Type = type;
            //Texture = $"Items/{type}/{itemName}";
        }
    }
}
