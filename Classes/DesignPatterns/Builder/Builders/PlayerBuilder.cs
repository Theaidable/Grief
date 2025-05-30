using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Grief.Classes.DesignPatterns.Builder.Builders
{
    /// <summary>
    /// Concrete builder for Player
    /// </summary>
    public class PlayerBuilder : IGameObjectBuilder
    {
        //Private fields
        private GameObject player;

        /// <summary>
        /// Constructor
        /// </summary>
        public PlayerBuilder()
        {
            player = new GameObject();
        }

        /// <summary>
        /// Metode til at sætte playerens tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public IGameObjectBuilder SetTag(string tag)
        {
            player.Tag = tag;
            return this;
        }

        /// <summary>
        /// Metode til at sætte playerens position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public IGameObjectBuilder SetPosition(Vector2 position)
        {
            player.Transform.Position = position;
            return this;
        }

        /// <summary>
        /// Metode til at tilføje en bestemt komponent - SpriteRenderer
        /// </summary>
        /// <returns></returns>
        public IGameObjectBuilder AddSpriteRenderer()
        {
            var spriteRenderer = player.AddComponent<SpriteRenderer>();
            spriteRenderer.SetSprite("MainCharacter/Idle/Idle01");
            return this;
        }

        /// <summary>
        /// Metode til at tilføje en bestemt komponent - Animator
        /// </summary>
        /// <returns></returns>
        public IGameObjectBuilder AddAnimator()
        {
            player.AddComponent<Animator>();
            return this;
        }

        /// <summary>
        /// Metode til at tilføje en bestemt komponent - Collider
        /// </summary>
        /// <returns></returns>
        public IGameObjectBuilder AddCollider()
        {
            var collider = player.AddComponent<Collider>();
            collider.ColliderSize = new Point(20, 30);
            return this;
        }

        /// <summary>
        /// Metode til at tilføje en bestemt unik komponent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Component AddScriptComponent<T>() where T : Component
        {
            return player.AddComponent<T>();
        }

        /// <summary>
        /// Returnere et GameObject (Playeren)
        /// </summary>
        /// <returns></returns>
        public GameObject GetResult()
        {
            return player;
        }
    }
}
