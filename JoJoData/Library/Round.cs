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
		
		if (CurrentPlayer.Mp < BattleConstants.BASE_MP / 2)
		{
			CurrentPlayer.GrantMP(BattleConstants.LOW_MP_GAIN);
		}

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
				
			}
			else if (CurrentPlayer.Status is TurnSkipStatus tsStatus)
			{
				battleMsgs.Add(tsStatus.Execute(Opponent, CurrentPlayer));
				RoundSkipped = true;
				return;
			}
		}
		
		if (CurrentPlayer.Buff is not null) 
		{
			battleMsgs.Add(CurrentPlayer.Buff.Execute(CurrentPlayer));
		}

		if (Opponent.Buff != null && Opponent.Buff is Protect protect)
		{
			protect.GrantProtect(Opponent);
		}
		
		if (Opponent.Status != null && Opponent.Status is Weak weak) 
		{
			weak.Weaken(Opponent);
		}
	}

	public void Execute(Ability ability, out List<DiscordMessageBuilder?> battleMsgs)
	{
		battleMsgs =
		[
			new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithDescription($"🌟 {CurrentPlayer.Stand!.CoolName} ({CurrentPlayer.User.Mention}) uses {ability.Name}!")
				.WithColor(DiscordColor.Gold))
		];
		CurrentPlayer.UseMP(ability.MpCost);
		
		if (ability is AttackAbility attack)
		{
			if (Opponent.Buff is Await await && Opponent.StatusDuration == 2)
			{

			}
			
			battleMsgs.Add(attack.Attack.Execute(CurrentPlayer, Opponent));
			if (!Opponent.IsAlive)
				return;

			if (Opponent.Status is Sleep sleep)
			{
				battleMsgs.Add(sleep.RollForWakeUp(Opponent));
			}

			if (ability is StatusAttackAbility statusAttk) 
			{
				battleMsgs.Add(statusAttk.Status.TryApply(CurrentPlayer, Opponent));
			}
			
			if (ability is BuffAttackAbility buffAtk) 
			{
				battleMsgs.Add(buffAtk.Buff.Apply(CurrentPlayer));
			}
		}

		if (ability is InflictStatusAbility status)
		{
			battleMsgs.Add(status.Status.TryApply(CurrentPlayer, Opponent));
		}

		if (ability is BuffAbility buff)
		{
			battleMsgs.Add(buff.Buff.Apply(CurrentPlayer));
		}

		if (ability is StatChangeAbility statChange)
		{
			battleMsgs.Add(statChange.StatChange.Execute(CurrentPlayer));
		}
	}
	
	public bool AbilitySelectCheck(Ability ability, DiscordClient s, out DiscordMessageBuilder msg) 
	{
		msg = new DiscordMessageBuilder();
		
		if (CurrentPlayer.Mp < ability.MpCost) 
		{
			msg = new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithDescription($"{DiscordEmoji.FromName(s, ":x:", false)} Not enough MP!")
				.WithColor(DiscordColor.Red));
			return false;
		}
		
		if (CurrentPlayer.Status is Silence silence) 
		{
			msg = silence.SilenceMessage(CurrentPlayer);
			return false;
		}

		return true;
	}
	#endregion
}