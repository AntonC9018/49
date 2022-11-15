using Nuke.Common;
using Nuke.Common.Tooling;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
    [Parameter("MigrationOperation")]
    readonly string MigrationOperation;
    
    [Parameter("MigrationName")]
    readonly string MigrationName;

    Target Migrations => _ => _
        .DependsOn(Restore)
        .Requires(
            () => MigrationOperation,
            () => MigrationName)
        .Executes(() =>
        {
            // https://learn.microsoft.com/en-us/ef/core/cli/dotnet#target-project-and-startup-project
            var targetProject = Solution.fourtynine_DataAccess;
            var startupProject = Solution.fourtynine;

            DotNet(new Arguments()
                .Add("ef migrations")
                .Add(MigrationOperation)
                .Add(MigrationName)
                .Add("--project {value}", targetProject)
                .Add("--startup-project {value}", startupProject)
                .RenderForExecution());
        });
}