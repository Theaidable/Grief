using Grief.Classes.DesignPatterns.Composite.Components;
using Grief.Classes.DesignPatterns.Composite;
using System.Collections.Generic;

namespace Grief.Classes.DesignPatterns.Command.Commands
{
    public class ToggleColliderDrawingCommand : ICommand
    {
        private List<GameObject> gameObjects;
        private bool shouldDraw;

        public ToggleColliderDrawingCommand(List<GameObject> gameObjects)
        {
            this.gameObjects = gameObjects;
        }

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
