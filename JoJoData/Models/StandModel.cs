namespace JoJoData.Models;

public class StandModel {
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public int Part { get; set; }
	public int BaseHp { get; set; }
	public int BaseMinDamage { get; set; }
	public int BaseMaxDamage { get; set; }
	public int Speed { get; set; }
	public string Ability1 { get; set; } = string.Empty;
	public string Ability2 { get; set; } = string.Empty;
	public string Ability3 { get; set; } = string.Empty;
	public string Ability4 { get; set; } = string.Empty;
	public int Stars { get; set; }
	public string ImageUrl { get; set; } = string.Empty;
	public string? Passive { get; set; }
}