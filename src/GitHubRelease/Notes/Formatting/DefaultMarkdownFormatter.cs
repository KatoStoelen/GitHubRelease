using System;
using System.Text;
using GitHubRelease.Internal;

namespace GitHubRelease.Notes.Formatting
{
    /// <summary>
    /// The default markdown formatter.
    /// <para>
    /// Issues are grouped by their label(s), and contributors are
    /// listed with links to their profile.
    /// </para>
    /// </summary>
    /// <remarks>
    /// If an issue has multiple labels, the issue is listed once per
    /// label.
    /// </remarks>
    public class DefaultMarkdownFormatter : IReleaseNotesFormatter
    {
        private readonly int _headingLevel;

        /// <summary>
        /// Initializes the markdown formatter with heading level 1.
        /// </summary>
        public DefaultMarkdownFormatter()
            : this(headingLevel: 1)
        {
        }

        /// <summary>
        /// Initializes the markdown formatter with a custom heading level.
        /// </summary>
        /// <param name="headingLevel">The heading level.</param>
        public DefaultMarkdownFormatter(int headingLevel)
        {
            if (!(1 <= headingLevel && headingLevel <= 5))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(headingLevel), headingLevel, "Heading level must >= 1 and <= 5");
            }

            _headingLevel = headingLevel;
        }

        /// <inheritdoc/>
        public FormattedReleaseNotes Format(string? header, ReleaseNotes releaseNotes)
        {
            var builder = new StringBuilder();

            var headingLevel = _headingLevel;

            if (!string.IsNullOrEmpty(header))
            {
                builder
                    .AppendLF($"{new string('#', headingLevel)} {header}")
                    .AppendLF();

                headingLevel++;
            }

            headingLevel = Math.Max(headingLevel, 2);

            foreach (var label in releaseNotes.Labels)
            {
                builder
                    .AppendLF($"{new string('#', headingLevel)} {label.DisplayName}")
                    .AppendLF();

                foreach (var issue in releaseNotes.IssuesByLabel[label.Name])
                {
                    builder
                        .Append("- ")
                        .Append($"[#{issue.Number}]({issue.Url}) {issue.Title}");

                    if (issue.Contributor != null)
                    {
                        builder
                            .AppendLF($" ([@{issue.Contributor.Login}]({issue.Contributor.Url}))");
                    }
                    else
                    {
                        builder.AppendLF();
                    }
                }
            }

            return FormattedReleaseNotes.FromString(builder.AppendLF().ToString());
        }
    }
}
