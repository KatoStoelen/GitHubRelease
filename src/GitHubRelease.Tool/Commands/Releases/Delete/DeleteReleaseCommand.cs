using System.CommandLine;
using System.CommandLine.IO;
using System.Threading.Tasks;
using SystemCommandLineExtensions.AutoOptions;

namespace GitHubRelease.Tool.Commands.Releases.Delete
{
    internal class DeleteReleaseCommand : Command<DeleteReleaseOptions>
    {
        public DeleteReleaseCommand()
            : base(
                name: "delete",
                description: "Delete a GitHub release",
                invokeMethodName: nameof(InvokeAsync))
        {
            // Bug in System.CommandLine: https://github.com/dotnet/command-line-api/issues/911
            this.AddOptions<ReleaseOptions>();
        }

        private async Task InvokeAsync(DeleteReleaseOptions options, IConsole console)
        {
            options.EnsureValid();

            var releaser = options.Releaser;

            if (options.Id.HasValue)
            {
                await releaser.DeleteReleaseAsync(options.Id.Value);

                console.Out.WriteLine($"Release with ID '{options.Id}' deleted");
            }
            else
            {
                await releaser.DeleteReleaseAsync(options.TagName!);

                console.Out.WriteLine($"Release with tag name '{options.TagName}' deleted");
            }
        }
    }
}