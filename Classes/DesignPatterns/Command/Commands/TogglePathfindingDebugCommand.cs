using Grief.Classes.Levels;

namespace Grief.Classes.DesignPatterns.Command.Commands
{
    /// <summary>
    /// Klasse som skal bruges til at debugge AStar
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public class TogglePathfindingDebugCommand : ICommand
    {
        private readonly Level level;

        public TogglePathfindingDebugCommand(Level level)
        {
            this.level = level;
        }

        public void Execute()
        {
            level.ShowPathfindingDebug = !level.ShowPathfindingDebug;
        }
    }
}
