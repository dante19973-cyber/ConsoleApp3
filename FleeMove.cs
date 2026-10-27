namespace ConsoleApp3;
public sealed class FleeMove: IMoveStrategy
{
    private const double DefRandomMoveC = 0.10;
    private const double DistWeight = 10.0;
    private const double ExitWeight = 3.0;
    private const double MomentumB = 1.0;
    private readonly Random random;
    private readonly double randomMoveChance;
    public FleeMove()
        : this(new Random(), DefRandomMoveC)
    {
    }
    public FleeMove(Random random, double randomMoveChance)
    {
        ArgumentNullException.ThrowIfNull(random);
        this.random = random;
        this.randomMoveChance = Math.Clamp(randomMoveChance, 0.0, 1.0);
    }
    public Direct ChooseDirect(GMap map, Pos ownPosition, Direct currentDirection, Pos threatPosition)
    {
        List<Direct> options = new List<Direct>();

        foreach (Direct direction in DirectExtensions.MovingDirect)
        {
            if (map.IsWalkable(ownPosition.Move(direction)))
            {
                options.Add(direction);
            }
        }
        if (options.Count == 0)
        {
            return Direct.None;
        }
        if (random.NextDouble() < randomMoveChance)
        {
            return options[random.Next(options.Count)];
        }
        int[,] distancesFromThreat = map.CalculateDistancesFrom(threatPosition);

        Direct bestDirect = options[0];
        double bestScore = double.MinValue;

        foreach (Direct direction in options)
        {
            Pos next = ownPosition.Move(direction);
            int distance = distancesFromThreat[next.Row, next.Column];

            double score = distance * DistWeight
                         + map.CountWalkableNeighbors(next) * ExitWeight
                         + (direction == currentDirection ? MomentumB : 0.0)
                         + random.NextDouble();
            if (score > bestScore)
            {
                bestScore = score;
                bestDirect = direction;
            }
        }
        return bestDirect;
    }
}
