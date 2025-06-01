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
        private Collider collider;
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
            collider = GameObject.GetComponent<Collider>();
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
            grounded = collider.CheckGrounded(GameObject);

            if (grounded == true && velocity.Y > 0)
            {
                velocity.Y = 0;
                animator.PlayAnimation($"Idle{Name}");
            }
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
        /// Tilføj animationer
        /// </summary>
        private void AddAnimations()
        {
            //Load Frames
            idleDadFrames = animator.LoadFrames("NPCs/Dad/Idle/Idle", 2);
            happyDadFrames = animator.LoadFrames("NPCs/Dad/Happy/Happy", 2);

            //Add animations
            animator.AddAnimation(new Animation("IdleDad", 2.5f, true, idleDadFrames));
            animator.AddAnimation(new Animation("HappyDad", 2.5f, true, happyDadFrames));
        }
    }
}
