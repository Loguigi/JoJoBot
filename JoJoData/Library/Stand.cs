using System.Text;
using DSharpPlus.Entities;
using JoJoData.Abilities;
using JoJoData.Controllers;
using JoJoData.Data;
using JoJoData.Models;

namespace JoJoData.Library;

public class Stand(StandModel model, Passive? psv, Ability a1, Ability a2, Ability a3, Ability a4) 
{
	#region Properties
	public int Id { get; private set; } = model.Id;
	public string Name { get; private set; } = model.Name;
	public string CoolName => $"「{Name}」";
	public int Part { get; private set; } = model.Part;
	public int BaseHp { get; private set; } = model.BaseHp;
	public int BaseMinDamage { get; private set; } = model.BaseMinDamage;
	public int BaseMaxDamage { get; private set; } = model.BaseMaxDamage;
	public int Speed { get; private set; } = model.Speed;
	public Passive? Passive { get; private set; } = psv;
	public Ability Ability0 { get; private set; } = new StandardAttack();
	public Ability Ability1 { get; private set; } = a1;
	public Ability Ability2 { get; private set; } = a2;
	public Ability Ability3 { get; private set; } = a3;
	public Ability Ability4 { get; private set; } = a4;
	public int Stars { get; private set; } = model.Stars;
	public string ImageUrl { get; private set; } = model.ImageUrl;
	#endregion
	
	#region Public Methods

	public DiscordMessageBuilder FormatDescription(BattlePlayer? player = null)
	{
		StringBuilder description = new();
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

		description.AppendLine($"### 1️⃣ {Ability1.CoolName}");
		description.Append(Ability1.FormatLongDescription(this, player));
		description.AppendLine($"### 2️⃣ {Ability2.CoolName}");
		description.Append(Ability2.FormatLongDescription(this, player));
		description.AppendLine($"### 3️⃣ {Ability3.CoolName}");
		description.Append(Ability3.FormatLongDescription(this, player));
		description.AppendLine($"### 4️⃣ {Ability4.CoolName}");
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