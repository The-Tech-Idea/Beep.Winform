using System.Globalization;
using System.Text.Json;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Tokens
{
    public sealed class RibbonTokenImportResult
    {
        public RibbonTheme Theme { get; init; } = new RibbonTheme();
        public IReadOnlyList<string> Diagnostics { get; init; } = Array.Empty<string>();
    }

    public static class RibbonTokenImporter
    {
        public static RibbonTheme ImportFromFile(string filePath, string mode = "light", RibbonTheme? fallback = null)
        {
            return ImportWithDiagnosticsFromFile(filePath, mode, fallback).Theme;
        }

        public static RibbonTheme ImportFromJson(string json, string mode = "light", RibbonTheme? fallback = null)
        {
            return ImportWithDiagnosticsFromJson(json, mode, fallback).Theme;
        }

        public static RibbonTokenImportResult ImportWithDiagnosticsFromFile(string filePath, string mode = "light", RibbonTheme? fallback = null)
        {
            string json = File.ReadAllText(filePath);
            return ImportWithDiagnosticsFromJson(json, mode, fallback);
        }

        public static RibbonTokenImportResult ImportWithDiagnosticsFromJson(string json, string mode = "light", RibbonTheme? fallback = null)
        {
            var diagnostics = new List<string>();
            var theme = CloneTheme(fallback ?? new RibbonTheme());
            if (string.IsNullOrWhiteSpace(json))
            {
                diagnostics.Add("Token source is empty; fallback theme was used.");
                return new RibbonTokenImportResult
                {
                    Theme = theme,
                    Diagnostics = diagnostics
                };
            }

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                var tokenBag = BuildTokenBag(root, mode, diagnostics);
                ValidateTokenBag(tokenBag, diagnostics);
                ApplyTokens(theme, tokenBag);
            }
            catch (JsonException jsonEx)
            {
                diagnostics.Add($"Invalid token JSON: {jsonEx.Message}");
            }

            return new RibbonTokenImportResult
            {
                Theme = theme,
                Diagnostics = diagnostics
            };
        }

        private static Dictionary<string, JsonElement> BuildTokenBag(JsonElement root, string mode, IList<string> diagnostics)
        {
            var bag = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
            string selectedMode = string.IsNullOrWhiteSpace(mode) ? "light" : mode.Trim();

            if (root.ValueKind == JsonValueKind.Object)
            {
                if (root.TryGetProperty("tokens", out var tokens) && tokens.ValueKind == JsonValueKind.Object)
                {
                    FlattenTokenNode(tokens, string.Empty, bag);
                }
                else
                {
                    FlattenTokenNode(root, string.Empty, bag, skipReservedRootKeys: true);
                }

                if (root.TryGetProperty("modes", out var modes) &&
                    modes.ValueKind == JsonValueKind.Object)
                {
                    ValidateModeInheritanceGraph(modes, diagnostics);
                    MergeModeTokensWithInheritance(modes, selectedMode, bag, diagnostics);
                }
                else if (root.TryGetProperty(selectedMode, out var directMode) && directMode.ValueKind == JsonValueKind.Object)
                {
                    if (directMode.TryGetProperty("tokens", out var directModeTokens) && directModeTokens.ValueKind == JsonValueKind.Object)
                    {
                        FlattenTokenNode(directModeTokens, string.Empty, bag);
                    }
                    else
                    {
                        FlattenTokenNode(directMode, string.Empty, bag);
                    }
                }
                else if (!selectedMode.Equals("light", StringComparison.OrdinalIgnoreCase))
                {
                    diagnostics.Add($"Mode '{selectedMode}' was not found; base token set was applied.");
                }
            }

            return bag;
        }

        private static void ValidateModeInheritanceGraph(JsonElement modesNode, IList<string> diagnostics)
        {
            if (modesNode.ValueKind != JsonValueKind.Object)
            {
                return;
            }

            var modeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var mode in modesNode.EnumerateObject())
            {
                if (mode.Value.ValueKind == JsonValueKind.Object && !string.IsNullOrWhiteSpace(mode.Name))
                {
                    modeNames.Add(mode.Name.Trim());
                }
            }

            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var visiting = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var reportedMissing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var reportedCycles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            void Walk(string modeName)
            {
                if (visited.Contains(modeName))
                {
                    return;
                }

                if (!TryGetModeNode(modesNode, modeName, out var modeNode))
                {
                    return;
                }

                if (!visiting.Add(modeName))
                {
                    if (reportedCycles.Add(modeName))
                    {
                        diagnostics.Add($"Cyclic mode inheritance detected in mode graph at '{modeName}'.");
                    }
                    return;
                }

                foreach (var parent in GetInheritedModes(modeNode))
                {
                    if (!modeNames.Contains(parent))
                    {
                        string missingKey = $"{modeName}->{parent}";
                        if (reportedMissing.Add(missingKey))
                        {
                            diagnostics.Add($"Mode '{modeName}' inherits from missing mode '{parent}'.");
                        }
                        continue;
                    }

                    Walk(parent);
                }

                visiting.Remove(modeName);
                visited.Add(modeName);
            }

            foreach (var modeName in modeNames)
            {
                Walk(modeName);
            }
        }

        private static void MergeModeTokensWithInheritance(
            JsonElement modesNode,
            string selectedMode,
            IDictionary<string, JsonElement> bag,
            IList<string> diagnostics)
        {
            var applying = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            void MergeMode(string modeName)
            {
                if (string.IsNullOrWhiteSpace(modeName))
                {
                    return;
                }

                string normalized = modeName.Trim();
                if (!applying.Add(normalized))
                {
                    diagnostics.Add($"Cyclic mode inheritance detected while resolving mode '{selectedMode}'.");
                    return;
                }

                if (!TryGetModeNode(modesNode, normalized, out var modeNode))
                {
                    diagnostics.Add($"Mode '{normalized}' was referenced but not found.");
                    applying.Remove(normalized);
                    return;
                }

                foreach (var parent in GetInheritedModes(modeNode))
                {
                    MergeMode(parent);
                }

                if (modeNode.TryGetProperty("tokens", out var modeTokenBag) && modeTokenBag.ValueKind == JsonValueKind.Object)
                {
                    FlattenTokenNode(modeTokenBag, string.Empty, bag);
                }
                else
                {
                    FlattenTokenNode(modeNode, string.Empty, bag, skipReservedRootKeys: true);
                }

                applying.Remove(normalized);
            }

            MergeMode(selectedMode);
        }

        private static bool TryGetModeNode(JsonElement modesNode, string modeName, out JsonElement modeNode)
        {
            modeNode = default;
            if (modesNode.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            if (modesNode.TryGetProperty(modeName, out var direct) && direct.ValueKind == JsonValueKind.Object)
            {
                modeNode = direct;
                return true;
            }

            foreach (var prop in modesNode.EnumerateObject())
            {
                if (!prop.NameEquals(modeName) &&
                    !prop.Name.Equals(modeName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (prop.Value.ValueKind == JsonValueKind.Object)
                {
                    modeNode = prop.Value;
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<string> GetInheritedModes(JsonElement modeNode)
        {
            if (modeNode.ValueKind != JsonValueKind.Object)
            {
                yield break;
            }

            static IEnumerable<string> ReadModeNames(JsonElement value)
            {
                if (value.ValueKind == JsonValueKind.String)
                {
                    var single = value.GetString();
                    if (!string.IsNullOrWhiteSpace(single))
                    {
                        yield return single.Trim();
                    }
                    yield break;
                }

                if (value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in value.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                        {
                            var text = item.GetString();
                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                yield return text.Trim();
                            }
                        }
                    }
                }
            }

            if (modeNode.TryGetProperty("inherits", out var inherits))
            {
                foreach (var mode in ReadModeNames(inherits))
                {
                    yield return mode;
                }
            }

            if (modeNode.TryGetProperty("inherit", out var inherit))
            {
                foreach (var mode in ReadModeNames(inherit))
                {
                    yield return mode;
                }
            }

            if (modeNode.TryGetProperty("$extends", out var extends))
            {
                foreach (var mode in ReadModeNames(extends))
                {
                    yield return mode;
                }
            }

            if (modeNode.TryGetProperty("extends", out var plainExtends))
            {
                foreach (var mode in ReadModeNames(plainExtends))
                {
                    yield return mode;
                }
            }

            if (modeNode.TryGetProperty("parent", out var parent))
            {
                foreach (var mode in ReadModeNames(parent))
                {
                    yield return mode;
                }
            }
        }

        private static void FlattenTokenNode(
            JsonElement source,
            string basePath,
            IDictionary<string, JsonElement> bag,
            bool skipReservedRootKeys = false)
        {
            if (source.ValueKind != JsonValueKind.Object)
            {
                if (!string.IsNullOrWhiteSpace(basePath))
                {
                    bag[basePath] = source;
                }
                return;
            }

            if (TryGetLeafTokenValue(source, out var leaf))
            {
                if (!string.IsNullOrWhiteSpace(basePath))
                {
                    bag[basePath] = leaf;
                }
                return;
            }

            foreach (var prop in source.EnumerateObject())
            {
                if (ShouldSkipReservedProperty(prop.Name, skipReservedRootKeys))
                {
                    continue;
                }

                string childPath = string.IsNullOrWhiteSpace(basePath)
                    ? prop.Name
                    : $"{basePath}.{prop.Name}";

                FlattenTokenNode(prop.Value, childPath, bag);
            }
        }

        private static void ApplyTokens(RibbonTheme theme, IReadOnlyDictionary<string, JsonElement> tokens)
        {
            theme.Background = ReadColor(tokens, theme.Background, "background", "surface.background", "ribbon.background");
            theme.TabActiveBack = ReadColor(tokens, theme.TabActiveBack, "tab.active.back", "tabActiveBack", "tabs.active.background");
            theme.TabInactiveBack = ReadColor(tokens, theme.TabInactiveBack, "tab.inactive.back", "tabInactiveBack", "tabs.inactive.background");
            theme.TabBorder = ReadColor(tokens, theme.TabBorder, "tab.border", "tabBorder");
            theme.GroupBack = ReadColor(tokens, theme.GroupBack, "group.back", "groupBack", "group.background");
            theme.GroupBorder = ReadColor(tokens, theme.GroupBorder, "group.border", "groupBorder");
            theme.Text = ReadColor(tokens, theme.Text, "text", "foreground", "ribbon.text");
            theme.IconColor = ReadColor(tokens, theme.IconColor, "icon", "icon.color");
            theme.HoverBack = ReadColor(tokens, theme.HoverBack, "hover.back", "state.hover.background");
            theme.PressedBack = ReadColor(tokens, theme.PressedBack, "pressed.back", "state.pressed.background");
            theme.SelectionBack = ReadColor(tokens, theme.SelectionBack, "selection.back", "state.selected.background", "button.selected.back");
            theme.FocusBorder = ReadColor(tokens, theme.FocusBorder, "focus.border", "focus");
            theme.Separator = ReadColor(tokens, theme.Separator, "separator", "group.separator");
            theme.QuickAccessBack = ReadColor(tokens, theme.QuickAccessBack, "quickaccess.back", "quickAccessBack", "qat.background");
            theme.QuickAccessBorder = ReadColor(tokens, theme.QuickAccessBorder, "quickaccess.border", "quickAccessBorder", "qat.border");
            theme.DisabledBack = ReadColor(tokens, theme.DisabledBack, "disabled.back", "state.disabled.background");
            theme.DisabledText = ReadColor(tokens, theme.DisabledText, "disabled.text", "state.disabled.text", "disabled.fore");
            theme.DisabledBorder = ReadColor(tokens, theme.DisabledBorder, "disabled.border", "state.disabled.border");
            theme.ElevationColor = ReadColor(tokens, theme.ElevationColor, "elevation.color", "shadow.color");

            theme.CornerRadius = ReadInt(tokens, theme.CornerRadius, "corner.radius", "radius", "border.radius");
            theme.GroupSpacing = ReadInt(tokens, theme.GroupSpacing, "group.spacing", "spacing.group");
            theme.ItemSpacing = ReadInt(tokens, theme.ItemSpacing, "item.spacing", "spacing.item");
            theme.ElevationLevel = ReadInt(tokens, theme.ElevationLevel, "elevation.level", "shadow.level");
            theme.ElevationStrongLevel = ReadInt(tokens, theme.ElevationStrongLevel, "elevation.strong.level", "shadow.strong.level");
            theme.FocusBorderThickness = ReadFloat(tokens, theme.FocusBorderThickness, "focus.border.thickness", "focus.thickness");

            ApplyTypography(theme.CommandTypography, tokens, "command.font", "font.command");
            ApplyTypography(theme.TabTypography, tokens, "tab.font", "font.tab");
            ApplyTypography(theme.GroupTypography, tokens, "group.font", "font.group");
            ApplyTypography(theme.ContextHeaderTypography, tokens, "context.font", "font.context");
        }

        private static void ApplyTypography(TypographyStyle style, IReadOnlyDictionary<string, JsonElement> tokens, params string[] baseKeys)
        {
            foreach (var key in baseKeys)
            {
                string familyKey = $"{key}.family";
                string sizeKey = $"{key}.size";
                string weightKey = $"{key}.weight";

                if (TryReadString(tokens, familyKey, out var family) && !string.IsNullOrWhiteSpace(family))
                {
                    style.FontFamily = family;
                }

                if (TryReadFloat(tokens, sizeKey, out float size) && size > 0)
                {
                    style.FontSize = size;
                }

                if (TryReadInt(tokens, weightKey, out int weight) &&
                    Enum.IsDefined(typeof(FontWeight), weight))
                {
                    style.FontWeight = (FontWeight)weight;
                }
            }
        }

        private static bool ShouldSkipReservedProperty(string key, bool skipRoot)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return true;
            }

            if (key.Equals("$extensions", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("extensions", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("$description", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("description", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("$type", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("type", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("$extends", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("extends", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("inherits", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("inherit", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("parent", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (!skipRoot)
            {
                return false;
            }

            return key.Equals("tokens", StringComparison.OrdinalIgnoreCase) ||
                   key.Equals("modes", StringComparison.OrdinalIgnoreCase) ||
                   key.Equals("meta", StringComparison.OrdinalIgnoreCase) ||
                   key.Equals("$schema", StringComparison.OrdinalIgnoreCase) ||
                   key.Equals("schema", StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryGetLeafTokenValue(JsonElement source, out JsonElement value)
        {
            value = source;
            if (source.ValueKind != JsonValueKind.Object)
            {
                return true;
            }

            if (source.TryGetProperty("$value", out var dtcgValue))
            {
                value = dtcgValue;
                return true;
            }

            if (source.TryGetProperty("value", out var plainValue))
            {
                value = plainValue;
                return true;
            }

            return false;
        }

        private static void ValidateTokenBag(IReadOnlyDictionary<string, JsonElement> tokens, IList<string> diagnostics)
        {
            foreach (var pair in tokens)
            {
                string key = pair.Key;
                var value = pair.Value;

                if (value.ValueKind == JsonValueKind.String)
                {
                    string raw = value.GetString() ?? string.Empty;
                    if (TryParseReference(raw, out string reference))
                    {
                        if (!TryGetResolvedElement(tokens, reference, out _, diagnostics: diagnostics, sourceKey: key))
                        {
                            diagnostics.Add($"Unresolved token reference '{raw}' in '{key}'.");
                        }
                        continue;
                    }
                }

                if (IsLikelyColorToken(key))
                {
                    if (!CanInterpretAsColor(value))
                    {
                        diagnostics.Add($"Token '{key}' is expected to be a color but value '{FormatElement(value)}' is not valid.");
                    }
                    continue;
                }

                if (IsLikelyNumericToken(key) && !CanInterpretAsNumber(value))
                {
                    diagnostics.Add($"Token '{key}' is expected to be numeric but value '{FormatElement(value)}' is not valid.");
                }
            }
        }

        private static bool IsLikelyColorToken(string key)
        {
            string token = key.ToLowerInvariant();
            return token.Contains("color") ||
                   token.Contains("background") ||
                   token.Contains(".back") ||
                   token.Contains("border") ||
                   token.Contains("text") ||
                   token.Contains("foreground") ||
                   token.Contains("icon") ||
                   token.Contains("selection") ||
                   token.Contains("disabled") ||
                   token.Contains("shadow") ||
                   token.Contains("elevation") ||
                   token.Contains("hover") ||
                   token.Contains("pressed") ||
                   token.Contains("focus") ||
                   token.Contains("separator");
        }

        private static bool IsLikelyNumericToken(string key)
        {
            string token = key.ToLowerInvariant();
            return token.Contains("radius") ||
                   token.Contains("spacing") ||
                   token.Contains("weight") ||
                   token.Contains("size") ||
                   token.Contains("level") ||
                   token.Contains("thickness") ||
                   token.Contains("opacity") ||
                   token.EndsWith(".width", StringComparison.OrdinalIgnoreCase) ||
                   token.EndsWith(".height", StringComparison.OrdinalIgnoreCase);
        }

        private static bool CanInterpretAsColor(JsonElement value)
        {
            if (value.ValueKind == JsonValueKind.Number)
            {
                return value.TryGetInt32(out _);
            }

            if (value.ValueKind == JsonValueKind.String)
            {
                return ParseColor(value.GetString()).HasValue;
            }

            return false;
        }

        private static bool CanInterpretAsNumber(JsonElement value)
        {
            if (value.ValueKind == JsonValueKind.Number)
            {
                return value.TryGetDouble(out _);
            }

            if (value.ValueKind == JsonValueKind.String)
            {
                return double.TryParse(value.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out _);
            }

            return false;
        }

        private static string FormatElement(JsonElement value)
        {
            try
            {
                return value.ValueKind == JsonValueKind.String
                    ? value.GetString() ?? string.Empty
                    : value.GetRawText();
            }
            catch
            {
                return "<unreadable>";
            }
        }

        private static RibbonTheme CloneTheme(RibbonTheme source)
        {
            return new RibbonTheme
            {
                Background = source.Background,
                TabActiveBack = source.TabActiveBack,
                TabInactiveBack = source.TabInactiveBack,
                TabBorder = source.TabBorder,
                GroupBack = source.GroupBack,
                GroupBorder = source.GroupBorder,
                HoverBack = source.HoverBack,
                PressedBack = source.PressedBack,
                FocusBorder = source.FocusBorder,
                Separator = source.Separator,
                Text = source.Text,
                IconColor = source.IconColor,
                QuickAccessBack = source.QuickAccessBack,
                QuickAccessBorder = source.QuickAccessBorder,
                DisabledBack = source.DisabledBack,
                DisabledText = source.DisabledText,
                DisabledBorder = source.DisabledBorder,
                SelectionBack = source.SelectionBack,
                ElevationColor = source.ElevationColor,
                CornerRadius = source.CornerRadius,
                GroupSpacing = source.GroupSpacing,
                ItemSpacing = source.ItemSpacing,
                ElevationLevel = source.ElevationLevel,
                ElevationStrongLevel = source.ElevationStrongLevel,
                FocusBorderThickness = source.FocusBorderThickness,
                TabTypography = CopyTypography(source.TabTypography),
                GroupTypography = CopyTypography(source.GroupTypography),
                CommandTypography = CopyTypography(source.CommandTypography),
                ContextHeaderTypography = CopyTypography(source.ContextHeaderTypography)
            };
        }

        private static TypographyStyle CopyTypography(TypographyStyle source)
        {
            return new TypographyStyle
            {
                FontFamily = source.FontFamily,
                FontSize = source.FontSize,
                FontWeight = source.FontWeight,
                FontStyle = source.FontStyle,
                IsUnderlined = source.IsUnderlined,
                IsStrikeout = source.IsStrikeout
            };
        }

        private static Color ReadColor(IReadOnlyDictionary<string, JsonElement> tokens, Color fallback, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (!TryGetResolvedElement(tokens, key, out var value))
                {
                    continue;
                }

                if (value.ValueKind == JsonValueKind.String)
                {
                    var color = ParseColor(value.GetString());
                    if (color.HasValue) return color.Value;
                }

                if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out int argb))
                {
                    return Color.FromArgb(argb);
                }
            }

            return fallback;
        }

        private static int ReadInt(IReadOnlyDictionary<string, JsonElement> tokens, int fallback, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (TryReadInt(tokens, key, out int value))
                {
                    return value;
                }
            }

            return fallback;
        }

        private static float ReadFloat(IReadOnlyDictionary<string, JsonElement> tokens, float fallback, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (TryReadFloat(tokens, key, out float value))
                {
                    return value;
                }
            }

            return fallback;
        }

        private static bool TryReadInt(IReadOnlyDictionary<string, JsonElement> tokens, string key, out int value)
        {
            value = 0;
            if (!TryGetResolvedElement(tokens, key, out var element))
            {
                return false;
            }

            if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out value))
            {
                return true;
            }

            if (element.ValueKind == JsonValueKind.String &&
                int.TryParse(element.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }

            return false;
        }

        private static bool TryReadFloat(IReadOnlyDictionary<string, JsonElement> tokens, string key, out float value)
        {
            value = 0;
            if (!TryGetResolvedElement(tokens, key, out var element))
            {
                return false;
            }

            if (element.ValueKind == JsonValueKind.Number && element.TryGetSingle(out value))
            {
                return true;
            }

            if (element.ValueKind == JsonValueKind.String &&
                float.TryParse(element.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }

            return false;
        }

        private static bool TryReadString(IReadOnlyDictionary<string, JsonElement> tokens, string key, out string value)
        {
            value = string.Empty;
            if (!TryGetResolvedElement(tokens, key, out var element) || element.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            value = element.GetString() ?? string.Empty;
            return true;
        }

        private static bool TryGetResolvedElement(
            IReadOnlyDictionary<string, JsonElement> tokens,
            string key,
            out JsonElement value,
            HashSet<string>? visited = null,
            IList<string>? diagnostics = null,
            string? sourceKey = null)
        {
            value = default;
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            visited ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!visited.Add(key))
            {
                diagnostics?.Add($"Cyclic token reference detected while resolving '{sourceKey ?? key}'.");
                return false;
            }

            if (!tokens.TryGetValue(key, out var element))
            {
                diagnostics?.Add($"Token key '{key}' was not found while resolving '{sourceKey ?? key}'.");
                return false;
            }

            if (element.ValueKind == JsonValueKind.Object)
            {
                if (element.TryGetProperty("$value", out var tokenValue))
                {
                    element = tokenValue;
                }
                else if (element.TryGetProperty("value", out var fallbackValue))
                {
                    element = fallbackValue;
                }
            }

            if (element.ValueKind == JsonValueKind.String)
            {
                string raw = element.GetString() ?? string.Empty;
                if (TryParseReference(raw, out string referenceKey))
                {
                    return TryGetResolvedElement(tokens, referenceKey, out value, visited, diagnostics, sourceKey ?? key);
                }
            }

            value = element;
            return true;
        }

        private static bool TryParseReference(string raw, out string referenceKey)
        {
            referenceKey = string.Empty;
            if (string.IsNullOrWhiteSpace(raw))
            {
                return false;
            }

            string text = raw.Trim();
            if (text.StartsWith('{') && text.EndsWith('}') && text.Length > 2)
            {
                referenceKey = text[1..^1].Trim();
                return !string.IsNullOrWhiteSpace(referenceKey);
            }

            if (text.StartsWith('$') && text.Length > 1)
            {
                referenceKey = text[1..].Trim();
                return !string.IsNullOrWhiteSpace(referenceKey);
            }

            return false;
        }

        private static Color? ParseColor(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return null;
            }

            string text = raw.Trim();
            try
            {
                if (text.StartsWith('#'))
                {
                    if (text.Length == 7 || text.Length == 9)
                    {
                        return ColorTranslator.FromHtml(text);
                    }

                    return null;
                }

                var named = Color.FromName(text);
                if (named.IsKnownColor || named.IsSystemColor)
                {
                    return named;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
