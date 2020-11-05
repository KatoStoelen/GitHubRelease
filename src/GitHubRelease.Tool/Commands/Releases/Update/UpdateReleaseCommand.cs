using System.CommandLine;
using System.CommandLine.IO;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SystemCommandLineExtensions.AutoOptions;

namespace GitHubRelease.Tool.Commands.Releases.Update
{
    internal class UpdateReleaseCommand : Command<UpdateReleaseOptions>
    {
        public UpdateReleaseCommand()
            : base(
                name: "update",
                description: "Update a GitHub release",
                invokeMethodName: nameof(InvokeAsync))
        {
            // Bug in System.CommandLine: https://github.com/dotnet/command-line-api/issues/911
            this.AddOptions<ReleaseOptions>();
        }

        private async Task InvokeAsync(
            UpdateReleaseOptions options,
            IConsole console,
            CancellationToken cancellationToken)
        {
            options.EnsureValid();

            var releaser = options.Releaser;

            var releaseToUpdate = options.Id.HasValue
                ? await releaser.GetReleaseAsync(options.Id.Value)
                : await releaser.GetReleaseAsync(options.TagName!);

            var updatedRelease = releaseToUpdate.ToUpdate();

            updatedRelease.Name = options.Name ?? updatedRelease.Name;
            updatedRelease.TagName = options.NewTagName ?? updatedRelease.TagName;
            updatedRelease.TargetCommitish = options.TargetCommitish ?? updatedRelease.TargetCommitish;
            updatedRelease.Body = options.Body != null
                ? await File.ReadAllTextAsync(options.Body.FullName, cancellationToken)
                : updatedRelease.Body;
            updatedRelease.IsDraft = options.Draft ?? updatedRelease.IsDraft;
            updatedRelease.IsPrerelease = options.Prerelease ?? updatedRelease.IsPrerelease;
            updatedRelease.DeleteExistingAssets = options.ClearAssets;
            updatedRelease.OverwriteExistingAssets = options.OverwriteAssets;

            foreach (var asset in options.Assets)
            {
                updatedRelease.AddAsset(asset);
            }

            var createdRelease = await releaser.UpdateReleaseAsync(updatedRelease, cancellationToken);

            console.Out.WriteLine($"GitHub release (ID: {createdRelease.Id}) updated: {createdRelease.HtmlUrl}");
        }
    }
}