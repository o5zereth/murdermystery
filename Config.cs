using Exiled.API.Interfaces;

namespace MurderMystery
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool RequireRoundRestart { get; set; } = true;

        public float MurdererPercent { get; set; } = 1f / 6f;
        public float DetectivePercent { get; set; } = 1f / 12f;

        internal static void Validate()
        {

        }
    }
}