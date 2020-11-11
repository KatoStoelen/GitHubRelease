using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;
using GitHubRelease.Cake.Internal;
using GitHubRelease.Notes;
using GitHubRelease.Notes.Formatting;
using GitHubRelease.Releases;

/// <summary>
/// Cake extensions for using KatoStoelen.GitHubRelease.
/// </summary>
public static class GitHubReleaseCakeExtension
{
    /// <summary>
    /// Creates release notes of issues linked in commits added since the latest git tag.
    /// <para>
    /// The result is returned as a string of the specified <see cref="ReleaseNotesFormat"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// If no git tag is found, the commit traversal starts from the first commit
    /// in the repository.
    /// <para>
    /// If you tag quite often, e.g. once per build, and want to traverse commits
    /// from the latest tag of a specific format, you can set
    /// <see cref="ReleaseNotesSettings.GitTagRegex"/> which will then be used when looking
    /// up the latest tag.
    /// </para>
    /// </remarks>
    /// <param name="context">The Cake context.</param>
    /// <param name="settings">The release notes settings.</param>
    /// <returns>The release notes in the sepcified <see cref="ReleaseNotesFormat"/>.</returns>
    [CakeMethodAlias]
    public static string GetReleaseNotes(
            this ICakeContext context, ReleaseNotesSettings settings) =>
        context
            .GetReleaseNotesAsync(settings)
            .GetAwaiter()
            .GetResult();

    /// <summary>
    /// Creates release notes of issues linked in commits added since the latest git tag.
    /// <para>
    /// The result is returned as a string of the specified <see cref="ReleaseNotesFormat"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// If no git tag is found, the commit traversal starts from the first commit
    /// in the repository.
    /// <para>
    /// If you tag quite often, e.g. once per build, and want to traverse commits
    /// from the latest tag of a specific format, you can set
    /// <see cref="ReleaseNotesSettings.GitTagRegex"/> which will then be used when looking
    /// up the latest tag.
    /// </para>
    /// </remarks>
    /// <param name="context">The Cake context.</param>
    /// <param name="settings">The release notes settings.</param>
    /// <returns>The release notes in the sepcified <see cref="ReleaseNotesFormat"/>.</returns>
    [CakeMethodAlias]
    public static Task<string> GetReleaseNotesAsync(
            this ICakeContext context, ReleaseNotesSettings settings) =>
        context
            .GetReleaseNotesAsync(settings, CancellationToken.None);

