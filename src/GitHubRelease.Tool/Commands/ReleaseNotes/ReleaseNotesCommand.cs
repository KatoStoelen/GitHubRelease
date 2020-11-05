using System.CommandLine;
using System.CommandLine.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SystemCommandLineExtensions.AutoOptions;

namespace GitHubRelease.Tool.Commands.ReleaseNotes
{
    internal class ReleaseNotesCommand : Command<ReleaseNotesOptions>
    {
        public ReleaseNotesCommand()
            : base(
                name: "notes",
                description: "Create release notes",
                invokeMethodName: nameof(InvokeAsync))
        {
        }

        private async Task InvokeAsync(
            ReleaseNotesOptions options,
            IConsole console,
            CancellationToken cancellationToken)
        {
            var creator = options.ConfigurationFile != null
                ? new ReleaseNotesCreator(
                    options.RepositoryDir, options.GithubToken, options.ConfigurationFile)
                : new ReleaseNotesCreator(options.RepositoryDir, options.GithubToken);

            var releaseNotes = await creator.CreateReleaseNotesAsync(options.TagRegex, cancellationToken);
            var formattedReleaseNotes = releaseNotes.Format(options.Formatter, options.Header);

            if (options.OutputFile != null)
            {
                await formattedReleaseNotes.WriteToFileAsync(
                    options.OutputFile,
                    options.Encoding,
                    options.OutputMode,
                    cancellationToken);

                console.Out.WriteLine($"Release notes written to {options.OutputFile}");
            }
            else
            {
                console.Out.WriteLine(formattedReleaseNotes);
            }
        }
    }
}