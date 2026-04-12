// ICloudSyncProvider.cs
// Abstraction for cloud/remote storage used by BeepCloudSyncManager.
// Concrete implementations: LocalFileSyncProvider (built-in, no extra deps)
// and AzureBlobSyncProvider (requires Azure.Storage.Blobs NuGet package).
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Abstraction over a key/value remote store used by <see cref="BeepCloudSyncManager"/>.
    /// Keys are slash-separated paths; values are UTF-8 text blobs.
    /// </summary>
    public interface ICloudSyncProvider
    {
        /// <summary>Uploads (creates or replaces) a text entry at <paramref name="key"/>.</summary>
        Task UploadAsync(string key, string content, CancellationToken ct = default);

        /// <summary>
        /// Downloads the text entry at <paramref name="key"/>.
        /// Returns <c>null</c> if the key does not exist.
        /// </summary>
        Task<string?> DownloadAsync(string key, CancellationToken ct = default);

        /// <summary>Returns all keys that start with <paramref name="prefix"/>.</summary>
        Task<IReadOnlyList<string>> ListAsync(string prefix, CancellationToken ct = default);

        /// <summary>
        /// Deletes the entry at <paramref name="key"/>.
        /// Does not throw if the key does not exist.
        /// </summary>
        Task DeleteAsync(string key, CancellationToken ct = default);
    }

    // ── Configuration ─────────────────────────────────────────────────────────

    /// <summary>
    /// Configuration for <see cref="BeepCloudSyncManager"/>.
    /// </summary>
    public sealed class BeepCloudSyncSettings
    {
        /// <summary>
        /// Provider type hint: <c>"None"</c>, <c>"LocalFile"</c>, <c>"AzureBlob"</c>.
        /// Used by <see cref="BeepCloudSyncManager.CreateProvider"/> factory.
        /// </summary>
        public string ProviderType { get; set; } = "LocalFile";

        /// <summary>
        /// Connection string passed to the provider.
        /// For <c>LocalFile</c>: the root directory path.
        /// For <c>AzureBlob</c>: the Azure Storage connection string.
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>Container / bucket name (used by AzureBlob provider).</summary>
        public string ContainerName { get; set; } = "beep-workspaces";

        /// <summary>Automatically sync after every workspace save when <c>true</c>.</summary>
        public bool SyncOnSave { get; set; } = false;

        /// <summary>Sync before restoring a workspace when <c>true</c>.</summary>
        public bool SyncOnLoad { get; set; } = false;

        /// <summary>
        /// Background sync interval.  Set to <see cref="TimeSpan.Zero"/> to disable
        /// background sync (manual / event-driven only).
        /// </summary>
        public TimeSpan SyncInterval { get; set; } = TimeSpan.Zero;
    }

    // ── Sync event args ───────────────────────────────────────────────────────

    public sealed class SyncEventArgs : EventArgs
    {
        public string Key { get; }
        public string? Message { get; }
        public SyncEventArgs(string key, string? message = null) { Key = key; Message = message; }
    }

    public sealed class SyncConflictEventArgs : EventArgs
    {
        public string Key          { get; }
        public string LocalContent { get; }
        public string RemoteContent { get; }
        /// <summary>Set to the content that should be kept. Defaults to <see cref="RemoteContent"/> (last-write-wins).</summary>
        public string ResolvedContent { get; set; }

        public SyncConflictEventArgs(string key, string local, string remote)
        {
            Key             = key;
            LocalContent    = local;
            RemoteContent   = remote;
            ResolvedContent = remote; // default: last-write-wins
        }
    }
}
