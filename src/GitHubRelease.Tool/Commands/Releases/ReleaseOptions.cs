using System;
using System.ComponentModel;
using System.IO;
using SystemCommandLineExtensions.AutoOptions;

namespace GitHubRelease.Tool.Commands.Releases
{
    internal class ReleaseOptions : GlobalOptions
    {
        [Alias("-t")]
        [Description("The token used for GitHub API authentication")]
        public string GithubToken { get; set; } = string.Empty;

        [Alias("--owner")]
        [Description("The name of the GitHub organization or user owning the target repository")]
        public string? RepositoryOwner { get; set; }

        [Alias("--repo")]
        [Description("The name of the target repository")]
        public string? RepositoryName { get; set; }

        [Alias("-r")]
        [Description(
            "The root directory of the git repository. " +
            "Alternative of --repository-owner and --repository-name. " +
            "Determines target repository owner and name by the remotes of the " +
            "local git repository")]
        public DirectoryInfo? RepositoryDir { get; set; }

        [NotAnOption]
        public Releaser Releaser => RepositoryDir != null
            ? new Releaser(RepositoryDir, GithubToken)
            : new Releaser(RepositoryOwner!, RepositoryName!, GithubToken);

        public virtual void EnsureValid()
        {
            if (string.IsNullOrWhiteSpace(GithubToken))
            {
                throw new ArgumentException("GitHub token must be set");
            }

            if (RepositoryDir != null)
            {
                if (!RepositoryDir.Exists)
                {
                    throw new ArgumentException(
                        $"The repository root directory '{RepositoryDir}' does not exist");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(RepositoryOwner) ||
                    string.IsNullOrWhiteSpace(RepositoryName))
                {
                    throw new ArgumentException(
                        "Both target repository owner and name must be set " +
                        "when repository root directory is not specified");
                }
            }
        }
    }
}
