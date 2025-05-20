using Grief.Classes.DesignPatterns.Composite.Animation;
using Grief.Classes.DesignPatterns.Composite.Sprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Grief.Classes.DesignPatterns.Composite.Colliders
{
    public class Collider : Component
    {
        private Texture2D pixel;
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private bool shouldDraw;
        //private Lazy<List<RectangleData>> pixelPerfectRectangles;
        private Rectangle spriteSourceRectangle;
        private Rectangle lastSpriteSourceRectangle;
        private Texture2D spriteTexture;

        public List<RectangleData> PixelPerfectRectangles { get; private set; } = new List<RectangleData>();

        //public List<RectangleData> PixelPerfectRectangles { get => pixelPerfectRectangles.Value; }
        public Collider(GameObject gameObject) : base(gameObject) { }
        public Rectangle CollisionBox
        {
            get
            {
                return new Rectangle(
                    (int)(GameObject.Transform.Position.X - spriteSourceRectangle.Width / 2),
                    (int)(GameObject.Transform.Position.Y - spriteSourceRectangle.Height / 2),
                    spriteSourceRectangle.Width,
                    spriteSourceRectangle.Height);
            }
        }

        public override void Awake()
        {
        }

        public override void Start()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            animator = GameObject.GetComponent<Animator>();
            pixel = GameWorld.Instance.Content.Load<Texture2D>("Assets/Collider/Pixel");
            RefreshSourceRect();
            RebuildPixelPerfect();
            lastSpriteSourceRectangle = spriteSourceRectangle;
            UpdatePixelCollider();
        }

        public override void Update()
        {
            RefreshSourceRect();

            if (spriteSourceRectangle != lastSpriteSourceRectangle)
            {
                RebuildPixelPerfect();
                lastSpriteSourceRectangle = spriteSourceRectangle;
            }

            UpdatePixelCollider();
        }

        private void RefreshSourceRect()
        {
            if (animator != null && animator.CurrentAnimation != null)
            {
                spriteTexture = animator.CurrentAnimation.SpriteSheet;
                spriteSourceRectangle = animator.CurrentAnimation.Frames[animator.CurrentIndex];
            }
            else
            {
                spriteTexture = spriteRenderer.Sprite;
                spriteSourceRectangle = spriteRenderer.SourceRectangle ?? new Rectangle(0, 0, spriteTexture.Width, spriteTexture.Height);
            }
        }

        private void RebuildPixelPerfect()
        {
            PixelPerfectRectangles.Clear();

            // Hent pixel‐data linje for linje
            var lines = new List<Color[]>();
            for (int y = 0; y < spriteSourceRectangle.Height; y++)
            {
                var row = new Color[spriteSourceRectangle.Width];
                spriteTexture.GetData(
                    0,
                    new Rectangle(
                        spriteSourceRectangle.X,
                        spriteSourceRectangle.Y + y,
                        spriteSourceRectangle.Width, 1),
                    row, 0,
                    spriteSourceRectangle.Width);
                lines.Add(row);
            }

            // Find kant‐pixels (alpha>0 + mindst én gennemsigtig nabo)
            for (int y = 0; y < spriteSourceRectangle.Height; y++)
            {
                for (int x = 0; x < spriteSourceRectangle.Width; x++)
                {
                    if (lines[y][x].A == 0) continue;

                    bool isEdge =
                        x == 0 || x == spriteSourceRectangle.Width - 1
                     || y == 0 || y == spriteSourceRectangle.Height - 1
                     || lines[y][x - 1].A == 0
                     || lines[y][x + 1].A == 0
                     || lines[y - 1][x].A == 0
                     || lines[y + 1][x].A == 0;

                    if (isEdge)
                        PixelPerfectRectangles.Add(new RectangleData(x, y));
                }
            }
        }

        private void UpdatePixelCollider()
        {
            foreach (RectangleData rectangleData in PixelPerfectRectangles)
            {
                rectangleData.UpdatePosition(GameObject, spriteSourceRectangle);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (shouldDraw)
            {
                DrawRectangle(CollisionBox, spriteBatch);
                foreach (RectangleData rectangleData in PixelPerfectRectangles)
                {
                    DrawRectangle(rectangleData.Rectangle, spriteBatch);
                }
            }
        }

        private void DrawRectangle(Rectangle collisionBox, SpriteBatch spriteBatch)
        {
            Rectangle topLine = new Rectangle(collisionBox.X, collisionBox.Y, collisionBox.Width, 1);
            Rectangle bottomLine = new Rectangle(collisionBox.X, collisionBox.Y + collisionBox.Height, collisionBox.Width, 1);
            Rectangle rightLine = new Rectangle(collisionBox.X + collisionBox.Width, collisionBox.Y, 1, collisionBox.Height);
            Rectangle leftLine = new Rectangle(collisionBox.X, collisionBox.Y, 1, collisionBox.Height);

            spriteBatch.Draw(pixel, topLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(pixel, bottomLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(pixel, rightLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(pixel, leftLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }

        public void ToggleDrawing(bool shouldDraw)
        {
            this.shouldDraw = shouldDraw;
        }
    }
}
