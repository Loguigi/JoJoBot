using JoJoData.Library;

namespace JoJoData.Models;

public class InventoryModel 
{
	public long GuildId { get; set; }
	public long PlayerId { get; set; }
	public string ItemId { get; set; } = string.Empty;
	public int Count { get; set; } = 0;
	
	public InventoryModel() { }
	
	public InventoryModel(Player player, Item item) 
	{
		GuildId = (long)player.Guild.Id;
		PlayerId = (long)player.User.Id;
		ItemId = item.GetType().Name;
		Count = player.Inventory[item.GetType().Name].Count;
	}
}