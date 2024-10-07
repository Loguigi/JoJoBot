using DSharpPlus;

namespace JoJoData.Controllers;

public static class JoJo 
{
	public static DiscordClient? Client { get; private set; } = null;
	public static Dictionary<int, BattleController> Battles { get; private set; } = [];
	public static Random RNG { get; private set; } = new();
	
	public static void Create(DiscordClient client) 
	{
		Client = client;
		RNG = new();
		RNG.Next();
	}

	public static double ConvertToPercent(double value) => value * 100;
}