using Greif;
using Grief.Classes.DesignPatterns.Command;
using Grief.Classes.DesignPatterns.Command.Commands;
using Grief.Classes.DesignPatterns.Composite.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Shapes;
using System;
using System.Diagnostics;
using System.Linq;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    /// <summary>
    /// Player component
    /// </summary>
    public class PlayerComponent : Component
    {
        //Private fields
        private Animator animator;
        private Collider collider;
        private InventoryComponent inventory;
        private Vector2 moveDirection;
        private Vector2 velocity;
        private float gravity = 600f;
        private float jumpForce = -200f;
        private float attackCooldown = 0.1f;
        private float interactionCooldown = 0.1f;
        private float cooldownTimer = 0f;
        private bool isAttacking = false;
        private bool isInteracting = false;
        private bool grounded;

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
        public float Health { get; set; }
        public float MovementSpeed { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameObject"></param>
        public PlayerComponent(GameObject gameObject) : base(gameObject)
        {
            Damage = 25;
            Health = 100;
            MovementSpeed = 100f;
        }

        /// <summary>
        /// Find komponenter og bind commands
        /// </summary>
        public override void Start()
        {
            animator = GameObject.GetComponent<Animator>();
            collider = GameObject.GetComponent<Collider>();
            inventory = GameObject.GetComponent<InventoryComponent>();
            AddAnimations();
            BindCommands();
            animator.PlayAnimation("Idle");
        }

        /// <summary>
        /// Physics udregninger, som gør at spilleren kan falde
        /// </summary>
        public override void Update()
        {
            if (cooldownTimer > 0)
            {
                cooldownTimer -= GameWorld.Instance.DeltaTime;
            }

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
                animator.PlayAnimation("Idle");
            }
            else if (grounded == false)
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

        /// <summary>
        /// Metode til at bevæge spilleren
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Vector2 direction)
        {
            if (inventory.ShowInventory == true)
            {
                return;
            }

            SpriteRenderer spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            moveDirection = direction;

            //Flip sprite baseret på direction
            if (direction.X < 0)
            {
                spriteRenderer.SetEffects(SpriteEffects.FlipHorizontally);
            }
            else if (direction.X > 0)
            {
                spriteRenderer.SetEffects(SpriteEffects.None);
            }

            Vector2 originalPosition = GameObject.Transform.Position;
            Vector2 movement = direction * MovementSpeed * GameWorld.Instance.DeltaTime;

            GameObject.Transform.Translate(movement);

            //AABB
            var playerCollider = GameObject.GetComponent<Collider>().CollisionBox;
            bool rectangleCollision = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.CollisionRectangles.Any(tile => tile.Intersects(playerCollider));
            bool polygonCollision = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.CollisionPolygons.Any(poly => poly.BoundingRectangle.Intersects(playerCollider));
            bool snappedToSlope = false;

            if (rectangleCollision == true && polygonCollision == false)
            {
                GameObject.Transform.Position = originalPosition;
            }

            if (polygonCollision == true)
            {
                foreach (Polygon polygon in GameWorld.Instance.GameManager.LevelManager.CurrentLevel.CollisionPolygons)
                {
                    var points = polygon.Vertices;

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

                        float playerX = playerCollider.Center.X;

                        if (playerX >= p1.X && playerX <= p2.X)
                        {
                            float slope = (p2.Y - p1.Y) / (p2.X - p1.X);
                            float yOnSlope = p1.Y + slope * (playerX - p1.X);

                            float playerBottom = GameObject.Transform.Position.Y + playerCollider.Height / 2f;

                            if (playerBottom >= yOnSlope - 10 && playerBottom <= yOnSlope + 10)
                            {
                                GameObject.Transform.Position = new Vector2(GameObject.Transform.Position.X, yOnSlope - playerCollider.Height / 2f);
                                snappedToSlope = true;
                                break;
                            }
                        }
                    }

                    if (snappedToSlope == true)
                    {
                        break;
                    }
                }
            }

            if (isAttacking == false && grounded == true)
            {
                animator.PlayAnimation("Run");
            }
        }

        /// <summary>
        /// Metode til at sørge for at spilleren går tilbage til idle ved stop af bevægelse
        /// </summary>
        public void Stop()
        {
            if (isAttacking == false && grounded == true)
            {
                animator.PlayAnimation("Idle");
            }
        }

        /// <summary>
        /// Metode til at få spilleren til at hoppe
        /// </summary>
        public void Jump()
        {
            if (grounded == true && inventory.ShowInventory == false)
            {
                velocity.Y = jumpForce;
                grounded = false;
            }
        }

        /// <summary>
        /// Metode til at få spilleren til at angribe
        /// </summary>
        public void Attack()
        {
            if (cooldownTimer <= 0f && inventory.ShowInventory == false)
            {
                isAttacking = true;
                animator.PlayAnimation("Attack");
                animator.ClearOnAnimationComplete();

                animator.OnAnimationComplete = () =>
                {
                    var playerCollider = GameObject.GetComponent<Collider>();
                    var level = GameWorld.Instance.GameManager.LevelManager.CurrentLevel;

                    foreach (GameObject gameObjects in level.GameObjects)
                    {
                        var enemy = gameObjects.GetComponent<EnemyComponent>();
                        Collider enemyCollider = gameObjects.GetComponent<Collider>();

                        if (enemy == null || enemyCollider == null)
                        {
                            continue;
                        }

                        if (playerCollider.CollisionBox.Intersects(enemyCollider.CollisionBox) == false)
                        {
                            continue;
                        }

                        bool hit = playerCollider.PixelPerfectRectangles
                        .Any(pr => enemyCollider.PixelPerfectRectangles
                        .Any(er => pr.Rectangle.Intersects(er.Rectangle)));

                        if (hit == true)
                        {
                            enemy.TakeDamage(Damage);
                            break;
                        }
                    }

                    isAttacking = false;
                    animator.PlayAnimation(grounded ? "Idle" : "Fall");
                };

                cooldownTimer = attackCooldown;
            }
        }

        /// <summary>
        /// Boolean som bruges til at finde ud af om man må angribe
        /// </summary>
        /// <returns></returns>
        public bool CanUseAttack()
        {
            return cooldownTimer <= 0f;
        }

        /// <summary>
        /// Spillerens interaction med andre objekter
        /// </summary>
        public void Interact()
        {
            if (cooldownTimer <= 0f && inventory.ShowInventory == false && grounded == true && isAttacking == false)
            {
                isInteracting = true;

                var nearbyNpc = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.GameObjects
                    .FirstOrDefault(gameObject => Vector2.Distance(gameObject.Transform.Position, GameObject.Transform.Position) < 40
                    && gameObject.GetComponent<NpcComponent>() != null);

                var nearbyItem = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.GameObjects
                    .FirstOrDefault(gameObject => Vector2.Distance(gameObject.Transform.Position, GameObject.Transform.Position) < 40
                    && gameObject.GetComponent<ItemComponent> != null);
                Debug.WriteLine(nearbyItem != null, "Nearby Item blev fundet korrekt");

                if (nearbyNpc != null)
                {
                    nearbyNpc.GetComponent<NpcComponent>().Interaction();
                }
                else if (nearbyItem != null)
                {
                    var itemComp = nearbyItem.GetComponent<ItemComponent>();
                    Debug.WriteLine(itemComp != null, "ItemComponent blev hentet korrekt");

                    if (itemComp != null)
                    {
                        itemComp.PickUpItem();
                    }
                    else
                    {
                        Debug.WriteLine("Fejl med at få ItemComponent");
                    }
                }

                cooldownTimer = interactionCooldown;
            }
        }

        /// <summary>
        /// Metode til at finde ud af om spilleren kan interagere med et objekt
        /// </summary>
        /// <returns></returns>
        public bool CanInteract()
        {
            return cooldownTimer <= 0f;
        }

        /// <summary>
        /// Metode til at få spilleren til at tage skade
        /// </summary>
        /// <param name="amount"></param>
        public void TakeDamage(int amount)
        {
            Health -= amount;

        }

        /// <summary>
        /// Tilføj animationer
        /// </summary>
        private void AddAnimations()
        {
            //Load Frames
            idleFrames = animator.LoadFrames("MainCharacter/Idle/Idle", 2);
            walkFrames = animator.LoadFrames("MainCharacter/Walk/Walk", 4);
            runFrames = animator.LoadFrames("MainCharacter/Run/Run", 8);
            jumpFrames = animator.LoadFrames("MainCharacter/Jump/Jump", 3);
            fallFrames = animator.LoadFrames("MainCharacter/Fall/Fall", 5);
            attackFrames = animator.LoadFrames("MainCharacter/Attack/Attack", 8);
            blinkFrames = animator.LoadFrames("MainCharacter/Blink/Blink", 2);
            deathFrames = animator.LoadFrames("MainCharacter/Death/Death", 4);
            dieFrames = animator.LoadFrames("MainCharacter/Die/Die", 8);
            sitFrames = animator.LoadFrames("MainCharacter/Sit/Sit", 6);

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


        /// <summary>
        /// Bind de forskellige commands til forskellige knapper
        /// </summary>
        private void BindCommands()
        {
            //UpdateCommands
            InputHandler.Instance.AddUpdateCommand(Keys.A, new MoveCommand(new Vector2(-1, 0), this));
            InputHandler.Instance.AddUpdateCommand(Keys.D, new MoveCommand(new Vector2(1, 0), this));

            //ButtonDown Commands
            InputHandler.Instance.AddButtonDownCommand(Keys.Space, new JumpCommand(this));
            InputHandler.Instance.AddButtonDownCommand(Keys.E, new InteractionCommand(this));
            InputHandler.Instance.AddButtonDownCommand(Keys.I, new OpenInventoryCommand(inventory));

            //ButtonUp Commands
            InputHandler.Instance.AddButtonUpCommand(Keys.A, new StopCommand(this));
            InputHandler.Instance.AddButtonUpCommand(Keys.D, new StopCommand(this));

            //MouseCommands
            InputHandler.Instance.AddMouseButtonDownCommand(MouseButton.Left, new AttackCommand(this));
        }
    }
}
