using Grief.Classes.DesignPatterns.Composite.ObjectComponents;

namespace Grief.Classes.DesignPatterns.Command.Commands
{
    public class StopCommand : ICommand
    {
        private PlayerComponent player;

        public StopCommand(PlayerComponent player)
        {
            this.player = player;
        }

        public void Execute()
        {
            player.Stop();
        }
    }
}
