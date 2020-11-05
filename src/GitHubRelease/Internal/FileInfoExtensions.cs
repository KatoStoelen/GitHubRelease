using System.IO;

namespace GitHubRelease.Internal
{
    internal static class FileInfoExtensions
    {
        public static FileStream OpenAsyncFileStream(
                this FileInfo fileInfo,
                FileMode fileMode = FileMode.Create,
                FileAccess fileAccess = FileAccess.Write) =>
            new FileStream(
                fileInfo.FullName,
                fileMode,
                fileAccess,
                FileShare.None,
                bufferSize: 4096,
                useAsync: true);
    }
}