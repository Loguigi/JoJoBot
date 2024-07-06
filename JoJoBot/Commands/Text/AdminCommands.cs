using DSharpPlus.Entities;
using JoJoData.Library;
using JoJoData.Controllers;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using System.Reflection;

namespace JoJoBot.Commands.Text;

public class AdminCommands
{
	[Command("givestand"), RequireGuild]
	public async Task GiveStand(CommandContext ctx, DiscordUser user, int id) 
	{
		try
		{
			var player = new Player(ctx.Guild!, user);
			player.Load();
			if (!StandLoader.TryGetStand(id, out var stand))
			{
				await ctx.RespondAsync("Stand not found");
			}

			player.ChangeStand(stand);

			await ctx.RespondAsync($"Gave `{player.Stand!.Id}` **{player.Stand.Name}** to {user.GlobalName}");
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}
}