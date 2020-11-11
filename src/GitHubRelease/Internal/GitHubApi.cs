using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octokit;

namespace GitHubRelease.Internal
{
    internal class GitHubApi
    {
        private readonly GitHubRepository _repository;
        private readonly GitHubClient _gitHubClient;

        public GitHubApi(GitHubRepository repository, string token)
        {
            _repository = repository;
            _gitHubClient = new GitHubClient(
                new ProductHeaderValue("KatoStoelen.GitHubRelease"))
            {
                Credentials = new Credentials(token)
            };
        }

        public async Task<IReadOnlyList<GitHubCommit>> GetCommitsBetweenAsync(
            string @base, string head, bool includeBaseCommit = false)
        {
            var compareResult = await _gitHubClient.Repository.Commit
                .Compare(_repository.Owner, _repository.Name, @base, @head)
                .ConfigureAwait(false);

            return includeBaseCommit
                ? new[] { compareResult.BaseCommit }.Concat(compareResult.Commits).ToList()
                : compareResult.Commits;
        }

        public async Task<IReadOnlyCollection<Issue>> GetIssuesAsync(
            IEnumerable<int> issueNumbers, CancellationToken cancellationToken)
        {
            var issues = new List<Issue>();

            foreach (var issueNumber in issueNumbers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var issue = await _gitHubClient.Issue
                    .Get(_repository.Owner, _repository.Name, issueNumber)
                    .ConfigureAwait(false);

                if (issue == null)
                {
                    continue;
                }

                issues.Add(issue);
            }

            return issues.AsReadOnly();
        }

        public async Task<Release> GetReleaseAsync(int id)
        {
            try
            {
                return await _gitHubClient.Repository.Release
                    .Get(_repository.Owner, _repository.Name, id)
                    .ConfigureAwait(false);
            }
            catch (NotFoundException)
            {
                throw new ArgumentException($"Could not find release with ID '{id}'", nameof(id));
            }
        }

        public async Task<Release> GetReleaseAsync(string tagName)
        {
            try
            {
                return await _gitHubClient.Repository.Release
                    .Get(_repository.Owner, _repository.Name, tagName)
                    .ConfigureAwait(false);
            }
            catch (NotFoundException)
            {
                throw new ArgumentException(
                    $"Could not find release with tag name '{tagName}'", nameof(tagName));
            }
        }

        public Task<IReadOnlyList<Release>> GetAllReleasesAsync() =>
            _gitHubClient.Repository.Release.GetAll(_repository.Owner, _repository.Name);

        public Task<Release> CreateReleaseAsync(NewRelease newRelease) =>
            _gitHubClient.Repository.Release.Create(
                _repository.Owner, _repository.Name, newRelease);

        public async Task<ReleaseAsset> UploadAssetAsync(
            Release release, FileInfo asset, bool overwriteExisting = false)
        {
            if (overwriteExisting)
            {
                _ = await DeleteAssetAsync(release.Id, asset.Name).ConfigureAwait(false);
            }

            using var assetStream = asset.OpenRead();

            try
            {
                return await _gitHubClient.Repository.Release
                    .UploadAsset(release, new ReleaseAssetUpload
                    {
                        FileName = asset.Name,
                        ContentType = "application/octet-stream",
                        RawData = assetStream
                    })
                    .ConfigureAwait(false);
            }
            catch (NotFoundException)
            {
                throw new ArgumentException($"Could not find release with ID '{release.Id}'");
            }
        }

        public async Task<bool> DeleteAssetAsync(int releaseId, string assetName)
        {
            var existingAssets = await GetAllAssetsAsync(releaseId).ConfigureAwait(false);

            var existingAsset = existingAssets.SingleOrDefault(
                a => a.Name.Equals(assetName, StringComparison.OrdinalIgnoreCase));

            return
                existingAsset != null &&
                await DeleteAssetAsync(existingAsset.Id).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<ReleaseAsset>> GetAllAssetsAsync(int releaseId)
        {
            try
            {
                return await _gitHubClient.Repository.Release
                    .GetAllAssets(_repository.Owner, _repository.Name, releaseId)
                    .ConfigureAwait(false);
            }
            catch (NotFoundException)
            {
                throw new ArgumentException(
                    $"Could not find release with ID '{releaseId}'", nameof(releaseId));
            }
        }

        public async Task<bool> DeleteAssetAsync(int id)
        {
            try
            {
                await _gitHubClient.Repository.Release
                    .DeleteAsset(_repository.Owner, _repository.Name, id)
                    .ConfigureAwait(false);

                return true;
            }
            catch (NotFoundException)
            {
                return false;
            }
        }

        public async Task DeleteAllAssetsAsync(int releaseId, CancellationToken cancellationToken)
        {
            var existingAssets = await GetAllAssetsAsync(releaseId).ConfigureAwait(false);

            foreach (var existingAsset in existingAssets)
            {
                cancellationToken.ThrowIfCancellationRequested();

                _ = await DeleteAssetAsync(existingAsset.Id).ConfigureAwait(false);
            }
        }

        public async Task<Release> UpdateReleaseAsync(string tagName, ReleaseUpdate updatedRelease)
        {
            try
            {
                var release = await GetReleaseAsync(tagName).ConfigureAwait(false);

                return await UpdateReleaseAsync(release.Id, updatedRelease).ConfigureAwait(false);
            }
            catch (NotFoundException)
            {
                throw new ArgumentException(
                    $"Could not find release with tag '{tagName}'", nameof(tagName));
            }
        }

        public async Task<Release> UpdateReleaseAsync(int id, ReleaseUpdate updatedRelease)
        {
            try
            {
                return await _gitHubClient.Repository.Release
                    .Edit(_repository.Owner, _repository.Name, id, updatedRelease)
                    .ConfigureAwait(false);
            }
            catch (NotFoundException)
            {
                throw new ArgumentException($"Could not find release with ID '{id}'", nameof(id));
            }
        }

        public async Task<bool> DeleteReleaseAsync(string tagName)
        {
            try
            {
                var release = await GetReleaseAsync(tagName).ConfigureAwait(false);

                return await DeleteReleaseAsync(release.Id).ConfigureAwait(false);
            }
            catch (NotFoundException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteReleaseAsync(int id)
        {
            try
            {
                await _gitHubClient.Repository.Release
                    .Delete(_repository.Owner, _repository.Name, id)
                    .ConfigureAwait(false);

                return true;
            }
            catch (NotFoundException)
            {
                return false;
            }
        }
    }
}