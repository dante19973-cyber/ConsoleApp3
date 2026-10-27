namespace ConsoleApp3;

public sealed class Anakin : Entity
{
    public string PlayerName { get; }
    public int Score { get; private set; }
    public int UnlinksKilled { get; private set; }
    public Direct RequestedDirect{ get; private set; }
    public int StepsMade { get; private set; }
    public Anakin(Pos startPosition, string playerName)
        : base(startPosition, GConfig.AnakinSymbol, GConfig.AnakinColor)
    {
        ArgumentNullException.ThrowIfNull(playerName);

        PlayerName = playerName;
        Score = 0;
        UnlinksKilled = 0;
        RequestedDirect = Direct.None;
        StepsMade = 0;
    }

    public void SetRDirection(Direct direction)
    {
        RequestedDirect = direction;
    }
    public void Update(GMap map)
    {
        if (RequestedDirect != Direct.None && TryStep(map, RequestedDirect))
        {
            RequestedDirect = Direct.None;
            StepsMade++;
            return;
        }

        if (TryStep(map, CurrentDirection))
        {
            StepsMade++;
        }
    }

    public void KillUnlink(Unlink unlink)
    {
        ArgumentNullException.ThrowIfNull(unlink);

        unlink.Kill();
        UnlinksKilled++;
        Score += unlink.PointsValue;
    }
}
