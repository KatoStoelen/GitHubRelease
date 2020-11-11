#tool "nuget:?package=GitVersion.CommandLine&version=5.3.7"
#addin "nuget:?package=Cake.Git&version=0.22.0"
#addin "nuget:?package=KatoStoelen.GitHubRelease.Cake&version=1.0.0-beta.2&prerelease"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var _target = Argument("target", "Pack");
var _configuration = Argument("configuration", "Debug");
var _buildNumber = Argument("build-number", 1);
var _nugetFeed = Argument("nuget-feed", (string)null);
var _nugetUserName = Argument("nuget-user-name", (string)null);
var _nugetApiKey = Argument("nuget-api-key", (string)null);
var _pullRequestNumber = Argument("pr-number", (string)null);
var _gitHubPat = Argument("github-pat", (string)null);

///////////////////////////////////////////////////////////////////////////////
// VARIABLES
///////////////////////////////////////////////////////////////////////////////

var _rootDir = Directory("..");
var _srcDir = _rootDir + Directory("src");
var _testsDir = _rootDir + Directory("tests");
var _artifactsDir = _rootDir + Directory("artifacts");
var _solutionFile = GetFiles($"{_rootDir}/*.sln").SingleOrDefault() ??
                    throw new InvalidOperationException("Did not find the solution file");
var _nugetConfigFile = _rootDir + File("nuget.config");
var _releaseNotesFile = _artifactsDir + File("ReleaseNotes.md");

var _defaultMSBuildSettings = new DotNetCoreMSBuildSettings
{
    NoLogo = true,
    Verbosity = DotNetCoreVerbosity.Minimal
};

var _isAzurePipelinesBuild =
    BuildSystem.IsRunningOnAzurePipelines ||
    BuildSystem.IsRunningOnAzurePipelinesHosted;

var _gitVersionInfo = GitVersion();

var _currentBranch = _gitVersionInfo.BranchName;

var _isCI = !BuildSystem.IsLocalBuild;
var _isMasterBuild = _currentBranch
    .Equals("master", StringComparison.OrdinalIgnoreCase);

var _version = GetLibraryVersion(_gitVersionInfo, _pullRequestNumber, _buildNumber, _isMasterBuild);
var _buildName = GetBuildName(_version, _buildNumber, _isMasterBuild);

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(_ =>
{
    Info($"Branch: {_currentBranch}", appendEmptyLine: false);
    Info($"Version: {_version}", appendEmptyLine: false);
    Info($"Configuration: {_configuration}", appendEmptyLine: false);
    Info($"Is CI? {_isCI}", appendEmptyLine: false);
    Info($"Is master build? {_isMasterBuild}", appendEmptyLine: false);

    if (_isAzurePipelinesBuild)
    {
        AzurePipelines.Commands.UpdateBuildNumber(_buildName);
    }
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    Info($"Cleaning {_solutionFile}");

    DotNetCoreClean(_solutionFile.FullPath, new DotNetCoreCleanSettings
    {
        Configuration = _configuration,
        MSBuildSettings = _defaultMSBuildSettings
    });
});

Task("Restore")
    .Does(() =>
{
    Info($"Restoring {_solutionFile}");

    DotNetCoreRestore(_solutionFile.FullPath, new DotNetCoreRestoreSettings
    {
        MSBuildSettings = _defaultMSBuildSettings
    });
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    Info($"Building {_solutionFile}");

    DotNetCoreBuild(_solutionFile.FullPath, new DotNetCoreBuildSettings
    {
        Configuration = _configuration,
        NoRestore = true,
        MSBuildSettings = _defaultMSBuildSettings
            .SetVersion(_version)
            .WithProperty("ContinuousIntegrationBuild", _isCI.ToString().ToLower())
            .WithProperty("Copyright", $"Copyright © {DateTime.Now.Year} Kato Stoelen")
    });
});

