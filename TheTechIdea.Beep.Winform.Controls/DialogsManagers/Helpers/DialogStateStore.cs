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

            try
            {
                var json = File.ReadAllText(_storePath);
                var states = JsonSerializer.Deserialize<Dictionary<string, DialogStateRecord>>(json);
                return states ?? new Dictionary<string, DialogStateRecord>(StringComparer.OrdinalIgnoreCase);
            }
            catch
            {
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
            catch
            {
                // Silently fail — persistence is non-critical
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
