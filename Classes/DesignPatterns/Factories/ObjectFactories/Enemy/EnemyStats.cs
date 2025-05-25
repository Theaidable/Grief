using Microsoft.Xna.Framework;

namespace Grief.Classes.DesignPatterns.Factories.ObjectFactories.Enemy
{
    public class EnemyStats
    {
        public int Health { get; set; }
        public int Damage { get; set; }
        public float Speed { get; set; }
        public int DetectionRange { get; set; }
        public int AttackRange { get; set; }
        public Point ColliderSize { get; set; }

        public EnemyStats(int health, int damage, float speed, int detectionRange, int attackRange, Point colliderSize)
        {
            Health = health;
            Damage = damage;
            Speed = speed;
            DetectionRange = detectionRange;
            AttackRange = attackRange;
            ColliderSize = colliderSize;
        }
    }
}
