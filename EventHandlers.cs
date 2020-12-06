using Exiled.Events.EventArgs;
using Handlers = Exiled.Events.Handlers;

namespace MurderMystery
{
    public class EventHandlers
    {
        private GamemodeStatus gamemodeStatus => Plugin.GamemodeStatus;
        private readonly Plugin plugin;
        internal EventHandlers(Plugin plugin) => this.plugin = plugin;

        internal void EnablePrimary(bool enable = true)
        {
            if (enable)
            {
                if (gamemodeStatus.PrimaryHandlersEnabled) { return; }
                Handlers.Server.WaitingForPlayers += WaitingForPlayers;
                Handlers.Server.RoundStarted += RoundStarted;
                Handlers.Server.RoundEnded += RoundEnded;
                Handlers.Server.RestartingRound += RestartingRound;
            }
            else
            {
                if (!gamemodeStatus.PrimaryHandlersEnabled) { return; }
                Handlers.Server.WaitingForPlayers -= WaitingForPlayers;
                Handlers.Server.RoundStarted -= RoundStarted;
                Handlers.Server.RoundEnded -= RoundEnded;
                Handlers.Server.RestartingRound -= RestartingRound;
            }
        }

        internal void EnableSecondary(bool enable = true)
        {
            if (enable)
            {
                if (gamemodeStatus.SecondaryHandlersEnabled) { return; }
            }
            else
            {
                if (!gamemodeStatus.SecondaryHandlersEnabled) { return; }
            }
        }

        // Primary Events
        private void WaitingForPlayers()
        {
            gamemodeStatus.WaitingForPlayers = true;
        }

        private void RoundStarted()
        {
            gamemodeStatus.RoundStarted = true;
        }

        private void RoundEnded(RoundEndedEventArgs ev)
        {
            
        }

        private void RestartingRound()
        {
            
        }
    }
}