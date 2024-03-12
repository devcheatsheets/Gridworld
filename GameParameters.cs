namespace GridWorld
{
    public static class GameParameters
    {
        public static readonly int MaxMoves = 500;

        // rewards
        public static readonly int GoalReward = 10000;
        public static readonly int MoveIntoWallReward = -1000;
        public static readonly int MoveIntoVisitedCellReward = -70;
        public static readonly int OutsideOfTheMapReward = -10000;
    }
}