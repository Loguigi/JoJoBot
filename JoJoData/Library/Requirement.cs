using System.Text;
using DSharpPlus.Entities;

namespace JoJoData.Library;

public abstract class Requirement 
{
	public abstract bool Check(BattlePlayer caster, BattlePlayer target, out DiscordMessageBuilder? msg);

	public abstract StringBuilder GetLongDescription();
}

public class StatusRequirement(Type statusType) : Requirement 
{
	public readonly Type StatusType = statusType;

	public override bool Check(BattlePlayer caster, BattlePlayer target, out DiscordMessageBuilder? msg)
	{
		if (target.Status is not null && target.Status.GetType() == StatusType) 
		{
			msg = null;
			return true;
		}
		else 
		{
			msg = new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(caster.User.GlobalName, "", caster.User.AvatarUrl)
				.WithDescription($"❌ {target.User.Mention} requires {StatusType.Name} status to use this ability!")
				.WithColor(DiscordColor.Red));

			return false;
		}
	}
	
	public override StringBuilder GetLongDescription() => new($"* 🔒 Requires enemy to have status **{StatusType.Name}**");
}