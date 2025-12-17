using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DockerCompose;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NUnit;
using Nuke.Common.Tools.ReportGenerator;
using Serilog;
using Serilog.Events;
using static Nuke.Common.Tools.DockerCompose.DockerComposeTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;

namespace build;

[GitHubActions(nameof(Compile), GitHubActionsImage.UbuntuLatest, On = new[] { GitHubActionsTrigger.WorkflowDispatch }, InvokedTargets = new[] { nameof(Compile) })]
[GitHubActions(nameof(TestResults), GitHubActionsImage.UbuntuLatest, On = new[] { GitHubActionsTrigger.WorkflowDispatch }, InvokedTargets = new[] { nameof(TestResults) })]
[GitHubActions(nameof(TestReport), GitHubActionsImage.UbuntuLatest, On = new[] { GitHubActionsTrigger.WorkflowDispatch }, InvokedTargets = new[] { nameof(TestReport) })]
public class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Solution]
    private readonly Solution Solution;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    private static AbsolutePath DockerDirectory => $"{RootDirectory}/docker";
    private static AbsolutePath SourceDirectory => $"{RootDirectory}/src";
    private static AbsolutePath TestsDirectory => $"{RootDirectory}/tests";
    private static AbsolutePath TestResultsDirectory => $"{RootDirectory}/TestResults";
    private static AbsolutePath TestReportDirectory => $"{TestResultsDirectory}/TestReport";

    private Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            Log.Write(LogEventLevel.Information, "Running {Clean} pipeline...", nameof(Clean));

            var absolutePathsToClean = new AbsolutePath[]
            {
                DockerDirectory,
                SourceDirectory,
                TestsDirectory,
            };

            var absolutePathsToRemove = new AbsolutePath[]
            {
                TestReportDirectory,
                TestResultsDirectory,
            };

            foreach (var absolutePath in absolutePathsToClean)
            {
                Log.Write(LogEventLevel.Information, "Cleaning directory: {AbsolutePathName}", absolutePath.Name);

                absolutePath.GlobDirectories("**/{bin,obj}").DeleteDirectories();
            }

            foreach (var absolutePath in absolutePathsToRemove)
            {
                Log.Write(LogEventLevel.Information, "Removing directory: {AbsolutePathName}", absolutePath.Name);

                absolutePath.DeleteDirectory();
            }
        });

    private Target Restore => _ => _
        .DependsOn(Clean)
        .Before(Compile)
        .Executes(() =>
        {
            Log.Write(LogEventLevel.Information, "Running {Restore} pipeline...", nameof(Restore));

            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    private Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            Log.Write(LogEventLevel.Information, "Running {Compile} pipeline...", nameof(Compile));

            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    private Target TestResults => _ => _
        .DependsOn(Compile)
        .Produces(TestResultsDirectory)
        .Executes(() =>
        {
            Log.Write(LogEventLevel.Information, "Running {TestResults} pipeline...", nameof(TestResults));

            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetSettingsFile($"--settings:{$"{RootDirectory}/.runsettings"}")
                .EnableNoRestore()
                .EnableNoBuild());
        });

    private Target TestReport => _ => _
        .DependsOn(TestResults)
        .Consumes(TestResults)
        .Produces(TestReportDirectory)
        .Executes(() =>
        {
            Log.Write(LogEventLevel.Information, "Running {TestReport} pipeline...", nameof(TestReport));

            ReportGenerator(s => s
                .AddReports(string.Join(',', TestResultsDirectory.GetFiles("*.xml", depth: 2)))
                .SetTargetDirectory(TestReportDirectory));
        });

    private Target DockerComposeUp => _ => _
        .Executes(() =>
        {
            DockerCompose(s => s
                .SetYmlFile($"{DockerDirectory}/docker-compose.yml")
                .Up());
        });

    private Target DockerComposeDown => _ => _
        .Executes(() =>
        {
            DockerCompose(s => s
                .SetYmlFile($"{DockerDirectory}/docker-compose.yml")
                .Down());
        });
}