using DSharpPlus;
using JoJoData.Controllers;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext;
using JoJoBot.Commands.Slash;
using JoJoBot.Handlers;
using JoJoBot.Commands.Text;
using Microsoft.Extensions.Logging;

namespace JoJoBot;

public class Bot 
{
	public static Dictionary<int, BattleController> Battles { get; set; } = [];

	public static async Task Main() 
	{

		var client = new DiscordClient(new DiscordConfiguration()
		{
			Intents = DiscordIntents.All,
			TokenType = TokenType.Bot,
			Token = Environment.GetEnvironmentVariable("JOJOTOKEN") ?? throw new Exception("No token set"),
			AutoReconnect = true,
			LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt",
			MinimumLogLevel = LogLevel.Debug,
		});

		var text = client.UseCommandsNext(new CommandsNextConfiguration()
		{
			StringPrefixes = ["--"],
		});
		text.RegisterCommands<AdminCommands>();
		var slash = client.UseSlashCommands();
		slash.RegisterCommands<BattleCommand>();

		client.ComponentInteractionCreated += BattleHandlers.HandleAcceptChallenge;
		client.ComponentInteractionCreated += BattleHandlers.HandleDeclineChallenge;
		client.ComponentInteractionCreated += BattleHandlers.HandleAbilitySelect;

		DiscordController.Create(client);
		StandLoader.Load();

		await client.ConnectAsync();
		await Task.Delay(-1);
	}
}
