using System.Collections.Generic;
using System.IO;
using OctokitNewRelease = Octokit.NewRelease;

namespace GitHubRelease.Releases
{
    /// <summary>
    /// Create a new GitHub release.
    /// </summary>
    public class NewRelease
    {
        /// <summary>
        /// The name of the release (optional).
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The tag name of the release (required).
        /// </summary>
        public string TagName { get; set; } = string.Empty;

        /// <summary>
        /// A commitish value to specify the target of the tag (optional).
        /// <para>
        /// Defaults to the repository's default branch.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Unused if the tag already exists.
        /// </remarks>
        public string TargetCommitish { get; set; } = string.Empty;

        /// <summary>
        /// The body of the release.
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Whether or not this release is a pre-release.
        /// </summary>
        public bool IsPrerelease { get; set; }

        /// <summary>
        /// Whether or not this release is in draft state.
        /// </summary>
        public bool IsDraft { get; set; } = true;

        /// <summary>
        /// A collection of assets to include in the release.
        /// </summary>
        public ICollection<FileInfo> Assets { get; set; } = new List<FileInfo>();

        /// <summary>
        /// Adds an asset to the release.
        /// </summary>
        /// <param name="asset">The asset file to include.</param>
        public void AddAsset(FileInfo asset) => Assets.Add(asset);

        internal OctokitNewRelease OctokitNewRelease =>
            new OctokitNewRelease(TagName)
            {
                Name = Name,
                TargetCommitish = TargetCommitish,
                Body = Body,
                Draft = IsDraft,
                Prerelease = IsPrerelease
            };
    }
}
