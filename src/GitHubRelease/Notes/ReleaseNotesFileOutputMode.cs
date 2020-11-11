namespace GitHubRelease.Notes
{
    /// <summary>
    /// Specifies how to output the release notes to a file.
    /// </summary>
    public enum ReleaseNotesFileOutputMode
    {
        /// <summary>
        /// Overwrites the specified file with the newly generated release notes.
        /// </summary>
        /// <remarks>
        /// If the specified file does not exist, it is created.
        /// </remarks>
        Overwrite = 0,

        /// <summary>
        /// Appends the release notes at the end of the specified file.
        /// </summary>
        /// <remarks>
        /// If the specified file does not exist, it is created.
        /// </remarks>
        Append = 1,

        /// <summary>
        /// Prepends the release notes at the start of the specified file.
        /// </summary>
        /// <remarks>
        /// If the specified file does not exist, it is created.
        /// <para>
        /// This mode writes the new release notes content to a temporary file
        /// and uses <see cref="System.IO.File.Replace(string, string, string)"/>
        /// to set the new content of the specified output file. A backup of the
        /// existing file is not created.
        /// </para>
        /// </remarks>
        Prepend = 2
    }
}