using System;
using System.Text;
using Cake.Core.IO;
using GitHubRelease.Cake.Internal;
using GitHubRelease.Notes;
using GitHubRelease.Notes.Formatting;

/// <summary>
/// Settings for writing release notes to a file.
/// </summary>
public class OutputReleaseNotesSettings : ReleaseNotesSettings
{
    /// <summary>
    /// The file to write the release notes to.
    /// </summary>
    public FilePath OutputFile { get; set; } = new FilePath(string.Empty);

    /// <summary>
    /// Specifies how to output the release notes to a file.
    /// <para>
    /// Defaults to <see cref="ReleaseNotesFileOutputMode.Overwrite"/>.
    /// </para>
    /// </summary>
    public ReleaseNotesFileOutputMode OutputMode { get; set; }

    /// <summary>
    /// The encoding to use when writing to <see cref="OutputFile"/>.
    /// <para>
    /// Defaults to UTF-8 (with BOM).
    /// </para>
    /// </summary>
    public Encoding OutputEncoding { get; set; } =
        new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

    internal override IReleaseNotesFormatter Formatter =>
        Format switch
        {
            ReleaseNotesFormat.Markdown => new DefaultMarkdownFormatter(
                OutputMode.ToMarkdownHeadingLevel()),

            _ => base.Formatter
        };

    internal override void EnsureValid()
    {
        base.EnsureValid();

        if (string.IsNullOrEmpty(OutputFile.FullPath))
        {
            throw new ArgumentException("Output file must be set", nameof(OutputFile));
        }
    }
}
