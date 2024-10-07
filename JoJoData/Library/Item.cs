using System.Text;
using DSharpPlus.Entities;
using JoJoData.Controllers;
using JoJoData.Helpers;

namespace JoJoData.Library;

public abstract class Item(int count)
{
	public int Count { get; set; } = count;
	public abstract DiscordMessageBuilder Use(Player player);
}

public abstract class Arrow(int count) : Item(count)
{
	public abstract Dictionary<int, double> Rarities { get; protected set; }

	public override DiscordMessageBuilder Use(Player player)
	{
		player.UseItem(this);
		Stand stand = RollStand(RollRarity());
		StringBuilder stars = new();
		for (var i = 1; i <= stand.Stars; ++i) 
		{
			stars.Append("⭐️");
		}
		
		var embed = new DiscordEmbedBuilder()
			.WithAuthor(player.User.GlobalName, "", player.User.AvatarUrl)
			.WithTitle($"{stand.CoolName} {stars}")
			.AddField("Stats", $"❤️ {stand.BaseHp}\n⚔️`{stand.BaseMinDamage}`-`{stand.BaseMaxDamage}`\n💨`{stand.Speed}`", true)
			.AddField("Abilities", $"{stand.Ability1.CoolName}\n{stand.Ability2.CoolName}\n{stand.Ability3.CoolName}\n{stand.Ability4.CoolName}", true)
			.WithImageUrl(stand.ImageUrl)
			.WithColor(DiscordColor.Gold)
			.WithFooter($"Remaining stand arrows: {Count}");

		var acceptBtn = new DiscordButtonComponent(DiscordButtonStyle.Success, $"{IDHelper.Inventory.ARROW_STAND_ACCEPT}\\{player.User.Id}\\{stand.Id}", "✅");
		var declineBtn = new DiscordButtonComponent(DiscordButtonStyle.Danger, $"{IDHelper.Inventory.ARROW_STAND_DECLINE}\\{player.User.Id}\\{stand.Id}", "❌");

		var msg = new DiscordMessageBuilder()
			.WithContent(player.User.Mention)
			.AddMention(new UserMention(player.User))
			.AddEmbed(embed)
			.AddComponents(new DiscordComponent[] {acceptBtn, declineBtn});

		return msg;
	}

	protected int RollRarity() => JoJo.RNG.NextDouble() switch
	{
		double i when i >= 0 && i < Rarities[1] => 1,
		double i when i >= Rarities[1] && i < Rarities[2] => 2,
		double i when i >= Rarities[2] && i < Rarities[3] => 3,
		double i when i >= Rarities[3] && i < Rarities[4] => 4,
		double i when i >= Rarities[4] && i <= 1 => 5,
		_ => throw new Exception("you can't do math loser")
	};
	
	protected Stand RollStand(int rarity) 
	{
		List<Stand> stands = StandLoader.Stands.Values.Where(x => x.Stars == rarity).ToList();
		return stands[JoJo.RNG.Next(0, stands.Count)];
	}
}

public class StandArrow(int count) : Arrow(count)
{
	public override Dictionary<int, double> Rarities { get; protected set; } = new(5)
	{
		{1, 0.4}, // 40% chance
		{2, 0.7}, // 30%
		{3, 0.9}, // 20%
		{4, 0.99}, // 9%
		{5, 1} // 1%
	};
}