using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Widgets.Models;

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

        // NAVIGATION PROPERTIES
        public List<NavigationItem> NavigationItems { get; set; } = new List<NavigationItem>();
        
        public List<(string, List<string>)>? NavigationGroups { get; set; }
        public List<QuickAction> QuickActions { get; set; } = new List<QuickAction>();
        
        public List<ProcessFlowStep> ProcessFlowItems { get; set; } = new List<ProcessFlowStep>();
        public int ActiveIndex { get; set; }
        public int CurrentIndex { get; set; }
        public int ActiveProcessIndex { get; set; } = 1;
        public string? SidebarSelectionId { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 10;
        public bool ShowPageInfo { get; set; }
        public bool ShowTabSeparators { get; set; }

        // SOCIAL/COMMUNICATION PROPERTIES
        public List<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
        public int MessageCount { get; set; }
        public int UnreadCount { get; set; }
        public bool IsTyping { get; set; }
        public List<ChatParticipant> ChatParticipants { get; set; } = new List<ChatParticipant>();
        public string CurrentUserId { get; set; } = "current_user";
        public string InputText { get; set; } = "";
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public string CommentText { get; set; } = "";
        public bool HasNewComments { get; set; }
        public List<ActivityItem> ActivityItems { get; set; } = new List<ActivityItem>();
        public string UserName { get; set; } = "User";
        public string UserRole { get; set; } = "Role";
        public string UserStatus { get; set; } = "Offline";
        public ContactInfo? ContactInfo { get; set; }
        public List<TheTechIdea.Beep.Winform.Controls.Widgets.SocialItem> SocialItems { get; set; } = new List<TheTechIdea.Beep.Winform.Controls.Widgets.SocialItem>();
        public int OnlineCount { get; set; }
        public int TotalCount { get; set; }
        public Image? AvatarImage { get; set; }
        public string? AvatarImagePath { get; set; }
        public Color StatusColor { get; set; } = Color.Gray;
        public bool IsVerticalLayout { get; set; }

        // NOTIFICATION/ALERT PROPERTIES
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
        public List<NotificationMessage> NotificationMessages { get; set; } = new List<NotificationMessage>();

        // MEDIA/DISPLAY PROPERTIES
        public Image? Image { get; set; }
        public bool ShowImageOverlay { get; set; }
        public bool ShowBadge { get; set; }
        public string OverlayText { get; set; } = "";
        public List<MediaItem> MediaItems { get; set; } = new List<MediaItem>();

        // LOCATION/MAP PROPERTIES
        public string Address { get; set; } = "Unknown Location";
        public string City { get; set; } = "";
        public string Region { get; set; } = "";
        public string Country { get; set; } = "";
        public string PostalCode { get; set; } = "";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public List<MapMarker> MapMarkers { get; set; } = new List<MapMarker>();
        public List<MapRoute> MapRoutes { get; set; } = new List<MapRoute>();
        public Color RouteColor { get; set; } = Color.Blue;

        // METRIC PROPERTIES
        public string MetricType { get; set; } = "";
        public bool ShowPercentage { get; set; }

        // LIST/ITEM PROPERTIES
        public List<ListItem> ListItems { get; set; } = new List<ListItem>();
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public int SortColumnIndex { get; set; } = -1;
        public string SortDirection { get; set; } = "";
        public string EmptyText { get; set; } = "No data to display";
        public List<StatusOverride> StatusOverrides { get; set; } = new List<StatusOverride>();

        // FINANCE PROPERTIES
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
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public List<PortfolioItem> PortfolioItems { get; set; } = new List<PortfolioItem>();
        public bool ShowCurrency { get; set; } = true;
        public string Trend { get; set; } = "";
        public string Merchant { get; set; } = "";
        public string Category { get; set; } = "";
        public decimal? Amount { get; set; }
        public string Currency { get; set; } = "$";
        public DateTime? Date { get; set; }
        public string Status { get; set; } = "";
        public string FilterByCategory { get; set; } = "";
        public bool ShowTransactionDetails { get; set; }
        public bool ShowFullDetails { get; set; }
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
        public bool IsPortfolio { get; set; }
        public decimal? CurrentValue { get; set; }
        public decimal? InitialValue { get; set; }
        public bool ShowInvestmentDetails { get; set; }
        public string ChartPeriod { get; set; } = "1M";
        public bool ShowValueHistory { get; set; }
        public bool ShowPerformanceAnalytics { get; set; }
        public bool ShowPortfolioBreakdown { get; set; }
        public string PaymentMethod { get; set; } = "";
        public string CardNumber { get; set; } = "";
        public string ExpiryDate { get; set; } = "";
        public decimal? Balance { get; set; }
        public bool ShowFullNumber { get; set; }
        public string BalanceView { get; set; } = "Current";
        public bool ShowRenewalOptions { get; set; }
        public bool ShowPortfolioDetails { get; set; }
        public bool ShowPerformance { get; set; }
        public bool ShowRevenueDetails { get; set; }
        public bool ShowTransactionsList { get; set; }
        public bool ShowTrendChart { get; set; }

        // Media-specific additional properties
        public int SelectedPhotoIndex { get; set; } = -1;
        public string MediaViewerAction { get; set; } = "play";
        public bool ShowMediaInfo { get; set; }
        public bool Fullscreen { get; set; }

        // Form-specific properties
        public List<FormField>? Fields { get; set; }
        public List<ValidationResult>? ValidationResults { get; set; }
        public bool ShowValidation { get; set; }
        public bool ShowRequired { get; set; }
        public int CurrentStep { get; set; }
        public int TotalSteps { get; set; }
        public int SelectedFieldIndex { get; set; } = -1;
        public bool LegendClicked { get; set; }
        public bool ContentClicked { get; set; }
        public string[]? InlineFields { get; set; }
        public List<InlineFieldLayout> InlineFieldLayouts { get; set; } = new List<InlineFieldLayout>();
        public string? Description { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsFocused { get; set; }
        public bool HeaderLabelClicked { get; set; }
        public bool InputClicked { get; set; }
        public bool ValidationIconClicked { get; set; }
        public bool FooterClicked { get; set; }
        public bool LabelClicked { get; set; }
        public bool ValidationClicked { get; set; }
        public bool ValidationHeaderClicked { get; set; }
        public bool ValidationMessagesClicked { get; set; }
        public bool ValidationFieldsClicked { get; set; }

        // Navigation Additional properties
        public int ActiveTabIndex { get; set; } = -1;
        public List<TreeNodeItem>? TreeItems { get; set; }

        // INTERACTION/STATE PROPERTIES (Event Results)
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
        public string? ValidationCopyRequested { get; set; }
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

        // Control category properties
        public double MinValue { get; set; } = 0;
        public double MaxValue { get; set; } = 100;
        public Color SelectedColor { get; set; } = Color.Empty;
        public string SelectedButton { get; set; } = "";
        public int SelectedButtonIndex { get; set; } = -1;
        public bool ShowButtonGroupOptions { get; set; }
        public List<bool> CheckedItems { get; set; } = new List<bool>();

        // Calendar category properties
        public DateTime SelectedDate { get; set; } = DateTime.Today;
        public DateTime DisplayMonth { get; set; } = DateTime.Now;
        public List<CalendarEvent> Events { get; set; } = new List<CalendarEvent>();
        
        public List<CalendarEvent> ScheduleEvents { get; set; } = new List<CalendarEvent>();
        public int Completed { get; set; }
        public int Total { get; set; }
        public Color EventColor { get; set; } = Color.Blue;
        public Color TodayColor { get; set; } = Color.Red;
        public bool ShowToday { get; set; } = true;
        public bool ShowEvents { get; set; } = true;
        public DateTime ScheduleDate { get; set; } = DateTime.Today;
        public int SelectedScheduleIndex { get; set; } = -1;
        public bool StatusClicked { get; set; }
        public int SelectedEventIndex { get; set; } = -1;
        public bool EventListClicked { get; set; }
        public string FooterText { get; set; } = "";
        public bool OpenDatePicker { get; set; }
        public bool OpenDatePickerFromIcon { get; set; }
        public bool PrevMonth { get; set; }
        public bool NextMonth { get; set; }
        public bool CalendarGridClicked { get; set; }
        public List<TheTechIdea.Beep.Winform.Controls.Widgets.TimeSlot> TimeSlots { get; set; } = new List<TheTechIdea.Beep.Winform.Controls.Widgets.TimeSlot>();
        public List<CalendarEvent> WeekEvents { get; set; } = new List<CalendarEvent>();
        public string SelectedDayHeader { get; set; } = "";
        public string SelectedEventDay { get; set; } = "";
        public bool WeekGridClicked { get; set; }
        public string SelectedDay { get; set; } = "";
        public string SelectedSlot { get; set; } = "";
        public string HeaderDayClicked { get; set; } = "";
        public bool GridClicked { get; set; }

        // Chart category properties
        public bool PieChartClicked { get; set; }
        public bool LineChartClicked { get; set; }
        public bool HeatmapChartClicked { get; set; }
        public int SelectedCellIndex { get; set; } = -1;
        public bool GaugeClicked { get; set; }
        public bool CombinationChartClicked { get; set; }
        public bool BarChartClicked { get; set; }

        // Dashboard category properties
        public List<DashboardMetric> Metrics { get; set; } = new List<DashboardMetric>();
        public int Columns { get; set; } = 2;
        public int Rows { get; set; } = 2;
        public bool StatusHeaderClicked { get; set; }
        public int SelectedMetricIndex { get; set; } = -1;
        public bool TitleClicked { get; set; }
        public bool LeftPanelClicked { get; set; }
        public bool RightPanelClicked { get; set; }
        public bool VsClicked { get; set; }
        public bool IsExpandable { get; set; }
        public bool ExpandRequested { get; set; }
        public string TitleIcon { get; set; } = "";
        public bool ChartClicked { get; set; }

        // List widget properties
        public int SelectedIndex { get; set; } = -1;

        // Media widget properties
        public bool ShowOverlay { get; set; }

        // Navigation widget properties
        public bool IsHorizontal { get; set; } = true;

        // Notification widget properties
        public string ActionText { get; set; } = "";
        public bool ShowAction { get; set; }

        // Social widget properties
        public bool ShowAvatar { get; set; } = true;

        // Note: We don't need ClickableAreas here because BaseControl handles hit areas
        // through its AddHitArea(), ClearHitList(), HitTest(), etc. methods
        // Painters will use owner.AddHitArea() in UpdateHitAreas() method
    }
}
