using JoJoData.Library;
using Barrier = JoJoData.Library.Barrier;

namespace JoJoData.Abilities;

#region Star Platinum
public class Ora : AttackAbility
{
	public Ora()
	{
		Name = "Ora";
		Description = "";
		MpCost = 15;
		Attack = new BasicAttack(damage: 2);
	}
}

public class OraOraOraOra : AttackAbility
{
	public OraOraOraOra()
	{
		Name = "Ora Ora Ora Ora";
		Description = "";
		MpCost = 35;
		Attack = new MultiHitAttack(damage: 1, minHits: 2, maxHits: 5);
	}
}

public class StarFinger : AttackAbility
{

	public StarFinger()
	{
		Name = "Star Finger";
		Description = "";
		MpCost = 25;
		Attack = new CritChanceIncreaseAttack(damage: 2, increase: 0.3);
	}
}

public class StarPlatinumTheWorld : InflictStatusAbility
{
	public StarPlatinumTheWorld()
	{
		Name = "Star Platinum : The World";
		Description = "";
		MpCost = 75;
		Status = new TimeStop(duration: 3);
	}
}
#endregion

#region Magician's Red
public class MagicianBurn : StatusAttackAbility
{
	public MagicianBurn()
	{
		Name = "Burn";
		Description = "";
		MpCost = 15;
		Attack = new BasicAttack(damage: 1.5);
		Status = new Burn(duration: 3, applyChance: 0.2);
	}
}

public class CrossFireHurricane : AttackAbility
{
	public CrossFireHurricane()
	{
		Name = "Cross Fire Hurricane";
		Description = "";
		MpCost = 40;
		Attack = new BasicAttack(damage: 4);
	}
}

public class RedBind : BuffAttackAbility
{
	public RedBind()
	{
		Name = "Red Bind";
		Description = "";
		MpCost = 50;
		Attack = new BasicAttack(damage: 1.2);
		Buff = new Haste(duration: 1);
	}
}

public class LifeDetector : BuffAbility
{
	public LifeDetector()
	{
		Name = "Life Detector";
		Description = "";
		MpCost = 25;
		Buff = new Protect(duration: 2, dr: 0.50);
	}
}
#endregion

#region Hermit Purple
public class PurpleWhip : AttackAbility
{
	public PurpleWhip()
	{
		Name = "Whip";
		Description = "";
		MpCost = 15;
		Attack = new BasicAttack(damage: 2.5);
	}
}

public class Overdrive : AttackAbility
{
	public Overdrive()
	{
		Name = "Overdrive";
		Description = "";
		MpCost = 30;
		Attack = new CritChanceIncreaseAttack(damage: 3, increase: 0.5);
	}
}

public class Ensnare : InflictStatusAbility
{
	public Ensnare()
	{
		Name = "Ensnare";
		Description = "";
		MpCost = 20;
		Status = new Weak(duration: 4, drReduction: 0.15);
	}
}

public class HamonHeal : StatChangeAbility
{
	public HamonHeal()
	{
		Name = "Hamon";
		Description = "";
		MpCost = 40;
		StatChange = new Heal(healPercent: 0.45);
	}
}
#endregion

#region Hierophant Green
public class EmeraldSplash : AttackAbility
{
	public EmeraldSplash()
	{
		Name = "Emerald Splash";
		MpCost = 20;
		Attack = new MultiHitAttack(damage: 0.5, minHits: 2, maxHits: 4);
	}
}

public class TwentyMeterEmeraldSplash : AttackAbility
{
	public TwentyMeterEmeraldSplash()
	{
		MpCost = 55;
		Attack = new MultiHitAttack(damage: 0.5, minHits: 4, maxHits: 10);
	}
}

public class HierophantBodyTakeover : AttackAbility
{
	public HierophantBodyTakeover()
	{
		MpCost = 15;
		Attack = new TakeoverAttack(damage: 1.5);
	}
}

public class ReroReroRero : StatChangeAbility
{
	public ReroReroRero()
	{
		MpCost = 25;
		StatChange = new Heal(healPercent: 0.25);
	}
}
#endregion

#region Silver Chariot
public class ChariotSlash : StatusAttackAbility
{
	public ChariotSlash()
	{
		MpCost = 10;
		Attack = new BasicAttack(damage: 1.25);
		Status = new Bleed(duration: 3, applyChance: 0.2);
	}
}

public class HoraRush : AttackAbility
{
	public HoraRush()
	{
		MpCost = 35;
		Attack = new MultiHitAttack(damage: 0.25, minHits: 5, maxHits: 15);
	}
}

public class ArmorRemoval : BuffAttackAbility
{
	public ArmorRemoval()
	{
		MpCost = 30;
		Attack = new BasicAttack(damage: 0.3);
		Buff = new Haste(duration: 1);
	}
}

public class SwordLaunch : AttackAbility
{
	public SwordLaunch()
	{
		MpCost = 25;
		Attack = new CritDamageIncreaseAttack(damage: 4, increase: 1);
		// TODO skip ur next turn
	}
}
#endregion

#region The Fool
public class SandAttack : AttackAbility
{
	public SandAttack()
	{
		MpCost = 15;
		Attack = new BasicAttack(damage: 2);
	}
}

public class SandConstruct : StatChangeAbility
{
	public SandConstruct()
	{
		MpCost = 45;
		StatChange = new Barrier(barrier: 0.5);
	}
}

