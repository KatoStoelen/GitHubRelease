using System;
using System.IO;
using Cake.Core.IO;
using GitHubRelease;

/// <summary>
/// Base settings of GitHub release management.
/// </summary>
public class GitHubReleaseSettings
{
    /// <summary>
    /// The token used for GitHub API authentication.
    /// </summary>
    public string GitHubToken { get; set; } = string.Empty;

    /// <summary>
    /// The name of the user or organization owning the target
    /// GitHub repository.
    /// </summary>
    /// <remarks>
    /// To determine this value based on the remotes of the local
    /// git repository, set <see cref="RepositoryRootDirectory"/>
    /// instead.
    /// </remarks>
    public string? RepositoryOwner { get; set; }

    /// <summary>
    /// The name of the target repository.
    /// </summary>
    /// <remarks>
    /// To determine this value based on the remotes of the local
    /// git repository, set <see cref="RepositoryRootDirectory"/>
    /// instead.
    /// </remarks>
    public string? RepositoryName { get; set; }

    /// <summary>
    /// The root directory of the local git repository (optional).
    /// <para>
    /// Used to determine target GitHub repository owner and name
    /// by looking at the remotes of the local git repository.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Can be set as an alternative of setting
    /// <see cref="RepositoryOwner"/> and <see cref="RepositoryName"/>.
    /// </remarks>
    public DirectoryPath? RepositoryRootDirectory { get; set; }

    internal Releaser Releaser =>
        RepositoryRootDirectory != null
            ? new Releaser(new DirectoryInfo(RepositoryRootDirectory.FullPath), GitHubToken)
            : new Releaser(RepositoryOwner!, RepositoryName!, GitHubToken);

    internal virtual void EnsureValid()
    {
        if (string.IsNullOrWhiteSpace(GitHubToken))
        {
            throw new ArgumentException("GitHub token must be set.", nameof(GitHubToken));
        }

        if (RepositoryRootDirectory == null &&
            (string.IsNullOrWhiteSpace(RepositoryOwner) || string.IsNullOrWhiteSpace(RepositoryName)))
        {
            throw new ArgumentException("Either repository root directory or repository owner AND name must be set.");
        }
    }
}
