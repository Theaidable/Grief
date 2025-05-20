using Microsoft.Xna.Framework;

namespace Grief.Classes.DesignPatterns.Composite.Components
{
    public class Transform : Component
    {
        /// <summary>
        /// Properties til at tilgå værdier for transformer
        /// </summary>
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public float Rotation { get; set; }

        public Transform(GameObject gameObject) : base(gameObject) { }

        /// <summary>
        /// Metode til at translate position for objektet
        /// </summary>
        /// <param name="translation"></param>
        public void Translate(Vector2 translation)
        {
            Position += translation;
        }
    }
}
