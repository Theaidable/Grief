using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace Grief.Classes.GameManager
{
    public static class SoundManager
    {
        private static Song menuSong;
        private static Song levelSong;
        private static Song currentSong;

        public static void LoadContent(ContentManager content)
        {
            menuSong = content.Load<Song>("Sound/BGM/Evening Harmony");
            levelSong = content.Load<Song>("Sound/BGM/Wanderer's Tale");
        }

        public static void PlayMenuMusic()
        {
            if (currentSong != menuSong)
            {
                MediaPlayer.Stop();
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(menuSong);
                currentSong = menuSong;
            }
        }

        public static void PlayLevelMusic()
        {
            if (currentSong != levelSong)
            {
                MediaPlayer.Stop();
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(levelSong);
                currentSong = levelSong;
            }
        }

        public static void StopMusic()
        {
            MediaPlayer.Stop();
        }
    }
}
