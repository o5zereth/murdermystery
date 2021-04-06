using Exiled.API.Features;
using Exiled.API.Interfaces;

namespace MurderMystery
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool RequireRoundRestart { get; set; } = true;

        public float MurdererPercent { get; set; } = 1f / 6f;
        public float DetectivePercent { get; set; } = 1f / 12f;
        public float EquipmentTime { get; set; } = 60f;
        public float RoundTime { get; set; } = 600f;

        internal static void Validate(Config cfg)
        {
            if (cfg.MurdererPercent + cfg.DetectivePercent > 1f)
            {
                Log.Warn("The given config values for murderer and detective percent are too large. Setting to default...");
                cfg.MurdererPercent = 1f / 6f;
                cfg.DetectivePercent = 1f / 12f;
            }
            else
            {
                if (cfg.MurdererPercent <= 0)
                {
                    Log.Warn("The given config value for murderer percent is either zero or less than zero which is not permitted. Setting to default...");
                    cfg.MurdererPercent = 1f / 6f;
                }
                if (cfg.DetectivePercent <= 0f)
                {
                    Log.Warn("The given config value for detective percent is either zero or less than zero which is not permitted. Setting to default...");
                    cfg.DetectivePercent = 1f / 12f;
                }
            }

            Log.Info($"Loaded percentages: (Murderer: {cfg.MurdererPercent}), (Detective: {cfg.DetectivePercent})");

            Log.Info($"The murder mystery gamemode will {(cfg.RequireRoundRestart ? "" : "not ")}require a round restart to enable.");

            if (cfg.EquipmentTime < 45f)
            {
                Log.Warn("Equipment handout time is less than 45 seconds, recommend atleast 45.");
            }

            Log.Info("Config validated successfully.");
        }
    }
}