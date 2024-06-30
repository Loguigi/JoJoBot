using JoJoData.Library;

namespace JoJoData.Models;

public class AbilityModel
{
	public string Id { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public int MpCost { get; set; }

	public AbilityModel() { }

	public AbilityModel(Ability ability)
	{
		Id = ability.GetType().Name;
		Name = ability.Name;
		Description = ability.Description is null ? null : ability.Description;
		MpCost = ability.MpCost;
	}
}