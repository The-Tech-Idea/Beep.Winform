using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.ListBoxs
{
    /// <summary>
    /// Non-breaking metadata that describes behavioral/layout intent for each distinct ListBoxType.
    /// This does not consolidate or alias variants; it standardizes how infrastructure interprets them.
    /// </summary>
    public sealed class ListBoxVariantMetadata
    {
        public ListBoxType Variant { get; init; }
        public string Surface { get; init; } = "standard";
        public string SelectionAffordance { get; init; } = "row";
        public ListDensityMode DensityDefault { get; init; } = ListDensityMode.Comfortable;
        public string ContentLayout { get; init; } = "titleOnly";
        public string InteractionPattern { get; init; } = "listSelect";
        public bool SupportsSearch { get; init; }
        public bool SupportsCheckboxes { get; init; }
        public bool SupportsGrouping { get; init; }
    }

    internal static class ListBoxVariantMetadataCatalog
    {
        private static readonly IReadOnlyDictionary<ListBoxType, ListBoxVariantMetadata> _map =
            new Dictionary<ListBoxType, ListBoxVariantMetadata>
            {
                [ListBoxType.Standard] = Build(ListBoxType.Standard, "standard", "row", ListDensityMode.Comfortable, "titleOnly", "listSelect", true, true, true),
                [ListBoxType.Minimal] = Build(ListBoxType.Minimal, "minimal", "row", ListDensityMode.Compact, "titleOnly", "listSelect", true, false, false),
                [ListBoxType.Outlined] = Build(ListBoxType.Outlined, "outlined", "row", ListDensityMode.Comfortable, "titleOnly", "listSelect", true, true, true),
                [ListBoxType.Rounded] = Build(ListBoxType.Rounded, "rounded", "row", ListDensityMode.Comfortable, "titleOnly", "listSelect", true, true, true),
                [ListBoxType.MaterialOutlined] = Build(ListBoxType.MaterialOutlined, "outlined", "row", ListDensityMode.Comfortable, "titleSubtext", "listSelect", true, true, true),
                [ListBoxType.Filled] = Build(ListBoxType.Filled, "filled", "row", ListDensityMode.Comfortable, "titleOnly", "listSelect", true, true, true),
                [ListBoxType.Borderless] = Build(ListBoxType.Borderless, "borderless", "row", ListDensityMode.Compact, "titleOnly", "listSelect", true, true, false),
                [ListBoxType.CategoryChips] = Build(ListBoxType.CategoryChips, "chips", "chip", ListDensityMode.Comfortable, "chipWrap", "multiSelect", true, true, false),
                [ListBoxType.SearchableList] = Build(ListBoxType.SearchableList, "outlined", "row", ListDensityMode.Comfortable, "titleSubtext", "searchSelect", true, true, true),
                [ListBoxType.WithIcons] = Build(ListBoxType.WithIcons, "standard", "row", ListDensityMode.Comfortable, "leadingIcon", "listSelect", true, false, true),
                [ListBoxType.CheckboxList] = Build(ListBoxType.CheckboxList, "standard", "checkbox", ListDensityMode.Comfortable, "checkboxDescription", "multiSelect", true, true, true),
                [ListBoxType.SimpleList] = Build(ListBoxType.SimpleList, "minimal", "row", ListDensityMode.Compact, "titleOnly", "listSelect", false, false, false),
                [ListBoxType.LanguageSelector] = Build(ListBoxType.LanguageSelector, "standard", "row", ListDensityMode.Comfortable, "leadingIconTrailingMeta", "listSelect", true, false, false),
                [ListBoxType.CardList] = Build(ListBoxType.CardList, "card", "row", ListDensityMode.Comfortable, "titleSubtext", "listSelect", true, true, true),
                [ListBoxType.Compact] = Build(ListBoxType.Compact, "compact", "row", ListDensityMode.Dense, "titleOnly", "listSelect", true, false, false),
                [ListBoxType.Grouped] = Build(ListBoxType.Grouped, "outlined", "row", ListDensityMode.Comfortable, "titleSubtext", "groupedSelect", true, true, true),
                [ListBoxType.TeamMembers] = Build(ListBoxType.TeamMembers, "card", "row", ListDensityMode.Comfortable, "avatarSecondaryAction", "listSelect", true, false, true),
                [ListBoxType.FilledStyle] = Build(ListBoxType.FilledStyle, "filled", "row", ListDensityMode.Comfortable, "titleOnly", "listSelect", false, false, false),
                [ListBoxType.FilterStatus] = Build(ListBoxType.FilterStatus, "outlined", "row", ListDensityMode.Compact, "leadingIconTrailingMeta", "listSelect", true, false, true),
                [ListBoxType.OutlinedCheckboxes] = Build(ListBoxType.OutlinedCheckboxes, "outlined", "checkbox", ListDensityMode.Comfortable, "checkboxDescription", "multiSelect", true, true, true),
                [ListBoxType.RaisedCheckboxes] = Build(ListBoxType.RaisedCheckboxes, "card", "checkbox", ListDensityMode.Comfortable, "checkboxDescription", "multiSelect", true, true, true),
                [ListBoxType.MultiSelectionTeal] = Build(ListBoxType.MultiSelectionTeal, "filled", "row", ListDensityMode.Comfortable, "titleOnly", "multiSelect", true, false, false),
                [ListBoxType.ColoredSelection] = Build(ListBoxType.ColoredSelection, "filled", "row", ListDensityMode.Comfortable, "titleOnly", "listSelect", false, false, false),
                [ListBoxType.RadioSelection] = Build(ListBoxType.RadioSelection, "outlined", "radio", ListDensityMode.Comfortable, "titleSubtext", "singleChoice", true, false, false),
                [ListBoxType.ErrorStates] = Build(ListBoxType.ErrorStates, "outlined", "row", ListDensityMode.Comfortable, "leadingIconTrailingMeta", "listSelect", true, false, true),
                [ListBoxType.Custom] = Build(ListBoxType.Custom, "custom", "custom", ListDensityMode.Comfortable, "custom", "custom", true, true, true),
                [ListBoxType.Glassmorphism] = Build(ListBoxType.Glassmorphism, "glass", "row", ListDensityMode.Comfortable, "titleSubtext", "listSelect", true, true, true),
                [ListBoxType.Neumorphic] = Build(ListBoxType.Neumorphic, "neumorphic", "row", ListDensityMode.Comfortable, "titleOnly", "listSelect", true, true, true),
                [ListBoxType.GradientCard] = Build(ListBoxType.GradientCard, "gradient", "row", ListDensityMode.Comfortable, "titleSubtext", "listSelect", true, true, true),
                [ListBoxType.ChipStyle] = Build(ListBoxType.ChipStyle, "chips", "chip", ListDensityMode.Compact, "chipWrap", "multiSelect", true, false, false),
                [ListBoxType.AvatarList] = Build(ListBoxType.AvatarList, "card", "row", ListDensityMode.Comfortable, "avatarSecondaryAction", "listSelect", true, false, true),
                [ListBoxType.Timeline] = Build(ListBoxType.Timeline, "timeline", "row", ListDensityMode.Comfortable, "titleSubtext", "timeline", true, false, true),
                [ListBoxType.InfiniteScroll] = Build(ListBoxType.InfiniteScroll, "outlined", "row", ListDensityMode.Comfortable, "titleOnly", "infiniteScroll", true, false, false),
                [ListBoxType.CommandList] = Build(ListBoxType.CommandList, "minimal", "row", ListDensityMode.Compact, "leadingIconTrailingMeta", "commandPalette", true, false, true),
                [ListBoxType.NavigationRail] = Build(ListBoxType.NavigationRail, "rail", "row", ListDensityMode.Compact, "iconTopLabel", "navigation", false, false, false),
                [ListBoxType.ChatList] = Build(ListBoxType.ChatList, "standard", "row", ListDensityMode.Comfortable, "avatarSecondaryAction", "listSelect", true, false, false),
                [ListBoxType.ContactList] = Build(ListBoxType.ContactList, "standard", "row", ListDensityMode.Comfortable, "avatarSecondaryAction", "listSelect", true, false, true),
                [ListBoxType.ThreeLineList] = Build(ListBoxType.ThreeLineList, "standard", "row", ListDensityMode.Comfortable, "titleSubtext", "listSelect", true, true, true),
                [ListBoxType.NotificationList] = Build(ListBoxType.NotificationList, "standard", "row", ListDensityMode.Comfortable, "leadingIconTrailingMeta", "listSelect", true, false, false),
                [ListBoxType.ProfileCard] = Build(ListBoxType.ProfileCard, "card", "row", ListDensityMode.Comfortable, "avatarSecondaryAction", "listSelect", false, false, false),
                [ListBoxType.RekaUI] = Build(ListBoxType.RekaUI, "minimal", "row", ListDensityMode.Compact, "titleSubtext", "listSelect", true, true, true),
                [ListBoxType.ChakraUI] = Build(ListBoxType.ChakraUI, "outlined", "row", ListDensityMode.Comfortable, "titleOnly", "listSelect", true, true, true),
                [ListBoxType.HeroUI] = Build(ListBoxType.HeroUI, "card", "row", ListDensityMode.Comfortable, "titleSubtext", "listSelect", true, true, true),
            };

        public static ListBoxVariantMetadata Resolve(ListBoxType type)
            => _map.TryGetValue(type, out var metadata)
                ? metadata
                : Build(type, "standard", "row", ListDensityMode.Comfortable, "titleOnly", "listSelect", true, true, true);

        private static ListBoxVariantMetadata Build(
            ListBoxType variant,
            string surface,
            string selectionAffordance,
            ListDensityMode density,
            string contentLayout,
            string interactionPattern,
            bool supportsSearch,
            bool supportsCheckboxes,
            bool supportsGrouping)
            => new ListBoxVariantMetadata
            {
                Variant = variant,
                Surface = surface,
                SelectionAffordance = selectionAffordance,
                DensityDefault = density,
                ContentLayout = contentLayout,
                InteractionPattern = interactionPattern,
                SupportsSearch = supportsSearch,
                SupportsCheckboxes = supportsCheckboxes,
                SupportsGrouping = supportsGrouping
            };
    }
}
