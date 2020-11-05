using System.CommandLine;
using System.CommandLine.IO;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GitHubRelease.Releases;
using SystemCommandLineExtensions.AutoOptions;

namespace GitHubRelease.Tool.Commands.Releases.Create
{
    internal class CreateReleaseCommand : Command<CreateReleaseOptions>
    {
        public CreateReleaseCommand()
            : base(
                name: "create",
                description: "Create a GitHub release",
                invokeMethodName: nameof(InvokeAsync))
        {
            // Bug in System.CommandLine: https://github.com/dotnet/command-line-api/issues/911
            this.AddOptions<ReleaseOptions>();
        }

        private async Task InvokeAsync(
            CreateReleaseOptions options,
            IConsole console,
            CancellationToken cancellationToken)
        {
            options.EnsureValid();

            var newRelease = new NewRelease
            {
                Name = options.Name,
                TagName = options.TagName,
                TargetCommitish = options.TargetCommitish,
                Body = options.Body != null
                    ? await File.ReadAllTextAsync(options.Body.FullName, cancellationToken)
                    : string.Empty,
                IsPrerelease = options.Prerelease,
                IsDraft = options.Draft
            };

            foreach (var asset in options.Assets)
            {
                newRelease.AddAsset(asset);
            }

            var createdRelease = await options.Releaser.CreateReleaseAsync(newRelease, cancellationToken);

            console.Out.WriteLine($"New GitHub release (ID: {createdRelease.Id}) created: {createdRelease.HtmlUrl}");
        }
    }
}