    /// <summary>
    /// Creates release notes of issues linked in commits added since the latest git tag.
    /// <para>
    /// The result is returned as a string of the specified <see cref="ReleaseNotesFormat"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// If no git tag is found, the commit traversal starts from the first commit
    /// in the repository.
    /// <para>
    /// If you tag quite often, e.g. once per build, and want to traverse commits
    /// from the latest tag of a specific format, you can set
    /// <see cref="ReleaseNotesSettings.GitTagRegex"/> which will then be used when looking
    /// up the latest tag.
    /// </para>
    /// </remarks>
    /// <param name="context">The Cake context.</param>
    /// <param name="settings">The release notes settings.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The release notes in the sepcified <see cref="ReleaseNotesFormat"/>.</returns>
    [CakeMethodAlias]
    public static async Task<string> GetReleaseNotesAsync(
        this ICakeContext context,
        ReleaseNotesSettings settings,
        CancellationToken cancellationToken)
    {
        return await context
            .GetFormattedReleaseNotesAsync(settings, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Creates release notes of issues linked in commits added since the latest git tag.
    /// <para>
    /// The result written to <see cref="OutputReleaseNotesSettings.OutputFile"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// If no git tag is found, the commit traversal starts from the first commit
    /// in the repository.
    /// <para>
    /// If you tag quite often, e.g. once per build, and want to traverse commits
    /// from the latest tag of a specific format, you can set
    /// <see cref="ReleaseNotesSettings.GitTagRegex"/> which will then be used when looking
    /// up the latest tag.
    /// </para>
    /// </remarks>
    /// <param name="context">The Cake context.</param>
    /// <param name="settings">The create release notes settings.</param>
    [CakeMethodAlias]
    public static void CreateReleaseNotes(
            this ICakeContext context,
            OutputReleaseNotesSettings settings) =>
        context
            .CreateReleaseNotesAsync(settings)
            .GetAwaiter()
            .GetResult();

    /// <summary>
    /// Creates release notes of issues linked in commits added since the latest git tag.
    /// <para>
    /// The result written to <see cref="OutputReleaseNotesSettings.OutputFile"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// If no git tag is found, the commit traversal starts from the first commit
    /// in the repository.
    /// <para>
    /// If you tag quite often, e.g. once per build, and want to traverse commits
    /// from the latest tag of a specific format, you can set
    /// <see cref="ReleaseNotesSettings.GitTagRegex"/> which will then be used when looking
    /// up the latest tag.
    /// </para>
    /// </remarks>
    /// <param name="context">The Cake context.</param>
    /// <param name="settings">The create release notes settings.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous release notes creation.</returns>
    [CakeMethodAlias]
    public static Task CreateReleaseNotesAsync(
            this ICakeContext context,
            OutputReleaseNotesSettings settings) =>
        context
            .CreateReleaseNotesAsync(settings, CancellationToken.None);

    /// <summary>
    /// Creates release notes of issues linked in commits added since the latest git tag.
    /// <para>
    /// The result written to <see cref="OutputReleaseNotesSettings.OutputFile"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// If no git tag is found, the commit traversal starts from the first commit
    /// in the repository.
    /// <para>
    /// If you tag quite often, e.g. once per build, and want to traverse commits
    /// from the latest tag of a specific format, you can set
    /// <see cref="ReleaseNotesSettings.GitTagRegex"/> which will then be used when looking
    /// up the latest tag.
    /// </para>
    /// </remarks>
    /// <param name="context">The Cake context.</param>
    /// <param name="settings">The create release notes settings.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous release notes creation.</returns>
    [CakeMethodAlias]
    public static async Task CreateReleaseNotesAsync(
        this ICakeContext context,
        OutputReleaseNotesSettings settings,
        CancellationToken cancellationToken)
    {
        var formattedReleaseNotes = await context
            .GetFormattedReleaseNotesAsync(settings, cancellationToken)
            .ConfigureAwait(false);

        await formattedReleaseNotes
            .WriteToFileAsync(
                new FileInfo(settings.OutputFile.FullPath),
                settings.OutputEncoding,
                settings.OutputMode,
                cancellationToken)
            .ConfigureAwait(false);
    }

    private static async Task<FormattedReleaseNotes> GetFormattedReleaseNotesAsync(
        this ICakeContext context, ReleaseNotesSettings settings, CancellationToken cancellationToken)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        settings.EnsureValid();

        var repositoryAbsolutePath = settings.RepositoryRootDirectory
            .MakeAbsolute(context.Environment.WorkingDirectory)
            .FullPath;

        var releaseNotes =
            settings.NoCache
                ? await CreateReleaseNotes().ConfigureAwait(false)
                : await ReleaseNotesCache
                    .GetOrAdd(repositoryAbsolutePath, CreateReleaseNotes)
                    .ConfigureAwait(false);

        return releaseNotes.Format(settings.Formatter, settings.Header);

        Task<ReleaseNotes> CreateReleaseNotes()
        {
            return settings.ReleaseNotesCreator
                .CreateReleaseNotesAsync(settings.GitTagRegex, cancellationToken);
        }
    }

    /// <summary>
    /// Gets a GitHub release by ID.
    /// </summary>
    /// <param name="context">The cake context.</param>
    /// <param name="id">The ID of the release.</param>
    /// <param name="settings">The GitHubRelease settings.</param>
    /// <returns>A <see cref="Release"/> representing the GitHub release.</returns>
    [CakeMethodAlias]
    public static Release GetRelease(
            this ICakeContext context, int id, GitHubReleaseSettings settings) =>
        context
            .GetReleaseAsync(id, settings)
            .GetAwaiter()
            .GetResult();

    /// <summary>
    /// Gets a GitHub release by ID.
    /// </summary>
    /// <param name="_">The cake context.</param>
    /// <param name="id">The ID of the release.</param>
    /// <param name="settings">The GitHubRelease settings.</param>
    /// <returns>A <see cref="Release"/> representing the GitHub release.</returns>
    [CakeMethodAlias]
    public static Task<Release> GetReleaseAsync(
        this ICakeContext _, int id, GitHubReleaseSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        settings.EnsureValid();

        return settings.Releaser.GetReleaseAsync(id);
    }

    /// <summary>
    /// Gets a GitHub release by tag name.
    /// </summary>
    /// <remarks>
    /// If the specified tag does not exist in the GitHub repository,
    /// the release will not be found, even though a release with this
    /// tag name exists. This usually happens when the release is a
    /// draft.
    /// </remarks>
    /// <param name="context">The cake context.</param>
    /// <param name="tagName">The tag name of the release.</param>
    /// <param name="settings">The GitHubRelease settings.</param>
    /// <returns>A <see cref="Release"/> representing the GitHub release.</returns>
    [CakeMethodAlias]
    public static Release GetRelease(
            this ICakeContext context, string tagName, GitHubReleaseSettings settings) =>
        context
            .GetReleaseAsync(tagName, settings)
            .GetAwaiter()
            .GetResult();

    /// <summary>
    /// Gets a GitHub release by tag name.
    /// </summary>
    /// <remarks>
    /// If the specified tag does not exist in the GitHub repository,
    /// the release will not be found, even though a release with this
    /// tag name exists. This usually happens when the release is a
    /// draft.
    /// </remarks>
    /// <param name="_">The cake context.</param>
    /// <param name="tagName">The tag name of the release.</param>
    /// <param name="settings">The GitHubRelease settings.</param>
    /// <returns>A <see cref="Release"/> representing the GitHub release.</returns>
    [CakeMethodAlias]
    public static Task<Release> GetReleaseAsync(
        this ICakeContext _, string tagName, GitHubReleaseSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        settings.EnsureValid();

        return settings.Releaser.GetReleaseAsync(tagName);
    }

    /// <summary>
    /// Gets all GitHub releases in the specified repository.
    /// </summary>
    /// <param name="context">The cake context.</param>
    /// <param name="settings">The GitHubRelease settings.</param>
    /// <returns>A collection of <see cref="Release"/> representing all GitHub releases.</returns>
    [CakeMethodAlias]
    public static IReadOnlyCollection<Release> GetAllReleases(
            this ICakeContext context, GitHubReleaseSettings settings) =>
        context
            .GetAllReleasesAsync(settings)
            .GetAwaiter()
            .GetResult();

    /// <summary>
    /// Gets all GitHub releases in the specified repository.
    /// </summary>
    /// <param name="_">The cake context.</param>
    /// <param name="settings">The GitHubRelease settings.</param>
    /// <returns>A collection of <see cref="Release"/> representing all GitHub releases.</returns>
    [CakeMethodAlias]
    public static Task<IReadOnlyCollection<Release>> GetAllReleasesAsync(
        this ICakeContext _, GitHubReleaseSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        settings.EnsureValid();

        return settings.Releaser.GetAllReleasesAsync();
    }

    /// <summary>
    /// Creates a new GitHub release.
    /// </summary>
    /// <param name="context">The Cake context.</param>
    /// <param name="settings">The create new release settings.</param>
    /// <returns>A <see cref="Release"/> representing the newly created GitHub release.</returns>
    [CakeMethodAlias]
    public static Release CreateGitHubRelease(
            this ICakeContext context,
            NewGitHubReleaseSettings settings) =>
        context
            .CreateGitHubReleaseAsync(settings)
            .GetAwaiter()
            .GetResult();

    /// <summary>
    /// Creates a new GitHub release.
    /// </summary>
    /// <param name="context">The Cake context.</param>
    /// <param name="settings">The create new release settings.</param>
    /// <returns>A <see cref="Release"/> representing the newly created GitHub release.</returns>
    [CakeMethodAlias]
    public static Task<Release> CreateGitHubReleaseAsync(
            this ICakeContext context,
            NewGitHubReleaseSettings settings) =>
        context.CreateGitHubReleaseAsync(settings, CancellationToken.None);

    /// <summary>
    /// Creates a new GitHub release.
    /// </summary>
    /// <param name="context">The Cake context.</param>
    /// <param name="settings">The create new release settings.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Release"/> representing the newly created GitHub release.</returns>
    [CakeMethodAlias]
    public static Task<Release> CreateGitHubReleaseAsync(
        this ICakeContext context,
        NewGitHubReleaseSettings settings,
        CancellationToken cancellationToken)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        settings.EnsureValid();

        return settings.Releaser.CreateReleaseAsync(
            new NewRelease
            {
                Name = settings.Name,
                TagName = settings.TagName,
                TargetCommitish = settings.TargetCommitish,
                Body = settings.GetBody(context.FileSystem),
                IsDraft = settings.IsDraft,
                IsPrerelease = settings.IsPrerelease,
                Assets = settings.AssetFileInfos
            },
            cancellationToken);
    }

