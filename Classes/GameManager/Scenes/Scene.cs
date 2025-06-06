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
        /// <summary>
        /// Abstrakt update metode
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Abstrakt Draw metode
        /// </summary>
        /// <param name="spriteBatch"></param>
        public abstract void Draw(SpriteBatch spriteBatch);

        /// <summary>
        /// Virtuel metode som kan bruges
        /// </summary>
        public virtual void LoadContent() { }

    }
}
