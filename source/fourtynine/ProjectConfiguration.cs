﻿namespace fourtynine;

public static class ProjectConfiguration
{
    public const string StaticFilesFolderRelativePath = "wwwroot";
    
    /// <summary>
    /// This has to stay empty.
    /// Otherwise it's a real pain in the butt to configure everything.
    /// Firstly, razor pages wouldn't have the ability to link directly to "public" files
    /// (they'd have to prefix the names with that route).
    /// Secondly, vite will have to be configured to serve all of its files under this prefix,
    /// so one more piece of configuration that would have to be shared / duplicated.
    /// </summary>
    public const string StaticFilesRoute = "/";
}