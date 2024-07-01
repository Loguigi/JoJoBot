using DSharpPlus.Entities;
using JoJoData.Library;
using JoJoData.Controllers;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;

namespace JoJoBot.Commands.Text;

public class AdminCommands
{
	[Command("givestand"), RequireGuild]
	public async Task GiveStand(CommandContext ctx, DiscordUser user, int id) 
	{
		var player = new Player(ctx.Guild!, user);
		if (!StandLoader.TryGetStand(id, out var stand)) 
		{
			await ctx.RespondAsync("Stand not found");
		}

		player.Stand = stand;
		player.Save();

		await ctx.RespondAsync($"Gave `{player.Stand.Id}` **{player.Stand.Name}** to {user.GlobalName}");
	}
}