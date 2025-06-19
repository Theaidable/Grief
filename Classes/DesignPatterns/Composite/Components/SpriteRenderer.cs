using Greif;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;

namespace Grief.Classes.DesignPatterns.Composite.Components
{
    /// <summary>
    /// Enumaration til at bestemme hvilken type af sprite der bruges
    /// </summary>
    public enum SpriteType
    {
        Sprite,
        Rectangle
    }

    /// <summary>
    /// Spriterenderer component med optimering (view-culling + texture-cache)
    /// </summary>
    public class SpriteRenderer : Component
    {
        // Texture-cache så vi ikke loader den samme sprite flere gange
        private static Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

        public Vector2 Origin { get; set; }
        public Texture2D Sprite { get; set; }
        public Color Color { get; set; }
        public Rectangle? SourceRectangle { get; set; }
        public SpriteEffects Effects { get; set; }

        public event Action OnSpriteChanged;
        public event Action OnEffectsChanged;

        public SpriteRenderer(GameObject gameObject) : base(gameObject)
        {
            Color = Color.White;
            Effects = SpriteEffects.None;
        }

        /// <summary>
        /// Sæt sprite, bruger texture-cache for at undgå at loade flere gange
        /// </summary>
        public void SetSprite(string spriteName, Rectangle? sourceRectangle = null)
        {
            if (!textureCache.TryGetValue(spriteName, out var texture))
            {
                texture = GameWorld.Instance.Content.Load<Texture2D>(spriteName);
                textureCache[spriteName] = texture;
            }
            Sprite = texture;
            SourceRectangle = sourceRectangle;
            OnSpriteChanged?.Invoke();
        }

        /// <summary>
        /// Sæt effect på sprite (f.eks. flip horisontalt)
        /// </summary>
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
            else if (Sprite != null)
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
        /// Tegner kun objektet hvis det er synligt på kameraet ("view frustum culling")
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Sprite == null)
            {
                Console.WriteLine($"[FEJL] Sprite er null for objekt: {GameObject?.Tag}");
                return;
            }

            // --- OPTIMERING: Kun tegn hvis inden for kameraet ---
            var camera = GameWorld.Instance.Camera;
            RectangleF camView = camera.BoundingRectangle;

            // Hent skalering på både X og Y (i tilfælde af ikke-ens skala)
            float scaleX = GameObject.Transform.Scale.X;
            float scaleY = GameObject.Transform.Scale.Y;

            float width = (SourceRectangle?.Width ?? Sprite.Width) * scaleX;
            float height = (SourceRectangle?.Height ?? Sprite.Height) * scaleY;
            RectangleF objBounds = new RectangleF(
                GameObject.Transform.Position.X - width / 2f,
                GameObject.Transform.Position.Y - height / 2f,
                width,
                height
            );

            // Tjek om objektet overlapper kameraet (view-frustum culling)
            if (!camView.Intersects(objBounds))
            {
                // Uden for skærmen, så returnér uden at tegne
                return;
            }

            spriteBatch.Draw(
                Sprite,
                GameObject.Transform.Position,
                SourceRectangle,
                Color,
                GameObject.Transform.Rotation,
                Origin,
                GameObject.Transform.Scale, // Her bruger vi stadig Vector2 (MonoGame overload)
                Effects,
                0 // layerDepth, kan evt. erstattes med GameObject.LayerDepth hvis du har det
            );
        }

    }
}
