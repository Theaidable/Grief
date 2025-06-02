using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Grief.Classes.Levels
{
    public class LevelManager
    {
        public Level CurrentLevel { get; private set; }

        public void LoadLevel(string levelName)
        {
            CurrentLevel = new Level();
            CurrentLevel.Load(levelName);

            foreach (var gameObject in CurrentLevel.GameObjects)
            {
                gameObject.LateStart();
            }
        }

        public void UnloadLevel()
        {
            CurrentLevel = null;
        }

        public void Update(GameTime gameTime)
        {
            CurrentLevel?.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, Matrix viewMatrix)
        {
            CurrentLevel?.Draw(spriteBatch, viewMatrix);
        }
    }
}
