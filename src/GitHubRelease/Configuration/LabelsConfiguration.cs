using System;
using System.Collections.Generic;
using System.Linq;

namespace GitHubRelease.Configuration
{
    /// <summary>
    /// Configuration options of issue labels.
    /// </summary>
    public class LabelsConfiguration
    {
        /// <summary>
        /// Gets or sets the list of issue labels to include in the release
        /// notes.
        /// </summary>
        public ICollection<string> Include { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the configuration of specific issue labels.
        /// </summary>
        public ICollection<LabelConfiguration> Configs { get; set; } = new List<LabelConfiguration>();

        internal bool ShouldIncludeLabel(string label) =>
            !Include.Any() ||
            Include.Any(lbl => lbl.Equals(label, StringComparison.OrdinalIgnoreCase));

        internal void EnsureValid()
        {
            var configsPerLabel = Configs.GroupBy(
                config => config.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var group in configsPerLabel)
            {
                if (group.Count() == 1) continue;

                throw new Exception($"Found multiple label configurations of {group.Key}");
            }
        }
    }
}
