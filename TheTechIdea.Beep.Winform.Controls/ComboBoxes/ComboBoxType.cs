namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes
{
    /// <summary>
    /// Defines the visual Style/variant of the combo box
    /// Based on modern dropdown design patterns
    /// </summary>
    public enum ComboBoxType
    {
        /// <summary>
        /// Style #1: Simple rectangular dropdown with minimal border
        /// </summary>
        Minimal = 0,
        
        /// <summary>
        /// Style #2: Outlined Style with clear border and rounded corners
        /// </summary>
        Outlined = 1,
        
        /// <summary>
        /// Style #3: Rounded corners with prominent border radius
        /// </summary>
        Rounded = 2,
        
        /// <summary>
        /// Style #4: Material Design outlined variant with floating label
        /// </summary>
        MaterialOutlined = 3,
        
        /// <summary>
        /// Style #5: Filled background with subtle shadow elevation
        /// </summary>
        Filled = 4,
        
        /// <summary>
        /// Style #6: Clean minimal borderless design
        /// </summary>
        Borderless = 5,
        
        /// <summary>
        /// Standard dropdown (default Windows Style)
        /// </summary>
        Standard = 6,
        
        /// <summary>
        /// Blue themed dropdown with colored accents
        /// </summary>
        BlueDropdown = 7,
        
        /// <summary>
        /// Green themed dropdown with success/positive styling
        /// </summary>
        GreenDropdown = 8,
        
        /// <summary>
        /// Inverted color scheme dropdown (dark background)
        /// </summary>
        Inverted = 9,
        
        /// <summary>
        /// Error state dropdown with red/error styling
        /// </summary>
        Error = 10,
        
        /// <summary>
        /// Style with multiple selected items shown as chips/pills
        /// </summary>
        MultiSelectChips = 11,
        
        /// <summary>
        /// Dropdown with integrated search functionality
        /// </summary>
        SearchableDropdown = 12,
        
        /// <summary>
        /// Dropdown with icons displayed next to items
        /// </summary>
        WithIcons = 13,
        
        /// <summary>
        /// Menu-Style dropdown with categories/sections
        /// </summary>
        Menu = 14,
        
        /// <summary>
        /// Country selector dropdown with flags
        /// </summary>
        CountrySelector = 15,
        
        /// <summary>
        /// Dropdown with smooth, gradual border styling
        /// </summary>
        SmoothBorder = 16,
        
        /// <summary>
        /// Dark themed dropdown with prominent borders
        /// </summary>
        DarkBorder = 17,
        
        /// <summary>
        /// Pill-shaped dropdown with full rounded corners
        /// </summary>
        PillCorners = 18
    }
}
