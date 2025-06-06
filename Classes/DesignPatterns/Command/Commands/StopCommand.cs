using Grief.Classes.DesignPatterns.Composite.ObjectComponents;

namespace Grief.Classes.DesignPatterns.Command.Commands
{
    /// <summary>
    /// Command til at stoppe spilleren
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public class StopCommand : ICommand
    {
        //Reference til player
        private PlayerComponent player;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="player"></param>
        public StopCommand(PlayerComponent player)
        {
            this.player = player;
        }

        /// <summary>
        /// Metode til at eksekverer commanden - gøre brug af en metode fra player
        /// </summary>
        public void Execute()
        {
            player.Stop();
        }
    }
}
