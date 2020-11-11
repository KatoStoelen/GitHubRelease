using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LibGit2Sharp;

namespace GitHubRelease.Internal
{
    internal class LocalGitRepository : IDisposable
    {
        private readonly Repository _repository;

        public LocalGitRepository(string repositoryRootDirectory)
        {
            _repository = new Repository(repositoryRootDirectory);
        }

        public void Dispose()
        {
            _repository.Dispose();
        }

        public string? HeadCommitSha => _repository.Head.Tip?.Sha;
        public string? FirstCommitSha => _repository.Commits
            .QueryBy(new CommitFilter { SortBy = CommitSortStrategies.Reverse })
            .FirstOrDefault()?
            .Sha;
        public IReadOnlyCollection<Remote> Remotes => _repository.Network.Remotes.ToList().AsReadOnly();

        public string? GetLatestTagCommitSha(Regex? tagRegex = null)
        {
            var tag = tagRegex != null
                ? _repository.Tags.FirstOrDefault(tag => tagRegex.IsMatch(tag.FriendlyName))
                : _repository.Tags.FirstOrDefault();

            return tag?.Target.Sha;
        }
    }
}