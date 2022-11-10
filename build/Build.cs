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

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            var project49 = Solution.fourtynine;
            DotNetClean(_ => _.SetProject(project49));
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .DependsOn(GenerateSSLKeysDevelopment)
        .Executes(() =>
        {
            var project49 = Solution.fourtynine; 
            DotNetRestore(_ => _
                .SetProjectFile(project49));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution.fourtynine)
                .SetConfiguration(Configuration));
        });

    Target Publish => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            var outputAppDirectory = OutputDirectory / "app";
            var viteProjectDirectory = Solution.Directory / "source" / "fourtynine.ClientApp";
            var viteResultsDirectory = viteProjectDirectory / "dist";
            EnsureCleanDirectory(outputAppDirectory);
            DotNetPublish(_ => _
                .SetProject(Solution.fourtynine)
                .SetConfiguration(Configuration)
                .SetOutput(outputAppDirectory)
                .SetProcessWorkingDirectory(Solution.Directory));
            Npm("run build", workingDirectory: viteProjectDirectory);
            CopyDirectoryRecursively(viteResultsDirectory, outputAppDirectory / "StaticFiles");
        });

    Target GenerateSSLKeysDevelopment => _ => _
        .Executes(() =>
        {
            AbsolutePath aspnetcoreHttps;
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (string.IsNullOrEmpty(appdata))
            {
                aspnetcoreHttps = (AbsolutePath) Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) 
                    / ".aspnetcore" / "https";
            }
            else
            {
                aspnetcoreHttps = (AbsolutePath) appdata / "ASP.NET" / "https";
            }
            
            var certFilePath = Solution.Directory / "fourtynine.pem";
            var keyFilePath = Solution.Directory / "fourtynine.key";

            if (!File.Exists(certFilePath) || !File.Exists(keyFilePath))
            {
                string sourceCertPath = aspnetcoreHttps / "fourtynine.pem";
                string sourceKeyFilePath = aspnetcoreHttps / "fourtynine.key";
                if (File.Exists(sourceCertPath) && File.Exists(sourceKeyFilePath))
                {
                    File.Copy(sourceCertPath, certFilePath!);
                    File.Copy(sourceKeyFilePath, keyFilePath!);
                }
                else
                {
                    DotNetDevCertsHttpsCreate(new()
                    {
                        ExportPath = certFilePath,
                        NoPassword = true,
                    });
                }
            }

            DotNet("dev-certs https --trust");
        });
}