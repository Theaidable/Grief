using Grief.Classes.DesignPatterns.Composite.ObjectComponents;

namespace Grief.Classes.DesignPatterns.Command.Commands
{
    public class InteractionCommand : ICommand
    {
        private readonly PlayerComponent player;

        public InteractionCommand(PlayerComponent player)
        {
            this.player = player;
        }

        public void Execute()
        {
            if (player.CanInteract())
            {
                player.Interact();
            }
        }
    }
}
