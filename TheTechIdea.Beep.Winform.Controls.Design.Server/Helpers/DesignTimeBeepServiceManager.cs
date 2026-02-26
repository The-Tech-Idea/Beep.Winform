using System;
using System.Collections.Concurrent;
using System.IO;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Services;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers
{
    internal interface IDesignTimeServiceLease : IDisposable
    {
        IBeepService BeepService { get; }
        string ContextKey { get; }
    }

    internal static class DesignTimeBeepServiceManager
    {
        private sealed class ContextEntry
        {
            public required IBeepService BeepService { get; init; }
            public int RefCount { get; set; }
            public DateTime LastAccessUtc { get; set; } = DateTime.UtcNow;
        }

        private sealed class DesignTimeServiceLease : IDesignTimeServiceLease
        {
            private bool _disposed;
            private readonly string _contextKey;

            public IBeepService BeepService { get; }
            public string ContextKey => _contextKey;

            public DesignTimeServiceLease(string contextKey, IBeepService beepService)
            {
                _contextKey = contextKey;
                BeepService = beepService;
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                Release(_contextKey);
            }
        }

        private static readonly object SyncRoot = new();
        private static readonly ConcurrentDictionary<string, ContextEntry> Contexts = new(StringComparer.OrdinalIgnoreCase);

        public static IDesignTimeServiceLease Acquire(BeepDataConnection component)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            var contextKey = BuildContextKey(component);
            return AcquireInternal(contextKey, () => CreateServiceContext(component));
        }

        public static IDesignTimeServiceLease AcquireForDataBlock(string? baseDirectory = null, string appRepoName = "BeepPlatformConnections")
        {
            var resolvedBaseDirectory = string.IsNullOrWhiteSpace(baseDirectory)
                ? AppContext.BaseDirectory
                : Path.GetFullPath(baseDirectory);
            var resolvedAppRepoName = string.IsNullOrWhiteSpace(appRepoName)
                ? "BeepPlatformConnections"
                : appRepoName.Trim();
            var contextKey = $"DataBlock|{resolvedAppRepoName}|{resolvedBaseDirectory}";
            return AcquireInternal(contextKey, () => CreateServiceContext(resolvedBaseDirectory, resolvedAppRepoName));
        }

        private static void Release(string contextKey)
        {
            lock (SyncRoot)
            {
                if (!Contexts.TryGetValue(contextKey, out var existing))
                {
                    return;
                }

                existing.RefCount = Math.Max(0, existing.RefCount - 1);
                existing.LastAccessUtc = DateTime.UtcNow;

                if (existing.RefCount > 0)
                {
                    return;
                }

                Contexts.TryRemove(contextKey, out _);
                if (existing.BeepService is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        private static string BuildContextKey(BeepDataConnection component)
        {
            var scope = component.PersistenceScope.ToString();
            var appRepoName = string.IsNullOrWhiteSpace(component.AppRepoName) ? "BeepPlatformConnections" : component.AppRepoName.Trim();
            var profile = string.IsNullOrWhiteSpace(component.ActiveProfileName) ? "Default" : component.ActiveProfileName.Trim();
            var baseDirectory = ResolveBaseDirectory(component);
            return $"{scope}|{profile}|{appRepoName}|{baseDirectory}";
        }

        private static string ResolveBaseDirectory(BeepDataConnection component)
        {
            var baseDirectory = component.DirectoryPath;
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                baseDirectory = AppContext.BaseDirectory;
            }

            return Path.GetFullPath(baseDirectory);
        }

        private static IDesignTimeServiceLease AcquireInternal(string contextKey, Func<IBeepService> createService)
        {
            lock (SyncRoot)
            {
                if (!Contexts.TryGetValue(contextKey, out var existing))
                {
                    existing = new ContextEntry
                    {
                        BeepService = createService(),
                        RefCount = 0,
                        LastAccessUtc = DateTime.UtcNow
                    };
                    Contexts[contextKey] = existing;
                }

                existing.RefCount++;
                existing.LastAccessUtc = DateTime.UtcNow;
                return new DesignTimeServiceLease(contextKey, existing.BeepService);
            }
        }

        private static IBeepService CreateServiceContext(BeepDataConnection component)
        {
            var appRepoName = string.IsNullOrWhiteSpace(component.AppRepoName) ? "BeepPlatformConnections" : component.AppRepoName.Trim();
            var baseDirectory = ResolveBaseDirectory(component);
            return CreateServiceContext(baseDirectory, appRepoName);
        }

        private static IBeepService CreateServiceContext(string baseDirectory, string appRepoName)
        {
            var service = new BeepService();
            try
            {
                service.Configure(baseDirectory, appRepoName, BeepConfigType.DataConnector, true);
            }
            catch
            {
                // Keep the designer alive even when optional runtime dependencies
                // (for example logging adapters) are unavailable in design host.
                // Repository helpers can still fallback to local defaults.
            }

            return service;
        }
    }
}
