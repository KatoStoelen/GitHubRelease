using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Core.IO;

/// <summary>
/// Settings for creating a new GitHub release.
/// </summary>
public class NewGitHubReleaseSettings : GitHubReleaseSettings
{
    /// <summary>
    /// The name of the release.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The tag name of the release (required).
    /// </summary>
    public string TagName { get; set; } = string.Empty;

    /// <summary>
    /// A commitish value to specify the target of the tag.
    /// <para>
    /// Defaults to the repository's default branch.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Unused if the tag (<see cref="TagName"/>) already exists.
    /// </remarks>
    public string TargetCommitish { get; set; } = string.Empty;

    /// <summary>
    /// A string containing the body of the release.
    /// </summary>
    /// <remarks>
    /// Markdown is supported.
    /// </remarks>
    public string? Body { get; set; }

    /// <summary>
    /// The path to the file containing the body of the release.
    /// </summary>
    /// <remarks>
    /// An alternative of setting <see cref="Body"/>.
    /// </remarks>
    public FilePath? BodyFile { get; set; }

    /// <summary>
    /// Whether or not the release is in draft state.
    /// <para>
    /// Defaults to <see langword="true"/>.
    /// </para>
    /// </summary>
    public bool IsDraft { get; set; } = true;

    /// <summary>
    /// Whether or not the release is a pre-release.
    /// </summary>
    public bool IsPrerelease { get; set; }

    /// <summary>
    /// A collection of assets to include in the release.
    /// </summary>
    public ICollection<FilePath> Assets { get; set; } = new List<FilePath>();

    internal ICollection<FileInfo> AssetFileInfos =>
        Assets
            .Select(path => new FileInfo(path.FullPath))
            .ToList();

    internal string GetBody(IFileSystem fileSystem)
    {
        if (Body != null)
        {
            return Body;
        }
        else if (BodyFile != null)
        {
            var file = fileSystem.GetFile(BodyFile);

            using (var fileStream = file.OpenRead())
            using (var reader = new StreamReader(fileStream))
            {
                return reader.ReadToEnd();
            }
        }
        else
        {
            return string.Empty;
        }
    }

    internal override void EnsureValid()
    {
        base.EnsureValid();

        if (string.IsNullOrWhiteSpace(TagName))
        {
            throw new ArgumentException("Tag name must be set.", nameof(TagName));
        }
    }
}
