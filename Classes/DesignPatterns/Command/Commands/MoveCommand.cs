using Grief.Classes.DesignPatterns.Composite.ObjectComponents;
using Microsoft.Xna.Framework;

namespace Grief.Classes.DesignPatterns.Command.Commands
{
    public class MoveCommand : ICommand
    {
        private Vector2 direction;
        private PlayerComponent player;

        public MoveCommand(Vector2 direction, PlayerComponent player)
        {
            this.direction = direction;
            this.player = player;
        }

        public void Execute()
        {
            player.Move(direction);
        }
    }
}
