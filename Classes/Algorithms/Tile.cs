using Microsoft.Xna.Framework;

namespace Grief.Classes.Algorithms
{
    /// <summary>
    /// Hjælpeklasse som bruges til at definere tiles på mappet som skal bruges til algoritmen
    /// </summary>
    /// <author>David Gudmund Danielsen</author>
    public class Tile
    {
        //Properties
        public Point Position { get; private set; }
        public bool IsWalkable { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position"></param>
        /// <param name="isWalkable"></param>
        /// <author>David Gudmund Danielsen</author>
        public Tile(Point position, bool isWalkable)
        {
            Position = position;
            IsWalkable = isWalkable;
        }
    }
}
