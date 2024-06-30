using DSharpPlus;

namespace JoJoData.Controllers;

public static class DiscordController 
{
	public static DiscordClient? Client { get; private set; } = null;
	
	public static void Create(DiscordClient client) 
	{
		Client = client;
	}
}