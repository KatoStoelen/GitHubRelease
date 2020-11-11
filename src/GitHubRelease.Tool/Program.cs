using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using GitHubRelease.Tool.Commands;

namespace GitHubRelease.Tool
{
    internal class Program
    {
        private static Task<int> Main(string[] args) =>
            new CommandLineBuilder(new GitHubReleaseRootCommand())
                .UseDefaults()
                .Build()
                .InvokeAsync(args);
    }
}
