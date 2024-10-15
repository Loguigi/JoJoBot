using JoJoData.Library;
using Barrier = JoJoData.Library.Barrier;
using Random = JoJoData.Library.Random;

namespace JoJoData.Abilities;

#region Crazy Diamond
public class Dora : AttackAbility 
{
	public Dora() 
	{
		Name = "DORA!!";
		MpCost = 25;
		Attack = new BasicAttack(damage: 1.5);
	}
}

public class DoraOraOra : AttackAbility
{
	public DoraOraOra() 
	{
		Name = "DORAORAORAORA!!";
		MpCost = 40;
		Attack = new MultiHitAttack(damage: 0.55, minHits: 2, maxHits: 8);
	}
}

public class WallReconstruction : BuffAbility 
{
	public WallReconstruction() 
	{
		Name = "Wall Reconstruction";
		MpCost = 25;
		Buff = new Protect(duration: 3, dr: 0.5);
		Cooldown = 6;
	}
}

public class ObjectFusion : StatusAttackAbility 
{
	public ObjectFusion() 
	{
		Name = "Object Fusion";
		MpCost = 50;
		Attack = new MultiHitAttack(damage: 0.2, minHits: 3, maxHits: 9);
		Status = new Silence(duration: 3, applyChance: 0.5);
	}
}
#endregion

#region The Hand
public class DefenseErasure : AttackAbility 
{
	public DefenseErasure() 
	{
		Name = "Defense Erasure";
		MpCost = 30;
		Attack = new BypassProtectAttack(damage: 2.2);
	}
}

public class SpaceErasure : BuffAttackAbility 
{
	public SpaceErasure() 
	{
		Name = "Space Erasure";
		MpCost = 45;
		Attack = new BypassProtectAttack(damage: 1);
		Buff = new Haste(duration: 2);
		Cooldown = 9;
	}
}

public class AttackErasure : BuffAttackAbility 
{
	public AttackErasure() 
	{
		Name = "Attack Erasure";
		MpCost = 50;
		Attack = new BypassProtectAttack(damage: 1.7);
		Buff = new Await(2, 2.5);
		Cooldown = 3;
	}
}

public class UltimateErasure : AttackAbility 
{
	public UltimateErasure() 
	{
		Name = "Ultimate Erasure";
		MpCost = 50;
		Attack = new ErasureAttack(damage: 3.1, critDmgIncrease: 1);
		Cooldown = 6;
	}
}
#endregion

#region Echoes Act 0
public class EggThrow : AttackAbility 
{
	public EggThrow() 
	{
		Name = "Egg Throw";
		MpCost = 15;
		Attack = new BasicAttack(damage: 1.5);
	}
}

public class EggRoll : AttackAbility 
{
	public EggRoll() 
	{
		Name = "Egg Roll";
		MpCost = 30;
		Attack = new MultiHitAttack(damage: 1, minHits: 1, maxHits: 2);
	}
}

public class EggToss : AttackAbility 
{
	public EggToss() 
	{
		Name = "Egg Toss";
		MpCost = 45;
		Attack = new CritChanceIncreaseAttack(damage: 1.3, increase: 0.8);
	}
}

public class EggDunk : AttackAbility 
{
	public EggDunk() 
	{
		Name = "Egg DUNK";
		MpCost = 60;
		Attack = new CritDamageIncreaseAttack(damage: 1.2, increase: 2);
	}
}
#endregion

#region Echoes Act 1
public class IHateYou : AttackAbility 
{
	public IHateYou() 
	{
		Name = "I HATE YOU!";
		MpCost = 25;
		Attack = new BasicAttack(damage: 2.2);
	}
}

public class BelieveInMe : StatChangeAbility 
{
	public BelieveInMe() 
	{
		Name = "Believe in me!";
		MpCost = 45;
		StatChange = new Strength(increase: 0.2);
	}
}

public class Scout : BuffAbility 
{
	public Scout() 
	{
		Name = "Scout";
		MpCost = 30;
		Buff = new Await(3, 1.5);
		Cooldown = 6;
	}
}

