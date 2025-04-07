using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JoJoLibrary;
using JoJoLibrary.Helpers;

namespace JoJoBot.Handlers;

public static class ItemHandler 
{
	public static async Task HandleArrowStandAccept(DiscordClient s, ComponentInteractionCreatedEventArgs e) 
	{
		try
		{
			if (!e.Id.Contains(IDHelper.Inventory.ArrowStandAccept) || e.User.Id != ulong.Parse(IDHelper.GetID(e.Id, PLAYER_ID_INDEX))) 
			{
				await Task.CompletedTask;
				return;
			}

			Player player = new(e.Guild, e.User);
			Stand stand = StandLoader.Stands[int.Parse(IDHelper.GetID(e.Id, STAND_ID_INDEX))];
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
			if (!e.Id.Contains(IDHelper.Inventory.ArrowStandDecline) || e.User.Id != ulong.Parse(IDHelper.GetID(e.Id, PLAYER_ID_INDEX)))
			{
				await Task.CompletedTask;
				return;
			}

			Stand stand = StandLoader.Stands[int.Parse(IDHelper.GetID(e.Id, STAND_ID_INDEX))];

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
	
	public static async Task HandleAbilityView(DiscordClient s, ComponentInteractionCreatedEventArgs e)
	{
		if (!e.Id.Contains(IDHelper.Inventory.StandDetails))
		{
			await Task.CompletedTask;
			return;
		}
		
		Stand stand = StandLoader.Stands[int.Parse(IDHelper.GetID(e.Id, STAND_ID_INDEX))];
		await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(stand.FormatDescription()).AsEphemeral());
	}

	private const int PLAYER_ID_INDEX = 1;
	private const int STAND_ID_INDEX = 2;
}