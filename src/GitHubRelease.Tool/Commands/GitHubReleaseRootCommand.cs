using System.CommandLine;
using GitHubRelease.Tool.Commands.ReleaseNotes;
using GitHubRelease.Tool.Commands.Releases;
using SystemCommandLineExtensions.AutoOptions;

namespace GitHubRelease.Tool.Commands
{
    internal class GitHubReleaseRootCommand : RootCommand
    {
        public GitHubReleaseRootCommand()
            : base("Create release notes based on closed GitHub issues and publish GitHub releases.")
        {
            this.AddGlobalOptions<GlobalOptions>();

            AddCommand(new ReleaseNotesCommand());
            AddCommand(new ReleaseCommand());
        }
    }
}