using DSharpPlus;
using DSharpPlus.Entities;
using JoJoData.Controllers;
using JoJoData.Helpers;

namespace JoJoData.Library;

#region Base Attack
public abstract class Attack(double damage)
{
	public virtual string ShortDescription => $"⚔️ {DamageMultiplier}x DMG";
	public double DamageMultiplier { get; private set; } = damage;
	
	public virtual DiscordMessageBuilder Execute(BattlePlayer attacker, BattlePlayer defender) 
	{
		var dmg = CalculateDamage(attacker, defender, out bool crit);
		defender.ReceiveDamage(dmg, out int hpBefore);

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
#endregion

#region Attack Types
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
		int sum = 0;
		int critCount = 0;
		var hits = DiscordController.RNG.Next(MinHits, MaxHits + 1);
		
		for (var i = 1; i <= hits; ++i) 
		{
			sum += CalculateDamage(attacker, defender, out bool crit);
			if (crit) critCount++;
		}
		defender.ReceiveDamage(sum, out int hpBefore);

		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(attacker.User.GlobalName, "", attacker.User.AvatarUrl)
			.WithDescription($"{DiscordEmoji.FromName(attacker.Client, ":crossed_swords:")} **{attacker.Stand!.CoolName} attacks {hits} times {(critCount > 0 ? $"with {DiscordEmoji.FromName(attacker.Client, ":sparkles:")} CRIT (x{critCount})" : "")} for `{sum}` damage**")
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

public class HPLeechAttack(double damage) : Attack(damage)
{
	public override string ShortDescription => base.ShortDescription + $" Steal ❤️ HP";

	public override DiscordMessageBuilder Execute(BattlePlayer attacker, BattlePlayer defender)
	{	
		return base.Execute(attacker, defender).AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(defender.User.GlobalName, "", defender.User.AvatarUrl)
			.WithDescription($"🔮 **{attacker.Stand!.CoolName} steals `{defender.DamageReceived}` HP from {defender.Stand!.CoolName}**")
			.WithColor(DiscordColor.Magenta));
	}

	protected override int CalculateDamage(BattlePlayer attacker, BattlePlayer defender, out bool crit)
	{
		var dmg = base.CalculateDamage(attacker, defender, out crit);
		attacker.Heal(dmg, out _);
		
		return dmg;
	}
}

public class MPStealAttack(double damage, int mpStealAmount, double hpLossPercent) : Attack(damage)
{
	public override string ShortDescription => base.ShortDescription + $" -❤️ {HpLossAmount * 100}% Max HP, Steal 💎 {MpStealAmount} MP";
	public readonly int MpStealAmount = mpStealAmount;
	public readonly double HpLossAmount = hpLossPercent;

	public override DiscordMessageBuilder Execute(BattlePlayer attacker, BattlePlayer defender)
	{
		var mpSteal = MpStealAmount;
		if (defender.Mp < MpStealAmount) 
		{
			mpSteal = defender.Mp;
		}
		
		attacker.GrantMP(mpSteal, out int mpBefore);
		defender.UseMP(mpSteal, out _);
		attacker.ReceiveDamage((int)(HpLossAmount * attacker.MaxHp), out int hpBefore);
		
		return base.Execute(attacker, defender).AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(defender.User.GlobalName, "", defender.User.AvatarUrl)
			.WithDescription($"🔮 **{attacker.Stand!.CoolName} steals `{MpStealAmount}` MP from {defender.Stand!.CoolName}**")
			.WithFooter($"💎 {mpBefore} ➡️ 💎 {attacker.Mp}, ❤️ {hpBefore} ➡️ ❤️ {attacker.Hp}", attacker.User.AvatarUrl)
			.WithColor(DiscordColor.Purple));
	}
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
#endregion

#region Special Attacks
public class ErasureAttack(double damage, double critDmgIncrease) : CritDamageIncreaseAttack(damage, critDmgIncrease) 
{
	public override DiscordMessageBuilder Execute(BattlePlayer attacker, BattlePlayer defender)
	{
		return base.Execute(attacker, defender).AddEmbed(new DiscordEmbedBuilder()
			.WithImageUrl("https://c.tenor.com/4XTIvoIXUngAAAAC/tenor.gif"));
	}
}

public class DetonateAttack(double damage) : BypassProtectAttack(damage) 
{
	public override DiscordMessageBuilder Execute(BattlePlayer attacker, BattlePlayer defender)
	{
		return base.Execute(attacker, defender);
	}
	
	protected override int CalculateDamage(BattlePlayer attacker, BattlePlayer defender, out bool crit)
	{
		// Only perform attack when enemy is charged
		if (defender.Status is Charged) 
		{
			return base.CalculateDamage(attacker, defender, out crit);
		}
		else 
		{
			crit = false;
			return 0;
		}
		
	}
}
#endregion