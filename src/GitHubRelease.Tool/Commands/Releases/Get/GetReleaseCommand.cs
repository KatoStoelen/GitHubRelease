using System.CommandLine;
using System.CommandLine.IO;
using System.Linq;
using System.Threading.Tasks;
using GitHubRelease.Releases;
using GitHubRelease.Tool.Extensions;
using SystemCommandLineExtensions.AutoOptions;

namespace GitHubRelease.Tool.Commands.Releases.Get
{
    internal class GetReleaseCommand : Command<GetReleaseOptions>
    {
        public GetReleaseCommand()
            : base(
                name: "get",
                description: "Get GitHub releases",
                invokeMethodName: nameof(InvokeAsync))
        {
            // Bug in System.CommandLine: https://github.com/dotnet/command-line-api/issues/911
            this.AddOptions<ReleaseOptions>();
        }

        private async Task InvokeAsync(GetReleaseOptions options, IConsole console)
        {
            options.EnsureValid();

            var releaser = options.Releaser;

            if (options.All)
            {
                var releases = await releaser.GetAllReleasesAsync();

                if (!releases.Any())
                {
                    console.Out.WriteLine("(no releases)");
                }
                else
                {
                    foreach (var release in releases)
                    {
                        Dump(release, console);

                        console.Out.WriteLine();
                    }
                }
            }
            else
            {
                var release = options.Id.HasValue
                    ? await releaser.GetReleaseAsync(options.Id.Value)
                    : await releaser.GetReleaseAsync(options.TagName!);

                Dump(release, console);
            }
        }

        private static void Dump(Release release, IConsole console)
        {
            console.Out.WriteLine($"[Release #{release.Id}]");
            console.Out.WriteLineIndented($"Name: {release.Name}");
            console.Out.WriteLineIndented($"TagName: {release.TagName}");
            console.Out.WriteLineIndented($"TargetCommitish: {release.TargetCommitish}");
            console.Out.WriteLineIndented($"IsPrerelease: {release.IsPrerelease}");
            console.Out.WriteLineIndented($"IsDraft: {release.IsDraft}");
            console.Out.WriteLineIndented($"HtmlUrl: {release.HtmlUrl}");
            console.Out.WriteLineIndented($"CreatedAt: {release.CreatedAt}");
            console.Out.WriteLineIndented($"PublishedAt: {release.PublishedAt?.ToString() ?? "null"}");

            if (!release.Assets.Any())
            {
                console.Out.WriteLineIndented($"Assets: (none)");
            }
            else
            {
                console.Out.WriteLineIndented($"Assets:");

                foreach (var asset in release.Assets)
                {
                    console.Out.WriteLineIndented($"- [Asset #{asset.Id}]", level: 2);
                    console.Out.WriteLineIndented($"Name: {asset.Name}", level: 3);
                }
            }

            if (string.IsNullOrEmpty(release.Body))
            {
                console.Out.WriteLineIndented($"Body: (empty)");
            }
            else
            {
                console.Out.WriteLineIndented("Body:");
                console.Out.WriteLine(release.Body);
            }
        }
    }
}