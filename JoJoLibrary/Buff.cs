using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;

namespace JoJoLibrary;

public abstract class Buff(int duration) : BattleEffect(duration, applyChance: 1)
{
	public abstract override string Name { get; }
	public override string ShortDescription => $"{Name} {Duration} turns";
	
	public override StringBuilder GetLongDescription(Stand stand, BattlePlayer? player = null) => new($"* {Name} for `{Duration}` turns: ");

	public override void Apply(Turn turn, BattlePlayer caster, BattlePlayer target) 
	{
		target.AddBuff(this);
		target.BuffDuration = Duration;

		turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
			.WithDescription($"⬆️ **{Name} for `{Duration}` turns**")
			.WithColor(DiscordColor.SpringGreen)));
	}

	protected override void PostCurrentTurn(object? s, PostCurrentTurnEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		ReduceDuration(e.Turn, e.Player);
	}

	protected override bool CheckEffectOwner(BattlePlayer player) => player.Buff == this;

	protected override void ReduceDuration(Turn turn, BattlePlayer player, bool remove = false)
	{
		if (player.ReduceBuffDuration(remove))
		{
			return;
		}

		turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(player.User.GlobalName, "", player.User.AvatarUrl)
			.WithDescription($"**{Name} has worn off**")
			.WithColor(DiscordColor.Green)));
	}
}

public class Protect(int duration, double dr) : Buff(duration) 
{
	public override string Name => "🛡️ Protect";
	
	public override StringBuilder GetLongDescription(Stand stand, BattlePlayer? player = null) => base.GetLongDescription(stand, player).Append($"`+{JoJo.ConvertToPercent(dr)}%` damage resistance\n");

	protected override void PreCurrentTurn(object? s, PreCurrentTurnEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		GrantProtect(e.Player);
	}

	protected override void PreEnemyTurn(object? s, PreEnemyTurnEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		GrantProtect(e.Player);
	}

	private void GrantProtect(BattlePlayer target)
	{
		target.DamageResistance += dr;
	}
}

public class Haste(int duration) : Buff(duration) 
{
	public override string Name => $"💨 Haste";
	
	public override StringBuilder GetLongDescription(Stand stand, BattlePlayer? player = null) => base.GetLongDescription(stand, player).Append($"gain {Duration} extra {(Duration == 1 ? "turn" : "turns")}\n");

	public override void Apply(Turn turn, BattlePlayer caster, BattlePlayer target)
	{
		base.Apply(turn, caster, target);
		turn.RoundRepeat = true;
	}

	protected override void PostCurrentTurn(object? s, PostCurrentTurnEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		base.PostCurrentTurn(s, e);
		if (e.Player.BuffDuration != 0)
		{
			e.Turn.RoundRepeat = true;
		}
	}
}

public class Await(int duration, double damage) : Buff(duration)
{
	public override string Name => "🗡️ Await";
	
	public override StringBuilder GetLongDescription(Stand stand, BattlePlayer? player = null) => base.GetLongDescription(stand, player).Append($"dodge next attack, then counter with a `x{damage}` attack\n");


	protected override void BeforeAttacked(object? s, BeforeAttackedEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		
		e.EvadeAttack = true;
		BasicAttack counter = new(damage);
		e.Turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(e.Player.User.GlobalName, "", e.Player.User.AvatarUrl)
			.WithDescription($"🍃 **{e.Player.Stand!.CoolName} evades the attack!**")
			.WithColor(DiscordColor.White)));
		counter.Execute(e.Turn, e.Player, e.Attacker);
		ReduceDuration(e.Turn, e.Player, true);
	} 
}

public class Charge(int duration) : Buff(duration) 
{
	public override string Name => "💪 Charge";
	
	public override StringBuilder GetLongDescription(Stand stand, BattlePlayer? player = null) => base.GetLongDescription(stand, player).Append("gain `2x` damage\n");

	protected override void PreCurrentTurn(object? s, PreCurrentTurnEventArgs e)
	{
		base.PreCurrentTurn(s, e);
		e.Turn.CurrentPlayer.MinDamage *= 2;
		e.Turn.CurrentPlayer.MaxDamage *= 2;
	}
}

public class Thorns(int duration, double reflectPercent) : Buff(duration) 
{
	public override string Name => "🌵 Thorns";
	
	public override StringBuilder GetLongDescription(Stand stand, BattlePlayer? player = null) => base.GetLongDescription(stand, player).Append($"reflect `{JoJo.ConvertToPercent(reflectPercent)}%` damage to attacker\n");

	protected override void AfterAttacked(object? s, AfterAttackedEventArgs e)
	{
		int thornsDamage = (int)(e.Damage * reflectPercent);
		int hpBefore = 0;
		
		try
		{
			if (!CheckEffectOwner(e.Player)) return;
			e.Attacker.ReceiveDamage(e.Turn, thornsDamage, out hpBefore);
			e.Turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(e.Attacker.User.GlobalName, "", e.Attacker.User.AvatarUrl)
				.WithDescription($"🌵 **{e.Attacker.Stand!.CoolName} takes `{thornsDamage}` damage!**")
				.WithColor(DiscordColor.DarkGreen)
				.WithFooter($"❤️ {hpBefore} ➡️ ❤️ {e.Attacker.Hp}")));
		}
		catch (OnDeathException)
		{
			e.Turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(e.Attacker.User.GlobalName, "", e.Attacker.User.AvatarUrl)
				.WithDescription($"🌵 **{e.Attacker.Stand!.CoolName} takes `{thornsDamage}` damage!**")
				.WithColor(DiscordColor.DarkGreen)
				.WithFooter($"❤️ {hpBefore} ➡️ ❤️ {e.Attacker.Hp}")));
			throw;
		}
	}
}