using JoJoData.Abilities;
using JoJoData.Models;

namespace JoJoData.Library;

public class Stand(StandModel model, Passive psv, Ability a1, Ability a2, Ability a3, Ability a4) {
	#region Properties
	public int Id { get; private set; } = model.Id;
	public string Name { get; private set; } = model.Name;
	public string CoolName => $"「{Name}」";
	public int BaseHp { get; private set; } = model.BaseHp;
	public int BaseMinDamage { get; private set; } = model.BaseMinDamage;
	public int BaseMaxDamage { get; private set; } = model.BaseMaxDamage;
	public int Speed { get; private set; } = model.Speed;
	public Passive Passive { get; private set; } = psv;
	public Ability Ability0 { get; private set; } = new StandardAttack();
	public Ability Ability1 { get; private set; } = a1;
	public Ability Ability2 { get; private set; } = a2;
	public Ability Ability3 { get; private set; } = a3;
	public Ability Ability4 { get; private set; } = a4;
	public int Stars { get; private set; } = model.Stars;
	public string ImageUrl { get; private set; } = model.ImageUrl;

	#endregion
}