using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using JoJoLibrary.Helpers;

namespace JoJoLibrary;

public class BattlePlayer : Player 
{
	#region Properties
	public readonly DiscordClient Client;
	public bool IsAlive { get; set; } = true;
	public Dictionary<Ability, int> Cooldowns { get; set; } = [];
	
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
	public int PassiveTriggers { get; set; } = 0;
	#endregion
	
	#region Status Properties
	public Status? Status { get; private set; } = null;
	public int StatusDuration { get; set; } = 0;
	public int DamageOverTime { get; set; }
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
		_hp = GetMaxHp(Level);
		MaxHp = Hp;
		MinDamage = _minDamage = GetMinDmg(Level);
		MaxDamage = _maxDamage = GetMaxDmg(Level);
		Cooldowns = new Dictionary<Ability, int>()
		{
			{Stand!.Ability0, 0},
			{Stand.Ability1, 0},
			{Stand.Ability2, 0},
			{Stand.Ability3, 0},
			{Stand.Ability4, 0}
		};
	}

	// copy constructor
	public BattlePlayer(BattlePlayer player) : base(player.Guild, player.User)
	{
		Client = player.Client;
		Stand = player.Stand;
		Level = player.Level;
		Experience = player.Experience;
		Cooldowns = player.Cooldowns;
		
		_hp = player._hp;
		MaxHp = player.MaxHp;
		_barrier = player._barrier;
		Mp = player.Mp;
		MinDamage = _minDamage = player._minDamage;
		MaxDamage = _maxDamage = player._maxDamage;
		CritChance = player.CritChance;
		CritDamageMultiplier = player.CritDamageMultiplier;
		PassiveTriggers = player.PassiveTriggers;
		Status = player.Status;
		StatusDuration = player.StatusDuration;
		DamageOverTime = player.DamageOverTime;
		Buff = player.Buff;
		BuffDuration = player.BuffDuration;
	}
	#endregion

	#region Player Control Methods

	public void ReceiveDamage(Turn turn, int damage, out int hpBefore) 
	{
		hpBefore = Hp;
		switch (_barrier)
		{
			case > 0 when damage < _barrier:
				_barrier -= damage;
				break;
			case > 0 when damage > _barrier:
				damage -= _barrier;
				_barrier = 0;
				_hp -= damage;
				break;
			default:
				_hp -= damage;
				break;
		}
		DamageReceived = damage;

		if (Hp > 0) return;
		
		IsAlive = false;
		_hp = 0;
		throw new OnDeathException(turn, this);
	}

	public void AddStatus(Status status) => Status = status;
	
	public bool ReduceStatusDuration(bool remove)
	{
		StatusDuration--;
		if (!remove && StatusDuration != 0) return true;
		
		Status = null;
		StatusDuration = 0;
		DamageOverTime = 0;
		return false;
	}
	
	public void AddDamageOverTime(int dot, Status status) 
	{
		if (Status is not null && Status.GetType() == status.GetType()) // check for stacking DOT
		{
			DamageOverTime += dot;
		}
		else 
		{
			DamageOverTime = dot;
		}
	}

	public void AddBuff(Buff buff) => Buff = buff;
	
	public bool ReduceBuffDuration(bool remove) 
	{
		BuffDuration--;
		if (!remove && BuffDuration != 0) return true;
		
		Buff = null;
		BuffDuration = 0;
		return false;
	}

	public void Heal(int hp, out int hpBefore) 
	{
		hpBefore = _hp;
		_hp += hp;
		if (_hp > MaxHp)
			_hp = MaxHp;
	}

	public void GrantMP(int mp, out int mpBefore) 
	{
		mpBefore = Mp;
		Mp += mp;
		if (Mp > BattleConstants.BASE_MP)
			Mp = BattleConstants.BASE_MP;
	}

	public void UseMP(int mp, out int mpBefore) 
	{
		mpBefore = Mp;
		Mp -= mp;
		if (Mp < 0)
			Mp = 0;
	}

	public void GrantBarrier(int barrier, out int barrierBefore) 
	{
		barrierBefore = _barrier;
		_barrier += barrier;
	}
	
	public void IncreaseAttack(int minDamageIncrease, int maxDamageIncrease) 
	{
		_minDamage += minDamageIncrease;
		_maxDamage += maxDamageIncrease;
		MinDamage = _minDamage;
		MaxDamage = _maxDamage;
	}
	
	public void DecreaseAttack(int minDamageDecrease, int maxDamageDecrease) 
	{
		_minDamage -= minDamageDecrease;
		_maxDamage -= maxDamageDecrease;
		MinDamage = _minDamage;
		MaxDamage = _maxDamage;
	}
	#endregion

	#region Discord Output Methods
	public string FormatBattleInfo()
	{
		var info = new StringBuilder();
		info.AppendLine($"{(_barrier > 0 ? "💙" : "❤️")} HP: {Hp} / {MaxHp}");
		info.AppendLine($"💎 MP: {Mp} / {BattleConstants.BASE_MP}");
		info.AppendLine($"⚔️ Damage: {MinDamage} - {MaxDamage}");
		if (DamageResistance != 0) info.AppendLine($"🛡️ DR: {JoJo.ConvertToPercent(DamageResistance)}%");
		if (Buff != null) info.AppendLine($"⬆️ Buff: {Buff.Name} `({BuffDuration})`");
		if (Status != null) info.AppendLine($"⬇️ Status: {Status.Name} `({StatusDuration})`");

		return info.ToString();
	}

	public string FormatBattleDetails()
	{
		if (Stand is null) return string.Empty;
		StringBuilder description = new();
		
		description.AppendLine($"### {User.Mention} (Level `{Level}`)");
		description.AppendLine($"### {(Barrier > 0 ? "💙" : "❤️")} HP: `{Hp}` / `{MaxHp}`");
		description.AppendLine($"### 💎 MP: `{Mp}` / `{BattleConstants.BASE_MP}`");
		description.AppendLine($"### ⚔️ Damage: `{MinDamage}` - `{_maxDamage}`");
		if (DamageResistance != 0) description.AppendLine($"### 🛡️ Damage Resistance: `{JoJo.ConvertToPercent(DamageResistance)}%`");
		if (Status is not null)
		{
			description.AppendLine("### ⬇️ Status");
			description.AppendLine($"* `{Status.Name}`");
			description.AppendLine($"* Duration: `{StatusDuration}` turns");
			if (DamageOverTime != 0) description.AppendLine($"* Damage Over Time: `{DamageOverTime}`");
		}
		
		if (Buff is not null)
		{
			description.AppendLine("### ⬆️ Buff");
			description.AppendLine($"* `{Buff.Name}`");
			description.AppendLine($"* Duration: `{BuffDuration}` turns");
		}

		return description.ToString();
	}
	#endregion

	#region Private Members
	private int _hp = 0;
	private int _barrier = 0;
	private int _minDamage = 0;
	private int _maxDamage = 0;
	#endregion
}