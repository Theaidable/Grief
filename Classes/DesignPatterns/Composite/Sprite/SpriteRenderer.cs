using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Grief.Classes.DesignPatterns.Composite.Sprite
{
    public class SpriteRenderer : Component
    {
        //Properties til at tilgå værdier som skal bruges for at sætte en sprite til et object
        public Vector2 Origin { get; set; }
        public Texture2D Sprite { get; set; }
        public Color Color { get; set; } = Color.White;
        public Rectangle? SourceRectangle { get; set; } = null;

        /// <summary>
        /// Tom Constructor, men nødvendig da den nedarver fra component
        /// </summary>
        /// <param name="gameObject"></param>
        public SpriteRenderer(GameObject gameObject) : base(gameObject) { }

        /// <summary>
        /// Metode til at sætte et objects sprite
        /// </summary>
        /// <param name="spriteName"></param>
        /// <param name="sourceRect"></param>
        public void SetSprite(string spriteName, Rectangle? sourceRect = null)
        {
            Sprite = GameWorld.Instance.Content.Load<Texture2D>(spriteName);
            SourceRectangle = sourceRect;
        }

        /// <summary>
        /// Sætter origin i midten af objektets sprite i stedet for venstre hjørne
        /// </summary>
        public override void Start()
        {
            if (SourceRectangle.HasValue)
            {
                Origin = new Vector2(SourceRectangle.Value.Width / 2f, SourceRectangle.Value.Height / 2f);
            }
            else
            {
                Origin = new Vector2(Sprite.Width / 2f, Sprite.Height / 2f);
            }
        }

        /// <summary>
        /// Tegner spriten
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, GameObject.Transform.Position, SourceRectangle, Color, GameObject.Transform.Rotation, Origin, GameObject.Transform.Scale, SpriteEffects.None, 0);

        }
    }
}
