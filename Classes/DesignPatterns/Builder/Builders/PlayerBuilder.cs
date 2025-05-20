using Grief.Classes.DesignPatterns.Composite;
using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Microsoft.Xna.Framework;

namespace Grief.Classes.DesignPatterns.Builder.Builders
{
    public class PlayerBuilder : IGameObjectBuilder
    {
        private GameObject player;

        public PlayerBuilder()
        {
            player = new GameObject();
            player.AddComponent<PlayerComponent>();
        }

        public IGameObjectBuilder SetPosition(Vector2 position)
        {
            player.Transform.Position = position;
            return this;
        }

        public IGameObjectBuilder AddSpriteRenderer()
        {
            player.AddComponent<SpriteRenderer>();
            return this;
        }

        public IGameObjectBuilder AddAnimator()
        {
            player.AddComponent<Animator>();
            return this;
        }

        public IGameObjectBuilder AddCollider()
        {
            player.AddComponent<Collider>();
            return this;
        }

        public IGameObjectBuilder SetTag(string tag)
        {
            player.Tag = tag;
            return this;
        }

        public GameObject GetResult()
        {
            return player;
        }
    }
}
