using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
using JoJoData.Helpers;

namespace JoJoData.Library;

#region Abstract Status
public abstract class Status(int duration, double applyChance) {
	public readonly int Duration = duration;
	public readonly double ApplyChance = applyChance;

	public abstract string GetName(DiscordClient s);
	
	public virtual string GetDescription(DiscordClient s) =>
		$"{GetName(s)}\n{DiscordEmoji.FromName(s, ":game_die:")} Chance to Apply: {ApplyChance * 100}%\n{DiscordEmoji.FromName(s, ":clock:")} Duration: {Duration} turns\n";

	public virtual DiscordMessageBuilder? TryApply(BattlePlayer caster, BattlePlayer target)
	{
		if (!RollStatus()) 
		{
			return null;
		}

		Apply(caster, target);
		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
			.WithDescription($"⬇️ **{GetName(target.Client)} for `{target.StatusDuration}` turns**")
			.WithColor(DiscordColor.Purple));
	}

	public virtual DiscordMessageBuilder? Execute(BattlePlayer caster, BattlePlayer target) 
	{
		if (target.ReduceStatusDuration()) 
		{
			return Action(caster, target);
		}
		else 
		{
			return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
				.WithDescription($"**{GetName(target.Client)} has worn off**")
				.WithColor(DiscordColor.Green));
		}
	}

	protected virtual void Apply(BattlePlayer caster, BattlePlayer target) 
	{
		target.AddStatus(this);
		target.StatusDuration = Duration;
	}

	protected abstract DiscordMessageBuilder? Action(BattlePlayer caster, BattlePlayer target);
	
	protected bool RollStatus() => RandomHelper.RNG.NextDouble() < ApplyChance;
}
#endregion

#region Damage Statuses
public abstract class DamageStatus(int duration, double applyChance) : Status(duration, applyChance) 
{
	protected override DiscordMessageBuilder Action(BattlePlayer caster, BattlePlayer target)
	{
		target.ReceiveDamage(target.DamageOverTime);

		return new DiscordMessageBuilder();
	}
}

public class Burn(int duration, double applyChance = 1) : DamageStatus(duration, applyChance) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":fire:")} Burn";

	protected override DiscordMessageBuilder Action(BattlePlayer caster, BattlePlayer target)
	{
		return base.Action(caster, target).AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
			.WithDescription($"🔥 **{target.Stand!.CoolName} burns for `{target.DamageOverTime}` damage**")
			.WithColor(DiscordColor.Orange));
	}

	protected override void Apply(BattlePlayer caster, BattlePlayer target)
	{
		target.DamageOverTime = (int)(caster.MinDamage * 2.5);
		base.Apply(caster, target);
	}
}

public class Bleed(int duration, double applyChance = 1) : DamageStatus(duration, applyChance) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":drop_of_blood:")} Bleed";

	protected override DiscordMessageBuilder Action(BattlePlayer caster, BattlePlayer target)
	{
		return base.Action(caster, target).AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
			.WithDescription($"🩸 **{target.Stand!.CoolName} bleeds for `{target.DamageOverTime}` damage**")
			.WithColor(DiscordColor.DarkRed));
	}

	protected override void Apply(BattlePlayer caster, BattlePlayer target)
	{
		target.DamageOverTime = (int)Math.Ceiling(target.Hp * 0.25);
		base.Apply(caster, target);
	}
}

public class Poison(int duration, double applyChance = 1) : DamageStatus(duration, applyChance) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":snake:")} Poison";

	protected override DiscordMessageBuilder Action(BattlePlayer caster, BattlePlayer target)
	{
		var output = base.Action(caster, target).AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
			.WithDescription($"💀 **{target.Stand!.CoolName} suffers poison for `{target.DamageOverTime}` damage**")
			.WithColor(DiscordColor.Purple));
		target.DamageOverTime -= (int)Math.Ceiling(target.DamageOverTime * 0.3);
		return output;
	}

	protected override void Apply(BattlePlayer caster, BattlePlayer target)
	{
		target.DamageOverTime = (int)Math.Ceiling(target.MaxHp * 0.2);
		base.Apply(caster, target);
	}
}

public class Doom(int duration, double applyChance = 1) : DamageStatus(duration, applyChance) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":skull:")} Doom";

	protected override DiscordMessageBuilder Action(BattlePlayer caster, BattlePlayer target)
	{
		if (target.StatusDuration == 1) 
		{
			return new BypassProtectAttack(damage: 9999).Execute(caster, target);
		}
		else 
		{
			return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
				.WithDescription("Death approaches..."));
		}
	}
}
#endregion

#region Passive Statuses
public abstract class PassiveStatus(int duration, double applyChance) : Status(duration, applyChance) 
{
	protected override DiscordMessageBuilder? Action(BattlePlayer caster, BattlePlayer target) => null;
}

