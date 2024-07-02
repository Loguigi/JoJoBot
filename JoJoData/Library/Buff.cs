using DSharpPlus;
using DSharpPlus.Entities;

namespace JoJoData.Library;

public abstract class Buff(int duration) 
{
	public readonly int Duration = duration;

	public abstract string GetName(DiscordClient s);
	
	public virtual DiscordMessageBuilder Apply(BattlePlayer target) 
	{
		target.AddBuff(this);
		target.BuffDuration = Duration;

		return new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
			.WithAuthor(target.User.GlobalName, "", target.User.AvatarUrl)
			.WithDescription($"⬆️ **{GetName(target.Client)} for `{Duration}` turns**")
			.WithColor(DiscordColor.Aquamarine));
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
				.WithDescription($"**{GetName(target.Client)} has worn off**")
				.WithColor(DiscordColor.Green));
		}
	}
}

public class Protect(int duration, double dr) : Buff(duration) 
{
	public double DamageResistance { get; set; } = dr;
	
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":shield:")} Protect";
	
	public void GrantProtect(BattlePlayer target) 
	{
		target.DamageResistance += DamageResistance;
	}
}

public class Haste(int duration) : Buff(duration) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":dash:")} Haste";

	public override DiscordMessageBuilder? Execute(BattlePlayer target)
	{
		return base.Execute(target);
	}
}

public class Await() : Buff(duration: 2)
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":dagger:")} Await";
}

public class Charge(int duration) : Buff(duration) 
{
	public override string GetName(DiscordClient s) => $"{DiscordEmoji.FromName(s, ":muscle:")} Charge";

	public override DiscordMessageBuilder? Execute(BattlePlayer target)
	{
		target.MinDamage *= 2;
		target.MaxDamage *= 2;
		return base.Execute(target);
	}
}