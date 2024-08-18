using System.Security.Cryptography;
using System.Text;
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

namespace JoJoData.Controllers;

public class BattleController(DiscordClient client, DiscordGuild guild, DiscordChannel channel, DiscordUser player1, DiscordUser player2) : DataAccess
{
	#region Properties
	public int Id { get; private set; } = 0;
	public DateTime BattleStart { get; private set; } = DateTime.Now;
	public DateTime? BattleEnd { get; private set; }
	public int CurrentRound { get; private set; } = 0;
	public BattlePlayer Player1 { get; private set; } = new(client, guild, player1);
	public BattlePlayer Player2 { get; private set; } = new(client, guild, player2);
	public BattlePlayer CurrentPlayer => Rounds[CurrentRound].CurrentPlayer;
	public BattlePlayer Opponent => Rounds[CurrentRound].Opponent;
	public Player? Winner { get; private set; }
	public Dictionary<int, Round> Rounds { get; private set; } = [];
	public readonly DiscordClient Client = client;
	public readonly DiscordGuild Guild = guild;
	public readonly DiscordChannel Channel = channel;
	#endregion

	#region Battle Control Methods
	public async void StartBattle() 
	{
		try
		{
			Id = SaveBattleInfo();
			CurrentRound++;

			if (Player1.Stand!.Speed > Player2.Stand!.Speed)
				Rounds.Add(CurrentRound, new Round(Player1, Player2));
			else
				Rounds.Add(CurrentRound, new Round(Player2, Player1));

			Rounds[CurrentRound].PreCheck(out var msgs);
			await SendBattleMessages(msgs);
			await CreateBattleInterface();
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}

	public async void ContinueBattle(Ability ability) 
	{
		try
		{
			Rounds[CurrentRound].Execute(ability, out var msgs);

			await SendBattleMessages(msgs);

			if (!CurrentPlayer.IsAlive)
			{
				EndBattle(winner: Opponent);
			}
			else if (!Opponent.IsAlive)
			{
				EndBattle(winner: CurrentPlayer);
			}
			else
			{
				// not dead
				StartNextRound();
			}
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}

	public async void StartNextRound() 
	{
		BattlePlayer nextPlayer;
		BattlePlayer opponent;
		
		if (Rounds[CurrentRound].RoundRepeat) 
		{
			nextPlayer = new BattlePlayer(CurrentPlayer);
			opponent = new BattlePlayer(Opponent);
		} 
		else
		{
			nextPlayer = new BattlePlayer(Opponent);
			opponent = new BattlePlayer(CurrentPlayer);
		}

		CurrentRound++;
		Rounds.Add(CurrentRound, new Round(nextPlayer, opponent));

		Rounds[CurrentRound].PreCheck(out var msgs);
		await SendBattleMessages(msgs);

		if (!CurrentPlayer.IsAlive)
		{
			EndBattle(winner: Opponent);
		}
		else if (!Opponent.IsAlive)
		{
			EndBattle(winner: CurrentPlayer);
		}
		else
		{
			// not dead
			if (Rounds[CurrentRound].RoundSkipped)
				StartNextRound();
			else
				await CreateBattleInterface();
		}
	}

	public async void EndBattle(BattlePlayer winner)
	{
		DiscordController.Battles.Remove(Id);
		BattleEnd = DateTime.Now;
		Winner = winner;
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
			.AddField("Rounds", CurrentRound.ToString(), true)
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
	#endregion

	#region Discord Output Control
	public async Task CreateBattleInterface() 
	{
		var embed = new DiscordEmbedBuilder()
			.WithAuthor($"{CurrentPlayer.User.GlobalName}'s turn", "", CurrentPlayer.User.AvatarUrl)
			.WithTitle($"{CurrentPlayer.Stand!.CoolName}")
			.WithThumbnail(CurrentPlayer.Stand.ImageUrl)
			.WithColor(DiscordColor.Aquamarine)
			.WithFooter($"Round {CurrentRound}");

		embed.AddField($"{CurrentPlayer.User.GlobalName} (Level: {CurrentPlayer.Level})", CurrentPlayer.FormatBattleInfo(Client), true);
		embed.AddField($"{Opponent.User.GlobalName} (Level: {Opponent.Level})", Opponent.FormatBattleInfo(Client), true);

		var msg = new DiscordMessageBuilder()
			.WithContent(CurrentPlayer.User.Mention)
			.AddMention(new UserMention(CurrentPlayer.User))
			.AddEmbed(embed)
			.AddComponents(CreateAbilityDropdown(CurrentPlayer));

		await Channel!.SendMessageAsync(msg);
	}
	
	public async Task SendBattleMessages(List<DiscordMessageBuilder?> msgs) 
	{
		foreach (var msg in msgs.Where(x => x is not null)) 
		{
			await Channel.SendMessageAsync(msg!);
			await Task.Delay(1000);
		}
	}

	public DiscordSelectComponent CreateAbilityDropdown(BattlePlayer player) 
	{
		var options = new List<DiscordSelectComponentOption>() 
		{
			player.Stand!.Ability0.CreateSelection(Client, 0, player),
			player.Stand.Ability1.CreateSelection(Client, 1, player),
			player.Stand.Ability2.CreateSelection(Client, 2, player),
			player.Stand.Ability3.CreateSelection(Client, 3, player),
			player.Stand.Ability4.CreateSelection(Client, 4, player)
		};

		return new DiscordSelectComponent($"{IDHelper.Battle.ABILITY_SELECT}\\{Id}\\{CurrentPlayer.User.Id}", "Select ability...", options);
	}
	#endregion
	
	#region DB Methods
	private int SaveBattleInfo() 
	{
		try 
		{
			var param = new DynamicParameters(new BattleModel(this));
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