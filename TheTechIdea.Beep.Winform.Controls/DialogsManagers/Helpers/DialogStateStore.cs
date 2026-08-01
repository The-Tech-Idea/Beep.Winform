using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers
{
    internal static class DialogStateStore
    {
        private static readonly string _storePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Beep", "DialogStates.json");

        private static readonly object _lock = new();

        /// <summary>
        /// Raised when dialog state cannot be read or written. Subscribe to route it to a log; the
        /// feature degrades either way, but the failure is observable.
        /// </summary>
        public static event EventHandler<DialogStateStoreErrorEventArgs>? Error;

        private static void Report(string context, Exception ex)
        {
            System.Diagnostics.Trace.TraceWarning($"[DialogStateStore] {context}: {ex.GetType().Name}: {ex.Message}");
            Error?.Invoke(null, new DialogStateStoreErrorEventArgs(context, ex));
        }

        public static void Save(string key, Rectangle bounds)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            lock (_lock)
            {
                var states = LoadStates();
                states[key] = new DialogStateRecord
                {
                    X = bounds.X,
                    Y = bounds.Y,
                    Width = bounds.Width,
                    Height = bounds.Height,
                    LastUsed = DateTime.UtcNow
                };
                SaveStates(states);
            }
        }

        public static Rectangle? Load(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;

            lock (_lock)
            {
                var states = LoadStates();
                if (states.TryGetValue(key, out var record))
                {
                    return new Rectangle(record.X, record.Y, record.Width, record.Height);
                }
            }
            return null;
        }

        public static void Clear(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            lock (_lock)
            {
                var states = LoadStates();
                if (states.Remove(key))
                {
                    SaveStates(states);
                }
            }
        }

        private static Dictionary<string, DialogStateRecord> LoadStates()
        {
            if (!File.Exists(_storePath))
                return new Dictionary<string, DialogStateRecord>(StringComparer.OrdinalIgnoreCase);

            // Narrow, and reported. The previous bare catch made three different situations
            // indistinguishable — a corrupt file, a permissions failure, and a schema change from a
            // future version — and silently discarded every remembered dialog position for all
            // three. "No saved state yet" is normal; "state exists but could not be read" is a
            // fault, and returning an empty dictionary for both is why it was invisible.
            try
            {
                var json = File.ReadAllText(_storePath);
                var states = JsonSerializer.Deserialize<Dictionary<string, DialogStateRecord>>(json);
                return states ?? new Dictionary<string, DialogStateRecord>(StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex) when (ex is IOException
                                       or UnauthorizedAccessException
                                       or JsonException)
            {
                Report($"could not read dialog state from '{_storePath}'", ex);
                return new Dictionary<string, DialogStateRecord>(StringComparer.OrdinalIgnoreCase);
            }
        }

        private static void SaveStates(Dictionary<string, DialogStateRecord> states)
        {
            try
            {
                var dir = Path.GetDirectoryName(_storePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(states, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                File.WriteAllText(_storePath, json);
            }
            catch (Exception ex) when (ex is IOException
                                       or UnauthorizedAccessException
                                       or NotSupportedException)
            {
                // Persistence may degrade silently *to the user*; it must not degrade silently to
                // the developer. If the directory is read-only, dialog positions never persist for
                // the life of the installation, and the previous comment — "Silently fail —
                // persistence is non-critical" — guaranteed nobody would ever find out why.
                Report($"could not write dialog state to '{_storePath}'", ex);
            }
        }

        private class DialogStateRecord
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public DateTime LastUsed { get; set; }
        }
    }
}
