using Greif;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.Dialog;
using Grief.Classes.Quests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    /// <summary>
    /// Npc component
    /// </summary>
    public class NpcComponent : Component
    {
        //Fields
        private Animator animator;
        private Vector2 velocity;
        private float gravity = 600f;
        private bool grounded;

        //DadFrames
        private Texture2D[] idleDadFrames;
        private Texture2D[] happyDadFrames;

        //Properties
        public string Name { get; set; }
        public List<string> DialogLinesBeforeAccept { get; set; }
        public List<string> DialogLinesAcceptedNotCompleted { get; set; }
        public List<string> DialogLinesOnCompleted { get; set; }
        public List<string> DialogLinesAlreadyCompleted { get; set; }

        public Quest QuestToGive { get; set; }
        public Texture2D Portrait { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameObject"></param>
        public NpcComponent(GameObject gameObject) : base(gameObject) { }

        /// <summary>
        /// Tilføj animationer ved start
        /// </summary>
        public override void Start()
        {
            animator = GameObject.GetComponent<Animator>();
            AddAnimations();
            animator.PlayAnimation($"Idle{Name}");
        }

        /// <summary>
        /// Sørg for at en NPC ikke står i luften
        /// </summary>
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
                animator.PlayAnimation($"Idle{Name}");
            }
        }

        /// <summary>
        /// Hjælpemetode til at tjekke om et objekt står på jorden
        /// </summary>
        /// <returns></returns>
        private bool CheckGrounded()
        {
            var collider = GameObject.GetComponent<Collider>().CollisionBox;
            var rectTiles = GameWorld.Instance.LevelManager.CurrentLevel.CollisionRectangles;
            var polyTiles = GameWorld.Instance.LevelManager.CurrentLevel.CollisionPolygons;

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

                        float bottom = GameObject.Transform.Position.Y + collider.Height / 2f;

                        if (bottom >= yOnSlope - 10 && bottom <= yOnSlope + 10)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Hvis playeren interagere med en NPC, så startes dialog
        /// </summary>
        public void Interaction()
        {
            Debug.WriteLine($"Interacting with {Name}. Dialog count: {DialogLinesBeforeAccept?.Count}");

            DialogSystem dialog = GameWorld.Instance.Dialog;
            var player = GameWorld.Instance.LevelManager.CurrentLevel.GameObjects.FirstOrDefault(gameObject => gameObject.Tag == "Player");
            var inventory = player?.GetComponent<InventoryComponent>();

            if(QuestToGive != null)
            {
                if (QuestToGive.IsCompleted)
                {
                    dialog.StartDialog(DialogLinesAlreadyCompleted, Portrait);
                    return;
                }

                if (QuestToGive.IsAccepted)
                {
                    if (QuestToGive.CanComplete(inventory))
                    {
                        QuestToGive.GrantReward(inventory);
                        QuestToGive.Complete();

                        Debug.WriteLine("Quest has been completed");

                        dialog.StartDialog(DialogLinesOnCompleted, Portrait);
                        return;
                    }

                    dialog.StartDialog(DialogLinesAcceptedNotCompleted, Portrait);
                    return;
                }

                dialog.StartDialog(DialogLinesBeforeAccept, Portrait, accepted =>
                {
                    if (accepted == true)
                    {
                        QuestToGive.Accept();
                        Debug.WriteLine("Quest has been accepted");
                    }
                });

                return;
            }
        }

        /// <summary>
        /// Tilføj animationer
        /// </summary>
        private void AddAnimations()
        {
            //Load Frames
            idleDadFrames = LoadFrames("NPCs/Dad/Idle/Idle", 2);
            happyDadFrames = LoadFrames("NPCs/Dad/Happy/Happy", 2);

            //Add animations
            animator.AddAnimation(new Animation("IdleDad", 2.5f, true, idleDadFrames));
            animator.AddAnimation(new Animation("HappyDad", 2.5f, true, happyDadFrames));
        }

        /// <summary>
        /// Hjælpemetode til at indlæse Textur2D arrays
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="frameCount"></param>
        /// <returns></returns>
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
