using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Grief.Classes.GameManager.Scenes
{
    /// <summary>
    /// Abstract super klasse for alle scener
    /// </summary>
    /// <author>Asbjørn Ryberg</author>
    public abstract class Scene
    {
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
        public virtual void LoadContent() { }

    }
}
