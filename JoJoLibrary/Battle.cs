using DSharpPlus;
using DSharpPlus.Entities;
using System.Data;
using System.Reflection;
using Dapper;
using DSharpPlus.EventArgs;
using JoJoData;
using JoJoData.Models;
using JoJoLibrary.Helpers;

namespace JoJoLibrary;

public class Battle(DiscordClient client, DiscordGuild guild, DiscordChannel channel, DiscordUser player1, DiscordUser player2) : DataAccess
{
	#region Properties
	public int Id { get; private set; }
	public DateTime BattleStart { get; } = DateTime.Now;
	public DateTime? BattleEnd { get; private set; }
	public int CurrentTurn { get; private set; }
	public BattlePlayer Player1 { get; } = new(client, guild, player1);
	public BattlePlayer Player2 { get; } = new(client, guild, player2);
	public BattlePlayer CurrentPlayer => _turns[CurrentTurn].CurrentPlayer;
	public BattlePlayer Opponent => _turns[CurrentTurn].Opponent;
	public readonly DiscordGuild Guild = guild;

	#endregion

	#region Public Battle Control Methods
	public BattlePlayer GetPlayer(ulong userid) => CurrentPlayer.User.Id == userid ? CurrentPlayer : Opponent;
	
	public async void StartBattle()
	{
		try
		{
			Id = await SaveBattleInfo();
			CurrentTurn++;

			_turns.Add(CurrentTurn,
				Player1.Stand!.Speed > Player2.Stand!.Speed ? new Turn(Player1, Player2) : new Turn(Player2, Player1));

			_turns[CurrentTurn].PreCheck();
			await SendBattleMessages(_turns[CurrentTurn].BattleLog);
			await CreateBattleInterface();
		}
		catch (OnDeathException ex)
		{
			await SendBattleMessages(_turns[CurrentTurn].BattleLog);
			DeathFlagEventArgs deathFlag = new(ex.DeadPlayer, _turns[CurrentTurn]);
			_turns[CurrentTurn].OnDeathFlag(deathFlag);
			if (deathFlag.IsDead)
			{
				EndBattle();
			}
			else
			{
				await CreateBattleInterface();
			}
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}

	public async void ContinueBattle(Ability ability, ComponentInteractionCreatedEventArgs e) 
	{
		try
		{
			AbilityCastEventArgs abilityCast = new(CurrentPlayer, _turns[CurrentTurn], ability);
			_turns[CurrentTurn].OnAbilityCast(abilityCast);
			await SendBattleMessages(_turns[CurrentTurn].BattleLog);
			if (abilityCast is { IsValidCast: false, OutputMessage: not null })
			{
				await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder(abilityCast.OutputMessage).AsEphemeral());
				return;
			}

			await e.Message.DeleteAsync();
			_turns[CurrentTurn].Execute(ability);
			await SendBattleMessages(_turns[CurrentTurn].BattleLog);
			StartNextRound();
		}
		catch (OnDeathException ex)
		{
			await SendBattleMessages(_turns[CurrentTurn].BattleLog);
			PerformAutopsy(ex);
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
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

			if (_turns[CurrentTurn].RoundRepeat)
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
			_turns.Add(CurrentTurn, new Turn(nextPlayer, opponent));
			_turns[CurrentTurn].PreCheck();
			await SendBattleMessages(_turns[CurrentTurn].BattleLog);
			await CreateBattleInterface();
		}
		catch (TurnSkipException)
		{
			await SendBattleMessages(_turns[CurrentTurn].BattleLog);	
			StartNextRound();
		}
		catch (OnDeathException ex)
		{
			await SendBattleMessages(_turns[CurrentTurn].BattleLog);	
			PerformAutopsy(ex);
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}

	private async void EndBattle()
	{
		JoJo.Battles.Remove(Id);
		BattleEnd = DateTime.Now;
		_winner = GetWinner();
		if (_winner == Player1) 
		{
			Player1.GrantExp(opponent: Player2, winner: Player1);
			Player2.GrantExp(opponent: Player1, winner: Player1);
		}
		else if (_winner == Player2) 
		{
			Player1.GrantExp(opponent: Player2, winner: Player2);
			Player2.GrantExp(opponent: Player1, winner: Player2);
		}
		
		SaveBattleInfo();
		
		await channel.SendMessageAsync(new DiscordEmbedBuilder()
			.WithAuthor(_winner.User.GlobalName, "", _winner.User.AvatarUrl)
			.WithTitle($"🎉 {_winner.Stand!.CoolName} wins! 🎉")
			.WithDescription($"{Player1.User.Mention}: `+{Player1.ExpGain} EXP`\n{Player2.User.Mention}: `+{Player2.ExpGain} EXP`")
			.WithThumbnail(_winner.Stand!.ImageUrl)
			.AddField("Rounds", CurrentTurn.ToString(), true)
			.AddField("Start", BattleStart.ToString("M/d h:mm tt"), true)
			.AddField("End", BattleEnd?.ToString("M/d h:mm tt") ?? "never", true)
			.WithColor(DiscordColor.HotPink));
			
		if (Player1.LevelledUp) 
		{
			await channel.SendMessageAsync(Player1.LevelUpMessage());
		}
		
		if (Player2.LevelledUp) 
		{
			await channel.SendMessageAsync(Player2.LevelUpMessage());
		}
	}

	private void PerformAutopsy(OnDeathException ex)
	{
		DeathFlagEventArgs deathFlag = new(ex.DeadPlayer, _turns[CurrentTurn]);
		_turns[CurrentTurn].OnDeathFlag(deathFlag);
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
			.WithFooter($"Turn {CurrentTurn}");

		embed.AddField($"{CurrentPlayer.User.GlobalName} (Level: {CurrentPlayer.Level})", CurrentPlayer.FormatBattleInfo(), true);
		embed.AddField($"{Opponent.User.GlobalName} (Level: {Opponent.Level})", Opponent.FormatBattleInfo(), true);
		DiscordButtonComponent details = new(DiscordButtonStyle.Success, $@"{IDHelper.Battle.AbilityView}\{Id}\{CurrentPlayer.User.Id}", $"{CurrentPlayer.Stand.CoolName} details");
		DiscordButtonComponent enemyDetails = new(DiscordButtonStyle.Danger, $@"{IDHelper.Battle.AbilityView}\{Id}\{Opponent.User.Id}", $"{Opponent.Stand!.CoolName} details");

		var msg = new DiscordMessageBuilder()
			.WithContent(CurrentPlayer.User.Mention)
			.AddMention(new UserMention(CurrentPlayer.User))
			.AddEmbed(embed)
			.AddComponents(details, enemyDetails)
			.AddComponents(CreateAbilityDropdown(CurrentPlayer));
		
		await channel.SendMessageAsync(msg);
	}

	private async Task SendBattleMessages(List<DiscordMessageBuilder?> msgs) 
	{
		foreach (var msg in msgs.Where(x => x is not null)) 
		{
			await channel.SendMessageAsync(msg!);
			await Task.Delay(750);
		}
	}

	private DiscordSelectComponent CreateAbilityDropdown(BattlePlayer player) 
	{
		List<DiscordSelectComponentOption> options =
        [
            player.Stand!.Ability0.CreateSelection(client, 0, player),
			player.Stand.Ability1.CreateSelection(client, 1, player),
			player.Stand.Ability2.CreateSelection(client, 2, player),
			player.Stand.Ability3.CreateSelection(client, 3, player),
			player.Stand.Ability4.CreateSelection(client, 4, player)
		];

		return new DiscordSelectComponent($@"{IDHelper.Battle.AbilitySelect}\{Id}\{CurrentPlayer.User.Id}", "Select ability...", options);
	}
	#endregion
	
	#region DB Methods
	private async Task<int> SaveBattleInfo()
	{
		try 
		{
			DynamicParameters param = new(new BattleModel(Guild.Id, player1.Id, player2.Id, CurrentTurn, BattleStart, BattleEnd));
			param.Add("@Id", Id, DbType.Int32, ParameterDirection.InputOutput);
			await SaveData(StoredProcedures.SAVE_BATTLE_DATA, param);

			return param.Get<int>("@Id");
		} 
		catch (Exception ex) 
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}
	#endregion
	
	#region Private Members
	private readonly Dictionary<int, Turn> _turns = [];
	private Player? _winner;
	#endregion
}