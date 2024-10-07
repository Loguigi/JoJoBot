using JoJoData.Controllers;

namespace JoJoData.Models;

public class BattleModel {
	public long GuildId { get; set; }
	public long Player1Id { get; set; }
	public long Player2Id { get; set; }
	public int TurnCount { get; set; }
	public DateTime BattleStart { get; set; }
	public DateTime? BattleEnd { get; set; }

	public BattleModel() { }
	
	public BattleModel(BattleController battle) 
	{
		GuildId = (long)battle.Guild.Id;
		Player1Id = (long)battle.Player1.User.Id;
		Player2Id = (long)battle.Player2.User.Id;
		TurnCount = battle.CurrentTurn;
		BattleStart = battle.BattleStart;
		BattleEnd = battle.BattleEnd;
	}
}