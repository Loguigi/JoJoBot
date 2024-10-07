using System.Reflection;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using JoJoData.Helpers;
using JoJoData.Library;

namespace JoJoBot.Commands.Slash;

public class ItemCommands 
{
	[Command("arrow"), RequireGuild]
	public async Task Arrow(SlashCommandContext ctx)
	{
		try
		{
			Player player = new(ctx.Guild!, ctx.User);
			player.Load();
			Item arrow = player.Inventory["StandArrow"];
			
			if (arrow.Count < 1) 
			{
				await ctx.RespondAsync("You don't have any Stand Arrows!", true);
				return;
			}
			
			await ctx.RespondAsync(player.Inventory["StandArrow"].Use(player));
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}
}