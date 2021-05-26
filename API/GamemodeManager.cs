using Exiled.API.Features;

namespace MurderMystery.API
{
    public class GamemodeManager
    {
        internal GamemodeManager() { }

        public bool Enabled { get; private set; } = false;
        public bool PlayerEventsEnabled { get; private set; } = false;
        public bool SecondaryEventsEnabled { get; private set; } = false;
        public bool Started { get; private set; }

        // Implement primary functions.

        internal void ToggleGamemode(bool enable)
        {
            Log.Debug($"[GamemodeManager::ToggleGamemode] {(enable ? "Enabling" : "Disabling")} gamemode for the next round.", MurderMystery.Singleton.LogDebug);

            if (enable)
            {
                
            }
            else
            {

            }
        }

        internal void TogglePlayerEvents(bool enable)
        {
            Log.Debug($"[GamemodeManager::TogglePlayerEvents] {(enable ? "Enabling" : "Disabling")} player events.", MurderMystery.Singleton.LogDebug);

            if (enable)
            {

            }
            else
            {

            }
        }

        internal void ToggleSecondaryEvents(bool enable)
        {
            Log.Debug($"[GamemodeManager::ToggleSecondaryEvents] {(enable ? "Enabling" : "Disabling")} secondary events.", MurderMystery.Singleton.LogDebug);

            if (enable)
            {

            }
            else
            {

            }
        }

        internal void StartGamemode()
        {
            Started = true;

            // Do later.
        }
    }
}
