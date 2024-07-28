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
		Attack = new MultiHitAttack(damage: 0.5, minHits: 2, maxHits: 8);
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
		Attack = new BypassProtectAttack(damage: 1.6);
	}
}

public class SpaceErasure : BuffAttackAbility 
{
	public SpaceErasure() 
	{
		Name = "Space Erasure";
		MpCost = 50;
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
		MpCost = 60;
		Attack = new BypassProtectAttack(damage: 1);
		Buff = new Await();
	}
}

public class UltimateErasure : AttackAbility 
{
	public UltimateErasure() 
	{
		Name = "Ultimate Erasure";
		MpCost = 60;
		Attack = new ErasureAttack(damage: 2.5, critDmgIncrease: 1);
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
		Attack = new BasicAttack(damage: 2);
	}
}

public class BelieveInMe : StatChangeAbility 
{
	public BelieveInMe() 
	{
		Name = "Believe in me!";
		MpCost = 45;
		StatChange = new Strength(increase: 0.15);
	}
}

public class Scout : BuffAbility 
{
	public Scout() 
	{
		Name = "Scout";
		MpCost = 30;
		Buff = new Await();
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
		Status = new Silence(duration: 3, applyChance: 0.3);
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
		Attack = new MultiHitAttack(damage: 0.6, minHits: 1, maxHits: 5);
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
		MpCost = 40;
		Attack = new BypassProtectAttack(damage: 2.7);
	}
}

public class Boing 
{
	public Boing() 
	{
		// TODO boing
	}
}
#endregion

#region Echoes Act 3

#endregion

#region Heaven's Door

#endregion

#region Killer Queen
public class BombCharge : InflictStatusAbility 
{
	public BombCharge() 
	{
		Name = "Bomb Charge";
		MpCost = 25;
		Status = new Charged(duration: 3);
	}
}

public class PrimaryBomb : StatusAttackAbility 
{
	public PrimaryBomb() 
	{
		Name = "Primary Bomb";
		MpCost = 40;
		Attack = new DetonateAttack(damage: 3);
		Status = new Burn(duration: 3, applyChance: 0.5);
	}
}

public class SheerHeartAttack : AttackAbility 
{
	public SheerHeartAttack() 
	{
		Name = "Sheer Heart Attack";
		MpCost = 40;
		Attack = new WeaknessAttack(damage: 0.2, increase: 5, typeof(Burn));
	}
}

public class BitesTheDust : StatChangeAbility 
{
	public BitesTheDust()
	{
		Name = "Bites the Dust";
		MpCost = 45;
		StatChange = new Heal(healPercent: 0.6);
		Cooldown = 10;
	}
}
#endregion

#region Aqua Necklace

#endregion

#region Bad Company

#endregion

#region Red Hot Chili Pepper
public class ElectricityAbsorbtion : StatChangeAbility
{
	public ElectricityAbsorbtion() 
	{
		Name = "Electricity Absorbtion";
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
		StatChange = new Barrier(barrier: 0.4);
		Cooldown = 5;
	}
}
#endregion

#region Pearl Jam

#endregion

#region Achtung Baby

#endregion

#region Ratt

#endregion

#region Harvest

#endregion

#region Cinderella

#endregion

#region Atom Heart Father

#endregion

#region Boy II Man

#endregion

#region Earth Wind and Fire

#endregion

#region Highway Star

#endregion

#region Stray Cat

#endregion

#region Super Fly

#endregion

#region Enigma

#endregion

#region Cheap Trick

#endregion