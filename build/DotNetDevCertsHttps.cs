using System;
using System.Collections.Generic;
using System.Reflection;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

namespace CustomToolWrappers;

public class DotNetDevCertsHttpsSettings : ToolSettings
{
    public static IReadOnlyCollection<Output> DotNetDevCertsHttpsCreate(DotNetDevCertsHttpsSettings settings)
    {
        using var process = ProcessTasks.StartProcess(settings);
        process.AssertZeroExitCode();
        return process.Output;
    }

    /// <summary>
    ///   Path to the DotNet executable.
    /// </summary>
    public override string ProcessToolPath => base.ProcessToolPath ?? DotNetTasks.DotNetPath;
    public override Action<OutputType, string> ProcessCustomLogger => DotNetTasks.DotNetLogger;
    
    public string ExportPath { get; set; }
    public bool? NoPassword { get; set; }
    public string WorkingDirectory { get; set; }
    public override string ProcessWorkingDirectory => WorkingDirectory;

    protected override Arguments ConfigureProcessArguments(Arguments arguments)
    {
        arguments
            .Add("dev-certs https --format Pem")
            .Add("--export-path {value}", ExportPath)
            .Add("--no-password", NoPassword);
        return base.ConfigureProcessArguments(arguments);
    }
}
