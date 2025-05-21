using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Microsoft.Xna.Framework;

namespace Grief.Classes.DesignPatterns.Factories.ObjectFactories
{
    public class EnemyFactory
    {
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

        public GameObject Create(Vector2 position)
        {
            GameObject enemyObject = new GameObject();
            SpriteRenderer spriteRenderer = enemyObject.AddComponent<SpriteRenderer>();
            Animator animator = enemyObject.AddComponent<Animator>();
            Collider collider = enemyObject.AddComponent<Collider>();
            var enemy = enemyObject.AddComponent<EnemyComponent>();

            enemyObject.Transform.Position = position;

            //Sæt sprite for enemy her
            spriteRenderer.SetSprite("");

            return enemyObject;
        }
    }
}
