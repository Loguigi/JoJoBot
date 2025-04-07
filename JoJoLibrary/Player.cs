using System.Reflection;
using DSharpPlus.Entities;
using JoJoData;
using JoJoLibrary.Data;
using JoJoLibrary.Models;

namespace JoJoLibrary;

public class Player(DiscordGuild guild, DiscordUser user) : DataAccess
{
	#region Properties
	public readonly DiscordGuild Guild = guild;
	public readonly DiscordUser User = user;
	public Stand? Stand { get; protected set; }
	public int Level { get; protected set; }
	public int LevelBefore { get; protected set; } = 0;
	public bool LevelledUp { get; protected set; } = false;
	public int Experience { get; protected set; }
	public int ExpNeededForNextLevel => (int)((Math.Pow(Level + 1, 2) + (Level + 1)) / 2 * 10 - (10 * (Level + 1)));
	public int ExpGain { get; private set; } = 0;
	public int BattlesWon { get; protected set; }
	public int BattlesLost { get; protected set; }
	public Dictionary<string, Item> Inventory { get; protected set; } = [];
	#endregion

	#region Public Methods
	public static bool operator ==(Player p1, Player p2) => p1.User.Id == p2.User.Id && p1.Guild.Id == p2.Guild.Id;
	
	public static bool operator !=(Player p1, Player p2) => p1.User.Id != p2.User.Id || p1.Guild.Id != p2.Guild.Id;
	
	public async void Load() 
	{
		try
		{
			var data = await GetData<PlayerModel>(StoredProcedures.GET_PLAYER_DATA, new { GuildId = (long)Guild.Id, UserId = (long)User.Id });
			
			PlayerModel player = data.First();
			Stand = player.StandId != null ? StandLoader.Stands[player.StandId ?? 0] : null;
			Level = player.Level;
			Experience = player.Experience;
			BattlesWon = player.BattlesWon;
			BattlesLost = player.BattlesLost;

			var items = await GetData<InventoryModel>(StoredProcedures.GET_PLAYER_INVENTORY, new { GuildId = (long)Guild.Id, UserId = (long)User.Id });
			foreach (InventoryModel i in items) 
			{
				var itemType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(x => x.Name == i.ItemId);
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
			int baseExp = 0;
			if (this == winner) 
			{
				baseExp = BattleConstants.BASE_EXP_GAIN;
				BattlesWon++;
			}
			else if (opponent == winner)
			{
				baseExp = BattleConstants.BASE_EXP_GAIN - 4;
				BattlesLost++;
			}
			
			if (Level == opponent.Level)
			{
				ExpGain = baseExp;
			}
			else if (Level < opponent.Level && (opponent.Level - Level) <= 3)
			{
				ExpGain = baseExp + (baseExp / 2);
			}
			else if (Level < opponent.Level && (opponent.Level - Level) > 3)
			{
				ExpGain = baseExp * 2;
			}
			else if (Level > opponent.Level && (Level - opponent.Level) <= 3)
			{
				ExpGain = baseExp - (baseExp / 2);
			}
			else if (Level > opponent.Level && (Level - opponent.Level) > 3)
			{
				ExpGain = baseExp / 2;
			}

			Experience += ExpGain;
			int newLevel = (int)(Math.Sqrt(10 * ((Experience * 2) + 2.5)) + 5) / 10;
			if (newLevel > Level) 
			{
				LevelBefore = Level;
				Level = newLevel;
				LevelledUp = true;
			}

			Save();
		}
		catch (Exception ex)
		{
			ex.Source = MethodBase.GetCurrentMethod()!.Name + "(): " + ex.Source;
			throw;
		}
	}
	
	public async void UseItem(Item item) 
	{
		item.Count--;
		
		try
		{
			await SaveData(StoredProcedures.INVENTORY_SAVE, new InventoryModel(this, item));
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

	public int GetMaxHp(int level) => Stand!.BaseHp + (int)(Stand.BaseHp * 0.2) * (level - 1);
	public int GetMinDmg(int level) => Stand!.BaseMinDamage + (int)(Stand.BaseMinDamage * 0.15) * (level - 1);
	public int GetMaxDmg(int level) => Stand!.BaseMaxDamage + (int)(Stand.BaseMaxDamage * 0.18) * (level - 1);
	
	public DiscordMessageBuilder LevelUpMessage() 
	{
		var embed = new DiscordEmbedBuilder()
			.WithTitle(Stand!.CoolName)
			.WithThumbnail(Stand!.ImageUrl)
			.WithDescription($"### ⬆️ {User.Mention} levelled up!")
			.AddField("✨ Level ✨", $"{LevelBefore} ➡️ {Level}", true)
			.AddField("❤️ HP ❤️", $"{GetMaxHp(LevelBefore)} ➡️ {GetMaxHp(Level)}", true)
			.AddField("⚔️ Damage ⚔️", $"{GetMinDmg(LevelBefore)}-{GetMaxDmg(LevelBefore)} ➡️ {GetMinDmg(Level)}-{GetMaxDmg(Level)}", true)
			.WithColor(DiscordColor.Aquamarine);

		return new DiscordMessageBuilder()
			.WithContent(User.Mention)
			.WithAllowedMention(new UserMention(User))
			.AddEmbed(embed);
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj is null)
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
	private async void Save() => await SaveData(StoredProcedures.SAVE_PLAYER_DATA, new PlayerModel());
	#endregion
}