using Microsoft.Xna.Framework;

namespace Grief.Classes.Algorithms
{
    public class Tile
    {
        public Point Position { get; private set; }
        public bool IsWalkable { get; private set; }

        public Tile(Point position, bool isWalkable)
        {
            Position = position;
            IsWalkable = isWalkable;
        }
    }
}
