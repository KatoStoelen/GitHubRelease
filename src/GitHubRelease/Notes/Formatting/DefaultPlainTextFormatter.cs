using System.Text;
using GitHubRelease.Internal;

namespace GitHubRelease.Notes.Formatting
{
    /// <summary>
    /// The default plain text formatter. Useful for NuGet package
    /// change log where markdown is not supported.
    /// <para>
    /// Issues are grouped by their label(s) including the GitHub
    /// login name of the contributor.
    /// </para>
    /// </summary>
    /// <remarks>
    /// If an issue has multiple labels, the issue is listed once per
    /// label.
    /// </remarks>
    public class DefaultPlainTextFormatter : IReleaseNotesFormatter
    {
        /// <inheritdoc/>
        public FormattedReleaseNotes Format(string? header, ReleaseNotes releaseNotes)
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(header))
            {
                builder
                    .AppendLF(header!)
                    .AppendLF();
            }

            foreach (var label in releaseNotes.Labels)
            {
                builder
                    .AppendLF($"{label.DisplayName}:");

                foreach (var issue in releaseNotes.IssuesByLabel[label.Name])
                {
                    builder
                        .Append("- ")
                        .Append($"#{issue.Number} {issue.Title}");

                    if (issue.Contributor != null)
                    {
                        builder
                            .AppendLF($" (@{issue.Contributor.Login})");
                    }
                    else
                    {
                        builder.AppendLF();
                    }
                }
            }

            return FormattedReleaseNotes.FromString(builder.ToString());
        }
    }
}
