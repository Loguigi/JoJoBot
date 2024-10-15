using System.ComponentModel;
using System.Reflection.Metadata;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JoJoData.Controllers;
using JoJoData.Helpers;
using JoJoData.Library;

namespace JoJoBot.Handlers;

public static class BattleHandler
{
	public static async Task HandleDeclineChallenge(DiscordClient s, ComponentInteractionCreatedEventArgs e) 
	{
		if (!e.Id.Contains(IDHelper.Battle.PlayerChallengeDecline) || e.User.Id != ulong.Parse(IDHelper.GetID(e.Id, PLAYER2_INDEX))) 
		{
			await Task.CompletedTask;
			return;
		}

		DiscordUser player1 = await s.GetUserAsync(ulong.Parse(IDHelper.GetID(e.Id, PLAYER1_INDEX)));
		DiscordUser player2 = await s.GetUserAsync(ulong.Parse(IDHelper.GetID(e.Id, PLAYER2_INDEX)));

		var embed = new DiscordEmbedBuilder()
			.WithDescription($"{DiscordEmoji.FromName(s, ":x:", false)} {player2.Mention} has declined the battle")
			.WithColor(DiscordColor.Red);

		var msg = new DiscordMessageBuilder()
			.WithContent(player1.Mention)
			.AddMention(new UserMention(player1))
			.AddEmbed(embed);

		await e.Message.DeleteAsync();
		await e.Channel.SendMessageAsync(msg);
	}
	
	public static async Task HandleAcceptChallenge(DiscordClient s, ComponentInteractionCreatedEventArgs e) 
	{
		if (!e.Id.Contains(IDHelper.Battle.PlayerChallengeAccept) || e.User.Id != ulong.Parse(IDHelper.GetID(e.Id, PLAYER2_INDEX)))
		{
			await Task.CompletedTask;
			return;
		}

		DiscordUser player1 = await JoJo.Client!.GetUserAsync(ulong.Parse(IDHelper.GetID(e.Id, PLAYER1_INDEX)));
		DiscordUser player2 = e.User;

		await e.Message.DeleteAsync();
		BattleController battle = new(s, e.Guild, e.Channel, player1, player2);
		battle.StartBattle();
		JoJo.Battles.Add(battle.Id, battle);
	}
	
	public static async Task HandleAbilitySelect(DiscordClient s, ComponentInteractionCreatedEventArgs e) 
	{
		if (!e.Id.Contains(IDHelper.Battle.AbilitySelect)) 
		{
			await Task.CompletedTask;
			return;
		}
		
		if (e.User.Id != ulong.Parse(IDHelper.GetID(e.Id, CURRENT_PLAYER_INDEX))) 
		{
			await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithContent("It's not your turn!")).AsEphemeral());
			return;
		}

		BattleController battle = JoJo.Battles[int.Parse(IDHelper.GetID(e.Id, BATTLE_ID_INDEX))];
		Ability ability = int.Parse(e.Values.First()) switch
		{
			0 => battle.CurrentPlayer.Stand!.Ability0,
			1 => battle.CurrentPlayer.Stand!.Ability1,
			2 => battle.CurrentPlayer.Stand!.Ability2,
			3 => battle.CurrentPlayer.Stand!.Ability3,
			4 => battle.CurrentPlayer.Stand!.Ability4,
			_ => throw new Exception("Major issue")
		};

		battle.ContinueBattle(ability, e);
	}

	public static async Task HandleAbilityView(DiscordClient s, ComponentInteractionCreatedEventArgs e)
	{
		if (!e.Id.Contains(IDHelper.Battle.AbilityView))
		{
			await Task.CompletedTask;
			return;
		}
		
		BattleController battle = JoJo.Battles[int.Parse(IDHelper.GetID(e.Id, BATTLE_ID_INDEX))];
		BattlePlayer player = battle.GetPlayer(ulong.Parse(IDHelper.GetID(e.Id, CURRENT_PLAYER_INDEX)));
		await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(player.Stand!.FormatDescription(player)).AsEphemeral());
	}

	private const int BATTLE_ID_INDEX = 1;
	private const int CURRENT_PLAYER_INDEX = 2;
	private const int PLAYER1_INDEX = 1;
	private const int PLAYER2_INDEX = 2;
}