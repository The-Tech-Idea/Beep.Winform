using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Tools;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    public sealed class NuggetInstallRequest
    {
        public string PackageId { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public List<string> Sources { get; set; } = new();
        public bool UseSingleSharedContext { get; set; } = true;
        public bool LoadAfterInstall { get; set; } = true;
        public bool UseProcessHost { get; set; }
        public string AppInstallPath { get; set; } = string.Empty;
    }

    public sealed class NuggetInstallResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int LoadedAssembliesCount { get; set; }
        public DateTime CompletedAtUtc { get; set; } = DateTime.UtcNow;
    }

    public sealed class NuggetInstalledState
    {
        public string PackageId { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string InstallPath { get; set; } = string.Empty;
        public bool IsLoaded { get; set; }
        public bool IsEnabledAtStartup { get; set; }
        public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
    }

    public sealed class NuggetsWizardPersistedState
    {
        public string LastSourceUrl { get; set; } = string.Empty;
        public string LastSearchTerm { get; set; } = string.Empty;
        public bool IncludePrerelease { get; set; }
        public bool LoadAfterInstall { get; set; } = true;
        public bool UseSingleSharedContext { get; set; } = true;
        public bool UseProcessHost { get; set; }
        public string LastInstallPath { get; set; } = string.Empty;
        public List<NuGetSourceConfig> Sources { get; set; } = new();
        public List<NuggetInstalledState> InstalledStates { get; set; } = new();
    }
}
