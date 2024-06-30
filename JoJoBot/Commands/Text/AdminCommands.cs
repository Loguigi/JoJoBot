using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using JoJoData.Library;
using JoJoData.Controllers;
using DSharpPlus.CommandsNext.Attributes;

namespace JoJoBot.Commands.Text;

public class AdminCommands : BaseCommandModule
{
	[Command("givestand")]
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