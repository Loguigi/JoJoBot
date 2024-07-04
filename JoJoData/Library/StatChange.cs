using DSharpPlus;
using DSharpPlus.Entities;

namespace JoJoData.Library;

public abstract class StatChange 
{
	public abstract string GetDescription(DiscordClient s);
	public abstract DiscordMessageBuilder Execute(BattlePlayer caster, BattlePlayer target);
}

public class Heal(double healPercent) : StatChange 
{
	public readonly double HealPercent = healPercent;

	public override string GetDescription(DiscordClient s)
	{
		return $"{DiscordEmoji.FromName(s, ":heart:")} Heal: {HealPercent * 100}% of max HP";
	}
	
	public override DiscordMessageBuilder Execute(BattlePlayer caster, BattlePlayer target)
	{
		int hpBefore = caster.Hp;
		var heal = (int)Math.Ceiling(caster.MaxHp * HealPercent);
		caster.Heal(heal);

		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(caster.User.GlobalName, "", caster.User.AvatarUrl)
			.WithDescription($"{DiscordEmoji.FromName(caster.Client, ":sparkling_heart:")} **{caster.Stand!.CoolName} heals for `{heal}` HP**")
			.WithColor(DiscordColor.Rose)
			.WithFooter($"{DiscordEmoji.FromName(caster.Client, ":heart:")} {hpBefore} {DiscordEmoji.FromName(caster.Client, ":arrow_right:")} {DiscordEmoji.FromName(caster.Client, ":heart:")} {caster.Hp}", caster.User.AvatarUrl));
	}
}

public class Barrier(double barrier) : StatChange 
{
	public readonly double BarrierAmount = barrier;

	public override string GetDescription(DiscordClient s)
	{
		return $"{DiscordEmoji.FromName(s, ":blue_heart:")} Barrier: {BarrierAmount * 100}% of current HP";
	}
	
	public override DiscordMessageBuilder Execute(BattlePlayer caster, BattlePlayer target)
	{
		int barrierBefore = caster.Barrier;
		var barrier = (int)Math.Ceiling(caster.Hp * BarrierAmount);
		caster.GrantBarrier(barrier);

		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(caster.User.GlobalName, "", caster.User.AvatarUrl)
			.WithDescription($"{DiscordEmoji.FromName(target.Client, ":shield:")} **{caster.Stand!.CoolName} creates a barrier for `{barrier}` HP**")
			.WithColor(DiscordColor.CornflowerBlue)
			.WithFooter($"{DiscordEmoji.FromName(caster.Client, ":blue_heart:")} {barrierBefore} {DiscordEmoji.FromName(caster.Client, ":arrow_right:")} {DiscordEmoji.FromName(caster.Client, ":blue_heart:")} {caster.Barrier}", caster.User.AvatarUrl));
	}
}

public class Strength(double increase) : StatChange 
{
	public readonly double DamageIncrease = increase;

	public override string GetDescription(DiscordClient s)
	{
		// TODO
		return string.Empty;
	}

	public override DiscordMessageBuilder Execute(BattlePlayer caster, BattlePlayer target)
	{
		var minDamageBefore = caster.MinDamage;
		var maxDamageBefore = caster.MaxDamage;
		
		caster.IncreaseAttack((int)Math.Ceiling(caster.MinDamage * DamageIncrease), (int)Math.Ceiling(caster.MaxDamage * DamageIncrease));

		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(caster.User.GlobalName, "", caster.User.AvatarUrl)
			.WithDescription($"💪 **Strength increased**")
			.WithColor(DiscordColor.Orange)
			.WithFooter($"🗡️ {minDamageBefore}-{maxDamageBefore} ➡️ {caster.MinDamage}-{caster.MaxDamage}", caster.User.AvatarUrl));
	}
}

public class Regress(double decrease) : StatChange
{
	public readonly double DamageDecrease = decrease;

	public override string GetDescription(DiscordClient s)
	{
		// TODO
		return string.Empty;
	}

	public override DiscordMessageBuilder Execute(BattlePlayer caster, BattlePlayer target)
	{
		var minDamageBefore = target.MinDamage;
		var maxDamageBefore = target.MaxDamage;

		target.DecreaseAttack((int)Math.Ceiling(target.MinDamage * DamageDecrease), (int)Math.Ceiling(target.MaxDamage * DamageDecrease));

		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
			.WithDescription($"💪 **Strength decreased**")
			.WithColor(DiscordColor.Red)
			.WithFooter($"🗡️ {minDamageBefore}-{maxDamageBefore} ➡️ {target.MinDamage}-{target.MaxDamage}", target.User.AvatarUrl));
	}
}