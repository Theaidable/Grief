using Greif;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Grief.Classes.DesignPatterns.Composite.Components
{
    /// <summary>
    /// Enumaration til at bestemme animationstype om det er Sprite eller SpriteSheet
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public enum AnimationType
    {
        Sprite,
        Rectangle
    }

    /// <summary>
    /// Animator component
    /// </summary>
    public class Animator : Component
    {
        //Fields
        private float elapsed;
        private SpriteRenderer spriteRenderer;
        private Dictionary<string, Animation> animations = new Dictionary<string, Animation>();

        //Properties
        public int CurrentIndex { get; private set; }
        public Animation CurrentAnimation { get; private set; }


        //Events
        public Action OnAnimationComplete;

        /// <summary>
        /// Animator constructor skal gøre brug af spriteRenderer, da vi skal bruge spritet af et objekt for at animere det
        /// </summary>
        /// <param name="gameObject"></param>
        public Animator(GameObject gameObject) : base(gameObject)
        {
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Update som gør at animationer opdateres i gameworld
        /// </summary>
        public override void Update()
        {
            if (CurrentAnimation == null)
            {
                return;
            }

            elapsed += GameWorld.Instance.DeltaTime;
            CurrentIndex = (int)(elapsed * CurrentAnimation.FPS);
            bool animationEnded = false;

            switch (CurrentAnimation.Type)
            {
                case AnimationType.Sprite:
                    if (CurrentIndex >= CurrentAnimation.Sprites.Length)
                    {
                        if (CurrentAnimation.Loop == true)
                        {
                            elapsed = 0;
                            CurrentIndex = 0;
                        }
                        else
                        {
                            CurrentIndex = CurrentAnimation.Sprites.Length - 1;
                            animationEnded = true;
                        }
                    }

                    spriteRenderer.Sprite = CurrentAnimation.Sprites[CurrentIndex];
                    spriteRenderer.SourceRectangle = null;
                    spriteRenderer.InvokeOnSpriteChanged();
                    break;

                case AnimationType.Rectangle:
                    if (CurrentIndex >= CurrentAnimation.Rectangles.Length)
                    {
                        if (CurrentAnimation.Loop == true)
                        {
                            elapsed = 0;
                            CurrentIndex = 0;
                        }
                        else
                        {
                            CurrentIndex = CurrentAnimation.Rectangles.Length - 1;
                            animationEnded = true;
                        }
                    }

                    spriteRenderer.SourceRectangle = CurrentAnimation.Rectangles[CurrentIndex];
                    spriteRenderer.InvokeOnSpriteChanged();
                    break;
            }

            if (animationEnded == true && OnAnimationComplete != null)
            {
                OnAnimationComplete.Invoke();
            }
        }

        public void ClearOnAnimationComplete()
        {
            OnAnimationComplete = null;
        }

        /// <summary>
        /// Adde animationer
        /// </summary>
        /// <param name="animation"></param>
        public void AddAnimation(Animation animation)
        {
            animations.Add(animation.Name, animation);
            if (CurrentAnimation == null)
            {
                CurrentAnimation = animation;
            }
        }

        /// <summary>
        /// Spille animationerne
        /// </summary>
        /// <param name="animationName"></param>
        public void PlayAnimation(string animationName)
        {
            if (animationName != CurrentAnimation.Name)
            {
                CurrentAnimation = animations[animationName];
                elapsed = 0;
                CurrentIndex = 0;
            }
        }

        /// <summary>
        /// Hjælpemetode til at indlæse Texture2D arrays
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="frameCount"></param>
        /// <returns></returns>
        public Texture2D[] LoadFrames(string basePath, int frameCount)
        {
            Texture2D[] frames = new Texture2D[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                frames[i] = GameWorld.Instance.Content.Load<Texture2D>($"{basePath}0{i + 1}");
            }
            return frames;
        }
    }

    /// <summary>
    /// Hjælpeklasse til at holde styr på information omkring animationer
    /// </summary>
    public class Animation
    {
        public float FPS { get; private set; }
        public string Name { get; private set; }
        public Texture2D[] Sprites { get; private set; }
        public Rectangle[] Rectangles { get; private set; }
        public AnimationType Type { get; private set; }
        public bool Loop { get; private set; }

        public Animation(string name, float fps, bool loop = true, Texture2D[] sprites = null, Rectangle[] rectangles = null)
        {
            Name = name;
            FPS = fps;
            Loop = loop;

            //Sæt sprites eller rectangles til de givne frames ellers sæt dem til 0
            Sprites = sprites ?? new Texture2D[0];
            Rectangles = rectangles ?? new Rectangle[0];

            //Her sætter vi også typen af animation
            if (Sprites.Length > 0)
            {
                Type = AnimationType.Sprite;
            }
            else if (Rectangles.Length > 0)
            {
                Type = AnimationType.Rectangle;
            }
        }

    }
}
