using System;
using System.Collections.Generic;

namespace GitHubRelease.Configuration
{
    /// <summary>
    /// Configuration options of a specific issue label.
    /// </summary>
    public class LabelConfiguration
    {
        internal static readonly IEqualityComparer<LabelConfiguration> EqualityComparer = new Comparer();

        /// <summary>
        /// Gets or sets the name of the label defined in GitHub.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of an issue label in the release notes.
        /// </summary>
        /// <remarks>
        /// If set to <see langword="null"/>, the name of the label defined in
        /// GitHub is used as is.
        /// </remarks>
        public string? DisplayName { get; set; }

        private class Comparer : IEqualityComparer<LabelConfiguration>
        {
            public bool Equals(LabelConfiguration? labelConfig, LabelConfiguration? other) =>
                (labelConfig is null && other is null) || (
                    labelConfig != null &&
                    other != null &&
                    labelConfig.Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase)
                );

            public int GetHashCode(LabelConfiguration labelConfig) =>
                labelConfig.Name.ToLowerInvariant().GetHashCode();
        }
    }
}
