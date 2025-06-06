using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Microsoft.Xna.Framework;

namespace Grief.Classes.DesignPatterns.Command.Commands
{
    /// <summary>
    /// Move Commnad
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public class MoveCommand : ICommand
    {
        //Field som bruges til at bestemme bevægelsesretning
        private Vector2 direction;

        //Reference til player
        private PlayerComponent player;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="player"></param>
        public MoveCommand(Vector2 direction, PlayerComponent player)
        {
            this.direction = direction;
            this.player = player;
        }

        /// <summary>
        /// Metode til at eksekverer commanden - gøre brug af en metode fra player
        /// </summary>
        public void Execute()
        {
            player.Move(direction);
        }
    }
}
