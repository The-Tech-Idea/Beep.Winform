using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Tools;

namespace TheTechIdea.Beep.Winform.Default.Views.NuggetsManage
{
    public sealed class NuggetsWizardAssemblyHandlerService
    {
        private const string DefaultNugetName = "nuget.org";
        private const string DefaultNugetUrl = "https://api.nuget.org/v3/index.json";

        private readonly IDMEEditor _editor;
        private readonly NuggetsWizardStateStore _stateStore;

        public NuggetsWizardAssemblyHandlerService(IDMEEditor editor)
        {
            _editor = editor ?? throw new ArgumentNullException(nameof(editor));
            _stateStore = new NuggetsWizardStateStore(editor);
        }

        public NuggetsWizardPersistedState LoadState()
        {
            var state = _stateStore.Load();
            if (state.Sources.Count == 0)
            {
                state.Sources.Add(new NuGetSourceConfig { Name = DefaultNugetName, Url = DefaultNugetUrl, IsEnabled = true });
            }

            return state;
        }

        public void SaveState(NuggetsWizardPersistedState state)
        {
            _stateStore.Save(state ?? new NuggetsWizardPersistedState());
        }

        public List<NuGetSourceConfig> GetAllSources()
        {
            var persisted = LoadState();
            var runtime = _editor.assemblyHandler.GetNuGetSources() ?? new List<NuGetSourceConfig>();

            foreach (var source in persisted.Sources)
            {
                if (runtime.Any(r => string.Equals(r.Name, source.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                runtime.Add(source);
            }

            if (!runtime.Any(s => string.Equals(s.Name, DefaultNugetName, StringComparison.OrdinalIgnoreCase)))
            {
                runtime.Insert(0, new NuGetSourceConfig { Name = DefaultNugetName, Url = DefaultNugetUrl, IsEnabled = true });
            }

            return runtime
                .Where(s => !string.IsNullOrWhiteSpace(s.Url))
                .GroupBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();
        }

        public void AddSource(string name, string url, bool isEnabled = true)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            _editor.assemblyHandler.AddNuGetSource(name.Trim(), url.Trim(), isEnabled);
            var state = LoadState();
            var existing = state.Sources.FirstOrDefault(s =>
                string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
            if (existing == null)
            {
                state.Sources.Add(new NuGetSourceConfig { Name = name.Trim(), Url = url.Trim(), IsEnabled = isEnabled });
            }
            else
            {
                existing.Url = url.Trim();
                existing.IsEnabled = isEnabled;
            }

            SaveState(state);
        }

        public void RemoveSource(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            _editor.assemblyHandler.RemoveNuGetSource(name.Trim());
            var state = LoadState();
            state.Sources.RemoveAll(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
            if (state.Sources.Count == 0)
            {
                state.Sources.Add(new NuGetSourceConfig { Name = DefaultNugetName, Url = DefaultNugetUrl, IsEnabled = true });
            }
            SaveState(state);
        }

        public Task<List<NuGetSearchResult>> SearchAsync(string term, bool includePrerelease, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Task.FromResult(new List<NuGetSearchResult>());
            }

            return _editor.assemblyHandler.SearchNuGetPackagesAsync(term.Trim(), 0, 30, includePrerelease, token);
        }

        public Task<List<string>> GetVersionsAsync(string packageId, bool includePrerelease, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                return Task.FromResult(new List<string>());
            }

            return _editor.assemblyHandler.GetNuGetPackageVersionsAsync(packageId.Trim(), includePrerelease, token);
        }

        public async Task<NuggetInstallResult> LoadFromNuGetAsync(NuggetInstallRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PackageId))
            {
                return new NuggetInstallResult { Success = false, Message = "Package id is required." };
            }

            try
            {
                var sources = request.Sources?.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct(StringComparer.OrdinalIgnoreCase);
                var loaded = await _editor.assemblyHandler.LoadNuggetFromNuGetAsync(
                    request.PackageId,
                    string.IsNullOrWhiteSpace(request.Version) ? null : request.Version,
                    sources,
                    request.UseSingleSharedContext,
                    string.IsNullOrWhiteSpace(request.AppInstallPath) ? null : request.AppInstallPath,
                    request.UseProcessHost).ConfigureAwait(false);

                var loadedCount = loaded?.Count ?? 0;
                var success = loadedCount > 0 || request.LoadAfterInstall == false;

                UpsertInstalledState(new NuggetInstalledState
                {
                    PackageId = request.PackageId,
                    Version = request.Version ?? string.Empty,
                    Source = request.Sources?.FirstOrDefault() ?? string.Empty,
                    InstallPath = request.AppInstallPath ?? string.Empty,
                    IsLoaded = loadedCount > 0,
                    IsEnabledAtStartup = request.LoadAfterInstall,
                    LastUpdatedUtc = DateTime.UtcNow
                });

                return new NuggetInstallResult
                {
                    Success = success,
                    Message = success
                        ? $"Package '{request.PackageId}' loaded ({loadedCount} assemblies)."
                        : $"Package '{request.PackageId}' did not load any assemblies.",
                    LoadedAssembliesCount = loadedCount,
                    CompletedAtUtc = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new NuggetInstallResult
                {
                    Success = false,
                    Message = $"Load from NuGet failed: {ex.Message}",
                    CompletedAtUtc = DateTime.UtcNow
                };
            }
        }

        public List<NuggetInstalledState> GetInstalledStates()
        {
            var state = LoadState();
            return state.InstalledStates
                .OrderByDescending(s => s.LastUpdatedUtc)
                .ToList();
        }

        public bool LoadInstalledNugget(string pathOrPackageId)
        {
            if (string.IsNullOrWhiteSpace(pathOrPackageId))
            {
                return false;
            }

            var loaded = _editor.assemblyHandler.LoadNugget(pathOrPackageId);
            if (loaded)
            {
                MarkLoaded(pathOrPackageId, true);
            }

            return loaded;
        }

        public bool UnloadInstalledNugget(string packageId)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                return false;
            }

