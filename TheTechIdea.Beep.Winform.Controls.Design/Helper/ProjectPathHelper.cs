using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TheTechIdea.Beep.Editor;
using VSLangProj;


namespace TheTechIdea.Beep.Winform.Controls.Design.Helper;
public static class ProjectPathHelper
{
    /// <summary>
    /// Retrieves the referencing project path during design time or runtime.
    /// </summary>
    public static string GetProjectPath(IServiceProvider serviceProvider = null)
    {
        // Attempt to get the project path during design time using IServiceProvider
        if (serviceProvider != null)
        {
            try
            {
                return GetReferencingProjectPath(serviceProvider);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Design-time project path retrieval failed: {ex.Message}");
            }
        }

        // Fallback to runtime methods
        try
        {
            return GetCallingProjectPath();
        }
        catch
        {
            try
            {
                return GetProjectPathFromCsproj();
            }
            catch
            {
                throw new Exception("Unable to determine the referencing project's path.");
            }
        }
    }

    /// <summary>
    /// Retrieves the calling project's path using the stack trace.
    /// </summary>
    public static string GetCallingProjectPath()
    {
        var stackTrace = new StackTrace();
        foreach (var frame in stackTrace.GetFrames())
        {
            var method = frame.GetMethod();
            var assembly = method.DeclaringType?.Assembly;

            if (assembly != null && !assembly.FullName.Contains("Design") && !assembly.FullName.Contains("System"))
            {
                return Path.GetDirectoryName(assembly.Location);
            }
        }

        throw new Exception("Unable to determine the calling project path.");
    }

    /// <summary>
    /// Retrieves the project path by locating the nearest .csproj file.
    /// </summary>
    public static string GetProjectPathFromCsproj()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        DirectoryInfo directoryInfo = new DirectoryInfo(currentDirectory);

        while (directoryInfo != null)
        {
            var csprojFiles = directoryInfo.GetFiles("*.csproj");
            if (csprojFiles.Length > 0)
            {
                return directoryInfo.FullName;
            }

            directoryInfo = directoryInfo.Parent;
        }

        throw new Exception("No .csproj file found in the directory hierarchy.");
    }
    /// <summary>
    /// Gets the path of the referencing project during design time.
    /// </summary>
    /// <param name="serviceProvider">The service provider to access Visual Studio services.</param>
    /// <returns>The project path of the referencing project.</returns>
    public static string GetReferencingProjectPath(IServiceProvider serviceProvider)
    {
        if (serviceProvider == null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        // Get the DTE (Development Tools Environment) from the service provider
        var dte = serviceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

        if (dte == null)
        {
            throw new InvalidOperationException("Unable to retrieve the Visual Studio DTE.");
        }

        // Get the active project (referencing project)
        var activeProject = GetActiveProject(dte);

        if (activeProject == null)
        {
            throw new InvalidOperationException("No active project found.");
        }

        // Return the full path to the project's directory
        return Path.GetDirectoryName(activeProject.FullName);
    }

    /// <summary>
    /// Retrieves the active project in Visual Studio.
    /// </summary>
    /// <param name="dte">The Visual Studio DTE object.</param>
    /// <returns>The active project.</returns>
    public static EnvDTE.Project GetActiveProject(EnvDTE.DTE dte)
    {
        if (dte.Solution == null || dte.Solution.Projects == null)
        {
            return null;
        }

        foreach (EnvDTE.Project project in dte.Solution.Projects)
        {
            if (project == null || string.IsNullOrEmpty(project.FullName))
            {
                continue;
            }

            return project; // Assume the first valid project is the active one
        }

        return null;
    }
    public static string GetProjectPath(string GetMyPath)
    {

        string projectPath = null;
        string fullPath = Path.GetDirectoryName(Path.GetFullPath(GetMyPath));

        DirectoryInfo directory = new DirectoryInfo(fullPath);
        // Runtime path handling
        projectPath = directory.Parent?.FullName;
       

        return projectPath;
    }
    public static string GetMyPath([CallerFilePath] string from = null)
    {
        return GetProjectPath(from);

    }
    #region "Project Helper"
   
    #endregion  "Project Helper"
}
