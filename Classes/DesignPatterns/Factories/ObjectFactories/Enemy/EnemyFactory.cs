using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Grief.Classes.Items;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Grief.Classes.DesignPatterns.Factories.ObjectFactories.Enemy
{
    /// <summary>
    /// Enumaration for de forskellige typer af enemies som skal eksistere i spillet
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public enum EnemyType
    {
        Enemy1,
        Enemy2
    }

    /// <summary>
    /// EnemyFactory som bruges til oprettelse af enemies
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public class EnemyFactory : Factory
    {
        //Dicitionary til at sætte sprite til den bestemte type af enemy
        private static readonly Dictionary<EnemyType, string> enemySpriteNames = new Dictionary<EnemyType, string>()
        {
            { EnemyType.Enemy1, "Enemies/Skeleton/Idle/Idle01" },
            { EnemyType.Enemy2, "Enemies/Skeleton/Idle/Idle01" },
        };

        //Dictionary til at sætte stats for enemies
        private static readonly Dictionary<EnemyType, EnemyStats> enemyStats = new Dictionary<EnemyType, EnemyStats>()
        {
            {EnemyType.Enemy1, new EnemyStats(100,40,50,150,15, new Point(15,49))},
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

        /// <summary>
        /// Private constructor
        /// </summary>
        private EnemyFactory() { }

        /// <summary>
        /// Override metode til standard oprettelse af en fjende
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override GameObject Create(Vector2 position)
        {
            return Create(position, EnemyType.Enemy1);
        }

        /// <summary>
        /// Metode til oprettelse af en enemy
        /// </summary>
        /// <param name="position"></param> Dens position
        /// <param name="enemyType"></param> Type af enemy
        /// <param name="patrolPoints"></param> om den skal have nogle punkter at patroljere mellem
        /// <param name="item"></param> Om den skal kunne droppe et item
        /// <returns></returns>
        public GameObject Create(Vector2 position, EnemyType enemyType, List<Vector2> patrolPoints = null, Item item = null)
        {
            GameObject enemyObject = new GameObject();
            SpriteRenderer spriteRenderer = enemyObject.AddComponent<SpriteRenderer>();
            Animator animator = enemyObject.AddComponent<Animator>();
            Collider collider = enemyObject.AddComponent<Collider>();

            if(enemyStats.TryGetValue(enemyType, out var stats))
            {
                collider.ColliderSize = stats.ColliderSize;
            }

            var enemy = enemyObject.AddComponent<EnemyComponent>();
            enemy.PatrolPoints = patrolPoints;
            enemyObject.Transform.Position = position;

            //Sæt sprite
            if(enemySpriteNames.TryGetValue(enemyType, out var spriteName))
            {
                spriteRenderer.SetSprite(spriteName);
            }

            //Sæt stats
            enemy.SetStats(stats);
            enemy.SetEnemyType(enemyType);

            if (item != null)
            {
                enemy.SetDropItem(item);
            }

            return enemyObject;
        }
    }
}
