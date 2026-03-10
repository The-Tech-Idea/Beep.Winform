using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TheTechIdea.Beep.Icons
{
    public sealed class IconCatalogEntry
    {
        public string Key { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Path { get; init; } = string.Empty;
        public string Source { get; init; } = string.Empty;
        public string Category { get; init; } = "Other";
    }

    /// <summary>
    /// Shared icon metadata catalog used by design-time picker and converters.
    /// </summary>
    public static class IconCatalog
    {
        private static readonly Lazy<IReadOnlyList<IconCatalogEntry>> _entries = new(LoadEntries);
        private static readonly StringComparer _cmp = StringComparer.OrdinalIgnoreCase;

        public static IReadOnlyList<IconCatalogEntry> GetAllEntries() => _entries.Value;

        public static IEnumerable<IconCatalogEntry> GetBySource(string source) =>
            GetAllEntries().Where(e => _cmp.Equals(e.Source, source));

        public static IEnumerable<IconCatalogEntry> GetByCategory(string category) =>
            GetAllEntries().Where(e => _cmp.Equals(e.Category, category));

        public static string? GetPathByKey(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            return GetAllEntries().FirstOrDefault(e => _cmp.Equals(e.Key, key))?.Path;
        }

        public static string? GetKeyByPath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            return GetAllEntries().FirstOrDefault(e => _cmp.Equals(e.Path, path))?.Key;
        }

        private static IReadOnlyList<IconCatalogEntry> LoadEntries()
        {
            var list = new List<IconCatalogEntry>(1024);
            AddFromType(list, typeof(SvgsUI), "UI");
            AddFromType(list, typeof(Svgs), "General");
            AddFromType(list, typeof(SvgsDatasources), "DataSources");

            return list
                .GroupBy(e => e.Path, _cmp)
                .Select(g => g.First())
                .OrderBy(e => e.Source)
                .ThenBy(e => e.Category)
                .ThenBy(e => e.Name)
                .ToArray();
        }

        private static void AddFromType(List<IconCatalogEntry> list, Type type, string source)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                if (field.FieldType != typeof(string))
                {
                    continue;
                }

                var path = field.GetValue(null) as string;
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                var name = FormatName(field.Name);
                list.Add(new IconCatalogEntry
                {
                    Key = $"{source}.{field.Name}",
                    Name = name,
                    Path = path,
                    Source = source,
                    Category = InferCategory(field.Name)
                });
            }
        }

        private static string FormatName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return fieldName;
            }

            return System.Text.RegularExpressions.Regex.Replace(fieldName, "(\\B[A-Z])", " $1");
        }

        public static string InferCategory(string name)
        {
            var lower = (name ?? string.Empty).ToLowerInvariant();
            if (lower.Contains("alert") || lower.Contains("bell") || lower.Contains("warning") || lower.Contains("info")) return "Alerts";
            if (lower.Contains("arrow") || lower.Contains("chevron") || lower.Contains("navigation")) return "Arrows";
            if (lower.Contains("check") || lower.Contains("plus") || lower.Contains("minus") || lower.Contains("x")) return "Actions";
            if (lower.Contains("user") || lower.Contains("users") || lower.Contains("person")) return "People";
            if (lower.Contains("file") || lower.Contains("folder") || lower.Contains("document")) return "Files";
            if (lower.Contains("calendar") || lower.Contains("clock") || lower.Contains("time")) return "Time";
            if (lower.Contains("mail") || lower.Contains("message") || lower.Contains("chat")) return "Communication";
            if (lower.Contains("settings") || lower.Contains("tool") || lower.Contains("wrench")) return "Settings";
            if (lower.Contains("home") || lower.Contains("building") || lower.Contains("map")) return "Places";
            if (lower.Contains("heart") || lower.Contains("star") || lower.Contains("bookmark")) return "Favorites";
            if (lower.Contains("database") || lower.Contains("server") || lower.Contains("cloud")) return "Data";
            if (lower.Contains("edit") || lower.Contains("pen") || lower.Contains("pencil")) return "Editing";
            if (lower.Contains("trash") || lower.Contains("delete") || lower.Contains("remove")) return "Delete";
            if (lower.Contains("copy") || lower.Contains("clipboard") || lower.Contains("paste")) return "Clipboard";
            if (lower.Contains("lock") || lower.Contains("key") || lower.Contains("shield")) return "Security";
            return "Other";
        }
    }
}
