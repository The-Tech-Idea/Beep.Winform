using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.GridX.Painters
{
    /// <summary>
    /// Interface for navigation painter implementations.
    /// Allows different visual styles for grid navigation controls.
    /// </summary>
    public interface INavigationPainter
    {
        /// <summary>
        /// Gets the style type this painter implements
        /// </summary>
        navigationStyle Style { get; }

        /// <summary>
        /// Gets the recommended height for the navigation area
        /// </summary>
        int RecommendedHeight { get; }

        /// <summary>
        /// Gets the recommended minimum width for the navigation area
        /// </summary>
        int RecommendedMinWidth { get; }

        /// <summary>
        /// Paint the complete navigation area
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Total bounds available for navigation</param>
        /// <param name="grid">The grid control</param>
        /// <param name="theme">Current theme colors</param>
        void PaintNavigation(Graphics g, Rectangle bounds, BeepGridPro grid, IBeepTheme theme);

        /// <summary>
        /// Paint a single navigation button
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Button bounds</param>
        /// <param name="buttonType">Type of button</param>
        /// <param name="state">Button state</param>
        /// <param name="component">UI component representing the button</param>
        /// <param name="theme">Current theme colors</param>
        void PaintButton(Graphics g, Rectangle bounds, NavigationButtonType buttonType, 
            NavigationButtonState state, IBeepUIComponent component, IBeepTheme theme);

        /// <summary>
        /// Paint the record position indicator (e.g., "1 of 100")
        /// </summary>
        /// <param name="g">Graphics context</param>
        /// <param name="bounds">Indicator bounds</param>
        /// <param name="currentPosition">Current record position (1-based)</param>
        /// <param name="totalRecords">Total number of records</param>
        /// <param name="theme">Current theme colors</param>
        void PaintPositionIndicator(Graphics g, Rectangle bounds, int currentPosition, 
            int totalRecords, IBeepTheme theme);

        /// <summary>
        /// Calculate the layout for navigation elements
        /// </summary>
        /// <param name="availableBounds">Available space for navigation</param>
        /// <param name="totalRecords">Total number of records</param>
        /// <param name="showCrudButtons">Whether to show CRUD buttons (Add, Delete, Save, Cancel)</param>
        /// <returns>Layout information including button positions</returns>
        NavigationLayout CalculateLayout(Rectangle availableBounds, int totalRecords, 
            bool showCrudButtons);

        /// <summary>
        /// Get the icon/text for a specific button type
        /// </summary>
        /// <param name="buttonType">Button type</param>
        /// <returns>Display text or icon character</returns>
        string GetButtonContent(NavigationButtonType buttonType);

        /// <summary>
        /// Get tooltip text for a button
        /// </summary>
        /// <param name="buttonType">Button type</param>
        /// <returns>Tooltip text</returns>
        string GetButtonTooltip(NavigationButtonType buttonType);
    }

    /// <summary>
    /// Layout information for navigation elements
    /// </summary>
    public class NavigationLayout
    {
        // Navigation Buttons
        public Rectangle FirstButtonRect { get; set; }
        public Rectangle PreviousButtonRect { get; set; }
        public Rectangle NextButtonRect { get; set; }
        public Rectangle LastButtonRect { get; set; }
        
        // CRUD Buttons
        public Rectangle AddNewButtonRect { get; set; }
        public Rectangle DeleteButtonRect { get; set; }
        public Rectangle SaveButtonRect { get; set; }
        public Rectangle CancelButtonRect { get; set; }
        public Rectangle EditButtonRect { get; set; }
        public Rectangle RefreshButtonRect { get; set; }
        
        // Information Display Areas
        public Rectangle PositionIndicatorRect { get; set; }
        public Rectangle PageInfoRect { get; set; }
        public Rectangle RecordCountRect { get; set; }
        public Rectangle StatusTextRect { get; set; }
        
        // Pagination Controls (for web-style navigation)
        public Rectangle PageSizeComboRect { get; set; }
        public Rectangle PageNumberInputRect { get; set; }
        public Rectangle GoToPageButtonRect { get; set; }
        public Rectangle[] PageNumberRects { get; set; } = new Rectangle[0]; // For numbered pagination
        
        // Search/Filter Controls
        public Rectangle QuickSearchRect { get; set; }
        public Rectangle FilterButtonRect { get; set; }
        public Rectangle ColumnsButtonRect { get; set; }
        public Rectangle ExportButtonRect { get; set; }
        public Rectangle PrintButtonRect { get; set; }
        
        // Additional Action Buttons
        public Rectangle SettingsButtonRect { get; set; }
        public Rectangle MoreActionsButtonRect { get; set; }
        public Rectangle HelpButtonRect { get; set; }
        
        // Layout Sections (for organizing different areas)
        public Rectangle LeftSectionRect { get; set; }
        public Rectangle CenterSectionRect { get; set; }
        public Rectangle RightSectionRect { get; set; }
        public Rectangle TopSectionRect { get; set; }
        public Rectangle BottomSectionRect { get; set; }
        
        // Spacing and Metrics
        public int ButtonSpacing { get; set; } = 4;
        public int GroupSpacing { get; set; } = 12;
        public int Padding { get; set; } = 8;
        public Size ButtonSize { get; set; }
        
        /// <summary>
        /// Total calculated height needed
        /// </summary>
        public int TotalHeight { get; set; }
        
        /// <summary>
        /// Total calculated width needed
        /// </summary>
        public int TotalWidth { get; set; }
        
        /// <summary>
        /// Whether the layout uses compact mode
        /// </summary>
        public bool IsCompact { get; set; }
        
        /// <summary>
        /// Whether the layout uses icon-only buttons
        /// </summary>
        public bool IconOnly { get; set; }
        
        /// <summary>
        /// Current page number (for paginated styles)
        /// </summary>
        public int CurrentPage { get; set; }
        
        /// <summary>
        /// Total number of pages (for paginated styles)
        /// </summary>
        public int TotalPages { get; set; }
        
        /// <summary>
        /// Number of page buttons to show (for numbered pagination)
        /// </summary>
        public int VisiblePageButtons { get; set; } = 5;
    }
}
