using DSharpPlus;
using DSharpPlus.Entities;
using JoJoData.Controllers;
using JoJoData.Helpers;

namespace JoJoData.Library;

#region Base Attack
public abstract class Attack(double damage) : BattleAction
{
	public virtual string ShortDescription => $"⚔️ {DamageMultiplier}x DMG";
	public double DamageMultiplier { get; private set; } = damage;
	
	public override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		int dmg = 0;
		int hpBefore = 0;
		bool crit = false;
		
		try
		{
			turn.Caster = caster;
			turn.Target = target;
			BeforeAttackedEventArgs beforeAttacked = new(target, turn, this, caster);
			turn.OnBeforeAttacked(beforeAttacked);
			if (beforeAttacked.EvadeAttack) return;

			dmg = CalculateDamage(turn.Caster, turn.Target, out crit);
			turn.Target.ReceiveDamage(turn, dmg, out hpBefore);
			turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(turn.Caster.User.GlobalName, "", turn.Caster.User.AvatarUrl)
				.WithDescription($"{DiscordEmoji.FromName(caster.Client, ":crossed_swords:")} **{turn.Caster.Stand!.CoolName} attacks {(crit ? $"with a {DiscordEmoji.FromName(caster.Client, ":sparkles:")} CRIT" : "")} for `{dmg}` damage**")
				.WithFooter($"{DiscordEmoji.FromName(caster.Client, ":heart:")} {hpBefore} {DiscordEmoji.FromName(caster.Client, ":arrow_right:")} {DiscordEmoji.FromName(caster.Client, ":heart:")} {turn.Target.Hp}", turn.Target.User.AvatarUrl)
				.WithColor(DiscordColor.Red)));

			AfterAttackedEventArgs afterAttacked = new(turn.Target, turn, this, turn.Caster, dmg);
			turn.OnAfterAttacked(afterAttacked);
		}
		catch (OnDeathException)
		{
			turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(turn.Caster.User.GlobalName, "", turn.Caster.User.AvatarUrl)
				.WithDescription($"{DiscordEmoji.FromName(caster.Client, ":crossed_swords:")} **{turn.Caster.Stand!.CoolName} attacks {(crit ? $"with a {DiscordEmoji.FromName(caster.Client, ":sparkles:")} CRIT" : "")} for `{dmg}` damage**")
				.WithFooter($"{DiscordEmoji.FromName(caster.Client, ":heart:")} {hpBefore} {DiscordEmoji.FromName(caster.Client, ":arrow_right:")} {DiscordEmoji.FromName(caster.Client, ":heart:")} {turn.Target.Hp}", turn.Target.User.AvatarUrl)
				.WithColor(DiscordColor.Red)));
			throw;
		}
	}

	protected virtual int CalculateDamage(BattlePlayer attacker, BattlePlayer defender, out bool crit)
	{
		return (int)Math.Ceiling(
			RollDamage(attacker.MinDamage, attacker.MaxDamage) *
			DamageMultiplier *
			RollCrit(attacker.CritChance, attacker.CritDamageMultiplier, out crit) *
			(1 - defender.DamageResistance));
	}

	protected static int RollDamage(int min, int max) => JoJo.RNG.Next(min, max + 1);

	protected static double RollCrit(double critChance, double critDamage, out bool crit)
	{
		double critDmg = JoJo.RNG.NextDouble() < critChance ? critDamage : 1;
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
	
	public override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		int dmg = 0;
		int hpBefore = 0;
		int critCount = 0;
		int hits = JoJo.RNG.Next(MinHits, MaxHits + 1);
		
		try
		{
			turn.Caster = caster;
			turn.Target = target;
			BeforeAttackedEventArgs beforeAttacked = new(target, turn, this, caster);
			turn.OnBeforeAttacked(beforeAttacked);
			if (beforeAttacked.EvadeAttack) return;

			for (var i = 1; i <= hits; ++i) 
			{
				dmg += CalculateDamage(turn.Caster, turn.Target, out bool crit);
				if (crit) critCount++;
			}
			
			turn.Target.ReceiveDamage(turn, dmg, out hpBefore);
			turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(turn.Caster.User.GlobalName, "", turn.Caster.User.AvatarUrl)
				.WithDescription($"{DiscordEmoji.FromName(caster.Client, ":crossed_swords:")} **{turn.Caster.Stand!.CoolName} attacks {hits} times {(critCount > 0 ? $"with {DiscordEmoji.FromName(caster.Client, ":sparkles:")} CRIT (x{critCount})" : "")} for `{dmg}` damage**")
				.WithFooter($"{DiscordEmoji.FromName(caster.Client, ":heart:")} {hpBefore} {DiscordEmoji.FromName(caster.Client, ":arrow_right:")} {DiscordEmoji.FromName(caster.Client, ":heart:")} {turn.Target.Hp}", turn.Target.User.AvatarUrl)
				.WithColor(DiscordColor.Red)));

			AfterAttackedEventArgs afterAttacked = new(turn.Target, turn, this, turn.Caster, dmg);
			turn.OnAfterAttacked(afterAttacked);
		}
		catch (OnDeathException)
		{
			turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(turn.Caster.User.GlobalName, "", turn.Caster.User.AvatarUrl)
				.WithDescription($"{DiscordEmoji.FromName(caster.Client, ":crossed_swords:")} **{turn.Caster.Stand!.CoolName} attacks {hits} times {(critCount > 0 ? $"with {DiscordEmoji.FromName(caster.Client, ":sparkles:")} CRIT (x{critCount})" : "")} for `{dmg}` damage**")
				.WithFooter($"{DiscordEmoji.FromName(caster.Client, ":heart:")} {hpBefore} {DiscordEmoji.FromName(caster.Client, ":arrow_right:")} {DiscordEmoji.FromName(caster.Client, ":heart:")} {turn.Target.Hp}", turn.Target.User.AvatarUrl)
				.WithColor(DiscordColor.Red)));
			throw;
		}
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

	public override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		base.Execute(turn, caster, target);
		turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
			.WithDescription($"🔮 **{caster.Stand!.CoolName} steals `{target.DamageReceived}` HP from {target.Stand!.CoolName}**")
			.WithColor(DiscordColor.Magenta)));
	}

	protected override int CalculateDamage(BattlePlayer attacker, BattlePlayer defender, out bool crit)
	{
		int dmg = base.CalculateDamage(attacker, defender, out crit);
		attacker.Heal(dmg, out _);
		
		return dmg;
	}
}

