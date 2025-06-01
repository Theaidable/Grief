using Grief.Classes.DesignPatterns.Composite;
using Microsoft.Xna.Framework;

namespace Greif.Classes.Cameras
{
    /// <summary>
    /// Camera klasse til at fokusere på bestemte objekter i spillet
    /// </summary>
    public class Camera
    {
        //Private fields
        private Vector2 position;
        private float zoom;
        private float rotation;

        //Matrix
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

        /// <summary>
        /// Constructor
        /// </summary>
        public Camera()
        {
            zoom = 2.7f;
            rotation = 0f;
            position = Vector2.Zero;
        }

        /// <summary>
        /// Metode til at følge et bestemt target
        /// </summary>
        /// <param name="target"></param>
        /// <param name="mapWidth"></param>
        /// <param name="mapHeight"></param>
        public void Follow(GameObject target, int mapWidth, int mapHeight)
        {
            position = target.Transform.Position;

            var viewport = GameWorld.Instance.GraphicsDevice.Viewport;

            position.X = MathHelper.Clamp(position.X,viewport.Width / (2 * zoom), mapWidth - viewport.Width / (2 * zoom));
            position.Y = MathHelper.Clamp(position.Y,viewport.Height / (2 * zoom),mapHeight - viewport.Height / (2 *zoom));
        }

        /// <summary>
        /// Hjælpemetode til at sætte zoom, hvis man gerne vil justere zoom niveau
        /// </summary>
        /// <param name="newZoom"></param>
        public void SetZoom(float newZoom)
        {
            zoom = MathHelper.Clamp(newZoom, 0.1f, 10f);
        }
    }
}
