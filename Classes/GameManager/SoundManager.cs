using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Grief.Classes.GameManager
{
    /// <summary>
    /// SoundManager som styrer lyd
    /// </summary>
    /// <author>Asbjørn Ryberg</author>
    public static class SoundManager
    {
        //Fields
        private static Song menuSong;
        private static Song levelSong;
        private static Song currentSong;

        private static List<SoundEffect> jumpSounds = new();
        private static List<SoundEffect> attackSounds = new();
        private static List<SoundEffect> enemyAttackSounds = new();

        private static Random random = new();

        /// <summary>
        /// Loader alle sangenen og soundeffects til deres lists
        /// </summary>
        /// <param name="content"></param>
        public static void LoadContent(ContentManager content)
        {
            menuSong = content.Load<Song>("Sound/BGM/Evening Harmony");
            levelSong = content.Load<Song>("Sound/BGM/Wanderer's Tale");

            jumpSounds.Add(content.Load<SoundEffect>("Sound/SFX/human_jump_1"));
            jumpSounds.Add(content.Load<SoundEffect>("Sound/SFX/human_jump_2"));
            jumpSounds.Add(content.Load<SoundEffect>("Sound/SFX/human_jump_3"));

            attackSounds.Add(content.Load<SoundEffect>("Sound/SFX/sword_miss_1"));
            attackSounds.Add(content.Load<SoundEffect>("Sound/SFX/sword_miss_2"));
            attackSounds.Add(content.Load<SoundEffect>("Sound/SFX/sword_miss_3"));
            
            enemyAttackSounds.Add(content.Load<SoundEffect>("Sound/SFX/human_atk_sword_1"));
            enemyAttackSounds.Add(content.Load<SoundEffect>("Sound/SFX/human_atk_sword_2"));
            enemyAttackSounds.Add(content.Load<SoundEffect>("Sound/SFX/human_atk_sword_3"));
        }

        /// <summary>
        /// Afspiller Menu Music
        /// </summary>
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

        /// <summary>
        /// Afspiller Level Music
        /// </summary>
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

        /// <summary>
        /// Afspiller Jump Sound
        /// </summary>
        public static void PlayJumpSound()
        {
            if (jumpSounds.Count > 0)
            {
                int index = random.Next(jumpSounds.Count);
                jumpSounds[index].Play();
            }
        }

        /// <summary>
        /// Afspillet Attack Sound
        /// </summary>
        public static void PlayAttackSound()
        {
            if (attackSounds.Count > 0)
            {
                int index = random.Next(attackSounds.Count);
                attackSounds[index].Play();
            }
        }

        /// <summary>
        /// Afspiller EnemyAttackSound
        /// </summary>
        public static void PlayEnemyAttackSound()
        {
            if (enemyAttackSounds.Count > 0)
            {
                int index = random.Next(enemyAttackSounds.Count);
                enemyAttackSounds[index].Play();
            }
        }

        /// <summary>
        /// Stopper musikken
        /// </summary>
        public static void StopMusic()
        {
            MediaPlayer.Stop();
        }
    }
}
