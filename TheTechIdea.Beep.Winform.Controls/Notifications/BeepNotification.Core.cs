using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Forms.ModernForm;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    /// <summary>
    /// Visual notification form based on BeepiFormPro
    /// Displays toast/notification with icon, title, message, action buttons, and progress bar
    /// Enhanced with helper classes and painter system
    /// </summary>
    public partial class BeepNotification : BeepiFormPro
    {
        #region Private Fields
        private NotificationData _notificationData;
        private Timer _autoDismissTimer;
        private Timer _progressTimer;
        private float _progressPercentage = 100f;
        private bool _isPaused = false;
        private DateTime _startTime;
        private int _remainingDuration;
        private string _iconPath;
        private Color _iconTint = Color.Gray;
        private Rectangle _iconRect;
        private Rectangle _titleRect;
        private Rectangle _messageRect;
        private Rectangle _closeButtonRect;
        private Rectangle _progressBarRect;
        private Rectangle _actionsRect;
        private const int ICON_SIZE = 24;
        private const int CLOSE_BUTTON_SIZE = 20;
        private const int PROGRESS_BAR_HEIGHT = 4;
        private const int ACTION_BUTTON_HEIGHT = 32;
        private const int PADDING = 12;

        // Painter system
        private INotificationPainter _painter;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of BeepNotification
        /// </summary>
        public BeepNotification()
        {
            _notificationData = new NotificationData();
            
            // Initialize timers
            _autoDismissTimer = new Timer { Interval = 100 };
            _autoDismissTimer.Tick += AutoDismissTimer_Tick;
            
            _progressTimer = new Timer { Interval = 50 };
            _progressTimer.Tick += ProgressTimer_Tick;

            // BeepiFormPro configuration
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            ShowCaptionBar = false;
            FormStyle = BeepThemesManager.CurrentStyle;
            
            // Size configuration
            MinimumSize = new Size(280, 60);
            MaximumSize = new Size(420, 300);
            Size = new Size(350, 80);
            
            // Enable opacity for fade animations
            Opacity = 1.0;

            // Mouse events for pause on hover
            MouseEnter += BeepNotification_MouseEnter;
            MouseLeave += BeepNotification_MouseLeave;
           
            // Keyboard support
            this.KeyPreview = true;
            this.TabStop = true;
            this.KeyDown += BeepNotification_KeyDown;

            // Initialize painter
            InitializePainter();
        }

        private void InitializePainter()
        {
            _painter = NotificationPainterFactory.CreatePainter(_notificationData?.Layout ?? NotificationLayout.Standard);
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Notification data model
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NotificationData NotificationData
        {
            get => _notificationData;
            set
            {
                _notificationData = value ?? new NotificationData();
                InitializePainter(); // Update painter when layout changes
                UpdateNotificationVisuals();
                Invalidate();
            }
        }

        /// <summary>
        /// Notification title
        /// </summary>
        [Category("Appearance")]
        [Description("The title text of the notification")]
        public string Title
        {
            get => _notificationData?.Title;
            set
            {
                if (_notificationData != null)
                {
                    _notificationData.Title = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Notification message
        /// </summary>
        [Category("Appearance")]
        [Description("The message text of the notification")]
        public string Message
        {
            get => _notificationData?.Message;
            set
            {
                if (_notificationData != null)
                {
                    _notificationData.Message = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Notification type
        /// </summary>
        [Category("Appearance")]
        [Description("The type of notification (Info, Success, Warning, Error, etc.)")]
        [DefaultValue(NotificationType.Info)]
        public NotificationType NotificationType
        {
            get => _notificationData?.Type ?? NotificationType.Info;
            set
            {
                if (_notificationData != null)
                {
                    _notificationData.Type = value;
                    UpdateNotificationVisuals();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Notification layout style
        /// </summary>
        [Category("Appearance")]
        [Description("The layout style of the notification")]
        [DefaultValue(NotificationLayout.Standard)]
        public NotificationLayout LayoutStyle
        {
            get => _notificationData?.Layout ?? NotificationLayout.Standard;
            set
            {
                if (_notificationData != null)
                {
                    _notificationData.Layout = value;
                    InitializePainter(); // Update painter when layout changes
                    RecalculateLayout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Auto-dismiss duration in milliseconds (0 = no auto-dismiss)
        /// </summary>
        [Category("Behavior")]
        [Description("Duration before auto-dismiss in milliseconds (0 = no auto-dismiss)")]
        [DefaultValue(5000)]
        public int Duration
        {
            get => _notificationData?.Duration ?? 5000;
            set
            {
                if (_notificationData != null)
                {
                    _notificationData.Duration = value;
                }
            }
        }

        /// <summary>
        /// Show close button
        /// </summary>
        [Category("Appearance")]
        [Description("Show close button")]
        [DefaultValue(true)]
        public new bool ShowCloseButton
        {
            get => _notificationData?.ShowCloseButton ?? true;
            set
            {
                if (_notificationData != null)
                {
                    _notificationData.ShowCloseButton = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Show progress bar
        /// </summary>
        [Category("Appearance")]
        [Description("Show progress bar for auto-dismiss countdown")]
        [DefaultValue(true)]
        public bool ShowProgressBar
        {
            get => _notificationData?.ShowProgressBar ?? true;
            set
            {
                if (_notificationData != null)
                {
                    _notificationData.ShowProgressBar = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Events

        /// <summary>
        /// Raised when the notification is dismissed
        /// </summary>
        public event EventHandler<NotificationEventArgs> NotificationDismissed;

        /// <summary>
        /// Raised when an action button is clicked
        /// </summary>
        public event EventHandler<NotificationEventArgs> ActionClicked;

        /// <summary>
        /// Raised when the notification is clicked (not action button)
        /// </summary>
        public event EventHandler<NotificationEventArgs> NotificationClicked;
        #endregion
    }
}
