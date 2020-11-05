using System;
using System.IO;
using GitHubRelease.Notes;

namespace GitHubRelease.Internal
{
    internal static class FileModeExtensions
    {
        public static (FileMode FileMode, FileAccess FileAccess) ToFileAndAccessMode(
                this ReleaseNotesFileOutputMode outputMode) =>
            outputMode switch
            {
                ReleaseNotesFileOutputMode.Overwrite => (FileMode.Create, FileAccess.Write),
                ReleaseNotesFileOutputMode.Append => (FileMode.Append, FileAccess.Write),
                ReleaseNotesFileOutputMode.Prepend => (FileMode.OpenOrCreate, FileAccess.Read),

                _ => throw new ArgumentOutOfRangeException(
                    nameof(outputMode), outputMode, $"Invalid release notes output mode: {outputMode}")
            };
    }
}