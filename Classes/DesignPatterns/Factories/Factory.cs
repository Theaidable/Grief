using Grief.Classes.DesignPatterns.Composite;
using Microsoft.Xna.Framework;

namespace Grief.Classes.DesignPatterns.Factories
{
    /// <summary>
    /// Factory superklasse
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public abstract class Factory
    {
        //Metode som alle factories skal bruge
        public abstract GameObject Create(Vector2 position);
    }
}
