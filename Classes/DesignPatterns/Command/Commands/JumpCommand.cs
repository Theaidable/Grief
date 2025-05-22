using Grief.Classes.DesignPatterns.Composite.ObjectComponents;

namespace Grief.Classes.DesignPatterns.Command.Commands
{
    public class JumpCommand : ICommand
    {
        private PlayerComponent player;

        public JumpCommand(PlayerComponent player)
        {
            this.player = player;
        }

        public void Execute()
        {
            player.Jump();
        }
    }
}
