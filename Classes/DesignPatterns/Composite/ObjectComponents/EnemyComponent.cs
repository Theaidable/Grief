using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Factories.ObjectFactories;
using Grief.Classes.DesignPatterns.Factories.ObjectFactories.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Grief.Classes.DesignPatterns.Composite.ObjectComponents
{
    public class EnemyComponent : Component
    {
        private Animator animator;

        private Texture2D[] walkLeftFrames;
        private Texture2D[] walkRightFrames;

        private Texture2D[] pursueLeftFrames;
        private Texture2D[] pursueRightFrames;

        private Texture2D[] jumpFrames;
        private Texture2D[] fallFrames;

        private Texture2D[] attackLeftFrames;
        private Texture2D[] attackRightFrames;

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
            /*
             * WalkFrames
             * walkLeftFrames = LoadFrames("stien for at finde den sprite som er walkingLeft", antal frames);
             * walkRightFrames = LoadFrames("stien for at finde den sprite som er walkingRight", antal frames);
             * 
             * pursueFrames
             * pursueLeftFrames = LoadFrames("stien for at finde den sprite som er pursueLeft", antal frames);
             * pursueRightFrames = LoadFrames("stien for at finde den sprite som er pursueRight", antal frames);
             * 
             * attackFrames
             * attackLeftFrames = LoadFrames("stien for at finde den sprite som er attackLeft", antal frames);
             * attackRightFrames = LoadFrames("stien for at finde den sprite som er attackRight", antal frames);
             * 
             * Jump and Fall Frames
             * jumpFrames = LoadFrames("stien for at finde den sprite som er jump", antal frames);
             * fallFrames = LoadFrames("stien for at finde den sprite som er fall", antal frames);
             * 
             * AddAnimations
             * animator.AddAnimation(new Animation("WalkLeft", 2.5f, true, walkLeftFrames));
             * animator.AddAnimation(new Animation("WalkRight", 2.5f, true, walkRightFrames));
             * animator.AddAnimation(new Animation("PursueLeft", 2.5f, true, idleLeftFrames));
             * animator.AddAnimation(new Animation("PursueRight", 2.5f, true, idleRightFrames));
             * animator.AddAnimation(new Animation("Jump", 2.5f, false, jumpFrames));
             * animator.AddAnimation(new Animation("Fall", 2.5f, false, fallFrames));
             * animator.AddAnimation(new Animation("AttackLeft", 2.5f, false, jumpFrames));
             * animator.AddAnimation(new Animation("AttackRight", 2.5f, false, fallFrames));
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

        private void PlayPatrolAnimation()
        {
            //Her skal vi afspille walkanimation baseret på hvilken retning fjenden går, om det er til venstre eller højre
        }

        private void PlayPursueAnimation()
        {
            //Her skal vi afspille pursueAnimation når fjenden jagter spilleren, og der skal køres om det er til højre eller venstre
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
