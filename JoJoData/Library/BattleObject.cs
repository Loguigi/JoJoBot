using System.Text;
using DSharpPlus.Entities;

namespace JoJoData.Library;

public abstract class BattleObject
{
	public abstract string Name { get; }
	public abstract string ShortDescription { get; }
	public abstract StringBuilder GetLongDescription(Stand stand, BattlePlayer? player = null);
}

public abstract class BattleEffect(int duration, double applyChance) : BattleObject
{
	protected int Duration { get; } = duration;
	protected double ApplyChance { get; } = applyChance;

	public void RegisterEvents(Turn turn)
	{
		turn.AbilityCast += AbilityCast;
		turn.PreCurrentTurn += PreCurrentTurn;
		turn.PreEnemyTurn += PreEnemyTurn;
		turn.PostCurrentTurn += PostCurrentTurn;
		turn.BeforeAttacked += BeforeAttacked;
		turn.AfterAttacked += AfterAttacked;
		turn.DeathFlag += DeathFlag;
	}
	
	public abstract void Apply(Turn turn, BattlePlayer caster, BattlePlayer target);
	protected abstract void ReduceDuration(Turn turn, BattlePlayer player, bool remove = false);
	protected abstract bool CheckEffectOwner(BattlePlayer player);
	
	protected virtual void AbilityCast(object? s, AbilityCastEventArgs e) { }
	protected virtual void PreCurrentTurn(object? s, PreCurrentTurnEventArgs e) { }
	protected virtual void PreEnemyTurn(object? s, PreEnemyTurnEventArgs e) { }
	protected virtual void PostCurrentTurn(object? s, PostCurrentTurnEventArgs e) { }
	protected virtual void BeforeAttacked(object? s, BeforeAttackedEventArgs e) { }
	protected virtual void AfterAttacked(object? s, AfterAttackedEventArgs e) { }
	protected virtual void DeathFlag(object? s, DeathFlagEventArgs e) { }
}

public abstract class BattleAction : BattleObject
{
	public abstract void Execute(Turn turn, BattlePlayer caster, BattlePlayer target);
}