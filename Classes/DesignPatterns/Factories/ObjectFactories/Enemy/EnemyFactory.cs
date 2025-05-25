using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Grief.Classes.DesignPatterns.Factories.ObjectFactories.Enemy
{
    public enum EnemyType
    {
        Enemy1,
        Enemy2
    }

    public class EnemyFactory : Factory
    {
        private static readonly Dictionary<EnemyType, string> enemySpriteNames = new Dictionary<EnemyType, string>()
        {
            { EnemyType.Enemy1, "Enemies/Skeleton/Idle/Idle01" },
            { EnemyType.Enemy2, "Enemies/Skeleton/Idle/Idle01" },
        };

        private static readonly Dictionary<EnemyType, EnemyStats> enemyStats = new Dictionary<EnemyType, EnemyStats>()
        {
            {EnemyType.Enemy1, new EnemyStats(100,20,50,150,10, new Point(65,50))},
            {EnemyType.Enemy2, new EnemyStats(100,20,50,150,10, Point.Zero)},
        };

        //Oprettelse af Singleton af EnemyFactory
        private static EnemyFactory instance;
        public static EnemyFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EnemyFactory();
                }
                return instance;
            }
        }

        private EnemyFactory() { }

        public override GameObject Create(Vector2 position)
        {
            return Create(position, EnemyType.Enemy1);
        }

        public GameObject Create(Vector2 position, EnemyType enemyType)
        {
            GameObject enemyObject = new GameObject();
            SpriteRenderer spriteRenderer = enemyObject.AddComponent<SpriteRenderer>();
            Animator animator = enemyObject.AddComponent<Animator>();
            Collider collider = enemyObject.AddComponent<Collider>();
            collider.ColliderSize = new Point(65, 55);
            var enemy = enemyObject.AddComponent<EnemyComponent>();

            enemyObject.Transform.Position = position;

            //Sæt sprite
            if(enemySpriteNames.TryGetValue(enemyType, out var spriteName))
            {
                spriteRenderer.SetSprite(spriteName);
                spriteRenderer.Origin = new Vector2(spriteRenderer.Sprite.Width / 2f, spriteRenderer.Sprite.Height / 1f);
            }

            //Sæt stats
            if(enemyStats.TryGetValue(enemyType, out var stats))
            {
                enemy.SetStats(stats);
            }

            return enemyObject;
        }
    }
}
