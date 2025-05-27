using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Grief.Classes.DesignPatterns.Builder.Builders
{
    public class PlayerBuilder : IGameObjectBuilder
    {
        private GameObject player;

        public PlayerBuilder()
        {
            player = new GameObject();
        }

        public IGameObjectBuilder SetPosition(Vector2 position)
        {
            player.Transform.Position = position;
            return this;
        }

        public IGameObjectBuilder SetTag(string tag)
        {
            player.Tag = tag;
            return this;
        }

        public IGameObjectBuilder AddSpriteRenderer()
        {
            var spriteRenderer = player.AddComponent<SpriteRenderer>();
            spriteRenderer.SetSprite("MainCharacter/Idle/Idle01");
            return this;
        }

        public IGameObjectBuilder AddAnimator()
        {
            player.AddComponent<Animator>();
            return this;
        }

        public IGameObjectBuilder AddCollider()
        {
            var collider = player.AddComponent<Collider>();
            collider.ColliderSize = new Point(20, 30);
            return this;
        }

        public Component AddScriptComponent<T>() where T : Component
        {
            return player.AddComponent<T>();
        }

        public GameObject GetResult()
        {
            return player;
        }
    }
}
