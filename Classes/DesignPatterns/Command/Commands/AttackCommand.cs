using Grief.Classes.DesignPatterns.Composite.ObjectComponents;

namespace Grief.Classes.DesignPatterns.Command.Commands
{
    public class AttackCommand : ICommand
    {
        private readonly PlayerComponent player;

        public AttackCommand(PlayerComponent player)
        {
            this.player = player;
        }

        public void Execute()
        {
            if (player.CanUseAttack())
            {
                player.Attack();
            }
        }
    }
}
