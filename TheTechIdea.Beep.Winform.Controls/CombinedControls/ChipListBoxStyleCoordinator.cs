using System;
using System.Collections.Generic;
using TheTechIdea.Beep.Winform.Controls.Chips;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;

namespace TheTechIdea.Beep.Winform.Controls.CombinedControls
{
    /// <summary>
    /// Coordinates visual styles between ListBox and ChipGroup components
    /// Provides mapping and synchronization for cohesive appearance
    /// </summary>
    public static class ChipListBoxStyleCoordinator
    {
        #region Style Mappings

        /// <summary>
        /// Maps ListBoxType to a corresponding ChipStyle for visual consistency
        /// </summary>
        private static readonly Dictionary<ListBoxType, ChipStyle> ListBoxToChipStyleMap = new Dictionary<ListBoxType, ChipStyle>
        {
            // Standard/Basic styles
            { ListBoxType.Standard, ChipStyle.Default },
            { ListBoxType.Minimal, ChipStyle.Minimalist },
            { ListBoxType.Outlined, ChipStyle.Classic },
            { ListBoxType.Rounded, ChipStyle.Pill },
            { ListBoxType.Borderless, ChipStyle.Minimalist },
            
            // Material/Modern styles
            { ListBoxType.MaterialOutlined, ChipStyle.Modern },
            { ListBoxType.Filled, ChipStyle.Default },
            
            // Category/Chip styles
            { ListBoxType.CategoryChips, ChipStyle.Pill },
            { ListBoxType.ChipStyle, ChipStyle.Default },
            
            // Searchable/Filter styles
            { ListBoxType.SearchableList, ChipStyle.Soft },
            { ListBoxType.FilterStatus, ChipStyle.Colorful },
            
            // Icon/Avatar styles
            { ListBoxType.WithIcons, ChipStyle.Modern },
            { ListBoxType.AvatarList, ChipStyle.Avatar },
            { ListBoxType.TeamMembers, ChipStyle.Avatar },
            
            // Checkbox styles
            { ListBoxType.CheckboxList, ChipStyle.Ingredient },
            { ListBoxType.OutlinedCheckboxes, ChipStyle.Classic },
            { ListBoxType.RaisedCheckboxes, ChipStyle.Elevated },
            
            // Selection styles
            { ListBoxType.SimpleList, ChipStyle.Soft },
            { ListBoxType.MultiSelectionTeal, ChipStyle.Shaded },
            { ListBoxType.ColoredSelection, ChipStyle.Colorful },
            { ListBoxType.RadioSelection, ChipStyle.Professional },
            { ListBoxType.FilledStyle, ChipStyle.Bold },
            
            // Card/Elevated styles
            { ListBoxType.CardList, ChipStyle.Elevated },
            { ListBoxType.GradientCard, ChipStyle.Shaded },
            
            // Special styles
            { ListBoxType.Compact, ChipStyle.Square },
            { ListBoxType.Grouped, ChipStyle.Professional },
            { ListBoxType.LanguageSelector, ChipStyle.Pill },
            { ListBoxType.ErrorStates, ChipStyle.HighContrast },
            { ListBoxType.Timeline, ChipStyle.Smooth },
            
            // Modern UI styles
            { ListBoxType.Glassmorphism, ChipStyle.Smooth },
            { ListBoxType.Neumorphic, ChipStyle.Elevated },
            
            // Custom
            { ListBoxType.Custom, ChipStyle.Default }
        };

