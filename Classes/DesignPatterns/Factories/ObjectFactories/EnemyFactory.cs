using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Grief.Classes.DesignPatterns.Factories.ObjectFactories
{
    public enum EnemyType
    {
        Enemy1,
        Enemy2
    }

    public class EnemyFactory : Factory
    {
        private string enemySpriteName;
        private static readonly Dictionary<EnemyType, string> enemySpriteNames = new Dictionary<EnemyType, string>()
        {
            { EnemyType.Enemy1, "" },
            { EnemyType.Enemy2, "" },
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
            var enemy = enemyObject.AddComponent<EnemyComponent>();
            enemySpriteName = enemySpriteNames[enemyType];
            spriteRenderer.SetSprite(enemySpriteName);
            enemyObject.Transform.Position = position;
            return enemyObject;
        }
    }
}
