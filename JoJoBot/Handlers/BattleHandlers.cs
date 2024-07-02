using System.ComponentModel;
using System.Reflection.Metadata;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using JoJoData.Controllers;
using JoJoData.Helpers;

namespace JoJoBot.Handlers;

public static class BattleHandlers
{
	public static async Task HandleDeclineChallenge(DiscordClient s, ComponentInteractionCreatedEventArgs e) 
	{
		if (!e.Id.Contains(IDHelper.Battle.PLAYER_CHALLENGE_DECLINE) || e.User.Id != ulong.Parse(IDHelper.GetID(e.Id, PLAYER2_INDEX))) 
		{
			await Task.CompletedTask;
			return;
		}

		var player1 = await s.GetUserAsync(ulong.Parse(IDHelper.GetID(e.Id, PLAYER1_INDEX)));
		var player2 = await s.GetUserAsync(ulong.Parse(IDHelper.GetID(e.Id, PLAYER2_INDEX)));

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
		if (!e.Id.Contains(IDHelper.Battle.PLAYER_CHALLENGE_ACCEPT) || e.User.Id != ulong.Parse(IDHelper.GetID(e.Id, PLAYER2_INDEX)))
		{
			await Task.CompletedTask;
			return;
		}

		var player1 = await DiscordController.Client!.GetUserAsync(ulong.Parse(IDHelper.GetID(e.Id, PLAYER1_INDEX)));
		var player2 = e.User;

		await e.Message.DeleteAsync();
		var battle = new BattleController(s, e.Guild, e.Channel, player1, player2);
		battle.StartBattle();
		Bot.Battles.Add(battle.Id, battle);

		if (battle.Winner is not null)
		{
			Bot.Battles.Remove(battle.Id);
			await e.Channel.SendMessageAsync(new DiscordEmbedBuilder()
				.WithAuthor(battle.Winner.User.GlobalName, "", battle.Winner.User.AvatarUrl)
				.WithDescription($"### 🎉 {battle.Winner.Stand!.CoolName} wins! 🎉")
				.WithThumbnail(battle.Winner.Stand!.ImageUrl)
				.AddField("Rounds", battle.CurrentRound.ToString(), true)
				.AddField("Start", battle.BattleStart.ToString("M/d h:m tt"), true)
				.AddField("End", battle.BattleEnd?.ToString("M/d h:m tt") ?? "never", true)
				.WithColor(DiscordColor.HotPink));
		}
	}
	
	public static async Task HandleAbilitySelect(DiscordClient s, ComponentInteractionCreatedEventArgs e) 
	{
		if (!e.Id.Contains(IDHelper.Battle.ABILITY_SELECT)) 
		{
			await Task.CompletedTask;
			return;
		}
		
		if (e.User.Id != ulong.Parse(IDHelper.GetID(e.Id, CURRENT_PLAYER_INDEX))) 
		{
			await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(new DiscordMessageBuilder().WithContent("It's not your turn!")).AsEphemeral());
			return;
		}

		var battle = Bot.Battles[int.Parse(IDHelper.GetID(e.Id, BATTLE_ID_INDEX))];
		var ability = int.Parse(e.Values.First()) switch
		{
			0 => battle.CurrentPlayer.Stand!.Ability0,
			1 => battle.CurrentPlayer.Stand!.Ability1,
			2 => battle.CurrentPlayer.Stand!.Ability2,
			3 => battle.CurrentPlayer.Stand!.Ability3,
			4 => battle.CurrentPlayer.Stand!.Ability4,
			_ => throw new Exception("Major issue")
		};
		
		if (!battle.Rounds[battle.CurrentRound].AbilitySelectCheck(ability, DiscordController.Client!, out var msg)) 
		{
			await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(msg).AsEphemeral());
			return;
		}

		await e.Message.DeleteAsync();
		battle.ContinueBattle(ability);
		
		if (battle.Winner is not null) 
		{
			Bot.Battles.Remove(battle.Id);
			await e.Channel.SendMessageAsync(new DiscordEmbedBuilder()
				.WithAuthor(battle.Winner.User.GlobalName, "", battle.Winner.User.AvatarUrl)
				.WithDescription($"### 🎉 {battle.Winner.Stand!.CoolName} wins! 🎉")
				.WithThumbnail(battle.Winner.Stand!.ImageUrl)
				.AddField("Rounds", battle.CurrentRound.ToString(), true)
				.AddField("Start", battle.BattleStart.ToString("M/d h:m tt"), true)
				.AddField("End", battle.BattleEnd?.ToString("M/d h:m tt") ?? "never", true)
				.WithColor(DiscordColor.HotPink));
		}
	}

	private const int BATTLE_ID_INDEX = 1;
	private const int CURRENT_PLAYER_INDEX = 2;
	private const int PLAYER1_INDEX = 1;
	private const int PLAYER2_INDEX = 2;
}