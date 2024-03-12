using System;
using System.Collections.Generic;
using System.IO;

namespace GridWorld
{
    class Program
    {
        static void Main()
        {
            var curDir = Directory.GetCurrentDirectory();
            var mapName = "map02";
            var pathToMap = Path.Combine(curDir, $"Maps/{mapName}.txt");
            var grid = InitGrid(pathToMap);
            var maxMoves = GameParameters.MaxMoves;
            PlayerLoop(grid, maxMoves);
        }

        private static void PlayerLoop(Grid grid, int maxMoves)
        {
            int moves = 0;
            while (moves < maxMoves)
            {
                var goalAchieved = grid.Update();
                if(goalAchieved)
                {
                    Console.WriteLine("Goal Reached!");
                    break;
                }
                moves++;

                Thread.Sleep(10);
            }

            if (moves >= maxMoves)
            {
                Console.WriteLine("Moves exhausted. Try again.");
            }
        }

        private static Grid InitGrid(string mapPath)
        {
            var grid = new Grid();

            grid.LoadGridFromFile(mapPath);
            grid.DisplayGrid();

            return grid;
        }
    }
}
