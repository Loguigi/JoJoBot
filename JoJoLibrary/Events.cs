using DSharpPlus.Entities;

namespace JoJoLibrary;

#region Normal Battle Events
public abstract class BattleEventArgs(BattlePlayer player, Turn turn) : EventArgs
{
	public readonly BattlePlayer Player = player;
	public readonly Turn Turn = turn;
}

public class AbilityCastEventArgs(BattlePlayer player, Turn turn, Ability ability) : BattleEventArgs(player, turn)
{
	public readonly Ability Ability = ability;
	public bool IsValidCast { get; set; } = true;
	public DiscordMessageBuilder? OutputMessage { get; set; }
}

public class PreCurrentTurnEventArgs(BattlePlayer player, Turn turn) : BattleEventArgs(player, turn) { }

public class PreEnemyTurnEventArgs(BattlePlayer player, Turn turn) : BattleEventArgs(player, turn) { }

public class PostCurrentTurnEventArgs(BattlePlayer player, Turn turn) : BattleEventArgs(player, turn) { }
#endregion

#region Attack Events

public abstract class AttackEventArgs(BattlePlayer player, Turn turn, Ability ability, BattlePlayer attacker) : BattleEventArgs(player, turn)
{
	public readonly Ability Ability = ability;
	public readonly BattlePlayer Attacker = attacker;
}

public class BeforeAttackedEventArgs(BattlePlayer player, Turn turn, Ability ability, BattlePlayer attacker) : AttackEventArgs(player, turn, ability, attacker)
{
	public bool EvadeAttack { get; set; }
}

public class AfterAttackedEventArgs(BattlePlayer player, Turn turn, Ability ability, BattlePlayer attacker, int damage) : AttackEventArgs(player, turn, ability, attacker)
{
	public readonly int Damage = damage;
}
#endregion

public class DeathFlagEventArgs(BattlePlayer player, Turn turn) : BattleEventArgs(player, turn)
{
	public bool IsDead { get; set; } = true;
}