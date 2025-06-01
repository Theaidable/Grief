using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Composite;
using System.Collections.Generic;

namespace Grief.Classes.DesignPatterns.Command.Commands
{
    /// <summary>
    /// Debug command til at tegne objekters collisions
    /// </summary>
    public class ToggleColliderDrawingCommand : ICommand
    {
        //Liste af de objekter som skal tegnes
        private List<GameObject> gameObjects;

        //Bool om de skal tegnes eller ej
        private bool shouldDraw;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameObjects"></param>
        public ToggleColliderDrawingCommand(List<GameObject> gameObjects)
        {
            this.gameObjects = gameObjects;
        }

        /// <summary>
        /// Metode til at eksekverer commanden
        /// </summary>
        public void Execute()
        {
            shouldDraw = !shouldDraw;
            List<Collider> colliders = new List<Collider>();
            foreach (GameObject gameObject in gameObjects)
            {
                Collider collider = gameObject.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.ToggleDrawing(shouldDraw);
                }
            }
        }
    }
}
