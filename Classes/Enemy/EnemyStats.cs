using Microsoft.Xna.Framework;

namespace Grief.Classes.Enemy
{
    /// <summary>
    /// Hjælpe klasse til at give enemies forskellige stats
    /// </summary>
    public class EnemyStats
    {
        public int Health { get; set; }
        public int Damage { get; set; }
        public float Speed { get; set; }
        public int DetectionRange { get; set; }
        public int AttackRange { get; set; }
        public Point ColliderSize { get; set; }

        /// <summary>
        /// Construcotr
        /// </summary>
        /// <param name="health"></param> dens liv
        /// <param name="damage"></param> dens skade
        /// <param name="speed"></param> dens bevægelseshastighed
        /// <param name="detectionRange"></param> detection range
        /// <param name="attackRange"></param> attack range
        /// <param name="colliderSize"></param> størrelsen på dens collider
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
