using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Widgets.Helpers;
using TheTechIdea.Beep.Editor;

namespace TheTechIdea.Beep.Winform.Controls.Widgets
{
    public enum NotificationWidgetStyle
    {
        ToastNotification,    // Pop-up toast messages
        AlertBanner,          // Banner-style alerts
        ProgressAlert,        // Progress with message
        StatusCard,           // Status card with icon
        MessageCenter,        // Message center widget
        SystemAlert,          // System status alerts
        ValidationMessage,    // Form validation messages
        InfoPanel,            // Information panel
        WarningBadge,         // Warning badge/indicator
        SuccessBanner         // Success confirmation banner
    }

    public enum NotificationType
    {
        Info,
        Success, 
        Warning,
        Error,
        Progress
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Notification Widget")]
    [Category("Beep Widgets")]
    [Description("Notification and alert widget with multiple display styles.")]
    public class BeepNotificationWidget : BaseControl
    {
        #region Fields
        private NotificationWidgetStyle _style = NotificationWidgetStyle.ToastNotification;
        private NotificationType _notificationType = NotificationType.Info;
        private IWidgetPainter _painter;
        private string _title = "Notification";
        private string _message = "This is a notification message";
        private string _actionText = "Action";
        private bool _isDismissible = true;
        private bool _showIcon = true;
        private bool _showAction = false;
        private int _progress = 0;
        private Color _accentColor = Color.FromArgb(33, 150, 243);
        private Color _successColor = Color.FromArgb(76, 175, 80);
        private Color _warningColor = Color.FromArgb(255, 193, 7);
        private Color _errorColor = Color.FromArgb(244, 67, 54);
        private Color _infoColor = Color.FromArgb(33, 150, 243);
        private Color _cardBackColor = Color.White;
        private Color _cardTitleForeColor = Color.Black;
        private Color _cardTextForeColor = Color.FromArgb(100, 100, 100);
        private Color _borderColor = Color.FromArgb(200, 200, 200);
        private Color _surfaceColor = Color.FromArgb(250, 250, 250);
        private DateTime _timestamp = DateTime.Now;

        // Events
        public event EventHandler<BeepEventDataArgs> NotificationClicked;
        public event EventHandler<BeepEventDataArgs> ActionClicked;
        public event EventHandler<BeepEventDataArgs> DismissClicked;
        #endregion

        #region Constructor
        public BeepNotificationWidget() : base()
        {
            IsChild = false;
            Padding = new Padding(5);
            this.Size = new Size(350, 80);
            ApplyThemeToChilds = false;
            ApplyTheme();
            CanBeHovered = true;
            InitializePainter();
        }

        private void InitializePainter()
        {
            switch (_style)
            {
                case NotificationWidgetStyle.ToastNotification:
                    _painter = new ToastNotificationPainter();
                    break;
                case NotificationWidgetStyle.AlertBanner:
                    _painter = new AlertBannerPainter();
                    break;
                case NotificationWidgetStyle.ProgressAlert:
                    _painter = new ProgressAlertPainter();
                    break;
                case NotificationWidgetStyle.StatusCard:
                    _painter = new StatusCardPainter();
                    break;
                case NotificationWidgetStyle.MessageCenter:
                    _painter = new MessageCenterPainter();
                    break;
                case NotificationWidgetStyle.SystemAlert:
                    _painter = new SystemAlertPainter();
                    break;
                case NotificationWidgetStyle.ValidationMessage:
                    _painter = new ValidationMessagePainter();
                    break;
                case NotificationWidgetStyle.InfoPanel:
                    _painter = new InfoPanelPainter();
                    break;
                case NotificationWidgetStyle.WarningBadge:
                    _painter = new WarningBadgePainter();
                    break;
                case NotificationWidgetStyle.SuccessBanner:
                    _painter = new SuccessBannerPainter();
                    break;
                default:
                    _painter = new ToastNotificationPainter();
                    break;
            }
            _painter?.Initialize(this, _currentTheme);
        }
        #endregion

        #region Properties
        [Category("Notification")]
        [Description("Visual style of the notification widget.")]
        public NotificationWidgetStyle Style
        {
            get => _style;
            set { _style = value; InitializePainter(); Invalidate(); }
        }

        [Category("Notification")]
        [Description("Type of notification (Info, Success, Warning, Error, Progress).")]
        public NotificationType NotificationType
        {
            get => _notificationType;
            set { _notificationType = value; UpdateAccentColor(); Invalidate(); }
        }

        [Category("Notification")]
        [Description("Title of the notification.")]
        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        [Category("Notification")]
        [Description("Message content of the notification.")]
        public string Message
        {
            get => _message;
            set { _message = value; Invalidate(); }
        }

