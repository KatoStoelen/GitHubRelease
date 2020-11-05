using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GitHubRelease.Internal;

namespace GitHubRelease.Notes.Formatting
{
    /// <summary>
    /// Represents release notes in a specific format.
    /// </summary>
    /// <remarks>
    /// This class supports implicit cast to <see cref="string"/>.
    /// </remarks>
    public class FormattedReleaseNotes
    {
        private readonly string _formattedValue;

        private FormattedReleaseNotes(string formattedValue)
        {
            _formattedValue = formattedValue ?? throw new ArgumentNullException(nameof(formattedValue));
        }

        /// <inheritdoc/>
        public override string? ToString() => _formattedValue;

        /// <summary>
        /// Writes the release notes to a file using UTF-8 (with BOM) encoding.
        /// </summary>
        /// <remarks>
        /// If the specified output file exists it will, by default, be
        /// overwritten. This behavior can be changed using the
        /// <paramref name="outputMode"/> parameter.
        /// </remarks>
        /// <param name="outputFile">The output file.</param>
        /// <param name="outputMode">
        /// The <see cref="ReleaseNotesFileOutputMode"/> to use.
        /// <para>
        /// Defaults to <see cref="ReleaseNotesFileOutputMode.Overwrite"/>.
        /// </para>
        /// </param>
        /// <param name="cancellationToken">An optional token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous write operation.</returns>
        public Task WriteToFileAsync(
                FileInfo outputFile,
                ReleaseNotesFileOutputMode outputMode = ReleaseNotesFileOutputMode.Overwrite,
                CancellationToken cancellationToken = default) =>
            WriteToFileAsync(
                outputFile,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: true),
                outputMode,
                cancellationToken);

        /// <summary>
        /// Writes the release notes to a file using the specified encoding.
        /// </summary>
        /// <remarks>
        /// If the specified output file exists it will, by default, be
        /// overwritten. This behavior can be changed using the
        /// <paramref name="outputMode"/> parameter.
        /// </remarks>
        /// <param name="outputFile">The output file.</param>
        /// <param name="encoding">The <see cref="Encoding"/> to use.</param>
        /// <param name="outputMode">
        /// The <see cref="ReleaseNotesFileOutputMode"/> to use.
        /// <para>
        /// Defaults to <see cref="ReleaseNotesFileOutputMode.Overwrite"/>.
        /// </para>
        /// </param>
        /// <param name="cancellationToken">An optional token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous write operation.</returns>
        public async Task WriteToFileAsync(
            FileInfo outputFile,
            Encoding encoding,
            ReleaseNotesFileOutputMode outputMode = ReleaseNotesFileOutputMode.Overwrite,
            CancellationToken cancellationToken = default)
        {
            var contentBytes = encoding.GetBytes(_formattedValue);

            var (fileMode, fileAccess) = outputMode.ToFileAndAccessMode();

            using var fileStream = outputFile.OpenAsyncFileStream(fileMode, fileAccess);

            if (outputMode != ReleaseNotesFileOutputMode.Prepend)
            {
                await fileStream
                    .WriteAsync(contentBytes, 0, contentBytes.Length, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                var tempFileInfo = new FileInfo(
                    Path.Combine(outputFile.DirectoryName, $"_{outputFile.Name}.tmp"));

                using (var tempFileStream = tempFileInfo.OpenAsyncFileStream())
                {
                    await tempFileStream
                        .WriteAsync(contentBytes, 0, contentBytes.Length, cancellationToken)
                        .ConfigureAwait(false);

                    using (fileStream)
                    {
                        await fileStream
                            .CopyToAsync(tempFileStream, bufferSize: 81920, cancellationToken)
                            .ConfigureAwait(false);
                    }
                }

                _ = tempFileInfo.Replace(outputFile.FullName, null);
            }
        }

        /// <summary>
        /// Creates formatted release notes from a string.
        /// </summary>
        /// <param name="formattedValue">A string containing the formatted release notes.</param>
        /// <returns>Formatted release notes.</returns>
        public static FormattedReleaseNotes FromString(string formattedValue) =>
            new FormattedReleaseNotes(formattedValue);

        /// <summary>
        /// Implicit cast operator to <see cref="string"/>.
        /// </summary>
        /// <param name="notes">The formatted release notes.</param>
        public static implicit operator string(FormattedReleaseNotes? notes) =>
            notes?.ToString() ?? string.Empty;
    }
}