namespace ConsoleApp3;

public enum Direct
{
    None,
    Up,
    Down,
    Left,
    Right
}
public static class DirectExtensions
{
    public static readonly Direct[] MovingDirect =
    {
        Direct.Up, Direct.Down, Direct.Left, Direct.Right
    };

    public static Direct Opposite(this Direct direction)
    {
        return direction switch
        {
            Direct.Up => Direct.Down,
            Direct.Down => Direct.Up,
            Direct.Left => Direct.Right,
            Direct.Right => Direct.Left,
            _ => Direct.None
        };
    }
}
