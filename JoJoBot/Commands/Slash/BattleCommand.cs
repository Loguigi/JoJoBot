using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using JoJoBot.Handlers;
using JoJoData.Helpers;
using JoJoData.Library;

namespace JoJoBot.Commands.Slash;


public class BattleCommand : ApplicationCommandModule
{
	[GuildOnly]
	[SlashCommand("battle", "Challenge a friend to a battle")]
	public async Task Execute(InteractionContext ctx,
		[Option("player", "Friend you want to have a stand off with")] DiscordUser player)
	{
		try 
		{
			if (player.IsBot || player == ctx.User) 
			{
				await ctx.CreateResponseAsync("https://tenor.com/view/jotaro-jojo-jojo-bizarre-adventure-bike-jojo-bike-gif-4228712888990777929");
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

			var acceptBtn = new DiscordButtonComponent(ButtonStyle.Success, $"{IDHelper.Battle.PLAYER_CHALLENGE_ACCEPT}\\{player1.User.Id}\\{player2.User.Id}", "Accept");
			var declineBtn = new DiscordButtonComponent(ButtonStyle.Danger, $"{IDHelper.Battle.PLAYER_CHALLENGE_DECLINE}\\{player1.User.Id}\\{player2.User.Id}", "Decline");

			var msg = new DiscordMessageBuilder()
				.WithContent(player2.User.Mention)
				.AddMention(new UserMention(player2.User))
				.AddEmbed(embed)
				.AddComponents(new DiscordComponent[] {acceptBtn, declineBtn});

			await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(msg));
		}
		catch (Exception ex) 
		{
			throw;
		}
	}
}