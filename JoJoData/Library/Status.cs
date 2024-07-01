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

		Apply(target);
		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithDescription($"⬇️ **{target.User.Mention}: {GetName(target.Client)} for `{target.StatusDuration}` turns**")
			.WithColor(DiscordColor.Purple));
	}

	public virtual DiscordMessageBuilder Execute(BattlePlayer caster, BattlePlayer target) 
	{
		if (target.ReduceStatusDuration()) 
		{
			return new DiscordMessageBuilder();
		}
		else 
		{
			return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithDescription($"**{target.User.Mention}: {GetName(target.Client)} has worn off**")
				.WithColor(DiscordColor.Green));
		}
	}

	protected virtual void Apply(BattlePlayer target) 
	{
		target.AddStatus(this);
		target.StatusDuration = Duration;
	}
	
	protected bool RollStatus() => RandomHelper.RNG.NextDouble() < ApplyChance;
}
#endregion

#region Damage Statuses
public abstract class DamageStatus(int duration, double applyChance) : Status(duration, applyChance) 
{
	public override DiscordMessageBuilder Execute(BattlePlayer caster, BattlePlayer target)
	{
		target.ReceiveDamage(target.DamageOverTime);

		return base.Execute(caster, target);
	}
}

public class Burn(int duration, double applyChance) : DamageStatus(duration, applyChance) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":fire:")} Burn";

	public override DiscordMessageBuilder Execute(BattlePlayer caster, BattlePlayer target)
	{
		return base.Execute(caster, target).AddEmbed(new DiscordEmbedBuilder()
			.WithDescription($"🔥 **{target.Stand!.CoolName} ({target.User.Mention}) burns for `{target.DamageOverTime}` damage**")
			.WithColor(DiscordColor.Orange));
	}

	protected override void Apply(BattlePlayer target)
	{
		target.DamageOverTime = target.DamageReceived * 2;
		base.Apply(target);
	}
}

public class Bleed(int duration, double applyChance) : DamageStatus(duration, applyChance) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":drop_of_blood:")} Bleed";

	public override DiscordMessageBuilder Execute(BattlePlayer caster, BattlePlayer target)
	{
		return base.Execute(caster, target).AddEmbed(new DiscordEmbedBuilder()
			.WithDescription($"🩸 **{target.Stand!.CoolName} ({target.User.Mention}) bleeds for `{target.DamageOverTime}` damage**")
			.WithColor(DiscordColor.DarkRed));
	}

	protected override void Apply(BattlePlayer target)
	{
		target.DamageOverTime = (int)Math.Ceiling(target.Hp * 0.1);
		base.Apply(target);
	}
}

public class Poison(int duration, double applyChance) : DamageStatus(duration, applyChance) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":snake:")} Poison";

	public override DiscordMessageBuilder Execute(BattlePlayer caster, BattlePlayer target)
	{
		var output = base.Execute(caster, target).AddEmbed(new DiscordEmbedBuilder()
			.WithDescription($"💀 **{target.Stand!.CoolName} ({target.User.Mention}) suffers poison for `{target.DamageOverTime}` damage**")
			.WithColor(DiscordColor.Purple));
		target.DamageOverTime -= (int)Math.Ceiling(target.DamageOverTime * 0.3);
		return output;
	}

	protected override void Apply(BattlePlayer target)
	{
		target.DamageOverTime = (int)Math.Ceiling(target.MaxHp * 0.1);
		base.Apply(target);
	}
}
#endregion

#region Passive Statuses
public abstract class PassiveStatus(int duration, double applyChance) : Status(duration, applyChance) {}

public class Silence(int duration, double applyChance) : PassiveStatus(duration, applyChance) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":mute:")} Silence";

	public DiscordMessageBuilder SilenceMessage(BattlePlayer target) => new DiscordMessageBuilder().AddEmbed(
		new DiscordEmbedBuilder()
		.WithDescription($"{DiscordEmoji.FromName(target.Client, ":mute:", false)} Silenced! Cannot use MP abilities for {target.StatusDuration} turns!")
		.WithColor(DiscordColor.Black)
	);
}

public class Confusion(int duration, double applyChance) : PassiveStatus(duration, applyChance) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":question:")} Confusion";
}

public class Douse(int duration, double applyChance) : PassiveStatus(duration, applyChance) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":oil:")} Douse";

	public DiscordMessageBuilder Ignite(BattlePlayer caster, BattlePlayer target) 
	{
		var ignite = new Burn(duration: 3, applyChance: 1);
		return ignite.TryApply(caster, target)!;
	}
}

public class Weak(int duration, double drReduction) : PassiveStatus(duration, 1)
{
	public readonly double DRReduction = drReduction;

	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":umbrella:")} Weak";

	public void Weaken(BattlePlayer target) => target.DamageResistance -= DRReduction;
}
#endregion

#region Turn Skip Statuses
public abstract class TurnSkipStatus(int duration, double applyChance) : Status(duration, applyChance) {
	
}

public class TimeStop(int duration) : TurnSkipStatus(duration, 1) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":clock10:")} Time Stop";

	public override DiscordMessageBuilder? TryApply(BattlePlayer caster, BattlePlayer target)
	{
		if (caster.Stand!.Name == "Star Platinum") 
		{
			return base.TryApply(caster, target)!.WithContent("https://tenor.com/view/star-platinum-za-warduo-gif-26209527");
		}
		else if (caster.Stand!.Name == "The World")
		{
			return base.TryApply(caster, target)!.WithContent("https://tenor.com/view/dio-the-world-power-charge-gif-13331683");
		}
		else
		{
			return base.TryApply(caster, target);
		}
	}
}

public class Sleep(int duration, double applyChance) : TurnSkipStatus(duration, applyChance) 
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
}
#endregion