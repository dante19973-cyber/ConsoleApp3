namespace ConsoleApp3;

public static class GConfig
{
   
    public const string WindowTitle = "ORDER 666";
    
    public const int MaxTicks = 500;

    public const int UnlinkMoveIntervalTicks = 2;

    public const int UnlinkCatchPoints = 100;

    public const char AnakinSymbol = '●';

    public const char UnlinkSymbol = '☻';

    public const ConsoleColor AnakinColor = ConsoleColor.Yellow;

    public static readonly string[] UnlinkNames = { "Blinky", "Pinky", "Inky", "Clyde" };

    public static readonly ConsoleColor[] UnlinkColors =
    {
        ConsoleColor.Red, ConsoleColor.Magenta, ConsoleColor.Cyan, ConsoleColor.Green
    };
}
