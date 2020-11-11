using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using GitHubRelease.Notes;

namespace GitHubRelease.Cake.Internal
{
    internal static class ReleaseNotesCache
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> s_locks =
            new ConcurrentDictionary<string, SemaphoreSlim>(StringComparer.OrdinalIgnoreCase);
        private static readonly ConcurrentDictionary<string, ReleaseNotes> s_cache =
            new ConcurrentDictionary<string, ReleaseNotes>(StringComparer.OrdinalIgnoreCase);

        public static async Task<ReleaseNotes> GetOrAdd(
            string repositoryAbsolutePath,
            Func<Task<ReleaseNotes>> factory)
        {
            ReleaseNotes releaseNotes;

            var @lock = s_locks.GetOrAdd(repositoryAbsolutePath, _ => new SemaphoreSlim(1));
            await @lock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (!s_cache.TryGetValue(repositoryAbsolutePath, out releaseNotes))
                {
                    releaseNotes = await factory().ConfigureAwait(false);

                    _ = s_cache.TryAdd(repositoryAbsolutePath, releaseNotes);
                }
            }
            finally
            {
                _ = @lock.Release();
            }

            return releaseNotes;
        }
    }
}