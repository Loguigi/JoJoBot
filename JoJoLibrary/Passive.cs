using System.Text;

namespace JoJoLibrary;

public abstract class Passive() : BattleEffect(-1, 1)
{
	public abstract override string Name { get; }
	public abstract override string ShortDescription { get; }
	public string Description { get; protected set; } = string.Empty;
	public abstract int MaxTriggers { get; }

	public sealed override void Apply(Turn turn, BattlePlayer caster, BattlePlayer target) { }

	public override StringBuilder GetLongDescription(Stand stand, BattlePlayer? player = null)
	{
		StringBuilder builder = new();
		if (MaxTriggers != -1)
		{
			builder.Append($"* Maximum Uses: `{MaxTriggers}`");
			if (player != null) builder.AppendLine($"* Remaining Uses: `{MaxTriggers - player.PassiveTriggers}");
		}
		else
		{
			builder.Append("* Maximum Uses: `Infinite`");
		}
		
		return builder;
	}

	protected override void ReduceDuration(Turn turn, BattlePlayer player, bool remove = false)
	{
		// Since triggers don't have a duration, this call will increase the trigger count
		if (MaxTriggers == -1) return;
		player.PassiveTriggers++;
	}

	protected override bool CheckEffectOwner(BattlePlayer player) => player.Stand!.Passive == this;

	protected bool CanTriggerPassive(BattlePlayer caster) => caster.PassiveTriggers == MaxTriggers || MaxTriggers == -1;
}

#region On Death Passives
public abstract class OnDeathPassive : Passive
{
	protected override void DeathFlag(object? s, DeathFlagEventArgs e) => ReduceDuration(e.Turn, e.Player);
}

public class GamblersDream : OnDeathPassive 
{
	public override string Name { get; }
	public override string ShortDescription { get; }
	public override int MaxTriggers => -1;
	
	public override StringBuilder GetLongDescription(Stand stand, BattlePlayer? player = null) => base.GetLongDescription(stand, player).AppendLine($"* `2%` chance on death to revive with full HP");

	protected override void DeathFlag(object? s, DeathFlagEventArgs e)
	{
		if (!CheckEffectOwner(e.Player) || !CanTriggerPassive(e.Player)) return;
		if (JoJo.RNG.NextDouble() < 0.02)
		{
			Heal heal = new(1);
			heal.Execute(e.Turn, e.Player, e.Player);
		}
		else
		{
			e.IsDead = true;
		}
	}
}

public class StrayCatReincarnation : OnDeathPassive 
{
	public override string Name { get; }
	public override string ShortDescription { get; }
	public override int MaxTriggers => 1;
	
	public override StringBuilder GetLongDescription(Stand stand, BattlePlayer? player = null) => base.GetLongDescription(stand, player).AppendLine($"* Revive with `40%` HP");

	protected override void DeathFlag(object? s, DeathFlagEventArgs e)
	{
		if (!CheckEffectOwner(e.Player) || !CanTriggerPassive(e.Player)) return;
		Heal heal = new(0.4);
		heal.Execute(e.Turn, e.Player, e.Player);
		e.IsDead = false;
		base.DeathFlag(s, e);
	}
}
#endregion
