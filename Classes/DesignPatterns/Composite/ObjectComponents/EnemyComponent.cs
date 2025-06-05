using Greif;
using Grief.Classes.Algorithms;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Factories.ObjectFactories;
using Grief.Classes.Enemy;
using Grief.Classes.GameManager;
using Grief.Classes.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    /// <summary>
    /// Enemy component – styrer AI, bevægelse og kamp for fjender.
    /// </summary>
    public class EnemyComponent : Component
    {
        // Fields
        private bool isHurt;
        private float attackCooldown = 1f;
        private float cooldownTimer = 0f;
        private bool isAttacking;
        private float gravity = 600f;
        private bool grounded;
        private Item droppedItem;

        private int currentPatrolIndex = 0;
        private bool patrolForward;

        private Animator animator;
        private Collider collider;
        private AStar astar;

        private Texture2D[] idleFrames;
        private Texture2D[] walkFrames;
        private Texture2D[] attackFrames;
        private Texture2D[] hurtFrames;
        private Texture2D[] deathFrames;

        private List<Vector2> path = new List<Vector2>();
        private readonly object pathLock = new object();
        private float recalculatePathTimer = 0f;

        // Properties
        public int EnemyHealth { get; private set; }
        public int EnemyDamage { get; private set; }
        public float EnemySpeed { get; private set; }
        public int EnemyDetectionRange { get; private set; }
        public int EnemyAttackRange { get; private set; }
        public Point EnemyColliderSize { get; private set; }
        public List<Vector2> PatrolPoints { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameObject"></param>
        public EnemyComponent(GameObject gameObject) : base(gameObject) { }

        /// <summary>
        /// Find animator/collider, sæt op til start.
        /// </summary>
        public override void Start()
        {
            animator = GameObject.GetComponent<Animator>();
            collider = GameObject.GetComponent<Collider>();
            AddAnimations();
            animator.PlayAnimation("Idle");
        }

        /// <summary>
        /// Sæt PathFinder og andre afhængigheder, når alt er loadet (bruges især hvis fjender skal bruge tilemap navigation).
        /// </summary>
        public override void LateStart()
        {
            var level = GameWorld.Instance.GameManager?.LevelManager?.CurrentLevel;

            if (level?.PathFinder == null)
            {
                Debug.WriteLine("ERROR (LateStart): PathFinder or CurrentLevel is null in EnemyComponent.");
                return;
            }

            astar = level.PathFinder;
            Debug.WriteLine("astar successfully assigned in LateStart.");
        }

        /// <summary>
        /// Sæt en enemies stats.
        /// </summary>
        /// <param name="stats"></param>
        public void SetStats(EnemyStats stats)
        {
            EnemyHealth = stats.Health;
            EnemyDamage = stats.Damage;
            EnemySpeed = stats.Speed;
            EnemyDetectionRange = stats.DetectionRange;
            EnemyAttackRange = stats.AttackRange;
            EnemyColliderSize = stats.ColliderSize;

            if (collider != null)
            {
                collider.ColliderSize = EnemyColliderSize;
            }
        }

        /// <summary>
        /// Styrer in-game logik for fjenden (bevægelser, angreb, patruljering).
        /// </summary>
        public override void Update()
        {
            // Opdater cooldownTimer
            if (cooldownTimer > 0)
            {
                cooldownTimer -= GameWorld.Instance.DeltaTime;
            }

            // Tilføj gravity via Transform.Velocity hvis ikke grounded
            if (grounded == false)
            {
                GameObject.Transform.Velocity += new Vector2(0, gravity * GameWorld.Instance.DeltaTime);
            }

            // Hvis enemy angriber, så stop alle andre handlinger
            if (isAttacking == true)
            {
                return;
            }

            //Opdatér position baseret på Y-velocity
            Vector2 movement = new Vector2(0, GameObject.Transform.Velocity.Y * GameWorld.Instance.DeltaTime);
            GameObject.Transform.Translate(movement);

            //Tjek om enemy er grounded via Collider
            grounded = collider != null && collider.CheckGrounded(GameObject);

            //Stop nedadgående bevægelse hvis grounded
            if (grounded == true && GameObject.Transform.Velocity.Y > 0)
            {
                GameObject.Transform.Velocity = new Vector2(GameObject.Transform.Velocity.X, 0);
            }
            else if (grounded == false)
            {
                animator.PlayAnimation("Idle"); //Mulighed for jump/fall animation
            }

            //Fjendens AI-logik: pursue, attack, patrol
            if (EnemyHealth > 0 && isHurt == false && grounded == true)
            {
                if (PlayerIsWithInDetectionRange() == true)
                {
                    Pursue();

                    if (PlayerIsWithInAttackRange() == true)
                    {
                        Attack();
                    }
                }
                else
                {
                    path.Clear();
                    Patrol();
                }
            }
        }


        /// <summary>
        /// Patruljer mellem forudbestemte punkter.
        /// </summary>
        private void Patrol()
        {
            // Hvis ikke der er nogen punkter, så stå idle
            if (PatrolPoints == null || PatrolPoints.Count < 2)
            {
                animator.PlayAnimation("Idle");
                return;
            }

            Vector2 target = PatrolPoints[currentPatrolIndex];
            Vector2 position = GameObject.Transform.Position;
            Vector2 direction = target - position;

            if (direction.Length() < 4f)
            {
                if (patrolForward)
                {
                    currentPatrolIndex++;
                    if (currentPatrolIndex >= PatrolPoints.Count)
                    {
                        currentPatrolIndex = 0;
                        patrolForward = false;
                    }
                }
                else
                {
                    currentPatrolIndex--;
                    if (currentPatrolIndex < 0)
                    {
                        currentPatrolIndex = 1;
                        patrolForward = true;
                    }
                }
                target = PatrolPoints[currentPatrolIndex];
                direction = target - position;
            }

            EnemySpeed = 40;
            direction.Normalize();
            Move(direction);
        }

        /// <summary>
        /// Brug A* til at jagte spilleren.
        /// </summary>
        private void Pursue()
        {
            var level = GameWorld.Instance.GameManager.LevelManager.CurrentLevel;
            var player = level.GameObjects.FirstOrDefault(g => g.GetComponent<PlayerComponent>() != null);

            if (player == null)
            {
                return;
            }

            Point start = new Point((int)(GameObject.Transform.Position.X / level.Map.TileWidth), (int)(GameObject.Transform.Position.Y / level.Map.TileHeight));
            Point goal = new Point((int)(player.Transform.Position.X / level.Map.TileWidth), (int)(player.Transform.Position.Y / level.Map.TileHeight));

            if (path == null || path.Count == 0 || recalculatePathTimer <= 0f)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    var newPath = astar.FindPath(start, goal);

                    lock (pathLock)
                    {
                        path = newPath.Select(t => new Vector2(t.Position.X * level.Map.TileWidth + level.Map.TileWidth / 2, GameObject.Transform.Position.Y)).ToList();
                    }
                });

                recalculatePathTimer = 1f;
            }

            lock (pathLock)
            {
                if (path != null && path.Count > 0)
                {
                    Vector2 next = path[0];
                    Vector2 direction = next - GameObject.Transform.Position;

                    if (direction.Length() < 4f)
                    {
                        path.RemoveAt(0);
                    }
                    else
                    {
                        EnemySpeed = 60;
                        direction.Normalize();
                        Move(direction);
                    }
                }
            }

            recalculatePathTimer -= GameWorld.Instance.DeltaTime;
        }

        /// <summary>
        /// Flytter fjenden i retning af en vektor, tjekker collisions.
        /// </summary>
        /// <param name="direction"></param>
        private void Move(Vector2 direction)
        {
            // Flip sprite baseret på direction
            SpriteRenderer spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
            if (direction.X < 0)
            {
                spriteRenderer.SetEffects(SpriteEffects.FlipHorizontally);
            }
            else if (direction.X > 0)
            {
                spriteRenderer.SetEffects(SpriteEffects.None);
            }

            Vector2 originalPosition = GameObject.Transform.Position;
            Vector2 movement = direction * EnemySpeed * GameWorld.Instance.DeltaTime;
            GameObject.Transform.Translate(movement);

            var enemyCollider = collider.CollisionBox;
            bool rectCollision = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.CollisionRectangles.Any(tile => tile.Intersects(enemyCollider));
            bool polygonCollision = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.CollisionPolygons.Any(poly => poly.BoundingRectangle.Intersects(enemyCollider));
            bool snappedToSlope = false;

            if (rectCollision == true && polygonCollision == false)
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
                            var temp = p1; p1 = p2; p2 = temp;
                        }

                        float enemyX = enemyCollider.Center.X;
                        
                        if (enemyX >= p1.X && enemyX <= p2.X)
                        {
                            float slope = (p2.Y - p1.Y) / (p2.X - p1.X);
                            float yOnSlope = p1.Y + slope * (enemyX - p1.X);
                            float enemyBottom = GameObject.Transform.Position.Y + enemyCollider.Height / 2f;
                            
                            if (enemyBottom >= yOnSlope - 15 && enemyBottom <= yOnSlope + 15)
                            {
                                GameObject.Transform.Position = new Vector2(GameObject.Transform.Position.X, yOnSlope - enemyCollider.Height / 2f);
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
                animator.PlayAnimation("Walk");
            }
        }

        /// <summary>
        /// Tjek om spilleren er inden for detection range.
        /// </summary>
        /// <returns></returns>
        private bool PlayerIsWithinDetectionRange()
        {
            var player = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.GameObjects.FirstOrDefault(g => g.GetComponent<PlayerComponent>() != null);
            
            if (player == null)
            {
                return false;
            }

            var distance = Vector2.Distance(GameObject.Transform.Position, player.Transform.Position);
            return distance <= EnemyDetectionRange;
        }

        /// <summary>
        /// Angrib spilleren, hvis muligt (cooldown, animation).
        /// </summary>
        private void Attack()
        {
            if (isAttacking == true)
            {
                return;
            }

            if (cooldownTimer <= 0f)
            {
                isAttacking = true;
                animator.PlayAnimation("Attack");
                SoundManager.PlayEnemyAttackSound();
                animator.ClearOnAnimationComplete();

                animator.OnAnimationComplete = () =>
                {
                    var enemyCollider = GameObject.GetComponent<Collider>();
                    var level = GameWorld.Instance.GameManager.LevelManager.CurrentLevel;

                    foreach (GameObject gameObjects in level.GameObjects)
                    {
                        var player = gameObjects.GetComponent<PlayerComponent>();
                        Collider playerCollider = gameObjects.GetComponent<Collider>();

                        if (player == null || playerCollider == null)
                        {
                            continue;
                        }

                        if (enemyCollider.CollisionBox.Intersects(playerCollider.CollisionBox) == false)
                        {
                            continue;
                        }

                        bool hit = enemyCollider.PixelPerfectRectangles
                            .Any(pr => playerCollider.PixelPerfectRectangles
                            .Any(er => pr.Rectangle.Intersects(er.Rectangle)));

                        if (hit)
                        {
                            player.TakeDamage(EnemyDamage);
                            break;
                        }
                    }

                    isAttacking = false;
                    animator.PlayAnimation("Idle");
                };

                cooldownTimer = attackCooldown;
            }
        }

        /// <summary>
        /// Tjek om spilleren er inden for attack range.
        /// </summary>
        /// <returns></returns>
        private bool PlayerIsWithInAttackRange()
        {
            var player = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.GameObjects.FirstOrDefault(g => g.GetComponent<PlayerComponent>() != null);
            
            if (player == null)
            {
                return false;
            }
                

            var distance = Vector2.Distance(GameObject.Transform.Position, player.Transform.Position);
            return distance <= EnemyAttackRange;
        }

        /// <summary>
        /// Fjenden tager skade – spiller animation, fjerner enemy hvis health <= 0, evt. drop item.
        /// </summary>
        /// <param name="amount"></param>
        public void TakeDamage(int amount)
        {
            EnemyHealth -= amount;

            if (EnemyHealth > 0)
            {
                isHurt = true;
                animator.PlayAnimation("Hurt");
                animator.ClearOnAnimationComplete();
                animator.OnAnimationComplete = () =>
                {
                    isHurt = false;
                    isAttacking = false;
                    animator.PlayAnimation("Idle");
                };
            }
            else
            {
                animator.PlayAnimation("Death");
                animator.OnAnimationComplete = () =>
                {
                    GameWorld.Instance.GameManager.LevelManager.CurrentLevel.QueueRemove(GameObject);

                    if (droppedItem != null)
                    {

                        if(droppedItem.Texture != null)
                        {
                            GameWorld.Instance.GameManager.LevelManager.CurrentLevel.QueueAdd(ItemFactory.Instance.Create(droppedItem, GameObject.Transform.Position + new Vector2(0, 20)));
                        }
                        else
                        {
                            DropItem(droppedItem);
                        }
                    }
                };
            }
        }

        /// <summary>
        /// Sæt hvilket item der skal droppes ved fjendens død.
        /// </summary>
        /// <param name="item"></param>
        public void SetDropItem(Item item)
        {
            droppedItem = item;
        }

        /// <summary>
        /// Tilføjer direkte et item til spillerens inventory (spawner det ikke i verdenen).
        /// </summary>
        /// <param name="item"></param>
        private void DropItem(Item item)
        {
            GameObject playerObject = GameWorld.Instance.GameManager.LevelManager.CurrentLevel.GameObjects.FirstOrDefault(gameObject => gameObject.GetComponent<PlayerComponent>() != null);

            if (playerObject == null)
            {
                return;
            }

            var inventory = playerObject.GetComponent<InventoryComponent>();
            
            if (inventory == null)
            {
                return;
            }

            inventory.AddItemToInventory(item);
        }

        /// <summary>
        /// Tilføj animationer til fjenden.
        /// </summary>
        private void AddAnimations()
        {
            idleFrames = animator.LoadFrames("Enemies/Skeleton/Idle/Idle", 4);
            walkFrames = animator.LoadFrames("Enemies/Skeleton/Walk/Walk", 4);
            attackFrames = animator.LoadFrames("Enemies/Skeleton/Attack/Attack", 8);
            hurtFrames = animator.LoadFrames("Enemies/Skeleton/Hurt/Hurt", 4);
            deathFrames = animator.LoadFrames("Enemies/Skeleton/Death/Death", 4);

            animator.AddAnimation(new Animation("Idle", 2.5f, true, idleFrames));
            animator.AddAnimation(new Animation("Walk", 5f, true, walkFrames));
            animator.AddAnimation(new Animation("Attack", 10f, false, attackFrames));
            animator.AddAnimation(new Animation("Hurt", 10f, false, hurtFrames));
            animator.AddAnimation(new Animation("Death", 2.5f, false, deathFrames));
        }
    }
}
