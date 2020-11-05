using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Octokit;

namespace GitHubRelease.Internal
{
    internal static class GitHubCommitExtensions
    {
        private static readonly Regex s_issueLinkRegex =
            new Regex(
                @"(close(s|d)?|fix(es|ed)?|resolve(s|d)?):?[^\S\r\n]+#(?<issueNumber>\d+)",
                RegexOptions.IgnoreCase);

        public static IEnumerable<(GitHubCommit Commit, int IssueNumber)> GetCommitsWithIssueLinks(
            this IEnumerable<GitHubCommit> commits)
        {
            foreach (var gitHubCommit in commits)
            {
                var containsIssueLinkMatches = s_issueLinkRegex.Matches(gitHubCommit.Commit.Message);

                foreach (var match in containsIssueLinkMatches.Cast<Match>())
                {
                    var issueNumber = int.Parse(match.Groups["issueNumber"].Value);

                    yield return (gitHubCommit, issueNumber);
                }
            }
        }
    }
}