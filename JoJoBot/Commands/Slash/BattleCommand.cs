using System.Reflection;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using JoJoBot.Handlers;
using JoJoData.Helpers;
using JoJoData.Library;

namespace JoJoBot.Commands.Slash;


public class BattleCommand
{
	[Command("battle"), RequireGuild]
	public async Task Execute(SlashCommandContext ctx, DiscordUser player)
	{
		try 
		{
			if (player.IsBot || player == ctx.User) 
			{
				await ctx.RespondAsync("https://tenor.com/view/jotaro-jojo-jojo-bizarre-adventure-bike-jojo-bike-gif-4228712888990777929");
				return;
			}
			Player player1 = new(ctx.Guild!, ctx.User);
			Player player2 = new(ctx.Guild!, player);
			player1.Load();
			player2.Load();
			
			if (player1.Stand is null) 
			{
				await ctx.RespondAsync(new DiscordEmbedBuilder()
					.WithDescription("❌ You don't have a Stand to fight with!")
					.WithColor(DiscordColor.Red), ephemeral: true);
				return;
			}
			else if (player2.Stand is null) 
			{
				await ctx.RespondAsync(new DiscordEmbedBuilder()
					.WithDescription($"❌ {player2.User.Mention} doesn't have a Stand to fight with!")
					.WithColor(DiscordColor.Red), ephemeral: true);
				return;
			}

			var embed = new DiscordEmbedBuilder()
				.WithDescription($"### {DiscordEmoji.FromName(ctx.Client, ":crossed_swords:", false)} {player1.User.Mention} (Lv. {player1.Level}) challenges {player2.User.Mention} (Lv. {player2.Level})!")
				.WithColor(DiscordColor.Gold)
				.WithFooter("Do you accept?", player2.User.AvatarUrl);

			var acceptBtn = new DiscordButtonComponent(DiscordButtonStyle.Success, $"{IDHelper.Battle.PlayerChallengeAccept}\\{player1.User.Id}\\{player2.User.Id}", "Accept");
			var declineBtn = new DiscordButtonComponent(DiscordButtonStyle.Danger, $"{IDHelper.Battle.PlayerChallengeDecline}\\{player1.User.Id}\\{player2.User.Id}", "Decline");

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