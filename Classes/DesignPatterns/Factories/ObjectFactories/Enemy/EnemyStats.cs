namespace Grief.Classes.DesignPatterns.Factories.ObjectFactories.Enemy
{
    public class EnemyStats
    {
        public int Health { get; set; }
        public int Damage { get; set; }
        public float Speed { get; set; }
        public int DetectionRange { get; set; }

        public EnemyStats(int health, int damage, float speed, int detectionRange)
        {
            Health = health;
            Damage = damage;
            Speed = speed;
            DetectionRange = detectionRange;
        }
    }
}
