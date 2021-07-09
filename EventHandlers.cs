using MEC;
using Exiled.Events.EventArgs;
using Exiled.API.Features;
using MurderMystery.API;
using MurderMystery.Extensions;
using MurderMystery.Patches;

namespace MurderMystery
{
    public class EventHandlers
    {
        internal EventHandlers() { }

		// Primary events.
        internal void WaitingForPlayers()
        {
			MurderMystery.Singleton.GamemodeManager.ToggleSecondaryEvents(true);

			CustomInfoPatch.Singleton.Patch(true);
			RoundStartPatch.Singleton.Patch(true);
			RoundSummaryPatch.Singleton.Patch(true);
        }

        internal void RoundStarted()
        {
            if (!MurderMystery.Singleton.GamemodeManager.SecondaryEventsEnabled) { return; }

            MurderMystery.Singleton.GamemodeManager.StartGamemode();
        }

        internal void RoundEnded(RoundEndedEventArgs ev)
        {
            if (!MurderMystery.Singleton.GamemodeManager.Started) { return; }

            MurderMystery.Singleton.GamemodeManager.ToggleGamemode(false);
        }

        internal void RestartingRound()
        {
            if (!MurderMystery.Singleton.GamemodeManager.Started) { return; }

            MurderMystery.Singleton.GamemodeManager.ToggleGamemode(false);
        }


		// Secondary events. (Sent to MMPlayer)
        internal void Verified(VerifiedEventArgs ev)
        {
			// Verifies a player and adds them to the MMPlayer list.
            MMPlayer.List.Add(new MMPlayer(ev.Player).Verified(ev));
			Log.Debug("Player added to list.", MurderMystery.Singleton.LogDebug);
        }

        internal void Destroying(DestroyingEventArgs ev)
        {
			// Destroys a player and removes them from the MMPlayer list.
            MMPlayer.List.Remove(MMPlayer.Get(ev.Player).Destroying(ev));
			Log.Debug("Player removed from list.", MurderMystery.Singleton.LogDebug);
        }

		internal void PickingUpItem(PickingUpItemEventArgs ev)
		{
			MMPlayer.Get(ev.Player).PickingUpItem(ev);
		}

		internal void DroppingItem(DroppingItemEventArgs ev)
		{
			MMPlayer.Get(ev.Player).DroppingItem(ev);
		}

		internal void Shooting(ShootingEventArgs ev)
		{
			MMPlayer.Get(ev.Shooter).Shooting(ev);
		}

		internal void Died(DiedEventArgs ev)
		{
			MMPlayer.Get(ev.Target).Died(ev);
		}

		internal void ChangedRole(ChangedRoleEventArgs ev)
		{
			MMPlayer.Get(ev.Player).ChangedRole(ev);
		}

		internal void SpawningRagdoll(SpawningRagdollEventArgs ev)
		{
			MMPlayer.Get(Player.Get(ev.PlayerId)).SpawningRagdoll(ev);
		}

		internal void Hurting(HurtingEventArgs ev)
		{
			MMPlayer.Get(ev.Target).Hurting(ev);
		}


		// Secondary events. (Not sent to MMPlayer)
		internal void Spawning(SpawningEventArgs ev)
		{
			// Spawns the player in SCP-049's chamber.
			ev.Position = Exiled.API.Extensions.Role.GetRandomSpawnPoint(RoleType.Scp049);
		}

		internal void InteractingLocker(InteractingLockerEventArgs ev)
		{
			// Don't allow locker interaction unless it is a medkit locker.
            if (ev.Chamber.name != "Medkit")
            {
                ev.IsAllowed = false;
            }
		}

		internal void OpeningGenerator(OpeningGeneratorEventArgs ev)
		{
            // Don't allow opening generators.
            ev.IsAllowed = false;
		}

		internal void TriggeringTesla(TriggeringTeslaEventArgs ev)
		{
            // Don't allow tesla activation.
            ev.IsTriggerable = false;
		}

		internal void EnteringFemurBreaker(EnteringFemurBreakerEventArgs ev)
		{
            // Don't allow femur breaker usage.
            ev.IsAllowed = false;
		}

		// Other secondary events.
		internal void EndingRound(EndingRoundEventArgs ev)
		{
			ev.IsAllowed = false;
			ev.IsRoundEnded = false;

			if (Round.ElapsedTime.TotalSeconds <= 5) { return; } // Prevents round from ending before it starts.
			if (MMPlayer.List.Count == 1 && MurderMystery.Singleton.DebugVersion && !MurderMystery.Singleton.GamemodeManager.ForcingRoundEnd) { return; } // If only one player, prevent the game from ending unless forced for debug purposes.

			int innocents = MMPlayer.List.GetRolesCount(MMRole.Innocent, MMRole.Detective);
			int murderers = MMPlayer.List.GetRoleCount(MMRole.Murderer);

			if (innocents > 0 && murderers == 0)
			{
				ev.IsAllowed = true;
				ev.IsRoundEnded = true;
				Map.ClearBroadcasts();
				Map.Broadcast(60, "\n<size=80><color=#00ff00><b>Innocents win</b></color></size>\n<size=30>All murderers have been defeated.</size>");
			}
			else if (innocents == 0 && murderers > 0)
			{
				ev.IsAllowed = true;
				ev.IsRoundEnded = true;
				Map.ClearBroadcasts();
				Map.Broadcast(60, "\n<size=80><color=#ff0000><b>Murderers win</b></color></size>\n<size=30>All innocents have been defeated.</size>");
			}
			else if (innocents == 0 && murderers == 0)
			{
				ev.IsAllowed = true;
				ev.IsRoundEnded = true;
				Map.ClearBroadcasts();
				Map.Broadcast(60, "\n<size=80><color=#7f7f7f><b>Stalemate</b></color></size>\n<size=30>All players have been killed. (magic...? :pog:)</size>");
			}
			else if (MurderMystery.Singleton.GamemodeManager.ForcingRoundEnd)
			{
				ev.IsAllowed = true;
				ev.IsRoundEnded = true;
				Map.ClearBroadcasts();
				Map.Broadcast(60, "\n<size=80><color=#7f7f7f><b>Stalemate</b></color></size>\n<size=30>Round was force ended by an administrator.</size>");
			}
			else if (Round.ElapsedTime.TotalSeconds >= MurderMystery.Singleton.Config.RoundTimeLimit)
			{
				ev.IsAllowed = true;
				ev.IsRoundEnded = true;
				Map.ClearBroadcasts();
				Map.Broadcast(60, "\n<size=80><color=#00ff00><b>Innocents win</b></color></size>\n<size=30>Murderers ran out of time and lost.</size>");
			}
		}

		internal void RespawningTeam(RespawningTeamEventArgs ev)
		{
			ev.MaximumRespawnAmount = 0;
			ev.IsAllowed = false;
		}
    }
}