public class SoundRush : StatusAttackAbility 
{
	public SoundRush() 
	{
		Name = "🎶 SOUND RUSH 🎶";
		MpCost = 40;
		Attack = new MultiHitAttack(damage: 0.3, minHits: 5, maxHits: 10);
		Status = new Silence(duration: 3, applyChance: 0.6);
	}
}
#endregion

#region Echoes Act 2
public class Whoosh : AttackAbility 
{
	public Whoosh() 
	{
		Name = "Whoosh";
		MpCost = 20;
		Attack = new MultiHitAttack(damage: 0.7, minHits: 1, maxHits: 5);
	}
}

public class Sizzle : InflictStatusAbility
{
	public Sizzle() 
	{
		Name = "Sizzle";
		MpCost = 30;
		Status = new Burn(duration: 3, applyChance: 0.8);
	}
}

public class BoomSound : AttackAbility
{
	public BoomSound()
	{
		Name = "BOOM";
		MpCost = 45;
		Attack = new BypassProtectAttack(damage: 2.7);
	}
}

public class Boing : BuffAbility
{
	public Boing() 
	{
		Name = "Boing";
		MpCost = 60;
		Buff = new Thorns(3, 0.6);
	}
}
#endregion

#region Echoes Act 3

public class ThreeFreeze : BuffAttackAbility
{
	public ThreeFreeze()
	{
		Name = "THREE FREEZE";
		MpCost = 50;
		Attack = new BypassProtectAttack(3);
		Buff = new Haste(2);
		Cooldown = 10;
	}
}

public class KillDaHo : AttackAbility
{
	public KillDaHo()
	{
		Name = "KILL DA HO!!!!";
		MpCost = 40;
		Attack = new CritChanceIncreaseAttack(2, 0.25);
	}
}

public class Bitch : StatusAttackAbility
{
	public Bitch()
	{
		Name = "BEEEEEECH";
		MpCost = 20;
		Attack = new BasicAttack(1.3);
		Status = new Confusion(4, 0.25);
	}
}

public class ReliableGuy : BuffAbility
{
	public ReliableGuy()
	{
		Name = "RELIABLE GUY";
		MpCost = 60;
		Buff = new Protect(4, 0.45);
	}
}
#endregion

#region Heaven's Door

public class BookUnravel : InflictStatusAbility
{
	public BookUnravel()
	{
		Name = "Unravel";
		MpCost = 20;
		Status = new Unravel(3);
		Cooldown = 3;
	}
}

public class SafetyLock : BuffAbility
{
	public SafetyLock()
	{
		Name = "Safety Lock";
		MpCost = 45;
		Buff = new Await(3, 3);
		Requirement = new StatusRequirement(typeof(Unravel));
	}
}

public class FlyBackwards : AttackAbility
{
	public FlyBackwards()
	{
		Name = "Fly Backwards";
		MpCost = 40;
		Attack = new BasicAttack(5);
		Requirement = new StatusRequirement(typeof(Unravel));
	}
}

public class MemoryRemoval : StatChangeAbility
{
	public MemoryRemoval()
	{
		Name = "Memory Removal";
		MpCost = 65;
		StatChange = new Regress(0.35);
		Requirement = new StatusRequirement(typeof(Unravel));
	}
}
#endregion

#region Killer Queen
public class BombCharge : InflictStatusAbility 
{
	public BombCharge() 
	{
		Name = "Bomb Charge";
		MpCost = 20;
		Status = new Charged(3);
	}
}

public class PrimaryBomb : StatusAttackAbility 
{
	public PrimaryBomb() 
	{
		Name = "Primary Bomb";
		MpCost = 35;
		Attack = new DetonateAttack(4);
		Status = new Burn(3, 0.5);
		Requirement = new StatusRequirement(typeof(Charged));
	}
}

public class SheerHeartAttack : AttackAbility 
{
	public SheerHeartAttack() 
	{
		Name = "Sheer Heart Attack";
		MpCost = 40;
		Attack = new WeaknessAttack(0.7, 6, typeof(Burn));
	}
}

public class BitesTheDust : StatChangeAbility 
{
	public BitesTheDust()
	{
		Name = "Bites the Dust";
		MpCost = 45;
		StatChange = new BitesZaDusto(0.6);
		Cooldown = 10;
	}
}
#endregion

