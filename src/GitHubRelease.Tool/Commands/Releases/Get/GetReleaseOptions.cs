using System;
using System.ComponentModel;
using SystemCommandLineExtensions.AutoOptions;

namespace GitHubRelease.Tool.Commands.Releases.Get
{
    internal class GetReleaseOptions : ReleaseOptions
    {
        [Description("Get release by ID")]
        public int? Id { get; set; }

        [Alias("--tag")]
        [Description("Get release by tag name")]
        public string? TagName { get; set; }

        [Description("Gets all releases")]
        public bool All { get; set; }

        public override void EnsureValid()
        {
            base.EnsureValid();

            if (!Id.HasValue && string.IsNullOrWhiteSpace(TagName) && !All)
            {
                throw new ArgumentException($"Either release ID, tag name or --all must be specified");
            }
        }
    }
}