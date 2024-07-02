using JoJoData.Library;

namespace JoJoData.Models;

public class PlayerModel 
{
	public long GuildId { get; set; }
	public long PlayerId { get; set; }
	public int StandId { get; set; }
	public int Level { get; set; }
	public int Experience { get; set; }
	public int BattlesWon { get; set; }

	public PlayerModel() { }
	
	public PlayerModel(Player player) 
	{
		GuildId = (long)player.Guild.Id;
		PlayerId = (long)player.User.Id;
		StandId = player.Stand?.Id ?? 0;
		Level = player.Level;
		Experience = player.Experience;
	}
}