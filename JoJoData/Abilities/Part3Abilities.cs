using JoJoData.Library;
using Barrier = JoJoData.Library.Barrier;
using Random = JoJoData.Library.Random;

namespace JoJoData.Abilities;

#region Star Platinum
public class Ora : AttackAbility
{
	public Ora()
	{
		Name = "ORA!!";
		Description = "";
		MpCost = 15;
		Attack = new BasicAttack(damage: 2);
	}
}

public class OraOraOraOra : AttackAbility
{
	public OraOraOraOra()
	{
		Name = "ORA ORA ORA ORA";
		Description = "";
		MpCost = 35;
		Attack = new MultiHitAttack(damage: 0.9, minHits: 2, maxHits: 5);
	}
}

public class StarFinger : AttackAbility
{

	public StarFinger()
	{
		Name = "STAR FINGER!!!";
		Description = "";
		MpCost = 25;
		Attack = new CritChanceIncreaseAttack(damage: 2, increase: 0.4);
	}
}

public class StarPlatinumTheWorld : InflictStatusAbility
{
	public StarPlatinumTheWorld()
	{
		Name = "Star Platinum : ZA WARUDO";
		Description = "";
		MpCost = 80;
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
		Name = "CROSS FIRE HURRICANE";
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
		Attack = new BasicAttack(damage: 2);
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
		Attack = new BasicAttack(damage: 2);
	}
}

public class Overdrive : AttackAbility
{
	public Overdrive()
	{
		Name = "HAMON OVERDRIVE";
		Description = "";
		MpCost = 35;
		Attack = new CritChanceIncreaseAttack(damage: 2.5, increase: 0.5);
	}
}

public class Ensnare : InflictStatusAbility
{
	public Ensnare()
	{
		Name = "Ensnare";
		Description = "";
		MpCost = 20;
		Status = new Frail(duration: 4);
	}
}

public class HamonHeal : StatChangeAbility
{
	public HamonHeal()
	{
		Name = "Hamon";
		Description = "";
		MpCost = 60;
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
		Attack = new MultiHitAttack(damage: 0.75, minHits: 2, maxHits: 4);
	}
}

public class TwentyMeterEmeraldSplash : AttackAbility
{
	public TwentyMeterEmeraldSplash()
	{
		Name = "TWENTY METER EMERALD SPLASH";
		MpCost = 55;
		Attack = new MultiHitAttack(damage: 0.8, minHits: 4, maxHits: 10);
	}
}

public class HierophantBodyTakeover : AttackAbility
{
	public HierophantBodyTakeover()
	{
		Name = "Body Takeover";
		MpCost = 15;
		Attack = new TakeoverAttack(damage: 2);
	}
}

public class ReroReroRero : StatChangeAbility
{
	public ReroReroRero()
	{
		Name = "🍒 Rero Rero Rero Rero 🍒";
		MpCost = 50;
		StatChange = new Heal(healPercent: 0.25);
	}
}
#endregion

#region Silver Chariot
public class ChariotSlash : StatusAttackAbility
{
	public ChariotSlash()
	{
		Name = "Slash";
		MpCost = 10;
		Attack = new BasicAttack(damage: 1.25);
		Status = new Bleed(duration: 3, applyChance: 0.4);
	}
}

public class HoraRush : AttackAbility
{
	public HoraRush()
	{
		Name = "Hora Rush";
		MpCost = 35;
		Attack = new MultiHitAttack(damage: 0.25, minHits: 5, maxHits: 15);
	}
}

public class ArmorRemoval : StatChangeAbility
{
	public ArmorRemoval()
	{
		Name = "Armor Removal";
		MpCost = 40;
		StatChange = new Strength(increase: 0.15);
	}
}

public class SwordLaunch : AttackAbility
{
	public SwordLaunch()
	{
		Name = "Sword Launch";
		MpCost = 75;
		Attack = new CritDamageIncreaseAttack(damage: 2, increase: 5);
	}
}
#endregion