public class Silence(int duration, double applyChance = 1) : PassiveStatus(duration, applyChance) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":mute:")} Silence";

	public DiscordMessageBuilder SilenceMessage(BattlePlayer target) => new DiscordMessageBuilder().AddEmbed(
		new DiscordEmbedBuilder()
		.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
		.WithDescription($"{DiscordEmoji.FromName(target.Client, ":mute:", false)} Silenced! Cannot use MP abilities for {target.StatusDuration} turns!")
		.WithColor(DiscordColor.Black)
	);
}

public class Confusion(int duration, double applyChance = 1) : PassiveStatus(duration, applyChance) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":question:")} Confusion";

	public bool RollConfusion() => RandomHelper.RNG.NextDouble() < 0.5;
}

public class Douse(int duration, double applyChance = 1) : PassiveStatus(duration, applyChance) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":oil:")} Douse";

	public DiscordMessageBuilder Ignite(BattlePlayer caster, BattlePlayer target) 
	{
		var ignite = new Burn(duration: 3, applyChance: 1);
		return ignite.TryApply(caster, target)!;
	}
}

public class Frail(int duration, double applyChance = 1) : PassiveStatus(duration, applyChance)
{
	public readonly double DRReduction = 0.25;

	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":umbrella:")} Frail";

	public void Weaken(BattlePlayer target) => target.DamageResistance -= DRReduction;
}

public class Shock(int duration, double applyChance = 1) : PassiveStatus(duration, applyChance) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":zap:")} Shock";

	public DiscordMessageBuilder Electrocute(BattlePlayer caster, BattlePlayer target) 
	{
		var hpBefore = target.Hp;
		target.ReceiveDamage((int)(caster.DamageReceived * 0.5));

		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
			.WithDescription($"⛈️ **{target.Stand!.CoolName} shocks themself for `{caster.DamageReceived * 0.5}` damage!**")
			.WithColor(DiscordColor.Yellow)
			.WithFooter($"❤️ {hpBefore} ➡️ ❤️ {target.Hp}"));
	}
}
#endregion

#region Turn Skip Statuses
public abstract class TurnSkipStatus(int duration, double applyChance) : Status(duration, applyChance) 
{

}

public class TimeStop(int duration, double applyChance = 1) : TurnSkipStatus(duration, applyChance) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":clock10:")} Time Stop";

	public override DiscordMessageBuilder? TryApply(BattlePlayer caster, BattlePlayer target)
	{
		if (caster.Stand!.Name == "Star Platinum") 
		{
			return base.TryApply(caster, target)!.AddEmbed(new DiscordEmbedBuilder().WithImageUrl("https://c.tenor.com/0kMboLznZGwAAAAC/tenor.gif"));
		}
		else if (caster.Stand!.Name == "The World")
		{
			return base.TryApply(caster, target)!.AddEmbed(new DiscordEmbedBuilder().WithImageUrl("https://c.tenor.com/R_NQbI9vk1UAAAAC/tenor.gif"));
		}
		else
		{
			return base.TryApply(caster, target)!.AddEmbed(new DiscordEmbedBuilder().WithImageUrl("https://c.tenor.com/qAhqQaBghmkAAAAC/tenor.gif"));
		}
	}

	protected override DiscordMessageBuilder Action(BattlePlayer caster, BattlePlayer target)
	{
		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
			.WithDescription("🕐 **Time is frozen...** 🕥")
			.WithColor(DiscordColor.DarkBlue));
	}
}

public class Sleep(int duration, double applyChance = 1) : TurnSkipStatus(duration, applyChance) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":zzz:")} Sleep";

	public DiscordMessageBuilder? RollForWakeUp(BattlePlayer target) {
		var awake = RandomHelper.RNG.NextDouble() < 0.5;
		if (!awake)
		{
			return null;
		}

		target.ReduceStatusDuration(remove: true);

		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithDescription($"⏰ **{target.Stand!.CoolName} ({target.User.Mention}) has woken up!**")
			.WithColor(DiscordColor.Blurple));
	}

	protected override DiscordMessageBuilder Action(BattlePlayer caster, BattlePlayer target)
	{
		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
			.WithDescription("💤**Asleep...** 💤")
			.WithColor(DiscordColor.DarkBlue));
	}
}
#endregion

public class Random(int duration, double applyChance = 1) : Status(duration, applyChance)
{
	public override string GetName(DiscordClient s) => $"🎲 Random";

	protected override DiscordMessageBuilder? Action(BattlePlayer caster, BattlePlayer target) => null;

	protected override void Apply(BattlePlayer caster, BattlePlayer target)
	{
		var statuses = Assembly.GetExecutingAssembly().GetTypes().Where(x =>
			x.IsClass &&
			x.Namespace != null &&
			x.Namespace.Contains("JoJoData.Library")).ToList().Where(y =>
			!y.IsAbstract && (y.BaseType == typeof(PassiveStatus) || y.BaseType == typeof(PassiveStatus) || y.BaseType == typeof(TurnSkipStatus))).ToList();

		var selection = RandomHelper.RNG.Next(0, statuses.Count);
		if (Activator.CreateInstance(statuses[selection], Duration, ApplyChance) is Status status)
		{
			status.TryApply(caster, target);
		}
	}
}