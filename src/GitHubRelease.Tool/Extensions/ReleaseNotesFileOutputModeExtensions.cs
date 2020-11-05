using System;
using GitHubRelease.Notes;

namespace GitHubRelease.Tool.Extensions
{
    internal static class ReleaseNotesFileOutputModeExtensions
    {
        public static int ToMarkdownHeadingLevel(this ReleaseNotesFileOutputMode outputMode) =>
            outputMode switch
            {
                ReleaseNotesFileOutputMode.Overwrite => 1,
                ReleaseNotesFileOutputMode.Append => 2,
                ReleaseNotesFileOutputMode.Prepend => 2,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(outputMode), outputMode, $"Unknown output mode: {outputMode}")
            };
    }
}