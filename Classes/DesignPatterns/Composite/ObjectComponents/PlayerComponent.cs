using Greif;
using Grief.Classes.DesignPatterns.Command;
using Grief.Classes.DesignPatterns.Command.Commands;
using Grief.Classes.DesignPatterns.Composite.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    public class PlayerComponent : Component
    {
        //Private fields
        private Animator animator;
        private Vector2 moveDirection;
        private Vector2 velocity;
        private float gravity = 600f;
        private float jumpForce = -200f;
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
        public int Damage { get; private set; }
        public float Health { get; private set; }
        public float MovementSpeed { get; private set; }
        public bool Grounded { get; private set; }

        public PlayerComponent(GameObject gameObject) : base(gameObject)
        {
            Damage = 25;
            Health = 100;
            MovementSpeed = 100f;
        }

        public override void Start()
        {
            animator = GameObject.GetComponent<Animator>();

            AddAnimations();
            BindCommands();
            animator.PlayAnimation("Idle");
        }

        
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

            Grounded = CheckGrounded();
            
            if (Grounded == true && velocity.Y > 0)
            {
                velocity.Y = 0;
                
                animator.PlayAnimation("Idle");
            }
            else if (Grounded == false)
            {
                if (velocity.Y < 0)
                {
                    animator.PlayAnimation("Jump");
                }
                else
                {
                    animator.PlayAnimation("Fall");
                }
            }
        }

        private bool CheckGrounded()
        {
            var collider = GameObject.GetComponent<Collider>().CollisionBox;

            return GameWorld.Instance.LevelManager.CurrentLevel.CollisionRectangles
            .Any(r => r.Left < collider.Right && r.Right > collider.Left
            && Math.Abs(collider.Bottom - r.Top) < 3);
        }


        public void Move(Vector2 direction)
        {
            SpriteRenderer spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            moveDirection = direction;
            
            //Flip sprite baseret på direction
            if(direction.X < 0)
            {
                spriteRenderer.Effects = SpriteEffects.FlipHorizontally;
            } 
            else if(direction.X > 0)
            {
                spriteRenderer.Effects = SpriteEffects.None;
            }

            Vector2 movement = direction * MovementSpeed * GameWorld.Instance.DeltaTime;
            Vector2 originalPosition = GameObject.Transform.Position;
            GameObject.Transform.Translate(movement);

            var playerCollider = GameObject.GetComponent<Collider>().PixelPerfectRectangles.Select(rect => rect.Rectangle);
            bool collision = playerCollider.Any(pc => GameWorld.Instance.LevelManager.CurrentLevel.CollisionRectangles.Any(tile => tile.Intersects(pc)));

            if(collision == true)
            {
                GameObject.Transform.Position = originalPosition;
            }
            else
            {
                if (isAttacking == false && Grounded == true)
                {
                    animator.PlayAnimation("Run");
                }
            }
        }

        public void Stop()
        {
            if(isAttacking == false && Grounded == true)
            {
                animator.PlayAnimation("Idle");
            }
        }

        public void Jump()
        {
            if (Grounded == true)
            {
                velocity.Y = jumpForce;
                Grounded = false;
            }
        }

        public void Attack()
        {
            if (cooldownTimer <= 0f)
            {
                isAttacking = true;
                animator.PlayAnimation("Attack");
                animator.ClearOnAnimationComplete();

                animator.OnAnimationComplete = () =>
                {
                    var playerCollider = GameObject.GetComponent<Collider>();
                    var level = GameWorld.Instance.LevelManager.CurrentLevel;

                    foreach (GameObject gameObjects in level.GameObjects)
                    {
                        var enemy = gameObjects.GetComponent<EnemyComponent>();
                        Collider enemyCollider = gameObjects.GetComponent<Collider>();

                        if(enemy == null || enemyCollider == null)
                        {
                            continue;
                        }

                        if(playerCollider.CollisionBox.Intersects(enemyCollider.CollisionBox) == false)
                        {
                            continue;
                        }

                        bool hit = playerCollider.PixelPerfectRectangles
                        .Any(pr => enemyCollider.PixelPerfectRectangles
                        .Any(er => pr.Rectangle.Intersects(er.Rectangle)));

                        if(hit == true)
                        {
                            enemy.TakeDamage(Damage);
                            break;
                        }
                    }

                    isAttacking = false;
                    animator.PlayAnimation(Grounded ? "Idle" : "Fall");
                };

                cooldownTimer = attackCooldown;
            }
        }

        public bool CanUseAttack()
        {
            return cooldownTimer <= 0f;
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;

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
            animator.AddAnimation(new Animation("Idle", 3.5f, true, idleFrames));
            animator.AddAnimation(new Animation("Walk", 5f, true, walkFrames));
            animator.AddAnimation(new Animation("Run", 10f, true, runFrames));
            animator.AddAnimation(new Animation("Jump", 5f, false, jumpFrames));
            animator.AddAnimation(new Animation("Fall", 10f, false, fallFrames));
            animator.AddAnimation(new Animation("Attack", 15f, false, attackFrames));
            animator.AddAnimation(new Animation("Blink", 5f, false, blinkFrames));
            animator.AddAnimation(new Animation("Death", 5f, false, deathFrames));
            animator.AddAnimation(new Animation("Die", 5f, false, dieFrames));
            animator.AddAnimation(new Animation("Sit", 5f, true, sitFrames));
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

        private void BindCommands()
        {
            //Move on A and D
            InputHandler.Instance.AddUpdateCommand(Keys.A, new MoveCommand(new Vector2(-1, 0), this));
            InputHandler.Instance.AddUpdateCommand(Keys.D, new MoveCommand(new Vector2(1, 0), this));

            //Jump on Spacebar
            InputHandler.Instance.AddButtonDownCommand(Keys.Space, new JumpCommand(this));

            //Idle hvis man stopper med at gå eller man begynder at falde
            InputHandler.Instance.AddButtonUpCommand(Keys.A, new StopCommand(this));
            InputHandler.Instance.AddButtonUpCommand(Keys.D, new StopCommand(this));

            //Attack med musen
            InputHandler.Instance.AddMouseButtonDownCommand(MouseButton.Left, new AttackCommand(this));
        }
    }
}
