using Grief.Classes.DesignPatterns.Composite.ObjectComponents;

namespace Grief.Classes.Items
{
    /// <summary>
    /// Abstrakt hovedklasse for Items
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public abstract class Item
    {
        public string Texture { get; protected set; }
        public string DisplayName { get; protected set; }
        public string Type { get; protected set; }

        public virtual void Use(PlayerComponent player) { }

    }
}
