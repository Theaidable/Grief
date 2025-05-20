using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Grief.Classes.DesignPatterns.Composite.Animation
{
    public class Animation
    {
        public float FPS { get; private set; }
        public string Name { get; private set; }
        public Texture2D SpriteSheet { get; private set; }
        public Rectangle[] Frames { get; private set; }
        public bool IsLooping { get; private set; }

        public Animation(string name, Texture2D spriteSheet, Rectangle[] frames, float fps, bool isLooping = true)
        {
            Name = name;
            SpriteSheet = spriteSheet;
            Frames = frames;
            FPS = fps;
            IsLooping = isLooping;
        }
    }
}
