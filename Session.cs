namespace ConsoleApp3;

public class Session
{
    public GMap Map { get; }
    public Anakin Anakin { get; }
    public IReadOnlyList<Unlink> Unlinks { get; }
    public GState State { get; private set; }
    public int MoveCnt { get; private set; }
    public int MoveLimit { get; }

    public Session(GMap map, Anakin anakin, IReadOnlyList<Unlink> unlinks, int moveLimit)
    {
        ArgumentNullException.ThrowIfNull(map);
        ArgumentNullException.ThrowIfNull(anakin);
        ArgumentNullException.ThrowIfNull(unlinks);

        Map = map;
        Anakin = anakin;
        Unlinks = unlinks;
        MoveLimit = moveLimit;
        MoveCnt = 0;
        State = GState.Start;
    }

    public void Move(Direct direction)
    {
        if (State != GState.Start)
        {
            return;
        }

        MoveCnt++;

        Anakin.RememberPosition();
        foreach (Unlink unlink in Unlinks)
        {
            unlink.RememberPosition();
        }

        if (direction != Direct.None)
        {
            Anakin.SetRDirection(direction);
        }
        Anakin.Update(Map);

        foreach (Unlink unlink in Unlinks)
        {
            unlink.Update(Map, Anakin.Position);
        }

        ResCollision();
        EvState();
    }

    public void Quit()
    {
        if (State == GState.Start)
        {
            State = GState.Quit;
        }
    }

    public int GetActUnlink()
    {
        int count = 0;
        foreach (Unlink unlink in Unlinks)
        {
            if (unlink.IsActive)
            {
                count++;
            }
        }
        return count;
    }

    private void ResCollision()
    {
        foreach (Unlink unlink in Unlinks)
        {
            if (unlink.IsActive && HasCollided(unlink))
            {
                Anakin.KillUnlink(unlink);
            }
        }
    }

    private bool HasCollided(Unlink unlink)
    {
        bool sameCell = Anakin.Position == unlink.Position;
        bool swapPlace = unlink.Position == Anakin.PrevPosition && Anakin.Position == unlink.PrevPosition;
        return sameCell || swapPlace;
    }

    private void EvState()
    {
        if (GetActUnlink() == 0)
        {
            State = GState.Won;
        }
        else if (MoveCnt >= MoveLimit)
        {
            State = GState.Lost;
        }
    }
}
