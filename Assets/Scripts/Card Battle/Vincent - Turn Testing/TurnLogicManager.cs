//Written by: Vincent Luong

public class TurnLogicManager
{
    public int trackTurn {get; private set;}

    public TurnLogicManager()
    {
        trackTurn = 0; //initial turn starts at 0
    }

    public void NextTurn()
    {
        trackTurn++; //increment turn count by 1
    }

    public bool IsPlayerTurn()
    {
        return trackTurn % 2 == 1; //odd turns are player turns (turn 1, 3, 5, ...)
    }
}

