namespace GitHubRelease.Notes.Formatting
{
    /// <summary>
    /// Interface of release notes formatters.
    /// <para>
    /// Create your own implementation if you want to customize the release
    /// notes output.
    /// </para>
    /// </summary>
    public interface IReleaseNotesFormatter
    {
        /// <summary>
        /// Formats the specified list of GitHub issues into release notes.
        /// </summary>
        /// <param name="header">An optional header of the release notes.</param>
        /// <param name="releaseNotes">The release notes.</param>
        /// <returns>A string representation of the release notes.</returns>
        FormattedReleaseNotes Format(string? header, ReleaseNotes releaseNotes);
    }
}
