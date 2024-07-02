using System.Reflection;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Entities;
using JoJoBot.Handlers;
using JoJoData.Helpers;
using JoJoData.Library;

namespace JoJoBot.Commands.Slash;


public class BattleCommand
{
	[Command("battle"), RequireGuild]
	public async Task Execute(CommandContext ctx, DiscordUser player)
	{
		try 
		{
			if (player.IsBot || player == ctx.User) 
			{
				await ctx.RespondAsync("https://tenor.com/view/jotaro-jojo-jojo-bizarre-adventure-bike-jojo-bike-gif-4228712888990777929");
				return;
			}
			var player1 = new Player(ctx.Guild!, ctx.User);
			var player2 = new Player(ctx.Guild!, player);
			
			if (player1.Stand is null) 
			{
				// TODO no stand error
				//await ctx.CreateResponseAsync()
			}

			var embed = new DiscordEmbedBuilder()
				.WithDescription($"### {DiscordEmoji.FromName(ctx.Client, ":crossed_swords:", false)} {player1.User.Mention} (Lv. {player1.Level}) challenges {player2.User.Mention} (Lv. {player2.Level})!")
				.WithThumbnail(player1.Stand!.ImageUrl)
				.WithColor(DiscordColor.Gold)
				.WithFooter("Do you accept?", player2.User.AvatarUrl);

			var acceptBtn = new DiscordButtonComponent(DiscordButtonStyle.Success, $"{IDHelper.Battle.PLAYER_CHALLENGE_ACCEPT}\\{player1.User.Id}\\{player2.User.Id}", "Accept");
			var declineBtn = new DiscordButtonComponent(DiscordButtonStyle.Danger, $"{IDHelper.Battle.PLAYER_CHALLENGE_DECLINE}\\{player1.User.Id}\\{player2.User.Id}", "Decline");

			var msg = new DiscordMessageBuilder()
				.WithContent(player2.User.Mention)
				.AddMention(new UserMention(player2.User))
				.AddEmbed(embed)
				.AddComponents(new DiscordComponent[] {acceptBtn, declineBtn});

			await ctx.RespondAsync(msg);
		}
		catch (Exception ex) 
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}
}