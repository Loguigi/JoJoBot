using DSharpPlus;
using DSharpPlus.Entities;

namespace JoJoData.Library;

public abstract class StatChange  : BattleAction
{
	public abstract string Name { get; }
	public virtual string ShortDescription => Name;
	public abstract override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target);
}

public class Heal(double healPercent) : StatChange 
{
	public override string Name => "💖 Heal";
	public override string ShortDescription => base.ShortDescription + $" {HealPercent * 100}% of Max HP";
	public readonly double HealPercent = healPercent;
	
	public override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		var heal = (int)Math.Ceiling(caster.MaxHp * HealPercent);
		caster.Heal(heal, out var hpBefore);

		turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(caster.User.GlobalName, "", caster.User.AvatarUrl)
			.WithDescription($"{DiscordEmoji.FromName(caster.Client, ":sparkling_heart:")} **{caster.Stand!.CoolName} heals for `{heal}` HP**")
			.WithColor(DiscordColor.Rose)
			.WithFooter($"{DiscordEmoji.FromName(caster.Client, ":heart:")} {hpBefore} {DiscordEmoji.FromName(caster.Client, ":arrow_right:")} {DiscordEmoji.FromName(caster.Client, ":heart:")} {caster.Hp}", caster.User.AvatarUrl)));
	}
}

public class Barrier(double barrier) : StatChange 
{
	public override string Name => "💙 Barrier";
	public override string ShortDescription => base.ShortDescription + $" {BarrierAmount * 100}% of Max HP";
	public readonly double BarrierAmount = barrier;
	
	public override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		var barrier = (int)Math.Ceiling(caster.Hp * BarrierAmount);
		caster.GrantBarrier(barrier, out int barrierBefore);

		turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(caster.User.GlobalName, "", caster.User.AvatarUrl)
			.WithDescription($"{DiscordEmoji.FromName(target.Client, ":shield:")} **{caster.Stand!.CoolName} creates a barrier for `{barrier}` HP**")
			.WithColor(DiscordColor.CornflowerBlue)
			.WithFooter($"{DiscordEmoji.FromName(caster.Client, ":blue_heart:")} {barrierBefore} {DiscordEmoji.FromName(caster.Client, ":arrow_right:")} {DiscordEmoji.FromName(caster.Client, ":blue_heart:")} {caster.Barrier}", caster.User.AvatarUrl)));
	}
}

public class Strength(double increase) : StatChange 
{
	public override string Name => "💪 Strength";
	public override string ShortDescription => base.ShortDescription + $" +{DamageIncrease * 100}% DMG";
	public readonly double DamageIncrease = increase;

	public override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		var minDamageBefore = caster.MinDamage;
		var maxDamageBefore = caster.MaxDamage;
		
		caster.IncreaseAttack((int)Math.Ceiling(caster.MinDamage * DamageIncrease), (int)Math.Ceiling(caster.MaxDamage * DamageIncrease));

		turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(caster.User.GlobalName, "", caster.User.AvatarUrl)
			.WithDescription($"💪 **Strength increased**")
			.WithColor(DiscordColor.Orange)
			.WithFooter($"🗡️ {minDamageBefore}-{maxDamageBefore} ➡️ {caster.MinDamage}-{caster.MaxDamage}", caster.User.AvatarUrl)));
	}
}

public class Regress(double decrease) : StatChange
{
	public override string Name => "⬇️ Regress";
    public override string ShortDescription => base.ShortDescription + $" -{DamageDecrease * 100}% Enemy DMG";
    public readonly double DamageDecrease = decrease;

	public override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		var minDamageBefore = target.MinDamage;
		var maxDamageBefore = target.MaxDamage;

		target.DecreaseAttack((int)Math.Ceiling(target.MinDamage * DamageDecrease), (int)Math.Ceiling(target.MaxDamage * DamageDecrease));

		turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
			.WithDescription($"💪 **Strength decreased**")
			.WithColor(DiscordColor.Red)
			.WithFooter($"🗡️ {minDamageBefore}-{maxDamageBefore} ➡️ {target.MinDamage}-{target.MaxDamage}", target.User.AvatarUrl)));
	}
}