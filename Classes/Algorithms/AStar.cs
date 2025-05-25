using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Grief.Classes.Algorithms
{
    public class AStar
    {
        private Dictionary<Point, Tile> tiles;
        private readonly int[] directionsX = { 0, 1, 0, -1 };
        private readonly int[] directionsY = { -1, 0, 1, 0 };

        public AStar(Dictionary<Point, Tile> tiles)
        {
            this.tiles = tiles;
        }

        public List<Tile> FindPath(Point startPosition, Point goalPosition)
        {
            //Tjek om start/goal findes i dictionary
            if(tiles.ContainsKey(startPosition) == false || tiles.ContainsKey(goalPosition) == false)
            {
                return new List<Tile>();
            }

            Tile startCell = tiles[startPosition];
            Tile goalCell = tiles[goalPosition];

            //Open/Close lister
            var openList = new List<Tile>();
            var closedList = new HashSet<Tile>();

            //G og F værdier for hver celle
            var gCost = new Dictionary<Tile, float>();
            var fCost = new Dictionary<Tile, float>();
            var parent = new Dictionary<Tile, Tile>();

            //Initier gCost og fCost
            foreach (var kvp in tiles)
            {
                gCost[kvp.Value] = float.PositiveInfinity;
                fCost[kvp.Value] = float.PositiveInfinity;
                parent[kvp.Value] = null;
            }

            //Startnode: gCost = 0, fCost = heuristisk
            gCost[startCell] = 0;
            fCost[startCell] = Heuristic(startCell, goalCell);

            //Tilføj start til openList
            openList.Add(startCell);

            //A* Loop
            while (openList.Count > 0)
            {
                //1. Find cellen med lavest fCost i openList
                Tile current = GetCellWithLowestF(openList, fCost);

                if(current == goalCell)
                {
                    return ReconstructPath(parent, goalCell);
                }

                openList.Remove(current);
                closedList.Add(current);

                //2. Undersøg naboer
                foreach (Tile neighbor in GetNeighbors(current))
                {
                    //Spring over, hvis nabo er ikke er walkable
                    if(closedList.Contains(neighbor) == true|| neighbor.IsWalkable == false)
                    {
                        continue;
                    }

                    //Beregn ny gCost
                    float tentativeG = gCost[current] + Heuristic(current,neighbor);

                    //Hvis bedre gCost eller nabo ikke er i openlist
                    if(tentativeG < gCost[neighbor] || openList.Contains(neighbor) == false)
                    {
                        gCost[neighbor] = tentativeG;
                        fCost[neighbor] = tentativeG + Heuristic(current,neighbor);
                        parent[neighbor] = current;

                        if(openList.Contains(neighbor) == false)
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }

            return new List<Tile>();
        }

        //Heuriostic afstand mellem to tiles
        private float Heuristic(Tile a, Tile b)
        {
            var distanceX = Math.Abs(a.Position.X - b.Position.X);
            var distanceY = Math.Abs(a.Position.Y - b.Position.Y);
            return distanceX + distanceY;
        }

        private Tile GetCellWithLowestF(List<Tile> openList, Dictionary<Tile, float> fCost)
        {
            Tile best = openList[0];
            float bestF = fCost[best];

            for (int i = 0; i < openList.Count; i++)
            {
                Tile current = openList[i];

                if (fCost[current] < bestF)
                {
                    bestF = fCost[current];
                    best = current;
                }
            }

            return best;
        }

        private List<Tile> GetNeighbors(Tile tile)
        {
            var result = new List<Tile>();

            //Tjek de 4 retninger (op, ned, venstre og højre)
            for (int i = 0; i < directionsX.Length; i++)
            {
                int newX = (int)tile.Position.X + directionsX[i];
                int newY = (int)tile.Position.Y + directionsY[i];

                Point neighborPosition = new Point(newX, newY);

                //Tjek om naboer er inden for grid
                if (tiles.ContainsKey(neighborPosition))
                {
                    result.Add(tiles[neighborPosition]);
                }
            }

            return result;
        }

        private List<Tile> ReconstructPath(Dictionary<Tile, Tile> parent, Tile goal)
        {
            var path = new List<Tile>();
            var current = goal;

            while (current != null)
            {
                path.Add(current);
                current = parent[current];
            }

            path.Reverse();
            return path;
        }
    }
}
