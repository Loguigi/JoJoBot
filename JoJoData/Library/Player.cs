using System.Data;
using System.Reflection;
using Dapper;
using DSharpPlus;
using DSharpPlus.Entities;
using JoJoData.Controllers;
using JoJoData.Data;
using JoJoData.Models;

namespace JoJoData.Library;

public class Player(DiscordGuild guild, DiscordUser user) : DataAccess
{
	#region Properties
	public readonly DiscordGuild Guild = guild;
	public readonly DiscordUser User = user;
	public Stand? Stand { get; protected set; }
	public int Level { get; protected set; }
	public int Experience { get; protected set; }
	public int BattlesWon { get; protected set; }
	public int BattlesLost { get; protected set; }
	public Dictionary<string, Item> Inventory { get; protected set; } = [];
	#endregion

	#region Public Methods
	public static bool operator ==(Player p1, Player p2) => p1.User.Id == p2.User.Id && p1.Guild.Id == p2.Guild.Id;
	
	public static bool operator !=(Player p1, Player p2) => p1.User.Id != p2.User.Id || p1.Guild.Id != p2.Guild.Id;
	
	public void Load() 
	{
		try
		{
			var param = new DynamicParameters();
			param.Add("@GuildId", (long)Guild.Id, DbType.Int64, ParameterDirection.Input);
			param.Add("@PlayerId", (long)User.Id, DbType.Int64, ParameterDirection.Input);
			var result = GetData<PlayerModel>(StoredProcedures.GET_PLAYER_DATA, param, out var data);
			if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);

			var player = data.First();
			Stand = player.StandId != null ? StandLoader.Stands[player.StandId ?? 0] : null;
			Level = player.Level;
			Experience = player.Experience;
			BattlesWon = player.BattlesWon;
			BattlesLost = player.BattlesLost;

			param = new DynamicParameters();
			param.Add("@GuildId", (long)Guild.Id, DbType.Int64, ParameterDirection.Input);
			param.Add("@PlayerId", (long)User.Id, DbType.Int64, ParameterDirection.Input);
			result = GetData<InventoryModel>(StoredProcedures.GET_PLAYER_INVENTORY, param, out var items);
			foreach (var i in items) 
			{
				var itemType = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.Name == i.ItemId).FirstOrDefault();
				if (itemType is not null && Activator.CreateInstance(itemType, i.Count) is Item item) 
				{
					Inventory.Add(i.ItemId, item);
				}
			}
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}

	public void GrantExp(Player opponent, Player winner)
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
	
	public void UseItem(Item item) 
	{
		item.Count--;
		try
		{
			var result = SaveData(StoredProcedures.INVENTORY_SAVE, new DynamicParameters(new InventoryModel(this, item)));
			if (result.Status != StatusCodes.SUCCESS) throw new Exception(result.Message);
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}

	public void ChangeStand(Stand stand) 
	{
		try
		{
			Stand = stand;
			Save();
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (ReferenceEquals(obj, null))
		{
			return false;
		}

		throw new NotImplementedException();
	}

	public override int GetHashCode()
	{
		throw new NotImplementedException();
	}
	#endregion

	#region Private Methods
	private void Save()
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
	#endregion
}