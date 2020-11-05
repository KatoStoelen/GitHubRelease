using OctokitReleaseAsset = Octokit.ReleaseAsset;

namespace GitHubRelease.Releases
{
    /// <summary>
    /// Represents a GitHub release asset.
    /// </summary>
    public class ReleaseAsset
    {
        private readonly OctokitReleaseAsset _releaseAsset;

        internal ReleaseAsset(OctokitReleaseAsset releaseAsset)
        {
            _releaseAsset = releaseAsset;
        }

        /// <summary>
        /// The ID of the release asset.
        /// </summary>
        public int Id => _releaseAsset.Id;

        /// <summary>
        /// The name of the release asset.
        /// </summary>
        public string Name => _releaseAsset.Name;
    }
}
