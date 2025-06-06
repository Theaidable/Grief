using Grief.Classes.DesignPatterns.Composite;
using Microsoft.Xna.Framework;

namespace Greif.Classes.Cameras
{
    /// <summary>
    /// Camera klasse til at fokusere på bestemte objekter i spillet
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public class Camera
    {
        //Private fields
        private float zoom;
        private float rotation;
        private Vector2 position;

        public Vector2 Position { get; set; }

        //Matrix
        public Matrix ViewMatrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
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
            Position = Vector2.Zero;
        }

        /// <summary>
        /// Metode til at følge et bestemt target
        /// </summary>
        /// <param name="target"></param>
        /// <param name="mapWidth"></param>
        /// <param name="mapHeight"></param>
        public void Follow(GameObject target, int mapWidth, int mapHeight)
        {
            Position = target.Transform.Position;

            var viewport = GameWorld.Instance.GraphicsDevice.Viewport;

            position.X = MathHelper.Clamp(Position.X, viewport.Width / (2 * zoom), mapWidth - viewport.Width / (2 * zoom));
            position.Y = MathHelper.Clamp(Position.Y, viewport.Height / (2 * zoom), mapHeight - viewport.Height / (2 * zoom));

            Position = position;
        }

        /// <summary>
        /// Hjælpemetode til at sætte zoom, hvis man gerne vil justere zoom niveau
        /// </summary>
        /// <param name="newZoom"></param>
        public void SetZoom(float newZoom)
        {
            zoom = MathHelper.Clamp(newZoom, 0.1f, 10f);
        }

        /// <summary>
        /// Skal bruges til at omsætte screen position til world position
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <returns></returns>
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(ViewMatrix));
        }
    }
}
