using Greif;
using Grief.Classes.Algorithms;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Factories.ObjectFactories.Enemy;
using Grief.Classes.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    public class EnemyComponent : Component
    {
        //Fields
        private bool isHurt;
        private float attackCooldown = 1f;
        private float cooldownTimer = 0f;
        private bool isAttacking;
        private Vector2 velocity;
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
        private float RecalculatePathTimer = 0f;

        //Properties
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
        /// Tilføj vigtige elementer når spillet startes
        /// </summary>
        public override void Start()
        {
            animator = GameObject.GetComponent<Animator>();
            collider = GameObject.GetComponent<Collider>();
            astar = GameWorld.Instance.LevelManager.CurrentLevel.PathFinder;
            AddAnimations();
            animator.PlayAnimation("Idle");
        }

        /// <summary>
        /// Sæt en enemies stats
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
        /// Styrer in-game logik
        /// </summary>
        public override void Update()
        {
            //Opdatere cooldownTimeren
            if (cooldownTimer > 0)
            {
                cooldownTimer -= GameWorld.Instance.DeltaTime;
            }

            //Hvis man ikke er på jorden, så tilføjes gravity
            if (grounded == false)
            {
                velocity.Y += gravity * GameWorld.Instance.DeltaTime;
            }

            //Hvis enemy angriber, så stopper den sine andre handlinger
            if (isAttacking == true)
            {
                return;
            }

            //Sæt positionen / opdatere positionen
            Vector2 originalPosition = GameObject.Transform.Position;
            GameObject.Transform.Translate(new Vector2(0, velocity.Y * GameWorld.Instance.DeltaTime));

            //Sæt grounded
            grounded = collider.CheckGrounded(GameObject);

            //Hvis man rammer jorden, så resettes ens velocity på y-aksen - Man stopper med at falde
            if (grounded == true && velocity.Y > 0)
            {
                velocity.Y = 0;
            }
            //Hvis den ikke står på jorden, så afspilles en animation
            else if (grounded == false)
            {
                animator.PlayAnimation("Idle"); //Her kunn man lave en jump/fall animation - dog har vi ikke sprites til det
            }

            //Kør enemys logik så længe de er på jorden, ikke tager skade og har mere end 0 liv
            if (EnemyHealth > 0 && isHurt == false && grounded == true)
            {

                //Hvis player er inde for detection range, så begynder fjenden at jagte spilleren
                if (PlayerIsWithInDetectionRange() == true)
                {
                    Pursue();

                    //Hvis spilleren kommer inden for attack range, så angriber fjenden
                    if (PlayerIsWithInAttackRange() == true)
                    {
                        Attack();
                    }
                }
                //Ellers patroljere fjenden
                else
                {
                    path.Clear();
                    Patrol();
                }
            }
        }

        /// <summary>
        /// Metode til at patroljere mellem to punkter
        /// </summary>
        public void Patrol()
        {
            //Hvis ikke der er nogen punkter, så står den idle
            if (PatrolPoints == null || PatrolPoints.Count < 2)
            {
                animator.PlayAnimation("Idle");
                return;
            }

            //Finde punkt man skal gå hen til, ens position og udregne retningen
            Vector2 target = PatrolPoints[currentPatrolIndex];
            Vector2 position = GameObject.Transform.Position;
            Vector2 direction = target - position;

            //Hvis man kommer tæt på punktet, så fortsætter man videre til næste pinkt
            if (direction.Length() < 4f)
            {
                if (patrolForward == true)
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

            //Sæt dens speed til 40
            EnemySpeed = 40;

            //Bevæg fjenden
            direction.Normalize();
            Move(direction);
        }

        /// <summary>
        /// Metode til at jagte spilleren
        /// </summary>
        public void Pursue()
        {
            //Level
            var level = GameWorld.Instance.LevelManager.CurrentLevel;

            //Find spilleren
            var player = level.GameObjects.FirstOrDefault(g => g.GetComponent<PlayerComponent>() != null);

            //Hvis ikke der er nogen player, så skal man ikke kunne jagte
            if (player == null)
            {
                return;
            }

            //Start position
            Point start = new Point((int)(GameObject.Transform.Position.X / level.Map.TileWidth), (int)(GameObject.Transform.Position.Y / level.Map.TileHeight));

            //Goal position
            Point goal = new Point((int)(player.Transform.Position.X / level.Map.TileWidth), (int)(player.Transform.Position.Y / level.Map.TileHeight));

            //Find en sti
            if (path == null || path.Count == 0 || RecalculatePathTimer <= 0f)
            {
                //Udregn stien i en baggrundstråd
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    var newPath = astar.FindPath(start, goal);

                    lock (pathLock)
                    {
                        path = newPath.Select(t => new Vector2(t.Position.X * level.Map.TileWidth + level.Map.TileWidth / 2, GameObject.Transform.Position.Y)).ToList();
                    }
                });

                RecalculatePathTimer = 1f;
            }

            //Gå hen ad stien
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

            RecalculatePathTimer -= GameWorld.Instance.DeltaTime;
        }

        /// <summary>
        /// Metode til at bevæge fjenden
        /// </summary>
        /// <param name="direction"></param>
        private void Move(Vector2 direction)
        {
            //Flip sprite baseret på direction
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

            //AABB
            var enemyCollider = GameObject.GetComponent<Collider>().CollisionBox;
            bool rectCollision = GameWorld.Instance.LevelManager.CurrentLevel.CollisionRectangles.Any(tile => tile.Intersects(enemyCollider));
            bool polygonCollision = GameWorld.Instance.LevelManager.CurrentLevel.CollisionPolygons.Any(poly => poly.BoundingRectangle.Intersects(enemyCollider));
            bool snappedToSlope = false;

            if (rectCollision == true && polygonCollision == false)
            {
                GameObject.Transform.Position = originalPosition;
            }

            if (polygonCollision == true)
            {
                foreach (Polygon polygon in GameWorld.Instance.LevelManager.CurrentLevel.CollisionPolygons)
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
        /// Hjælpemetode til at beregne om spilleren er inde for detection range
        /// </summary>
        /// <returns></returns>
        private bool PlayerIsWithInDetectionRange()
        {
            var player = GameWorld.Instance.LevelManager.CurrentLevel.GameObjects.FirstOrDefault(g => g.GetComponent<PlayerComponent>() != null);

            if (player == null)
            {
                return false;
            }

            var distance = Vector2.Distance(GameObject.Transform.Position, player.Transform.Position);
            return distance <= EnemyDetectionRange;
        }

        /// <summary>
        /// Metode til at angribe
        /// </summary>
        public void Attack()
        {
            //Man kan ikke angribe mens man er igang med at angribe
            if (isAttacking == true)
            {
                return;
            }

            //Udfør et angreb
            if (cooldownTimer <= 0f)
            {
                //Sæt attacking til true
                isAttacking = true;

                //Afspil animation
                animator.PlayAnimation("Attack");
                animator.ClearOnAnimationComplete();

                //Når animationen er færdig, tjek om angreb rammer spilleren
                animator.OnAnimationComplete = () =>
                {
                    var enemyCollider = GameObject.GetComponent<Collider>();
                    var level = GameWorld.Instance.LevelManager.CurrentLevel;

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

                        //Hvis man rammer spilleren, så tager spilleren skade
                        if (hit == true)
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
        /// Hjælpemetode til at finde ud af om spilleren er inde for attacking range
        /// </summary>
        /// <returns></returns>
        private bool PlayerIsWithInAttackRange()
        {
            var player = GameWorld.Instance.LevelManager.CurrentLevel.GameObjects.FirstOrDefault(g => g.GetComponent<PlayerComponent>() != null);

            if (player == null)
            {
                return false;
            }

            var distance = Vector2.Distance(GameObject.Transform.Position, player.Transform.Position);
            return distance <= EnemyAttackRange;
        }

        /// <summary>
        /// Metode til at tage skade
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
                animator.OnAnimationComplete = () => GameWorld.Instance.LevelManager.CurrentLevel.QueueRemove(GameObject);

                if (droppedItem != null)
                {
                    //Midligertidig måde for at tilføje en item til inventory
                    DropItem(droppedItem);

                    //Tilføj denne linje når der findes en sprite for item i verdenen
                    //GameWorld.Instance.LevelManager.CurrentLevel.AddGameObject(ItemFactory.Instance.Create(droppedItem, GameObject.Transform.Position));
                }
            }
        }

        /// <summary>
        /// Hjælpemetode til at sætte sætte hvilket item der skal droppes
        /// </summary>
        /// <param name="item"></param>
        public void SetDropItem(Item item)
        {
            droppedItem = item;
        }

        /// <summary>
        /// Tilføjer direkte et item til spillerens inventory (spawner det ikke i verdenen)
        /// </summary>
        /// <param name="item"></param>
        private void DropItem(Item item)
        {
            GameObject playerObject = GameWorld.Instance.LevelManager.CurrentLevel.GameObjects.FirstOrDefault(gameObject => gameObject.GetComponent<PlayerComponent>() != null);

            if (playerObject == null)
            {
                return;
            }

            InventoryComponent inventory = playerObject.GetComponent<InventoryComponent>();

            if (inventory == null)
            {
                return;
            }

            inventory.AddItemToInventory(item);
        }

        /// <summary>
        /// Tilføj animationer til enemy
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
