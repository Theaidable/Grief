using Grief.Classes.DesignPatterns.Composite;
using Microsoft.Xna.Framework;

namespace Greif.Classes.Cameras
{
    public class Camera
    {
        private Vector2 position;
        private float zoom;
        private float rotation;

        public Matrix ViewMatrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
                       Matrix.CreateRotationZ(rotation) *
                       Matrix.CreateScale(zoom, zoom, 1f) *
                       Matrix.CreateTranslation(new Vector3(GameWorld.Instance.GraphicsDevice.Viewport.Width / 2f, GameWorld.Instance.GraphicsDevice.Viewport.Height / 2f, 0));
            }
        }

        public Camera()
        {
            zoom = 2.7f;
            rotation = 0f;
            position = Vector2.Zero;
        }

        public void Follow(GameObject target, int mapWidth, int mapHeight)
        {
            position = target.Transform.Position;

            var viewport = GameWorld.Instance.GraphicsDevice.Viewport;

            position.X = MathHelper.Clamp(position.X,viewport.Width / (2 * zoom), mapWidth - viewport.Width / (2 * zoom));
            position.Y = MathHelper.Clamp(position.Y,viewport.Height / (2 * zoom),mapHeight - viewport.Height / (2 *zoom));
        }

        public void SetZoom(float newZoom)
        {
            zoom = MathHelper.Clamp(newZoom, 0.1f, 10f);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(ViewMatrix));
        }
    }
}
