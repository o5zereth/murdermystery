using Exiled.API.Interfaces;
using System.ComponentModel;

namespace MurderMystery
{
    public sealed class Config : IConfig
    {
        [Description("Enables the plugin.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enables debug logging. Config is overriden for in-dev and pre-release versions.")]
        public bool Debug { get; set; } = DefaultDebugEnabled;

		[Description("The minimum number of players required to start the gamemode. This isn't accounted for in debug versions.")]
		public int MinimumPlayersStart { get; set; } = DefaultMinimumPlayers;

		[Description("Specifies the minimum player count requried to open all heavy containment rooms. If condition is not met, most heavy rooms will be inaccessible to minimize map size.")]
		public int MinimumPlayersOpenHeavy { get; set; } = DefaultPlayersOpenHeavy;

		[Description("Specifies the percentage of murderers that are selected during the gamemode.")]
		public float MurdererPercentage { get; set; } = DefaultMurdererPercentage;

		[Description("Specifies the percentage of detectives that are selected during the gamemode.")]
		public float DetectivePercentage { get; set; } = DefaultDetectivePercentage;

		[Description("Enables developer access to debug commands. Config is overriden for in-dev and pre-release versions. (We do not abuse them!)")]
		public bool DeveloperDebugAccess { get; set; } = false;

		[Description("Maximum amount of time a round can last (in seconds) before innocents win by default. Roundlock will still prevent the round from ending.")]
		public int RoundTimeLimit { get; set; } = DefaultRoundTime;

		[Description("Time after the round starts (in seconds) when the equipment is given to players.")]
		public int EquipmentTime { get; set; } = DefaultEquipmentTime;

		public const float DefaultMurdererPercentage = 1f / 6f;
		public const float DefaultDetectivePercentage = 1f / 12f;
		public const int DefaultRoundTime = 600;
		public const int DefaultEquipmentTime = 60;
		public const int DefaultMinimumPlayers = 8;
		public const int DefaultPlayersOpenHeavy = 16;
		public const bool DefaultDebugEnabled = true;
    }
}