#region The Fool
public class SandAttack : AttackAbility
{
	public SandAttack()
	{
		Name = "Sand Attack";
		MpCost = 15;
		Attack = new BasicAttack(damage: 2.3);
	}
}

public class SandConstruct : StatChangeAbility
{
	public SandConstruct()
	{
		Name = "Sand Construct";
		MpCost = 45;
		StatChange = new Barrier(barrier: 0.35);
	}
}

public class Sandstorm : StatusAttackAbility
{
	public Sandstorm()
	{
		Name = "Sandstorm";
		MpCost = 40;
		Attack = new MultiHitAttack(damage: 1, minHits: 2, maxHits: 4);
		Status = new Confusion(duration: 3, applyChance: 0.3);
	}
}

public class DogBite : AttackAbility
{
	public DogBite()
	{
		Name = "Doggy Bite";
		MpCost = 0;
		Attack = new MPStealAttack(damage: 0.25, mpStealAmount: 15, hpLossPercent: 0.1);
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
		MpCost = 30;
		Attack = new MultiHitAttack(damage: 0.75, minHits: 1, maxHits: 6);
	}
}

public class RoadRoller : AttackAbility
{
	public RoadRoller()
	{
		Name = "ROAD ROLLA DA";
		Description = "";
		MpCost = 40;
		Attack = new CritChanceIncreaseAttack(damage: 4, increase: 0);
	}
}

public class TheWorld : InflictStatusAbility
{
	public TheWorld()
	{
		Name = "ZA WARUDO";
		Description = "";
		MpCost = 80;
		Status = new TimeStop(duration: 3);
	}
}
#endregion

#region Tower of Gray
public class AirplaneHijack : AttackAbility
{
	public AirplaneHijack()
	{
		Name = "Airplane Hijack";
		Description = "";
		MpCost = 20;
		Attack = new BasicAttack(damage: 2.5);
		//Buff = new Haste(duration: 1);
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
		Status = new Bleed(duration: 4, applyChance: 0.5);
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
		MpCost = 45;
		Buff = new Await();
	}
}
#endregion

#region Dark Blue Moon
public class WetSlap : AttackAbility
{
	public WetSlap()
	{
		Name = "Wet Slap";
		MpCost = 10;
		Attack = new BasicAttack(damage: 1.5);
	}
}

public class RazorScales : AttackAbility
{
	public RazorScales()
	{
		Name = "Razor Scales";
		MpCost = 35;
		Attack = new MultiHitAttack(damage: 1, minHits: 2, maxHits: 4);
	}
}

public class Barnacles : InflictStatusAbility
{
	public Barnacles()
	{
		Name = "Barnacles";
		MpCost = 20;
		Status = new Poison(duration: 4, applyChance: 0.5);
	}
}

public class Drown : AttackAbility
{
	public Drown()
	{
		Name = "Drown";
		MpCost = 50;
		Attack = new HPLeechAttack(damage: 3);
	}
}
#endregion

#region Strength
public class MonkeySlam : AttackAbility
{
	public MonkeySlam()
	{
		Name = "Monkey Slam";
		MpCost = 20;
		Attack = new BasicAttack(damage: 3);
	}
}

public class MonkeyPipeWhip : AttackAbility
{
	public MonkeyPipeWhip()
	{
		Name = "Monkey Pipe Whip";
		MpCost = 30;
		Attack = new CritDamageIncreaseAttack(damage: 2, increase: 1);
	}
}

public class EatBanana : StatChangeAbility
{
	public EatBanana()
	{
		Name = "🍌";
		MpCost = 40;
		StatChange = new Heal(healPercent: 0.2);
	}
}

public class MonkeyHug : AttackAbility
{
	public MonkeyHug()
	{
		Name = "Monkey Hug";
		MpCost = 0;
		Attack = new MPStealAttack(damage: 0.2, mpStealAmount: 20, hpLossPercent: 0.3);
	}
}
#endregion