#region Aqua Necklace

public class BodyInvasion : StatusAttackAbility
{
	public BodyInvasion()
	{
		Name = "Body Invasion";
		MpCost = 30;
		Attack = new BasicAttack(1.4);
		Status = new Poison(2, 0.6);
	}
}

public class Waterlog : StatusAttackAbility
{
	public Waterlog()
	{
		Name = "Waterlog";
		MpCost = 55;
		Attack = new BasicAttack(1.7);
		Status = new Drown(3, 0.7);
	}
}

public class OrganFlooding : AttackAbility
{
	public OrganFlooding()
	{
		Name = "Organ Flooding";
		MpCost = 45;
		Attack = new CritChanceIncreaseAttack(2, 0.3);
	}
}

public class YoAngelo : StatChangeAbility
{
	public YoAngelo()
	{
		Name = "Yo, Angelo!";
		MpCost = 70;
		StatChange = new Barrier(0.5);
	}
}
#endregion

#region Bad Company
public class CombatKnife : StatusAttackAbility 
{
	public CombatKnife() 
	{
		Name = "Combat Knife";
		MpCost = 20;
		Status = new Bleed(duration: 2, applyChance: 0.35);
		Attack = new MultiHitAttack(damage: 0.55, minHits: 2, maxHits: 4);
	}
}

public class MachineGuns : StatusAttackAbility 
{
	public MachineGuns() 
	{
		Name = "Machine Guns";
		MpCost = 45;
		Status = new Bleed(duration: 2, applyChance: 0.35);
		Attack = new MultiHitAttack(damage: 0.25, minHits: 10, maxHits: 20);
	}
}

public class Missiles : AttackAbility 
{
	public Missiles() 
	{
		Name = "Missles";
		MpCost = 50;
		Attack = new MultiHitAttack(damage: 1.5, minHits: 1, maxHits: 3);
	}
}

public class Redeploy : BuffAbility 
{
	public Redeploy() 
	{
		Name = "Redeploy";
		MpCost = 30;
		Buff = new Haste(2);
		Cooldown = 6;
	}
}
#endregion

#region Red Hot Chili Pepper
public class ElectricityAbsorption : StatChangeAbility
{
	public ElectricityAbsorption() 
	{
		Name = "Electricity Absorption";
		MpCost = 30;
		StatChange = new Strength(increase: 0.25);
		Cooldown = 3;
	}
}

public class ElectricalCharge : BuffAbility 
{
	public ElectricalCharge() 
	{
		Name = "Electrical Charge";
		MpCost = 45;
		Buff = new Charge(duration: 1);
	}
}

public class SuperChargedRush : StatusAttackAbility 
{
	public SuperChargedRush() 
	{
		Name = "Super Charged Rush";
		MpCost = 25;
		Attack = new MultiHitAttack(damage: 0.8, minHits: 3, maxHits: 6);
		Status = new Shock(duration: 3, applyChance: 0.35);
	}
}

public class Electrocution : AttackAbility 
{
	public Electrocution() 
	{
		Name = "Electrocution";
		MpCost = 50;
		Attack = new WeaknessAttack(damage: 0.75, increase: 4, typeof(Shock));
		Cooldown = 5;
	}
}
#endregion

#region The Lock
public class GuiltTrip : StatusAttackAbility 
{
	public GuiltTrip() 
	{
		Name = "Guilt Trip";
		MpCost = 30;
		Attack = new BasicAttack(damage: 1.3);
		Status = new Silence(duration: 3, applyChance: 0.2);
	}
}

public class LieDetector : AttackAbility 
{
	public LieDetector() 
	{
		Name = "Lie Detector";
		MpCost = 35;
		Attack = new CritDamageIncreaseAttack(damage: 1.4, increase: 0.5);
	}
}

public class ExtremeGaslighting : BuffAbility
{
	public ExtremeGaslighting() 
	{
		Name = "Extreme Gaslighting";
		MpCost = 40;
		Buff = new Protect(duration: 4, dr: 0.3);
	}
}

