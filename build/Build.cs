using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DockerCompose;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NUnit;
using Nuke.Common.Tools.ReportGenerator;
using static Nuke.Common.Tools.DockerCompose.DockerComposeTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;

namespace build;

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

    private Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DockerDirectory.GlobDirectories("**/{bin,obj}").DeleteDirectories();
            SourceDirectory.GlobDirectories("**/{bin,obj}").DeleteDirectories();
            TestsDirectory.GlobDirectories("**/{bin,obj}").DeleteDirectories();
            TestResultsDirectory.DeleteDirectory();
        });

    private Target Restore => _ => _
        .DependsOn(Clean)
        .Before(Compile)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    private Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    private Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetSettingsFile($"--settings:{$"{RootDirectory}/.runsettings"}")
                .EnableNoRestore()
                .EnableNoBuild());

            ReportGenerator(s => s
                .AddReports(string.Join(',', TestResultsDirectory.GetFiles("*.xml", depth: 2)))
                .SetTargetDirectory($"{TestResultsDirectory}/Report"));
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