namespace GridWorld
{
    public class Grid
    {
        private const char EmptyTile = '.';
        private const char WallTile = '#';
        private const char PlayerTile = 'P';
        private const char GoalTile = 'G';
        private const char InvalidTile = 'X';

        private int _gridWidth = 0;
        private int _gridHeight = 0;

        private char[,] _grid;

        private Position _playerPosition = new(0, 0);
        private Position _goalPosition = new(0, 0);

        private int[,] _visitedCells;
        private Dictionary<string, MoveReward>  _navigationMemory = new();

        private static readonly List<Position> directions = new()
        {
            new Position(0, 1), // up
            new Position(1, 0) , // right
            new Position(-1, 0), // down
            new Position(0, -1), // left
        };

        private static readonly Random random = new Random();

        public bool Update()
        {
            MovePlayer();
            DisplayGrid();

            if (_playerPosition == _goalPosition)
            {
                return true;
            }
            return false;
        }

        public void LoadGridFromFile(string path)
        {
            string[] lines = File.ReadAllLines(path);

            _gridHeight = lines.Length;

            if (_gridHeight <= 1)
                throw new Exception("Provide a map with more than 1 row.");

            _gridWidth = lines[0].Length;

            _grid = new char[_gridHeight, _gridWidth];
            _visitedCells = new int[_gridHeight, _gridWidth];

            for (int i = 0; i < _gridHeight; i++)
            {
                if (lines[i].Length != _gridWidth)
                    throw new Exception($"Invalid width on line {i + 1} in file.");

                for (int j = 0; j < _gridWidth; j++)
                {
                    char cell = lines[i][j];
                    _visitedCells[i, j] = 0;

                    switch (cell)
                    {
                        case PlayerTile:
                            _playerPosition = new Position(i, j);
                            _grid[i, j] = PlayerTile;
                            break;
                        case GoalTile:
                            _goalPosition = new Position(i, j);
                            _grid[i, j] = GoalTile;
                            break;
                        case WallTile:
                            _grid[i, j] = WallTile;
                            break;
                        case EmptyTile:
                            _grid[i, j] = EmptyTile;
                            break;
                        default:
                            throw new Exception($"Invalid character '{cell}' on line {i + 1}, position {j + 1} in file.");
                    }
                }
            }
        }

        public void DisplayGrid()
        {
            Console.Clear();
            for (int i = 0; i < _gridHeight; i++)
            {
                for (int j = 0; j < _gridWidth; j++)
                {
                    Console.Write(_grid[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void MovePlayer()
        {
            var surroundingCells = GetSurroundingCells(_playerPosition);
            var surroundingCellsRewards = GetSurroundingCellsRewards(_playerPosition);

            // if the player has been in the same situation, read the learned rewards associated with each potential move 
            if (_navigationMemory.TryGetValue(surroundingCells, out var moveReward))
            {
                var direction = moveReward.GetBestDirection(surroundingCellsRewards);

                UpdatePlayerPosition(direction, surroundingCells, moveReward);
            }
            else // otherwise, make a random move and remember the consequences for later
            {
                var index = random.Next(directions.Count);
                var direction = directions[index];

                UpdatePlayerPosition(direction, surroundingCells);
            }
        }

        private void UpdatePlayerPosition(Position direction, string surroundingCells, MoveReward? moveReward = null)
        {
            var newPosition = _playerPosition + direction;
            if (IsValidCoordinate(newPosition))
            {
                if (_grid[newPosition.x, newPosition.y] == WallTile)
                {
                    // punish for trying to move into a wall
                    // if such move is not yet in the memory, remember the consequences
                    if (moveReward == null)
                    {
                        moveReward = new MoveReward();
                        moveReward.Reinforce(direction, GameParameters.MoveIntoWallReward);
                        _navigationMemory.Add(surroundingCells, moveReward);
                        return;
                    }
                    else
                    {
                        moveReward.Reinforce(direction, GameParameters.MoveIntoWallReward);
                        _navigationMemory[surroundingCells] = moveReward;
                        return;
                    }
                }

                UpdateGridAfterPlayerMove(newPosition);
            }
        }

        private void UpdateGridAfterPlayerMove(Position newPosition)
        {
            _grid[_playerPosition.x, _playerPosition.y] = EmptyTile;
            _playerPosition = newPosition;
            _grid[_playerPosition.x, _playerPosition.y] = PlayerTile;
            _visitedCells[newPosition.x, newPosition.y] += GameParameters.MoveIntoVisitedCellReward;
        }

        private string GetSurroundingCells(Position playerPos)
        {
            var surroundingCells = new List<char>();

            var potentialCoordinates = GetPotentialCoordinates(playerPos.x, playerPos.y);

            foreach (var coord in potentialCoordinates)
            {
                if (IsValidCoordinate(coord))
                    surroundingCells.Add(_grid[coord.x, coord.y]);
                else
                    surroundingCells.Add(InvalidTile);
            }

            return new string(surroundingCells.ToArray());
        }

        private List<int> GetSurroundingCellsRewards(Position playerPos)
        {
            var surroundingCellsRewards = new List<int>();

            var potentialCoordinates = GetPotentialCoordinates(playerPos.x, playerPos.y);

            foreach (var coord in potentialCoordinates)
            {
                if (IsValidCoordinate(coord))
                {
                    if (coord == _goalPosition)
                        surroundingCellsRewards.Add(GameParameters.GoalReward);
                    else
                        surroundingCellsRewards.Add(_visitedCells[coord.x, coord.y]);
                }
                else
                {
                    surroundingCellsRewards.Add(GameParameters.OutsideOfTheMapReward);
                }
            }

            return surroundingCellsRewards;
        }

        private List<Position> GetPotentialCoordinates(int x, int y)
        {
            return new List<Position>
            {
                new Position(x, y + 1), // up
                new Position(x + 1, y), // right
                new Position(x, y - 1), // down
                new Position(x - 1, y), // left
            };
        }

        private bool IsValidCoordinate(Position position)
        {
            return position.x >= 0 && position.x < _gridHeight && position.y >= 0 && position.y < _gridWidth;
        }
    }
}