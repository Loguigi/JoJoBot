using System.Text;
using DSharpPlus.Entities;
using JoJoLibrary.Data;
using JoJoLibrary.Models;

namespace JoJoLibrary;

public class Stand(StandModel model, Passive? psv, Ability a1, Ability a2, Ability a3, Ability a4) 
{
	#region Properties
	public int Id { get; private set; } = model.Id;
	public string Name { get; } = model.Name;
	public string CoolName => $"「{Name}」";
	public int Part { get; } = model.Part;
	public int BaseHp { get; } = model.BaseHp;
	public int BaseMinDamage { get; } = model.BaseMinDamage;
	public int BaseMaxDamage { get; } = model.BaseMaxDamage;
	public int Speed { get; } = model.Speed;
	public Passive? Passive { get; } = psv;
	public Ability Ability0 { get; private set; } = new StandardAttack();
	public Ability Ability1 { get; } = a1;
	public Ability Ability2 { get; } = a2;
	public Ability Ability3 { get; } = a3;
	public Ability Ability4 { get; } = a4;
	public int Stars { get; } = model.Stars;
	public string ImageUrl { get; } = model.ImageUrl;
	#endregion
	
	#region Public Methods

	public DiscordMessageBuilder FormatDescription(BattlePlayer? player = null)
	{
		StringBuilder description = new("## Stats\n");
		if (player is not null)
		{
			description.Append(player.FormatBattleDetails());
		}
		else
		{
			description.AppendLine($"### ❤️ HP: {BaseHp}");
			description.AppendLine($"### ⚔️ Damage: `{BaseMinDamage}` - `{BaseMaxDamage}`");
			description.AppendLine($"### 🍃 Speed: `{Speed}`");
		}
		
		if (Passive is not null)
		{
			description.AppendLine("### ⚡️ Passive");
			description.AppendLine($"* `{Passive.Name}`");
			description.Append(Passive.GetLongDescription(this, player));
		}

		description.AppendLine("\n## Abilities");
		description.AppendLine($"### 1️⃣ {Ability1.CoolName} 💎 {Ability1.MpCost} MP");
		description.Append(Ability1.FormatLongDescription(this, player));
		description.AppendLine();
		description.AppendLine($"### 2️⃣ {Ability2.CoolName} 💎 {Ability2.MpCost} MP");
		description.Append(Ability2.FormatLongDescription(this, player));
		description.AppendLine();
		description.AppendLine($"### 3️⃣ {Ability3.CoolName} 💎 {Ability3.MpCost} MP");
		description.Append(Ability3.FormatLongDescription(this, player));
		description.AppendLine();
		description.AppendLine($"### 4️⃣ {Ability4.CoolName} 💎 {Ability4.MpCost} MP");
		description.Append(Ability4.FormatLongDescription(this, player));
		string stars = string.Empty;

		for (var i = 1; i <= Stars; i++)
		{
			stars += "⭐️";
		}

		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithTitle($"{CoolName} {stars}")
			.WithDescription(description.ToString())
			.WithThumbnail(ImageUrl)
			.WithColor(DiscordColor.Goldenrod)
			.WithFooter($"JoJo's Bizarre Adventure: Part {Part}"));
	}
	#endregion
}