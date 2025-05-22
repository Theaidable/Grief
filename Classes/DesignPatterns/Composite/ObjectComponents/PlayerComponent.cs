using Greif;
using Grief.Classes.DesignPatterns.Command;
using Grief.Classes.DesignPatterns.Command.Commands;
using Grief.Classes.DesignPatterns.Composite.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.CompilerServices;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    public class PlayerComponent : Component
    {
        //Private fields
        private Animator animator;
        private Vector2 moveDirection;
        private Vector2 velocity;
        private float gravity = 800f;
        private float jumpForce = -400f;
        private float attackCooldown = 0.1f;
        private float cooldownTimer = 0f;
        private bool isAttacking = false;

        //Walk animation frames
        private Texture2D[] walkLeftFrames;
        private Texture2D[] walkRightFrames;

        //Jump animation frames
        private Texture2D[] jumpFrames;
        private Texture2D[] fallFrames;

        //Public properties
        public float MovementSpeed { get; private set; }
        public bool Grounded { get; set; }

        public PlayerComponent(GameObject gameObject) : base(gameObject)
        {
            MovementSpeed = 200f;
            moveDirection = Vector2.One;
        }

        public override void Start()
        {
            animator = GameObject.GetComponent<Animator>();
            AddAnimations();
            BindCommands();
            animator.PlayAnimation("IdleRight");
        }

        /*
        public override void Update()
        {
            if (cooldownTimer > 0)
            {
                cooldownTimer -= GameWorld.Instance.DeltaTime;
            }

            if (Grounded == false)
            {
                velocity.Y += gravity * GameWorld.Instance.DeltaTime;
            }

            //Bevæg spilleren baseret på velocity
            GameObject.Transform.Translate(new Vector2(0, velocity.Y * GameWorld.Instance.DeltaTime));

            //Her skal vi lave et collisoncheck om player står på jorden, før man kan hoppe. Dette skal implementeres når levelet er lavet
            //Dette er udelukkende for at jeg ved hvordan collision i levelet fungere

            if (velocity.Y > 0)
            {
                PlayFallAnimation();
            }
        }
        */

        public void Move(Vector2 direction)
        {
            moveDirection = direction;
            GameObject.Transform.Translate(direction * MovementSpeed * GameWorld.Instance.DeltaTime);
            PlayMoveAnimation(direction);
        }

        public void Stop()
        {
            PlayStopAnimation(moveDirection);
        }

        public void Jump()
        {
            if (Grounded == true)
            {
                velocity.Y = jumpForce;
                Grounded = false;
                PlayJumpAnimation();
            }
        }

        public void Attack()
        {
            //Her skal vi lave logikken for players attack

            if (cooldownTimer <= 0f)
            {
                animator.ClearOnAnimationComplete();
                isAttacking = true;
                PlayAttackAnimation();

                animator.OnAnimationComplete = () =>
                {
                    cooldownTimer = attackCooldown;
                    Stop();
                    isAttacking = false;
                };
            }
        }

        public bool CanUseAttack()
        {
            return cooldownTimer <= 0f;
        }

        private void AddAnimations()
        {
            /*
             * WalkFrames
             * walkLeftFrames = LoadFrames("stien for at finde den sprite som er walkingLeft", antal frames);
             * walkRightFrames = LoadFrames("stien for at finde den sprite som er walkingRight", antal frames);
             * 
             * IdleFrames
             * idleLeftFrames = LoadFrames("stien for at finde den sprite som er idleLeft", antal frames);
             * idleRightFrames = LoadFrames("stien for at finde den sprite som er idleRight", antal frames);
             * 
             * Jump and Fall Frames
             * jumpFrames = LoadFrames("stien for at finde den sprite som er jump", antal frames);
             * fallFrames = LoadFrames("stien for at finde den sprite som er fall", antal frames);
             * 
             * AttackFrames
             * attackLeftFrames = LoadFrames("stien for at finde den sprite som er attackLeft", antal frames);
             * attackRightFrames = LoadFrames("stien for at finde den sprite som er attackRight", antal frames);
             * 
             * AddAnimations
             * animator.AddAnimation(new Animation("WalkLeft", 2.5f, true, walkLeftFrames));
             * animator.AddAnimation(new Animation("WalkRight", 2.5f, true, walkRightFrames));
             * animator.AddAnimation(new Animation("IdleLeft", 2.5f, true, idleLeftFrames));
             * animator.AddAnimation(new Animation("IdleRight", 2.5f, true, idleRightFrames));
             * animator.AddAnimation(new Animation("Jump", 2.5f, false, jumpFrames));
             * animator.AddAnimation(new Animation("Fall", 2.5f, false, fallFrames));
             * animator.AddAnimation(new Animation("AttackLeft", 2.5f, false, attackLeftFrames));
             * animator.AddAnimation(new Animation("AttackRight", 2.5f, false, attackRightFrames));
             */
        }

        private Texture2D[] LoadFrames(string basePath, int frameCount)
        {
            Texture2D[] frames = new Texture2D[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                frames[i] = GameWorld.Instance.Content.Load<Texture2D>($"{basePath}_00{i}"); //Sørg for at dette svare korrekt til stinavn
            }
            return frames;
        }

        private void PlayMoveAnimation(Vector2 direction)
        {
            if (direction.X < 0)
            {
                animator.PlayAnimation("WalkLeft");
            }
            else if (direction.X > 0)
            {
                animator.PlayAnimation("WalkRight");
            }
        }

        private void PlayStopAnimation(Vector2 direction)
        {
            if (direction.X < 0)
            {
                animator.PlayAnimation("IdleLeft");
            }
            else if (direction.X > 0)
            {
                animator.PlayAnimation("IdleRight");
            }
        }

        private void PlayJumpAnimation()
        {
            animator.ClearOnAnimationComplete();

            if (moveDirection.Y < 0)
            {
                animator.PlayAnimation("Jump");
            }
        }

        private void PlayFallAnimation()
        {
            animator.ClearOnAnimationComplete();

            if (moveDirection.Y > 0)
            {
                animator.PlayAnimation("Fall");
            }
        }

        private void PlayAttackAnimation()
        {
            animator.ClearOnAnimationComplete();

            if (moveDirection.X < 0)
            {
                animator.PlayAnimation("AttackLeft");
            }
            else if (moveDirection.X > 0)
            {
                animator.PlayAnimation("AttackRight");
            }

            //Hvis vi vil tilføje angreb op og ned, så skal det være i forhold til moveDirection.Y større eller mindre end 0
        }

        private void BindCommands()
        {
            //Move on A and D
            InputHandler.Instance.AddUpdateCommand(Keys.A, new MoveCommand(new Vector2(-1, 0), this));
            InputHandler.Instance.AddUpdateCommand(Keys.D, new MoveCommand(new Vector2(1, 0), this));

            //Jump on W or Spacebar
            InputHandler.Instance.AddUpdateCommand(Keys.W, new JumpCommand(this));
            InputHandler.Instance.AddUpdateCommand(Keys.Space, new JumpCommand(this));

            //Idle hvis man stopper med at gå eller man begynder at falde
            InputHandler.Instance.AddButtonUpCommand(Keys.A, new StopCommand(this));
            InputHandler.Instance.AddButtonUpCommand(Keys.D, new StopCommand(this));

            //Attack med musen
            InputHandler.Instance.AddMouseButtonDownCommand(MouseButton.Left, new AttackCommand(this));
        }
    }
}
