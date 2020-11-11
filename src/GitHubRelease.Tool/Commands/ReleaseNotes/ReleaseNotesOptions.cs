using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using GitHubRelease.Notes;
using GitHubRelease.Notes.Formatting;
using GitHubRelease.Tool.Extensions;
using SystemCommandLineExtensions.AutoOptions;

namespace GitHubRelease.Tool.Commands.ReleaseNotes
{
    internal class ReleaseNotesOptions : GlobalOptions
    {
        [Alias("-r")]
        [DefaultDirectory(".")]
        [Description("The root directory of the git repository")]
        public DirectoryInfo RepositoryDir { get; set; } = new DirectoryInfo(".");

        [Description("An optional header of the release notes")]
        public string? Header { get; set; }

        [Alias("-f")]
        [DefaultValue(ReleaseNotesFormat.Markdown)]
        [Description("The output format of the release notes")]
        public ReleaseNotesFormat Format { get; set; }

        [Alias("-o")]
        [Description("The output file path. If not specified, the release notes are written to STDOUT")]
        public FileInfo? OutputFile { get; set; }

        [Alias("-m")]
        [DefaultValue(ReleaseNotesFileOutputMode.Overwrite)]
        [Description("Whether output-file should be overwritten, appended or prepended to")]
        public ReleaseNotesFileOutputMode OutputMode { get; set; }

        [Alias("-e")]
        [DefaultValue("utf-8")]
        [Description("The encoding to use when writing release notes to a file")]
        public string OutputEncoding { get; set; } = string.Empty;

        [Alias("-t")]
        [Description("The token used for GitHub API authentication")]
        public string GithubToken { get; set; } = string.Empty;

        [Alias("-g")]
        [Description(
            "A regex to use when looking for the git tag representing the previous release. " +
            "If not specified, the most recent tag is considered as the previous release")]
        public string? GitTagRegex { get; set; }

        [Alias("-c")]
        [Description(
            "The release notes configuration file to use. If not specified, the repository " +
            "root directory and current directory are searched to find one")]
        public FileInfo? ConfigurationFile { get; set; }

        [NotAnOption]
        public IReleaseNotesFormatter Formatter =>
            Format switch
            {
                ReleaseNotesFormat.Markdown => new DefaultMarkdownFormatter(
                    OutputMode.ToMarkdownHeadingLevel()),

                ReleaseNotesFormat.PlainText => new DefaultPlainTextFormatter(),

                _ => throw new ArgumentOutOfRangeException(
                    nameof(Format),
                    Format,
                    $"Unknown release notes format: {Format}")
            };

        [NotAnOption]
        public Encoding Encoding => Encoding.GetEncoding(OutputEncoding);

        [NotAnOption]
        public Regex? TagRegex => !string.IsNullOrEmpty(GitTagRegex)
            ? new Regex(GitTagRegex)
            : null;
    }
}