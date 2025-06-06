using Grief.Classes.DesignPatterns.Composite.ObjectComponents;

namespace Grief.Classes.DesignPatterns.Command.Commands
{
    /// <summary>
    /// Jump command - til at få spilleren til at hoppe
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public class JumpCommand : ICommand
    {
        //Reference til player
        private PlayerComponent player;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="player"></param>
        public JumpCommand(PlayerComponent player)
        {
            this.player = player;
        }

        /// <summary>
        /// Metode til at eksekverer commanden - gøre brug af en metode fra player
        /// </summary>
        public void Execute()
        {
            player.Jump();
        }
    }
}
