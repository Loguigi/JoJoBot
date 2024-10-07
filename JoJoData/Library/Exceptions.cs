namespace JoJoData.Library;

public abstract class BattleException(Turn turn) : Exception
{
    public Turn Turn = turn;
}

public class OnDeathException(Turn turn, BattlePlayer deadPlayer) : BattleException(turn)
{
    public readonly BattlePlayer DeadPlayer = deadPlayer;
}

public class TurnSkipException(Turn turn, BattlePlayer playerSkipped) : BattleException(turn)
{
    public BattlePlayer SkippedPlayer = playerSkipped;
}