using System.Text;

namespace GitHubRelease.Internal
{
    internal static class StringBuilderExtensions
    {
        private const char LF = '\n';

        public static StringBuilder AppendLF(this StringBuilder builder, params string[] texts) =>
            builder
                .Append(string.Join(string.Empty, texts))
                .Append(LF);

    }
}
