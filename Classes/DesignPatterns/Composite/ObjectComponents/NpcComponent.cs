using Greif;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.Dialog;
using Grief.Classes.Quests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    public class NpcComponent : Component
    {
        private Animator animator;
        private Texture2D[] idleFrames;
        private Vector2 velocity;
        private float gravity = 600f;
        private bool grounded;

        public string Name { get; set; }
        public List<string> DialogLines { get; set; }
        public Quest QuestToGive { get; set; }
        public Texture2D Portrait { get; set; }

        public NpcComponent(GameObject gameObject) : base(gameObject) { }

        public override void Start()
        {
            animator = GameObject.GetComponent<Animator>();
            AddAnimations();
            animator.PlayAnimation("Idle");
        }

        public override void Update()
        {
            if (grounded == false)
            {
                velocity.Y += gravity * GameWorld.Instance.DeltaTime;
            }

            //Bevæg spilleren baseret på velocity
            Vector2 originalPosition = GameObject.Transform.Position;
            Vector2 movement = new Vector2(0, velocity.Y * GameWorld.Instance.DeltaTime);
            GameObject.Transform.Translate(movement);
            grounded = CheckGrounded();

            if (grounded == true && velocity.Y > 0)
            {
                velocity.Y = 0;
                animator.PlayAnimation("Idle");
            }
        }

        private bool CheckGrounded()
        {
            var collider = GameObject.GetComponent<Collider>().CollisionBox;
            var rectTiles = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.CollisionRectangles;
            var polyTiles = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.CollisionPolygons;

            foreach (var tile in rectTiles)
            {
                bool isAbove = collider.Bottom <= tile.Top + 5;
                bool isFallingOnto = collider.Bottom + velocity.Y * GameWorld.Instance.DeltaTime >= tile.Top;
                bool horizontalOverlap = collider.Right > tile.Left && collider.Left < tile.Right;

                if (isAbove == true && isFallingOnto == true && horizontalOverlap == true)
                {
                    GameObject.Transform.Position = new Vector2(GameObject.Transform.Position.X, tile.Top - collider.Height / 2f);
                    return true;
                }
            }

            foreach (var tile in polyTiles)
            {
                var points = tile.Vertices;

                for (int i = 0; i < points.Length - 1; i++)
                {
                    Vector2 p1 = points[i];
                    Vector2 p2 = points[(i + 1) % points.Length];

                    if (Math.Abs(p1.X - p2.X) < 1f)
                    {
                        continue;
                    }

                    if (p1.X > p2.X)
                    {
                        var temp = p1;
                        p1 = p2;
                        p2 = temp;
                    }

                    float playerX = collider.Center.X;

                    if (playerX >= p1.X && playerX <= p2.X)
                    {
                        float slope = (p2.Y - p1.Y) / (p2.X - p1.X);
                        float yOnSlope = p1.Y + slope * (playerX - p1.X);

                        float playerBottom = GameObject.Transform.Position.Y + collider.Height / 2f;

                        if (playerBottom >= yOnSlope - 10 && playerBottom <= yOnSlope + 10)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void Interaction()
        {
            Debug.WriteLine($"Interacting with {Name}. Dialog count: {DialogLines?.Count}");

            DialogSystem dialog = GameWorld.Instance.Dialog;
            dialog.StartDialog(DialogLines, Portrait, accepted =>
            {
                if (accepted == true)
                {
                    //Giv quest
                }
            });
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
