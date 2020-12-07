using Exiled.API.Interfaces;
using System.ComponentModel;

namespace MurderMystery
{
    public sealed class Config : IConfig
    {
        [Description("Enables the plugin.")]
        public bool IsEnabled { get; set; } = true;
        [Description("Specifies if the gamemode should require a round restart to begin.")]
        public bool RequireRoundRestart { get; set; } = true;
        [Description("Specifies the time (in seconds) to give out player equipment when the round starts. Must be atleast 10 seconds. (Recommend atleast 45)")]
        public float EquipmentCooldown { get; set; } = 60f;
        [Description("Specifies the murderer percentage. Must be between zero and one.")]
        public float MurdererPercent { get; set; } = 1 / 6;
        [Description("Specifies the detective percentage. Must be between zero and one.")]
        public float DetectivePercent { get; set; } = 1 / 12;

        public void Validate()
        {
            if (EquipmentCooldown < 10f) { EquipmentCooldown = 10f; }
            if (MurdererPercent <= 0 || MurdererPercent >= 1)
            {
                MurdererPercent = 1 / 6;
            }
            if (DetectivePercent <= 0 || DetectivePercent >= 1)
            {
                DetectivePercent = 1 / 12;
            }
            if (MurdererPercent + DetectivePercent >= 1)
            {
                MurdererPercent = 1 / 6;
                DetectivePercent = 1 / 12;
            }
        }
    }
}