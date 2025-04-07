namespace JoJoLibrary.Models;

public class PlayerModel 
{
	public long GuildId { get; set; }
	public long PlayerId { get; set; }
	public int? StandId { get; set; } = null;
	public int Level { get; set; }
	public int Experience { get; set; }
	public int BattlesWon { get; set; }
	public int BattlesLost { get; set; }

	public PlayerModel() { }

	public PlayerModel(Player player) 
	{
		GuildId = (long)player.Guild.Id;
		PlayerId = (long)player.User.Id;
		StandId = player.Stand?.Id ?? null;
		Level = player.Level;
		Experience = player.Experience;
		BattlesWon = player.BattlesWon;
		BattlesLost = player.BattlesLost;
	}
}