    /// <summary>
    /// Updates a GitHub release.
    /// </summary>
    /// <param name="context">The cake context.</param>
    /// <param name="settings">The update release settings.</param>
    /// <returns>A <see cref="Release"/> representing the updated GitHub release.</returns>
    [CakeMethodAlias]
    public static Release UpdateGitHubRelease(
            this ICakeContext context,
            UpdateGitHubReleaseSettings settings) =>
        context
            .UpdateGitHubReleaseAsync(settings, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

    /// <summary>
    /// Updates a GitHub release.
    /// </summary>
    /// <param name="context">The cake context.</param>
    /// <param name="settings">The update release settings.</param>
    /// <returns>A <see cref="Release"/> representing the updated GitHub release.</returns>
    [CakeMethodAlias]
    public static Task<Release> UpdateGitHubReleaseAsync(
            this ICakeContext context,
            UpdateGitHubReleaseSettings settings) =>
        context
            .UpdateGitHubReleaseAsync(settings, CancellationToken.None);

    /// <summary>
    /// Updates a GitHub release.
    /// </summary>
    /// <param name="context">The cake context.</param>
    /// <param name="settings">The update release settings.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Release"/> representing the updated GitHub release.</returns>
    [CakeMethodAlias]
    public static Task<Release> UpdateGitHubReleaseAsync(
        this ICakeContext context,
        UpdateGitHubReleaseSettings settings,
        CancellationToken cancellationToken)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        settings.EnsureValid();
        settings.SetAssets();
        settings.SetBody(context.FileSystem);

        return settings.Releaser.UpdateReleaseAsync(
            settings.UpdateRelease, cancellationToken);
    }