public class Overburden : InflictStatusAbility 
{
	public Overburden() 
	{
		Name = "Overburden";
		MpCost = 50;
		Status = new Doom(duration: 10, applyChance: 0.5);
	}
}
#endregion

#region Surface

#endregion

#region Love Deluxe
public class HairWhip : AttackAbility 
{
	public HairWhip() 
	{
		Name = "Hair Whip";
		MpCost = 25;
		Attack = new MultiHitAttack(damage: 0.8, minHits: 1, maxHits: 3);
	}
}

public class HairTangle : StatusAttackAbility 
{
	public HairTangle() 
	{
		Name = "Hair Tangle";
		MpCost = 50;
		Attack = new BasicAttack(damage: 2);
		Status = new Silence(duration: 2, applyChance: 0.75);
	}
}

public class HairBurn : InflictStatusAbility 
{
	public HairBurn() 
	{
		Name = "Hair Burn";
		MpCost = 30;
		Status = new Burn(duration: 2, applyChance: 0.75);
	}
}

public class HairGrowth : StatChangeAbility 
{
	public HairGrowth() 
	{
		Name = "Hair Growth";
		MpCost = 35;
		StatChange = new Barrier(barrierAmount: 0.3);
		Cooldown = 3;
	}
}
#endregion

#region Pearl Jam

#endregion

#region Achtung Baby

public class Nibble : AttackAbility
{
	public Nibble()
	{
		Name = "Nibble";
		MpCost = 20;
		Attack = new MultiHitAttack(0.05, 2, 10);
	}
}

public class CrawlAway : BuffAbility
{
	public CrawlAway()
	{
		Name = "Crawl Away";
		MpCost = 20;
		Buff = new Haste(2);
		Cooldown = 5;
	}
}

public class InvisibleDemonBaby : BuffAbility
{
	public InvisibleDemonBaby()
	{
		Name = "Invisible Demon Baby";
		MpCost = 60;
		Buff = new Await(3, 5);
		Cooldown = 15;
	}
}

public class Cry : StatChangeAbility
{
	public Cry()
	{
		Name = "Cry";
		MpCost = 20;
		StatChange = new Heal(0.1);
	}
}
#endregion

#region Ratt
public class DartShot : AttackAbility 
{
	public DartShot() 
	{
		Name = "Dart Shot";
		MpCost = 25;
		Attack = new CritChanceIncreaseAttack(damage: 1.4, increase: 0.3);
	}
}

public class FleshMelt : StatusAttackAbility 
{
	public FleshMelt()
	{
		Name = "Flesh Melt";
		MpCost = 50;
		Attack = new CritDamageIncreaseAttack(1.2, 0.5);
		Status = new Poison(3, 0.5);
	}
}

public class MeatCube : InflictStatusAbility 
{
	public MeatCube() 
	{
		Name = "Meat Cube";
		MpCost = 35;
		Status = new Silence(duration: 2, applyChance: 0.75);
		Cooldown = 5;
	}
}

public class TheresAnotherRat : BuffAbility 
{
	public TheresAnotherRat() 
	{
		Name = "There's another rat!";
		MpCost = 40;
		Buff = new Charge(duration: 1);
		Cooldown = 8;
	}
}
#endregion

#region Harvest
public class HeeHee : AttackAbility 
{
	public HeeHee() 
	{
		Name = "Hee hee!";
		MpCost = 45;
		Attack = new MultiHitAttack(damage: 0.02, minHits: 1, maxHits: 500);
	}
}

public class FoundIt : AttackAbility 
{
	public FoundIt() 
	{
		Name = "Found it!";
		MpCost = 0;
		Attack = new MPStealAttack(damage: 2, mpStealAmount: 20, hpLossPercent: 0.05);
		Cooldown = 4;
	}
}

public class Stinger : StatusAttackAbility 
{
	public Stinger() 
	{
		Name = "Stinger";
		MpCost = 30;
		Attack = new BasicAttack(damage: 1);
		Status = new Poison(duration: 4, applyChance: 0.45);
	}
}

