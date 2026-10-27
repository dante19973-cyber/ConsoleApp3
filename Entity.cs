namespace ConsoleApp3;

public abstract class Entity
{
    public Pos Position { get; private set; }
    public Pos PrevPosition { get; private set; }
    public Pos StartPosition { get; }
    public Direct CurrentDirection { get; protected set; }
    public char Symbol { get; }
    public ConsoleColor Color { get; }
    public bool IsActive { get; protected set; }
    protected Entity(Pos startPosition, char symbol, ConsoleColor color)
    {
        StartPosition = startPosition;
        Position = startPosition;
        PrevPosition = startPosition;
        Symbol = symbol;
        Color = color;
        CurrentDirection = Direct.None;
        IsActive = true;
    }
    
    public void RememberPosition()
    {
        PrevPosition = Position;
    }
    
    public void ResetToStart()
    {
        Position = StartPosition;
        PrevPosition = StartPosition;
        CurrentDirection = Direct.None;
    }
    
    protected bool CanMove(GMap map, Direct direction)
    {
        return direction != Direct.None && map.IsWalkable(Position.Move(direction));
    }
    
    protected bool TryStep(GMap map, Direct direction)
    {
        if (!CanMove(map, direction))
        {
            return false;
        }

        Position = Position.Move(direction);
        CurrentDirection = direction;
        return true;
    }
}
