using System.CommandLine;
using GitHubRelease.Tool.Commands.Releases.Create;
using GitHubRelease.Tool.Commands.Releases.Delete;
using GitHubRelease.Tool.Commands.Releases.Get;
using GitHubRelease.Tool.Commands.Releases.Update;

namespace GitHubRelease.Tool.Commands.Releases
{
    internal class ReleaseCommand : Command
    {
        public ReleaseCommand()
            : base("release", "Manage GitHub releases")
        {
            // Bug in System.CommandLine: https://github.com/dotnet/command-line-api/issues/911
            // this.AddGlobalOptions<ReleaseOptions>();

            AddCommand(new CreateReleaseCommand());
            AddCommand(new GetReleaseCommand());
            AddCommand(new UpdateReleaseCommand());
            AddCommand(new DeleteReleaseCommand());
        }
    }
}