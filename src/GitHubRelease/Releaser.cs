using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GitHubRelease.Internal;
using GitHubRelease.Releases;

namespace GitHubRelease
{
    /// <summary>
    /// An API for managing GitHub releases.
    /// </summary>
    public class Releaser
    {
        private readonly GitHubApi _gitHubApi;

        /// <summary>
        /// Initializes the release creator.
        /// <para>
        /// The target GitHub repository is determined by the remotes of the local
        /// repository found in <paramref name="repositoryRootDirectory"/>.
        /// </para>
        /// </summary>
        /// <param name="repositoryRootDirectory">The root directory of the local git repository.</param>
        /// <param name="githubToken">A token used for GitHub API authentication.</param>
        public Releaser(DirectoryInfo repositoryRootDirectory, string githubToken)
        {
            using var localRepository = new LocalGitRepository(repositoryRootDirectory.FullName);

            _gitHubApi = new GitHubApi(
                GitHubRepository.FindByRemotes(localRepository.Remotes),
                githubToken);
        }

        /// <summary>
        /// Initializes the release creator.
        /// </summary>
        /// <param name="repositoryOwner">The owner of the GitHub repository.</param>
        /// <param name="repositoryName">The name of the GitHub repository.</param>
        /// <param name="githubToken">A token used for GitHub API authentication.</param>
        public Releaser(string repositoryOwner, string repositoryName, string githubToken)
        {
            _gitHubApi = new GitHubApi(
                new GitHubRepository(repositoryOwner, repositoryName),
                githubToken);
        }

        /// <summary>
        /// Gets an existing GitHub release by ID.
        /// </summary>
        /// <param name="id">The ID of the release.</param>
        /// <returns>The release.</returns>
        public async Task<Release> GetReleaseAsync(int id)
        {
            var octokitRelease = await _gitHubApi.GetReleaseAsync(id)
                .ConfigureAwait(false);

            return new Release(octokitRelease);
        }

        /// <summary>
        /// Gets an existing GitHub release by tag name.
        /// </summary>
        /// <remarks>
        /// If the specified tag does not exist in the GitHub repository,
        /// the release will not be found, even though a release with this
        /// tag name exists. This usually happens when the release is a
        /// draft.
        /// </remarks>
        /// <param name="tagName">The tag name of the release.</param>
        /// <returns>The release.</returns>
        public async Task<Release> GetReleaseAsync(string tagName)
        {
            var octokitRelease = await _gitHubApi.GetReleaseAsync(tagName)
                .ConfigureAwait(false);

            return new Release(octokitRelease);
        }

        /// <summary>
        /// Gets all existing GitHub releases.
        /// </summary>
        /// <returns>A collection of all existing releases.</returns>
        public async Task<IReadOnlyCollection<Release>> GetAllReleasesAsync()
        {
            var octokitReleases = await _gitHubApi.GetAllReleasesAsync()
                .ConfigureAwait(false);

            return octokitReleases
                .Select(octokitRelease => new Release(octokitRelease))
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Creates a new GitHub release.
        /// </summary>
        /// <param name="newRelease">The new release.</param>
        /// <param name="cancellationToken">
        /// An optional token to monitor for cancellation requests.
        /// </param>
        /// <returns>The created release.</returns>
        public async Task<Release> CreateReleaseAsync(
            NewRelease newRelease, CancellationToken cancellationToken = default)
        {
            var octokitRelease = await _gitHubApi
                .CreateReleaseAsync(newRelease.OctokitNewRelease)
                .ConfigureAwait(false);

            var release = new Release(octokitRelease);

            foreach (var asset in newRelease.Assets)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var octokitAsset = await _gitHubApi.UploadAssetAsync(octokitRelease, asset)
                    .ConfigureAwait(false);

                release.AddAsset(new ReleaseAsset(octokitAsset));
            }

            return release;
        }

        /// <summary>
        /// Updates an existing GitHub release.
        /// </summary>
        /// <param name="updateRelease">The updated release.</param>
        /// <param name="cancellationToken">
        /// An optional token to monitor for cancellation requests.
        /// </param>
        /// <returns>The updated release.</returns>
        public async Task<Release> UpdateReleaseAsync(
            UpdateRelease updateRelease, CancellationToken cancellationToken = default)
        {
            var octokitRelease = updateRelease.Id.HasValue
                ? await _gitHubApi
                    .UpdateReleaseAsync(updateRelease.Id.Value, updateRelease.OctokitUpdateRelease)
                    .ConfigureAwait(false)
                : await _gitHubApi
                    .UpdateReleaseAsync(updateRelease.CurrentTagName!, updateRelease.OctokitUpdateRelease)
                    .ConfigureAwait(false);

            var release = new Release(octokitRelease);

            if (updateRelease.DeleteExistingAssets)
            {
                await _gitHubApi.DeleteAllAssetsAsync(octokitRelease.Id, cancellationToken)
                    .ConfigureAwait(false);
            }

            foreach (var asset in updateRelease.NewAssets)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var octokitAsset = await _gitHubApi
                    .UploadAssetAsync(
                        octokitRelease,
                        asset,
                        overwriteExisting:
                            updateRelease.OverwriteExistingAssets &&
                            !updateRelease.DeleteExistingAssets)
                    .ConfigureAwait(false);

                release.AddAsset(new ReleaseAsset(octokitAsset));
            }

            return release;
        }

        /// <summary>
        /// Deletes an existing GitHub release by ID.
        /// </summary>
        /// <param name="id">The ID of the release.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        public Task DeleteReleaseAsync(int id) => _gitHubApi.DeleteReleaseAsync(id);

        /// <summary>
        /// Deletes an existing GitHub release by tag name.
        /// </summary>
        /// <param name="tagName">The tag name of the release.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        public Task DeleteReleaseAsync(string tagName) => _gitHubApi.DeleteReleaseAsync(tagName);
    }
}