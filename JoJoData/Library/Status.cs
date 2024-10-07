using System.Formats.Asn1;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
using JoJoData.Controllers;
using JoJoData.Helpers;

namespace JoJoData.Library;

#region Abstract Status
public abstract class Status(int duration, double applyChance) : BattleEffect(duration, applyChance)
{
	public abstract override string Name { get; }
	public override string ShortDescription => $"{Name} {ApplyChance * 100}% chance {Duration} turns";

	public override void Apply(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		if (!RollStatus()) 
		{
			return;
		}

		OnApplied(turn, caster, target);
		turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
			.WithDescription($"⬇️ **{Name} for `{target.StatusDuration}` turns**")
			.WithColor(DiscordColor.Purple)));
	}

	protected virtual void OnApplied(Turn turn, BattlePlayer caster, BattlePlayer target) 
	{
		target.AddStatus(this);
		target.StatusDuration = Duration;
	}
	
	protected override bool CheckEffectOwner(BattlePlayer player) => player.Status == this;

	protected override void PreCurrentTurn(object? s, PreCurrentTurnEventArgs e) => ReduceDuration(e.Turn, e.Player);

	protected override void ReduceDuration(Turn turn, BattlePlayer player, bool remove = false) 
	{
		if (player.ReduceStatusDuration(remove))
		{
			return;
		}

		turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(player.User.GlobalName, "", player.User.AvatarUrl)
			.WithDescription($"**{Name} has worn off**")
			.WithColor(DiscordColor.Green)));
	}
	
	private bool RollStatus() => JoJo.RNG.NextDouble() < ApplyChance;
}
#endregion

#region Damage Statuses
public abstract class DamageStatus(int duration, double applyChance) : Status(duration, applyChance) 
{
	protected override void PreCurrentTurn(object? s, PreCurrentTurnEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		e.Player.ReceiveDamage(e.Turn, e.Player.DamageOverTime, out _);
		base.PreCurrentTurn(s, e);
	}
	
	protected abstract int SetDamageOverTime(BattlePlayer caster, BattlePlayer target);

	protected override void OnApplied(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		target.AddDamageOverTime(dot: SetDamageOverTime(caster, target), status: this);
		base.OnApplied(turn, caster, target);
	}
}

public class Burn(int duration, double applyChance = 1) : DamageStatus(duration, applyChance) 
{
	public override string Name => $"🔥 Burn";
	
	protected override int SetDamageOverTime(BattlePlayer caster, BattlePlayer target) => caster.MinDamage * 2;

	protected override void PreCurrentTurn(object? s, PreCurrentTurnEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		e.Turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(e.Player.User.GlobalName, "", e.Player.User.AvatarUrl)
			.WithDescription($"🔥 **{e.Player.Stand!.CoolName} burns for `{e.Player.DamageOverTime}` damage**")
			.WithColor(DiscordColor.Orange)));
		base.PreCurrentTurn(s, e);
	}
}

public class Bleed(int duration, double applyChance = 1) : DamageStatus(duration, applyChance) 
{
	public override string Name => $"🩸 Bleed";

	protected override int SetDamageOverTime(BattlePlayer caster, BattlePlayer target) => (int)Math.Ceiling(target.Hp * 0.2);

	protected override void PreCurrentTurn(object? s, PreCurrentTurnEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		e.Turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(e.Player.User.GlobalName, "", e.Player.User.AvatarUrl)
			.WithDescription($"🩸 **{e.Player.Stand!.CoolName} bleeds for `{e.Player.DamageOverTime}` damage**")
			.WithColor(DiscordColor.DarkRed)));
		base.PreCurrentTurn(s, e);
	}
}

public class Poison(int duration, double applyChance = 1) : DamageStatus(duration, applyChance) 
{
	public override string Name => $"🐍 Poison";

	protected override int SetDamageOverTime(BattlePlayer caster, BattlePlayer target) => (int)Math.Ceiling(target.MaxHp * 0.2);

	protected override void PreCurrentTurn(object? s, PreCurrentTurnEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		e.Turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(e.Player.User.GlobalName, "", e.Player.User.AvatarUrl)
			.WithDescription($"💀 **{e.Player.Stand!.CoolName} suffers poison for `{e.Player.DamageOverTime}` damage**")
			.WithColor(DiscordColor.Purple)));
		e.Player.DamageOverTime -= (int)Math.Ceiling(e.Player.MaxHp * 0.05);
		base.PreCurrentTurn(s, e);
	}
}

public class Doom(int duration, double applyChance = 1) : DamageStatus(duration, applyChance) 
{
	public override string Name => $"💀 Doom";

	protected override int SetDamageOverTime(BattlePlayer caster, BattlePlayer target) => 0;

