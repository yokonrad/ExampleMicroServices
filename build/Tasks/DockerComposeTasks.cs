using Nuke.Common.Tooling;
using Serilog.Events;
using System.Collections.Generic;

namespace Nuke.Common.Tools.DockerCompose;

#region DockerComposeUpSettings

[Command(Type = typeof(DockerComposeTasks), Command = nameof(DockerComposeTasks.DockerCompose))]
public class DockerComposeUpSettings : ToolOptions
{
    [Argument(Format = "--file {value}", Position = 1)]
    public string YmlFile => Get<string>(() => YmlFile);

    [Argument(Format = "up --detach", Position = 2)]
    public bool Up => Get<bool>(() => Up);
}

#endregion DockerComposeUpSettings

#region DockerComposeDownSettings

[Command(Type = typeof(DockerComposeTasks), Command = nameof(DockerComposeTasks.DockerCompose))]
public class DockerComposeDownSettings : ToolOptions
{
    [Argument(Format = "--file {value}", Position = 1)]
    public string YmlFile => Get<string>(() => YmlFile);

    [Argument(Format = "down", Position = 2)]
    public bool Down => Get<bool>(() => Down);
}

#endregion DockerComposeDownSettings

#region DockerComposeUpSettingsExtensions

public static class DockerComposeUpSettingsExtensions
{
    [Builder(Type = typeof(DockerComposeUpSettings), Property = nameof(DockerComposeUpSettings.YmlFile))]
    public static T SetYmlFile<T>(this T o, string v) where T : DockerComposeUpSettings => o.Modify(b => b.Set(() => o.YmlFile, v));

    [Builder(Type = typeof(DockerComposeUpSettings), Property = nameof(DockerComposeUpSettings.YmlFile))]
    public static T ResetYmlFile<T>(this T o) where T : DockerComposeUpSettings => o.Modify(b => b.Remove(() => o.YmlFile));

    [Builder(Type = typeof(DockerComposeUpSettings), Property = nameof(DockerComposeUpSettings.Up))]
    public static T Up<T>(this T o) where T : DockerComposeUpSettings => o.Modify(b => b.Set(() => o.Up, true));
}

#endregion DockerComposeUpSettingsExtensions

#region DockerComposeDownSettingsExtensions

public static class DockerComposeDownSettingsExtensions
{
    [Builder(Type = typeof(DockerComposeDownSettings), Property = nameof(DockerComposeDownSettings.YmlFile))]
    public static T SetYmlFile<T>(this T o, string v) where T : DockerComposeDownSettings => o.Modify(b => b.Set(() => o.YmlFile, v));

    [Builder(Type = typeof(DockerComposeDownSettings), Property = nameof(DockerComposeDownSettings.YmlFile))]
    public static T ResetYmlFile<T>(this T o) where T : DockerComposeDownSettings => o.Modify(b => b.Remove(() => o.YmlFile));

    [Builder(Type = typeof(DockerComposeDownSettings), Property = nameof(DockerComposeDownSettings.Down))]
    public static T Down<T>(this T o) where T : DockerComposeDownSettings => o.Modify(b => b.Set(() => o.Down, true));
}

#endregion DockerComposeDownSettingsExtensions

[LogErrorAsStandard]
[LogLevelPattern(LogEventLevel.Warning, "failed")]
[PathTool(Executable = PathExecutable)]
public class DockerComposeTasks : ToolTasks, IRequirePathTool
{
    public static string DockerComposePath { get => new DockerComposeTasks().GetToolPathInternal(); set => new DockerComposeTasks().SetToolPath(value); }

    public const string PathExecutable = "docker-compose";

    public static IReadOnlyCollection<Output> DockerCompose(DockerComposeUpSettings options = null) => new DockerComposeTasks().Run(options);

    public static IReadOnlyCollection<Output> DockerCompose(Configure<DockerComposeUpSettings> configurator) => new DockerComposeTasks().Run(configurator.Invoke(new DockerComposeUpSettings()));

    public static IReadOnlyCollection<Output> DockerCompose(DockerComposeDownSettings options = null) => new DockerComposeTasks().Run(options);

    public static IReadOnlyCollection<Output> DockerCompose(Configure<DockerComposeDownSettings> configurator) => new DockerComposeTasks().Run(configurator.Invoke(new DockerComposeDownSettings()));
}