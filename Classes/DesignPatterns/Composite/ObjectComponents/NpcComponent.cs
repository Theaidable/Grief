using Greif;
using Grief.Classes.DesignPatterns.Composite.Components;
using Microsoft.Xna.Framework.Graphics;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    public class NpcComponent : Component
    {
        private Animator animator;
        private Texture2D[] idleFrames;

        public NpcComponent(GameObject gameObject) : base(gameObject) { }

        public override void Start()
        {
            animator = GameObject.GetComponent<Animator>();
            AddAnimations();
            animator.PlayAnimation("Idle");
        }

        private void AddAnimations()
        {
            //Load Frames
            idleFrames = LoadFrames("MainCharacter/Idle/Idle", 2);

            //Add animations
            animator.AddAnimation(new Animation("Idle", 2.5f, true, idleFrames));
        }

        private Texture2D[] LoadFrames(string basePath, int frameCount)
        {
            Texture2D[] frames = new Texture2D[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                frames[i] = GameWorld.Instance.Content.Load<Texture2D>($"{basePath}0{i+1}");
            }
            return frames;
        }
    }
}