public class HarvestSuperSpeed : BuffAbility 
{
	public HarvestSuperSpeed() 
	{
		Name = "Harvest Super Speed";
		MpCost = 60;
		Buff = new Await(2, 2);
		Cooldown = 4;
	}
}
#endregion

#region Cinderella

public class PlasticSurgery : StatChangeAbility
{
	public PlasticSurgery()
	{
		Name = "Plastic Surgery";
		MpCost = 30;
		StatChange = new Strength(0.2);
		Cooldown = 4;
	}
}

public class EyeThief : StatusAttackAbility
{
	public EyeThief()
	{
		Name = "Eye Thief";
		MpCost = 40;
		Attack = new WeaknessAttack(1.6, 1.5, typeof(Blind));
		Status = new Blind(3, 0.45);
	}
}

public class MouthThief : StatusAttackAbility
{
	public MouthThief()
	{
		Name = "Mouth Thief";
		MpCost = 60;
		Attack = new WeaknessAttack(1.6, 1.5, typeof(Silence));
		Status = new Silence(3, 0.45);
	}
}

public class BodyReplacement : StatChangeAbility
{
	public BodyReplacement()
	{
		Name = "Body Replacement";
		MpCost = 25;
		StatChange = new Regress(0.1);
	}
}
#endregion

#region Atom Heart Father
public class PhotoCapture : InflictStatusAbility
{
	public PhotoCapture()
	{
		Name = "Photo Capture";
		MpCost = 20;
		Status = new Capture(2);
	}
}

public class PhotoTear : AttackAbility
{
	public PhotoTear()
	{
		Name = "Photo Tear";
		MpCost = 35;
		Attack = new BasicAttack(2.4);
		Requirement = new StatusRequirement(typeof(Capture));
	}
}

public class PhotoJump : BuffAbility
{
	public PhotoJump()
	{
		Name = "Photo Jump";
		MpCost = 50;
		Buff = new Haste(2);
		Requirement = new StatusRequirement(typeof(Capture));
	}
}

public class PhotoImmolation : AttackAbility
{
	public PhotoImmolation()
	{
		Name = "Photo Immolation";
		MpCost = 50;
		Attack = new CritDamageIncreaseAttack(3, 1);
		Requirement = new StatusRequirement(typeof(Capture));
		Cooldown = 5;
	}
}
#endregion

#region Boy II Man
public class Rock : StatusAttackAbility 
{
	public Rock() 
	{
		Name = "ROCK";
		MpCost = 35;
		Attack = new WeaknessAttack(damage: 2.5, increase: 1.2, typeof(Bleed));
		Status = new Confusion(duration: 3, applyChance: 0.25);
	}
}

public class Scissors : StatusAttackAbility 
{
	public Scissors() 
	{
		Name = "SCISSORS";
		MpCost = 35;
		Attack = new WeaknessAttack(damage: 2, increase: 2, typeof(Frail));
		Status = new Bleed(duration: 3, applyChance: 0.5);
	}
}

public class Paper : StatusAttackAbility 
{
	public Paper() 
	{
		Name = "PAPER";
		MpCost = 35;
		Attack = new WeaknessAttack(damage: 1.2, increase: 3, typeof(Confusion));
		Status = new Frail(duration: 3, applyChance: 0.75);
	}
}

public class EnergyTheft : AttackAbility 
{
	public EnergyTheft() 
	{
		Name = "Energy Theft";
		MpCost = 35;
		Attack = new RPSAttack(damage: 3, mpStealAmount: 30, hpLossPercent: 0);
		Cooldown = 3;
	}
}
#endregion

#region Earth Wind and Fire

public class DiceMorph : InflictStatusAbility
{
	public DiceMorph()
	{
		Name = "Dice Morph";
		MpCost = 25;
		Status = new Random(2);
		Cooldown = 5;
	}
}

public class SwordMorph : StatusAttackAbility
{
	public SwordMorph()
	{
		Name = "Sword Morph";
		MpCost = 45;
		Attack = new BasicAttack(1.3);
		Status = new Bleed(4, 0.5);
	}
}

public class SneakerMorph : BuffAbility
{
	public SneakerMorph()
	{
		Name = "Sneaker Morph";
		MpCost = 50;
		Buff = new Await(2, 2);
		Cooldown = 3;
	}
}

