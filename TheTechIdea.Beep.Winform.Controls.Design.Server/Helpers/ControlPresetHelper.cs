using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers
{
    /// <summary>
    /// Information about a saved control preset
    /// </summary>
    public class PresetInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ControlType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    /// <summary>
    /// Helper for saving and loading control configurations as presets
    /// Allows sharing common configurations between projects
    /// </summary>
    public static class ControlPresetHelper
    {
        private static readonly string PresetsDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "BeepControls",
            "Presets");

        /// <summary>
        /// Ensure the presets directory exists
        /// </summary>
        private static void EnsurePresetsDirectory()
        {
            if (!Directory.Exists(PresetsDirectory))
            {
                Directory.CreateDirectory(PresetsDirectory);
            }
        }

        /// <summary>
        /// Save the current control configuration as a preset
        /// </summary>
        /// <param name="control">The control to save</param>
        /// <param name="presetName">Name of the preset</param>
        /// <param name="category">Category for organization (e.g., "Forms", "Dashboards", "Cards")</param>
        /// <param name="description">Optional description of the preset</param>
        /// <returns>Path to the saved preset file</returns>
        public static string SavePreset(BaseControl control, string presetName, string category, string description = "")
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            if (string.IsNullOrWhiteSpace(presetName))
                throw new ArgumentException("Preset name cannot be empty", nameof(presetName));
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category cannot be empty", nameof(category));

            EnsurePresetsDirectory();

            var categoryDir = Path.Combine(PresetsDirectory, category);
            if (!Directory.Exists(categoryDir))
            {
                Directory.CreateDirectory(categoryDir);
            }

            // Serialize control properties to dictionary
            var properties = new Dictionary<string, object?>();
            var controlType = control.GetType();
            var props = controlType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && !p.GetIndexParameters().Any());

            foreach (var prop in props)
            {
                try
                {
                    var value = prop.GetValue(control);
                    if (value != null && IsSerializable(value))
                    {
                        properties[prop.Name] = value;
                    }
                }
                catch
                {
                    // Skip properties that can't be read
                }
            }

            // Create preset info
            var presetInfo = new PresetInfo
            {
                Name = presetName,
                Category = category,
                Description = description,
                ControlType = controlType.FullName ?? controlType.Name,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            // Save preset data
            var fileName = $"{presetName.Replace(" ", "_")}.json";
            var filePath = Path.Combine(categoryDir, fileName);
            presetInfo.FilePath = filePath;

            var presetData = new
            {
                Info = presetInfo,
                Properties = properties
            };

            var json = JsonSerializer.Serialize(presetData, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            File.WriteAllText(filePath, json);

            // Update preset index
            UpdatePresetIndex(presetInfo);

            return filePath;
        }

        /// <summary>
        /// Load a preset and apply it to a control
        /// </summary>
        /// <param name="control">The control to apply the preset to</param>
        /// <param name="presetName">Name of the preset to load</param>
        /// <param name="category">Category of the preset</param>
        /// <returns>True if preset was loaded successfully</returns>
        public static bool LoadPreset(BaseControl control, string presetName, string category)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            if (string.IsNullOrWhiteSpace(presetName))
                throw new ArgumentException("Preset name cannot be empty", nameof(presetName));
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category cannot be empty", nameof(category));

            var fileName = $"{presetName.Replace(" ", "_")}.json";
            var filePath = Path.Combine(PresetsDirectory, category, fileName);

            if (!File.Exists(filePath))
                return false;

            try
            {
                var json = File.ReadAllText(filePath);
                var presetData = JsonSerializer.Deserialize<JsonElement>(json);

                if (!presetData.TryGetProperty("Properties", out var propertiesElement))
                    return false;

                var controlType = control.GetType();
                var props = controlType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && !p.GetIndexParameters().Any())
                    .ToDictionary(p => p.Name, p => p);

                foreach (var prop in propertiesElement.EnumerateObject())
                {
                    if (props.TryGetValue(prop.Name, out var propertyInfo))
                    {
                        try
                        {
                            var value = ConvertPropertyValue(prop.Value, propertyInfo.PropertyType);
                            if (value != null)
                            {
                                propertyInfo.SetValue(control, value);
                            }
                        }
                        catch
                        {
                            // Skip properties that can't be set
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get all available presets for a category
        /// </summary>
        /// <param name="category">Category to filter by (null for all)</param>
        /// <returns>List of preset information</returns>
        public static List<PresetInfo> GetPresets(string? category = null)
        {
            EnsurePresetsDirectory();

            var presets = new List<PresetInfo>();
            var searchDir = category != null 
                ? Path.Combine(PresetsDirectory, category) 
                : PresetsDirectory;

            if (!Directory.Exists(searchDir))
                return presets;

            var files = Directory.GetFiles(searchDir, "*.json", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var presetData = JsonSerializer.Deserialize<JsonElement>(json);

                    if (presetData.TryGetProperty("Info", out var infoElement))
                    {
                        var presetInfo = JsonSerializer.Deserialize<PresetInfo>(infoElement.GetRawText());
                        if (presetInfo != null)
                        {
                            presets.Add(presetInfo);
                        }
                    }
                }
                catch
                {
                    // Skip invalid preset files
                }
            }

            return presets.OrderBy(p => p.Category).ThenBy(p => p.Name).ToList();
        }

        /// <summary>
        /// Delete a preset
        /// </summary>
        /// <param name="presetName">Name of the preset</param>
        /// <param name="category">Category of the preset</param>
        /// <returns>True if preset was deleted</returns>
        public static bool DeletePreset(string presetName, string category)
        {
            var fileName = $"{presetName.Replace(" ", "_")}.json";
            var filePath = Path.Combine(PresetsDirectory, category, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                UpdatePresetIndex(); // Rebuild index
                return true;
            }

            return false;
        }

        #region Private Helpers

        private static bool IsSerializable(object value)
        {
            var type = value.GetType();
            return type.IsPrimitive ||
                   type == typeof(string) ||
                   type == typeof(DateTime) ||
                   type.IsEnum ||
                   (type.IsValueType && type.Namespace == "System");
        }

        private static object? ConvertPropertyValue(JsonElement element, Type targetType)
        {
            if (element.ValueKind == JsonValueKind.Null)
                return null;

            if (targetType.IsEnum)
            {
                var enumString = element.GetString();
                if (enumString != null && Enum.TryParse(targetType, enumString, out var enumValue))
                    return enumValue;
            }

            if (targetType == typeof(string))
                return element.GetString();

            if (targetType == typeof(int))
                return element.GetInt32();

            if (targetType == typeof(bool))
                return element.GetBoolean();

            if (targetType == typeof(float))
                return (float)element.GetDouble();

            if (targetType == typeof(double))
                return element.GetDouble();

            if (targetType == typeof(DateTime))
                return element.GetDateTime();

            // Try JSON deserialization for complex types
            try
            {
                return JsonSerializer.Deserialize(element.GetRawText(), targetType);
            }
            catch
            {
                return null;
            }
        }

        private static void UpdatePresetIndex(PresetInfo? newPreset = null)
        {
            var indexFile = Path.Combine(PresetsDirectory, "index.json");
            var allPresets = GetPresets();

            if (newPreset != null && !allPresets.Any(p => p.FilePath == newPreset.FilePath))
            {
                allPresets.Add(newPreset);
            }

            var index = new
            {
                LastUpdated = DateTime.Now,
                Presets = allPresets
            };

            var json = JsonSerializer.Serialize(index, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(indexFile, json);
        }

        #endregion
    }
}