public class MPStealAttack(double damage, int mpStealAmount, double hpLossPercent) : Attack(damage)
{
	public override string ShortDescription => base.ShortDescription + $" -❤️ {HpLossAmount * 100}% Max HP, Steal 💎 {MpStealAmount} MP";
	public readonly int MpStealAmount = mpStealAmount;
	public readonly double HpLossAmount = hpLossPercent;

	public override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		base.Execute(turn, caster, target);
		
		int mpSteal = MpStealAmount;
		if (target.Mp < MpStealAmount) 
		{
			mpSteal = target.Mp;
		}
		
		caster.GrantMP(mpSteal, out int mpBefore);
		target.UseMP(mpSteal, out _);
		caster.ReceiveDamage(turn, (int)(HpLossAmount * caster.MaxHp), out int hpBefore);
		
		turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
			.WithDescription($"🔮 **{caster.Stand!.CoolName} steals `{MpStealAmount}` MP from {target.Stand!.CoolName}**")
			.WithFooter($"💎 {mpBefore} ➡️ 💎 {caster.Mp}, ❤️ {hpBefore} ➡️ ❤️ {caster.Hp}", caster.User.AvatarUrl)
			.WithColor(DiscordColor.Purple)));
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
	public override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		base.Execute(turn, caster, target);
		if (target.Status is Douse douse) 
		{
			douse.Ignite(turn, caster, target);
		}
	}
}
#endregion

#region Special Attacks
public class ErasureAttack(double damage, double critDmgIncrease) : CritDamageIncreaseAttack(damage, critDmgIncrease) 
{
	public override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder().WithImageUrl("https://c.tenor.com/4XTIvoIXUngAAAAC/tenor.gif")));
		base.Execute(turn, caster, target);
	}
}

public class DetonateAttack(double damage) : BypassProtectAttack(damage) 
{
	public override void Execute(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		target.ReduceStatusDuration(remove: true); // remove Charged status after execution
		base.Execute(turn, caster, target);
	}
}

public class RPSAttack(double damage, int mpStealAmount = 30, double hpLossPercent = 0) : MPStealAttack(damage, mpStealAmount, hpLossPercent) 
{
	
}
#endregion