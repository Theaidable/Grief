using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Grief.Classes.Levels
{
    /// <summary>
    /// Kontrollerer alle levels i spillet.
    /// </summary>
    /// <author>Asbjørn Ryberg</author>
    /// <author>David Gudmund Danielsen</author>
    public class LevelManager
    {
        /// <summary>
        /// Nuværende aktive level.
        /// </summary>
        public Level CurrentLevel { get; private set; }

        /// <summary>
        /// Loader et bestemt level og kalder LateStart() på alle objekter efter load.
        /// </summary>
        /// <param name="levelName"></param>
        public void LoadLevel(string levelName)
        {
            CurrentLevel = new Level();
            CurrentLevel.Load(levelName);

            // Kald LateStart på alle gameobjects (vigtigt for fx EnemyComponent og lign.)
            foreach (var gameObject in CurrentLevel.GameObjects)
            {
                gameObject.LateStart();
            }
        }

        /// <summary>
        /// Unloader det aktuelle level.
        /// </summary>
        public void UnloadLevel()
        {
            CurrentLevel = null;
        }

        /// <summary>
        /// Opdaterer alt i current level.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            CurrentLevel?.Update(gameTime);
        }

        /// <summary>
        /// Tegner alt i current level.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="viewMatrix"></param>
        public void Draw(SpriteBatch spriteBatch, Matrix viewMatrix)
        {
            CurrentLevel?.Draw(spriteBatch, viewMatrix);
        }
    }
}
