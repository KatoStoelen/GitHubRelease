using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GitHubRelease.Configuration
{
    /// <summary>
    /// Configuration options of release notes generation.
    /// </summary>
    public class ReleaseNotesConfiguration
    {
        /// <summary>
        /// Gets or sets the configuration of issue labels.
        /// </summary>
        public LabelsConfiguration Labels { get; set; } = new LabelsConfiguration();

        /// <summary>
        /// Whether or not to tag and link to the profile of the contributor
        /// that closed an issue when it is the repository owner.
        /// <para>
        /// Defaults to <see langword="true"/>, because it's cooler to opt-out
        /// than to opt-in for this setting.
        /// </para>
        /// </summary>
        [YamlMember(Alias = "creditRepoOwner")]
        [JsonProperty("creditRepoOwner")]
        public bool ShouldGiveCreditToRepositoryOwner { get; set; } = true;

        internal void EnsureValid()
        {
            Labels.EnsureValid();
        }

        private const string DefaultFileName = "GitHubRelease";

        private static readonly string[] s_supportedExtensions = new[]
        {
            ".json", ".jsonc",
            ".yml", ".yaml"
        };

        /// <summary>
        /// Loads the release notes configuration file (GitHubRelease.json|jsonc|yml|yaml)
        /// in the specified directory, if found.
        /// </summary>
        /// <param name="directory">
        /// The directory within which to look for a configuration file.
        /// </param>
        /// <returns>
        /// The loaded release notes configuration, or <see langword="null"/> if
        /// no configuration file is found in specified directory.
        /// </returns>
        public static ReleaseNotesConfiguration? FindInDirectory(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                throw new DirectoryNotFoundException($"Directory '{directory.FullName}' not found");
            }

            foreach (var extension in s_supportedExtensions)
            {
                var configFile = new FileInfo(Path.Combine(directory.FullName, DefaultFileName + extension));

                if (configFile.Exists)
                {
                    return FromFile(configFile);
                }
            }

            return null;
        }

        /// <summary>
        /// Loads the specified configuration file.
        /// <para>
        /// Supports both JSON and YAML. Uses the file extension to determine which
        /// serialization to use.
        /// </para>
        /// <para>
        /// JSON => .json or .jsonc (JSON with Comments)
        /// </para>
        /// <para>
        /// YAML => .yml or .yaml
        /// </para>
        /// </summary>
        /// <param name="configFile">The configuration file to load.</param>
        /// <returns>The loaded release notes configuration.</returns>
        public static ReleaseNotesConfiguration FromFile(FileInfo configFile)
        {
            if (!configFile.Exists)
            {
                throw new FileNotFoundException(
                    "Specified configuration file does not exist.",
                    configFile.FullName);
            }

            using var reader = new StreamReader(configFile.OpenRead(), Encoding.UTF8);

            return configFile.Extension.ToLower() switch
            {
                var ext when ext == ".yml" || ext == ".yaml" => FromYamlFile(reader),
                var ext when ext == ".json" || ext == ".jsonc" => FromJsonFile(reader),

                _ => throw new InvalidOperationException(
                    $"Unsupported file extension '{configFile.Extension}'. Must be '.yml', '.yaml', '.json' or '.jsonc'.")
            };
        }

        private static ReleaseNotesConfiguration FromJsonFile(TextReader reader)
        {
            using var jsonReader = new JsonTextReader(reader);

            return new JsonSerializer()
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            }.Deserialize<ReleaseNotesConfiguration>(jsonReader)!;
        }

        private static ReleaseNotesConfiguration FromYamlFile(TextReader reader) =>
            new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build()
                .Deserialize<ReleaseNotesConfiguration>(reader);
    }
}