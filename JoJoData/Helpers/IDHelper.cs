namespace JoJoData.Helpers;

public static class IDHelper 
{
	public static string GetID(string id, int index) => id.Split('\\')[index];

	public static class Battle 
	{
		public const string ABILITY_SELECT = "BattleAbilitySelect";
		public const string PLAYER_CHALLENGE_ACCEPT = "BattlePlayerChallengeAccept";
		public const string PLAYER_CHALLENGE_DECLINE = "BattlePlayerChallengeDecline";
	}
	
	public static class Inventory 
	{
		public const string ARROW_STAND_ACCEPT = "ArrowStandAccept";
		public const string ARROW_STAND_DECLINE = "ArrowStandDecline";
	}
}