using Exiled.API.Features;
using MEC;
using MurderMystery.Extensions;
using System.Collections.Generic;
using Handlers = Exiled.Events.Handlers;

namespace MurderMystery.API
{
    public class GamemodeManager
    {
        internal GamemodeManager()
        {
            Enabled = false;
            Started = false;
            Ended = false;
            ForceRoundEnd = false;
            RoundEndTime = -1f;
            PrimaryEventsEnabled = false;
            SecondaryEventsEnabled = false;
        }

        public bool Enabled { get; private set; }
        public bool Started { get; internal set; }
        public bool Ended { get; internal set; }
        public bool ForceRoundEnd { get; set; }
        public float RoundEndTime { get; internal set; }
        public bool WaitingForPlayers { get; internal set; }
        public bool PrimaryEventsEnabled { get; private set; }
        public bool SecondaryEventsEnabled { get; private set; }

        internal void EnableGamemode(bool enable = true)
        {
            Log.Debug($"EnableGamemode Primary Function called. {(enable ? "[Enabling]" : "[Disabling]")}", MurderMystery.Singleton.Debug);

            if (enable)
            {
                if (Enabled) { Log.Debug("EnableGamemode: Gamemode is already enabled.", MurderMystery.Singleton.Debug); return; }

                EnablePrimary();

                Enabled = true;
            }
            else
            {
                if (!Enabled) { Log.Debug("EnableGamemode: Gamemode is already disabled.", MurderMystery.Singleton.Debug); return; }

                EnablePrimary(false);

                if (!Ended && SecondaryEventsEnabled) // Makes sure that if the round is restarted without it ending, the events / coroutines are reset.
                {
                    EnableSecondary(false);

                    MurderMystery.CoroutineManager.Reset();
                }

                Enabled = false;
                Ended = false;
                Started = false;
                ForceRoundEnd = false;
                RoundEndTime = -1f;
                WaitingForPlayers = false;
            }
        }
        internal void EnablePrimary(bool enable = true)
        {
            Log.Debug($"EnablePrimary Primary Function called. {(enable ? "[Enabling]" : "[Disabling]")}", MurderMystery.Singleton.Debug);

            if (enable)
            {
                if (PrimaryEventsEnabled) { Log.Debug("EnablePrimary: Primary events are already enabled.", MurderMystery.Singleton.Debug); return; }

                Handlers.Server.WaitingForPlayers += MurderMystery.EventHandlers.WaitingForPlayers;
                Handlers.Server.RoundStarted += MurderMystery.EventHandlers.RoundStarted;
                Handlers.Server.RoundEnded += MurderMystery.EventHandlers.RoundEnded;
                Handlers.Server.RestartingRound += MurderMystery.EventHandlers.RestartingRound;

                PrimaryEventsEnabled = true;
            }
            else
            {
                if (!PrimaryEventsEnabled) { Log.Debug("EnablePrimary: Primary events are already disabled.", MurderMystery.Singleton.Debug); return; }

                Handlers.Server.WaitingForPlayers -= MurderMystery.EventHandlers.WaitingForPlayers;
                Handlers.Server.RoundStarted -= MurderMystery.EventHandlers.RoundStarted;
                Handlers.Server.RoundEnded -= MurderMystery.EventHandlers.RoundEnded;
                Handlers.Server.RestartingRound -= MurderMystery.EventHandlers.RestartingRound;

                PrimaryEventsEnabled = false;
            }
        }
        internal void EnableSecondary(bool enable = true)
        {
            Log.Debug($"EnableSecondary Primary Function called. {(enable ? "[Enabling]" : "[Disabling]")}", MurderMystery.Singleton.Debug);

            if (enable)
            {
                if (SecondaryEventsEnabled) { Log.Debug("EnableSecondary: Secondary events are already enabled.", MurderMystery.Singleton.Debug); return; }

                Handlers.Player.Verified += MurderMystery.EventHandlers.Joined;
                Handlers.Server.EndingRound += MurderMystery.EventHandlers.EndingRound;
                Handlers.Player.Died += MurderMystery.EventHandlers.Died;
                Handlers.Player.SpawningRagdoll += MurderMystery.EventHandlers.SpawningRagdoll;
                Handlers.Player.PickingUpItem += MurderMystery.EventHandlers.PickingUpItem;
                Handlers.Player.DroppingItem += MurderMystery.EventHandlers.DroppingItem;
                Handlers.Player.InteractingLocker += MurderMystery.EventHandlers.InteractingLocker;
                Handlers.Player.Shooting += MurderMystery.EventHandlers.Shooting;
                Handlers.Player.TriggeringTesla += MurderMystery.EventHandlers.TriggeringTesla;
                Handlers.Server.RespawningTeam += MurderMystery.EventHandlers.RespawningTeam;
                Handlers.Player.EnteringFemurBreaker += MurderMystery.EventHandlers.EnteringFemurBreaker;

                SecondaryEventsEnabled = true;
            }
            else
            {
                if (!SecondaryEventsEnabled) { Log.Debug("EnableSecondary: Secondary events are already disabled.", MurderMystery.Singleton.Debug); return; }

                Handlers.Player.Verified -= MurderMystery.EventHandlers.Joined;
                Handlers.Server.EndingRound -= MurderMystery.EventHandlers.EndingRound;
                Handlers.Player.Died -= MurderMystery.EventHandlers.Died;
                Handlers.Player.SpawningRagdoll -= MurderMystery.EventHandlers.SpawningRagdoll;
                Handlers.Player.PickingUpItem -= MurderMystery.EventHandlers.PickingUpItem;
                Handlers.Player.DroppingItem -= MurderMystery.EventHandlers.DroppingItem;
                Handlers.Player.InteractingLocker -= MurderMystery.EventHandlers.InteractingLocker;
                Handlers.Player.Shooting -= MurderMystery.EventHandlers.Shooting;
                Handlers.Player.TriggeringTesla -= MurderMystery.EventHandlers.TriggeringTesla;
                Handlers.Server.RespawningTeam -= MurderMystery.EventHandlers.RespawningTeam;
                Handlers.Player.EnteringFemurBreaker -= MurderMystery.EventHandlers.EnteringFemurBreaker;

                SecondaryEventsEnabled = false;
            }
        }
    }
}