        /// <summary>
        /// Maps ChipStyle to a corresponding ListBoxType for visual consistency
        /// </summary>
        private static readonly Dictionary<ChipStyle, ListBoxType> ChipToListBoxStyleMap = new Dictionary<ChipStyle, ListBoxType>
        {
            { ChipStyle.Default, ListBoxType.Standard },
            { ChipStyle.Modern, ListBoxType.MaterialOutlined },
            { ChipStyle.Classic, ListBoxType.Outlined },
            { ChipStyle.Minimalist, ListBoxType.Minimal },
            { ChipStyle.Colorful, ListBoxType.ColoredSelection },
            { ChipStyle.Professional, ListBoxType.Outlined },
            { ChipStyle.Soft, ListBoxType.SimpleList },
            { ChipStyle.HighContrast, ListBoxType.ErrorStates },
            { ChipStyle.Pill, ListBoxType.Rounded },
            { ChipStyle.Likeable, ListBoxType.ColoredSelection },
            { ChipStyle.Ingredient, ListBoxType.CheckboxList },
            { ChipStyle.Shaded, ListBoxType.GradientCard },
            { ChipStyle.Avatar, ListBoxType.AvatarList },
            { ChipStyle.Elevated, ListBoxType.CardList },
            { ChipStyle.Smooth, ListBoxType.Glassmorphism },
            { ChipStyle.Square, ListBoxType.Compact },
            { ChipStyle.Dashed, ListBoxType.Outlined },
            { ChipStyle.Bold, ListBoxType.FilledStyle }
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the recommended ChipStyle for a given ListBoxType
        /// </summary>
        public static ChipStyle GetChipStyleFor(ListBoxType listBoxType)
        {
            if (ListBoxToChipStyleMap.TryGetValue(listBoxType, out var chipStyle))
            {
                return chipStyle;
            }
            return ChipStyle.Default;
        }

        /// <summary>
        /// Gets the recommended ListBoxType for a given ChipStyle
        /// </summary>
        public static ListBoxType GetListBoxTypeFor(ChipStyle chipStyle)
        {
            if (ChipToListBoxStyleMap.TryGetValue(chipStyle, out var listBoxType))
            {
                return listBoxType;
            }
            return ListBoxType.Standard;
        }

        /// <summary>
        /// Gets a coordinated style pair from a unified style preset
        /// </summary>
        public static (ListBoxType ListStyle, ChipStyle ChipStyle) GetCoordinatedStyles(ChipListBoxStylePreset preset)
        {
            return preset switch
            {
                ChipListBoxStylePreset.Default => (ListBoxType.Standard, ChipStyle.Default),
                ChipListBoxStylePreset.Modern => (ListBoxType.MaterialOutlined, ChipStyle.Modern),
                ChipListBoxStylePreset.Minimal => (ListBoxType.Minimal, ChipStyle.Minimalist),
                ChipListBoxStylePreset.Outlined => (ListBoxType.Outlined, ChipStyle.Classic),
                ChipListBoxStylePreset.Rounded => (ListBoxType.Rounded, ChipStyle.Pill),
                ChipListBoxStylePreset.Colorful => (ListBoxType.ColoredSelection, ChipStyle.Colorful),
                ChipListBoxStylePreset.Professional => (ListBoxType.Outlined, ChipStyle.Professional),
                ChipListBoxStylePreset.Soft => (ListBoxType.SimpleList, ChipStyle.Soft),
                ChipListBoxStylePreset.HighContrast => (ListBoxType.ErrorStates, ChipStyle.HighContrast),
                ChipListBoxStylePreset.Avatar => (ListBoxType.AvatarList, ChipStyle.Avatar),
                ChipListBoxStylePreset.Card => (ListBoxType.CardList, ChipStyle.Elevated),
                ChipListBoxStylePreset.Glassmorphism => (ListBoxType.Glassmorphism, ChipStyle.Smooth),
                ChipListBoxStylePreset.Neumorphic => (ListBoxType.Neumorphic, ChipStyle.Elevated),
                ChipListBoxStylePreset.Gradient => (ListBoxType.GradientCard, ChipStyle.Shaded),
                ChipListBoxStylePreset.Checkbox => (ListBoxType.CheckboxList, ChipStyle.Ingredient),
                ChipListBoxStylePreset.Tag => (ListBoxType.CategoryChips, ChipStyle.Pill),
                ChipListBoxStylePreset.Timeline => (ListBoxType.Timeline, ChipStyle.Smooth),
                ChipListBoxStylePreset.Compact => (ListBoxType.Compact, ChipStyle.Square),
                _ => (ListBoxType.Standard, ChipStyle.Default)
            };
        }

        /// <summary>
        /// Gets the ChipSize that best matches the ListBox item height
        /// </summary>
        public static ChipSize GetChipSizeForItemHeight(int itemHeight)
        {
            if (itemHeight <= 24)
                return ChipSize.Small;
            else if (itemHeight <= 36)
                return ChipSize.Medium;
            else
                return ChipSize.Large;
        }

        /// <summary>
        /// Gets the recommended ListBox item height for a ChipSize
        /// </summary>
        public static int GetItemHeightForChipSize(ChipSize chipSize)
        {
            return chipSize switch
            {
                ChipSize.Small => 24,
                ChipSize.Medium => 32,
                ChipSize.Large => 40,
                _ => 32
            };
        }

        /// <summary>
        /// Determines if a ListBoxType naturally pairs with checkboxes
        /// </summary>
        public static bool ShouldShowCheckboxes(ListBoxType listBoxType)
        {
            return listBoxType switch
            {
                ListBoxType.CheckboxList => true,
                ListBoxType.OutlinedCheckboxes => true,
                ListBoxType.RaisedCheckboxes => true,
                ListBoxType.MultiSelectionTeal => true,
                ListBoxType.CategoryChips => true,
                _ => false
            };
        }

        /// <summary>
        /// Determines if a ListBoxType naturally includes a search box
        /// </summary>
        public static bool ShouldShowSearch(ListBoxType listBoxType)
        {
            return listBoxType switch
            {
                ListBoxType.SearchableList => true,
                ListBoxType.CategoryChips => true,
                ListBoxType.LanguageSelector => true,
                ListBoxType.TeamMembers => true,
                _ => false
            };
        }

        /// <summary>
        /// Determines if a ListBoxType naturally shows images/icons
        /// </summary>
        public static bool ShouldShowImages(ListBoxType listBoxType)
        {
            return listBoxType switch
            {
                ListBoxType.WithIcons => true,
                ListBoxType.AvatarList => true,
                ListBoxType.TeamMembers => true,
                ListBoxType.LanguageSelector => true,
                _ => false
            };
        }

        #endregion
    }

