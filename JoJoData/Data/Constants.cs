namespace JoJoData.Data;

public static class BattleConstants 
{
	public static readonly int BASE_MP = 150;
	public static readonly int LOW_MP_THRESHOLD = 50;
	public static readonly int LOW_MP_GAIN = 15;
	public static readonly double BASE_CRIT_CHANCE = 0.1;
	public static readonly double BASE_CRIT_DMG_MULT = 2;
	public static readonly double EXP_CONSTANT = 0.04;
}

public static class StoredProcedures 
{
	public static readonly string GET_PLAYER_DATA = "Players_GetData";
	public static readonly string SAVE_PLAYER_DATA = "Players_SaveData";
	public static readonly string GET_STANDS_DATA = "Stands_GetData";
	public static readonly string SAVE_BATTLE_DATA = "Battles_SaveData";
	public static readonly string SAVE_ABILITIES = "Abilities_SaveData";
}