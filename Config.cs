using Exiled.API.Interfaces;
using System.ComponentModel;

namespace MurderMystery
{
    public sealed class Config : IConfig
    {
        [Description("Enables the plugin.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Enables debug logging. Config is overriden for in-dev and pre-release versions.")]
        public bool Debug { get; set; } = true;
    }
}
