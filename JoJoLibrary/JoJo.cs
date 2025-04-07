using DSharpPlus;

namespace JoJoLibrary;

public static class JoJo 
{
	public static DiscordClient? Client { get; private set; } = null;
	public static Dictionary<int, Battle> Battles { get; private set; } = [];
	public static System.Random RNG { get; private set; } = new();
	public static Timer Timer { get; private set; }
	
	public static void Create(DiscordClient client) 
	{
		Client = client;
		RNG = new System.Random();
		RNG.Next();
	}

	public static double ConvertToPercent(double value) => value * 100;
	
	/// <summary>
	/// Converts double value from calculation to int
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static int Calculate(double value) => (int)Math.Ceiling(value);
}