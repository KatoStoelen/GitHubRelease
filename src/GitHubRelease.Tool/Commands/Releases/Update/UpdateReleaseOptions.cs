using System;
using System.ComponentModel;
using System.IO;
using SystemCommandLineExtensions.AutoOptions;

namespace GitHubRelease.Tool.Commands.Releases.Update
{
    internal class UpdateReleaseOptions : ReleaseOptions
    {
        [Description("The ID of the release to update")]
        public int? Id { get; set; }

        [Alias("--tag")]
        [Description(
            "The current tag of the release. Can be used instead of " +
            "'--id' to identify the release to update. Requires the " +
            "tag to exist in the GitHub repository")]
        public string? TagName { get; set; }

        [Alias("-n")]
        [Description("The new name of the release")]
        public string? Name { get; set; }

        [Alias("--new-tag")]
        [Description("The new tag name of the release")]
        public string? NewTagName { get; set; }

        [Alias("--target")]
        [Description(
            "The new commitish value to specify the target of the tag. " +
            "Defaults to the repository's default branch. " +
            "Unused if the tag already exists")]
        public string? TargetCommitish { get; set; }

        [Description("A file containing the new body of the release")]
        public FileInfo? Body { get; set; }

        [Description("Whether or not the release is a pre-release")]
        public bool? Prerelease { get; set; }

        [Description(
            "Whether or not the release is in draft state. " +
            "To publish the release use '--draft false'")]
        public bool? Draft { get; set; }

        [Description("A collection of assets to include in the updated release")]
        public FileInfo[] Assets { get; set; } = new FileInfo[0];

        [Description("Whether or not to delete all the existing assets")]
        public bool ClearAssets { get; set; }

        [Description(
            "Whether or not to overwrite assets that match the name of " +
            "assets in this update. Only has an effect if this update has " +
            "any assets and '--clear-assets' is not specified. To disable " +
            "overwriting of assets, use '--overwrite-assets false'")]
        [DefaultValue(true)]
        public bool OverwriteAssets { get; set; }

        public override void EnsureValid()
        {
            base.EnsureValid();

            if (!Id.HasValue && string.IsNullOrWhiteSpace(TagName))
            {
                throw new ArgumentException(
                    "Either release ID or current tag name must be specified " +
                    "to determine which release to update");
            }

            if (NewTagName != null && string.IsNullOrWhiteSpace(NewTagName))
            {
                throw new ArgumentException("The new tag name must be a non-empty value");
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