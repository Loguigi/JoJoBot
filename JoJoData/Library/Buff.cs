using DSharpPlus;
using DSharpPlus.Entities;

namespace JoJoData.Library;

public abstract class Buff(int duration) : BattleEffect(duration, applyChance: 1)
{
	public abstract override string Name { get; }
	public override string ShortDescription => $"{Name} {Duration} turns";

	public override void Apply(Turn turn, BattlePlayer caster, BattlePlayer target) 
	{
		target.AddBuff(this);
		target.BuffDuration = Duration;

		turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
			.WithDescription($"⬆️ **{Name} for `{Duration}` turns**")
			.WithColor(DiscordColor.SpringGreen)));
	}

	protected override void PostCurrentTurn(object? s, PostCurrentTurnEventArgs e) => ReduceDuration(e.Turn, e.Player);

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
	public readonly double DamageResistance = dr;

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
		target.DamageResistance += DamageResistance;
	}
}

public class Haste(int duration) : Buff(duration) 
{
	public override string Name => $"💨 Haste";

	protected override void PreCurrentTurn(object? s, PreCurrentTurnEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		e.Turn.RoundRepeat = true;
	}
}

public class Await(int duration) : Buff(duration)
{
	public override string Name => $"🗡️ Await";

	protected override void BeforeAttacked(object? s, BeforeAttackedEventArgs e)
	{
		if (!CheckEffectOwner(e.Player)) return;
		
		e.EvadeAttack = true;
		BasicAttack counter = new(2);
		e.Turn.BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(e.Player.User.GlobalName, "", e.Player.User.AvatarUrl)
			.WithDescription($"🍃 **{e.Player.Stand!.CoolName} evades the attack!**")
			.WithColor(DiscordColor.White)));
		counter.Execute(e.Turn, e.Player, e.Attacker);
		e.Player.ReduceStatusDuration(true);
	} 
}

public class Charge(int duration) : Buff(duration) 
{
	public override string Name => "💪 Charge";

	protected override void PreCurrentTurn(object? s, PreCurrentTurnEventArgs e)
	{
		base.PreCurrentTurn(s, e);
		e.Turn.CurrentPlayer.MinDamage *= 2;
		e.Turn.CurrentPlayer.MaxDamage *= 2;
	}
}

public class Thorns(int duration) : Buff(duration) 
{
	public override string Name => "🌵 Thorns";
	
}