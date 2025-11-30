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
        /// <summary>Gets or sets the tab text</summary>
        public string Text { get; set; } = "";

        /// <summary>Gets or sets the optional icon image path (SVG embedded resource path)</summary>
        public string ImagePath { get; set; } = "";

        /// <summary>Gets or sets the tooltip text shown on hover</summary>
        public string Tooltip { get; set; } = "";

        /// <summary>Gets or sets the unique tab identifier</summary>
        public int Id { get; set; } = -1;

        /// <summary>Gets or sets whether this tab is currently active/selected</summary>
        public bool IsActive { get; set; } = false;

        /// <summary>Gets or sets whether this tab is currently hovered</summary>
        public bool IsHovered { get; set; } = false;

        /// <summary>Gets or sets the calculated rectangle for this tab (set at draw time)</summary>
        public Rectangle Bounds { get; set; } = Rectangle.Empty;

        /// <summary>Gets or sets custom data associated with this tab</summary>
        public object? Tag { get; set; }

        /// <summary>Gets or sets whether this tab has child items (dropdown)</summary>
        public bool HasChildren { get; set; } = false;

        /// <summary>
        /// Creates a new WebHeaderTab with the specified text
        /// </summary>
        public WebHeaderTab(string text, int id = -1)
        {
            Text = text;
            Id = id >= 0 ? id : new Random().Next();
        }

        /// <summary>
        /// Creates a new WebHeaderTab with text and icon
        /// </summary>
        public WebHeaderTab(string text, string imagePath, int id = -1)
        {
            Text = text;
            ImagePath = imagePath;
            Id = id >= 0 ? id : new Random().Next();
        }

        /// <summary>Returns the tab text</summary>
        public override string ToString() => Text;
    }

    /// <summary>
    /// Represents an action button displayed on the right side of the header
    /// </summary>
    public class WebHeaderActionButton
    {
        /// <summary>Gets or sets the button text</summary>
        public string Text { get; set; } = "";

        /// <summary>Gets or sets the button icon image path (SVG embedded resource path)</summary>
        public string ImagePath { get; set; } = "";

        /// <summary>Gets or sets the preferred button width in pixels</summary>
        public int Width { get; set; } = 100;

        /// <summary>Gets or sets the button style (Outline, Solid, Minimal, Ghost)</summary>
        public WebHeaderButtonStyle Style { get; set; } = WebHeaderButtonStyle.Outline;

        /// <summary>Gets or sets the calculated rectangle for this button (set at draw time)</summary>
        public Rectangle Bounds { get; set; } = Rectangle.Empty;

        /// <summary>Gets or sets whether this button is currently hovered</summary>
        public bool IsHovered { get; set; } = false;

        /// <summary>Gets or sets the unique button identifier</summary>
        public int Id { get; set; } = -1;

        /// <summary>Gets or sets custom data associated with this button</summary>
        public object? Tag { get; set; }

        /// <summary>Gets or sets the badge count for notification icons (0 = no badge)</summary>
        public int BadgeCount { get; set; } = 0;

        /// <summary>
        /// Creates a new WebHeaderActionButton
        /// </summary>
        public WebHeaderActionButton(string text, WebHeaderButtonStyle style = WebHeaderButtonStyle.Outline, int id = -1)
        {
            Text = text;
            Style = style;
            Id = id >= 0 ? id : new Random().Next();
        }

        /// <summary>
        /// Creates a new WebHeaderActionButton with icon
        /// </summary>
        public WebHeaderActionButton(string text, string imagePath, WebHeaderButtonStyle style = WebHeaderButtonStyle.Outline, int id = -1)
        {
            Text = text;
            ImagePath = imagePath;
            Style = style;
            Id = id >= 0 ? id : new Random().Next();
        }

        /// <summary>Returns the button text</summary>
        public override string ToString() => Text;
    }

    /// <summary>
    /// Event args for tab selection
    /// </summary>
    public class TabSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new TabSelectedEventArgs
        /// </summary>
        /// <param name="tabIndex">The index of the selected tab</param>
        /// <param name="tab">The selected tab object</param>
        public TabSelectedEventArgs(int tabIndex, WebHeaderTab tab)
        {
            TabIndex = tabIndex;
            Tab = tab;
        }

        /// <summary>Gets the index of the selected tab</summary>
        public int TabIndex { get; }

        /// <summary>Gets the selected tab object</summary>
        public WebHeaderTab Tab { get; }

        /// <summary>Gets the tab ID</summary>
        public int TabId => Tab?.Id ?? -1;

        /// <summary>Gets the tab text</summary>
        public string TabText => Tab?.Text ?? "";
    }

    /// <summary>
    /// Event args for action button clicks
    /// </summary>
    public class ActionButtonClickedEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new ActionButtonClickedEventArgs
        /// </summary>
        /// <param name="buttonIndex">The index of the clicked button</param>
        /// <param name="button">The clicked button object</param>
        public ActionButtonClickedEventArgs(int buttonIndex, WebHeaderActionButton button)
        {
            ButtonIndex = buttonIndex;
            Button = button;
        }

        /// <summary>Gets the index of the clicked button</summary>
        public int ButtonIndex { get; }

        /// <summary>Gets the clicked button object</summary>
        public WebHeaderActionButton Button { get; }

        /// <summary>Gets the button text</summary>
        public string ButtonText => Button?.Text ?? "";

        /// <summary>Gets the button ID</summary>
        public int ButtonId => Button?.Id ?? -1;
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

    /// <summary>
    /// Configuration for WebHeader appearance and behavior
    /// </summary>
    public class WebHeaderConfig
    {
        /// <summary>Gets or sets the header style</summary>
        public WebHeaderStyle HeaderStyle { get; set; } = WebHeaderStyle.ShoppyStore1;

        /// <summary>Gets or sets the tab indicator style</summary>
        public TabIndicatorStyle IndicatorStyle { get; set; } = TabIndicatorStyle.UnderlineSimple;

        /// <summary>Gets or sets the header height in pixels</summary>
        public int Height { get; set; } = 60;

        /// <summary>Gets or sets the logo width in pixels</summary>
        public int LogoWidth { get; set; } = 40;

        /// <summary>Gets or sets the padding between elements in pixels</summary>
        public int Padding { get; set; } = 10;

        /// <summary>Gets or sets the spacing between tabs in pixels</summary>
        public int TabSpacing { get; set; } = 5;

        /// <summary>Gets or sets whether to show the search box</summary>
        public bool ShowSearchBox { get; set; } = true;

        /// <summary>Gets or sets whether to show logo</summary>
        public bool ShowLogo { get; set; } = true;

        /// <summary>Gets or sets the tab indicator thickness in pixels</summary>
        public int IndicatorThickness { get; set; } = 3;

        /// <summary>Gets or sets whether tabs have icons</summary>
        public bool ShowTabIcons { get; set; } = false;

        /// <summary>Gets or sets the tab text font size</summary>
        public int TabFontSize { get; set; } = 12;

        /// <summary>Gets or sets the action button font size</summary>
        public int ActionButtonFontSize { get; set; } = 11;
    }
}
