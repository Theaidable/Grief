using Greif;
using Grief.Classes.Algorithms;
using Grief.Classes.DesignPatterns.Command;
using Grief.Classes.DesignPatterns.Command.Commands;
using Grief.Classes.DesignPatterns.Composite.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
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
        private Texture2D[] idleFrames;
        private Texture2D[] walkFrames;
        private Texture2D[] runFrames;

        //Jump animation frames
        private Texture2D[] jumpFrames;
        private Texture2D[] fallFrames;

        //Combat animation frames
        private Texture2D[] attackFrames;
        private Texture2D[] blinkFrames;

        //Death animation frames
        private Texture2D[] deathFrames;
        private Texture2D[] dieFrames;

        //Sit animation frames when saving the game
        private Texture2D[] sitFrames;

        //Public properties
        public float MovementSpeed { get; private set; }
        public bool Grounded { get; set; }

        public PlayerComponent(GameObject gameObject) : base(gameObject)
        {
            MovementSpeed = 100f;
            moveDirection = Vector2.One;
        }

        public override void Start()
        {
            animator = GameObject.GetComponent<Animator>();

            AddAnimations();
            BindCommands();
            animator.PlayAnimation("Idle");
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
                Debug.WriteLine($"{moveDirection}");
                PlayFallAnimation();
            }
        }
        */

        public void Move(Vector2 direction)
        {
            moveDirection = direction;
            GameObject.Transform.Translate(direction * MovementSpeed * GameWorld.Instance.DeltaTime);
            Debug.WriteLine($"{moveDirection}");
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
            //Load Frames
            idleFrames = LoadFrames("MainCharacter/Idle/Idle",2);
            walkFrames = LoadFrames("MainCharacter/Walk/Walk",4);
            runFrames = LoadFrames("MainCharacter/Run/Run",8);
            jumpFrames = LoadFrames("MainCharacter/Jump/Jump",3);
            fallFrames = LoadFrames("MainCharacter/Fall/Fall",5);
            attackFrames = LoadFrames("MainCharacter/Attack/Attack",8);
            blinkFrames = LoadFrames("MainCharacter/Blink/Blink",2);
            deathFrames = LoadFrames("MainCharacter/Death/Death",4);
            dieFrames = LoadFrames("MainCharacter/Die/Die",8);
            sitFrames = LoadFrames("MainCharacter/Sit/Sit",6);

            //Add animations
            animator.AddAnimation(new Animation("Idle", 3f, true, idleFrames));
            animator.AddAnimation(new Animation("Walk", 2.5f, true, walkFrames));
            animator.AddAnimation(new Animation("Run", 10f, true, runFrames));
            animator.AddAnimation(new Animation("Jump", 2.5f, false, jumpFrames));
            animator.AddAnimation(new Animation("Fall", 2.5f, false, fallFrames));
            animator.AddAnimation(new Animation("Attack", 10f, false, attackFrames));
            animator.AddAnimation(new Animation("Blink", 2.5f, false, blinkFrames));
            animator.AddAnimation(new Animation("Death", 2.5f, false, deathFrames));
            animator.AddAnimation(new Animation("Die", 2.5f, false, dieFrames));
            animator.AddAnimation(new Animation("Sit", 2.5f, true, sitFrames));
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

        
        private void PlayMoveAnimation(Vector2 direction)
        {
            if (direction.X < 0)
            {
                animator.PlayAnimation("Run");
            }
            else if (direction.X > 0)
            {
                animator.PlayAnimation("Run");
            }
        }

        private void PlayStopAnimation(Vector2 direction)
        {
            if (direction.X < 0)
            {
                animator.PlayAnimation("Idle");
            }
            else if (direction.X > 0)
            {
                animator.PlayAnimation("Idle");
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
                animator.PlayAnimation("Attack");
            }
            else if (moveDirection.X > 0)
            {
                animator.PlayAnimation("Attack");
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
