using Greif;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Grief.Classes.DesignPatterns.Composite.Components
{
    /// <summary>
    /// Collider component
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public class Collider : Component
    {
        //Fields
        private SpriteRenderer spriteRenderer;
        private bool shouldDraw;
        private Lazy<List<RectangleData>> pixelPerfectRectangles;
        public List<RectangleData> PixelPerfectRectangles { get => pixelPerfectRectangles.Value; }
        public Point ColliderSize { get; set; } = Point.Zero;

        /// <summary>
        /// CollisonBox for object
        /// </summary>
        public Rectangle CollisionBox
        {
            get
            {
                int width = ColliderSize.X > 0 ? ColliderSize.X : spriteRenderer.SourceRectangle?.Width ?? spriteRenderer.Sprite.Width;
                int height = ColliderSize.Y > 0 ? ColliderSize.Y : spriteRenderer.SourceRectangle?.Height ?? spriteRenderer.Sprite.Height;
                return new Rectangle(
                    (int)(GameObject.Transform.Position.X - width / 2),
                    (int)(GameObject.Transform.Position.Y - height / 2),
                    width,
                    height);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameObject"></param>
        public Collider(GameObject gameObject) : base(gameObject) { }

        /// <summary>
        /// Lav pixelPerfectRectangles til objektet
        /// </summary>
        public override void Start()
        {
            spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.OnSpriteChanged += RebuildCollider;
            spriteRenderer.OnEffectsChanged += RebuildCollider;
            pixelPerfectRectangles = new Lazy<List<RectangleData>>(() => CreateRectangles());
            UpdatePixelCollider();
        }

        /// <summary>
        /// Update pixelcollider
        /// </summary>
        public override void Update()
        {
            if (spriteRenderer?.Sprite != null && pixelPerfectRectangles.IsValueCreated)
            {
                UpdatePixelCollider();
            }
        }

        private void RebuildCollider()
        {
            pixelPerfectRectangles = new Lazy<List<RectangleData>>(() => CreateRectangles());
            UpdatePixelCollider();
        }

        /// <summary>
        /// Hjælpemetode til at opdatere vores pixelcollider
        /// </summary>
        public void UpdatePixelCollider()
        {
            int width = spriteRenderer.SourceRectangle?.Width ?? spriteRenderer.Sprite.Width;
            int height = spriteRenderer.SourceRectangle?.Height ?? spriteRenderer.Sprite.Height;

            for (int i = 0; i < pixelPerfectRectangles.Value.Count; i++)
            {
                pixelPerfectRectangles.Value[i].UpdatePosition(GameObject, width, height);
            }
        }

        /// <summary>
        /// Draw vores rectangler hvis de toggles
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (shouldDraw)
            {
                DrawRectangle(CollisionBox, spriteBatch);
                if (pixelPerfectRectangles.IsValueCreated)
                {
                    foreach (var rect in pixelPerfectRectangles.Value)
                    {
                        DrawRectangle(rect.Rectangle, spriteBatch);
                    }
                }
            }
        }

        /// <summary>
        /// Tegner collider rectangles
        /// </summary>
        /// <param name="collisionBox"></param>
        /// <param name="spriteBatch"></param>
        private void DrawRectangle(Rectangle collisionBox, SpriteBatch spriteBatch)
        {
            Rectangle topLine = new Rectangle(collisionBox.X, collisionBox.Y, collisionBox.Width, 1);
            Rectangle bottomLine = new Rectangle(collisionBox.X, collisionBox.Y + collisionBox.Height, collisionBox.Width, 1);
            Rectangle rightLine = new Rectangle(collisionBox.X + collisionBox.Width, collisionBox.Y, 1, collisionBox.Height);
            Rectangle leftLine = new Rectangle(collisionBox.X, collisionBox.Y, 1, collisionBox.Height);

            spriteBatch.Draw(GameWorld.Instance.Pixel, topLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(GameWorld.Instance.Pixel, bottomLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(GameWorld.Instance.Pixel, rightLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(GameWorld.Instance.Pixel, leftLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }

        /// <summary>
        /// Bruges til at toggle drawing af og på
        /// </summary>
        /// <param name="shouldDraw"></param>
        public void ToggleDrawing(bool shouldDraw)
        {
            this.shouldDraw = shouldDraw;
        }

        /// <summary>
        /// Hjælpemetode til at oprette rectanglerne
        /// </summary>
        /// <returns></returns>
        private List<RectangleData> CreateRectangles()
        {
            List<Color[]> lines = new List<Color[]>();

            int startX = spriteRenderer.SourceRectangle?.X ?? 0;
            int startY = spriteRenderer.SourceRectangle?.Y ?? 0;
            int width = spriteRenderer.SourceRectangle?.Width ?? spriteRenderer.Sprite.Width;
            int height = spriteRenderer.SourceRectangle?.Height ?? spriteRenderer.Sprite.Height;

            for (int y = 0; y < height; y++)
            {
                Color[] colors = new Color[width];
                spriteRenderer.Sprite.GetData(0, new Rectangle(startX, startY + y, width, 1), colors, 0, width);
                lines.Add(colors);
            }

            List<RectangleData> returnListOfRectangles = new List<RectangleData>();
            for (int y = 0; y < lines.Count; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x].A != 0)
                    {
                        if (x == 0
                            || x == lines[y].Length
                            || x > 0 && lines[y][x - 1].A == 0
                            || x < lines[y].Length - 1 && lines[y][x + 1].A == 0
                            || y == 0
                            || y > 0 && lines[y - 1][x].A == 0
                            || y < lines.Count - 1 && lines[y + 1][x].A == 0)
                        {
                            RectangleData rd = new RectangleData(x, y);
                            returnListOfRectangles.Add(rd);
                        }
                    }
                }
            }
            return returnListOfRectangles;
        }

        /// <summary>
        /// Hjælpe metode som bruges til at finde ud af om enemies er på jorden
        /// </summary>
        /// <returns></returns>
        public bool CheckGrounded(GameObject gameObject)
        {
            var collider = gameObject.GetComponent<Collider>().CollisionBox;
            var rectTiles = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.CollisionRectangles;
            var polyTiles = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.CollisionPolygons;

            foreach (var tile in rectTiles)
            {
                bool isAbove = collider.Bottom <= tile.Top + 5;
                bool isFallingOnto = collider.Bottom + gameObject.Transform.Velocity.Y * GameWorld.Instance.DeltaTime >= tile.Top;
                bool horizontalOverlap = collider.Right > tile.Left && collider.Left < tile.Right;

                if (isAbove == true && isFallingOnto == true && horizontalOverlap == true)
                {
                    gameObject.Transform.Position = new Vector2(gameObject.Transform.Position.X, tile.Top - collider.Height / 2f);
                    return true;
                }
            }

            foreach (var tile in polyTiles)
            {
                var points = tile.Vertices;

                for (int i = 0; i < points.Length - 1; i++)
                {
                    Vector2 p1 = points[i];
                    Vector2 p2 = points[(i + 1) % points.Length];

                    if (Math.Abs(p1.X - p2.X) < 1f)
                    {
                        continue;
                    }

                    if (p1.X > p2.X)
                    {
                        var temp = p1;
                        p1 = p2;
                        p2 = temp;
                    }

                    float objectX = collider.Center.X;

                    if (objectX >= p1.X && objectX <= p2.X)
                    {
                        float slope = (p2.Y - p1.Y) / (p2.X - p1.X);
                        float yOnSlope = p1.Y + slope * (objectX - p1.X);

                        float enemyBottom = GameObject.Transform.Position.Y + collider.Height / 2f;

                        if (enemyBottom >= yOnSlope - 10 && enemyBottom <= yOnSlope + 10)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

    }

    /// <summary>
    /// Hjælpeklasse der holder styr på rectanglernes data
    /// </summary>
    public class RectangleData
    {
        public Rectangle Rectangle { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public RectangleData(int x, int y)
        {
            Rectangle = new Rectangle();
            X = x;
            Y = y;
        }

        public void UpdatePosition(GameObject gameObject, int width, int height)
        {
            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            var origin = spriteRenderer?.Origin ?? Vector2.Zero;

            int newX = spriteRenderer.Effects == SpriteEffects.FlipHorizontally ? (width - 1 - X) : X;

            Rectangle = new Rectangle((int)(gameObject.Transform.Position.X - origin.X + newX), (int)(gameObject.Transform.Position.Y - origin.Y + Y), 1, 1);
        }
    }
}
