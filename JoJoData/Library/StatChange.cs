using DSharpPlus;
using DSharpPlus.Entities;

namespace JoJoData.Library;

public abstract class StatChange 
{
	public abstract string GetDescription(DiscordClient s);
	public abstract DiscordMessageBuilder Execute(BattlePlayer target);
}

public class Heal(double healPercent) : StatChange 
{
	public readonly double HealPercent = healPercent;

	public override string GetDescription(DiscordClient s)
	{
		return $"{DiscordEmoji.FromName(s, ":heart:")} Heal: {HealPercent * 100}% of max HP";
	}
	
	public override DiscordMessageBuilder Execute(BattlePlayer target)
	{
		int hpBefore = target.Hp;
		var heal = (int)Math.Ceiling(target.MaxHp * HealPercent);
		target.Heal(heal);

		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithDescription($"{DiscordEmoji.FromName(target.Client, ":sparkling_heart:")} **{target.Stand!.CoolName} ({target.User.Mention}) heals for `{heal}` HP**")
			.WithColor(DiscordColor.Rose)
			.WithFooter($"{DiscordEmoji.FromName(target.Client, ":heart:")} {hpBefore} {DiscordEmoji.FromName(target.Client, ":arrow_right:")} {DiscordEmoji.FromName(target.Client, ":heart:")} {target.Hp}", target.User.AvatarUrl));
	}
}

public class Barrier(double barrier) : StatChange 
{
	public readonly double BarrierAmount = barrier;

	public override string GetDescription(DiscordClient s)
	{
		return $"{DiscordEmoji.FromName(s, ":blue_heart:")} Barrier: {BarrierAmount * 100}% of current HP";
	}
	
	public override DiscordMessageBuilder Execute(BattlePlayer target)
	{
		int barrierBefore = target.Barrier;
		var barrier = (int)Math.Ceiling(target.Hp * BarrierAmount);
		target.GrantBarrier(barrier);

		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithDescription($"{DiscordEmoji.FromName(target.Client, ":shield:")} **{target.Stand!.CoolName} ({target.User.Mention}) creates a barrier for `{barrier}` HP**")
			.WithColor(DiscordColor.CornflowerBlue)
			.WithFooter($"{DiscordEmoji.FromName(target.Client, ":blue_heart:")} {barrierBefore} {DiscordEmoji.FromName(target.Client, ":arrow_right:")} {DiscordEmoji.FromName(target.Client, ":blue_heart:")} {target.Barrier}", target.User.AvatarUrl));
	}
}