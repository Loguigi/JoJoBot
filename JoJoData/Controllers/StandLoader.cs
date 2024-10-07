using System.Reflection;
using Dapper;
using JoJoData.Abilities;
using JoJoData.Data;
using JoJoData.Library;
using JoJoData.Models;

namespace JoJoData.Controllers;

public static class StandLoader
{
	public static readonly Dictionary<int, Stand> Stands = [];
	private static readonly Dictionary<string, Ability> Abilities = [];
	public static void Load() 
	{
		try 
		{
			List<Type> abilities = Assembly.GetExecutingAssembly().GetTypes().Where(x => 
				x.IsClass &&
				x.Namespace != null &&
				x.Namespace.Contains("JoJoData.Abilities")).ToList().Where(y => 
				!y.IsAbstract).ToList();

			ResultArgs result = new((int)StatusCodes.UNKNOWN, "Unknown");
			
			foreach (Type type in abilities) 
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

			foreach (StandModel stand in stands) 
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
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
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
	private static readonly DataAccess db = new();
}