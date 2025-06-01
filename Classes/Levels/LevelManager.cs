using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Grief.Classes.Levels
{
    /// <summary>
    /// Kontrollere alle levels
    /// </summary>
    public class LevelManager
    {
        //Properties
        public Level CurrentLevel { get; private set; }

        /// <summary>
        /// Metode til at indlæse et bestemt level
        /// </summary>
        /// <param name="levelName"></param>
        public void LoadLevel(string levelName)
        {
            CurrentLevel = new Level();
            CurrentLevel.Load(levelName);
        }

        /// <summary>
        /// Opdatere alt i current level
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            CurrentLevel?.Update(gameTime);
        }

        /// <summary>
        /// Tegner alt i currentlevel
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="viewMatrix"></param>
        public void Draw(SpriteBatch spriteBatch, Matrix viewMatrix)
        {
            CurrentLevel?.Draw(spriteBatch, viewMatrix);
        }
    }
}