        [Category("Notification")]
        [Description("Text for action button.")]
        public string ActionText
        {
            get => _actionText;
            set { _actionText = value; Invalidate(); }
        }

        [Category("Notification")]
        [Description("Whether the notification can be dismissed.")]
        public bool IsDismissible
        {
            get => _isDismissible;
            set { _isDismissible = value; Invalidate(); }
        }

        [Category("Notification")]
        [Description("Whether to show the notification icon.")]
        public bool ShowIcon
        {
            get => _showIcon;
            set { _showIcon = value; Invalidate(); }
        }

        [Category("Notification")]
        [Description("Whether to show an action button.")]
        public bool ShowAction
        {
            get => _showAction;
            set { _showAction = value; Invalidate(); }
        }

        [Category("Notification")]
        [Description("Progress value (0-100) for progress notifications.")]
        public int Progress
        {
            get => _progress;
            set { _progress = Math.Max(0, Math.Min(100, value)); Invalidate(); }
        }

        [Category("Appearance")]
        [Description("Primary accent color for the notification.")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        [Category("Notification")]
        [Description("Timestamp of the notification.")]
        public DateTime Timestamp
        {
            get => _timestamp;
            set { _timestamp = value; Invalidate(); }
        }
        #endregion

        #region Drawing
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            var ctx = new WidgetContext
            {
                DrawingRect = DrawingRect,
                Title = _title,
                Value = _message,
                AccentColor = _accentColor,
                ShowIcon = _showIcon,
                IsInteractive = true,
                CornerRadius = BorderRadius,
                TrendPercentage = _progress,
                CustomData = new Dictionary<string, object>
                {
                    ["NotificationType"] = _notificationType,
                    ["ActionText"] = _actionText,
                    ["IsDismissible"] = _isDismissible,
                    ["ShowAction"] = _showAction,
                    ["Timestamp"] = _timestamp
                }
            };

            _painter?.Initialize(this, _currentTheme);
            ctx = _painter?.AdjustLayout(DrawingRect, ctx) ?? ctx;

            _painter?.DrawBackground(g, ctx);
            _painter?.DrawContent(g, ctx);
            _painter?.DrawForegroundAccents(g, ctx);

            RefreshHitAreas(ctx);
            _painter?.UpdateHitAreas(this, ctx, (name, rect) => { });
        }

        private void RefreshHitAreas(WidgetContext ctx)
        {
            ClearHitList();

            if (!ctx.ContentRect.IsEmpty)
            {
                AddHitArea("Notification", ctx.ContentRect, null, () =>
                {
                    NotificationClicked?.Invoke(this, new BeepEventDataArgs("NotificationClicked", this));
                });
            }

            if (_showAction && !ctx.FooterRect.IsEmpty)
            {
                AddHitArea("Action", ctx.FooterRect, null, () =>
                {
                    ActionClicked?.Invoke(this, new BeepEventDataArgs("ActionClicked", this));
                });
            }

            if (_isDismissible && !ctx.IconRect.IsEmpty)
            {
                AddHitArea("Dismiss", ctx.IconRect, null, () =>
                {
                    DismissClicked?.Invoke(this, new BeepEventDataArgs("DismissClicked", this));
                });
            }
        }

        private void UpdateAccentColor()
        {
            _accentColor = _notificationType switch
            {
                NotificationType.Success => Color.FromArgb(76, 175, 80),
                NotificationType.Warning => Color.FromArgb(255, 193, 7),
                NotificationType.Error => Color.FromArgb(244, 67, 54),
                NotificationType.Progress => Color.FromArgb(33, 150, 243),
                _ => Color.FromArgb(96, 125, 139)
            };
        }
        #endregion

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;

            // Apply notification-specific theme colors
            BackColor = _currentTheme.BackColor;
            ForeColor = _currentTheme.ForeColor;
            
            // Update status colors for different notification types
            _successColor = _currentTheme.SuccessColor;
            _warningColor = _currentTheme.WarningColor;
            _errorColor = _currentTheme.ErrorColor;
            _infoColor = _currentTheme.AccentColor;
            
            // Update card colors for notification display
            _cardBackColor = _currentTheme.CardBackColor;
            _cardTitleForeColor = _currentTheme.CardTitleForeColor;
            _cardTextForeColor = _currentTheme.CardTextForeColor;
            
            // Update border and surface colors
            _borderColor = _currentTheme.BorderColor;
            _surfaceColor = _currentTheme.SurfaceColor;
            
            InitializePainter();
            Invalidate();
        }
    }
}