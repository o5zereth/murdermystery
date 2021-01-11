using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using System.Collections.Generic;
using Handlers = Exiled.Events.Handlers;
using MurderMystery.Extensions;
using Interactables.Interobjects.DoorUtils;
using MurderMystery.Utils;

namespace MurderMystery
{
    public class EventHandlers
    {
        public MurderMystery Plugin => MurderMystery.Singleton;
        public GamemodeStatus GamemodeStatus => MurderMystery.GamemodeStatus;
        internal EventHandlers() { }

        internal List<CoroutineHandle> Coroutines = new List<CoroutineHandle>();

        private void WaitingForPlayers()
        {
            Log.Debug("WaitingForPlayers Primary Event called.", Plugin.Debug);
            GamemodeStatus.WaitingForPlayers = true;
            EnableSecondary();
        }
        private void RoundStarted()
        {
            Log.Debug("RoundStarted Primary Event called.", Plugin.Debug);

            if (Plugin.Config.RequireRoundRestart && !GamemodeStatus.WaitingForPlayers) { Log.Debug("Round has not restarted, the gamemode will not begin.", Plugin.Debug); return; } else { Log.Debug("Gamemode is starting..."); }

            GamemodeStatus.Started = true;

            Coroutines.RunAndAdd(SetupEvent()).RunAndAdd(MMPlayer.SetupPlayers());
        }
        private void RoundEnded(RoundEndedEventArgs ev)
        {
            Log.Debug("RoundEnded Primary Event called.", Plugin.Debug);

            if (!GamemodeStatus.Started) { return; }

            EnableSecondary(false);
            Coroutines.KillAll();

            GamemodeStatus.Ended = true;
        }
        private void RestartingRound()
        {
            Log.Debug("RestartingRound Primary Event called.", Plugin.Debug);

            if (!GamemodeStatus.Started) { return; }

            EnableGamemode(false);
        }





        internal void EnableGamemode(bool enable = true)
        {
            Log.Debug($"EnableGamemode Primary Function called. {(enable ? "[Enabling]" : "[Disabling]" )}", Plugin.Debug);
            if (enable)
            {
                if (GamemodeStatus.Enabled) { Log.Debug("EnableGamemode: Gamemode is already enabled.", Plugin.Debug); return; }

                EnablePrimary();

                GamemodeStatus.Enabled = true;
            }
            else
            {
                if (!GamemodeStatus.Enabled) { Log.Debug("EnableGamemode: Gamemode is already disabled.", Plugin.Debug); return; }

                EnablePrimary(false);

                if (!GamemodeStatus.Ended && GamemodeStatus.SecondaryEventsEnabled) { EnableSecondary(false); Coroutines.KillAll(); }

                GamemodeStatus.Enabled = false;
                GamemodeStatus.Ended = false;
                GamemodeStatus.Started = false;
                GamemodeStatus.WaitingForPlayers = false;
            }
        }

        internal void EnablePrimary(bool enable = true)
        {
            Log.Debug($"EnablePrimary Primary Function called. {(enable ? "[Enabling]" : "[Disabling]")}", Plugin.Debug);
            if (enable)
            {
                if (GamemodeStatus.PrimaryEventsEnabled) { Log.Debug("EnablePrimary: Primary events are already enabled.", Plugin.Debug); return; }

                Handlers.Server.WaitingForPlayers += WaitingForPlayers;
                Handlers.Server.RoundStarted += RoundStarted;
                Handlers.Server.RoundEnded += RoundEnded;
                Handlers.Server.RestartingRound += RestartingRound;

                GamemodeStatus.PrimaryEventsEnabled = true;
            }
            else
            {
                if (!GamemodeStatus.PrimaryEventsEnabled) { Log.Debug("EnablePrimary: Primary events are already disabled.", Plugin.Debug); return; }

                Handlers.Server.WaitingForPlayers -= WaitingForPlayers;
                Handlers.Server.RoundStarted -= RoundStarted;
                Handlers.Server.RoundEnded -= RoundEnded;
                Handlers.Server.RestartingRound -= RestartingRound;

                GamemodeStatus.PrimaryEventsEnabled = false;
            }
        }

        internal void EnableSecondary(bool enable = true)
        {
            Log.Debug($"EnableSecondary Primary Function called. {(enable ? "[Enabling]" : "[Disabling]")}", Plugin.Debug);
            if (enable)
            {
                if (GamemodeStatus.SecondaryEventsEnabled) { Log.Debug("EnableSecondary: Secondary events are already enabled.", Plugin.Debug); return; }



                GamemodeStatus.SecondaryEventsEnabled = true;
            }
            else
            {
                if (!GamemodeStatus.SecondaryEventsEnabled) { Log.Debug("EnableSecondary: Secondary events are already disabled.", Plugin.Debug); return; }



                GamemodeStatus.SecondaryEventsEnabled = false;
            }
        }

        internal IEnumerator<float> SetupEvent()
        {
            Log.Debug("SetupEvent called.", MurderMystery.Singleton.Debug);

            foreach (DoorVariant door in Map.Doors)
            {
                switch (door.RequiredPermissions.RequiredPermissions)
                {
                    case KeycardPermissions.None:
                        continue;
                    case KeycardPermissions.Checkpoints:
                        door.ServerChangeLock(DoorLockReason.AdminCommand, true);
                        door.NetworkTargetState = false;
                        continue;
                    default:
                        door.ServerChangeLock(DoorLockReason.AdminCommand, true);
                        door.NetworkTargetState = true;
                        continue;
                }
            }
            yield return Timing.WaitForSeconds(0.2f);
            foreach (Pickup item in UnityEngine.Object.FindObjectsOfType<Pickup>())
            {
                item.Delete();
            }
        }
    }
}