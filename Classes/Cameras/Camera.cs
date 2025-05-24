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
            zoom = 1f;
            rotation = 0f;
            position = Vector2.Zero;
        }

        public void Follow(GameObject target)
        {
            position = target.Transform.Position;
        }

        public void SetZoom(float newZoom)
        {
            zoom = MathHelper.Clamp(newZoom, 0.1f, 10f);
        }
    }
}
