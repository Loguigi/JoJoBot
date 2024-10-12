using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using JoJoData.Controllers;

namespace JoJoData.Library;

public abstract class StatChange  : BattleAction
{
	public abstract override string Name { get; }
	public override string ShortDescription => Name;

	public override StringBuilder GetLongDescription(Stand stand, BattlePlayer? player = null) => new(ShortDescription);
	
	public abstract override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target);
}

public class Heal(double healPercent) : StatChange 
{
	public override string Name => "💖 Heal";
	public override string ShortDescription => base.ShortDescription + $" {JoJo.ConvertToPercent(healPercent)}% of Max HP";

	public override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		var heal = (int)Math.Ceiling(caster.MaxHp * healPercent);
		caster.Heal(heal, out var hpBefore);

		turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(caster.User.GlobalName, "", caster.User.AvatarUrl)
			.WithDescription($"{DiscordEmoji.FromName(caster.Client, ":sparkling_heart:")} **{caster.Stand!.CoolName} heals for `{heal}` HP**")
			.WithColor(DiscordColor.Rose)
			.WithFooter($"{DiscordEmoji.FromName(caster.Client, ":heart:")} {hpBefore} {DiscordEmoji.FromName(caster.Client, ":arrow_right:")} {DiscordEmoji.FromName(caster.Client, ":heart:")} {caster.Hp}", caster.User.AvatarUrl)));
	}
}

public class Barrier(double barrierAmount) : StatChange 
{
	public override string Name => "💙 Barrier";
	public override string ShortDescription => base.ShortDescription + $" {barrierAmount * 100}% of Max HP";

	public override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		var barrier = (int)Math.Ceiling(caster.Hp * barrierAmount);
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
	public override string ShortDescription => base.ShortDescription + $" +{increase * 100}% DMG";

	public override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		var minDamageBefore = caster.MinDamage;
		var maxDamageBefore = caster.MaxDamage;
		
		caster.IncreaseAttack(JoJo.Calculate(caster.MinDamage * increase), JoJo.Calculate(caster.MaxDamage * increase));

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
    public override string ShortDescription => base.ShortDescription + $" -{JoJo.ConvertToPercent(DamageDecrease)}% Enemy DMG";
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

public class BitesZaDusto(double healPercent) : Heal(healPercent)
{
	public override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithImageUrl("https://c.tenor.com/oTklJ8haKowAAAAC/tenor.gif")));
		base.Execute(turn, caster, target);
	}
}