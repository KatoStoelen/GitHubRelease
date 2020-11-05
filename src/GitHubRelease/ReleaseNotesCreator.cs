using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GitHubRelease.Configuration;
using GitHubRelease.Internal;
using GitHubRelease.Notes;
using Octokit;

namespace GitHubRelease
{
    /// <summary>
    /// An API for creating release notes.
    /// </summary>
    public class ReleaseNotesCreator : IDisposable
    {
        private readonly LocalGitRepository _localRepository;
        private readonly GitHubRepository _gitHubRepository;
        private readonly GitHubApi _gitHubApi;
        private readonly ReleaseNotesConfiguration _configuration;

        /// <summary>
        /// Initializes the release notest creator.
        /// </summary>
        /// <remarks>
        /// This constructor will look for a configuration file
        /// in <paramref name="repositoryRootDirectory"/> and the current directory
        /// if not found in <paramref name="repositoryRootDirectory"/>.
        /// </remarks>
        /// <param name="repositoryRootDirectory">The root directory of the local git repository.</param>
        /// <param name="githubToken">A token used for GitHub API authentication.</param>
        public ReleaseNotesCreator(DirectoryInfo repositoryRootDirectory, string githubToken)
            : this(
                repositoryRootDirectory,
                githubToken,
                ReleaseNotesConfiguration.FindInDirectory(repositoryRootDirectory) ??
                ReleaseNotesConfiguration.FindInDirectory(
                    new DirectoryInfo(Directory.GetCurrentDirectory())) ??
                new ReleaseNotesConfiguration())
        {
        }

        /// <summary>
        /// Initializes the release notest creator.
        /// </summary>
        /// <param name="repositoryRootDirectory">The root directory of the local git repository.</param>
        /// <param name="githubToken">A token used for GitHub API authentication.</param>
        /// <param name="configFile">The configuration file to use.</param>
        public ReleaseNotesCreator(
                DirectoryInfo repositoryRootDirectory, string githubToken, FileInfo configFile)
            : this(
                repositoryRootDirectory,
                githubToken,
                ReleaseNotesConfiguration.FromFile(configFile))
        {
        }

        /// <summary>
        /// Initializes the release notest creator.
        /// </summary>
        /// <param name="repositoryRootDirectory">The root directory of the local git repository.</param>
        /// <param name="githubToken">A token used for GitHub API authentication.</param>
        /// <param name="configuration">The configuration to use.</param>
        public ReleaseNotesCreator(
            DirectoryInfo repositoryRootDirectory,
            string githubToken,
            ReleaseNotesConfiguration configuration)
        {
            _localRepository = new LocalGitRepository(repositoryRootDirectory.FullName);
            _gitHubRepository = GitHubRepository.FindByRemotes(_localRepository.Remotes);
            _gitHubApi = new GitHubApi(_gitHubRepository, githubToken);
            _configuration = configuration;

            _configuration.EnsureValid();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _localRepository.Dispose();
        }

        /// <summary>
        /// Creates release notes of issues linked in commits added since the latest
        /// git tag.
        /// </summary>
        /// <remarks>
        /// If no git tag is found, the commit traversal starts from the first commit
        /// in the repository.
        /// <para>
        /// If you tag quite often, e.g. once per build, and want to traverse commits
        /// from the latest tag of a specific format, you can supply a
        /// <see cref="Regex"/> that will be used when looking up the latest tag.
        /// </para>
        /// </remarks>
        /// <param name="latestTagRegex">
        /// An optional regex to find the latest tag of a specific format.
        /// </param>
        /// <param name="cancellationToken">
        /// An optional token to monitor for cancellation requests.
        /// </param>
        /// <returns>The release notes.</returns>
        public async Task<ReleaseNotes> CreateReleaseNotesAsync(
            Regex? latestTagRegex = null,
            CancellationToken cancellationToken = default)
        {
            var commitsWithIssueLinks = await GetCommitsWithIssueLinks(latestTagRegex)
                .ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            var distinctIssueNumbers = commitsWithIssueLinks.Select(pair => pair.IssueNumber).Distinct();

            var distinctIssues = await _gitHubApi
                .GetIssuesAsync(distinctIssueNumbers, cancellationToken)
                .ConfigureAwait(false);

            var gitHubIssues = distinctIssues
                .Where(issue => issue.Labels.Any(label => _configuration.Labels.ShouldIncludeLabel(label.Name)))
                .Select(issue =>
                    new GitHubIssue(
                        issue.Number,
                        issue.Title,
                        issue.HtmlUrl,
                        issue.Labels
                            .Where(label => _configuration.Labels.ShouldIncludeLabel(label.Name))
                            .Select(label => label.Name).ToList(),
                        contributor: commitsWithIssueLinks
                            .Where(pair =>
                                pair.IssueNumber == issue.Number &&
                                pair.Commit.Author != null && (
                                    _configuration.ShouldGiveCreditToRepositoryOwner ||
                                    !pair.Commit.Author.Login.Equals(
                                        _gitHubRepository.Owner, StringComparison.OrdinalIgnoreCase)
                                ))
                            .Select(pair => new GitHubContributor(
                                pair.Commit.Author.Login, pair.Commit.Author.HtmlUrl))
                            .FirstOrDefault()))
                .ToList()
                .AsReadOnly();

            return new ReleaseNotes(gitHubIssues, _configuration.Labels);
        }

        private async Task<IReadOnlyCollection<(GitHubCommit Commit, int IssueNumber)>> GetCommitsWithIssueLinks(
            Regex? latestTagRegex)
        {
            var headCommitSha = _localRepository.HeadCommitSha ??
                throw new InvalidOperationException("Empty repository?");
            var latestTagCommitSha = _localRepository.GetLatestTagCommitSha(latestTagRegex);
            var baseCommitSha = latestTagCommitSha ?? _localRepository.FirstCommitSha!;

            var commitsSinceLatestTag = await _gitHubApi
                .GetCommitsBetweenAsync(
                    baseCommitSha, headCommitSha, includeBaseCommit: latestTagCommitSha == null)
                .ConfigureAwait(false);

            return commitsSinceLatestTag
                .GetCommitsWithIssueLinks()
                .ToList()
                .AsReadOnly();
        }
    }
}
