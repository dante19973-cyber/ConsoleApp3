namespace ConsoleApp3;
public sealed class Unlink : Entity
{
 private int ticksLastMove;
    public string Name { get; }
    public IMoveStrategy MovementStrategy { get; private set; }
    public int MoveIntTicks { get; }
    public int PointsValue { get; }

 
    public Unlink(
        string name,
        Pos startPosition,
        ConsoleColor color,
        IMoveStrategy moveStrategy,
        int moveIntTicks,
        int pointsValue)
        : base(startPosition, GConfig.UnlinkSymbol, color)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(moveStrategy);
        Name = name;
        MovementStrategy = moveStrategy;
        MoveIntTicks = Math.Max(1, moveIntTicks);
        PointsValue = pointsValue;
        ticksLastMove = 0;
    }
    public void SetStrategy(IMoveStrategy moveStrategy)
    {
        ArgumentNullException.ThrowIfNull(moveStrategy);
        MovementStrategy = moveStrategy;
    }
    
    public void Update(GMap map, Pos anakinPos)
    {
        if (!IsActive)
        {
            return;
        }

        ticksLastMove++;

        if (ticksLastMove < MoveIntTicks)
        {
            return;
        }

        ticksLastMove = 0;

        Direct chosen = MovementStrategy.ChooseDirect(map, Position, CurrentDirection, anakinPos);
        TryStep(map, chosen);
    }
    public void Kill()
    {
        IsActive = false;
    }
}