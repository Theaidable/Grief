using Greif;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Grief.Classes.DesignPatterns.Composite.Components
{
    /// <summary>
    /// Enumaration til at bestemme hvilken type af sprite der bruges
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public enum SpriteType
    {
        Sprite,
        Rectangle
    }

    /// <summary>
    /// Spriterenderer component
    /// </summary>
    public class SpriteRenderer : Component
    {
        //Properties
        public Vector2 Origin { get; set; }
        public Texture2D Sprite { get; set; }
        public Color Color { get; set; }
        public Rectangle? SourceRectangle { get; set; }
        public SpriteEffects Effects { get; set; }
        
        //Events
        public event Action OnSpriteChanged;
        public event Action OnEffectsChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameObject"></param>
        public SpriteRenderer(GameObject gameObject) : base(gameObject)
        {
            Color = Color.White;
            Effects = SpriteEffects.None;
        }

        /// <summary>
        /// Sæt sprite af et object, kan gøre brug af et spriteSheet hvis nødvendigt
        /// </summary>
        /// <param name="spriteName"></param>
        public void SetSprite(string spriteName, Rectangle? sourceRectangle = null)
        {
            Sprite = GameWorld.Instance.Content.Load<Texture2D>(spriteName);
            SourceRectangle = sourceRectangle;
            OnSpriteChanged?.Invoke();
        }

        /// <summary>
        /// Metode som bruges til at sætte en effect til et sprite
        /// </summary>
        /// <param name="name"></param>
        public void SetEffects(SpriteEffects name)
        {
            Effects = name;
            OnEffectsChanged?.Invoke();
        }
         
        /// <summary>
        /// Sæt origin til midten af valgt sprite
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
        /// Invoke event OnSpriteChanged
        /// </summary>
        public void InvokeOnSpriteChanged()
        {
            OnSpriteChanged?.Invoke();
        }

        /// <summary>
        /// Invoke event OnEffectsChanged
        /// </summary>
        public void InvokeOnEffectsChanged()
        {
            OnEffectsChanged?.Invoke();
        }

        /// <summary>
        /// Tegn sprite af object
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Sprite == null)
            {
                Console.WriteLine($"[FEJL] Sprite er null for objekt: {GameObject?.Tag}");
                return;
            }

            spriteBatch.Draw(Sprite, GameObject.Transform.Position, SourceRectangle, Color, GameObject.Transform.Rotation, Origin, GameObject.Transform.Scale, Effects, 0);
        }
    }
}
