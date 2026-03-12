using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Common;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    internal enum ComboBoxInteractionState
    {
        Normal,
        Hover,
        Focused,
        Open,
        Disabled,
        Error
    }

    internal sealed class ComboBoxVisualTokens
    {
        public int ButtonWidth { get; init; }
        public Padding InnerPadding { get; init; }
        public int CornerRadius { get; init; }
        public int TextInset { get; init; }
        public int RowDensity { get; init; }
        public int ChipHeight { get; init; }
        public bool ShowButtonSeparator { get; init; }
        public bool UseSegmentedTrigger { get; init; }
        public bool IsFilled { get; init; }
    }

    internal static class ComboBoxVisualTokenCatalog
    {
        public static ComboBoxVisualTokens Resolve(ComboBoxType type)
        {
            return type switch
            {
                ComboBoxType.OutlineSearchable => new ComboBoxVisualTokens
                {
                    ButtonWidth = 34,
                    InnerPadding = new Padding(12, 6, 8, 6),
                    CornerRadius = 6,
                    TextInset = 8,
                    RowDensity = 34,
                    ChipHeight = 24,
                    ShowButtonSeparator = true
                },
                ComboBoxType.FilledSoft => new ComboBoxVisualTokens
                {
                    ButtonWidth = 34,
                    InnerPadding = new Padding(12, 8, 8, 8),
                    CornerRadius = 8,
                    TextInset = 8,
                    RowDensity = 34,
                    ChipHeight = 24,
                    ShowButtonSeparator = false,
                    IsFilled = true
                },
                ComboBoxType.RoundedPill => new ComboBoxVisualTokens
                {
                    ButtonWidth = 36,
                    InnerPadding = new Padding(16, 8, 12, 8),
                    CornerRadius = 18,
                    TextInset = 10,
                    RowDensity = 34,
                    ChipHeight = 24,
                    ShowButtonSeparator = false
                },
                ComboBoxType.SegmentedTrigger => new ComboBoxVisualTokens
                {
                    ButtonWidth = 36,
                    InnerPadding = new Padding(12, 6, 8, 6),
                    CornerRadius = 8,
                    TextInset = 8,
                    RowDensity = 34,
                    ChipHeight = 24,
                    ShowButtonSeparator = true,
                    UseSegmentedTrigger = true
                },
                ComboBoxType.MultiChipCompact => new ComboBoxVisualTokens
                {
                    ButtonWidth = 36,
                    InnerPadding = new Padding(10, 6, 8, 6),
                    CornerRadius = 8,
                    TextInset = 8,
                    RowDensity = 34,
                    ChipHeight = 24,
                    ShowButtonSeparator = false
                },
                ComboBoxType.MultiChipSearch => new ComboBoxVisualTokens
                {
                    ButtonWidth = 36,
                    InnerPadding = new Padding(10, 6, 8, 6),
                    CornerRadius = 8,
                    TextInset = 8,
                    RowDensity = 36,
                    ChipHeight = 24,
                    ShowButtonSeparator = false
                },
                ComboBoxType.DenseList => new ComboBoxVisualTokens
                {
                    ButtonWidth = 32,
                    InnerPadding = new Padding(8, 4, 6, 4),
                    CornerRadius = 4,
                    TextInset = 6,
                    RowDensity = 30,
                    ChipHeight = 22,
                    ShowButtonSeparator = true
                },
                ComboBoxType.MinimalBorderless => new ComboBoxVisualTokens
                {
                    ButtonWidth = 24,
                    InnerPadding = new Padding(4, 6, 4, 6),
                    CornerRadius = 4,
                    TextInset = 6,
                    RowDensity = 32,
                    ChipHeight = 22,
                    ShowButtonSeparator = false
                },
                _ => new ComboBoxVisualTokens
                {
                    ButtonWidth = 34,
                    InnerPadding = new Padding(12, 6, 8, 6),
                    CornerRadius = 6,
                    TextInset = 8,
                    RowDensity = 34,
                    ChipHeight = 24,
                    ShowButtonSeparator = true
                }
            };
        }

        public static ComboBoxType MapFromControlStyle(BeepControlStyle style)
        {
            return style switch
            {
                BeepControlStyle.Material3 or BeepControlStyle.MaterialYou or BeepControlStyle.Material => ComboBoxType.FilledSoft,
                BeepControlStyle.iOS15 or BeepControlStyle.Apple or BeepControlStyle.PillRail => ComboBoxType.RoundedPill,
                BeepControlStyle.Fluent2 or BeepControlStyle.Fluent or BeepControlStyle.Windows11Mica => ComboBoxType.OutlineDefault,
                BeepControlStyle.Shadcn or BeepControlStyle.RadixUI or BeepControlStyle.NextJS or BeepControlStyle.Linear => ComboBoxType.OutlineDefault,
                BeepControlStyle.Minimal or BeepControlStyle.NotionMinimal or BeepControlStyle.VercelClean => ComboBoxType.MinimalBorderless,
                BeepControlStyle.NeoBrutalist or BeepControlStyle.Brutalist => ComboBoxType.SegmentedTrigger,
                BeepControlStyle.Bootstrap or BeepControlStyle.AntDesign or BeepControlStyle.ChakraUI => ComboBoxType.OutlineSearchable,
                _ => ComboBoxType.OutlineDefault
            };
        }

        public static bool SupportsSearch(ComboBoxType type)
            => type == ComboBoxType.OutlineSearchable || type == ComboBoxType.MultiChipSearch;

        /// <summary>
        /// Returns true only for types that use an inline text editor in the field.
        /// MultiChipSearch has its search box in the popup, not inline.
        /// </summary>
        public static bool SupportsInlineEditor(ComboBoxType type)
            => type == ComboBoxType.OutlineSearchable;
    }
}