#region Yellow Temperance
public class GooPunch : AttackAbility 
{
	public GooPunch() 
	{
		Name = "Goo Punch";
		MpCost = 20;
		Attack = new BasicAttack(damage: 1.7);
	}
}

public class BlobArmor : BuffAbility
{
	public BlobArmor()
	{
		Name = "Blob Armor";
		MpCost = 35;
		Buff = new Protect(duration: 2, dr: 0.35);
	}
}

public class FleshEater : StatusAttackAbility 
{
	public FleshEater() 
	{
		Name = "Flesh Eater";
		MpCost = 45;
		Attack = new CritDamageIncreaseAttack(damage: 2.4, increase: 0.5);
		Status = new Poison(duration: 5, applyChance: 0.35);
	}
}

public class Materialization : StatChangeAbility 
{
	public Materialization() 
	{
		Name = "Materialization";
		MpCost = 50;
		StatChange = new Barrier(barrier: 0.25);
	}
}
#endregion

#region Ebony Devil
public class BiteNSlash : AttackAbility 
{
	public BiteNSlash() 
	{
		Name = "Bite n' Slash";
		MpCost = 20;
		Attack = new MultiHitAttack(damage: 1.5, minHits: 1, maxHits: 2);
	}
}

public class DevilRage : StatChangeAbility 
{
	public DevilRage() 
	{
		Name = "Devil's Rage";
		MpCost = 15;
		StatChange = new Strength(increase: 0.1);
	}
}

public class TearHisNuts : AttackAbility 
{
	public TearHisNuts() 
	{
		Name = "I'M GONNA TEAR YOUR NUTS!!";
		MpCost = 35;
		Attack = new CritChanceIncreaseAttack(damage: 2.2, increase: 0.3);
	}
}

public class DevilMisfortune : InflictStatusAbility 
{
	public DevilMisfortune() 
	{
		Name = "Devil's Misfortune";
		MpCost = 45;
		Status = new Random(duration: 4);
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
		Attack = new BasicAttack(damage: 2.3);
		Status = new Bleed(duration: 3, applyChance: 0.2);
	}
}