public class SirenMeltdown : InflictStatusAbility
{
	public SirenMeltdown()
	{
		Name = "Siren Meltdown";
		MpCost = 25;
		Status = new Silence(2, 0.3);
	}
}
#endregion

#region Highway Star

public class HighwayRush : AttackAbility
{
	public HighwayRush()
	{
		Name = "Highway Rush";
		MpCost = 45;
		Attack = new MultiHitAttack(0.6, 3, 8);
	}
}

public class IllusionRoom : InflictStatusAbility
{
	public IllusionRoom()
	{
		Name = "Illusion Room";
		MpCost = 30;
		Status = new Confusion(4, 0.25);
	}
}

public class LifeforceSiphon : AttackAbility
{
	public LifeforceSiphon()
	{
		Name = "Lifeforce Siphon";
		MpCost = 35;
		Attack = new HPLeechAttack(2);
		Cooldown = 4;
	}
}

public class BodySplit : BuffAbility
{
	public BodySplit()
	{
		Name = "Body Split";
		MpCost = 70;
		Buff = new Haste(2);
		Cooldown = 6;
	}
}
#endregion

#region Stray Cat

public class PlantCamo : AttackAbility
{
	public PlantCamo()
	{
		Name = "Plant Camo";
		MpCost = 35;
		Attack = new CritChanceIncreaseAttack(1.2, 0.5);
	}
}

public class AirBullet : AttackAbility
{
	public AirBullet()
	{
		Name = "Air Bullet";
		MpCost = 50;
		Attack = new CritDamageIncreaseAttack(2, 1);
	}
}

public class AirBarrage : AttackAbility
{
	public AirBarrage()
	{
		Name = "Air Barrage";
		MpCost = 40;
		Attack = new MultiHitAttack(0.6, 3, 5);
	}
}

public class AirShield : BuffAbility
{
	public AirShield()
	{
		Name = "Air Shield";
		MpCost = 60;
		Buff = new Protect(2, 0.6);
	}
}
#endregion

#region Super Fly
public class FiveGWaves : StatusAttackAbility
{
	public FiveGWaves()
	{
		Name = "5G Waves";
		MpCost = 45;
		Attack = new BypassProtectAttack(1.4);
		Status = new Poison(2, 0.75);
	}
}

public class RadioWaves : AttackAbility
{
	public RadioWaves()
	{
		Name = "Radio Waves";
		MpCost = 30;
		Attack = new BypassProtectAttack(3);
	}
}

public class BigBolts : AttackAbility
{
	public BigBolts()
	{
		Name = "Big Bolts";
		MpCost = 60;
		Attack = new MultiHitAttack(2, 1, 2);
	}
}

public class TheSuperFly : BuffAttackAbility
{
	public TheSuperFly()
	{
		Name = "The Super Fly...so epic";
		MpCost = 30;
		Attack = new CritChanceIncreaseAttack(1.2, 0.1);
		Buff = new Thorns(1, 1.5);
		Cooldown = 3;
	}
}
#endregion

#region Enigma

public class UnfoldGun : AttackAbility
{
	public UnfoldGun()
	{
		Name = "Unfold Gun";
		MpCost = 25;
		Attack = new CritChanceIncreaseAttack(1.4, 0.6);
		Cooldown = 2;
	}
}

public class UnfoldFlames : InflictStatusAbility
{
	public UnfoldFlames()
	{
		Name = "Unfold Flames";
		MpCost = 25;
		Status = new Burn(4, 0.9);
		Cooldown = 2;
	}
}

public class UnfoldFreshNoodles : StatChangeAbility
{
	public UnfoldFreshNoodles()
	{
		Name = "Unfold Fresh Noodles";
		MpCost = 50;
		StatChange = new Heal(0.25);
		Cooldown = 2;
	}
}

public class FearTrap : InflictStatusAbility
{
	public FearTrap()
	{
		Name = "Fear Trap";
		MpCost = 25;
		Status = new Doom(7, 0.66);
		Cooldown = 8;
	}
}
#endregion

#region Cheap Trick

#endregion