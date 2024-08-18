using System.ComponentModel.Design;
using DSharpPlus.Entities;
using JoJoData.Controllers;

namespace JoJoData.Library;

public abstract class Passive 
{
	public string Name { get; protected set; } = string.Empty;
	public string Description { get; protected set; } = string.Empty;
	public abstract int MaxTriggers { get; }

	public virtual DiscordMessageBuilder? Execute(BattlePlayer caster, BattlePlayer target, out bool battleEnd, out bool turnSkip, out bool roundRepeat) 
	{
		if (caster.PassiveTriggers == MaxTriggers || MaxTriggers == -1) 
		{
			caster.PassiveTriggers++;
			return Action(caster, target, out battleEnd, out turnSkip, out roundRepeat);
		}
		else 
		{
			battleEnd = true;
			turnSkip = false;
			roundRepeat = false;

			return null;
		}
	}

	protected abstract DiscordMessageBuilder? Action(BattlePlayer caster, BattlePlayer target, out bool battleEnd, out bool turnSkip, out bool roundRepeat);
}

#region On Death Passives
public abstract class OnDeathPassive : Passive { }

public class GamblersDream : OnDeathPassive 
{
	public override int MaxTriggers => -1;
	
	protected override DiscordMessageBuilder? Action(BattlePlayer caster, BattlePlayer target, out bool battleEnd, out bool turnSkip, out bool roundRepeat)
	{
		turnSkip = false;
		roundRepeat = false;
		
		if (DiscordController.RNG.NextDouble() < 0.02) 
		{
			battleEnd = false;
			var heal = new Heal(healPercent: 1);
			return heal.Execute(caster, target);
		}
		else 
		{
			battleEnd = true;
			return null;
		}
	}
}

public class StrayCatReincarnation : OnDeathPassive 
{
	public override int MaxTriggers => 1;
	protected override DiscordMessageBuilder? Action(BattlePlayer caster, BattlePlayer target, out bool battleEnd, out bool turnSkip, out bool roundRepeat)
	{
		battleEnd = false;
		turnSkip = false;
		roundRepeat = false;
		
		var heal = new Heal(healPercent: 0.4);

		return heal.Execute(caster: caster, target: target);
	}
}
#endregion
