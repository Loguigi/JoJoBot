using DSharpPlus;
using DSharpPlus.Entities;
using JoJoData.Data;
using JoJoData.Helpers;
using JoJoData.Library;
using JoJoData.Abilities;
using Dapper;
using JoJoData.Models;
using System.Data;
using System.Reflection;
using DSharpPlus.EventArgs;

namespace JoJoData.Controllers;

public class BattleController(DiscordClient client, DiscordGuild guild, DiscordChannel channel, DiscordUser player1, DiscordUser player2) : DataAccess
{
	#region Properties
	public int Id { get; private set; } = 0;
	public DateTime BattleStart { get; private set; } = DateTime.Now;
	public DateTime? BattleEnd { get; private set; }
	public int CurrentTurn { get; private set; } = 0;
	public BattlePlayer Player1 { get; private set; } = new(client, guild, player1);
	public BattlePlayer Player2 { get; private set; } = new(client, guild, player2);
	public BattlePlayer CurrentPlayer => Turns[CurrentTurn].CurrentPlayer;
	public BattlePlayer Opponent => Turns[CurrentTurn].Opponent;
	public Player? Winner { get; private set; }
	public Dictionary<int, Turn> Turns { get; private set; } = [];
	public readonly DiscordClient Client = client;
	public readonly DiscordGuild Guild = guild;
	public readonly DiscordChannel Channel = channel;
	#endregion

