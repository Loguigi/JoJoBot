using DSharpPlus;
using DSharpPlus.Entities;
using JoJoData.Abilities;
using JoJoData.Data;

namespace JoJoData.Library;

public class Round(BattlePlayer currentPlayer, BattlePlayer opponent)
{
	#region Properties
	public BattlePlayer CurrentPlayer { get; protected set; } = currentPlayer;
	public BattlePlayer Opponent { get; protected set; } = opponent;
	public bool RoundSkipped { get; set; } = false;
	public bool RoundRepeat { get; set; } = false;
	#endregion

	#region Round Control Methods
	public void PreCheck(out List<DiscordMessageBuilder?> battleMsgs)
	{
		battleMsgs = [];
		
		// Low MP gain; does not work while time is stopped
		if (CurrentPlayer.Mp < BattleConstants.LOW_MP_THRESHOLD && CurrentPlayer.Status is not TimeStop)
		{
			CurrentPlayer.GrantMP(BattleConstants.LOW_MP_GAIN, out int mpBefore);
			battleMsgs.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(CurrentPlayer.User.GlobalName, "", CurrentPlayer.User.AvatarUrl)
				.WithDescription("**Low MP Regen**")
				.WithColor(DiscordColor.Aquamarine)
				.WithFooter($"💎 {mpBefore} ➡️ 💎 {CurrentPlayer.Mp}")));
		}

		// Perform negative status action and reduce status duration
		if (CurrentPlayer.Status is not null)
		{
			if (CurrentPlayer.Status is DamageStatus dStatus)
			{
				battleMsgs.Add(dStatus.Execute(Opponent, CurrentPlayer));
				if (!CurrentPlayer.IsAlive)
					return;
			}
			else if (CurrentPlayer.Status is PassiveStatus pStatus)
			{
				battleMsgs.Add(pStatus.Execute(Opponent, CurrentPlayer));
			}
			else if (CurrentPlayer.Status is TurnSkipStatus tsStatus)
			{
				battleMsgs.Add(tsStatus.Execute(Opponent, CurrentPlayer));
				if (CurrentPlayer.Status is not null) 
				{
					RoundSkipped = true;
					return;
				}
			}
		}
		
		// Activate buff and reduce buff duration
		if (CurrentPlayer.Buff is not null) 
		{
			battleMsgs.Add(CurrentPlayer.Buff.Execute(CurrentPlayer));
		}

		if (Opponent.Buff != null && Opponent.Buff is Protect protect)
		{
			protect.GrantProtect(Opponent);
		}
		
		if (Opponent.Status != null && Opponent.Status is Frail frail) 
		{
			frail.Weaken(Opponent);
		}
		
		foreach (var ability in CurrentPlayer.Cooldowns.Where(x => x.Value != 0)) 
		{
			CurrentPlayer.Cooldowns[ability.Key]--;
		}
	}

	public void Execute(Ability ability, out List<DiscordMessageBuilder?> battleMsgs)
	{
		// Send name of ability used
		battleMsgs =
		[
			new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(CurrentPlayer.User.GlobalName, "", CurrentPlayer.User.AvatarUrl)
				.WithDescription($"🌟 **{CurrentPlayer.Stand!.CoolName} uses {ability.CoolName}!**")
				.WithColor(DiscordColor.Gold))
		];
		
		// Deduct ability MP and reset any cooldown
		CurrentPlayer.UseMP(ability.MpCost, out _);
		CurrentPlayer.Cooldowns[ability] = ability.Cooldown;
		
		if (ability is AttackAbility attack)
		{
			if (Opponent.Buff is Await awaitt && awaitt.IsEvadeTurn(Opponent))
			{
				battleMsgs.Add(awaitt.Action(caster: Opponent, target: CurrentPlayer, out bool evade));
				if (evade) return;
			}
			
			if (CurrentPlayer.Status is Confusion confusion && confusion.RollConfusion()) // Confused Attack: damage yourself, apply status to yourself, buff enemy
			{
				// use your attack against yourself
				battleMsgs.Add(attack.Attack.Execute(attacker: CurrentPlayer, defender: CurrentPlayer));
				if (!CurrentPlayer.IsAlive)
					return;

				if (ability is StatusAttackAbility statusAttk)
				{
					battleMsgs.Add(statusAttk.Status.TryApply(caster: CurrentPlayer, target: CurrentPlayer));
				}

				if (ability is BuffAttackAbility buffAtk)
				{
					battleMsgs.Add(buffAtk.Buff.Apply(target: Opponent));
				}
			}
			else // Normal Attack
			{
				battleMsgs.Add(attack.Attack.Execute(attacker: CurrentPlayer, defender: Opponent));
				if (!Opponent.IsAlive)
					return;

				if (Opponent.Status is Sleep sleep)
				{
					battleMsgs.Add(sleep.RollForWakeUp(target: Opponent));
				}

				if (ability is StatusAttackAbility statusAttk)
				{
					battleMsgs.Add(statusAttk.Status.TryApply(caster: CurrentPlayer, target: Opponent));
				}

				if (ability is BuffAttackAbility buffAtk)
				{
					battleMsgs.Add(buffAtk.Buff.Apply(target: CurrentPlayer));
				}
			}
			
			if (CurrentPlayer.Status is Shock shock)
			{
				battleMsgs.Add(shock.Electrocute(Opponent, CurrentPlayer));
				if (!CurrentPlayer.IsAlive)
					return;
			}
		}

		if (ability is InflictStatusAbility status)
		{
			battleMsgs.Add(status.Status.TryApply(caster: CurrentPlayer, target: Opponent));
		}

		if (ability is BuffAbility buff)
		{
			battleMsgs.Add(buff.Buff.Apply(target: CurrentPlayer));
		}

		if (ability is StatChangeAbility statChange)
		{
			battleMsgs.Add(statChange.StatChange.Execute(caster: CurrentPlayer, target: Opponent));
		}

		if (Opponent.Buff is Await a && a.IsAttackTurn(Opponent))
		{
			battleMsgs.Add(a.Action(caster: Opponent, target: CurrentPlayer, out _));
		}

		if (CurrentPlayer.Buff is Haste) 
		{
			RoundRepeat = true;
		}
	}
	
	public bool AbilitySelectCheck(Ability ability, DiscordClient s, out DiscordMessageBuilder msg) 
	{
		msg = new DiscordMessageBuilder();
		
		if (CurrentPlayer.Mp < ability.MpCost) 
		{
			msg = new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(CurrentPlayer.User.GlobalName, "", CurrentPlayer.User.AvatarUrl)
				.WithDescription($"{DiscordEmoji.FromName(s, ":x:", false)} Not enough MP!")
				.WithColor(DiscordColor.Red));
			return false;
		}
		
		if (CurrentPlayer.Status is Silence silence && ability.MpCost > 0)
		{
			msg = silence.SilenceMessage(CurrentPlayer);
			return false;
		}
		
		if (CurrentPlayer.Cooldowns[ability] > 0) 
		{
			msg = new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(CurrentPlayer.User.GlobalName, "", CurrentPlayer.User.AvatarUrl)
				.WithDescription($"{DiscordEmoji.FromName(s, ":x:", false)} On cooldown for `{CurrentPlayer.Cooldowns[ability]}` turns!")
				.WithColor(DiscordColor.Red));
			return false;
		}

		return true;
	}
	#endregion
}