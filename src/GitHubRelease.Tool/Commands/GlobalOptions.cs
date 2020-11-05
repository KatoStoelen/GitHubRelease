using System.ComponentModel;

namespace GitHubRelease.Tool.Commands
{
    internal class GlobalOptions
    {
        [Description("Enable verbose output")]
        public bool Verbose { get; set; }
    }
}
