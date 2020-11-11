using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Core.IO;
using GitHubRelease.Releases;

/// <summary>
/// Settings for updating a GitHub release.
/// </summary>
public class UpdateGitHubReleaseSettings : GitHubReleaseSettings
{

    /// <summary>
    /// Initializes update release settings from the specified
    /// GitHub release.
    /// </summary>
    /// <param name="release">The GitHub release to update.</param>
    public UpdateGitHubReleaseSettings(Release release)
    {
        UpdateRelease = release.ToUpdate();
    }

    /// <summary>
    /// The new name of the release.
    /// </summary>
    public string Name
    {
        get => UpdateRelease.Name;
        set => UpdateRelease.Name = value;
    }

    /// <summary>
    /// The new tag name of the release.
    /// </summary>
    public string TagName
    {
        get => UpdateRelease.TagName;
        set => UpdateRelease.TagName = value;
    }

    /// <summary>
    /// The new commitish value to specify the target of the tag.
    /// <para>
    /// Defaults to the repository's default branch.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Unused if the tag (<see cref="TagName"/>) already exists.
    /// </remarks>
    public string TargetCommitish
    {
        get => UpdateRelease.TargetCommitish;
        set => UpdateRelease.TargetCommitish = value;
    }

    /// <summary>
    /// A string containing the new body of the release.
    /// </summary>
    /// <remarks>
    /// Markdown is supported.
    /// </remarks>
    public string? Body { get; set; }

    /// <summary>
    /// The path to the file containing new the body of the release.
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
    public bool? IsDraft
    {
        get => UpdateRelease.IsDraft;
        set => UpdateRelease.IsDraft = value;
    }

    /// <summary>
    /// Whether or not the release is a pre-release.
    /// </summary>
    public bool? IsPrerelease
    {
        get => UpdateRelease.IsPrerelease;
        set => UpdateRelease.IsPrerelease = value;
    }

    /// <summary>
    /// A collection of assets to include in the release update.
    /// </summary>
    public ICollection<FilePath> NewAssets { get; set; } = new List<FilePath>();

    /// <summary>
    /// Whether or not to delete all the existing assets for this release.
    /// <para>
    /// Defaults to <see langword="false"/>.
    /// </para>
    /// </summary>
    public bool DeleteExistingAssets
    {
        get => UpdateRelease.DeleteExistingAssets;
        set => UpdateRelease.DeleteExistingAssets = value;
    }

    /// <summary>
    /// Whether or not to overwrite existing assets that has the same name
    /// as assets added to this update.
    /// <para>
    /// Setting this to <see langword="false"/> will cause exceptions to be
    /// thrown if any asset in this update matches an existing asset.
    /// </para>
    /// <para>
    /// Defaults to <see langword="true"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This setting only has an effect if this update contains any assets
    /// and <see cref="DeleteExistingAssets"/> is set to <see langword="false"/>.
    /// </remarks>
    public bool OverwriteExistingAssets
    {
        get => UpdateRelease.OverwriteExistingAssets;
        set => UpdateRelease.OverwriteExistingAssets = value;
    }

    internal UpdateRelease UpdateRelease { get; }

    internal void SetAssets()
    {
        UpdateRelease.NewAssets = NewAssets
            .Select(path => new FileInfo(path.FullPath))
            .ToList();
    }

    internal void SetBody(IFileSystem fileSystem)
    {
        if (Body != null)
        {
            UpdateRelease.Body = Body;
        }
        else if (BodyFile != null)
        {
            var file = fileSystem.GetFile(BodyFile);

            using (var fileStream = file.OpenRead())
            using (var reader = new StreamReader(fileStream))
            {
                UpdateRelease.Body = reader.ReadToEnd();
            }
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
