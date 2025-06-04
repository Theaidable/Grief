using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace Grief.Classes.GameManager
{
    public static class SoundManager
    {
        private static Song menuSong;
        private static Song levelSong;

        public static void LoadContent(ContentManager content)
        {
            menuSong = content.Load<Song>("Sound/BGM/Evening Harmony");
            levelSong = content.Load<Song>("Sound/BGM/Wanderer's Tale");
        }

        public static void PlayMenuMusic()
        {
            if (MediaPlayer.Queue.ActiveSong != menuSong)
            {
                MediaPlayer.Stop();
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(menuSong);
            }
        }

        public static void PlayLevelMusic()
        {
            if (MediaPlayer.Queue.ActiveSong != levelSong)
            {
                MediaPlayer.Stop();
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(levelSong);
            }
        }

        public static void StopMusic()
        {
            MediaPlayer.Stop();
        }
    }
}
