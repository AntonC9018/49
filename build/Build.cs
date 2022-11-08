using System;
using System.Linq;
using Microsoft.Build.Tasks;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    
    [Solution(GenerateProjects = true)]
    readonly Solution Solution;

    [PackageExecutable(
        packageId: "Microsoft.Web.LibraryManager.Cli",
        packageExecutable: "tools/netcoreapp2.1/any/libman.dll")]
    readonly Tool libman;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            var project49 = Solution.fourtynine;
            DotNetClean(_ => _.SetProject(project49));
            libman("clean", project49.Directory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            var project49 = Solution.fourtynine; 
            DotNetRestore(_ => _
                .SetProjectFile(project49));
            libman("restore", project49.Directory);
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution.fourtynine)
                .SetConfiguration(Configuration));
        });
}
