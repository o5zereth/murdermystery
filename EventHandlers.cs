using Exiled.Events.EventArgs;

namespace MurderMystery
{
    public class EventHandlers
    {
        internal EventHandlers() { }

        internal void WaitingForPlayers()
        {
            MurderMystery.Singleton.GamemodeManager.TogglePlayerEvents(true);

            // Add patches.
        }

        internal void RoundStarted()
        {
            if (!MurderMystery.Singleton.GamemodeManager.PlayerEventsEnabled) { return; }

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
    }
}
