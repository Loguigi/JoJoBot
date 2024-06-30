namespace JoJoData.Models;

public class RoundModel {
	public int Id { get; set; }
	public int BattleId { get; set; }
	public int RoundNum { get; set; }
	public long CurrentPlayerId { get; set; }
	public string AbilityUsed { get; set; } = string.Empty;
	public int DamageDone { get; set; }
	public string? BuffApplied { get; set; }
	public string? StatusApplied { get; set; }
	
}