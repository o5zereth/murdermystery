using Exiled.API.Interfaces;
using System.ComponentModel;

namespace MurderMystery
{
    public sealed class Config : IConfig
    {
        [Description("Enables the plugin.")]
        public bool IsEnabled { get; set; } = true;
    }
}