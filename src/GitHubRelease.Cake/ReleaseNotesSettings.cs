using System;
using System.IO;
using System.Text.RegularExpressions;
using Cake.Core.IO;
using GitHubRelease;
using GitHubRelease.Configuration;
using GitHubRelease.Notes.Formatting;

/// <summary>
/// Settings of release notes creation.
/// </summary>
public class ReleaseNotesSettings
{
    /// <summary>
    /// The root directory of local git repository.
    /// </summary>
    public DirectoryPath RepositoryRootDirectory { get; set; } = new DirectoryPath(string.Empty);

    /// <summary>
    /// The token used for GitHub API authentication.
    /// </summary>
    public string GitHubToken { get; set; } = string.Empty;

    /// <summary>
    /// An optional header of the release notes.
    /// </summary>
    public string? Header { get; set; }

    /// <summary>
    /// The format of the release notes.
    /// <para>
    /// Defaults to <see cref="ReleaseNotesFormat.Markdown"/>.
    /// </para>
    /// </summary>
    public ReleaseNotesFormat Format { get; set; }

    /// <summary>
    /// An optional path to a release notes configuration file (JSON or YAML).
    /// </summary>
    /// <remarks>
    /// If not specified, the specified repository root directory and current
    /// working directory will be searched to find a configuration file named
    /// 'GitHubRelease.json|jsonc|yml|yaml'.
    /// <para>
    /// Release notes configuration can also be set programmatically via the
    /// <see cref="Configuration"/> property.
    /// </para>
    /// </remarks>
    public FilePath? ConfigurationFile { get; set; }

    /// <summary>
    /// The release notes configuration to use (optional).
    /// </summary>
    /// <remarks>
    /// If set, this configuration will override any configuration file
    /// specified via the <see cref="ConfigurationFile"/> property or any
    /// 'GitHubRelease.json|jsonc|yml|yaml' file present in the specified
    /// repository root directory or current working directory.
    /// </remarks>
    public ReleaseNotesConfiguration? Configuration { get; set; }

    /// <summary>
    /// An optional regex to be used when looking for the latest git tag.
    /// </summary>
    public Regex? GitTagRegex { get; set; }

    /// <summary>
    /// Whether or not to disable caching and create new release notes.
    /// </summary>
    /// <remarks>
    /// Created release notes are cached to reduce GitHub API calls when
    /// using the release notes for multiple purposes (in GitHub releases
    /// and NuGet package release notes etc.).
    /// <para>
    /// Set this property to <see langword="true"/> to bypass the caching.
    /// </para>
    /// </remarks>
    public bool NoCache { get; set; }

    internal virtual IReleaseNotesFormatter Formatter =>
        Format switch
        {
            ReleaseNotesFormat.Markdown => new DefaultMarkdownFormatter(),
            ReleaseNotesFormat.PlainText => new DefaultPlainTextFormatter(),
            _ => throw new ArgumentOutOfRangeException(
                nameof(Format), Format, $"Unknown release notes format: {Format}")
        };

    internal ReleaseNotesCreator ReleaseNotesCreator
    {
        get
        {
            var repositoryDirectoryInfo = new DirectoryInfo(RepositoryRootDirectory.FullPath);

            if (Configuration != null)
            {
                return new ReleaseNotesCreator(
                    repositoryDirectoryInfo, GitHubToken, Configuration);
            }
            else if (ConfigurationFile != null)
            {
                return new ReleaseNotesCreator(
                    repositoryDirectoryInfo,
                    GitHubToken,
                    new FileInfo(ConfigurationFile.FullPath));
            }
            else
            {
                return new ReleaseNotesCreator(repositoryDirectoryInfo, GitHubToken);
            }
        }
    }

    internal virtual void EnsureValid()
    {
        if (string.IsNullOrWhiteSpace(RepositoryRootDirectory.FullPath))
        {
            throw new ArgumentNullException(
                "Repository root directory must be set", nameof(RepositoryRootDirectory));
        }

        if (string.IsNullOrWhiteSpace(GitHubToken))
        {
            throw new ArgumentException("GitHub token must be set", nameof(GitHubToken));
        }
    }
}
