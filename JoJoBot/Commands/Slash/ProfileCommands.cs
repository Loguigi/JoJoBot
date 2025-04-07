using System.Reflection;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using JoJoLibrary;

namespace JoJoBot.Commands.Slash;

public class ProfileCommands 
{
	[Command("profile")]
	public async Task Profile(SlashCommandContext ctx, DiscordUser? user=null) 
	{
		try
		{
			if (user! != null! && user!.IsBot) 
			{
				await ctx.RespondAsync("https://tenor.com/view/jojo-all-star-battle-r-jojos-bizarre-adventure-rise-and-shine-the-shining-jotaro-gif-27591331");
				return;
			}

			Player player = new(ctx.Guild!, user! != null! ? user : ctx.User);
			player.Load();
			
			if (player.Stand == null) 
			{
				await ctx.RespondAsync(new DiscordEmbedBuilder()
					.WithDescription($"❌ {player.User.Mention} doesn't have a Stand!")
					.WithColor(DiscordColor.Red), ephemeral: true);
				return;
			}

			var embed = new DiscordEmbedBuilder()
				.WithColor(DiscordColor.Blue)
				.WithAuthor(player.User.GlobalName, "", player.User.AvatarUrl)
				.WithTitle(player.Stand!.CoolName)
				.WithThumbnail(player.Stand!.ImageUrl)
				.AddField("Level", player.Level.ToString(), true)
				.AddField("Experience", $"{player.Experience} / {player.ExpNeededForNextLevel}", true)
				.AddField("Win/Loss", $"{player.BattlesWon} / {player.BattlesLost}", true)
				.WithTimestamp(DateTime.Now);

			await ctx.RespondAsync(embed);
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}
}