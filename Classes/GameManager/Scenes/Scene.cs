using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Grief.Classes.GameManager.Scenes
{
    public abstract class Scene
    {
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
