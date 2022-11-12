using System;
using System.IO;
using System.Linq;
using CliWrap;
using Microsoft.Build.Tasks;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.Npm.NpmTasks;
using static CustomToolWrappers.DotNetDevCertsHttpsSettings;

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
    
    AbsolutePath OutputDirectory => Solution.Directory / "output";
    AbsolutePath ViteDirectory => Solution.Directory / "source" / "fourtynine.ClientApp";
    AbsolutePath ViteOutputDirectory => ViteDirectory / "dist";


    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            var project49 = Solution.fourtynine;
            DotNetClean(_ => _.SetProject(project49));
            DeleteDirectory(OutputDirectory);

            var certificatePaths = CertificatePaths.Create(Solution.Directory);
            DeleteFile(certificatePaths.Certificate);
            DeleteFile(certificatePaths.Key);
            DotNet("dev-certs https --clean");

            DeleteDirectory(ViteOutputDirectory);
            DeleteDirectory(ViteDirectory / "node_modules");
        });

    Target Restore => _ => _
        .DependsOn(GenerateSSLKeysDevelopment)
        .Executes(() =>
        {
            var project49 = Solution.fourtynine; 
            DotNetRestore(_ => _
                .SetProjectFile(project49));
            Npm("install", workingDirectory: ViteDirectory);
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution.fourtynine)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Publish => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            var outputAppDirectory = OutputDirectory / "app";
            EnsureCleanDirectory(outputAppDirectory);

            DotNetPublish(_ => _
                .SetProject(Solution.fourtynine)
                .SetConfiguration(Configuration)
                .SetOutput(outputAppDirectory)
                .SetProcessWorkingDirectory(Solution.Directory)
                .EnableNoRestore()
                .SetProperty("PRODUCTION", "true"));

            Npm("run build", workingDirectory: ViteDirectory);
            
            CopyDirectoryRecursively(ViteOutputDirectory, outputAppDirectory / "StaticFiles");
        });

    record struct CertificatePaths(AbsolutePath Certificate, AbsolutePath Key)
    {
        public static CertificatePaths Create(AbsolutePath directory)
        {
            return new CertificatePaths(directory / "fourtynine.pem", directory / "fourtynine.key");
        }

        public bool Exist => Certificate.FileExists() && Key.FileExists();
    };

    Target GenerateSSLKeysDevelopment => _ => _
        .Executes(() =>
        {
            var paths = CertificatePaths.Create(Solution.Directory);
            
            if (!paths.Exist)
            {
                DotNetDevCertsHttpsCreate(new()
                {
                    ExportPath = paths.Certificate,
                    NoPassword = true,
                    WorkingDirectory = Solution.fourtynine.Directory,
                });
            }

            DotNet("dev-certs https --trust");
        });
}