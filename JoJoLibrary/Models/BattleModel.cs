namespace JoJoData.Models;

public class BattleModel {
	public long GuildId { get; set; }
	public long Player1Id { get; set; }
	public long Player2Id { get; set; }
	public int TurnCount { get; set; }
	public DateTime BattleStart { get; set; }
	public DateTime? BattleEnd { get; set; }

	public BattleModel() { }
	
	public BattleModel(ulong guildId, ulong player1Id, ulong player2Id, int turnCount, DateTime battleStart, DateTime? battleEnd) 
	{
		GuildId = (long)guildId;
		Player1Id = (long)player1Id;
		Player2Id = (long)player2Id;
		TurnCount = turnCount;
		BattleStart = battleStart;
		BattleEnd = battleEnd;
	}
}