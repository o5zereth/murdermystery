using Exiled.API.Features;
using Handlers = Exiled.Events.Handlers;

namespace MurderMystery.API
{
    public class GamemodeManager
    {
        internal GamemodeManager()
        {
            Log.Debug("[GamemodeManager] Initializing...", MurderMystery.Singleton.DebugVERBOSE);
            Enabled = false;
            Started = false;
            Ended = false;
            ForceRoundEnd = false;
            RoundEndTime = -1f;
            PrimaryEventsEnabled = false;
            SecondaryEventsEnabled = false;
        }

        public bool Enabled
        { 
            get
            {
                return _enabled;
            }
            set
            {
                if (MurderMystery.Singleton.DebugVERBOSE)
                {
                    Log.Debug($"[GamemodeManager] Setting Enabled to {value}.");
                }
                _enabled = value;
            }
        }
        public bool Started
        {
            get
            {
                return _started;
            }
            internal set
            {
                if (MurderMystery.Singleton.DebugVERBOSE)
                {
                    Log.Debug($"[GamemodeManager] Setting Started to {value}.");
                }
                _started = value;
            }
        }
        public bool Ended
        {
            get
            {
                return _ended;
            }
            internal set
            {
                if (MurderMystery.Singleton.DebugVERBOSE)
                {
                    Log.Debug($"[GamemodeManager] Setting Ended to {value}.");
                }
                _ended = value;
            }
        }
        public bool ForceRoundEnd
        {
            get
            {
                return _forceRoundEnd;
            }
            internal set
            {
                if (MurderMystery.Singleton.DebugVERBOSE)
                {
                    Log.Debug($"[GamemodeManager] Setting ForceRoundEnd to {value}.");
                }
                _forceRoundEnd = value;
            }
        }
        public float RoundEndTime
        {
            get
            {
                return _roundEndTime;
            }
            internal set
            {
                if (MurderMystery.Singleton.DebugVERBOSE && (value <= 0f || value == MurderMystery.Singleton.Config.RoundTime)) // Prevents spamming for the entire time and shows times that are seen as "important".
                {
                    Log.Debug($"[GamemodeManager] Setting RoundEndTime to {value}.");
                }
                _roundEndTime = value;
            }
        }
        public bool WaitingForPlayers
        {
            get
            {
                return _waitingForPlayers;
            }
            internal set
            {
                if (MurderMystery.Singleton.DebugVERBOSE)
                {
                    Log.Debug($"[GamemodeManager] Setting WaitingForPlayers to {value}.");
                }
                _waitingForPlayers = value;
            }
        }
        public bool PrimaryEventsEnabled
        {
            get
            {
                return _primaryEventsEnabled;
            }
            private set
            {
                if (MurderMystery.Singleton.DebugVERBOSE)
                {
                    Log.Debug($"[GamemodeManager] Setting PrimaryEventsEnabled to {value}.");
                }
                _primaryEventsEnabled = value;
            }
        }
        public bool SecondaryEventsEnabled
        {
            get
            {
                return _secondaryEventsEnabled;
            }
            private set
            {
                if (MurderMystery.Singleton.DebugVERBOSE)
                {
                    Log.Debug($"[GamemodeManager] Setting SecondaryEventsEnabled to {value}.");
                }
                _secondaryEventsEnabled = value;
            }
        }

        private bool _enabled;
        private bool _started;
        private bool _ended;
        private bool _forceRoundEnd;
        private float _roundEndTime;
        private bool _waitingForPlayers;
        private bool _primaryEventsEnabled;
        private bool _secondaryEventsEnabled;

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

                // Makes sure that if the round is restarted without it ending, the events / coroutines are reset.
                if (SecondaryEventsEnabled)
                {
                    EnableSecondary(false);
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
                Handlers.Player.OpeningGenerator += MurderMystery.EventHandlers.OpeningGenerator;
                Handlers.Player.Shooting += MurderMystery.EventHandlers.Shooting;
                Handlers.Player.TriggeringTesla += MurderMystery.EventHandlers.TriggeringTesla;
                Handlers.Server.RespawningTeam += MurderMystery.EventHandlers.RespawningTeam;
                Handlers.Player.EnteringFemurBreaker += MurderMystery.EventHandlers.EnteringFemurBreaker;
                Handlers.Player.ChangingRole += MurderMystery.EventHandlers.ChangingRole;

                MurderMystery.CompatabilityManager.TogglingSecondaryEvents(true);

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
                Handlers.Player.OpeningGenerator -= MurderMystery.EventHandlers.OpeningGenerator;
                Handlers.Player.Shooting -= MurderMystery.EventHandlers.Shooting;
                Handlers.Player.TriggeringTesla -= MurderMystery.EventHandlers.TriggeringTesla;
                Handlers.Server.RespawningTeam -= MurderMystery.EventHandlers.RespawningTeam;
                Handlers.Player.EnteringFemurBreaker -= MurderMystery.EventHandlers.EnteringFemurBreaker;
                Handlers.Player.ChangingRole -= MurderMystery.EventHandlers.ChangingRole;

                MurderMystery.CompatabilityManager.TogglingSecondaryEvents(false);

                SecondaryEventsEnabled = false;
            }
        }
    }
}