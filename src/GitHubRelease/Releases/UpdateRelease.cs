using System;
using System.Collections.Generic;
using System.IO;
using OctokitUpdateRelease = Octokit.ReleaseUpdate;

namespace GitHubRelease.Releases
{
    /// <summary>
    /// Update an existing release.
    /// </summary>
    public class UpdateRelease
    {
        /// <summary>
        /// Initialize an update of an existing release.
        /// </summary>
        /// <param name="id">The ID of the release to update.</param>
        public UpdateRelease(int id)
            : this(id, new OctokitUpdateRelease())
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "The ID must be greater than zero", nameof(id));
            }
        }

        /// <summary>
        /// Initialize an update of an existing release.
        /// </summary>
        /// <remarks>
        /// If the specified tag does not exist in the GitHub repository,
        /// the update will fail, even though a release with this
        /// tag name exists. This usually happens when the release is a
        /// draft.
        /// </remarks>
        /// <param name="tagName">The tag name of the release to update.</param>
        public UpdateRelease(string tagName)
            : this(null, new OctokitUpdateRelease { TagName = tagName })
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                throw new ArgumentException("Tag name must be set.", nameof(tagName));
            }
        }

        internal UpdateRelease(int? id, OctokitUpdateRelease releaseUpdate)
        {
            Id = id;
            OctokitUpdateRelease = releaseUpdate;
            CurrentTagName = releaseUpdate.TagName;
        }

        /// <summary>
        /// The ID of the release to update.
        /// </summary>
        public int? Id { get; }

        /// <summary>
        /// The tag name of the release to update.
        /// </summary>
        public string? CurrentTagName { get; }

        /// <summary>
        /// The name of the release (optional).
        /// </summary>
        public string Name
        {
            get => OctokitUpdateRelease.Name;
            set => OctokitUpdateRelease.Name = value;
        }

        /// <summary>
        /// The tag name of the release.
        /// </summary>
        public string TagName
        {
            get => OctokitUpdateRelease.TagName;
            set => OctokitUpdateRelease.TagName = value;
        }

        /// <summary>
        /// A commitish value to specify the target of the tag (optional).
        /// <para>
        /// Defaults to the repository's default branch.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Unused if the tag already exists.
        /// </remarks>
        public string TargetCommitish
        {
            get => OctokitUpdateRelease.TargetCommitish;
            set => OctokitUpdateRelease.TargetCommitish = value;
        }

        /// <summary>
        /// The body of the release.
        /// </summary>
        public string Body
        {
            get => OctokitUpdateRelease.Body;
            set => OctokitUpdateRelease.Body = value;
        }

        /// <summary>
        /// Whether or not this release is a pre-release.
        /// </summary>
        public bool? IsPrerelease
        {
            get => OctokitUpdateRelease.Prerelease;
            set => OctokitUpdateRelease.Prerelease = value;
        }

        /// <summary>
        /// Whether or not this release is in draft state.
        /// </summary>
        public bool? IsDraft
        {
            get => OctokitUpdateRelease.Draft;
            set => OctokitUpdateRelease.Draft = value;
        }

        /// <summary>
        /// A collection of new assets to include in the updated release.
        /// </summary>
        public ICollection<FileInfo> NewAssets { get; set; } = new List<FileInfo>();

        /// <summary>
        /// Whether or not to delete all the existing assets for this release.
        /// <para>
        /// Defaults to <see langword="false"/>.
        /// </para>
        /// </summary>
        public bool DeleteExistingAssets { get; set; }

        /// <summary>
        /// Whether or not to overwrite existing assets that has the same name
        /// as assets added to this update.
        /// <para>
        /// Setting this to <see langword="false"/> will cause exceptions to be
        /// thrown if any asset in this update matches an existing asset.
        /// </para>
        /// <para>
        /// Defaults to <see langword="true"/>.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This setting only has an effect if this update contains any assets
        /// and <see cref="DeleteExistingAssets"/> is set to <see langword="false"/>.
        /// </remarks>
        public bool OverwriteExistingAssets { get; set; } = true;

        internal OctokitUpdateRelease OctokitUpdateRelease { get; }

        /// <summary>
        /// Adds an asset to the updated release.
        /// </summary>
        /// <param name="asset">The asset file to include.</param>
        public void AddAsset(FileInfo asset) => NewAssets.Add(asset);
    }
}
