using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Helper to load and merge BeepForm style presets from JSON files.
    /// The JSON format is a Dictionary&lt;string, BeepFormStyleMetrics&gt;.
    /// </summary>
    public static class BeepFormStylePresetsLoader
    {
        public static void LoadFromFile(BeepFormStylePresets presets, string filePath)
        {
            if (presets == null) throw new ArgumentNullException(nameof(presets));
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return;
            try
            {
                var json = File.ReadAllText(filePath);
                var dict = JsonSerializer.Deserialize<Dictionary<string, BeepFormStyleMetrics>>(json);
                if (dict == null) return;
                foreach (var kv in dict)
                    presets.Presets[kv.Key] = kv.Value; // merge/overwrite
            }
            catch { /* ignore bad files */ }
        }

        public static void LoadFromFolder(BeepFormStylePresets presets, string folderPath, string searchPattern = "*.json")
        {
            if (presets == null) throw new ArgumentNullException(nameof(presets));
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath)) return;
            foreach (var file in Directory.GetFiles(folderPath, searchPattern))
                LoadFromFile(presets, file);
        }
    }
}
