using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers
{
    /// <summary>
    /// Interface for widget painters following the same pattern as ICardPainter
    /// </summary>
    internal interface IWidgetPainter
    {
        void Initialize(BaseControl owner, IBeepTheme theme);
        WidgetContext AdjustLayout(Rectangle drawingRect, WidgetContext ctx);
        void DrawBackground(Graphics g, WidgetContext ctx);
        void DrawContent(Graphics g, WidgetContext ctx);
        void DrawForegroundAccents(Graphics g, WidgetContext ctx);
        void UpdateHitAreas(BaseControl owner, WidgetContext ctx, Action<string, Rectangle> notifyAreaHit);
    }

    /// <summary>
    /// Layout and data context for widget rendering
    /// </summary>
    internal sealed class WidgetContext
    {
        // Layout rectangles
        public Rectangle DrawingRect;
        public Rectangle ContentRect;
        public Rectangle HeaderRect;
        public Rectangle ValueRect;
        public Rectangle TrendRect;
        public Rectangle IconRect;
        public Rectangle ChartRect;
        public Rectangle LegendRect;
        public Rectangle FooterRect;
        public Rectangle SubHeaderRect { get; internal set; }
        public Rectangle AvatarRect { get; internal set; }
        
        // Display flags
        public bool ShowHeader;
        public bool ShowIcon;
        public bool ShowTrend;
        public bool ShowLegend;
        public bool ShowFooter;
        public bool ShowStatus { get; internal set; }
        
        // Styling properties
        public Color AccentColor;
        public Color TrendColor;
        public int CornerRadius;
        
        // Data properties
        public object? DataSource;
        public string? Title;
        public string? Value;
        public string? TrendValue;
        public string? TrendDirection; // "up", "down", "neutral"
        public double TrendPercentage;
        public string? Units;
        public string? Subtitle;
        public List<string> Labels = new List<string>();
        public List<double> Values = new List<double>();
        public List<Color> Colors = new List<Color>();
        
        // Interactive properties
        public bool IsInteractive;
        public List<string> CustomImagePaths = new List<string>();
        public string ImagePath { get;  set; }
        public string? IconPath { get; internal set; }

        // ===========================================
        // NAVIGATION PROPERTIES
        // ===========================================
        public List<object> NavigationItems { get; set; } = new List<object>(); // Can be List<string> or List<NavigationItem>
        public List<(string, List<string>)>? NavigationGroups { get; set; }
        public List<object> QuickActions { get; set; } = new List<object>();
        public List<object> ProcessFlowItems { get; set; } = new List<object>();
        public int ActiveIndex { get; set; }
        public int CurrentIndex { get; set; }
        public int ActiveProcessIndex { get; set; } = 1;
        public string? SidebarSelectionId { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 10;
        public bool ShowPageInfo { get; set; }
        public bool ShowTabSeparators { get; set; }

        // ===========================================
        // SOCIAL/COMMUNICATION PROPERTIES
        // ===========================================
        public List<object> ChatMessages { get; set; } = new List<object>(); // List<ChatMessage> or List<Dictionary<string, object>>
        public int MessageCount { get; set; }
        public int UnreadCount { get; set; }
        public bool IsTyping { get; set; }
        public List<object> ChatParticipants { get; set; } = new List<object>();
        public string CurrentUserId { get; set; } = "current_user";
        public string InputText { get; set; } = "";
        public List<object> Comments { get; set; } = new List<object>();
        public string CommentText { get; set; } = "";
        public bool HasNewComments { get; set; }
        public List<object> ActivityItems { get; set; } = new List<object>();
        public string UserName { get; set; } = "User";
        public string UserRole { get; set; } = "Role";
        public string UserStatus { get; set; } = "Offline";
        public object? ContactInfo { get; set; }
        public List<object> SocialItems { get; set; } = new List<object>();
        public int OnlineCount { get; set; }
        public int TotalCount { get; set; }
        public Image? AvatarImage { get; set; }
        public string? AvatarImagePath { get; set; }
        public Color StatusColor { get; set; } = Color.Gray;
        public bool IsVerticalLayout { get; set; }

        // ===========================================
        // NOTIFICATION/ALERT PROPERTIES
        // ===========================================
        public string Message { get; set; } = "";
        public string Timestamp { get; set; } = "";
        public string NotificationType { get; set; } = "info"; // "info", "warning", "error", "success"
        public string ValidationType { get; set; } = "error";
        public string StatusType { get; set; } = "info";
        public bool IsDismissible { get; set; }
        public bool IsCollapsed { get; set; }
        public bool DisableCopy { get; set; }
        public bool HideAction { get; set; }
        public bool ShowProgress { get; set; }
        public float Progress { get; set; }
        public List<object>? NotificationMessages { get; set; } // List of notification message objects

        // ===========================================
        // MEDIA/DISPLAY PROPERTIES
        // ===========================================
        public Image? Image { get; set; }
        public bool ShowImageOverlay { get; set; }
        public bool ShowBadge { get; set; }
        public string OverlayText { get; set; } = "";
        public List<object>? MediaItems { get; set; } // List of media items (avatars, images, etc.)

        // ===========================================
        // LOCATION/MAP PROPERTIES
        // ===========================================
        public string Address { get; set; } = "Unknown Location";
        public string City { get; set; } = "";
        public string Region { get; set; } = "";
        public string Country { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public List<object> MapLocations { get; set; } = new List<object>();
        public List<object>? MapRoutes { get; set; } // List of route objects
        public Color RouteColor { get; set; } = Color.Blue;

        // ===========================================
        // METRIC PROPERTIES
        // ===========================================
        public string MetricType { get; set; } = "";
        public bool ShowPercentage { get; set; }

        // ===========================================
        // LIST/ITEM PROPERTIES
        // ===========================================
        public List<Dictionary<string, object>> ListItems { get; set; } = new List<Dictionary<string, object>>();
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public int SortColumnIndex { get; set; } = -1;
        public string SortDirection { get; set; } = "";
        public string EmptyText { get; set; } = "No data to display";
        public Dictionary<int, string>? StatusOverrides { get; set; } // Status override mapping

        // ===========================================
        // FINANCE PROPERTIES
        // ===========================================
        public decimal? PrimaryValue { get; set; }
        public decimal? SecondaryValue { get; set; }
        public string CurrencySymbol { get; set; } = "$";
        public decimal? Percentage { get; set; }
        public string AccountNumber { get; set; } = "";
        public string AccountType { get; set; } = "";
        public Color ValidColor { get; set; } = Color.Green;
        public Color ErrorColor { get; set; } = Color.Red;
        public Color WarningColor { get; set; } = Color.Orange;
        public Color PositiveColor { get; set; } = Color.Green;
        public Color NegativeColor { get; set; } = Color.Red;
        public Color NeutralColor { get; set; } = Color.Orange;
        public string CryptoName { get; set; } = "";
        public string CryptoSymbol { get; set; } = "";
        public Color CryptoColor { get; set; } = Color.FromArgb(255, 193, 7);
        public List<object>? FinanceItems { get; set; }
        public bool ShowCurrency { get; set; } = true;
        public string Trend { get; set; } = "";
        public string Merchant { get; set; } = "";
        public string Category { get; set; } = "";
        public decimal? Amount { get; set; }
        public string Currency { get; set; } = "$";
        public DateTime? Date { get; set; }
        public bool ShowBalanceHistory { get; set; }
        public bool ShowCopiedMessage { get; set; }
        public bool ShowCryptoInfo { get; set; }
        public bool ShowCryptoDetails { get; set; }
        public bool ShowOrderBook { get; set; }
        public bool ShowChangeHistory { get; set; }
        public bool ShowPriceChart { get; set; }
        public bool ToggleProgressView { get; set; }
        public bool ShowSpentDetails { get; set; }
        public bool ShowRemainingDetails { get; set; }
        public bool ShowVarianceAnalysis { get; set; }
        public bool ShowStatusInfo { get; set; }
        public bool ShowExpenseDetails { get; set; }
        public bool ShowExpensesList { get; set; }
        public bool ShowIconInfo { get; set; }

        // ===========================================
        // INTERACTION/STATE PROPERTIES (Event Results)
        // ===========================================
        public bool HeaderIconClicked { get; set; }
        public bool HeaderClicked { get; set; }
        public int SelectedMessageIndex { get; set; } = -1;
        public bool SelectedMessageIsMine { get; set; }
        public bool InputFocused { get; set; }
        public bool SendClicked { get; set; }
        public bool BannerClicked { get; set; }
        public bool IconClicked { get; set; }
        public bool Dismissed { get; set; }
        public bool SuccessDismissed { get; set; }
        public bool StatusCardClicked { get; set; }
        public bool StatusCardDismissed { get; set; }
        public bool ValidationDismissed { get; set; }
        public object? ValidationCopyRequested { get; set; }
        public bool BreadcrumbHomeClicked { get; set; }
        public int BreadcrumbIndex { get; set; } = -1;
        public int SelectedRankIndex { get; set; } = -1;
        public int SelectedProfileAvatarIndex { get; set; } = -1;
        public int SelectedProfileNameIndex { get; set; } = -1;
        public bool IconCardClicked { get; set; }
        public bool IconCardBadgeClicked { get; set; }
        public bool ImageClicked { get; set; }
        public bool OverlayClicked { get; set; }
        public int SelectedIconIndex { get; set; } = -1;
        public int SelectedAvatarItemIndex { get; set; } = -1;
        public int SelectedAvatarIndex { get; set; } = -1;
        public int SelectedStatusIndex { get; set; } = -1;
        public bool OverlayClosed { get; set; }
        public string OverlayAction { get; set; } = "";
        public int SelectedMediaIndex { get; set; } = -1;
        public int SelectedStatusRowIndex { get; set; } = -1;
        public int ToggledStatusRowIndex { get; set; } = -1;
        public string ToggledStatusNewValue { get; set; } = "";
        public int SelectedRowIndex { get; set; } = -1;
        public int SelectedTaskIndex { get; set; } = -1;
        public int ToggleTaskIndex { get; set; } = -1;
        public int MaxVisibleItems { get; set; } = 10;
        public int SelectedActivityIndex { get; set; } = -1;

        // Note: We don't need ClickableAreas here because BaseControl handles hit areas
        // through its AddHitArea(), ClearHitList(), HitTest(), etc. methods
        // Painters will use owner.AddHitArea() in UpdateHitAreas() method
    }
}