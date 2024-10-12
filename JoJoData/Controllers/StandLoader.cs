using System.Reflection;
using Dapper;
using JoJoData.Data;
using JoJoData.Library;
using JoJoData.Models;

namespace JoJoData.Controllers;

public static class StandLoader
{
	public static readonly Dictionary<int, Stand> Stands = [];
	private static readonly Dictionary<string, Ability> Abilities = [];
	private static readonly List<Passive> Passives = [];
	
	public static void Load() 
	{
		try 
		{
			List<Type> abilities = Assembly.GetExecutingAssembly().GetTypes().Where(x => 
				x.IsClass &&
				x.Namespace != null &&
				x.Namespace.Contains("JoJoData.Abilities")).ToList().Where(y => 
				!y.IsAbstract).ToList();
			
			List<Type> passives = Assembly.GetExecutingAssembly().GetTypes().Where(x =>
				x is { IsClass: true, Namespace: not null } &&
				x.Namespace.Contains("JoJoData.Library")).ToList().Where(y =>
				!y.IsAbstract && (y.BaseType == typeof(Passive) || y.BaseType == typeof(OnDeathPassive))).ToList();

			ResultArgs result;
			
			foreach (Type type in abilities)
			{
				if (Activator.CreateInstance(type) is not Ability ability) continue;
				if (string.IsNullOrEmpty(ability.Name)) 
				{
					ability.Name = ability.GetType().Name;
				}
				Abilities.Add(ability.GetType().Name, ability);

				result = Db.SaveData(StoredProcedures.SAVE_ABILITIES, new DynamicParameters(new AbilityModel(ability)));
				if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);
			}

			foreach (var type in passives)
			{
				if (Activator.CreateInstance(type) is not Passive passive) continue;
				Passives.Add(passive);
			}

			result = Db.GetData<StandModel>(StoredProcedures.GET_STANDS_DATA, new DynamicParameters(), out var stands);
			if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);

			foreach (StandModel stand in stands) 
			{
				
				Stands.Add(stand.Id, new Stand(
					stand,
					string.IsNullOrEmpty(stand.Passive) ? null : Passives.First(x => x.GetType().Name == stand.Passive),
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
	
	private static readonly DataAccess Db = new();
}