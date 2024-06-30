using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using JoJoData.Data;

namespace JoJoData.Library;

public class BattlePlayer : Player 
{
	#region Properties
	public readonly DiscordClient Client;
	public bool IsAlive { get; set; } = true;
	
	#region Base Stat Properties
	public int Hp 
	{
		get 
		{
			if (Barrier > 0) 
			{
				return Barrier + _hp;
			}

			return _hp;
		}
		private set 
		{
			_hp = value;
		}
	}
	public int MaxHp { get; private set; }
	public int Barrier { get; private set; } = 0;
	public int Mp { get; private set; } = BattleConstants.BASE_MP;
	public int MinDamage { get; set; }
	public int MaxDamage { get; set; }
	public double CritChance { get; private set; } = BattleConstants.BASE_CRIT_CHANCE;
	public double CritDamageMultiplier { get; set; } = BattleConstants.BASE_CRIT_DMG_MULT;
	#endregion
	
	#region Status Properties
	public Status? Status { get; private set; } = null;
	public int StatusDuration { get; set; } = 0;
	public int DamageOverTime { get; set; } = 0;
	#endregion
	
	#region Buff Properties
	public Buff? Buff { get; private set; } = null;
	public int BuffDuration { get; set; } = 0;
	public double DamageResistance { get; set; } = 0;
	#endregion
	public int DamageReceived { get; set; } = 0;
	#endregion

	#region Constructors
	// base constructor
	public BattlePlayer(DiscordClient client, DiscordGuild guild, DiscordUser user) : base(guild, user)
	{
		Client = client;
		Hp = Stand!.BaseHp + ((int)(Stand.BaseHp * 0.30) * Level);
		MaxHp = Hp;
		MinDamage = Stand.BaseMinDamage + ((int)(Stand.BaseMinDamage * 0.15) * Level);
		MaxDamage = Stand.BaseMaxDamage + ((int)(Stand.BaseMaxDamage * 0.20) * Level);
	}

	// copy constructor
	public BattlePlayer(BattlePlayer player) : base(player.Guild, player.User)
	{
		Client = player.Client;
		Hp = player.Hp;
		MaxHp = player.MaxHp;
		Barrier = player.Barrier;
		Mp = player.Mp;
		MinDamage = Stand!.BaseMinDamage + ((int)(Stand.BaseMinDamage * 0.15) * Level);
		MaxDamage = Stand.BaseMaxDamage + ((int)(Stand.BaseMaxDamage * 0.20) * Level);
		CritChance = player.CritChance;
		CritDamageMultiplier = player.CritDamageMultiplier;
		Status = player.Status;
		StatusDuration = player.StatusDuration;
		Buff = player.Buff;
		BuffDuration = player.BuffDuration;
	}
	#endregion

	#region Player Control Methods

	public void ReceiveDamage(int damage) 
	{
		if (Barrier > 0 && damage < Barrier) 
		{
			Barrier -= damage;
		}
		else if (Barrier > 0 && damage > Barrier) 
		{
			damage -= Barrier;
			Barrier = 0;
			Hp -= damage;
		}
		else 
		{
			Hp -= damage;
		}
		
		if (Hp <= 0) 
		{
			IsAlive = false;
			Hp = 0;
		}
		
		DamageReceived = damage;
	}

	public void AddStatus(Status status) 
	{
		Status = status;
	}
	
	public bool ReduceStatusDuration(bool remove = false)
	{
		StatusDuration--;
		if (remove || StatusDuration == 0) 
		{
			Status = null;
			StatusDuration = 0;
			DamageOverTime = 0;
			return false;
		}

		return true;
	}
	
	public void AddBuff(Buff buff) 
	{
		Buff = buff;
	}
	
	public bool ReduceBuffDuration() 
	{
		BuffDuration--;
		if (BuffDuration == 0) 
		{
			Buff = null;
			BuffDuration = 0;
			return false;
		}

		return true;
	}

	public void Heal(int hp) 
	{
		Hp += hp;
		if (hp > MaxHp)
			Hp = MaxHp;
	}

	public void GrantMP(int mp) 
	{
		Mp += mp;
		if (Mp > BattleConstants.BASE_MP)
			Mp = BattleConstants.BASE_MP;
	}

	public void UseMP(int mp) 
	{
		Mp -= mp;
		if (Mp < 0)
			Mp = 0;
	}

	public void GrantBarrier(int barrier) 
	{
		Barrier += barrier;
	}
	#endregion

	#region Discord Output Methods
	public string FormatBattleInfo(DiscordClient s)
	{
		var info = new StringBuilder();
		if (Barrier > 0)
		{
			info.Append($"{DiscordEmoji.FromName(s, ":blue_heart:", false)} HP: {Hp + Barrier} / {MaxHp}\n");
		}
		else
		{
			info.Append($"{DiscordEmoji.FromName(s, ":heart:", false)} HP: {Hp} / {MaxHp}\n");
		}
		info.Append($"{DiscordEmoji.FromName(s, ":gem:", false)} MP: {Mp} / {BattleConstants.BASE_MP}\n");
		info.Append($"{DiscordEmoji.FromName(s, ":crossed_swords:", false)} Damage: {MinDamage} - {MaxDamage}\n");
		if (Buff != null)
			info.Append($"{DiscordEmoji.FromName(s, ":up_arrow:", false)} Buff: {Buff.GetName(s)} `({Buff.Duration})`\n");

		if (Status != null)
			info.Append($"{DiscordEmoji.FromName(s, ":down_arrow:", false)} Status: {Status.GetName(s)} `({Status.Duration})`\n");

		return info.ToString();
	}
	#endregion

	#region Private Members
	private int _hp = 0;
	#endregion
}