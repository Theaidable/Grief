using Greif;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.Dialog;
using Grief.Classes.Quests;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    /// <summary>
    /// NPC component – styrer dialog, quests og grundlæggende NPC-animation.
    /// </summary>
    /// <author>Asbjørn Ryberg</author>
    /// <author>David Gudmund Danielsen</author>
    public class NpcComponent : Component
    {
        // Fields
        private Animator animator;
        private Collider collider;
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
            // Brug Transform.Velocity for at arbejde sammen med Collider-systemet
            if (!grounded)
            {
                GameObject.Transform.Velocity += new Vector2(0, gravity * GameWorld.Instance.DeltaTime);
            }

            // Bevæg NPC baseret på velocity
            Vector2 originalPosition = GameObject.Transform.Position;
            Vector2 movement = new Vector2(0, GameObject.Transform.Velocity.Y * GameWorld.Instance.DeltaTime);
            GameObject.Transform.Translate(movement);

            grounded = collider != null && collider.CheckGrounded(GameObject);

            if (grounded && GameObject.Transform.Velocity.Y > 0)
            {
                GameObject.Transform.Velocity = new Vector2(GameObject.Transform.Velocity.X, 0);
                animator.PlayAnimation($"Idle{Name}");
            }
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

                        animator.PlayAnimation($"Happy{Name}");
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
