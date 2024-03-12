using System.Numerics;
using System.Linq;

namespace GridWorld
{
    public class MoveReward
    {
        public int[] rewards = new int[4] { 0, 0, 0, 0 }; // up, right, down, left

        private static readonly Dictionary<int, Position> IndexToPositionDict = new() 
        {
            {0, Position.Up},
            {1, Position.Right},
            {2, Position.Down},
            {3, Position.Left},
        };

        private static readonly Dictionary<Position, int> PositionToIndexDict = new() 
        {
            {Position.Up, 0},
            {Position.Right, 1},
            {Position.Down, 2},
            {Position.Left, 3},
        };

        public Position GetBestDirection(List<int> visitedCellsRewards)
        {
            // add rewards for this move and visitedCellsRewards element-wise
            int[] totalReward = rewards.Zip(visitedCellsRewards, (a, b) => a + b).ToArray();

            // get the index of direction associated with the highest reward
            int maxIndex = totalReward
                .Select((value, index) => new { value, index })
                .OrderByDescending(item => item.value)
                .First()
                .index;

            // return the position associated with the index
            return IndexToPositionDict[maxIndex];
        }

        public void Reinforce(Position direction, int reward)
        {
            if (PositionToIndexDict.TryGetValue(direction, out int index))
            {
                rewards[index] += reward;
                return;
            }

            throw new Exception($"Provide proper direction. Direction provided: ({direction.x}, {direction.y})");
        }
    }
}