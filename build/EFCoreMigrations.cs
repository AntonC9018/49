using System;
using Nuke.Common;
using Nuke.Common.Tooling;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

partial class Build
{
    [Parameter("MigrationName")]
    readonly string MigrationName;

    private Arguments GetEntityFrameworkArguments(Action<Arguments> middleConfigurator)
    {
        // https://learn.microsoft.com/en-us/ef/core/cli/dotnet#target-project-and-startup-project
        var targetProject = Solution.fourtynine_DataAccess;
        var startupProject = Solution.fourtynine;
        
        var args = new Arguments()
            .Add("ef");
        
        middleConfigurator(args);
        
        args.Add("--project {value}", targetProject)
            .Add("--startup-project {value}", startupProject);

        return args;
    }

    Target AddMigration => _ => _
        .Requires(() => MigrationName)
        .Executes(() =>
        {
            var args = GetEntityFrameworkArguments(args =>
            {
                args.Add("migrations add")
                    .Add(MigrationName);
            });
            DotNet(args.RenderForExecution());
        });
    
    Target UpdateDatabase => _ => _
        .Executes(() =>
        {
            var args = GetEntityFrameworkArguments(args =>
            {
                args.Add("database update");
            });
            DotNet(args.RenderForExecution());
        });
}