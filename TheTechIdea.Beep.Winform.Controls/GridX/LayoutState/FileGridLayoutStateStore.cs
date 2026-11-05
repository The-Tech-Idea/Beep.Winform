using System;
using System.IO;
using System.Text.Json;
using TheTechIdea.Beep.Winform.Controls.Diagnostics;

namespace TheTechIdea.Beep.Winform.Controls.GridX.LayoutState
{
    /// <summary>
    /// Default JSON-backed layout store. Writes are atomic (temp file then
    /// replace) so a crash cannot leave a half-written state file behind.
    /// </summary>
    public sealed class FileGridLayoutStateStore : IGridLayoutStateStore
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        private readonly string _baseDirectory;

        public FileGridLayoutStateStore(string? baseDirectory = null)
        {
            _baseDirectory = string.IsNullOrWhiteSpace(baseDirectory)
                ? Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Beep.Winform",
                    "GridLayouts")
                : baseDirectory!;
        }

        public string BaseDirectory => _baseDirectory;

        public GridLayoutState? Load(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;

            string path = GetPath(key);
            if (!File.Exists(path)) return null;

            try
            {
                string json = File.ReadAllText(path);
                var state = JsonSerializer.Deserialize<GridLayoutState>(json, SerializerOptions);
                if (state == null) return null;

                if (state.Version > GridLayoutState.CurrentVersion)
                {
                    BeepLog.Warn(this, "Load layout state",
                        $"Ignored layout state '{key}': version {state.Version} is newer than {GridLayoutState.CurrentVersion}.");
                    return null;
                }

                return state;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
            {
                BeepLog.Failure(this, $"load layout state '{key}'", ex);
                return null;
            }
        }

        public bool Save(string key, GridLayoutState state)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                BeepLog.Warn(this, "Save layout state", "A layout state key is required.");
                return false;
            }
            if (state == null)
            {
                BeepLog.Warn(this, "Save layout state", "Layout state is null.");
                return false;
            }

            Directory.CreateDirectory(_baseDirectory);
            string path = GetPath(key);
            string tempPath = path + "." + Guid.NewGuid().ToString("N") + ".tmp";

            try
            {
                File.WriteAllText(tempPath, JsonSerializer.Serialize(state, SerializerOptions));
                File.Move(tempPath, path, overwrite: true);
                return true;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
            {
                TryDelete(tempPath);
                BeepLog.Failure(this, $"save layout state '{key}'", ex);
                return false;
            }
        }

        public void Delete(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            string path = GetPath(key);
            try
            {
                if (File.Exists(path)) File.Delete(path);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                BeepLog.Failure(this, $"delete layout state '{key}'", ex);
            }
        }

        private string GetPath(string key)
            => Path.Combine(_baseDirectory, SanitizeKey(key) + ".json");

        private static string SanitizeKey(string key)
        {
            var chars = key.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (Path.GetInvalidFileNameChars().AsSpan().IndexOf(chars[i]) >= 0)
                    chars[i] = '_';
            }
            return new string(chars);
        }

        private static void TryDelete(string path)
        {
            try
            {
                if (File.Exists(path)) File.Delete(path);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                BeepLog.Fallback(path, "remove temporary layout state file", ex);
            }
        }
    }
}
