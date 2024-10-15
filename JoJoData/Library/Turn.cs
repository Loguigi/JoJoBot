using DSharpPlus;
using DSharpPlus.Entities;
using JoJoData.Abilities;
using JoJoData.Data;

namespace JoJoData.Library;

public class Turn
{
	#region Properties
	public List<DiscordMessageBuilder?> BattleLog { get; private set; } = [];
	public BattlePlayer CurrentPlayer { get; }
	public BattlePlayer Opponent { get; }
	public BattlePlayer Caster { get; set; }
	public BattlePlayer Target { get; set; }
	public Ability? Ability { get; private set; }
	public bool RoundRepeat { get; set; }
	#endregion
	
	#region Events
	public event EventHandler<AbilityCastEventArgs>? AbilityCast;
	public event EventHandler<PreCurrentTurnEventArgs>? PreCurrentTurn;
	public event EventHandler<PreEnemyTurnEventArgs>? PreEnemyTurn;
	public event EventHandler<PostCurrentTurnEventArgs>? PostCurrentTurn;
	public event EventHandler<BeforeAttackedEventArgs>? BeforeAttacked;
	public event EventHandler<AfterAttackedEventArgs>? AfterAttacked;
	public event EventHandler<DeathFlagEventArgs>? DeathFlag; 
	#endregion
	
	#region Public Methods
	public Turn(BattlePlayer currentPlayer, BattlePlayer opponent) 
	{
		CurrentPlayer = Caster = currentPlayer;
		Opponent = Target = opponent;
		
		CurrentPlayer.Status?.RegisterEvents(this);
		CurrentPlayer.Buff?.RegisterEvents(this);
		CurrentPlayer.Stand!.Passive?.RegisterEvents(this);
		Opponent.Status?.RegisterEvents(this);
		Opponent.Buff?.RegisterEvents(this);
		Opponent.Stand!.Passive?.RegisterEvents(this);
	}

	public void OnAbilityCast(AbilityCastEventArgs e)
	{
		Ability = e.Ability;
		BattleLog = [];
		// Check MP
		if (CurrentPlayer.Mp < e.Ability.MpCost)
		{
			e.OutputMessage = new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(CurrentPlayer.User.GlobalName, "", CurrentPlayer.User.AvatarUrl)
				.WithDescription($"❌ Not enough MP!")
				.WithColor(DiscordColor.Red));
			e.IsValidCast = false;
			return;
		}
		
		// Check cooldowns
		if (CurrentPlayer.Cooldowns[e.Ability] > 0) 
		{
			e.OutputMessage = new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(CurrentPlayer.User.GlobalName, "", CurrentPlayer.User.AvatarUrl)
				.WithDescription($"❌ On cooldown for `{CurrentPlayer.Cooldowns[e.Ability]}` turns!")
				.WithColor(DiscordColor.Red));
			e.IsValidCast = false;
			return;
		}
		
		// Check ability requirements
		if (e.Ability.Requirement is not null) 
		{
			if (e.Ability.Requirement.Check(CurrentPlayer, Opponent, out var requirementMsg)) 
			{
				e.IsValidCast = true;
			}
			else 
			{
				e.OutputMessage = requirementMsg;
				e.IsValidCast = false;
				return;
			}
		}
		
		// Invokes all IBattleEffect related events
		AbilityCast?.Invoke(this, e);
	}
	
	public void OnBeforeAttacked(BeforeAttackedEventArgs e) => BeforeAttacked?.Invoke(this, e);
	
	public void OnAfterAttacked(AfterAttackedEventArgs e) => AfterAttacked?.Invoke(this, e);
	
	public void OnDeathFlag(DeathFlagEventArgs e) => DeathFlag?.Invoke(this, e);
	#endregion

	#region Turn Control Methods
	public void PreCheck()
	{
		try
		{
			BattleLog = [];

			// Low MP gain; does not work while time is stopped
			if (PlayerHasLowMP())
			{
				CurrentPlayer.GrantMP(BattleConstants.LOW_MP_GAIN, out int mpBefore);
				BattleLog.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
					.WithAuthor(CurrentPlayer.User.GlobalName, "", CurrentPlayer.User.AvatarUrl)
					.WithDescription("**Low MP Regen**")
					.WithColor(DiscordColor.Aquamarine)
					.WithFooter($"💎 {mpBefore} ➡️ 💎 {CurrentPlayer.Mp}")));
			}

			PreCurrentTurnEventArgs preCurrent = new(CurrentPlayer, this);
			PreEnemyTurnEventArgs preEnemy = new(Opponent, this);
			PreCurrentTurn?.Invoke(this, preCurrent);
			PreEnemyTurn?.Invoke(this, preEnemy);

			ReduceCooldowns();
		}
		catch (TurnSkipException)
		{
			PostCurrentTurnEventArgs postCurrent = new(CurrentPlayer, this);
			PostCurrentTurn?.Invoke(this, postCurrent);
			throw;
		}
	}

	public void Execute(Ability ability)
	{
		// Send name of ability used
		BattleLog =
		[
			new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(CurrentPlayer.User.GlobalName, "", CurrentPlayer.User.AvatarUrl)
				.WithDescription($"🌟 **{CurrentPlayer.Stand!.CoolName} uses {ability.CoolName}!**")
				.WithColor(DiscordColor.Gold))
		];
		
		// Deduct ability MP and reset any cooldown
		CurrentPlayer.UseMP(ability.MpCost, out _);
		CurrentPlayer.Cooldowns[ability] = ability.Cooldown;
		ability.Use(this);
		
		PostCurrentTurnEventArgs postCurrent = new(CurrentPlayer, this);
		PostCurrentTurn?.Invoke(this, postCurrent);
	}
	#endregion

	#region Private Methods
	private bool PlayerHasLowMP() => CurrentPlayer.Mp < BattleConstants.LOW_MP_THRESHOLD && CurrentPlayer.Status is not TimeStop;
	
	private void ReduceCooldowns()
	{
		foreach (var ability in CurrentPlayer.Cooldowns.Where(x => x.Value != 0)) 
		{
			CurrentPlayer.Cooldowns[ability.Key]--;
		}
	}
	#endregion
}