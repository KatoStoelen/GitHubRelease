using System;
using System.ComponentModel;
using System.IO;
using SystemCommandLineExtensions.AutoOptions;

namespace GitHubRelease.Tool.Commands.Releases.Create
{
    internal class CreateReleaseOptions : ReleaseOptions
    {
        [Alias("-n")]
        [Description("An optional name of the release")]
        public string Name { get; set; } = string.Empty;

        [Alias("--tag")]
        [Description("The tag name of the release")]
        public string TagName { get; set; } = string.Empty;

        [Alias("--target")]
        [Description(
            "A commitish value to specify the target of the tag. " +
            "Defaults to the repository's default branch. " +
            "Unused if the tag already exists")]
        public string TargetCommitish { get; set; } = string.Empty;

        [Description("A file containing the body of the release (usually a markdown file)")]
        public FileInfo? Body { get; set; }

        [Description("Whether or not the release is a pre-release")]
        public bool Prerelease { get; set; }

        [Description(
            "Whether or not the release is in draft state. " +
            "To publish the release use '--draft false'")]
        [DefaultValue(true)]
        public bool Draft { get; set; }

        [Description("A collection of assets to include in the release")]
        public FileInfo[] Assets { get; set; } = new FileInfo[0];

        public override void EnsureValid()
        {
            base.EnsureValid();

            if (string.IsNullOrWhiteSpace(TagName))
            {
                throw new ArgumentException("Tag name must be set");
            }

            if (Body != null && !Body.Exists)
            {
                throw new ArgumentException($"The body file '{Body}' does not exist");
            }

            foreach (var asset in Assets)
            {
                if (!asset.Exists)
                {
                    throw new ArgumentException($"The asset '{asset}' does not exist");
                }
            }
        }
    }
}