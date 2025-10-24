namespace TheTechIdea.Beep.Winform.Controls.ListBoxs
{
    /// <summary>
    /// Defines the visual Style/variant of the list box
    /// Based on modern list and dropdown design patterns
    /// </summary>
    public enum ListBoxType
    {
        /// <summary>
        /// Standard list box (default Windows Style)
        /// </summary>
        Standard = 0,
        
        /// <summary>
        /// Minimal list with subtle styling
        /// </summary>
        Minimal = 1,
        
        /// <summary>
        /// Outlined list with clear borders
        /// </summary>
        Outlined = 2,
        
        /// <summary>
        /// Rounded corners with prominent border radius
        /// </summary>
        Rounded = 3,
        
        /// <summary>
        /// Material Design outlined variant
        /// </summary>
        MaterialOutlined = 4,
        
        /// <summary>
        /// Filled background with elevation
        /// </summary>
        Filled = 5,
        
        /// <summary>
        /// Borderless minimal design
        /// </summary>
        Borderless = 6,
        
        /// <summary>
        /// Category list with chips/tags for multi-select (from image 1)
        /// Shows selected items as blue chips at top
        /// </summary>
        CategoryChips = 7,
        
        /// <summary>
        /// Searchable list with integrated search (from images 1-3)
        /// Search field at top with real-time filtering
        /// </summary>
        SearchableList = 8,
        
        /// <summary>
        /// List with icons/flags next to items (from image 2 - country flags)
        /// </summary>
        WithIcons = 9,
        
        /// <summary>
        /// List with checkboxes for multi-select (from images 1, 3)
        /// Shows checkboxes on left or right side
        /// </summary>
        CheckboxList = 10,
        
        /// <summary>
        /// Simple category list (from image 4 - Biography, Comedy, etc.)
        /// Clean list without extras
        /// </summary>
        SimpleList = 11,
        
        /// <summary>
        /// Language/locale selector with flags (from image 5)
        /// Similar to country selector but for languages
        /// </summary>
        LanguageSelector = 12,
        
        /// <summary>
        /// Card-Style list with elevated items
        /// </summary>
        CardList = 13,
        
        /// <summary>
        /// Compact list with smaller items
        /// </summary>
        Compact = 14,
        
        /// <summary>
        /// Grouped list with category headers
        /// </summary>
        Grouped = 15,
        
        /// <summary>
        /// Team members list with avatars (from image 1 - top right)
        /// Shows user avatars and selection state
        /// </summary>
        TeamMembers = 16,
        
        /// <summary>
        /// Filled Style list with colored backgrounds (from image 1 - bottom right)
        /// Blue filled background for selected items
        /// </summary>
        FilledStyle = 17,
        
        /// <summary>
        /// Filter status list with colored backgrounds (from image 2 - top left)
        /// Yellow/Red colored item backgrounds with counts
        /// </summary>
        FilterStatus = 18,
        
        /// <summary>
        /// Outlined checkbox states (from image 2 - right side)
        /// Shows various checkbox states with outlines
        /// </summary>
        OutlinedCheckboxes = 19,
        
        /// <summary>
        /// Raised/elevated checkbox list (from image 2 - bottom right)
        /// Shows elevated error states
        /// </summary>
        RaisedCheckboxes = 20,
        
        /// <summary>
        /// Multiselection mode with light teal selected items (from images 1 & 3)
        /// Teal/cyan background for selected items
        /// </summary>
        MultiSelectionTeal = 21,
        
        /// <summary>
        /// Selected items with full width colored backgrounds (from image 4)
        /// Gray/Green colored full-width selection backgrounds
        /// </summary>
        ColoredSelection = 22,
        
        /// <summary>
        /// Single choice radio-Style list (from image 5 - Tickets Setup)
        /// Radio buttons on right, one item has colored background
        /// </summary>
        RadioSelection = 23,
        
        /// <summary>
        /// Job type list with error states (from image 2 - bottom left)
        /// Shows items with error badges and prohibited states
        /// </summary>
        ErrorStates = 24,
        
        /// <summary>
        /// Custom list box Style where developers can provide their own item rendering logic
        /// Allows full customization through CustomItemRenderer delegate
        /// </summary>
        Custom = 25
    }
}
