namespace ConsoleApp3;
public readonly struct Pos : IEquatable<Pos>
{
    public int Row { get; }

    public int Column { get; }

    public Pos(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public Pos Move(Direct direction)
    {
        return direction switch
        {
            Direct.Up => new Pos(Row - 1, Column),
            Direct.Down => new Pos(Row + 1, Column),
            Direct.Left => new Pos(Row, Column - 1),
            Direct.Right => new Pos(Row, Column + 1),
            _ => this
        };
    }
    public bool Equals(Pos other) => Row == other.Row && Column == other.Column;

    public override bool Equals(object? obj) => obj is Pos other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Row, Column);

    public static bool operator ==(Pos left, Pos right) => left.Equals(right);

    public static bool operator !=(Pos left, Pos right) => !left.Equals(right);

    public override string ToString() => $"({Row}, {Column})";
}