            var unloaded = _editor.assemblyHandler.UnloadNugget(packageId);
            if (unloaded)
            {
                MarkLoaded(packageId, false);
            }

            return unloaded;
        }

        public void RemoveInstalledState(string packageId)
        {
            if (string.IsNullOrWhiteSpace(packageId))
            {
                return;
            }

            var state = LoadState();
            state.InstalledStates.RemoveAll(s => string.Equals(s.PackageId, packageId, StringComparison.OrdinalIgnoreCase));
            SaveState(state);
        }

        public void UpsertInstalledState(NuggetInstalledState installedState)
        {
            if (installedState == null || string.IsNullOrWhiteSpace(installedState.PackageId))
            {
                return;
            }

            var state = LoadState();
            var existing = state.InstalledStates.FirstOrDefault(s =>
                string.Equals(s.PackageId, installedState.PackageId, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                state.InstalledStates.Add(installedState);
            }
            else
            {
                existing.Version = installedState.Version;
                existing.Source = installedState.Source;
                existing.InstallPath = installedState.InstallPath;
                existing.IsLoaded = installedState.IsLoaded;
                existing.IsEnabledAtStartup = installedState.IsEnabledAtStartup;
                existing.LastUpdatedUtc = installedState.LastUpdatedUtc;
            }

            SaveState(state);
        }

        public bool IsAssemblyLoaded(string packageOrAssemblyName)
        {
            if (string.IsNullOrWhiteSpace(packageOrAssemblyName))
            {
                return false;
            }

            var candidate = packageOrAssemblyName.Trim();
            return _editor.assemblyHandler.LoadedAssemblies.Any(asm =>
                string.Equals(asm.GetName().Name, candidate, StringComparison.OrdinalIgnoreCase));
        }

        private void MarkLoaded(string packageId, bool loaded)
        {
            var state = LoadState();
            var installed = state.InstalledStates.FirstOrDefault(s =>
                string.Equals(s.PackageId, packageId, StringComparison.OrdinalIgnoreCase));
            if (installed == null)
            {
                installed = new NuggetInstalledState
                {
                    PackageId = packageId,
                    Version = string.Empty,
                    Source = string.Empty,
                    InstallPath = string.Empty
                };
                state.InstalledStates.Add(installed);
            }

            installed.IsLoaded = loaded;
            installed.LastUpdatedUtc = DateTime.UtcNow;
            SaveState(state);
        }
    }
}
