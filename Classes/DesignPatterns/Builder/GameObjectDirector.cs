using Grief.Classes.DesignPatterns.Composite;

namespace Grief.Classes.DesignPatterns.Builder
{
    public class GameObjectDirector
    {
        //Private fields
        private IGameObjectBuilder builder;

        public GameObjectDirector(IGameObjectBuilder builder)
        {
            this.builder = builder;
        }

        public GameObject Construct(string tag)
        {
            return builder.SetTag(tag).AddSpriteRenderer().AddAnimator().AddCollider().GetResult();
        }
    }
}
