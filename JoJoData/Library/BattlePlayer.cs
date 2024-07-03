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
			if (_barrier > 0)
			{
				return _barrier + _hp;
			}

			return _hp;
		}
		private set 
		{
			_hp = value;
		}
	}
	public int Barrier => _barrier;
	public int MaxHp { get; private set; }
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
		Load();
		Hp = Stand!.BaseHp + ((int)(Stand.BaseHp * 0.30) * Level);
		MaxHp = Hp;
		MinDamage = _minDamage = Stand.BaseMinDamage + ((int)(Stand.BaseMinDamage * 0.15) * Level);
		MaxDamage = _maxDamage = Stand.BaseMaxDamage + ((int)(Stand.BaseMaxDamage * 0.20) * Level);
	}

	// copy constructor
	public BattlePlayer(BattlePlayer player) : base(player.Guild, player.User)
	{
		Client = player.Client;
		Stand = player.Stand;
		Level = player.Level;
		Experience = player.Experience;
		
		_hp = player._hp;
		MaxHp = player.MaxHp;
		_barrier = player._barrier;
		Mp = player.Mp;
		MinDamage = _minDamage = player._minDamage;
		MaxDamage = _maxDamage = player._maxDamage;
		CritChance = player.CritChance;
		CritDamageMultiplier = player.CritDamageMultiplier;
		Status = player.Status;
		StatusDuration = player.StatusDuration;
		DamageOverTime = player.DamageOverTime;
		Buff = player.Buff;
		BuffDuration = player.BuffDuration;
	}
	#endregion

	#region Player Control Methods

	public void ReceiveDamage(int damage) 
	{
		if (_barrier > 0 && damage < _barrier) 
		{
			_barrier -= damage;
		}
		else if (_barrier > 0 && damage > _barrier) 
		{
			damage -= _barrier;
			_barrier = 0;
			_hp -= damage;
		}
		else 
		{
			_hp -= damage;
		}
		
		if (Hp <= 0) 
		{
			IsAlive = false;
			_hp = 0;
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
		_hp += hp;
		if (_hp > MaxHp)
			_hp = MaxHp;
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
		_barrier += barrier;
	}
	
	public void IncreaseAttack(int minDamageIncrease, int maxDamageIncrease) 
	{
		_minDamage += minDamageIncrease;
		_maxDamage += maxDamageIncrease;
		MinDamage = _minDamage;
		MaxDamage = _maxDamage;
	}
	#endregion

	#region Discord Output Methods
	public string FormatBattleInfo(DiscordClient s)
	{
		var info = new StringBuilder();
		if (_barrier > 0)
		{
			info.Append($"{DiscordEmoji.FromName(s, ":blue_heart:", false)} HP: {Hp} / {MaxHp}\n");
		}
		else
		{
			info.Append($"{DiscordEmoji.FromName(s, ":heart:", false)} HP: {Hp} / {MaxHp}\n");
		}
		info.Append($"{DiscordEmoji.FromName(s, ":gem:", false)} MP: {Mp} / {BattleConstants.BASE_MP}\n");
		info.Append($"{DiscordEmoji.FromName(s, ":crossed_swords:", false)} Damage: {MinDamage} - {MaxDamage}\n");
		if (Buff != null)
			info.Append($"{DiscordEmoji.FromName(s, ":up_arrow:", false)} Buff: {Buff.GetName(s)} `({BuffDuration})`\n");

		if (Status != null)
			info.Append($"{DiscordEmoji.FromName(s, ":down_arrow:", false)} Status: {Status.GetName(s)} `({StatusDuration})`\n");

		return info.ToString();
	}
	#endregion

	#region Private Members
	private int _hp = 0;
	private int _barrier = 0;
	private int _minDamage = 0;
	private int _maxDamage = 0;
	#endregion
}