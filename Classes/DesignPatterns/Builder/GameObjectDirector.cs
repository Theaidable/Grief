using Grief.Classes.DesignPatterns.Composite;

namespace Grief.Classes.DesignPatterns.Builder
{
    /// <summary>
    /// Directoren bruges til at bygge objekter
    /// </summary>
    public class GameObjectDirector
    {
        //Private fields
        private IGameObjectBuilder builder;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="builder"></param>
        public GameObjectDirector(IGameObjectBuilder builder)
        {
            this.builder = builder;
        }

        /// <summary>
        /// Metode til at construere et objekt med brug af en concrete builder
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public GameObject Construct(string tag)
        {
            return builder.SetTag(tag).AddSpriteRenderer().AddAnimator().AddCollider().GetResult();
        }
    }
}
