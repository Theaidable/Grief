using Grief.Classes.DesignPatterns.Composite;
using Microsoft.Xna.Framework;

namespace Grief.Classes.DesignPatterns.Builder
{
    /// <summary>
    /// Interface som bruges til at bygge et objekt
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public interface IGameObjectBuilder
    {
        //Metoder som skal bruges til at bygge objekter
        public IGameObjectBuilder SetTag(string tag);
        public IGameObjectBuilder SetPosition(Vector2 position);
        public IGameObjectBuilder AddSpriteRenderer();
        public IGameObjectBuilder AddAnimator();
        public IGameObjectBuilder AddCollider();
        public Component AddScriptComponent<T>() where T : Component;
        public GameObject GetResult();


    }
}