Task("Publish-CakeExtension")
    .Does(() =>
{
    var cakeExtensionProject =
        _srcDir +
        Directory("GitHubRelease.Cake") +
        File("GitHubRelease.Cake.csproj");

    Info($"Publishing {cakeExtensionProject}");

    DotNetCorePublish(cakeExtensionProject, new DotNetCorePublishSettings
    {
        Configuration = _configuration,
        NoRestore = true,
        NoBuild = true,
        MSBuildSettings = _defaultMSBuildSettings
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    Info($"Running tests in solution {_solutionFile}");

    DotNetCoreTest(_solutionFile.FullPath, new DotNetCoreTestSettings
    {
        Configuration = _configuration,
        NoRestore = true,
        NoBuild = true,
        ArgumentCustomization = args => args.Append("--nologo"),
        Logger = _isAzurePipelinesBuild ? "trx" : null
    });

    if (_isAzurePipelinesBuild)
    {
        var testResultFiles = GetFiles($"{_testsDir}/**/*.trx").ToList();

        if (!testResultFiles.Any())
        {
            return;
        }

        AzurePipelines.Commands.PublishTestResults(new AzurePipelinesPublishTestResultsData
        {
            TestRunTitle = _buildName,
            TestRunner = AzurePipelinesTestRunnerType.VSTest,
            Configuration = _configuration,
            TestResultsFiles = testResultFiles
        });
    }
});

Task("Clean-Artifacts")
    .Does(() =>
{
    if (DirectoryExists(_artifactsDir))
    {
        Info($"Cleaning artifacts directory {_artifactsDir}");
        CleanDirectory(_artifactsDir);
    }
    else
    {
        Info($"Creating artifacts directory {_artifactsDir}");
        CreateDirectory(_artifactsDir);
    }
});

Task("Pack")
    .IsDependentOn("Build")
    .IsDependentOn("Publish-CakeExtension")
    .IsDependentOn("Clean-Artifacts")
    .Does(() =>
{
    var releaseNotes = "(none)";

    if (!string.IsNullOrWhiteSpace(_gitHubPat))
    {
        Info("Creating package release notes");

        releaseNotes = GetReleaseNotes(new ReleaseNotesSettings
        {
            RepositoryRootDirectory = _rootDir,
            GitHubToken = _gitHubPat,
            Format = ReleaseNotesFormat.PlainText
        });
    }

    Info($"Packing project(s) in solution {_solutionFile}");

    DotNetCorePack(_solutionFile.FullPath, new DotNetCorePackSettings
    {
        Configuration = _configuration,
        OutputDirectory = _artifactsDir,
        NoRestore = true,
        NoBuild = true,
        MSBuildSettings = _defaultMSBuildSettings
            .WithProperty("PackageReleaseNotes", releaseNotes)
    });
});

Task("Push")
    .IsDependentOn("Pack")
    .WithCriteria(!string.IsNullOrEmpty(_nugetFeed), "NuGet feed not specified")
    .Does(() =>
{
    var sourceName = new Uri(_nugetFeed).Host.Replace("www.", string.Empty);

    Info($"Pushing package(s) in {_artifactsDir} to {sourceName}");

    var tempSource = !string.IsNullOrEmpty(_nugetUserName)
        ? new TemporaryNuGetSource(
            sourceName,
            _nugetFeed,
            _nugetUserName,
            _nugetApiKey,
            _nugetConfigFile,
            Context)
        : null;

    try
    {
        NuGetPush(GetFiles($"{_artifactsDir}/*.nupkg"), new NuGetPushSettings
        {
            Source = _nugetFeed,
            ApiKey = _nugetApiKey
        });
    }
    finally
    {
        tempSource?.Dispose();
    }
});

Task("Release-Notes")
    .WithCriteria(!string.IsNullOrWhiteSpace(_gitHubPat), "GitHub token not set")
    .Does(() =>
{
    Info($"Writing release notes to {_releaseNotesFile}");

    CreateReleaseNotes(new OutputReleaseNotesSettings
    {
        RepositoryRootDirectory = _rootDir,
        GitHubToken = _gitHubPat,
        OutputFile = _releaseNotesFile
    });
});

Task("GitHub-Release")
    .IsDependentOn("Release-Notes")
    .WithCriteria(_isMasterBuild, "Not on master branch")
    .WithCriteria(!string.IsNullOrWhiteSpace(_gitHubPat), "GitHub token not set")
    .Does(() =>
{
    Info($"Creating GitHub release v{_version}");

    CreateGitHubRelease(new NewGitHubReleaseSettings
    {
        RepositoryRootDirectory = _rootDir,
        GitHubToken = _gitHubPat,
        Name = $"v{_version}",
        TagName = _version,
        TargetCommitish = GitLogTip(_rootDir).Sha,
        BodyFile = _releaseNotesFile,
        IsDraft = false,
        IsPrerelease = !_isMasterBuild,
        Assets = GetFiles($"{_artifactsDir}/*nupkg").ToList()
    });
});

Task("CI")
    .IsDependentOn("Test")
    .IsDependentOn("Push")
    .IsDependentOn("GitHub-Release");

RunTarget(_target);

///////////////////////////////////////////////////////////////////////////////
// CUSTOM
///////////////////////////////////////////////////////////////////////////////

private void Info(string message, bool appendEmptyLine = true, bool prependEmptyLine = false)
{
    Console.ForegroundColor = ConsoleColor.Cyan;

    if (prependEmptyLine)
    {
        Information(string.Empty);
    }

    Information(message);

    if (appendEmptyLine)
    {
        Information(string.Empty);
    }

    Console.ResetColor();
}

private string GetLibraryVersion(
    GitVersion gitVersionInfo,
    string pullRequestNumber,
    int buildNumber,
    bool isMasterBuild)
{
    if (isMasterBuild)
    {
        return gitVersionInfo.MajorMinorPatch;
    }

    if (!string.IsNullOrEmpty(pullRequestNumber))
    {
        return $"{gitVersionInfo.MajorMinorPatch}-pr{pullRequestNumber}.{buildNumber}";
    }

    return $"{gitVersionInfo.MajorMinorPatch}-{gitVersionInfo.PreReleaseLabel}.{buildNumber}";
}

private string GetBuildName(string version, int buildNumber, bool isMasterBuild) =>
    isMasterBuild
        ? $"{version} (Build #{buildNumber})"
        : version;

internal class TemporaryNuGetSource : IDisposable
{
    private readonly string _sourceName;
    private readonly string _nugetFeed;
    private readonly string _nugetUserName;
    private readonly string _nugetApiKey;
    private readonly FilePath _nugetConfigFile;
    private readonly ICakeContext _context;

    public TemporaryNuGetSource(
        string sourceName,
        string nugetFeed,
        string nugetUserName,
        string nugetApiKey,
        FilePath nugetConfigFile,
        ICakeContext context)
    {
        _sourceName = sourceName + "_tmp";
        _nugetFeed = nugetFeed;
        _nugetUserName = nugetUserName;
        _nugetApiKey = nugetApiKey;
        _nugetConfigFile = nugetConfigFile;
        _context = context;

        AddSource();
    }

    private void AddSource()
    {
        _context.Information($"Adding temporary NuGet source {_sourceName} with basic auth");

        _context.NuGetAddSource(_sourceName, _nugetFeed, new NuGetSourcesSettings
        {
            UserName = _nugetUserName,
            Password = _nugetApiKey,
            ConfigFile = _nugetConfigFile
        });

        _context.Information(string.Empty);
    }

    private void RemoveSource()
    {
        _context.Information(string.Empty);
        _context.Information($"Removing temporary NuGet source {_sourceName}");

        _context.NuGetRemoveSource(_sourceName, _nugetFeed, new NuGetSourcesSettings
        {
            ConfigFile = _nugetConfigFile
        });
    }

    public void Dispose() => RemoveSource();
}