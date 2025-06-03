using Greif;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.Dialog;
using Grief.Classes.Quests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    /// <summary>
    /// NPC component – styrer dialog, quests og grundlæggende NPC-animation.
    /// </summary>
    public class NpcComponent : Component
    {
        // Fields
        private Animator animator;
        private Collider collider;
        private Vector2 velocity;
        private float gravity = 600f;
        private bool grounded;

        // Animation frames
        private Texture2D[] idleDadFrames;
        private Texture2D[] happyDadFrames;

        // Properties
        /// <summary>
        /// NPC’ens navn
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Dialog før quest er accepteret
        /// </summary>
        public List<string> DialogLinesBeforeAccept { get; set; }

        /// <summary>
        /// Dialog efter quest er accepteret, men ikke gennemført
        /// </summary>
        public List<string> DialogLinesAcceptedNotCompleted { get; set; }

        /// <summary>
        /// Dialog når quest er gennemført
        /// </summary>
        public List<string> DialogLinesOnCompleted { get; set; }

        /// <summary>
        /// Dialog hvis quest allerede er gennemført
        /// </summary>
        public List<string> DialogLinesAlreadyCompleted { get; set; }

        /// <summary>
        /// Den quest NPC’en kan give
        /// </summary>
        public Quest QuestToGive { get; set; }

        /// <summary>
        /// NPC’ens portræt (til dialogboks)
        /// </summary>
        public Texture2D Portrait { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameObject"></param>
        public NpcComponent(GameObject gameObject) : base(gameObject) { }

        /// <summary>
        /// Tilføj animationer ved start.
        /// </summary>
        public override void Start()
        {
            animator = GameObject.GetComponent<Animator>();
            collider = GameObject.GetComponent<Collider>();
            AddAnimations();
            animator.PlayAnimation($"Idle{Name}");
        }

        /// <summary>
        /// Opdater NPC: gravity og grounding.
        /// </summary>
        public override void Update()
        {
            if (!grounded)
            {
                velocity.Y += gravity * GameWorld.Instance.DeltaTime;
            }

            // Bevæg NPC baseret på velocity
            Vector2 originalPosition = GameObject.Transform.Position;
            Vector2 movement = new Vector2(0, velocity.Y * GameWorld.Instance.DeltaTime);
            GameObject.Transform.Translate(movement);

            grounded = collider != null
                ? collider.CheckGrounded(GameObject)
                : CheckGrounded();

            if (grounded && velocity.Y > 0)
            {
                velocity.Y = 0;
                animator.PlayAnimation($"Idle{Name}");
            }
        }

        /// <summary>
        /// Alternativ collision-tjek hvis ikke collider.CheckGrounded er implementeret.
        /// </summary>
        /// <returns></returns>
        private bool CheckGrounded()
        {
            var npcCollider = GameObject.GetComponent<Collider>()?.CollisionBox;
            if (npcCollider == null) return false;

            var rectTiles = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.CollisionRectangles;
            var polyTiles = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.CollisionPolygons;

            foreach (var tile in rectTiles)
            {
                bool isAbove = npcCollider.Bottom <= tile.Top + 5;
                bool isFallingOnto = npcCollider.Bottom + velocity.Y * GameWorld.Instance.DeltaTime >= tile.Top;
                bool horizontalOverlap = npcCollider.Right > tile.Left && npcCollider.Left < tile.Right;

                if (isAbove && isFallingOnto && horizontalOverlap)
                {
                    GameObject.Transform.Position = new Vector2(GameObject.Transform.Position.X, tile.Top - npcCollider.Height / 2f);
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
                    if (Math.Abs(p1.X - p2.X) < 1f) continue;
                    if (p1.X > p2.X)
                    {
                        var temp = p1; p1 = p2; p2 = temp;
                    }
                    float npcX = npcCollider.Center.X;
                    if (npcX >= p1.X && npcX <= p2.X)
                    {
                        float slope = (p2.Y - p1.Y) / (p2.X - p1.X);
                        float yOnSlope = p1.Y + slope * (npcX - p1.X);

                        float npcBottom = GameObject.Transform.Position.Y + npcCollider.Height / 2f;
                        if (npcBottom >= yOnSlope - 10 && npcBottom <= yOnSlope + 10)
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Spilleren interagerer med NPC (starter dialog, giver quest, mv.)
        /// </summary>
        public void Interaction()
        {
            Debug.WriteLine($"Interacting with {Name}. Dialog count: {DialogLinesBeforeAccept?.Count}");

            DialogSystem dialog = GameWorld.Instance.Dialog;
            var player = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.GameObjects.FirstOrDefault(gameObject => gameObject.Tag == "Player");
            var inventory = player?.GetComponent<InventoryComponent>();

            if (QuestToGive != null)
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
        /// Tilføj animationer til NPC’en.
        /// </summary>
        private void AddAnimations()
        {
            //Load Frames (eksempel: “Dad” – tilpas hvis du har flere NPC-varianter)
            idleDadFrames = animator.LoadFrames("NPCs/Dad/Idle/Idle", 2);
            happyDadFrames = animator.LoadFrames("NPCs/Dad/Happy/Happy", 2);

            animator.AddAnimation(new Animation("IdleDad", 2.5f, true, idleDadFrames));
            animator.AddAnimation(new Animation("HappyDad", 2.5f, true, happyDadFrames));
        }
    }
}
