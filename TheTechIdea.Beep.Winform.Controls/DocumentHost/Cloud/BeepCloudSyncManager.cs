// BeepCloudSyncManager.cs
// Orchestrates workspace and template sync between local WorkspaceManager
// and a remote ICloudSyncProvider (LocalFile or Azure Blob Storage).
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Orchestrates sync between local <see cref="WorkspaceManager"/> state and
    /// a remote <see cref="ICloudSyncProvider"/>.
    /// </summary>
    public sealed class BeepCloudSyncManager : IDisposable
    {
        private static readonly JsonSerializerOptions _json = new()
        {
            WriteIndented            = true,
            PropertyNameCaseInsensitive = true
        };

        private readonly ICloudSyncProvider _provider;
        private readonly WorkspaceManager   _workspaces;
        private readonly System.Windows.Forms.Timer? _timer;
        private bool _disposed;

        // ── Events ────────────────────────────────────────────────────────────

        /// <summary>Raised when a workspace is uploaded to the remote store.</summary>
        public event EventHandler<SyncEventArgs>?         Uploaded;

        /// <summary>Raised when a workspace is downloaded from the remote store.</summary>
        public event EventHandler<SyncEventArgs>?         Downloaded;

        /// <summary>
        /// Raised when both local and remote versions differ.
        /// The handler may set <see cref="SyncConflictEventArgs.ResolvedContent"/>
        /// to choose the winner; default is last-write-wins (remote).
        /// </summary>
        public event EventHandler<SyncConflictEventArgs>? ConflictDetected;

        /// <summary>Raised when any sync operation fails.</summary>
        public event EventHandler<SyncEventArgs>?         SyncError;

        // ── Construction ──────────────────────────────────────────────────────

        public BeepCloudSyncManager(
            ICloudSyncProvider provider,
            WorkspaceManager   workspaces,
            BeepCloudSyncSettings? settings = null)
        {
            ArgumentNullException.ThrowIfNull(provider);
            ArgumentNullException.ThrowIfNull(workspaces);
            _provider   = provider;
            _workspaces = workspaces;

            var s = settings ?? new BeepCloudSyncSettings();

            if (s.SyncOnSave)
                _workspaces.WorkspaceSaved += OnWorkspaceSavedAsync;

            if (s.SyncInterval > TimeSpan.Zero)
            {
                _timer          = new System.Windows.Forms.Timer
                {
                    Interval = (int)Math.Max(5000, s.SyncInterval.TotalMilliseconds)
                };
                _timer.Tick    += OnTimerTickAsync;
                _timer.Enabled = true;
            }
        }

        // ── Factory ───────────────────────────────────────────────────────────

        /// <summary>
        /// Creates the appropriate <see cref="ICloudSyncProvider"/> from
        /// <paramref name="settings"/>.  Returns a <see cref="LocalFileSyncProvider"/>
        /// when <c>ProviderType</c> is <c>"LocalFile"</c> or unrecognised.
        /// </summary>
        public static ICloudSyncProvider CreateProvider(BeepCloudSyncSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            return settings.ProviderType switch
            {
                "AzureBlob"  => new AzureBlobSyncProvider(settings),
                "LocalFile"  => new LocalFileSyncProvider(settings.ConnectionString ?? Path.GetTempPath()),
                _            => new LocalFileSyncProvider(settings.ConnectionString ?? Path.GetTempPath()),
            };
        }

        // ── Sync all workspaces ───────────────────────────────────────────────

        /// <summary>
        /// Pushes all local workspaces to the remote store and pulls any remote
        /// workspaces that are newer or not present locally.
        /// </summary>
        public async Task SyncWorkspacesAsync(CancellationToken ct = default)
        {
            try
            {
                // Upload local workspaces
                foreach (var ws in _workspaces.GetAll())
                {
                    ct.ThrowIfCancellationRequested();
                    await UploadWorkspaceAsync(ws, ct).ConfigureAwait(false);
                }

                // Download remote workspaces not present locally
                var remoteKeys = await _provider.ListAsync("workspaces/", ct).ConfigureAwait(false);
                foreach (var key in remoteKeys)
                {
                    ct.ThrowIfCancellationRequested();
                    await PullWorkspaceAsync(key, ct).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                SyncError?.Invoke(this, new SyncEventArgs("workspaces/", ex.Message));
            }
        }

        // ── Sync single workspace ─────────────────────────────────────────────

        public async Task UploadWorkspaceAsync(WorkspaceDefinition ws, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(ws);
            var key     = $"workspaces/{ws.Id}.json";
            var content = JsonSerializer.Serialize(ws, _json);

            try
            {
                var existing = await _provider.DownloadAsync(key, ct).ConfigureAwait(false);
                if (existing != null && existing != content)
                {
                    content = ResolveConflict(key, content, existing);
                }

                await _provider.UploadAsync(key, content, ct).ConfigureAwait(false);
                Uploaded?.Invoke(this, new SyncEventArgs(key, ws.Name));
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                SyncError?.Invoke(this, new SyncEventArgs(key, ex.Message));
            }
        }

        private async Task PullWorkspaceAsync(string key, CancellationToken ct)
        {
            try
            {
                var content = await _provider.DownloadAsync(key, ct).ConfigureAwait(false);
                if (string.IsNullOrEmpty(content)) return;

                var ws = JsonSerializer.Deserialize<WorkspaceDefinition>(content, _json);
                if (ws == null) return;

                var local = _workspaces.FindById(ws.Id);
                if (local == null)
                {
                    _workspaces.Save(ws.Name, ws.LayoutJson);
                    Downloaded?.Invoke(this, new SyncEventArgs(key, ws.Name));
                }
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                SyncError?.Invoke(this, new SyncEventArgs(key, ex.Message));
            }
        }

        // ── Conflict resolution ───────────────────────────────────────────────

        private string ResolveConflict(string key, string local, string remote)
        {
            if (ConflictDetected == null) return remote; // default: last-write-wins
            var args = new SyncConflictEventArgs(key, local, remote);
            ConflictDetected.Invoke(this, args);
            return args.ResolvedContent;
        }

        // ── Event handlers ────────────────────────────────────────────────────

        private async void OnWorkspaceSavedAsync(object? sender, WorkspaceEventArgs e)
        {
            try
            {
                await UploadWorkspaceAsync(e.Workspace).ConfigureAwait(false);
            }
            catch { /* fire-and-forget; SyncError event raised inside */ }
        }

        private async void OnTimerTickAsync(object? sender, EventArgs e)
        {
            try
            {
                await SyncWorkspacesAsync().ConfigureAwait(false);
            }
            catch { /* background sync; SyncError event raised inside */ }
        }

        // ── IDisposable ───────────────────────────────────────────────────────

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _timer?.Dispose();
            _workspaces.WorkspaceSaved -= OnWorkspaceSavedAsync;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // LocalFileSyncProvider — built-in impl, no extra dependencies
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// <see cref="ICloudSyncProvider"/> implementation backed by the local file system.
    /// Useful for team shares (UNC paths) or testing without a real cloud provider.
    /// </summary>
    public sealed class LocalFileSyncProvider : ICloudSyncProvider
    {
        private readonly string _rootDirectory;

        public LocalFileSyncProvider(string rootDirectory)
        {
            if (string.IsNullOrWhiteSpace(rootDirectory))
                throw new ArgumentException("Root directory must not be empty.", nameof(rootDirectory));
            _rootDirectory = rootDirectory;
        }

        public Task UploadAsync(string key, string content, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var path = KeyToPath(key);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, content, System.Text.Encoding.UTF8);
            return Task.CompletedTask;
        }

        public Task<string?> DownloadAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var path = KeyToPath(key);
            string? result = File.Exists(path)
                ? File.ReadAllText(path, System.Text.Encoding.UTF8)
                : null;
            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<string>> ListAsync(string prefix, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var dir  = KeyToPath(prefix.TrimEnd('/'));
            var keys = new List<string>();
            if (Directory.Exists(dir))
            {
                foreach (var f in Directory.GetFiles(dir, "*.json", SearchOption.AllDirectories))
                    keys.Add(PathToKey(f));
            }
            return Task.FromResult<IReadOnlyList<string>>(keys);
        }

        public Task DeleteAsync(string key, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();
            var path = KeyToPath(key);
            if (File.Exists(path)) File.Delete(path);
            return Task.CompletedTask;
        }

        private string KeyToPath(string key) =>
            Path.Combine(_rootDirectory, key.Replace('/', Path.DirectorySeparatorChar));

        private string PathToKey(string path) =>
            Path.GetRelativePath(_rootDirectory, path).Replace(Path.DirectorySeparatorChar, '/');
    }

    // ─────────────────────────────────────────────────────────────────────────
    // AzureBlobSyncProvider — stub; activate by adding Azure.Storage.Blobs
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Stub <see cref="ICloudSyncProvider"/> for Azure Blob Storage.
    /// <para>
    /// To activate: add the <c>Azure.Storage.Blobs</c> NuGet package and replace
    /// the TODO bodies with calls to <c>BlobContainerClient</c>.
    /// </para>
    /// </summary>
    public sealed class AzureBlobSyncProvider : ICloudSyncProvider
    {
        private readonly BeepCloudSyncSettings _settings;

        public AzureBlobSyncProvider(BeepCloudSyncSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            _settings = settings;
            // TODO: new BlobContainerClient(settings.ConnectionString, settings.ContainerName);
        }

        public Task UploadAsync(string key, string content, CancellationToken ct = default)
        {
            // TODO: var blob = _container.GetBlobClient(key);
            //        await blob.UploadAsync(BinaryData.FromString(content), overwrite: true, ct);
            throw new NotImplementedException(
                "Add Azure.Storage.Blobs NuGet package and implement AzureBlobSyncProvider.");
        }

        public Task<string?> DownloadAsync(string key, CancellationToken ct = default)
        {
            // TODO: var blob = _container.GetBlobClient(key);
            //        if (!await blob.ExistsAsync(ct)) return null;
            //        var response = await blob.DownloadContentAsync(ct);
            //        return response.Value.Content.ToString();
            throw new NotImplementedException(
                "Add Azure.Storage.Blobs NuGet package and implement AzureBlobSyncProvider.");
        }

        public Task<IReadOnlyList<string>> ListAsync(string prefix, CancellationToken ct = default)
        {
            // TODO: var pages = _container.GetBlobsAsync(prefix: prefix, cancellationToken: ct);
            //        var keys = new List<string>();
            //        await foreach (var item in pages) keys.Add(item.Name);
            //        return keys;
            throw new NotImplementedException(
                "Add Azure.Storage.Blobs NuGet package and implement AzureBlobSyncProvider.");
        }

        public Task DeleteAsync(string key, CancellationToken ct = default)
        {
            // TODO: await _container.DeleteBlobIfExistsAsync(key, cancellationToken: ct);
            throw new NotImplementedException(
                "Add Azure.Storage.Blobs NuGet package and implement AzureBlobSyncProvider.");
        }
    }
}
