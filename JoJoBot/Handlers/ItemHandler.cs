using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JoJoData.Controllers;
using JoJoData.Helpers;
using JoJoData.Library;

namespace JoJoBot.Handlers;

public static class ItemHandler 
{
	public static async Task HandleArrowStandAccept(DiscordClient s, ComponentInteractionCreatedEventArgs e) 
	{
		try
		{
			if (!e.Id.Contains(IDHelper.Inventory.ARROW_STAND_ACCEPT) || e.User.Id != ulong.Parse(IDHelper.GetID(e.Id, PLAYER_ID_INDEX))) 
			{
				await Task.CompletedTask;
				return;
			}

			var player = new Player(e.Guild, e.User);
			var stand = StandLoader.Stands[int.Parse(IDHelper.GetID(e.Id, STAND_ID_INDEX))];
			player.Load();
			player.ChangeStand(stand);

			var msg = new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(e.User.GlobalName, "", e.User.AvatarUrl)
				.WithDescription($"### ✅ Accepted {stand.CoolName}")
				.WithColor(DiscordColor.SpringGreen)
				.WithThumbnail(stand.ImageUrl));

			await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(msg));
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}

	public static async Task HandleArrowStandDecline(DiscordClient s, ComponentInteractionCreatedEventArgs e)
	{
		try
		{
			if (!e.Id.Contains(IDHelper.Inventory.ARROW_STAND_DECLINE) || e.User.Id != ulong.Parse(IDHelper.GetID(e.Id, PLAYER_ID_INDEX)))
			{
				await Task.CompletedTask;
				return;
			}

			var stand = StandLoader.Stands[int.Parse(IDHelper.GetID(e.Id, STAND_ID_INDEX))];

			var msg = new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(e.User.GlobalName, "", e.User.AvatarUrl)
				.WithDescription($"### ❌ Declined {stand.CoolName}")
				.WithColor(DiscordColor.Red)
				.WithThumbnail(stand.ImageUrl));

			await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder(msg));
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}

	private const int PLAYER_ID_INDEX = 1;
	private const int STAND_ID_INDEX = 2;
}