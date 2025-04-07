namespace JoJoLibrary.Helpers;

public static class IDHelper 
{
	public static string GetID(string id, int index) => id.Split('\\')[index];

	public static class Battle 
	{
		public const string AbilitySelect = "BattleAbilitySelect";
		public const string AbilityView = "BattleAbilityView";
		public const string PlayerChallengeAccept = "BattlePlayerChallengeAccept";
		public const string PlayerChallengeDecline = "BattlePlayerChallengeDecline";
	}
	
	public static class Inventory 
	{
		public const string ArrowStandAccept = "ArrowStandAccept";
		public const string ArrowStandDecline = "ArrowStandDecline";
		public const string StandDetails = "InventoryStandDetails";
	}
}