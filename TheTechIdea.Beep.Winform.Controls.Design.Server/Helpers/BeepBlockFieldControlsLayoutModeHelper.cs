using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Integrated.Blocks.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Helpers
{
    public enum BeepBlockFieldControlsLayoutMode
    {
        StackedVertical,
        LabelFieldPairs,
        GridLayout
    }

    internal static class BeepBlockFieldControlsLayoutModeHelper
    {
        internal const string MetadataKey = "FieldControlsLayoutMode";

        private static readonly BeepBlockFieldControlsLayoutMode[] AvailableModes =
        {
            BeepBlockFieldControlsLayoutMode.StackedVertical,
            BeepBlockFieldControlsLayoutMode.LabelFieldPairs,
            BeepBlockFieldControlsLayoutMode.GridLayout
        };

        internal static IReadOnlyList<BeepBlockFieldControlsLayoutMode> GetAvailableModes()
            => AvailableModes;

        internal static BeepBlockFieldControlsLayoutMode Resolve(BeepBlockDefinition? definition)
        {
            if (definition?.Metadata != null &&
                definition.Metadata.TryGetValue(MetadataKey, out string? value) &&
                TryParse(value, out BeepBlockFieldControlsLayoutMode parsedMode))
            {
                return parsedMode;
            }

            return BeepBlockFieldControlsLayoutMode.StackedVertical;
        }

        internal static void Apply(BeepBlockDefinition definition, BeepBlockFieldControlsLayoutMode mode)
        {
            if (definition == null)
            {
                return;
            }

            definition.Metadata ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            definition.Metadata[MetadataKey] = GetMetadataValue(mode);
        }

        internal static void Clear(BeepBlockDefinition? definition)
        {
            if (definition?.Metadata == null)
            {
                return;
            }

            definition.Metadata.Remove(MetadataKey);
        }

        internal static string GetDisplayName(BeepBlockFieldControlsLayoutMode mode)
            => mode switch
            {
                BeepBlockFieldControlsLayoutMode.StackedVertical => "Stacked Vertical",
                BeepBlockFieldControlsLayoutMode.LabelFieldPairs => "Label Field Pairs",
                BeepBlockFieldControlsLayoutMode.GridLayout => "Grid Layout",
                _ => mode.ToString()
            };

        internal static string GetDescription(BeepBlockFieldControlsLayoutMode mode)
            => mode switch
            {
                BeepBlockFieldControlsLayoutMode.StackedVertical => "One column with the label above each generated control.",
                BeepBlockFieldControlsLayoutMode.LabelFieldPairs => "One column with the label on the left and the generated control on the right.",
                BeepBlockFieldControlsLayoutMode.GridLayout => "A two-column grid with each label stacked above its generated control.",
                _ => string.Empty
            };

        private static string GetMetadataValue(BeepBlockFieldControlsLayoutMode mode)
            => mode switch
            {
                BeepBlockFieldControlsLayoutMode.StackedVertical => "StackedVertical",
                BeepBlockFieldControlsLayoutMode.LabelFieldPairs => "LabelFieldPairs",
                BeepBlockFieldControlsLayoutMode.GridLayout => "GridLayout",
                _ => "StackedVertical"
            };

        private static bool TryParse(string? value, out BeepBlockFieldControlsLayoutMode mode)
        {
            mode = BeepBlockFieldControlsLayoutMode.StackedVertical;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string normalizedValue = value.Trim()
                .Replace(" ", string.Empty, StringComparison.Ordinal)
                .Replace("-", string.Empty, StringComparison.Ordinal)
                .Replace("_", string.Empty, StringComparison.Ordinal);

            mode = normalizedValue.ToLowerInvariant() switch
            {
                "stackedvertical" => BeepBlockFieldControlsLayoutMode.StackedVertical,
                "labelfieldpairs" => BeepBlockFieldControlsLayoutMode.LabelFieldPairs,
                "gridlayout" => BeepBlockFieldControlsLayoutMode.GridLayout,
                _ => BeepBlockFieldControlsLayoutMode.StackedVertical
            };

            return true;
        }
    }
}