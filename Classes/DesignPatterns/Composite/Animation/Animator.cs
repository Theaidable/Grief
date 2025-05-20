using System.Collections.Generic;
using System;
using Grief.Classes.DesignPatterns.Composite.Sprite;

namespace Grief.Classes.DesignPatterns.Composite.Animation
{
    public class Animator : Component
    {
        public int CurrentIndex { get; private set; }
        public Animation CurrentAnimation { get => currentAnimation; set => currentAnimation = value; }
        public Action OnAnimationComplete;

        private float elapsed;
        private SpriteRenderer spriteRenderer;
        private Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
        private Animation currentAnimation;
        public Animator(GameObject gameObject) : base(gameObject)
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        public override void Start()
        {
            if (CurrentAnimation != null)
            {
                elapsed = 0f;
                CurrentIndex = 0;
                spriteRenderer.Sprite = CurrentAnimation.SpriteSheet;
                spriteRenderer.SourceRectangle = CurrentAnimation.Frames[0];
            }
        }

        public override void Update()
        {
            if (CurrentAnimation == null)
            {
                return;
            }

            elapsed += GameWorld.Instance.DeltaTime;

            if (CurrentAnimation.IsLooping)
            {
                CurrentIndex = (int)(elapsed * CurrentAnimation.FPS) % CurrentAnimation.Frames.Length;
            }
            else
            {
                CurrentIndex = (int)(elapsed * CurrentAnimation.FPS);
                if (CurrentIndex >= CurrentAnimation.Frames.Length)
                {
                    CurrentIndex = CurrentAnimation.Frames.Length - 1;
                    OnAnimationComplete?.Invoke();
                    OnAnimationComplete = null;
                }
            }

            spriteRenderer.SourceRectangle = CurrentAnimation.Frames[CurrentIndex];
        }

        public void AddAnimation(Animation animation)
        {
            animations[animation.Name] = animation;
            if (CurrentAnimation == null)
            {
                CurrentAnimation = animation;
            }
        }

        public void PlayAnimation(string animationName)
        {
            if (CurrentAnimation?.Name == animationName)
            {
                return;
            }

            CurrentAnimation = animations[animationName];
            spriteRenderer.Sprite = CurrentAnimation.SpriteSheet;
            elapsed = 0;
        }
    }
}
