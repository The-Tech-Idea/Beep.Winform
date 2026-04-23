using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Popup
{
    internal sealed class ComboBoxPopupHostProfile
    {
        public enum SearchPlacementMode
        {
            Top,
            Bottom
        }

        public string Name { get; init; } = "Default";
        public FormStyle FormStyle { get; init; } = FormStyle.Modern;

        public int BaseRowHeight { get; init; } = 32;
        public int GroupHeaderHeight { get; init; } = 28;
        public int DenseRowHeight { get; init; } = 30;
        public int SearchBoxHeight { get; init; } = 32;
        public int FooterHeight { get; init; } = 40;
        public int ListHorizontalPadding { get; init; } = 0;
        public int ListVerticalPadding { get; init; } = 0;
        public int MinWidth { get; init; } = 150;
        public int MaxHeight { get; init; } = 360;

        public bool ForceSearchVisible { get; init; }
        public bool ForceFooterVisible { get; init; }
        public bool ShowCheckmarkForSelected { get; init; } = true;
        public bool ShowRowSeparators { get; init; }
        public bool FooterLeftAligned { get; init; }
        public bool UseCardRows { get; init; }
        public int RowHorizontalInset { get; init; }
        public int RowVerticalInset { get; init; }
        public SearchPlacementMode SearchPlacement { get; init; } = SearchPlacementMode.Top;
        public string SearchPlaceholder { get; init; } = "Search...";

        // ── Pill grid layout (RoundedPill variant) ──────────────────────
        public int PillHeight { get; init; } = 36;
        public int PillSpacing { get; init; } = 4;

        // ── Chip header (MultiChip variants) ────────────────────────────
        public bool ShowChipArea { get; init; }
        public bool ShowDashedSeparator { get; init; }
        public int ChipAreaMaxHeight { get; init; } = 80;

        // ── Dense list (DenseList variant) ───────────────────────────────
        public bool UseCircularImages { get; init; }

        // ── Popup chrome (shadow + corner radius) ───────────────────────
        /// <summary>
        /// Corner radius for the popup form. 0 = use form default.
        /// </summary>
        public int PopupCornerRadius { get; init; }

        /// <summary>
        /// Shadow depth: 0 = none, 1 = light, 2 = medium, 3 = heavy.
        /// </summary>
        public int PopupShadowDepth { get; init; }

        public static ComboBoxPopupHostProfile OutlineDefault() => new ComboBoxPopupHostProfile
        {
            Name = "OutlineDefault",
            SearchPlaceholder = "Filter options...",
            BaseRowHeight = 32,
            PopupCornerRadius = 6,
            PopupShadowDepth = 1
        };

        public static ComboBoxPopupHostProfile OutlineSearchable() => new ComboBoxPopupHostProfile
        {
            Name = "OutlineSearchable",
            ForceSearchVisible = true,
            SearchPlaceholder = "Search options...",
            BaseRowHeight = 32,
            RowHorizontalInset = 2,
            PopupCornerRadius = 6,
            PopupShadowDepth = 1
        };

        public static ComboBoxPopupHostProfile FilledSoft() => new ComboBoxPopupHostProfile
        {
            Name = "FilledSoft",
            SearchPlaceholder = "Find...",
            BaseRowHeight = 34,
            UseCardRows = true,
            RowHorizontalInset = 4,
            RowVerticalInset = 2,
            PopupCornerRadius = 8,
            PopupShadowDepth = 2
        };

        public static ComboBoxPopupHostProfile RoundedPill() => new ComboBoxPopupHostProfile
        {
            Name = "RoundedPill",
            ForceSearchVisible = true,
            SearchPlaceholder = "Search tags...",
            ListHorizontalPadding = 8,
            BaseRowHeight = 36,
            UseCardRows = false,
            SearchPlacement = SearchPlacementMode.Bottom,
            PillHeight = 36,
            PillSpacing = 6,
            MaxHeight = 400,
            PopupCornerRadius = 12,
            PopupShadowDepth = 2
        };

        public static ComboBoxPopupHostProfile SegmentedTrigger() => new ComboBoxPopupHostProfile
        {
            Name = "SegmentedTrigger",
            SearchPlaceholder = "Filter list...",
            BaseRowHeight = 32,
            FooterLeftAligned = true,
            ShowRowSeparators = true,
            PopupCornerRadius = 6,
            PopupShadowDepth = 1
        };

        public static ComboBoxPopupHostProfile MultiChipCompact() => new ComboBoxPopupHostProfile
        {
            Name = "MultiChipCompact",
            ForceFooterVisible = true,
            ShowChipArea = true,
            ShowDashedSeparator = true,
            SearchPlaceholder = "Filter selected...",
            BaseRowHeight = 32,
            FooterHeight = 42,
            FooterLeftAligned = true,
            ChipAreaMaxHeight = 80,
            MaxHeight = 420,
            PopupCornerRadius = 8,
            PopupShadowDepth = 2
        };

        public static ComboBoxPopupHostProfile MultiChipSearch() => new ComboBoxPopupHostProfile
        {
            Name = "MultiChipSearch",
            ForceSearchVisible = true,
            ForceFooterVisible = true,
            ShowChipArea = true,
            ShowDashedSeparator = true,
            SearchPlaceholder = "Search categories...",
            BaseRowHeight = 34,
            FooterHeight = 42,
            ChipAreaMaxHeight = 80,
            MaxHeight = 450,
            RowHorizontalInset = 2,
            PopupCornerRadius = 8,
            PopupShadowDepth = 2
        };

        public static ComboBoxPopupHostProfile DenseList() => new ComboBoxPopupHostProfile
        {
            Name = "DenseList",
            ForceSearchVisible = true,
            SearchPlaceholder = "Add guests...",
            BaseRowHeight = 32,
            DenseRowHeight = 28,
            GroupHeaderHeight = 24,
            ShowRowSeparators = false,
            UseCircularImages = true,
            MaxHeight = 320,
            PopupCornerRadius = 4,
            PopupShadowDepth = 1
        };

        public static ComboBoxPopupHostProfile MinimalBorderless() => new ComboBoxPopupHostProfile
        {
            Name = "MinimalBorderless",
            SearchPlaceholder = "Search...",
            ShowCheckmarkForSelected = false,
            BaseRowHeight = 34,
            ShowRowSeparators = false,
            RowHorizontalInset = 4,
            RowVerticalInset = 1,
            ListHorizontalPadding = 2,
            ListVerticalPadding = 4,
            PopupCornerRadius = 4,
            PopupShadowDepth = 0
        };

        public static ComboBoxPopupHostProfile CommandMenu() => new ComboBoxPopupHostProfile
        {
            Name = "CommandMenu",
            SearchPlaceholder = "Type a command...",
            BaseRowHeight = 34,
            ShowRowSeparators = false,
            RowHorizontalInset = 4,
            RowVerticalInset = 1,
            ListHorizontalPadding = 4,
            ListVerticalPadding = 4,
            MaxHeight = 360
        };

        public static ComboBoxPopupHostProfile VisualDisplay() => new ComboBoxPopupHostProfile
        {
            Name = "VisualDisplay",
            SearchPlaceholder = "Search options...",
            BaseRowHeight = 34,
            ShowRowSeparators = false,
            RowHorizontalInset = 2,
            RowVerticalInset = 1,
            ListHorizontalPadding = 2,
            ListVerticalPadding = 4,
            MaxHeight = 360
        };
    }
}
