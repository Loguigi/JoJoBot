using DSharpPlus;
using DSharpPlus.Entities;
using JoJoData.Controllers;
using JoJoData.Helpers;

namespace JoJoData.Library;

public abstract class Attack(double damage)
{
	public virtual string ShortDescription => $"⚔️ {DamageMultiplier}x DMG";
	public double DamageMultiplier { get; private set; } = damage;
	
	public virtual DiscordMessageBuilder Execute(BattlePlayer attacker, BattlePlayer defender) 
	{
		var hpBefore = defender.Hp;
		var dmg = CalculateDamage(attacker, defender, out bool crit);
		defender.ReceiveDamage(dmg);

		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(attacker.User.GlobalName, "", attacker.User.AvatarUrl)
			.WithDescription($"{DiscordEmoji.FromName(attacker.Client, ":crossed_swords:")} **{attacker.Stand!.CoolName} attacks {(crit ? $"with a {DiscordEmoji.FromName(attacker.Client, ":sparkles:")} CRIT" : "")} for `{dmg}` damage**")
			.WithFooter($"{DiscordEmoji.FromName(attacker.Client, ":heart:")} {hpBefore} {DiscordEmoji.FromName(attacker.Client, ":arrow_right:")} {DiscordEmoji.FromName(attacker.Client, ":heart:")} {defender.Hp}", defender.User.AvatarUrl)
			.WithColor(DiscordColor.Red));
	}

	protected virtual int CalculateDamage(BattlePlayer attacker, BattlePlayer defender, out bool crit)
	{
		return (int)Math.Ceiling(
			RollDamage(attacker.MinDamage, attacker.MaxDamage) *
			DamageMultiplier *
			RollCrit(attacker.CritChance, attacker.CritDamageMultiplier, out crit) *
			(1 - defender.DamageResistance));
	}

	protected static int RollDamage(int min, int max) => DiscordController.RNG.Next(min, max + 1);

	protected static double RollCrit(double critChance, double critDamage, out bool crit)
	{
		var critDmg = DiscordController.RNG.NextDouble() < critChance ? critDamage : 1;
		crit = critDmg > 1;
		return critDmg;
	}
}

public class BasicAttack(double damage) : Attack(damage) { }

public class BypassProtectAttack(double damage) : Attack(damage)
{
	public override string ShortDescription => base.ShortDescription + " Pierce Protect";
	
	protected override int CalculateDamage(BattlePlayer attacker, BattlePlayer defender, out bool crit)
	{
		return (int)Math.Ceiling(
			RollDamage(attacker.MinDamage, attacker.MaxDamage) *
			DamageMultiplier *
			RollCrit(attacker.CritChance, attacker.CritDamageMultiplier, out crit) *
			// if target has negative DR, damage bonus is still applied
			(defender.DamageResistance < 0 ? 1 - defender.DamageResistance : 1));
	}
}

public class MultiHitAttack(double damage, int minHits, int maxHits) : Attack(damage)
{
	public override string ShortDescription => base.ShortDescription + $" {MinHits}-{MaxHits} hits";
	public readonly int MinHits = minHits;
	public readonly int MaxHits = maxHits;
	
	public override DiscordMessageBuilder Execute(BattlePlayer attacker, BattlePlayer defender)
	{
		var hpBefore = defender.Hp;
		int sum = 0;
		int critCount = 0;
		var hits = DiscordController.RNG.Next(MinHits, MaxHits);
		
		for (var i = 1; i <= hits; ++i) 
		{
			sum += CalculateDamage(attacker, defender, out bool crit);
			if (crit) critCount++;
		}
		defender.ReceiveDamage(sum);

		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithDescription($"{DiscordEmoji.FromName(attacker.Client, ":crossed_swords:")} **{attacker.Stand!.CoolName} ({attacker.User.Mention}) attacks {hits} times {(critCount > 0 ? $"with {DiscordEmoji.FromName(attacker.Client, ":sparkles:")} CRIT(x{critCount})" : "")} for `{sum}` damage**")
			.WithFooter($"{DiscordEmoji.FromName(attacker.Client, ":heart:")} {hpBefore} {DiscordEmoji.FromName(attacker.Client, ":arrow_right:")} {DiscordEmoji.FromName(attacker.Client, ":heart:")} {defender.Hp}", defender.User.AvatarUrl)
			.WithColor(DiscordColor.Red));
	}
}

