using System.Collections.Generic;

namespace GitHubRelease.Notes
{
    /// <summary>
    /// Represents a GitHub issue.
    /// </summary>
    public class GitHubIssue
    {
        internal GitHubIssue(
            int number,
            string title,
            string url,
            IReadOnlyCollection<string> labels,
            GitHubContributor? contributor)
        {
            Number = number;
            Title = title;
            Url = url;
            Labels = labels;
            Contributor = contributor;
        }

        /// <summary>
        /// The issue number.
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// The title of the issue.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The URL to browse the issue.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// The labels of the issue.
        /// </summary>
        public IReadOnlyCollection<string> Labels { get; }

        /// <summary>
        /// The contributor closing this issue.
        /// </summary>
        /// <remarks>
        /// This property is <see langword="null"/> if GitHub fails to
        /// match the commit author email with a GitHub account, or if
        /// the issue was closed by the repository owner and
        /// <see cref="Configuration.ReleaseNotesConfiguration.ShouldGiveCreditToRepositoryOwner"/>
        /// is set to <see langword="false"/>.
        /// </remarks>
        public GitHubContributor? Contributor { get; }
    }
}
