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
            /*
             * IdleFrames
             * idleFrames = LoadFrames("stien for at finde den sprite som er idleLeft", antal frames);
             * 
             * AddAnimationen
             * animator.AddAnimation(new Animation("Idle", 2.5f, true, idleFrames));
             */
        }
    }
}
