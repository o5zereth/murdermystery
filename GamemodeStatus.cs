namespace MurderMystery
{
    public class GamemodeStatus
    {
        internal GamemodeStatus()
        {
            Enabled = false;
            Started = false;
            Ended = false;
            PrimaryEventsEnabled = false;
            SecondaryEventsEnabled = false;
        }

        public bool Enabled { get; internal set; }
        public bool Started { get; internal set; }
        public bool Ended { get; internal set; }
        public bool WaitingForPlayers { get; internal set; }
        public bool PrimaryEventsEnabled { get; internal set; }
        public bool SecondaryEventsEnabled { get; internal set; }
    }
}