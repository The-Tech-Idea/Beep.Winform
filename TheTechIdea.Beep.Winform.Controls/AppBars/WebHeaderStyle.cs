using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.AppBars
{
    /// <summary>
    /// Enumeration of available WebHeader style variants
    /// Each style represents a complete design pattern from modern websites
    /// </summary>
    public enum WebHeaderStyle
    {
        /// <summary>Shoppy Store - E-commerce minimal style with category dropdown (Style 1)</summary>
        ShoppyStore1,

        /// <summary>Shoppy Store - E-commerce with centered tabs (Style 2)</summary>
        ShoppyStore2,

        /// <summary>TREND - Bold vibrant design with emphasis on action buttons</summary>
        TrendModern,

        /// <summary>Studiofok - Clean professional design with left-aligned navigation</summary>
        StudiofokMinimal,

        /// <summary>E-commerce Dark - Sleek dark design with product categories and cart</summary>
        EcommerceDark,

        /// <summary>SaaS Professional - Dashboard-style with user profile and notifications</summary>
        SaaSProfessional,

        /// <summary>Creative Agency - Bold typography with gradient backgrounds</summary>
        CreativeAgency,

        /// <summary>Corporate Minimal - Clean white design with subtle styling</summary>
        CorporateMinimal,

        /// <summary>Mobile-First - Responsive header optimized for smaller screens</summary>
        MobileFirst,

        /// <summary>Material Design - Google Material Design 3 pattern</summary>
        MaterialDesign3,

        /// <summary>Minimal Clean - Ultra-minimal design with text-only navigation</summary>
        MinimalClean,

        /// <summary>Multi-Row Compact - Multiple rows with category and secondary navigation</summary>
        MultiRowCompact,
        
        /// <summary>Startup Hero - Modern startup landing page style (GigSpot/BookMentor inspired)</summary>
        StartupHero,
        
        /// <summary>Portfolio Minimal - Personal portfolio/agency style (John Doe/Parise inspired)</summary>
        PortfolioMinimal,
        
        /// <summary>E-commerce Modern - Modern e-commerce with icons (StyleHub/HalfZone inspired)</summary>
        EcommerceModern
    }

    /// <summary>
    /// Enumeration of tab indicator styles for showing active tab
    /// </summary>
    public enum TabIndicatorStyle
    {
        /// <summary>Simple bottom line under active tab (thin line)</summary>
        UnderlineSimple,

        /// <summary>Full width bottom underline</summary>
        UnderlineFull,

        /// <summary>Pill-style background change</summary>
        PillBackground,

        /// <summary>Smooth sliding underline animation</summary>
        SlidingUnderline
    }

    /// <summary>
    /// Enumeration of button styles for action buttons
    /// </summary>
    public enum WebHeaderButtonStyle
    {
        /// <summary>Outline style - Border only</summary>
        Outline,

        /// <summary>Solid style - Filled background</summary>
        Solid,

        /// <summary>Minimal style - Text only</summary>
        Minimal,

        /// <summary>Ghost style - Very subtle</summary>
        Ghost
    }

    /// <summary>
    /// Represents a single tab in the WebHeader navigation
    /// </summary>
    public class WebHeaderTab
    {
        private static int _nextId = 1;
        private static int NextId => System.Threading.Interlocked.Increment(ref _nextId);

        public string Text { get; set; } = "";
        public string ImagePath { get; set; } = "";
        public string Tooltip { get; set; } = "";
        public int Id { get; set; } = -1;
        public bool IsActive { get; set; } = false;
        public bool IsHovered { get; set; } = false;
        public Rectangle Bounds { get; set; } = Rectangle.Empty;
        public object? Tag { get; set; }
        public bool HasChildren { get; set; } = false;

        public WebHeaderTab(string text, int id = -1)
        {
            Text = text;
            Id = id >= 0 ? id : NextId;
        }

        public WebHeaderTab(string text, string imagePath, int id = -1)
        {
            Text = text;
            ImagePath = imagePath;
            Id = id >= 0 ? id : NextId;
        }

        public override string ToString() => Text;
    }

    /// <summary>
    /// Represents an action button displayed on the right side of the header
    /// </summary>
    public class WebHeaderActionButton
    {
        private static int _nextId = 1;
        private static int NextId => System.Threading.Interlocked.Increment(ref _nextId);

        public string Text { get; set; } = "";
        public string ImagePath { get; set; } = "";
        public int Width { get; set; } = 100;
        public WebHeaderButtonStyle Style { get; set; } = WebHeaderButtonStyle.Outline;
        public Rectangle Bounds { get; set; } = Rectangle.Empty;
        public bool IsHovered { get; set; } = false;
        public int Id { get; set; } = -1;
        public object? Tag { get; set; }
        public int BadgeCount { get; set; } = 0;

        public WebHeaderActionButton(string text, WebHeaderButtonStyle style = WebHeaderButtonStyle.Outline, int id = -1)
        {
            Text = text;
            Style = style;
            Id = id >= 0 ? id : NextId;
        }

        public WebHeaderActionButton(string text, string imagePath, WebHeaderButtonStyle style = WebHeaderButtonStyle.Outline, int id = -1)
        {
            Text = text;
            ImagePath = imagePath;
            Style = style;
            Id = id >= 0 ? id : NextId;
        }

        public override string ToString() => Text;
    }

    /// <summary>
    /// Event args for search box text changes
    /// </summary>
    public class SearchChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new SearchChangedEventArgs
        /// </summary>
        /// <param name="searchText">The current search text</param>
        public SearchChangedEventArgs(string searchText)
        {
            SearchText = searchText;
        }

        /// <summary>Gets the current search text</summary>
        public string SearchText { get; }
    }
}
