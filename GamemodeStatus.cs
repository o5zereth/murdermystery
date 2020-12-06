using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MurderMystery
{
    public class GamemodeStatus
    {
        public bool Enabled { get; internal set; } = false;
        public bool Active { get; internal set; } = false;
        public bool PrimaryHandlersEnabled { get; internal set; } = false;
        public bool SecondaryHandlersEnabled { get; internal set; } = false;
        public bool WaitingForPlayers { get; internal set; } = false;
        public bool RoundStarted { get; internal set; } = false;
    }
}
