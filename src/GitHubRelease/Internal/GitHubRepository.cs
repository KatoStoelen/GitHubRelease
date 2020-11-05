using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LibGit2Sharp;

namespace GitHubRelease.Internal
{
    internal class GitHubRepository
    {
        private static readonly Regex s_githubRemoteRegex =
            new Regex(
                @"github.com[:/](?<owner>[^/]+)/(?<name>((?!(\.git|/)).)+)(\.git)?/?",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public GitHubRepository(string owner, string name)
        {
            Owner = owner;
            Name = name;
        }

        public string Owner { get; }
        public string Name { get; }

        public static GitHubRepository FindByRemotes(IReadOnlyCollection<Remote> remotes)
        {
            var orderedByPriority = remotes.OrderBy(
                remote => Math.Abs(string.Compare(remote.Name, "origin", ignoreCase: true)));

            foreach (var remote in orderedByPriority)
            {
                var isGithubRemoteMatch = s_githubRemoteRegex.Match(remote.Url);

                if (!isGithubRemoteMatch.Success)
                {
                    continue;
                }

                return new GitHubRepository(
                    isGithubRemoteMatch.Groups["owner"].Value,
                    isGithubRemoteMatch.Groups["name"].Value);
            }

            throw new ArgumentException($"No github remotes found", nameof(remotes));
        }
    }
}
