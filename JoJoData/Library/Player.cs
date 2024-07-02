using System.Data;
using System.Reflection;
using Dapper;
using DSharpPlus;
using DSharpPlus.Entities;
using JoJoData.Controllers;
using JoJoData.Data;
using JoJoData.Models;

namespace JoJoData.Library;

public class Player : DataAccess
{
	#region Properties
	public readonly DiscordGuild Guild;
	public DiscordUser User { get; private set; }
	public Stand? Stand { get; set; }
	public int Level { get; private set; }
	public int Experience { get; set; }
	public int BattlesWon { get; private set; }
	#endregion

	#region Public Methods
	public Player(DiscordGuild guild, DiscordUser user)
	{
		Guild = guild;
		User = user;

		try
		{
			var param = new DynamicParameters();
			param.Add("@GuildId", (long)Guild.Id, DbType.Int64, ParameterDirection.Input);
			param.Add("@PlayerId", (long)User.Id, DbType.Int64, ParameterDirection.Input);
			var result = GetData<PlayerModel>(StoredProcedures.GET_PLAYER_DATA, param, out var data);
			if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);
			if (data.Count != 0) 
			{
				var player = data.First();
				Stand = StandLoader.Stands[player.StandId];
			}
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}
	
	public void Save() 
	{
		try
		{
			var result = SaveData(StoredProcedures.SAVE_PLAYER_DATA, new DynamicParameters(new PlayerModel(this)));
			if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}

	public void GrantExp(int exp)
	{
		try
		{
			
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}
	#endregion
}