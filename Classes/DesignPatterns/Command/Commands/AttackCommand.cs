using Grief.Classes.DesignPatterns.Composite.ObjectComponents;

namespace Grief.Classes.DesignPatterns.Command.Commands
{
    /// <summary>
    /// Basic Attack Command
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public class AttackCommand : ICommand
    {
        //Reference til player
        private readonly PlayerComponent player;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="player"></param>
        public AttackCommand(PlayerComponent player)
        {
            this.player = player;
        }

        /// <summary>
        /// Metode til at eksekverer commanden - gøre brug af en metode fra player
        /// </summary>
        public void Execute()
        {
            if (player.CanUseAttack())
            {
                player.Attack();
            }
        }
    }
}