	#region Public Battle Control Methods
	public async void StartBattle()
	{
		try
		{
			Id = SaveBattleInfo();
			CurrentTurn++;

			Turns.Add(CurrentTurn,
				Player1.Stand!.Speed > Player2.Stand!.Speed ? new Turn(Player1, Player2) : new Turn(Player2, Player1));

			Turns[CurrentTurn].PreCheck();
			await CreateBattleInterface();
			StartNextRound();
		}
		catch (OnDeathException ex)
		{
			DeathFlagEventArgs deathFlag = new(ex.DeadPlayer, Turns[CurrentTurn]);
			Turns[CurrentTurn].OnDeathFlag(deathFlag);
			if (deathFlag.IsDead)
			{
				EndBattle();
			}
			else
			{
				StartNextRound();
			}
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
		finally
		{
			await SendBattleMessages(Turns[CurrentTurn].BattleLog);
		}
	}

	public async void ContinueBattle(Ability ability, ComponentInteractionCreatedEventArgs e) 
	{
		try
		{
			AbilityCastEventArgs abilityCast = new(CurrentPlayer, Turns[CurrentTurn], ability);
			Turns[CurrentTurn].OnAbilityCast(abilityCast);
			if (abilityCast is { IsValidCast: false, OutputMessage: not null })
			{
				await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(abilityCast.OutputMessage).AsEphemeral());
				return;
			}

			await e.Message.DeleteAsync();
			Turns[CurrentTurn].Execute(ability);
			await SendBattleMessages(Turns[CurrentTurn].BattleLog);
		}
		catch (OnDeathException ex)
		{
			PerformAutopsy(ex);
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
		finally
		{
			await SendBattleMessages(Turns[CurrentTurn].BattleLog);
		}
	}
	#endregion
	
	#region Private Battle Methods
	private async void StartNextRound() 
	{
		try
		{
			BattlePlayer nextPlayer;
			BattlePlayer opponent;

			if (Turns[CurrentTurn].RoundRepeat)
			{
				nextPlayer = new BattlePlayer(CurrentPlayer);
				opponent = new BattlePlayer(Opponent);
			}
			else
			{
				nextPlayer = new BattlePlayer(Opponent);
				opponent = new BattlePlayer(CurrentPlayer);
			}

			CurrentTurn++;
			Turns.Add(CurrentTurn, new Turn(nextPlayer, opponent));
			Turns[CurrentTurn].PreCheck();
			await SendBattleMessages(Turns[CurrentTurn].BattleLog);
		}
		catch (TurnSkipException)
		{
			StartNextRound();
		}
		catch (OnDeathException ex)
		{
			PerformAutopsy(ex);
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
		finally
		{
			await SendBattleMessages(Turns[CurrentTurn].BattleLog);	
		}
	}

	private async void EndBattle()
	{
		JoJo.Battles.Remove(Id);
		BattleEnd = DateTime.Now;
		Winner = GetWinner();
		if (Winner == Player1) 
		{
			Player1.GrantExp(opponent: Player2, winner: Player1);
			Player2.GrantExp(opponent: Player1, winner: Player1);
		}
		else if (Winner == Player2) 
		{
			Player1.GrantExp(opponent: Player2, winner: Player2);
			Player2.GrantExp(opponent: Player1, winner: Player2);
		}
		
		SaveBattleInfo();
		
		await Channel.SendMessageAsync(new DiscordEmbedBuilder()
			.WithAuthor(Winner.User.GlobalName, "", Winner.User.AvatarUrl)
			.WithTitle($"🎉 {Winner.Stand!.CoolName} wins! 🎉")
			.WithDescription($"{Player1.User.Mention}: `+{Player1.ExpGain} EXP`\n{Player2.User.Mention}: `+{Player2.ExpGain} EXP`")
			.WithThumbnail(Winner.Stand!.ImageUrl)
			.AddField("Rounds", CurrentTurn.ToString(), true)
			.AddField("Start", BattleStart.ToString("M/d h:mm tt"), true)
			.AddField("End", BattleEnd?.ToString("M/d h:mm tt") ?? "never", true)
			.WithColor(DiscordColor.HotPink));
			
		if (Player1.LevelledUp) 
		{
			await Channel.SendMessageAsync(Player1.LevelUpMessage());
		}
		
		if (Player2.LevelledUp) 
		{
			await Channel.SendMessageAsync(Player2.LevelUpMessage());
		}
	}

	private void PerformAutopsy(OnDeathException ex)
	{
		DeathFlagEventArgs deathFlag = new(ex.DeadPlayer, Turns[CurrentTurn]);
		Turns[CurrentTurn].OnDeathFlag(deathFlag);
		if (deathFlag.IsDead)
		{
			EndBattle();
		}
		else
		{
			StartNextRound();
		}
	}

	private BattlePlayer GetWinner() => !CurrentPlayer.IsAlive ? Opponent : !Opponent.IsAlive ? CurrentPlayer : throw new Exception("Both players are alive");
	#endregion

	#region Discord Output Control
	private async Task CreateBattleInterface() 
	{
		var embed = new DiscordEmbedBuilder()
			.WithAuthor($"{CurrentPlayer.User.GlobalName}'s turn", "", CurrentPlayer.User.AvatarUrl)
			.WithTitle($"{CurrentPlayer.Stand!.CoolName}")
			.WithThumbnail(CurrentPlayer.Stand.ImageUrl)
			.WithColor(DiscordColor.Aquamarine)
			.WithFooter($"Round {CurrentTurn}");

		embed.AddField($"{CurrentPlayer.User.GlobalName} (Level: {CurrentPlayer.Level})", CurrentPlayer.FormatBattleInfo(Client), true);
		embed.AddField($"{Opponent.User.GlobalName} (Level: {Opponent.Level})", Opponent.FormatBattleInfo(Client), true);

		var msg = new DiscordMessageBuilder()
			.WithContent(CurrentPlayer.User.Mention)
			.AddMention(new UserMention(CurrentPlayer.User))
			.AddEmbed(embed)
			.AddComponents(CreateAbilityDropdown(CurrentPlayer));

		await Channel!.SendMessageAsync(msg);
	}

	private async Task SendBattleMessages(List<DiscordMessageBuilder?> msgs) 
	{
		foreach (var msg in msgs.Where(x => x is not null)) 
		{
			await Channel.SendMessageAsync(msg!);
			await Task.Delay(750);
		}
	}

	private DiscordSelectComponent CreateAbilityDropdown(BattlePlayer player) 
	{
		List<DiscordSelectComponentOption> options =
        [
            player.Stand!.Ability0.CreateSelection(Client, 0, player),
			player.Stand.Ability1.CreateSelection(Client, 1, player),
			player.Stand.Ability2.CreateSelection(Client, 2, player),
			player.Stand.Ability3.CreateSelection(Client, 3, player),
			player.Stand.Ability4.CreateSelection(Client, 4, player)
		];

		return new DiscordSelectComponent($@"{IDHelper.Battle.ABILITY_SELECT}\{Id}\{CurrentPlayer.User.Id}", "Select ability...", options);
	}
	#endregion
	
	#region DB Methods
	private int SaveBattleInfo() 
	{
		try 
		{
			DynamicParameters param = new(new BattleModel(this));
			param.Add("@Id", Id, DbType.Int32, ParameterDirection.InputOutput);
			var result = SaveData(StoredProcedures.SAVE_BATTLE_DATA, param);
			if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);

			return param.Get<int>("@Id");
		} 
		catch (Exception ex) 
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}
	#endregion
}