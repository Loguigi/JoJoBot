
using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;

namespace JoJoData.Library;

#region Base Class
public abstract class Ability
{
	public string Name { get; set; } = string.Empty;
	public string CoolName => $"「{Name}」";
	public string Description { get; protected set; } = string.Empty;
	public int MpCost { get; protected set; } = 0;
	public int Cooldown { get; protected set; } = 0;
	
	public DiscordSelectComponentOption CreateSelection(DiscordClient s, int abilityNum, BattlePlayer currentPlayer) 
	{
		var emoji = abilityNum switch
		{
			0 => ":punch:",
			1 => ":one:",
			2 => ":two:",
			3 => ":three:",
			4 => ":four:",
			_ => throw new Exception("Invalid ability number")
		};
		
		return new DiscordSelectComponentOption(
			CreateAbilityTitle(currentPlayer),
			abilityNum.ToString(),
			FormatShortDescription(),
			false,
			new DiscordComponentEmoji(DiscordEmoji.FromName(s, emoji, false)));
	}
	
	protected string CreateAbilityTitle(BattlePlayer currentPlayer) 
	{
		if (currentPlayer.Cooldowns[this] > 0) 
		{
			return $"{CoolName} 💎 {MpCost} MP ⏱️ {currentPlayer.Cooldowns[this]} {(currentPlayer.Cooldowns[this] == 1 ? "turn" : "turns")}";
		}
		else 
		{
			return $"{CoolName} 💎 {MpCost} MP";
		}
	}

	protected string FormatShortDescription()
	{
		var desc = new StringBuilder();
		desc.AppendLine(Description);

		if (this is AttackAbility ab)
		{
			desc.Append(ab.Attack.ShortDescription + " ");

			if (this is StatusAttackAbility saa)
			{
				desc.Append(saa.Status.ShortDescription);
			}
			else if (this is BuffAttackAbility baa)
			{
				desc.Append(baa.Buff.ShortDescription);
			}
		}

		if (this is InflictStatusAbility isa)
		{
			desc.Append(isa.Status.ShortDescription);
		}

		if (this is BuffAbility ba)
		{
			desc.Append(ba.Buff.ShortDescription);
		}

		if (this is StatChangeAbility sca)
		{
			desc.Append(sca.StatChange.ShortDescription);
		}
		
		if (Cooldown > 0) 
		{
			desc.Append($" ⏱️ CD: {Cooldown} turns");
		}

		return desc.ToString();
	}
}
#endregion

#region Ability Subclasses
public abstract class AttackAbility : Ability 
{
	public Attack Attack { get; protected set; } = new BasicAttack(damage: 1);
}

public abstract class StatusAttackAbility : AttackAbility 
{
	public Status Status { get; protected set; } = new Random(duration: 1);
}

public abstract class InflictStatusAbility : Ability 
{
	public Status Status { get; protected set; } = new Random(duration: 1);
}

public abstract class BuffAbility : Ability 
{
	public Buff Buff { get; protected set; } = new Protect(duration: 1, dr: 0.25);
}

public abstract class BuffAttackAbility : AttackAbility 
{
	public Buff Buff { get; protected set; } = new Protect(duration: 1, dr: 0.25);
}

public abstract class StatChangeAbility : Ability 
{
	public StatChange StatChange { get; protected set; } = new Heal(healPercent: 0.1);
}
#endregion

#region Standard Attack Ability
public class StandardAttack : AttackAbility
{
	public StandardAttack()
	{
		Name = "Attack";
		MpCost = 0;
		Attack = new BasicAttack(damage: 1);
	}
}
#endregion