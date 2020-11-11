using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GitHubRelease.Configuration;
using GitHubRelease.Notes.Formatting;

namespace GitHubRelease.Notes
{
    /// <summary>
    /// A list of GitHub issues that represents the release notes.
    /// </summary>
    public class ReleaseNotes
    {
        private readonly LabelsConfiguration _labelsConfiguration;

        internal ReleaseNotes(
            IReadOnlyCollection<GitHubIssue> issues,
            LabelsConfiguration labelsConfiguration)
        {
            _labelsConfiguration = labelsConfiguration;

            Issues = issues;
        }

        /// <summary>
        /// The GitHub issues included in the release notes.
        /// </summary>
        public IReadOnlyCollection<GitHubIssue> Issues { get; }

        /// <summary>
        /// The GitHub issues grouped by label.
        /// </summary>
        public IReadOnlyDictionary<string, ReadOnlyCollection<GitHubIssue>> IssuesByLabel =>
            new ReadOnlyDictionary<string, ReadOnlyCollection<GitHubIssue>>(
                Labels.ToDictionary(
                    label => label.Name,
                    label => Issues
                        .Where(issue => issue.Labels.Contains(label.Name))
                        .ToList()
                        .AsReadOnly()));

        /// <summary>
        /// The distinct labels of the issues in the release notes.
        /// </summary>
        public IEnumerable<LabelConfiguration> Labels =>
            Issues
                .SelectMany(issue => issue.Labels)
                .Select(label =>
                    _labelsConfiguration.Configs.SingleOrDefault(
                        config =>
                            config.Name.Equals(label, StringComparison.OrdinalIgnoreCase)) ??
                    new LabelConfiguration { Name = label, DisplayName = label })
                .Distinct(LabelConfiguration.EqualityComparer);

        /// <summary>
        /// The distinct contributors of the issues in the release notes.
        /// </summary>
        /// <remarks>
        /// This list might be empty if the issues are closed by the repository
        /// owner and <see cref="ReleaseNotesConfiguration.ShouldGiveCreditToRepositoryOwner"/>
        /// is set to <see langword="false"/>.
        /// </remarks>
        public IEnumerable<GitHubContributor> Contributors =>
            Issues
                .Select(issue => issue.Contributor)
                .Where(contributor => contributor != null)
                .Distinct(GitHubContributor.EqualityComparer)!;


        /// <summary>
        /// Gets the release notes as markdown.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="DefaultMarkdownFormatter"/>.
        /// </remarks>
        /// <param name="releaseNotesHeader">An optional release notes header.</param>
        /// <param name="headingLevel">
        /// An optional heading level to use.
        /// <para>
        /// Defaults to 1. Which means that the optionally supplied
        /// <paramref name="releaseNotesHeader"/> would become a top-level header.
        /// </para>
        /// </param>
        /// <returns>The release notes as markdown.</returns>
        public FormattedReleaseNotes ToMarkdown(string? releaseNotesHeader = null, int headingLevel = 1) =>
            Format(new DefaultMarkdownFormatter(headingLevel), releaseNotesHeader);

        /// <summary>
        /// Gets the release notes as plain text.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="DefaultPlainTextFormatter"/>.
        /// </remarks>
        /// <param name="releaseNotesHeader">An optional release notes header.</param>
        /// <returns>The release notes as plain text.</returns>
        public FormattedReleaseNotes ToPlainText(string? releaseNotesHeader = null) =>
            Format(new DefaultPlainTextFormatter(), releaseNotesHeader);

        /// <summary>
        /// Formats the release notes using the specified <see cref="IReleaseNotesFormatter"/>.
        /// </summary>
        /// <param name="formatter">The formatter to use.</param>
        /// <param name="releaseNotesHeader">An optional release notes header.</param>
        /// <returns>
        /// A string representation of the release notes.
        /// </returns>
        public FormattedReleaseNotes Format(
            IReleaseNotesFormatter formatter, string? releaseNotesHeader = null)
        {
            return formatter.Format(releaseNotesHeader, this);
        }
    }
}