
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
	
	public DiscordSelectComponentOption CreateSelection(DiscordClient s, int abilityNum) 
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
			$"{CoolName} 💎 {MpCost} MP",
			abilityNum.ToString(),
			FormatShortDescription(),
			false,
			new DiscordComponentEmoji(DiscordEmoji.FromName(s, emoji, false)));
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

		return desc.ToString();
	}
}
#endregion

#region Ability Subclasses
public abstract class AttackAbility : Ability 
{
	public required Attack Attack { get; set; }
}

public abstract class StatusAttackAbility : AttackAbility 
{
	public required Status Status { get; set; }
}

public abstract class InflictStatusAbility : Ability 
{
	public required Status Status { get; set; }
}

public abstract class BuffAbility : Ability 
{
	public required Buff Buff { get; set; }
}

public abstract class BuffAttackAbility : AttackAbility 
{
	public required Buff Buff { get; set; }
}

public abstract class StatChangeAbility : Ability 
{
	public required StatChange StatChange { get; set; }
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