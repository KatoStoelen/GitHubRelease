using System.Collections.Generic;

namespace GitHubRelease.Notes
{
    /// <summary>
    /// Represents a contributor (GitHub user).
    /// </summary>
    public class GitHubContributor
    {
        internal static readonly IEqualityComparer<GitHubContributor?> EqualityComparer = new Comparer();

        internal GitHubContributor(string login, string url)
        {
            Login = login;
            Url = url;
        }

        /// <summary>
        /// The GitHub user login.
        /// </summary>
        public string Login { get; }

        /// <summary>
        /// The URL to the user's profile page.
        /// </summary>
        public string Url { get; }

        /// <inheritdoc/>
        public override bool Equals(object? obj) =>
            obj is GitHubContributor contributor &&
            Login == contributor.Login;

        /// <inheritdoc/>
        public override int GetHashCode() => Login.GetHashCode();

        private class Comparer : IEqualityComparer<GitHubContributor?>
        {
            public bool Equals(GitHubContributor? contributor, GitHubContributor? other) =>
                (contributor == null && other == null) ||
                (contributor != null && contributor.Equals(other));

            public int GetHashCode(GitHubContributor? contributor) => contributor?.GetHashCode() ?? 0;
        }
    }
}
