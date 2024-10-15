using DSharpPlus;
using JoJoData.Controllers;
using JoJoBot.Handlers;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Commands.Processors.SlashCommands;
using JoJoData;

namespace JoJoBot;

public class Bot 
{

	public static async Task Main() 
	{
		DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(Config.Token, DiscordIntents.All);
		builder.ConfigureEventHandlers
		(
			b => b.HandleComponentInteractionCreated(BattleHandler.HandleAcceptChallenge)
				.HandleComponentInteractionCreated(BattleHandler.HandleDeclineChallenge)
				.HandleComponentInteractionCreated(BattleHandler.HandleAbilitySelect)
				.HandleComponentInteractionCreated(ItemHandler.HandleArrowStandAccept)
				.HandleComponentInteractionCreated(ItemHandler.HandleArrowStandDecline)
				.HandleComponentInteractionCreated(BattleHandler.HandleAbilityView)
		);
		DiscordClient client = builder.Build();

		CommandsExtension commands = client.UseCommands(new CommandsConfiguration()
		{
			DebugGuildId = 0,
			RegisterDefaultCommandProcessors = true
		});
		commands.AddCommands(typeof(Bot).Assembly);
		var text = new TextCommandProcessor(new()
		{
			PrefixResolver = new DefaultPrefixResolver(true, "--").ResolvePrefixAsync
		});
		var slash = new SlashCommandProcessor();
		await commands.AddProcessorsAsync(text, slash);

		JoJo.Create(client);
		StandLoader.Load();

		await client.ConnectAsync();
		await Task.Delay(-1);
	}
}
