using Grief.Classes.DesignPatterns.Composite;
using Microsoft.Xna.Framework;

namespace Grief.Classes.DesignPatterns.Builder
{
    public interface IGameObjectBuilder
    {
        public IGameObjectBuilder SetPosition(Vector2 position);
        public IGameObjectBuilder AddSpriteRenderer();
        public IGameObjectBuilder AddAnimator();
        public IGameObjectBuilder AddCollider();
        public IGameObjectBuilder SetTag(string tag);
        public GameObject GetResult();


    }
}
