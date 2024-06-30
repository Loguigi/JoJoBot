using System.Reflection;
using Dapper;
using JoJoData.Abilities;
using JoJoData.Data;
using JoJoData.Library;
using JoJoData.Models;

namespace JoJoData.Controllers;

public static class StandLoader
{
	public readonly static Dictionary<int, Stand> Stands = [];
	public readonly static Dictionary<string, Ability> Abilities = [];
	public static void Load() 
	{
		try 
		{
			var abilities = Assembly.GetExecutingAssembly().GetTypes().Where(x => 
				x.IsClass &&
				x.Namespace != null &&
				x.Namespace.Contains("JoJoData.Abilities")).ToList().Where(y => 
				!y.IsAbstract).ToList();

			var result = new ResultArgs((int)StatusCodes.UNKNOWN, "Unknown");
			
			foreach (var type in abilities) 
			{
				if (Activator.CreateInstance(type) is Ability ability) 
				{
					if (string.IsNullOrEmpty(ability.Name)) 
					{
						ability.Name = ability.GetType().Name;
					}
					Abilities.Add(ability.GetType().Name, ability);

					result = db.SaveData(StoredProcedures.SAVE_ABILITIES, new DynamicParameters(new AbilityModel(ability)));
					if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);
				}
			}

			result = db.GetData<StandModel>(StoredProcedures.GET_STANDS_DATA, new DynamicParameters(), out var stands);
			if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);

			foreach (var stand in stands) 
			{
				Stands.Add(stand.Id, new Stand(
					stand,
					Abilities[stand.Ability1],
					Abilities[stand.Ability2],
					Abilities[stand.Ability3],
					Abilities[stand.Ability4]
				));
			}
		} 
		catch (Exception ex) 
		{
			
		}
	}

	public static bool TryGetStand(int id, out Stand stand) 
	{
		stand = Stands.First().Value;
		if (!Stands.Keys.Where(x => x == id).Any()) 
		{
			return false;
		}

		stand = Stands[id];
		return true;
	}
	private static DataAccess db = new();
}