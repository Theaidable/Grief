using Greif;
using Grief.Classes.Algorithms;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Factories.ObjectFactories;
using Grief.Classes.DesignPatterns.Factories.ObjectFactories.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System.Collections.Generic;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    public class EnemyComponent : Component
    {
        private Animator animator;

        private Texture2D[] idleFrames;
        private Texture2D[] walkFrames;

        private Texture2D[] attackFrames;

        private Texture2D[] hurtFrames;
        private Texture2D[] deathFrames;

        public int EnemyHealth { get; private set; }
        public int EnemyDamage { get; private set; }
        public float EnemySpeed { get; private set; }
        public int EnemyDetectionRange { get; private set; } //Skal muligivis være en rectangle eller en cirkel, sådan eller bruge detectionrange som radius.
        
        public List<Vector2> PatrolPoints { get; set; }

        public EnemyComponent(GameObject gameObject) : base(gameObject) { }

        public override void Start()
        {
            animator = GameObject.GetComponent<Animator>();
            AddAnimations();
            animator.PlayAnimation("Idle");
        }

        public override void Update()
        {
            //Her skal vi lave physics for at sørge for at når velocity er over 0, så skal FallAnimation afspilles
        }

        public void SetStats(EnemyStats stats)
        {
            EnemyHealth = stats.Health;
            EnemyDamage = stats.Damage;
            EnemySpeed = stats.Speed;
            EnemyDetectionRange = stats.DetectionRange;
        }

        public void Patrol()
        {
            //Logik for at patrole mellem to punkter eller flere

            //PlayWalkAnimation mens man patroljere
        }

        public void Pursue()
        {
            //Hvis player kommer ind for detectionrange (insection mellem playerCollider og EnemyDectionRange?) så skal A* køres
            //Hvis der findes en sti, så skal enemy jagte spilleren og PlayPursueAnimation
        }

        public void Jump()
        {
            //Her skal vi lave logikken for enemies jumps
        }

        public void Attack()
        {
            //Her skal vi lave logikken for enemies attack metode
        }

        private void AddAnimations()
        {
            idleFrames = LoadFrames("Enemies/Skeleton/Idle/Idle", 4);
            walkFrames = LoadFrames("Enemies/Skeleton/Walk/Walk", 4);

            attackFrames = LoadFrames("Enemies/Skeleton/Attack/Attack", 4);

            hurtFrames = LoadFrames("Enemies/Skeleton/Hurt/Hurt", 4);
            deathFrames = LoadFrames("Enemies/Skeleton/Death/Death", 4);

            animator.AddAnimation(new Animation("Idle", 2.5f, true, idleFrames));
            animator.AddAnimation(new Animation("Walk", 2.5f, true, walkFrames));
            animator.AddAnimation(new Animation("Attack", 2.5f, false, attackFrames));
            animator.AddAnimation(new Animation("Hurt", 2.5f, false, hurtFrames));
            animator.AddAnimation(new Animation("Death", 2.5f, false, deathFrames));
        }

        private Texture2D[] LoadFrames(string basePath, int frameCount)
        {
            Texture2D[] frames = new Texture2D[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                frames[i] = GameWorld.Instance.Content.Load<Texture2D>($"{basePath}0{i+1}"); //Sørg for at dette svare korrekt til stinavn
            }
            return frames;
        }

        private void PlayPatrolAnimation()
        {
            //Her skal vi afspille walkanimation baseret på hvilken retning fjenden går, om det er til venstre eller højre
        }

        private void PlayJumpAnimation()
        {
            //Her skal vi afspille jumpAnimation hvis fjenden har brug for at hoppe for at patroljere eller jagte spilleren
        }

        private void PlayFallAnimation()
        {
            //Her skal vi afspille fallAnimation hvis fjenden falder fra et sted for at patroljere eller jagte spilleren
        }

        private void PlayAttackAnimation()
        {
            //Her skal vi afspille fjenders attackAnimation når fjenden prøver at angribe spilleren
        }
    }
}
