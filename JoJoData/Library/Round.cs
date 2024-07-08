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
		
		if (CurrentPlayer.Mp < BattleConstants.LOW_MP_THRESHOLD && CurrentPlayer.Status is not TurnSkipStatus)
		{
			CurrentPlayer.GrantMP(BattleConstants.LOW_MP_GAIN, out int mpBefore);
			battleMsgs.Add(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(CurrentPlayer.User.GlobalName, "", CurrentPlayer.User.AvatarUrl)
				.WithDescription("**Low MP Regen**")
				.WithColor(DiscordColor.Aquamarine)
				.WithFooter($"💎 {mpBefore} ➡️ 💎 {CurrentPlayer.Mp}")));
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
	}

	public void Execute(Ability ability, out List<DiscordMessageBuilder?> battleMsgs)
	{
		battleMsgs =
		[
			new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
				.WithAuthor(CurrentPlayer.User.GlobalName, "", CurrentPlayer.User.AvatarUrl)
				.WithDescription($"🌟 **{CurrentPlayer.Stand!.CoolName} uses {ability.CoolName}!**")
				.WithColor(DiscordColor.Gold))
		];
		CurrentPlayer.UseMP(ability.MpCost, out _);
		
		if (ability is AttackAbility attack)
		{
			if (Opponent.Buff is Await awaitt && Opponent.StatusDuration == 2)
			{
				battleMsgs.Add(awaitt.Action(caster: Opponent, target: CurrentPlayer, out bool evade));
				if (evade) return;
			}
			
			if (CurrentPlayer.Status is Confusion confusion && confusion.RollConfusion()) 
			{
				// use your attack against yourself
				battleMsgs.Add(attack.Attack.Execute(attacker: CurrentPlayer, defender: CurrentPlayer));
				if (!CurrentPlayer.IsAlive)
					return;
			}
			else 
			{
				battleMsgs.Add(attack.Attack.Execute(attacker: CurrentPlayer, defender: Opponent));
				if (!Opponent.IsAlive)
					return;
			}
			
			if (CurrentPlayer.Status is Shock shock) 
			{
				battleMsgs.Add(shock.Electrocute(Opponent, CurrentPlayer));
				if (!CurrentPlayer.IsAlive)
					return;
			}

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

		if (Opponent.Buff is Await await && Opponent.StatusDuration == 1)
		{
			battleMsgs.Add(await.Action(caster: Opponent, target: CurrentPlayer, out bool evade));
			if (evade) return;
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

		return true;
	}
	#endregion
}