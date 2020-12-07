using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using MurderMystery.Enums;
using MurderMystery.Utils;
using System.Collections.Generic;
using Handlers = Exiled.Events.Handlers;
using CustomPlayerEffects;

namespace MurderMystery
{
    public class EventHandlers
    {
        private Plugin plugin => Plugin.Singleton;
        private GamemodeStatus gamemodeStatus => Plugin.GamemodeStatus;
        internal List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();
        internal EventHandlers()
        {

        }

        internal void EnablePrimary(bool enable = true)
        {
            if (enable)
            {
                if (gamemodeStatus.PrimaryHandlersEnabled) { return; }
                Handlers.Server.WaitingForPlayers += WaitingForPlayers;
                Handlers.Server.RoundStarted += RoundStarted;
                Handlers.Server.RoundEnded += RoundEnded;
                Handlers.Server.RestartingRound += RestartingRound;
                gamemodeStatus.PrimaryHandlersEnabled = true;
                gamemodeStatus.Enabled = true;
            }
            else
            {
                if (!gamemodeStatus.PrimaryHandlersEnabled) { return; }
                Handlers.Server.WaitingForPlayers -= WaitingForPlayers;
                Handlers.Server.RoundStarted -= RoundStarted;
                Handlers.Server.RoundEnded -= RoundEnded;
                Handlers.Server.RestartingRound -= RestartingRound;
                gamemodeStatus.PrimaryHandlersEnabled = false;
                gamemodeStatus.Enabled = false;
            }
        }

        internal void EnableSecondary(bool enable = true)
        {
            if (enable)
            {
                if (gamemodeStatus.SecondaryHandlersEnabled) { return; }
                Handlers.Player.Joined += Joined;
                //Handlers.Player.Left += Left;
                Handlers.Server.RespawningTeam += RespawningTeam;
                gamemodeStatus.SecondaryHandlersEnabled = true;
            }
            else
            {
                if (!gamemodeStatus.SecondaryHandlersEnabled) { return; }
                Handlers.Player.Joined -= Joined;
                //Handlers.Player.Left -= Left;
                Handlers.Server.RespawningTeam -= RespawningTeam;
                gamemodeStatus.SecondaryHandlersEnabled = false;
            }
        }

        // Primary Events
        private void WaitingForPlayers()
        {
            Log.Debug("Primary Event WaitingForPlayers called.", plugin.Debug);
            gamemodeStatus.WaitingForPlayers = true;
            EnableSecondary();
        }

        private void RoundStarted()
        {
            Log.Debug("Primary Event RoundStarted called.", plugin.Debug);
            if (plugin.Config.RequireRoundRestart && !gamemodeStatus.WaitingForPlayers) { Log.Debug("RequireRoundRestart is enabled, the round will not begin because a restart has not occured.", plugin.Debug); return; }
            if (!plugin.Config.RequireRoundRestart && !gamemodeStatus.WaitingForPlayers) { EnableSecondary(); }
            gamemodeStatus.RoundStarted = true;
            Coroutines.Add(Timing.RunCoroutine(SetupMap()));
            Coroutines.Add(Timing.RunCoroutine(MMPlayer.SetupPlayers()));
            Coroutines.Add(Timing.RunCoroutine(HandoutEquipment(plugin.Config.EquipmentCooldown)));
        }

        private void RoundEnded(RoundEndedEventArgs ev)
        {
            Log.Debug("Primary Event RoundEnded called.", plugin.Debug);
            DisableEvent();
        }

        private void RestartingRound()
        {
            Log.Debug("Primary Event RestartingRound called.", plugin.Debug);
            DisableEvent();
        }

        // Secondary Events
        private void Joined(JoinedEventArgs ev)
        {
            if (gamemodeStatus.Active)
            {
                ev.Player.Broadcast(15, $"<size=30>Murder Mystery gamemode is currently in progress.\n{plugin.VersionStr}</size>");
            }
            else if (gamemodeStatus.RoundStarted)
            {
                ev.Player.Broadcast(15, $"<size=30>Murder Mystery gamemode has ended.\n{plugin.VersionStr}</size>");
            }
            else
            {
                ev.Player.Broadcast(15, $"<size=30>Murder Mystery gamemode is enabled for this round.\n{plugin.VersionStr}</size>");
            }
        }

        private void Left(LeftEventArgs ev)
        {
            // TODO: If a detective leaves the game, a new one will be set to balance the game properly.
        }

        private void RespawningTeam(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
        }


        // Basic Functions
        private void DisableEvent()
        {
            EnablePrimary(false);
            EnableSecondary(false);
            foreach (CoroutineHandle handle in Coroutines)
            {
                Timing.KillCoroutines(handle);
            }
            gamemodeStatus.Active = false; // Active property will be used to determine if coroutines are running.
            gamemodeStatus.WaitingForPlayers = false;
            gamemodeStatus.RoundStarted = false;
        }
        private IEnumerator<float> SetupMap()
        {
            yield return Timing.WaitForSeconds(0.5f);
            foreach (Door door in Map.Doors)
            {
                switch (door.PermissionLevels)
                {
                    case 0:
                        break;
                    case Door.AccessRequirements.Checkpoints:
                        door.NetworkisOpen = false;
                        door.Networklocked = true;
                        break;
                    default:
                        door.NetworkisOpen = true;
                        door.Networklocked = true;
                        break;
                }
            }
            yield return Timing.WaitForSeconds(0.2f);
            foreach (Pickup item in UnityEngine.Object.FindObjectsOfType<Pickup>())
            {
                item.Delete();
            }
        }
        private IEnumerator<float> HandoutEquipment(float waittime)
        {
            yield return Timing.WaitForSeconds(waittime);
            // do stuff
        }

        internal CoroutineHandle AddCoroutine(CoroutineHandle handle)
        {
            Coroutines.Add(handle);
            return handle;
        }
    }
}