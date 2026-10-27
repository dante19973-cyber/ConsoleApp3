namespace ConsoleApp3;

public sealed class GMap
{
    private const char WallChar = '#';
    private const char PlayerSpawnChar = 'A';
    private const char UnlinkSpawnChar = 'U';
    private readonly CellType[,] cells;
    private readonly List<Pos> unlinkSpawns = new();
    public int Width { get; }
    public int Height { get; }
    public string LevelName { get; }
    public Pos PlayerSpawn { get; }
    public IReadOnlyList<Pos> UnlinkSpawns => unlinkSpawns;
    public GMap(string levelName, IReadOnlyList<string> lines)
    {
        ArgumentNullException.ThrowIfNull(levelName);
        ArgumentNullException.ThrowIfNull(lines);

        if (lines.Count == 0)
        {
            throw new InvalidDataException("The maze layout is empty.");
        }
        LevelName = levelName;
        Height = lines.Count;
        Width = lines[0].Length;
        cells = new CellType[Height, Width];
        bool playerFound = false;
        Pos playerSpawn = new Pos(0, 0);

        for (int row = 0; row < Height; row++)
        {
            string line = lines[row];
            if (line.Length != Width)
            {
                throw new InvalidDataException($"Row {row + 1} has length {line.Length}, expected {Width}.");
            }
            for (int column = 0; column < Width; column++)
            {
                char symbol = line[column];
                Pos current = new Pos(row, column);

                switch (symbol)
                {
                    case WallChar:
                        cells[row, column] = CellType.Wall;
                        break;

                    case ' ':
                        cells[row, column] = CellType.Empty;
                        break;

                    case PlayerSpawnChar:
                        if (playerFound)
                        {
                            throw new InvalidDataException("The maze has more than one player position ('P').");
                        }
                        playerFound = true;
                        playerSpawn = current;
                        cells[row, column] = CellType.Empty;
                        break;

                    case UnlinkSpawnChar:
                        unlinkSpawns.Add(current);
                        cells[row, column] = CellType.Empty;
                        break;

                    default:
                        throw new InvalidDataException($"Unknown symbol '{symbol}' (row {row + 1}, column {column + 1}).");
                }
            }
        }

        if (!playerFound)
        {
            throw new InvalidDataException("The maze has no player position ('P').");
        }

        if (unlinkSpawns.Count == 0)
        {
            throw new InvalidDataException("The maze has no unlinks ('G').");
        }

        PlayerSpawn = playerSpawn;
        EnsureAllCellsReachable();
    }

    public bool IsInBounds(Pos position)
    {
        return position.Row >= 0 && position.Row < Height
            && position.Column >= 0 && position.Column < Width;
    }
    public bool IsWalkable(Pos position)
    {
        return IsInBounds(position) && cells[position.Row, position.Column] == CellType.Empty;
    }
    public CellType GetCell(Pos position)
    {
        return IsInBounds(position) ? cells[position.Row, position.Column] : CellType.Wall;
    }
    public int CountWalkableNeighbors(Pos position)
    {
        int count = 0;

        foreach (Direct direction in DirectExtensions.MovingDirect)
        {
            if (IsWalkable(position.Move(direction)))
            {
                count++;
            }
        }

        return count;
    }
    public int[,] CalculateDistancesFrom(Pos origin)
    {
        int[,] distances = new int[Height, Width];
        for (int row = 0; row < Height; row++)
        {
            for (int column = 0; column < Width; column++)
            {
                distances[row, column] = -1;
            }
        }
        if (!IsWalkable(origin))
        {
            return distances;
        }

        Queue<Pos> queue = new Queue<Pos>();
        distances[origin.Row, origin.Column] = 0;
        queue.Enqueue(origin);

        while (queue.Count > 0)
        {
            Pos current = queue.Dequeue();
            int currentDistance = distances[current.Row, current.Column];

            foreach (Direct direction in DirectExtensions.MovingDirect)
            {
                Pos next = current.Move(direction); 
                if (IsWalkable(next) && distances[next.Row, next.Column] == -1)
                {
                    distances[next.Row, next.Column] = currentDistance + 1;
                    queue.Enqueue(next);
                }
            }
        }
        return distances;
    }

    private void EnsureAllCellsReachable()
    {
        int[,] distances = CalculateDistancesFrom(PlayerSpawn);

        for (int row = 0; row < Height; row++)
        {
            for (int column = 0; column < Width; column++)
            {
                if (cells[row, column] == CellType.Empty && distances[row, column] == -1)
                {
                    throw new InvalidDataException(
                        $"Cell (row {row + 1}, column {column + 1}) is unreachable.");
                }
            }
        }
    }
}
