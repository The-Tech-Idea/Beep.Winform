namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes
{
    /// <summary>
    /// Defines the design-system variant of the combo box.
    /// These values intentionally replace the legacy enum set.
    /// </summary>
    public enum ComboBoxType
    {
        /// <summary>Primary enterprise outlined dropdown.</summary>
        OutlineDefault = 0,

        /// <summary>Outlined dropdown with search-first popup behavior.</summary>
        OutlineSearchable = 1,

        /// <summary>Soft filled field with subtle contrast and modern density.</summary>
        FilledSoft = 2,

        /// <summary>High-radius pill shell.</summary>
        RoundedPill = 3,

        /// <summary>Split trigger with a visually distinct chevron segment.</summary>
        SegmentedTrigger = 4,

        /// <summary>Multi-select chip field with compact token layout.</summary>
        MultiChipCompact = 5,

        /// <summary>Multi-select chips with search + checkbox popup flow.</summary>
        MultiChipSearch = 6,

        /// <summary>Data-dense list-oriented variant.</summary>
        DenseList = 7,

        /// <summary>Low-chrome borderless/minimal shell.</summary>
        MinimalBorderless = 8,

        /// <summary>Command-menu style field and popup rows with shortcut metadata.</summary>
        CommandMenu = 9,

        /// <summary>Visual-forward display (icon/swatch-first) for selected values.</summary>
        VisualDisplay = 10
    }
}
