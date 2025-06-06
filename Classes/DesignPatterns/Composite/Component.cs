using Microsoft.Xna.Framework.Graphics;

namespace Grief.Classes.DesignPatterns.Composite
{
    /// <summary>
    /// Abstract hovedklasse for alle komponenter
    /// </summary>
    /// <author>Asbjørn Ryberg</author>
    /// <author>David Gudmund Danielsen</author>
    public abstract class Component
    {
        //Properties for component
        public GameObject GameObject { get; private set; }

        /// <summary>
        /// Components constructor
        /// </summary>
        /// <param name="gameObject"></param>
        public Component(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }

        //Virtuelle metoder som skal bruges af components children
        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void LateStart() { }
        public virtual void Update() { }
        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}