	protected override void PreCurrentTurn(object? s, PreCurrentTurnEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		base.PreCurrentTurn(s, e);
		if (e.Player.StatusDuration == 0)
		{
			BypassProtectAttack insta = new(9999);
			insta.Execute(e.Turn,e.Turn.Opponent, e.Player);
		}
		else
		{
			e.Turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(e.Player.User.GlobalName, "", e.Player.User.AvatarUrl)
				.WithDescription("Death approaches...")));
		}
	}
}

public class Drown(int duration, double applyChance = 1) : DamageStatus(duration, applyChance) 
{
	public override string Name => "🌊 Drown";

	protected override int SetDamageOverTime(BattlePlayer caster, BattlePlayer target) => (int)Math.Ceiling(target.MaxHp * 0.1);

	protected override void PreCurrentTurn(object? s, PreCurrentTurnEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		e.Turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(e.Player.User.GlobalName, "", e.Player.User.AvatarUrl)
			.WithDescription($"🌊 **{e.Player.Stand!.CoolName} drowns `{e.Player.DamageOverTime}` damage**")
			.WithColor(DiscordColor.DarkBlue)));
		base.PreCurrentTurn(s, e);
	}
}
#endregion

#region Passive Statuses
public abstract class PassiveStatus(int duration, double applyChance) : Status(duration, applyChance) { }

public class Silence(int duration, double applyChance = 1) : PassiveStatus(duration, applyChance) 
{
	public override string Name => $"🔇 Silence";

	protected override void AbilityCast(object? s, AbilityCastEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		if (e.Ability.MpCost == 0)
		{
			e.IsValidCast = true;
			return;
		}
		
		e.OutputMessage = new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(e.Player.User.GlobalName, "", e.Player.User.AvatarUrl)
			.WithDescription($"🔇 Silenced! Cannot use MP abilities for {e.Player.StatusDuration} turns!")
			.WithColor(DiscordColor.Black));

		e.IsValidCast = false;
		base.AbilityCast(s, e);
	}
}

public class Confusion(int duration, double applyChance = 1) : PassiveStatus(duration, applyChance) 
{
	public override string Name => "❓ Confusion";

	protected override void BeforeAttacked(object? s, BeforeAttackedEventArgs e)
	{
		if (!CheckEffectOwner(e.Attacker)) return;
		var confused = JoJo.RNG.NextDouble() < 0.5;
		
		if (confused)
		{
			e.Turn.Caster = e.Player;
			e.Turn.Target = e.Attacker;
		}
		
		base.BeforeAttacked(s, e);
	}
}

public class Douse(int duration, double applyChance = 1) : PassiveStatus(duration, applyChance) 
{
	public override string Name => "🛢️ Douse";

	public void Ignite(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		var ignite = new Burn(duration: 4, applyChance: 1);
		ignite.Apply(turn, caster, target);
	}
}

public class Frail(int duration, double applyChance = 1) : PassiveStatus(duration, applyChance)
{
	public override string Name => "☔️ Frail";
	// TODO add -dr to short description
	public readonly double DrReduction = 0.25;

	protected override void PreEnemyTurn(object? s, PreEnemyTurnEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		Weaken(e.Player);
		base.PreEnemyTurn(s, e);
	}
	
	protected override void PreCurrentTurn(object? s, PreCurrentTurnEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		Weaken(e.Player);
		base.PreCurrentTurn(s, e);
	}

	private void Weaken(BattlePlayer target) => target.DamageResistance -= DrReduction;
}

public class Shock(int duration, double applyChance = 1) : PassiveStatus(duration, applyChance) 
{
	public override string Name => "⚡️ Shock";

	protected override void AfterAttacked(object? s, AfterAttackedEventArgs e)
	{
		int shockDamage = (int)(e.Damage * 0.5);
		int hpBefore = 0;
		
		try
		{
			if (!CheckEffectOwner(e.Attacker)) return;
			e.Attacker.ReceiveDamage(e.Turn, shockDamage, out hpBefore);
			
			e.Turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(e.Attacker.User.GlobalName, "", e.Attacker.User.AvatarUrl)
				.WithDescription($"⛈️ **{e.Attacker.Stand!.CoolName} shocks themself for `{shockDamage}` damage!**")
				.WithColor(DiscordColor.Yellow)
				.WithFooter($"❤️ {hpBefore} ➡️ ❤️ {e.Attacker.Hp}")));
			base.AfterAttacked(s, e);
		}
		catch (OnDeathException)
		{
			e.Turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(e.Attacker.User.GlobalName, "", e.Attacker.User.AvatarUrl)
				.WithDescription($"⛈️ **{e.Attacker.Stand!.CoolName} shocks themself for `{shockDamage}` damage!**")
				.WithColor(DiscordColor.Yellow)
				.WithFooter($"❤️ {hpBefore} ➡️ ❤️ {e.Attacker.Hp}")));
			throw;
		}
	}
}

public class Charged(int duration, double applyChance = 1) : PassiveStatus(duration, applyChance) 
{
	public override string Name => "💣 Charged";
}