    /// <summary>
    /// Deletes a GitHub release by ID.
    /// </summary>
    /// <param name="context">The cake context.</param>
    /// <param name="id">The ID of the release.</param>
    /// <param name="settings">The GitHubRelease settings.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    [CakeMethodAlias]
    public static void DeleteGitHubRelease(
            this ICakeContext context, int id, GitHubReleaseSettings settings) =>
        context
            .DeleteGitHubReleaseAsync(id, settings)
            .GetAwaiter()
            .GetResult();

    /// <summary>
    /// Deletes a GitHub release by ID.
    /// </summary>
    /// <param name="_">The cake context.</param>
    /// <param name="id">The ID of the release.</param>
    /// <param name="settings">The GitHubRelease settings.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    [CakeMethodAlias]
    public static Task DeleteGitHubReleaseAsync(
        this ICakeContext _, int id, GitHubReleaseSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        settings.EnsureValid();

        return settings.Releaser.DeleteReleaseAsync(id);
    }

    /// <summary>
    /// Deletes a GitHub release by tag name.
    /// </summary>
    /// <remarks>
    /// If the specified tag does not exist in the GitHub repository,
    /// the release will not be found, even though a release with this
    /// tag name exists. This usually happens when the release is a
    /// draft.
    /// </remarks>
    /// <param name="context">The cake context.</param>
    /// <param name="tagName">The tag name of the release.</param>
    /// <param name="settings">The GitHubRelease settings.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    [CakeMethodAlias]
    public static void DeleteGitHubRelease(
            this ICakeContext context, string tagName, GitHubReleaseSettings settings) =>
        context
            .DeleteGitHubReleaseAsync(tagName, settings)
            .GetAwaiter()
            .GetResult();

    /// <summary>
    /// Deletes a GitHub release by tag name.
    /// </summary>
    /// <remarks>
    /// If the specified tag does not exist in the GitHub repository,
    /// the release will not be found, even though a release with this
    /// tag name exists. This usually happens when the release is a
    /// draft.
    /// </remarks>
    /// <param name="_">The cake context.</param>
    /// <param name="tagName">The tag name of the release.</param>
    /// <param name="settings">The GitHubRelease settings.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    [CakeMethodAlias]
    public static Task DeleteGitHubReleaseAsync(
        this ICakeContext _, string tagName, GitHubReleaseSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        settings.EnsureValid();

        return settings.Releaser.DeleteReleaseAsync(tagName);
    }
}