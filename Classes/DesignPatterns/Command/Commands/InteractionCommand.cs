using Grief.Classes.DesignPatterns.Composite.ObjectComponents;

namespace Grief.Classes.DesignPatterns.Command.Commands
{
    /// <summary>
    /// Interaction Command
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public class InteractionCommand : ICommand
    {
        //Reference til player
        private readonly PlayerComponent player;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="player"></param>
        public InteractionCommand(PlayerComponent player)
        {
            this.player = player;
        }

        /// <summary>
        /// Metode til at eksekverer commanden - gøre brug af en metode fra player
        /// </summary>
        public void Execute()
        {
            if (player.CanInteract())
            {
                player.Interact();
            }
        }
    }
}