    #region Enums

    /// <summary>
    /// Defines how styles are coordinated between ListBox and ChipGroup
    /// </summary>
    public enum StyleCoordinationMode
    {
        /// <summary>
        /// Styles are automatically coordinated - changing either updates the other
        /// </summary>
        Automatic,

        /// <summary>
        /// ListBox style changes lead - ChipStyle follows ListBoxType
        /// </summary>
        ListLeads,

        /// <summary>
        /// ChipGroup style changes lead - ListBoxType follows ChipStyle
        /// </summary>
        ChipLeads,

        /// <summary>
        /// No automatic coordination - styles are set independently
        /// </summary>
        Manual
    }

    /// <summary>
    /// Unified style presets for BeepChipListBox that coordinate both ListBox and Chip styles
    /// </summary>
    public enum ChipListBoxStylePreset
    {
        /// <summary>Standard default appearance</summary>
        Default,
        
        /// <summary>Modern Material Design style</summary>
        Modern,
        
        /// <summary>Minimal clean design</summary>
        Minimal,
        
        /// <summary>Outlined borders style</summary>
        Outlined,
        
        /// <summary>Rounded/pill-shaped elements</summary>
        Rounded,
        
        /// <summary>Colorful vibrant appearance</summary>
        Colorful,
        
        /// <summary>Professional business style</summary>
        Professional,
        
        /// <summary>Soft pastel colors</summary>
        Soft,
        
        /// <summary>High contrast for accessibility</summary>
        HighContrast,
        
        /// <summary>Avatar/user-focused style</summary>
        Avatar,
        
        /// <summary>Card-based elevated style</summary>
        Card,
        
        /// <summary>Glassmorphism frosted glass effect</summary>
        Glassmorphism,
        
        /// <summary>Neumorphic soft shadows style</summary>
        Neumorphic,
        
        /// <summary>Gradient backgrounds</summary>
        Gradient,
        
        /// <summary>Checkbox-focused selection style</summary>
        Checkbox,
        
        /// <summary>Tag/category chip style</summary>
        Tag,
        
        /// <summary>Timeline/activity log style</summary>
        Timeline,
        
        /// <summary>Compact space-efficient style</summary>
        Compact
    }

    #endregion
}


