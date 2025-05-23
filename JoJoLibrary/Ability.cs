
using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;

namespace JoJoLibrary;

#region Base Class
public abstract class Ability
{
	public string Name { get; set; } = string.Empty;
	public string CoolName => $"「{Name}」";
	public string Description { get; protected set; } = string.Empty;
	public int MpCost { get; protected set; } = 0;
	public int Cooldown { get; protected set; } = 0;
	public Requirement? Requirement { get; protected set; } = null;

	public abstract void Use(Turn turn);
	
	public DiscordSelectComponentOption CreateSelection(DiscordClient s, int abilityNum, BattlePlayer currentPlayer) 
	{
		string emoji = abilityNum switch
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

	public string FormatLongDescription(Stand stand, BattlePlayer? player = null)
	{
		StringBuilder desc = new();

		if (this is AttackAbility ab)
		{
			desc.Append(ab.Attack.GetLongDescription(stand, player));

			if (this is StatusAttackAbility saa)
			{
				desc.Append(saa.Status.GetLongDescription(stand, player));
			}
			else if (this is BuffAttackAbility baa)
			{
				desc.Append(baa.Buff.GetLongDescription(stand, player));
			}
		}

		if (this is InflictStatusAbility isa)
		{
			desc.Append(isa.Status.GetLongDescription(stand, player));
		}

		if (this is BuffAbility ba)
		{
			desc.Append(ba.Buff.GetLongDescription(stand, player));
		}

		if (this is StatChangeAbility sca)
		{
			desc.Append(sca.StatChange.GetLongDescription(stand, player));
		}

		if (Cooldown > 0)
		{
			desc.AppendLine($"* ⏱️ Cooldown: `{Cooldown}` turns");
		}

		if (Requirement is not null)
		{
			desc.Append(Requirement.GetLongDescription());
			desc.AppendLine();
		}
		
		return desc.ToString();
	}

	private string CreateAbilityTitle(BattlePlayer currentPlayer) 
	{
		if (currentPlayer.Cooldowns[this] > 0) 
		{
			return $"{CoolName} 💎 {MpCost} MP ⏱️ {currentPlayer.Cooldowns[this]} {(currentPlayer.Cooldowns[this] == 1 ? "turn" : "turns")}";
		}
		
		return $"{CoolName} 💎 {MpCost} MP";
	}

	private string FormatShortDescription()
	{
		StringBuilder desc = new();

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
			desc.Append($"⏱️ CD: {Cooldown} turns");
		}

		return desc.ToString();
	}
}
#endregion

#region Ability Subclasses
public abstract class AttackAbility : Ability 
{
	public Attack Attack { get; protected set; } = new BasicAttack(damage: 1);

	public override void Use(Turn turn)
	{
		Attack.Execute(turn, turn.Caster, turn.Target);
	}
}

public abstract class StatusAttackAbility : AttackAbility
{
	public Status Status { get; protected set; } = new Random(duration: 1);

	public override void Use(Turn turn)
	{
		base.Use(turn);
		Status.Apply(turn, turn.Caster, turn.Target);
	}
}

public abstract class InflictStatusAbility : Ability 
{
	public Status Status { get; protected set; } = new Random(duration: 1);

	public override void Use(Turn turn)
	{
		Status.Apply(turn, turn.Caster, turn.Target);
	}
}

public abstract class BuffAbility : Ability 
{
	public Buff Buff { get; protected set; } = new Protect(duration: 1, dr: 0.25);

	public override void Use(Turn turn)
	{
		Buff.Apply(turn, turn.Caster, turn.Caster);
	}
}

public abstract class BuffAttackAbility : AttackAbility 
{
	public Buff Buff { get; protected set; } = new Protect(duration: 1, dr: 0.25);

	public override void Use(Turn turn)
	{
		base.Use(turn);
		Buff.Apply(turn, turn.Caster, turn.Caster);
	}
}

public abstract class StatChangeAbility : Ability 
{
	public StatChange StatChange { get; protected set; } = new Heal(healPercent: 0.1);

    public override void Use(Turn turn)
    {
		StatChange.Execute(turn, turn.Caster, turn.Target);
    }
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