public class CritChanceIncreaseAttack(double damage, double increase) : Attack(damage)
{
	public override string ShortDescription => base.ShortDescription + $" +{CritChanceIncrease * 100}% Crit Chance";
	public readonly double CritChanceIncrease = increase;
	
	protected override int CalculateDamage(BattlePlayer attacker, BattlePlayer defender, out bool crit)
	{
		return (int)Math.Ceiling(
			RollDamage(attacker.MinDamage, attacker.MaxDamage) *
			DamageMultiplier *
			RollCrit(CritChanceIncrease == 0 ? 0 : attacker.CritChance + CritChanceIncrease, attacker.CritDamageMultiplier, out crit) *
			(1 - defender.DamageResistance));
	}
}

public class CritDamageIncreaseAttack(double damage, double increase) : Attack(damage)
{
	public override string ShortDescription => base.ShortDescription + $" +{CritDamageIncrease * 100}% Crit DMG";
	public readonly double CritDamageIncrease = increase;
	
	protected override int CalculateDamage(BattlePlayer attacker, BattlePlayer defender, out bool crit)
	{
		return (int)Math.Ceiling(
			RollDamage(attacker.MinDamage, attacker.MaxDamage) *
			DamageMultiplier *
			RollCrit(attacker.CritChance, attacker.CritDamageMultiplier + CritDamageIncrease, out crit) *
			(1 - defender.DamageResistance));
	}
}

public class WeaknessAttack(double damage, double increase, Type status) : Attack(damage)
{
	public override string ShortDescription => base.ShortDescription + $" x{WeaknessDamageIncrease} against {StatusType.Name}";
	public readonly double WeaknessDamageIncrease = increase;
	public readonly Type StatusType = status;

	protected override int CalculateDamage(BattlePlayer attacker, BattlePlayer defender, out bool crit)
	{
		if (defender.Status is not null && defender.Status.GetType() == StatusType)
		{
			return (int)Math.Ceiling(base.CalculateDamage(attacker, defender, out crit) * WeaknessDamageIncrease);
		}
		else
		{
			return base.CalculateDamage(attacker, defender, out crit);
		}
	}
}

public class HPLeechAttack(double damage, double hpStealPercent) : Attack(damage)
{
	public double HPStealPercent { get; private set; } = hpStealPercent;

	public override DiscordMessageBuilder Execute(BattlePlayer attacker, BattlePlayer defender)
	{
		
		return base.Execute(attacker, defender);
	}

	// TODO
	// public override int PerformAttack(BattlePlayer attacker, BattlePlayer defender)
	// {
	// 	var leech = (int)(defender.MaxHp * HPStealPercent);
	// 	defender.ReceiveDamage(leech);
	// 	attacker.Heal(leech);
	// 	return base.PerformAttack(attacker, defender);
	// }
}

public class MPStealAttack(double damage, int mpStealAmount, double hpLossPercent) : Attack(damage)
{
	public readonly int MpStealAmount = mpStealAmount;
	public readonly double HpLossAmount = hpLossPercent;

	// TODO
	// public override int PerformAttack(BattlePlayer attacker, BattlePlayer defender)
	// {
	// 	attacker.GrantMP(MpStealAmount);
	// 	defender.UseMP(MpStealAmount);
	// 	attacker.ReceiveDamage((int)Math.Ceiling(attacker.MaxHp * HpLossAmount));
	// 	return base.PerformAttack(attacker, defender);
	// }
}

public class TakeoverAttack(double damage) : Attack(damage)
{
	public override string ShortDescription => base.ShortDescription + " Uses Enemy DMG";
	
	protected override int CalculateDamage(BattlePlayer attacker, BattlePlayer defender, out bool crit)
	{
		return base.CalculateDamage(defender, attacker, out crit);
	}
}

public class IgniteAttack(double damage) : Attack(damage) 
{
    public override string ShortDescription => base.ShortDescription + " 100% Burn when Doused";
    public override DiscordMessageBuilder Execute(BattlePlayer attacker, BattlePlayer defender)
	{
		if (defender.Status is Douse douse) 
		{
			return base.Execute(attacker, defender).AddEmbed(douse.Ignite(attacker, defender).Embeds[0]);
		}
		else 
		{
			return base.Execute(attacker, defender);
		}
	}
}