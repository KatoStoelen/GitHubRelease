using System;
using System.CommandLine.IO;

namespace GitHubRelease.Tool.Extensions
{
    internal static class StandardStreamWriterExtensions
    {
        private const int DefaultIndent = 4;

        public static void WriteIndented(
                this IStandardStreamWriter writer,
                string text,
                int level = 1,
                int indent = DefaultIndent) =>
            WriteIndented(s => writer.Write(s), text, level, indent);

        public static void WriteLineIndented(
                this IStandardStreamWriter writer,
                string text,
                int level = 1,
                int indent = DefaultIndent) =>
            WriteIndented(s => writer.WriteLine(s), text, level, indent);

        private static void WriteIndented(
            Action<string> write,
            string text,
            int level,
            int indent)
        {
            var padding = new string(' ', level * indent);

            write(padding + text);
        }
    }
}