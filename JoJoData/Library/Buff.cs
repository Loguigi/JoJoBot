using DSharpPlus;
using DSharpPlus.Entities;

namespace JoJoData.Library;

public abstract class Buff(int duration) 
{
	public abstract string Name { get; }
	public virtual string ShortDescription => $"{Name} {Duration} turns";
	public readonly int Duration = duration;
	
	public virtual DiscordMessageBuilder Apply(BattlePlayer target) 
	{
		target.AddBuff(this);
		target.BuffDuration = Duration;

		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
			.WithDescription($"⬆️ **{Name} for `{Duration}` turns**")
			.WithColor(DiscordColor.SpringGreen));
	}

	public virtual DiscordMessageBuilder? Execute(BattlePlayer target) 
	{
		if (target.ReduceBuffDuration()) 
		{
			return null;
		}
		else 
		{
			return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
				.WithDescription($"**{Name} has worn off**")
				.WithColor(DiscordColor.Green));
		}
	}
}

public class Protect(int duration, double dr) : Buff(duration) 
{
	public override string Name => "🛡️ Protect";
	public double DamageResistance { get; set; } = dr;
	
	public void GrantProtect(BattlePlayer target) 
	{
		target.DamageResistance += DamageResistance;
	}
}

public class Haste(int duration) : Buff(duration) 
{
	public override string Name => $"💨 Haste";

	public override DiscordMessageBuilder? Execute(BattlePlayer target)
	{
		return base.Execute(target);
	}
}

public class Await() : Buff(duration: 2)
{
	public override string Name => $"🗡️ Await";
	
	public DiscordMessageBuilder Action(BattlePlayer caster, BattlePlayer target, out bool evade) 
	{
		if (caster.BuffDuration == 2) 
		{
			evade = true;
			return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(caster.User.GlobalName, "", caster.User.AvatarUrl)
				.WithDescription($"🍃 **{caster.Stand!.CoolName} evades the attack!**")
				.WithColor(DiscordColor.White));
		}
		else // duration == 1
		{
			evade = false;
			return new BasicAttack(damage: 3).Execute(attacker: caster, defender: target);
		}
	}

	public bool IsEvadeTurn(BattlePlayer caster) => caster.BuffDuration == 2;

	public bool IsAttackTurn(BattlePlayer caster) => caster.BuffDuration == 1;
}

public class Charge(int duration) : Buff(duration) 
{
	public override string Name => $"💪 Charge";

	public override DiscordMessageBuilder? Execute(BattlePlayer target)
	{
		target.MinDamage *= 2;
		target.MaxDamage *= 2;
		return base.Execute(target);
	}
}