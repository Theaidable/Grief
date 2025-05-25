using Greif;
using Grief.Classes.Algorithms;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Factories.ObjectFactories.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    public class EnemyComponent : Component
    {
        private bool isHurt;
        private float attackCooldown = 0.1f;
        private float cooldownTimer = 0f;
        private bool isAttacking;
        private Vector2 velocity;
        private float gravity = 600f;
        private bool grounded;

        private int currentPatrolIndex = 0;
        private bool patrolForward;

        private Animator animator;
        private Collider Collider;
        private AStar astar;

        private Texture2D[] idleFrames;
        private Texture2D[] walkFrames;

        private Texture2D[] attackFrames;

        private Texture2D[] hurtFrames;
        private Texture2D[] deathFrames;

        private List<Vector2> path = new List<Vector2>();
        private readonly object pathLock = new object();
        private float RecalculatePathTimer = 0f;

        public int EnemyHealth { get; private set; }
        public int EnemyDamage { get; private set; }
        public float EnemySpeed { get; private set; }
        public int EnemyDetectionRange { get; private set; }
        public int EnemyAttackRange { get; private set; }
        public Point EnemyColliderSize { get; private set; }
        
        public List<Vector2> PatrolPoints { get; set; }

        public EnemyComponent(GameObject gameObject) : base(gameObject) { }

        public override void Start()
        {
            animator = GameObject.GetComponent<Animator>();
            astar = GameWorld.Instance.LevelManager.CurrentLevel.PathFinder;
            AddAnimations();
            animator.PlayAnimation("Idle");
        }

        public void SetStats(EnemyStats stats)
        {
            EnemyHealth = stats.Health;
            EnemyDamage = stats.Damage;
            EnemySpeed = stats.Speed;
            EnemyDetectionRange = stats.DetectionRange;
            EnemyAttackRange = stats.AttackRange;
            EnemyColliderSize = stats.ColliderSize;

            var collider = GameObject.GetComponent<Collider>();
            if(collider != null)
            {
                collider.ColliderSize = EnemyColliderSize;
            }
        }

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

            if(isAttacking == true)
            {
                return;
            }

            //Vector2 originalPosition = GameObject.Transform.Position;
            GameObject.Transform.Translate(new Vector2(0, velocity.Y * GameWorld.Instance.DeltaTime));
            grounded = CheckGrounded();

            if (grounded == true && velocity.Y > 0)
            {
                velocity.Y = 0;
            }

            if (EnemyHealth > 0 && isHurt == false && grounded == true)
            {
                if (PlayerIsWithInAttackRange() == true)
                {
                    Attack();
                }
                else if (PlayerIsWithInDetectionRange() == false)
                {
                    path.Clear();
                    Patrol();
                }
                else
                {
                    Pursue();
                }
            }
        }

        private bool CheckGrounded()
        {
            var collider = GameObject.GetComponent<Collider>().CollisionBox;
            var tiles = GameWorld.Instance.LevelManager.CurrentLevel.CollisionRectangles;

            foreach (var tile in tiles)
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

            return false;
        }

        public void Patrol()
        {
            //Fejl kontrol
            if (PatrolPoints == null || PatrolPoints.Count < 2)
            {
                animator.PlayAnimation("Idle");
                return;
            }

            Vector2 target = PatrolPoints[currentPatrolIndex];
            Vector2 position = GameObject.Transform.Position;
            Vector2 direction = target - position;

            if(direction.Length() < 4f)
            {
                if(patrolForward == true)
                {
                    currentPatrolIndex++;
                    if(currentPatrolIndex >= PatrolPoints.Count)
                    {
                        currentPatrolIndex = 0;
                        patrolForward = false;
                    }
                }
                else
                {
                    currentPatrolIndex--;
                    if(currentPatrolIndex < 0)
                    {
                        currentPatrolIndex = 1;
                        patrolForward = true;
                    }
                }

                target = PatrolPoints[currentPatrolIndex];
                direction = target - position;
            }

            direction.Normalize();
            Move(direction);
        }

        public void Pursue()
        {
            var level = GameWorld.Instance.LevelManager.CurrentLevel;
            var player = level.GameObjects.FirstOrDefault(g => g.GetComponent<PlayerComponent>() != null);

            if(player == null)
            {
                return;
            }

            Point start = new Point((int)(GameObject.Transform.Position.X / level.Map.TileWidth), (int)(GameObject.Transform.Position.Y / level.Map.TileHeight));
            Point goal = new Point((int)(player.Transform.Position.X / level.Map.TileWidth), (int)(player.Transform.Position.Y / level.Map.TileHeight));

            if (path == null || path.Count == 0 || RecalculatePathTimer <= 0f)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    var newPath = astar.FindPath(start, goal);

                    lock (pathLock)
                    {
                        path = newPath.Select(t => new Vector2(t.Position.X * level.Map.TileWidth + level.Map.TileWidth/2, t.Position.Y * level.Map.TileHeight + level.Map.TileHeight/2)).ToList();
                    }
                });

                RecalculatePathTimer = 1f;
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
                        direction.Normalize();
                        Move(direction);
                    }
                }
            }

            RecalculatePathTimer -= GameWorld.Instance.DeltaTime;
        }

        private void Move(Vector2 direction)
        {
            SpriteRenderer spriteRenderer = GameObject.GetComponent<SpriteRenderer>();

            //Flip sprite baseret på direction
            if (direction.X < 0)
            {
                spriteRenderer.Effects = SpriteEffects.FlipHorizontally;
            }
            else if (direction.X > 0)
            {
                spriteRenderer.Effects = SpriteEffects.None;
            }

            Vector2 movement = direction * EnemySpeed * GameWorld.Instance.DeltaTime;
            Vector2 originalPosition = GameObject.Transform.Position;
            GameObject.Transform.Translate(movement);

            //AABB
            var playerCollider = GameObject.GetComponent<Collider>().CollisionBox;
            bool collision = GameWorld.Instance.LevelManager.CurrentLevel.CollisionRectangles.Any(tile => tile.Intersects(playerCollider));

            if (collision == true)
            {
                GameObject.Transform.Position = originalPosition;
            }
            else
            {
                if (isAttacking == false && grounded == true)
                {
                    animator.PlayAnimation("Walk");
                }
            }
        }

        private bool PlayerIsWithInDetectionRange()
        {
            var player = GameWorld.Instance.LevelManager.CurrentLevel.GameObjects.FirstOrDefault(g => g.GetComponent<PlayerComponent>() != null);

            if(player == null)
            {
                return false;
            }


            var distance = Vector2.Distance(GameObject.Transform.Position, player.Transform.Position);
            return distance <= EnemyDetectionRange;
        }

        public void Attack()
        {
            if(isAttacking == true)
            {
                return;
            }

            if(cooldownTimer <= 0f)
            {
                isAttacking = true;

                animator.PlayAnimation("Attack");
                animator.ClearOnAnimationComplete();

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

        public void TakeDamage(int amount)
        {
            EnemyHealth -= amount;

            if(EnemyHealth > 0)
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
            }
        }

        private void AddAnimations()
        {
            idleFrames = LoadFrames("Enemies/Skeleton/Idle/Idle", 4);
            walkFrames = LoadFrames("Enemies/Skeleton/Walk/Walk", 4);

            attackFrames = LoadFrames("Enemies/Skeleton/Attack/Attack", 8);

            hurtFrames = LoadFrames("Enemies/Skeleton/Hurt/Hurt", 4);
            deathFrames = LoadFrames("Enemies/Skeleton/Death/Death", 4);

            animator.AddAnimation(new Animation("Idle", 2.5f, true, idleFrames));
            animator.AddAnimation(new Animation("Walk", 4f, true, walkFrames));
            animator.AddAnimation(new Animation("Attack", 10f, false, attackFrames));
            animator.AddAnimation(new Animation("Hurt", 10f, false, hurtFrames));
            animator.AddAnimation(new Animation("Death", 2.5f, false, deathFrames));
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
    }
}
