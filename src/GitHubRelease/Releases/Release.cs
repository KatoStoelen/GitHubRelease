using System;
using System.Collections.Generic;
using System.Linq;
using OctokitRelease = Octokit.Release;

namespace GitHubRelease.Releases
{
    /// <summary>
    /// Represents a GitHub release.
    /// </summary>
    public class Release
    {
        private readonly List<ReleaseAsset> _assets = new List<ReleaseAsset>();

        internal Release(OctokitRelease release)
        {
            OctokitRelease = release;

            _assets.AddRange(release.Assets.Select(asset => new ReleaseAsset(asset)));
        }

        /// <summary>
        /// The ID of the release.
        /// </summary>
        public int Id => OctokitRelease.Id;

        /// <summary>
        /// The name of the release.
        /// </summary>
        public string Name => OctokitRelease.Name;

        /// <summary>
        /// The tag name of the release.
        /// </summary>
        public string TagName => OctokitRelease.TagName;

        /// <summary>
        /// The target commitish of the release.
        /// </summary>
        public string TargetCommitish => OctokitRelease.TargetCommitish;

        /// <summary>
        /// The body of the release.
        /// </summary>
        public string Body => OctokitRelease.Body;

        /// <summary>
        /// Whether or not the release is a pre-release.
        /// </summary>
        public bool IsPrerelease => OctokitRelease.Prerelease;

        /// <summary>
        /// Whether or not the release is in draft state.
        /// </summary>
        public bool IsDraft => OctokitRelease.Draft;

        /// <summary>
        /// The URL to browse the release.
        /// </summary>
        public string HtmlUrl => OctokitRelease.HtmlUrl;

        /// <summary>
        /// The creation time of the release.
        /// </summary>
        public DateTimeOffset CreatedAt => OctokitRelease.CreatedAt;

        /// <summary>
        /// If published, the publish date of the release, otherwise <see langword="null"/>.
        /// </summary>
        public DateTimeOffset? PublishedAt => OctokitRelease.PublishedAt;

        /// <summary>
        /// The assets of the release.
        /// </summary>
        public IReadOnlyList<ReleaseAsset> Assets => _assets;

        internal OctokitRelease OctokitRelease { get; }

        /// <summary>
        /// Gets a mutable object that can be used to update the release.
        /// </summary>
        /// <returns>A mutable object that can be used to update the release.</returns>
        public UpdateRelease ToUpdate() => new UpdateRelease(Id, OctokitRelease.ToUpdate());

        internal void AddAsset(ReleaseAsset asset) => _assets.Add(asset);
    }
}