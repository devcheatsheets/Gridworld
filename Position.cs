namespace GridWorld
{
    public struct Position
    {
        public int x;
        public int y;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Position Up => new Position(0, 1);
        public static Position Right => new Position(1, 0);
        public static Position Down => new Position(0, -1);
        public static Position Left => new Position(-1, 0);

        public static bool operator == (Position lhs, Position rhs) => lhs.x == rhs.x && lhs.y == rhs.y;
        public static bool operator != (Position lhs, Position rhs) => lhs.x != rhs.x || lhs.y != rhs.y;

        public static Position operator +(Position lhs, Position rhs) => new(lhs.x + rhs.x, lhs.y + rhs.y);
    }
}