public class Sandstorm : StatusAttackAbility
{
	public Sandstorm()
	{
		MpCost = 35;
		Attack = new MultiHitAttack(damage: 0.9, minHits: 2, maxHits: 4);
		Status = new Confusion(duration: 3, applyChance: 0.2);
	}
}

public class DogBite : AttackAbility
{
	public DogBite()
	{
		MpCost = 0;
		Attack = new MPStealAttack(damage: 0.25, mpStealAmount: 20, hpLossPercent: 0.1);
	}
}
#endregion

#region The World
public class Muda : AttackAbility
{
	public Muda()
	{
		Name = "MUDA";
		Description = "";
		MpCost = 15;
		Attack = new BasicAttack(damage: 2);
	}
}

public class MudaMudaMudaMuda : AttackAbility
{
	public MudaMudaMudaMuda()
	{
		Name = "MUDA MUDA MUDA MUDA";
		Description = "";
		MpCost = 25;
		Attack = new MultiHitAttack(damage: 0.75, minHits: 1, maxHits: 6);
	}
}

public class RoadRoller : AttackAbility
{
	public RoadRoller()
	{
		Name = "ROAD ROLLA DA";
		Description = "";
		MpCost = 35;
		Attack = new CritChanceIncreaseAttack(damage: 3, increase: 0);
	}
}

public class TheWorld : InflictStatusAbility
{
	public TheWorld()
	{
		Name = "The World";
		Description = "";
		MpCost = 75;
		Status = new TimeStop(duration: 3);
	}
}
#endregion

#region Tower of Gray
public class AirplaneHijack : BuffAttackAbility
{
	public AirplaneHijack()
	{
		Name = "Airplane Hijack";
		Description = "";
		MpCost = 20;
		Attack = new BasicAttack(damage: 0.5);
		Buff = new Haste(duration: 1);
	}
}

public class TowerNeedle : StatusAttackAbility
{
	public TowerNeedle()
	{
		Name = "Tower Needle";
		Description = "";
		MpCost = 25;
		Attack = new BasicAttack(damage: 1.5);
		Status = new Bleed(duration: 4, applyChance: 0.25);
	}
}

public class TongueSteal : InflictStatusAbility
{
	public TongueSteal()
	{
		Name = "Tongue Steal";
		Description = "";
		MpCost = 45;
		Status = new Silence(duration: 2, applyChance: 0.5);
	}
}

public class TowerFly : BuffAbility
{
	public TowerFly()
	{
		Name = "Tower Fly";
		Description = "";
		MpCost = 25;
		Buff = new Await();
	}
}
#endregion

#region Dark Blue Moon
public class WetSlap : AttackAbility
{
	public WetSlap()
	{
		MpCost = 10;
		Attack = new BasicAttack(damage: 1.5);
	}
}

public class RazorScales : AttackAbility
{
	public RazorScales()
	{
		MpCost = 35;
		Attack = new MultiHitAttack(damage: 1, minHits: 2, maxHits: 4);
	}
}

public class Barnacles : InflictStatusAbility
{
	public Barnacles()
	{
		MpCost = 20;
		Status = new Poison(duration: 4, applyChance: 0.5);
	}
}

public class Drown : AttackAbility
{
	public Drown()
	{
		MpCost = 50;
		Attack = new HPLeechAttack(damage: 3, hpStealPercent: 0.15);
	}
}
#endregion

#region Strength
public class MonkeySlam : AttackAbility
{
	public MonkeySlam()
	{
		MpCost = 20;
		Attack = new BasicAttack(damage: 3);
	}
}

public class MonkeyPipeWhip : AttackAbility
{
	public MonkeyPipeWhip()
	{
		MpCost = 30;
		Attack = new CritDamageIncreaseAttack(damage: 2, increase: 1);
	}
}

public class EatBanana : StatChangeAbility
{
	public EatBanana()
	{
		MpCost = 40;
		StatChange = new Heal(healPercent: 0.25);
	}
}

public class MonkeyHug : StatusAttackAbility
{
	public MonkeyHug()
	{
		MpCost = 0;
		Attack = new MPStealAttack(damage: 0.2, mpStealAmount: 20, hpLossPercent: 0.15);
		Status = new Confusion(duration: 2, applyChance: 1);
	}
}
#endregion

#region Hanged Man
public class MirrorSlash : StatusAttackAbility
{
	public MirrorSlash()
	{
		Name = "Mirror Slash";
		MpCost = 15;
		Attack = new BasicAttack(damage: 1.5);
		Status = new Bleed(duration: 3, applyChance: 0.2);
	}
}

public class LightspeedSlash : AttackAbility
{
	public LightspeedSlash()
	{
		Name = "Lightspeed Slash";
		MpCost = 35;
		Attack = new CritChanceIncreaseAttack(damage: 2.5, increase: 0.3);
	}
}

public class MirrorSneak : BuffAbility 
{
	public MirrorSneak() 
	{
		Name = "Mirror Sneak";
		MpCost = 30;
		Buff = new Charge(duration: 1);
	}
}

public class RainBarrier : StatChangeAbility 
{
	public RainBarrier() 
	{
		Name = "Rain Barrier";
		MpCost = 40;
		StatChange = new Barrier(barrier: 0.35);
	}
}
#endregion