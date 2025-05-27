using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Microsoft.Xna.Framework.Graphics;

namespace Grief.Classes.Items
{
    public abstract class Item
    {
        public string Texture { get; protected set; }
        public string DisplayName { get; protected set; }

        public virtual void Use(PlayerComponent player) { }

    }
}