public class Capture(int duration, double applyChance = 1) : PassiveStatus(duration, applyChance)
{
	public override string Name => "📸 Capture";

	protected override void AfterAttacked(object? s, AfterAttackedEventArgs e)
	{
		if (!CheckEffectOwner(e.Player) || !CheckCaptureRequirement(e.Ability)) return;
		ReduceDuration(e.Turn, e.Player, true);
	}

	private bool CheckCaptureRequirement(Ability ability)
	{
		var requirement = ability.Requirement as StatusRequirement;
		return requirement?.StatusType == typeof(Capture);
	}
}

public class Blind(int duration, double applyChance = 1) : PassiveStatus(duration, applyChance)
{
	public override string Name => "🕶️ Blind";

	protected override void BeforeAttacked(object? s, BeforeAttackedEventArgs e)
	{
		if (!CheckEffectOwner(e.Attacker)) return;
		if (!RollBlind()) return;

		e.EvadeAttack = true;
		e.Turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(e.Attacker.User.GlobalName, "", e.Attacker.User.AvatarUrl)
			.WithDescription($"🍃 **{e.Attacker.Stand!.CoolName} misses!**")
			.WithColor(DiscordColor.DarkGray)));
	}
	
	private bool RollBlind() => JoJo.RNG.NextDouble() < 0.5;
}
#endregion

#region Turn Skip Statuses
public abstract class TurnSkipStatus(int duration, double applyChance) : Status(duration, applyChance) 
{

}

public class TimeStop(int duration, double applyChance = 1) : TurnSkipStatus(duration, applyChance) 
{
	public override string Name => $"🕚 Time Stop";

	protected override void PreCurrentTurn(object? s, PreCurrentTurnEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		base.PreCurrentTurn(s, e);
		if (e.Player.StatusDuration == 0) return;
		
		e.Turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(e.Player.User.GlobalName, "", e.Player.User.AvatarUrl)
			.WithDescription("🕐 **Time is frozen...** 🕥")
			.WithColor(DiscordColor.DarkBlue)));
		throw new TurnSkipException(e.Turn, e.Player);
	}

	protected override void OnApplied(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		turn.BattleLog.Add(caster.Stand!.Name switch
		{
			"Star Platinum" => new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder().WithImageUrl("https://c.tenor.com/0kMboLznZGwAAAAC/tenor.gif")),
			"The World" => new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder().WithImageUrl("https://c.tenor.com/R_NQbI9vk1UAAAAC/tenor.gif")),
			_ => new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder().WithImageUrl("https://c.tenor.com/qAhqQaBghmkAAAAC/tenor.gif"))
		});
		base.OnApplied(turn, caster, target);
	}
}

public class Sleep(int duration, double applyChance = 1) : TurnSkipStatus(duration, applyChance) 
{
	public override string Name => $"💤 Sleep";

	protected override void PreCurrentTurn(object? s, PreCurrentTurnEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		base.PreCurrentTurn(s, e);
		if (e.Player.StatusDuration == 0) return;
		
		e.Player.GrantMP(10, out var mpBefore);
		e.Player.Heal((int)(e.Player.MaxHp * 0.05), out var hpBefore);
		e.Turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(e.Player.User.GlobalName, "", e.Player.User.AvatarUrl)
			.WithDescription("💤 **Asleep...** 💤")
			.WithColor(DiscordColor.DarkBlue)
			.WithFooter($"❤️ {hpBefore} ➡️ ❤️ {e.Player.Hp}, 💎 {mpBefore} ➡️ 💎 {e.Player.Mp}")));
		throw new TurnSkipException(e.Turn, e.Player);
	}

	protected override void AfterAttacked(object? s, AfterAttackedEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		if (!RollForWakeUp()) return;

		e.Player.ReduceStatusDuration(true);
		e.Turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithDescription($"⏰ **{e.Player.Stand!.CoolName} ({e.Player.User.Mention}) has woken up!**")
			.WithColor(DiscordColor.Blurple)));
		base.AfterAttacked(s, e);
	}
	
	private bool RollForWakeUp() => JoJo.RNG.NextDouble() < 0.5;
}
#endregion

public class Random(int duration, double applyChance = 1) : Status(duration, applyChance)
{
	public override string Name => $"🎲 Random";

	public override void Apply(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		List<Type> statuses = Assembly.GetExecutingAssembly().GetTypes().Where(x =>
			x is { IsClass: true, Namespace: not null } &&
			x.Namespace.Contains("JoJoData.Library")).ToList().Where(y =>
			!y.IsAbstract && (y.BaseType == typeof(DamageStatus) || y.BaseType == typeof(PassiveStatus) || y.BaseType == typeof(TurnSkipStatus))).ToList();

		var selection = JoJo.RNG.Next(0, statuses.Count);
		if (Activator.CreateInstance(statuses[selection], Duration, ApplyChance) is Status status)
		{
			status.Apply(turn, caster, target);
		}
	}
}