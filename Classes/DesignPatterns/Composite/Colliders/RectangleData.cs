using Microsoft.Xna.Framework;

namespace Grief.Classes.DesignPatterns.Composite.Colliders
{
    public class RectangleData
    {
        public Rectangle Rectangle { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public RectangleData(int x, int y)
        {
            this.Rectangle = new Rectangle();
            this.X = x;
            this.Y = y;
        }

        public void UpdatePosition(GameObject gameObject, Rectangle sourceRectangle)
        {
            float worldX = gameObject.Transform.Position.X - sourceRectangle.Width / 2f + X;
            float worldY = gameObject.Transform.Position.Y - sourceRectangle.Height / 2f + Y;

            Rectangle = new Rectangle((int)worldX, (int)worldY, 1, 1);
        }
    }
}
