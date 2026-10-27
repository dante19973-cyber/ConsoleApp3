using System.Text;

namespace ConsoleApp3;

public static class Program
{
    private static readonly string[] Layout =
    {
        "#####################",
        "#U        #        U#",
        "# ### ### # ### ### #",
        "#                   #",
        "# ### # ##### # ### #",
        "#     #       #     #",
        "##### # ##### # #####",
        "#         A         #",
        "##### # ##### # #####",
        "#     #       #     #",
        "# ### # ##### # ### #",
        "#                   #",
        "# ### ### # ### ### #",
        "#U        #        U#",
        "#####################"
    };

    private const int TickDelayMs = 150;

    public static int Main()
    {
        int exitCode = 0;

        try
        {
            PrepareConsole();

            if (Console.IsInputRedirected)
            {
                return 1;
            }

            RunGame();
        }
        catch (Exception ex)
        {
            Console.ResetColor();
            try
            {
                Console.CursorVisible = true;
            }
            catch
            {
            }

            Console.WriteLine();
            Console.WriteLine("Unexpected error:");
            Console.WriteLine(ex);
            exitCode = 1;
        }

        WaitForKey();
        return exitCode;
    }

    private static void WaitForKey()
    {
        if (Console.IsInputRedirected)
        {
            return;
        }

        try
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to close the window...");

            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }

            Console.ReadKey(true);
        }
        catch (InvalidOperationException)
        {
        }
    }
    private static void RunGame()
    {
        Session session = CreateSession();

        Console.CursorVisible = false;
        Console.Clear();

        try
        {
            Render(session);

            while (session.State == GState.Start)
            {
                Direct direction = ReadDirection(out bool quit);

                if (quit)
                {
                    session.Quit();
                    break;
                }

                session.Move(direction);
                Render(session);
                Thread.Sleep(TickDelayMs);
            }
        }
        finally
        {
            Console.CursorVisible = true;
            Console.ResetColor();
        }

        Console.SetCursorPosition(0, session.Map.Height + 4);
        string result = session.State switch
        {
            GState.Won => "You win! All unlinks are killed.",
            GState.Lost => "Time is up. You lose.",
            _ => "Game exited."
        };
        Console.WriteLine(result);
        Console.WriteLine($"Score: {session.Anakin.Score}, killed: {session.Anakin.UnlinksKilled}, steps: {session.Anakin.StepsMade}");
    }

    private static Direct ReadDirection(out bool quit)
    {
        quit = false;
        Direct result = Direct.None;

        while (Console.KeyAvailable)
        {
            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    result = Direct.Up;
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    result = Direct.Down;
                    break;
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    result = Direct.Left;
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    result = Direct.Right;
                    break;
                case ConsoleKey.Escape:
                    quit = true;
                    break;
            }
        }

        return result;
    }

    private static void Render(Session session)
    {
        int width = session.Map.Width;
        int lineWidth = Math.Max(width, 50);

        Console.SetCursorPosition(0, 0);
        Console.ResetColor();
        Console.WriteLine(GConfig.WindowTitle.PadRight(lineWidth));
        Console.WriteLine($"Score: {session.Anakin.Score,-5} Unlinks: {session.GetActUnlink()}/{session.Unlinks.Count}  Move: {session.MoveCnt}/{session.MoveLimit}".PadRight(lineWidth));

        for (int row = 0; row < session.Map.Height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                (char symbol, ConsoleColor color) = GetCellView(session, new Pos(row, column));
                Console.ForegroundColor = color;
                Console.Write(symbol);
            }

            Console.ResetColor();
            Console.WriteLine();
        }

        Console.ResetColor();
        Console.WriteLine("Arrows or WASD - move, Esc - quit".PadRight(lineWidth));
    }

    private static (char Symbol, ConsoleColor Color) GetCellView(Session session, Pos position)
    {
        if (session.Anakin.Position == position)
        {
            return (session.Anakin.Symbol, session.Anakin.Color);
        }

        foreach (Unlink unlink in session.Unlinks)
        {
            if (unlink.IsActive && unlink.Position == position)
            {
                return (unlink.Symbol, unlink.Color);
            }
        }

        return session.Map.GetCell(position) == CellType.Wall
            ? ('█', ConsoleColor.DarkBlue)
            : (' ', ConsoleColor.Gray);
    }

    private static void PrepareConsole()
    {
        try
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = GConfig.WindowTitle;
        }
        catch (IOException)
        {
        }
        catch (PlatformNotSupportedException)
        {
        }
    }

    private static Session CreateSession()
    {
        GMap map = new GMap("Test maze", Layout);
        Anakin anakin = new Anakin(map.PlayerSpawn, "Player");
        List<Unlink> unlinks = new List<Unlink>();

        for (int index = 0; index < map.UnlinkSpawns.Count; index++)
        {
            unlinks.Add(new Unlink(
                GConfig.UnlinkNames[index % GConfig.UnlinkNames.Length],
                map.UnlinkSpawns[index],
                GConfig.UnlinkColors[index % GConfig.UnlinkColors.Length],
                new FleeMove(),
                GConfig.UnlinkMoveIntervalTicks,
                GConfig.UnlinkCatchPoints));
        }

        return new Session(map, anakin, unlinks, GConfig.MaxTicks);
    }
}