public class LightspeedSlash : AttackAbility
{
	public LightspeedSlash()
	{
		Name = "Lightspeed Slash";
		MpCost = 35;
		Attack = new CritChanceIncreaseAttack(damage: 2, increase: 0.3);
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

#region Emperor
public class ShootGun : AttackAbility
{
	public ShootGun() 
	{
		Name = "Shoot";
		MpCost = 15;
		Attack = new BasicAttack(damage: 2);
	}
}

public class UnloadClip : AttackAbility 
{
	public UnloadClip() 
	{
		Name = "Unload Clip";
		MpCost = 30;
		Attack = new MultiHitAttack(damage: 0.6, minHits: 3, maxHits: 8);
	}
}

public class PrecisionShot : AttackAbility 
{
	public PrecisionShot() 
	{
		Name = "Precision Shot";
		MpCost = 45;
		Attack = new CritChanceIncreaseAttack(damage: 2.5, increase: 0.4);
	}
}

public class HomingBullet : AttackAbility 
{
	public HomingBullet() 
	{
		Name = "Homing Bullet";
		MpCost = 25;
		Attack = new BypassProtectAttack(damage: 3);
	}
}
#endregion

#region Empress
public class NailStab : AttackAbility 
{
	public NailStab() 
	{
		Name = "Stab";
		MpCost = 15;
		Attack = new CritDamageIncreaseAttack(damage: 3, increase: 0.5);
	}
}

public class LifeDrain : AttackAbility 
{
	public LifeDrain() 
	{
		Name = "Life Drain";
		MpCost = 45;
		Attack = new HPLeechAttack(damage: 2);
	}
}

public class EssenceDrain : AttackAbility 
{
	public EssenceDrain() 
	{
		Name = "Essence Drain";
		MpCost = 0;
		Attack = new MPStealAttack(damage: 1.5, mpStealAmount: 15, hpLossPercent: 0.15);
	}
}

public class Grow : StatChangeAbility 
{
	public Grow() 
	{
		Name = "Grow";
		MpCost = 15;
		StatChange = new Strength(increase: 0.15);
	}
}
#endregion

#region Wheel of Fortune
public class HitAndRun : AttackAbility 
{
	public HitAndRun() 
	{
		Name = "Hit and Run";
		MpCost = 30;
		Attack = new BasicAttack(damage: 3.5); 
	}
}

public class VehicularManslaughter : AttackAbility 
{
	public VehicularManslaughter() 
	{
		Name = "Vehicular Manslaughter";
		MpCost = 40;
		Attack = new CritDamageIncreaseAttack(damage: 2, increase: 1);
	}
}

public class GasolineBullets : StatusAttackAbility 
{
	public GasolineBullets() 
	{
		Name = "Gasoline Bullets";
		MpCost = 15;
		Attack = new BasicAttack(damage: 0.5);
		Status = new Douse(duration: 4, applyChance: 0.5);
	}
}

public class CableAttack : AttackAbility 
{
	public CableAttack() 
	{
		Name = "Cable Attack";
		MpCost = 20;
		Attack = new IgniteAttack(damage: 1);
	}
}
#endregion

#region Justice
public class GhostBaby : StatusAttackAbility 
{
	public GhostBaby() 
	{
		Name = "Ghost Baby";
		MpCost = 25;
		Attack = new BasicAttack(damage: 1.4);
		Status = new Bleed(duration: 2, applyChance: 0.75);
	}
}

public class HoleAttack : AttackAbility
{
	public HoleAttack() 
	{
		Name = "Hole Attack";
		MpCost = 35;
		Attack = new WeaknessAttack(damage: 2, increase: 2, typeof(Bleed));
	}
}

public class FogIllusion : BuffAbility 
{
	public FogIllusion() 
	{
		Name = "Fog Illusion";
		MpCost = 45;
		Buff = new Await();
	}
}

public class FogStringPuppetry : StatusAttackAbility 
{
	public FogStringPuppetry() 
	{
		Name = "Fog String Puppetry";
		MpCost = 30;
		Attack = new BasicAttack(damage: 1.8);
		Status = new Confusion(duration: 2, applyChance: 0.9);
	}
}
#endregion

#region Lovers
public class BrainDamage : StatusAttackAbility 
{
	public BrainDamage() 
	{
		Name = "Brain Damage";
		MpCost = 15;
		Attack = new BasicAttack(damage: 0.8);
		Status = new Confusion(duration: 3, applyChance: 0.6);
	}
}

public class BrainPod : AttackAbility
{
	public BrainPod() 
	{
		Name = "Brain Pod";
		MpCost = 30;
		Attack = new BasicAttack(damage: 3);
	}
}

public class PainSplit : AttackAbility 
{
	public PainSplit() 
	{
		Name = "Pain Split";
		MpCost = 25;
		Attack = new TakeoverAttack(damage: 2.5);
	}
}

public class BrainDeath : InflictStatusAbility
{
	public BrainDeath() 
	{
		Name = "Brain Death";
		MpCost = 75;
		Status = new Doom(duration: 5);
	}
}
#endregion

#region Sun
public class HeatWave : StatChangeAbility
{
	public HeatWave() 
	{
		Name = "Heat Wave";
		MpCost = 35;
		StatChange = new Strength(increase: 0.25);
	}
}

public class LightRayEnergy : AttackAbility
{
	public LightRayEnergy() 
	{
		Name = "Light Ray Energy";
		MpCost = 25;
		Attack = new MultiHitAttack(damage: 0.3, minHits: 5, maxHits: 15);
	}
}

public class SolarFlare : InflictStatusAbility 
{
	public SolarFlare() 
	{
		Name = "Solar Flare";
		MpCost = 50;
		Status = new Burn(duration: 3, applyChance: 1);
	}
}

public class TwoRocksLol : AttackAbility 
{
	public TwoRocksLol() 
	{
		Name = "TWO ROCKS LMAO";
		MpCost = 10;
		Attack = new CritDamageIncreaseAttack(damage: 0.25, increase: 100);
	}
}
#endregion

#region Death 13
public class Bedtime : InflictStatusAbility 
{
	public Bedtime()
	{
		Name = "Bedtime";
		MpCost = 30;
		Status = new Sleep(duration: 3, applyChance: 0.75);
	}
}

public class Nightmare : AttackAbility 
{
	public Nightmare() 
	{
		Name = "Nightmare";
		MpCost = 35;
		Attack = new WeaknessAttack(damage: 2.5, increase: 2, typeof(Sleep));
	}
}

public class PoopySoupy : StatChangeAbility
{
	public PoopySoupy() 
	{
		Name = "💩 Poopy Soupy 💩";
		MpCost = 15;
		StatChange = new Heal(healPercent: 0.1);
	}
}

public class Dreamland : BuffAbility 
{
	public Dreamland() 
	{
		Name = "Dreamland";
		MpCost = 40;
		Buff = new Protect(duration: 4, dr: 0.3);
	}
}
#endregion

#region Judgment
public class Gaslight : InflictStatusAbility
{
	public Gaslight() 
	{
		Name = "Gaslight";
		MpCost = 25;
		Status = new Confusion(duration: 2, applyChance: 0.9);
	}
}

public class Gatekeep : BuffAbility 
{
	public Gatekeep() 
	{
		Name = "Gatekeep";
		MpCost = 45;
		Buff = new Protect(duration: 3, dr: 0.3);
	}
}

public class Girlboss : InflictStatusAbility 
{
	public Girlboss() 
	{
		Name = "Girlboss";
		MpCost = 20;
		Status = new Random(duration: 3);
	}
}

public class GoldenShower : AttackAbility 
{
	public GoldenShower() 
	{
		Name = "Golden Shower";
		MpCost = 0;
		Attack = new MPStealAttack(damage: 0.2, mpStealAmount: 25, hpLossPercent: 0.4);
	}
}
#endregion

#region High Priestess
public class Gnaw : AttackAbility
{
	public Gnaw() 
	{
		Name = "Gnaw";
		MpCost = 15;
		Attack = new MultiHitAttack(damage: 0.6, minHits: 1, 6);
	}
}

public class SpearGun : AttackAbility 
{
	public SpearGun() 
	{
		Name = "Spear Gun";
		MpCost = 25;
		Attack = new BasicAttack(damage: 2.5);
	}
}

public class BiteBiteBite : StatusAttackAbility 
{
	public BiteBiteBite() 
	{
		Name = "Bite";
		MpCost = 35;
		Attack = new BasicAttack(damage: 2);
		Status = new Bleed(duration: 3, applyChance: 0.4);
	}
}

public class Assimilate : BuffAbility 
{
	public Assimilate() 
	{
		Name = "Assimilate";
		MpCost = 50;
		Buff = new Await();
	}
}
#endregion

#region Geb
public class WaterSlash : StatusAttackAbility 
{
	public WaterSlash() 
	{
		Name = "Water Slash";
		MpCost = 25;
		Attack = new BasicAttack(damage: 2);
		Status = new Bleed(duration: 4, applyChance: 0.5);
	}
}

public class WaterBullets : AttackAbility 
{
	public WaterBullets() 
	{
		Name = "Water Bullets";
		MpCost = 50;
		Attack = new MultiHitAttack(damage: 1, minHits: 3, maxHits: 5);
	}
}

public class SandSonar : BuffAbility 
{
	public SandSonar() 
	{
		Name = "Sand Sonar";
		MpCost = 30;
		Buff = new Protect(duration: 1, dr: 0.8);
	}
}

public class SurpriseAttack : BuffAbility 
{
	public SurpriseAttack() 
	{
		Name = "Surprise Attack";
		MpCost = 40;
		Buff = new Charge(duration: 1);
	}
}
#endregion

#region Khnum
public class FlailAbout : AttackAbility 
{
	public FlailAbout() 
	{
		Name = "Flail Helplessly";
		MpCost = 30;
		Attack = new MultiHitAttack(damage: 0.2, minHits: 8, maxHits: 12);
	}
}

public class Smoke4Ciggys : StatChangeAbility 
{
	public Smoke4Ciggys() 
	{
		Name = "Smoke 4 Ciggys";
		MpCost = 50;
		StatChange = new Heal(healPercent: 0.15);
	}
}

public class SweatNervously : BuffAbility 
{
	public SweatNervously() 
	{
		Name = "Sweat Nervously";
		MpCost = 20;
		Buff = new Charge(duration: 1);
	}
}

public class OrangeBomb : InflictStatusAbility 
{
	public OrangeBomb() 
	{
		Name = "Orange Bomb 🍊";
		MpCost = 65;
		Status = new Doom(duration: 10);
	}
}
#endregion

#region Anubis
public class DarkSlash : StatusAttackAbility 
{
	public DarkSlash() 
	{
		Name = "Dark Slash";
		MpCost = 25;
		Attack = new MultiHitAttack(damage: 1.2, minHits: 1, maxHits: 4);
		Status = new Bleed(duration: 5, applyChance: 0.3);
	}
}

public class CombatDevelopment : StatChangeAbility 
{
	public CombatDevelopment() 
	{
		Name = "Combat Development";
		MpCost = 45;
		StatChange = new Strength(increase: 0.25);
	}
}

public class PhaseSlash : AttackAbility 
{
	public PhaseSlash() 
	{
		Name = "Phase Slash";
		MpCost = 35;
		Attack = new BypassProtectAttack(damage: 3);
	}
}

public class SwitchHosts : StatChangeAbility 
{
	public SwitchHosts() 
	{
		Name = "Switch Hosts";
		MpCost = 60;
		StatChange = new Heal(healPercent: 0.6);
	}
}
#endregion

#region Bastet
public class NutsNBolts : AttackAbility
{
	public NutsNBolts() 
	{
		Name = "Nuts n' Bolts";
		MpCost = 25;
		Attack = new MultiHitAttack(damage: 0.15, minHits: 15, maxHits: 30);
	}
}

public class OutletElectrocution : StatusAttackAbility
{
	public OutletElectrocution() 
	{
		Name = "Outlet Electrocution";
		MpCost = 45;
		Attack = new BasicAttack(damage: 1.5);
		Status = new Shock(duration: 3, applyChance: 0.75);
	}
}

public class Magnetism : AttackAbility 
{
	public Magnetism() 
	{
		Name = "Magnetism";
		MpCost = 35;
		Attack = new TakeoverAttack(damage: 2.5);
	}
}

public class ChargeTheyPhone : BuffAbility
{
	public ChargeTheyPhone() 
	{
		Name = "Charge They Phone";
		MpCost = 50;
		Buff = new Charge(duration: 2);
	}
}
#endregion

#region Sethan
public class AgeRegression : StatChangeAbility 
{
	public AgeRegression() 
	{
		Name = "Age Regression";
		MpCost = 50;
		StatChange = new Regress(decrease: 0.25);
	}
}

public class HatchetStrike : AttackAbility 
{
	public HatchetStrike() 
	{
		Name = "Hatchet Strike";
		MpCost = 15;
		Attack = new BasicAttack(damage: 2);
	}
}

public class Predation : InflictStatusAbility 
{
	public Predation() 
	{
		Name = "Predation";
		MpCost = 35;
		Status = new Frail(duration: 4, applyChance: 0.5);
	}
}

public class ShadowReflection : AttackAbility 
{
	public ShadowReflection() 
	{
		Name = "Shadow Reflection";
		MpCost = 30;
		Attack = new WeaknessAttack(damage: 1.2, increase: 3, status: typeof(Frail));
	}
}
#endregion

#region Osiris
public class DiceRoll : AttackAbility 
{
	public DiceRoll() 
	{
		Name = "Dice Roll";
		MpCost = 25;
		Attack = new CritDamageIncreaseAttack(damage: 1, increase: 4);
	}
}

public class SleightOfHand : StatusAttackAbility 
{
	public SleightOfHand() 
	{
		Name = "Sleight of Hand";
		MpCost = 50;
		Attack = new CritDamageIncreaseAttack(damage: 0.5, increase: 3);
		Status = new Random(duration: 4, applyChance: 1);
	}
}

public class SoulSiphon : AttackAbility 
{
	public SoulSiphon() 
	{
		Name = "Soul Siphon";
		MpCost = 0;
		Attack = new MPStealAttack(damage: 0.2, mpStealAmount: 20, hpLossPercent: 0.15);
	}
}

public class IdleDeathGamble : StatusAttackAbility 
{
	public IdleDeathGamble() 
	{
		Name = "Idle Death Gamble";
		MpCost = 10;
		Attack = new BasicAttack(damage: 0.5);
		Status = new Doom(duration: 1, applyChance: 0.10);
	}
}
#endregion

#region Horus
public class FreezeAttack : AttackAbility 
{
	public FreezeAttack() 
	{
		Name = "Freeze";
		MpCost = 20;
		Attack = new BypassProtectAttack(damage: 2);
	}
}

public class IcicleBarrage : AttackAbility 
{
	public IcicleBarrage() 
	{
		Name = "Icicle Barrage";
		MpCost = 35;
		Attack = new MultiHitAttack(damage: 0.5, minHits: 3, maxHits: 6);
	}
}

public class IceWall : BuffAbility 
{
	public IceWall()
	{
		Name = "Ice Wall";
		MpCost = 40;
		Buff = new Protect(duration: 3, dr: 0.4);
	}
}

public class IceBlock : AttackAbility 
{
	public IceBlock() 
	{
		Name = "Ice Block";
		MpCost = 50;
		Attack = new CritChanceIncreaseAttack(damage: 2.5, increase: 0.3);
	}
}
#endregion

#region Atum
public class FMega : StatusAttackAbility 
{
	public FMega() 
	{
		Name = "F-MEGA";
		MpCost = 25;
		Attack = new BasicAttack(damage: 1.2);
		Status = new Shock(duration: 2, applyChance: 0.6);
	}
}

public class OhThatsABaseball : AttackAbility 
{
	public OhThatsABaseball() 
	{
		Name = "OH! THAT'S A BASEBALL";
		MpCost = 60;
		Attack = new MultiHitAttack(damage: 0.2, minHits: 1, maxHits: 30);
	}
}

public class SoulRead : BuffAbility 
{
	public SoulRead() 
	{
		Name = "Soul Read";
		MpCost = 50;
		Buff = new Await();
	}
}
#endregion

#region Cream
public class ThroatChop : AttackAbility 
{
	public ThroatChop() 
	{
		Name = "Throat Chop";
		MpCost = 30;
		Attack = new BasicAttack(damage: 2.5);
	}
}

public class Blackhole : AttackAbility 
{
	public Blackhole() 
	{
		Name = "Blackhole";
		MpCost = 45;
		Attack = new CritDamageIncreaseAttack(damage: 3, increase: 1);
	}
}

public class DimensionWarp : BuffAbility 
{
	public DimensionWarp() 
	{
		Name = "Dimension Warp";
		MpCost = 50;
		Buff = new Await();
	}
}

public class CreamInvisibility : InflictStatusAbility 
{
	public CreamInvisibility() 
	{
		Name = "Invisibility";
		MpCost = 25;
		Status = new Confusion(duration: 2, applyChance: 0.6);
	}
}
#endregion

#region Holly's Stand
public class Debilitate : AttackAbility 
{
	public Debilitate() 
	{
		Name = "Debilitate";
		MpCost = 10;
		Attack = new MPStealAttack(damage: 1, mpStealAmount: 5, hpLossPercent: 0.1);
	}
}
#endregion