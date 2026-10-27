namespace ConsoleApp3;

public interface IMoveStrategy
{
    Direct ChooseDirect(GMap map, Pos ownPosition, Direct currentDirection, Pos threatPosition);
}