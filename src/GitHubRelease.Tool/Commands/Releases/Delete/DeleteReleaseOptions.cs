using System;
using System.ComponentModel;
using SystemCommandLineExtensions.AutoOptions;

namespace GitHubRelease.Tool.Commands.Releases.Delete
{
    internal class DeleteReleaseOptions : ReleaseOptions
    {
        [Description("Delete release by ID")]
        public int? Id { get; set; }

        [Alias("--tag")]
        [Description("Delete release by tag name")]
        public string? TagName { get; set; }

        public override void EnsureValid()
        {
            base.EnsureValid();

            if (!Id.HasValue && string.IsNullOrWhiteSpace(TagName))
            {
                throw new ArgumentException($"Either release ID or tag name must be set");
            }
        }
    }
}