using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    /// <summary>
    /// Singleton notification manager for showing and managing toast/notification messages
    /// Similar to DevExpress, Telerik, and other commercial notification managers
    /// </summary>
    public sealed class BeepNotificationManager
    {
        #region Singleton Pattern
        private static readonly Lazy<BeepNotificationManager> _instance = 
            new Lazy<BeepNotificationManager>(() => new BeepNotificationManager());

        /// <summary>
        /// Gets the singleton instance of BeepNotificationManager
        /// </summary>
        public static BeepNotificationManager Instance => _instance.Value;

        private BeepNotificationManager()
        {
            _activeNotifications = new List<BeepNotification>();
            _notificationQueue = new Queue<NotificationData>();
            _animators = new Dictionary<BeepNotification, BeepNotificationAnimator>();
            _notificationGroups = new Dictionary<string, BeepNotificationGroup>();
        }
        #endregion

        #region Private Fields
        private readonly List<BeepNotification> _activeNotifications;
        private readonly Queue<NotificationData> _notificationQueue;
        private readonly Dictionary<BeepNotification, BeepNotificationAnimator> _animators;
        private readonly Dictionary<string, BeepNotificationGroup> _notificationGroups;
        private BeepNotificationHistory? _historyPanel;
        private NotificationPosition _defaultPosition = NotificationPosition.BottomRight;
        private int _maxVisibleNotifications = 5;
        private int _notificationSpacing = 10;
        private int _screenMargin = 20;
        private Screen? _targetScreen = Screen.PrimaryScreen;
        private NotificationAnimation _defaultAnimation = NotificationAnimation.SlideAndFade;
        private bool _trackHistory = true;
        private bool _enableGrouping = true;
        private int _groupingThreshold = 3; // Min notifications to create a group
        private bool _doNotDisturbMode = false;
        private NotificationPriority _doNotDisturbMinPriority = NotificationPriority.Critical;
        private bool _smartPositioning = true;
        private Form? _anchorForm; // Form to position relative to

        // Phase 4.4 / G23 — protects _activeNotifications, _notificationQueue,
        // _animators, _notificationGroups, _scheduledNotifications, _templates.
        // Callers from any thread may invoke Show(); manager-internal events run
        // on the UI thread but the lock is required to keep list/dict mutations
        // race-free. NEVER call user code or invoke events while holding this lock;
        // snapshot first, release, then act.
        private readonly object _lock = new object();
        #endregion

        #region Public Properties

        /// <summary>
        /// Default position for notifications
        /// </summary>
        public NotificationPosition DefaultPosition
        {
            get => _defaultPosition;
            set => _defaultPosition = value;
        }

        /// <summary>
        /// Maximum number of notifications visible at once
        /// </summary>
        public int MaxVisibleNotifications
        {
            get => _maxVisibleNotifications;
            set => _maxVisibleNotifications = Math.Max(1, value);
        }

        /// <summary>
        /// Spacing between notifications in pixels
        /// </summary>
        public int NotificationSpacing
        {
            get => _notificationSpacing;
            set => _notificationSpacing = Math.Max(0, value);
        }

        /// <summary>
        /// Margin from screen edge in pixels
        /// </summary>
        public int ScreenMargin
        {
            get => _screenMargin;
            set => _screenMargin = Math.Max(0, value);
        }

        /// <summary>
        /// Target screen for notification display
        /// </summary>
        public Screen TargetScreen
        {
            get => _targetScreen ?? Screen.PrimaryScreen!;
            set => _targetScreen = value;
        }

        /// <summary>
        /// Default animation style
        /// </summary>
        public NotificationAnimation DefaultAnimation
        {
            get => _defaultAnimation;
            set => _defaultAnimation = value;
        }

        /// <summary>
        /// Enable or disable all notification sounds globally
        /// </summary>
        public bool SoundEnabled
        {
            get => BeepNotificationSound.SoundEnabled;
            set => BeepNotificationSound.SoundEnabled = value;
        }

        /// <summary>
        /// Enable or disable notification history tracking
        /// </summary>
        public bool TrackHistory
        {
            get => _trackHistory;
            set => _trackHistory = value;
        }

        /// <summary>
        /// Enable or disable automatic grouping of similar notifications
        /// </summary>
        public bool EnableGrouping
        {
            get => _enableGrouping;
            set => _enableGrouping = value;
        }

        /// <summary>
        /// Minimum number of notifications with same GroupKey to create a group
        /// </summary>
        public int GroupingThreshold
        {
            get => _groupingThreshold;
            set => _groupingThreshold = Math.Max(2, value);
        }

        /// <summary>
        /// Enable or disable Do Not Disturb mode
        /// When enabled, only notifications meeting MinimumPriorityInDND are shown
        /// </summary>
        public bool DoNotDisturbMode
        {
            get => _doNotDisturbMode;
            set => _doNotDisturbMode = value;
        }

        /// <summary>
        /// Minimum priority level for notifications to show when in Do Not Disturb mode
        /// Default is Critical (only critical notifications shown)
        /// </summary>
        public NotificationPriority MinimumPriorityInDND
        {
            get => _doNotDisturbMinPriority;
            set => _doNotDisturbMinPriority = value;
        }

        /// <summary>
        /// Enable smart positioning (auto-detect active monitor, avoid covering UI)
        /// </summary>
        public bool SmartPositioning
        {
            get => _smartPositioning;
            set => _smartPositioning = value;
        }

        /// <summary>
        /// Anchor form for relative positioning. If set, notifications appear on the same monitor as this form.
        /// </summary>
        public Form? AnchorForm
        {
            get => _anchorForm;
            set
            {
                _anchorForm = value;
                if (_smartPositioning && _anchorForm != null)
                {
                    // Update target screen to match anchor form's screen
                    _targetScreen = Screen.FromControl(_anchorForm);
                }
            }
        }

        /// <summary>
        /// Get the notification history panel (creates if needed).
        /// Phase 4.7 / G24: when <see cref="HistoryPanelShowOnFirstDisplay"/>
        /// is true, this property surfaces the panel as a top-level form so the
        /// user can actually see it; off by default to keep with classic
        /// toast semantics (history visible only when explicitly opened).
        /// </summary>
        public BeepNotificationHistory HistoryPanel
        {
            get
            {
                if (_historyPanel == null || _historyPanel.IsDisposed)
                {
                    _historyPanel = new BeepNotificationHistory();
                }
                return _historyPanel;
            }
        }

        /// <summary>
        /// Phase 4.7 / G24. When true, calling <see cref="HistoryPanel"/> shows
        /// the panel the first time it is accessed in the session (so users
        /// actually know it exists). Default false.
        /// </summary>
        public bool HistoryPanelShowOnFirstDisplay
        {
            get => _historyPanelShownOnFirstDisplay;
            set
            {
                if (_historyPanelShownOnFirstDisplay == value) return;
                _historyPanelShownOnFirstDisplay = value;
                if (value && _historyPanel != null && !_historyPanel.IsDisposed && !_historyPanel.Visible)
                {
                    _historyPanel.Show();
                }
            }
        }
        private bool _historyPanelShownOnFirstDisplay;

        /// <summary>
        /// Number of active (visible) notifications
        /// </summary>
        public int ActiveCount => _activeNotifications.Count;

        /// <summary>
        /// Number of queued notifications waiting to be shown
        /// </summary>
        public int QueuedCount => _notificationQueue.Count;
        #endregion

        #region Events

        /// <summary>
        /// Raised when a notification is shown
        /// </summary>
        public event EventHandler<NotificationEventArgs>? NotificationShown;

        /// <summary>
        /// Raised when a notification is dismissed
        /// </summary>
        public event EventHandler<NotificationEventArgs>? NotificationDismissed;

        /// <summary>
        /// Raised when a notification action is clicked
        /// </summary>
        public event EventHandler<NotificationEventArgs>? NotificationActionClicked;
        #endregion

        #region Public Methods - Show Notifications

        /// <summary>
        /// Show a simple notification with title and message
        /// </summary>
        public void Show(string title, string message, NotificationType type = NotificationType.Info)
        {
            var data = new NotificationData
            {
                Title = title,
                Message = message,
                Type = type,
                Duration = NotificationData.GetDefaultDuration(NotificationPriority.Normal)
            };
            Show(data);
        }

        /// <summary>
        /// Show a notification with full data model. Thread-safe: protected by
        /// <see cref="_lock"/> because callers may invoke from a worker thread
        /// (Phase 4.4 / G23).
        /// </summary>
        public void Show(NotificationData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // Phase 4.4 / G23 — single lock guards all three bucket decisions
            // (DND, Focus Session, capacity) plus the resulting Enqueue. Critical
            // bypass is honored across all three.
            lock (_lock)
            {
                // Check Do Not Disturb mode
                if (_doNotDisturbMode && data.Priority < _doNotDisturbMinPriority)
                {
                    _notificationQueue.Enqueue(data);
                    return;
                }

                // Phase 5.6 / G27 — Win 11 Focus Session detection.
                if (data.Priority != NotificationPriority.Critical && FocusSessionDetector.IsInFocusSession())
                {
                    _notificationQueue.Enqueue(data);
                    return;
                }

                // If at max capacity, queue it — Critical bypasses the cap.
                if (_activeNotifications.Count >= _maxVisibleNotifications
                    && data.Priority != NotificationPriority.Critical)
                {
                    _notificationQueue.Enqueue(data);
                    return;
                }
            }

            ShowNotificationInternal(data);
        }

        /// <summary>
        /// Show a success notification
        /// </summary>
        public void ShowSuccess(string title, string message, int duration = 5000)
        {
            Show(new NotificationData
            {
                Title = title,
                Message = message,
                Type = NotificationType.Success,
                Duration = duration
            });
        }

        /// <summary>
        /// Show an error notification
        /// </summary>
        public void ShowError(string title, string message, int duration = 0)
        {
            Show(new NotificationData
            {
                Title = title,
                Message = message,
                Type = NotificationType.Error,
                Priority = NotificationPriority.High,
                Duration = duration // 0 = no auto-dismiss for errors
            });
        }

        /// <summary>
        /// Show a warning notification
        /// </summary>
        public void ShowWarning(string title, string message, int duration = 8000)
        {
            Show(new NotificationData
            {
                Title = title,
                Message = message,
                Type = NotificationType.Warning,
                Priority = NotificationPriority.High,
                Duration = duration
            });
        }

        /// <summary>
        /// Show an info notification
        /// </summary>
        public void ShowInfo(string title, string message, int duration = 5000)
        {
            Show(new NotificationData
            {
                Title = title,
                Message = message,
                Type = NotificationType.Info,
                Duration = duration
            });
        }

        /// <summary>
        /// Show a notification with action buttons
        /// </summary>
        public void ShowWithActions(string title, string message, NotificationType type, params NotificationAction[] actions)
        {
            Show(new NotificationData
            {
                Title = title,
                Message = message,
                Type = type,
                Actions = actions,
                Duration = 0 // Don't auto-dismiss when there are actions
            });
        }
        #endregion

        #region Public Methods - Management

        /// <summary>
        /// Dismiss a specific notification
        /// </summary>
        public void Dismiss(BeepNotification notification)
        {
            if (notification == null || !_activeNotifications.Contains(notification))
                return;

            DismissNotificationInternal(notification);
        }

        /// <summary>
        /// Dismiss all active notifications
        /// </summary>
        public void DismissAll()
        {
            var notifications = SnapshotLive();
            foreach (var notification in notifications)
            {
                DismissNotificationInternal(notification);
            }
        }

        /// <summary>
        /// Dismiss all notifications of a specific type
        /// </summary>
        public void DismissAllByType(NotificationType type)
        {
            var notifications = SnapshotLive(n => n.NotificationData?.Type == type);

            foreach (var notification in notifications)
            {
                DismissNotificationInternal(notification);
            }
        }

        /// <summary>
        /// Dismiss all notifications of a specific priority
        /// </summary>
        public void DismissAllByPriority(NotificationPriority priority)
        {
            var notifications = SnapshotLive(n => n.NotificationData?.Priority == priority);

            foreach (var notification in notifications)
            {
                DismissNotificationInternal(notification);
            }
        }

        /// <summary>
        /// Dismiss all notifications except specified priority (useful for keeping critical notifications)
        /// </summary>
        public void DismissAllExceptPriority(NotificationPriority priority)
        {
            var notifications = SnapshotLive(n => n.NotificationData?.Priority != priority);

            foreach (var notification in notifications)
            {
                DismissNotificationInternal(notification);
            }
        }

        /// <summary>
        /// Clear the notification queue without showing. Thread-safe.
        /// </summary>
        public void ClearQueue()
        {
            lock (_lock) _notificationQueue.Clear();
        }

        /// <summary>
        /// Dismiss all and clear queue
        /// </summary>
        public void Clear()
        {
            DismissAll();
            ClearQueue();
        }

        /// <summary>
        /// Mark all active notifications as read
        /// </summary>
        public void MarkAllRead()
        {
            var notifications = SnapshotLive();
            foreach (var notification in notifications)
            {
                if (notification.NotificationData != null && !notification.NotificationData.IsRead)
                {
                    notification.NotificationData.IsRead = true;
                    notification.NotificationData.ReadTimestamp = DateTime.Now;
                    notification.Invalidate();
                }
            }
        }

        /// <summary>
        /// Mark all active notifications as unread
        /// </summary>
        public void MarkAllUnread()
        {
            var notifications = SnapshotLive();
            foreach (var notification in notifications)
            {
                if (notification.NotificationData != null && notification.NotificationData.IsRead)
                {
                    notification.NotificationData.IsRead = false;
                    notification.NotificationData.ReadTimestamp = null;
                    notification.Invalidate();
                }
            }
        }

        /// <summary>
        /// Pin all active notifications (prevent auto-dismiss)
        /// </summary>
        public void PinAll()
        {
            var notifications = SnapshotLive();
            foreach (var notification in notifications)
            {
                if (notification.NotificationData != null)
                {
                    notification.NotificationData.IsPinned = true;
                    notification.Invalidate();
                }
            }
        }

        /// <summary>
        /// Unpin all active notifications
        /// </summary>
        public void UnpinAll()
        {
            var notifications = SnapshotLive();
            foreach (var notification in notifications)
            {
                if (notification.NotificationData != null)
                {
                    notification.NotificationData.IsPinned = false;
                    notification.Invalidate();
                }
            }
        }

        /// <summary>
        /// Dismiss all read notifications
        /// </summary>
        public void DismissAllRead()
        {
            var notifications = SnapshotLive(n => n.NotificationData?.IsRead == true);

            foreach (var notification in notifications)
            {
                DismissNotificationInternal(notification);
            }
        }

        /// <summary>
        /// Dismiss all unread notifications
        /// </summary>
        public void DismissAllUnread()
        {
            var notifications = SnapshotLive(n => n.NotificationData?.IsRead != true);

            foreach (var notification in notifications)
            {
                DismissNotificationInternal(notification);
            }
        }

        /// <summary>
        /// Phase 7.6 (G28) — mark the underlying data of the notification
        /// identified by <paramref name="notificationId"/> as consumed
        /// (no longer relevant to the user). The notification is removed
        /// from the active stack so it won't re-appear, the history entry is
        /// updated to <c>IsRead = true</c>, and (when a Win11 toast bridge
        /// is wired in Phase 8) the corresponding OS toast is dismissed from
        /// the Action Center.
        /// <para>
        /// Safe to call with an unknown id (no-op). Thread-safe — guarded by
        /// <see cref="_lock"/>.
        /// </para>
        /// </summary>
        public void MarkConsumed(string? notificationId)
        {
            if (string.IsNullOrEmpty(notificationId)) return;

            BeepNotification? toRemove = null;
            lock (_lock)
            {
                for (int i = _activeNotifications.Count - 1; i >= 0; i--)
                {
                    var n = _activeNotifications[i];
                    if (n?.NotificationData == null) continue;
                    if (string.Equals(n.NotificationData.Id, notificationId, StringComparison.Ordinal))
                    {
                        // Mark read in history then drop the active toast.
                        n.NotificationData.IsRead = true;
                        n.NotificationData.ReadTimestamp = DateTime.Now;
                        toRemove = n;
                        break;
                    }
                }
            }

            if (toRemove == null) return;

            // Dismiss the captured notification outside the lock — the animator
            // path needs to call back into the manager and re-entering the
            // same lock would deadlock the scheduler/animation loop.
            DismissNotificationInternal(toRemove);

            NotificationConsumed?.Invoke(this, new NotificationEventArgs
            {
                Notification = toRemove.NotificationData
            });
        }

        /// <summary>
        /// Raised when <see cref="MarkConsumed"/> removes a notification from
        /// the active stack. Subscribers (e.g. a future Win11 toast bridge)
        /// can match this event to the corresponding OS-side toast to remove
        /// it from Action Center.
        /// </summary>
        public event EventHandler<NotificationEventArgs>? NotificationConsumed;

        /// <summary>
        /// Get count of unread notifications
        /// </summary>
        public int UnreadCount => _activeNotifications.Count(n => n.NotificationData?.IsRead != true);

        /// <summary>
        /// Get count of pinned notifications
        /// </summary>
        public int PinnedCount => _activeNotifications.Count(n => n.NotificationData?.IsPinned == true);

        #endregion

        #region Notification Templates

        private readonly Dictionary<string, NotificationData> _templates = new Dictionary<string, NotificationData>();

        /// <summary>
        /// Register a notification template for reuse. Thread-safe.
        /// </summary>
        public void RegisterTemplate(string name, NotificationData template)
        {
            if (string.IsNullOrEmpty(name) || template == null)
                throw new ArgumentNullException(string.IsNullOrEmpty(name) ? nameof(name) : nameof(template));

            lock (_lock) _templates[name] = template;
        }

        /// <summary>
        /// Show a notification using a registered template. Thread-safe.
        /// </summary>
        public void ShowFromTemplate(string name, Action<NotificationData> customize = null)
        {
            NotificationData template;
            lock (_lock)
            {
                if (!_templates.TryGetValue(name, out template))
                    throw new KeyNotFoundException($"Notification template '{name}' not found.");
            }

            var data = CloneNotificationData(template);
            customize?.Invoke(data);
            Show(data);
        }

        /// <summary>
        /// Remove a registered template. Thread-safe.
        /// </summary>
        public void RemoveTemplate(string name)
        {
            lock (_lock) _templates.Remove(name);
        }

        /// <summary>
        /// Get all registered template names (snapshot, thread-safe).
        /// </summary>
        public IEnumerable<string> GetTemplateNames()
        {
            lock (_lock) return _templates.Keys.ToList();
        }

        private static NotificationData CloneNotificationData(NotificationData source)
        {
            return new NotificationData
            {
                Title = source.Title,
                Message = source.Message,
                Type = source.Type,
                Priority = source.Priority,
                Layout = source.Layout,
                VisualStyle = source.VisualStyle,
                GroupKey = source.GroupKey,
                Source = source.Source,
                IconPath = source.IconPath,
                IconTint = source.IconTint,
                EmbeddedImagePath = source.EmbeddedImagePath,
                ShowAccentStripe = source.ShowAccentStripe,
                AccentStripeColor = source.AccentStripeColor,
                CornerRadiusOverride = source.CornerRadiusOverride,
                EnableRichText = source.EnableRichText,
                ProgressValue = source.ProgressValue,
                ProgressText = source.ProgressText,
                CustomBackColor = source.CustomBackColor,
                CustomForeColor = source.CustomForeColor,
                Duration = source.Duration,
                ShowCloseButton = source.ShowCloseButton,
                ShowProgressBar = source.ShowProgressBar,
                PauseOnHover = source.PauseOnHover,
                PlaySound = source.PlaySound,
                CustomSoundPath = source.CustomSoundPath,
                Actions = source.Actions?.ToArray(),
                Tag = source.Tag,
                Persistent = source.Persistent,
                IsPinned = source.IsPinned
            };
        }

        #endregion

        #region Notification Scheduling

        private readonly List<ScheduledNotification> _scheduledNotifications = new List<ScheduledNotification>();
        private Timer _schedulerTimer;

        /// <summary>
        /// Schedule a notification to be shown at a specific time
        /// </summary>
        public void ScheduleNotification(NotificationData data, DateTime showTime)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            lock (_lock)
                _scheduledNotifications.Add(new ScheduledNotification
                {
                    Data = data,
                    ShowTime = showTime,
                    IsRecurring = false
                });

            EnsureSchedulerTimerRunning();
        }

        /// <summary>
        /// Schedule a recurring notification
        /// </summary>
        public void ScheduleRecurringNotification(NotificationData data, TimeSpan interval)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            lock (_lock)
                _scheduledNotifications.Add(new ScheduledNotification
                {
                    Data = data,
                    ShowTime = DateTime.Now.Add(interval),
                    Interval = interval,
                    IsRecurring = true
                });

            EnsureSchedulerTimerRunning();
        }

        /// <summary>
        /// Cancel a scheduled notification
        /// </summary>
        public void CancelScheduledNotification(NotificationData data)
        {
            // Match by Id, not reference equality — otherwise multiple identical
            // NotificationData instances get removed in a single call (G7).
            lock (_lock)
                _scheduledNotifications.RemoveAll(s => s.Data != null && data != null && s.Data.Id == data.Id);
        }

        /// <summary>
        /// Cancel all scheduled notifications
        /// </summary>
        public void CancelAllScheduledNotifications()
        {
            lock (_lock) _scheduledNotifications.Clear();
        }

        /// <summary>
        /// Get count of pending scheduled notifications. Thread-safe (Phase 4.4).
        /// </summary>
        public int ScheduledCount
        {
            get { lock (_lock) return _scheduledNotifications.Count; }
        }

        private void EnsureSchedulerTimerRunning()
        {
            if (_schedulerTimer == null)
            {
                _schedulerTimer = new Timer { Interval = 1000 };
                _schedulerTimer.Tick += SchedulerTimer_Tick;
                _schedulerTimer.Start();
            }
            else if (!_schedulerTimer.Enabled)
            {
                _schedulerTimer.Start();
            }
        }

        private void SchedulerTimer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;

            // Snapshot due notifications first; mutations live outside the lock.
            List<ScheduledNotification> dueNotifications;
            bool listEmpty;
            lock (_lock)
            {
                dueNotifications = _scheduledNotifications.Where(s => s.ShowTime <= now).ToList();
                listEmpty = _scheduledNotifications.Count == 0;
            }

            foreach (var scheduled in dueNotifications)
            {
                // Catch-up loop for recurring notifications: if the OS slept or the
                // timer fell behind, fire as many times as we should have until we're
                // back on schedule. Cap the catch-up at one Interval to avoid pile-up
                // after a long sleep (G8).
                if (scheduled.IsRecurring)
                {
                    int catchUps = 0;
                    while (scheduled.ShowTime <= now && catchUps < 1)
                    {
                        Show(scheduled.Data);
                        scheduled.ShowTime = scheduled.ShowTime.Add(scheduled.Interval);
                        catchUps++;
                    }
                    // If we're still behind by more than one interval, skip ahead
                    // instead of piling catches (e.g. laptop woke from sleep).
                    if (scheduled.ShowTime < now - scheduled.Interval)
                    {
                        scheduled.ShowTime = now;
                    }
                }
                else
                {
                    Show(scheduled.Data);
                    lock (_lock) _scheduledNotifications.Remove(scheduled);
                }
            }

            if (listEmpty && _schedulerTimer != null)
            {
                _schedulerTimer.Stop();
            }
        }

        private class ScheduledNotification
        {
            public NotificationData Data { get; set; }
            public DateTime ShowTime { get; set; }
            public TimeSpan Interval { get; set; }
            public bool IsRecurring { get; set; }
        }

        #endregion

        #region Private Methods - Internal Management

        private void ShowNotificationInternal(NotificationData data)
        {
            // Check if we should group this notification
            if (_enableGrouping && !string.IsNullOrEmpty(data.GroupKey))
            {
                if (TryAddToGroup(data))
                {
                    // Notification was added to existing group
                    return;
                }

                // Check if we should create a new group
                var similarCount = _activeNotifications.Count(n =>
                    n.NotificationData?.GroupKey == data.GroupKey ||
                    n.NotificationData?.Source == data.Source);

                if (similarCount >= _groupingThreshold - 1)
                {
                    CreateGroup(data);
                    return;
                }
            }

            // Create notification control
            var notification = new BeepNotification
            {
                NotificationData = data,
                Visible = true
            };

            // Subscribe to events
            notification.NotificationDismissed += Notification_Dismissed;
            notification.ActionClicked += Notification_ActionClicked;
            notification.NotificationClicked += Notification_Clicked;

            // Add to active list (Phase 4.4 / G23 — locked).
            lock (_lock)
            {
                _activeNotifications.Add(notification);
            }

            // Play sound if requested
            if (data.PlaySound)
            {
                BeepNotificationSound.PlaySound(data.Type, data.CustomSoundPath);
            }

            // Get target position
            var targetLocation = CalculateNotificationPosition(notification);

            // Create animator (Phase 4.4 / G23 — registration under lock).
            var animator = new BeepNotificationAnimator();
            lock (_lock) _animators[notification] = animator;

            // Animate in
            var animation = _defaultAnimation;
            animator.AnimateShow(notification, targetLocation, animation, () =>
            {
                notification.BringToFront();
                NotificationShown?.Invoke(this, new NotificationEventArgs { Notification = data });
            });
        }

        private bool TryAddToGroup(NotificationData data)
        {
            var groupKey = data.GroupKey ?? data.Source;
            if (string.IsNullOrEmpty(groupKey))
                return false;

            // Phase 4.4 / G23 — read group under lock; group mutations on the
            // returned BeepNotificationGroup instance are not under our lock
            // because the group owns its own list and is internally serialized.
            BeepNotificationGroup group;
            lock (_lock)
            {
                _notificationGroups.TryGetValue(groupKey, out group);
            }

            if (group != null)
            {
                group.AddNotification(data);

                // Play sound for grouped notification
                if (data.PlaySound)
                {
                    BeepNotificationSound.PlaySound(data.Type, data.CustomSoundPath);
                }
                
                return true;
            }

            return false;
        }

        private void CreateGroup(NotificationData newData)
        {
            var groupKey = newData.GroupKey ?? newData.Source ?? Guid.NewGuid().ToString();

            // Collect similar notifications
            var similarNotifications = _activeNotifications
                .Where(n => n.NotificationData?.GroupKey == newData.GroupKey ||
                           n.NotificationData?.Source == newData.Source)
                .ToList();

            // Create group
            var group = new BeepNotificationGroup
            {
                GroupKey = groupKey,
                GroupTitle = newData.Source ?? newData.Title ?? "Notifications",
                GroupType = newData.Type
            };

            // Add similar notifications to group
            foreach (var notification in similarNotifications)
            {
                group.AddNotification(notification.NotificationData);
                
                // Remove individual notification
                DismissNotificationInternal(notification);
            }

            // Add new notification to group
            group.AddNotification(newData);

            // Subscribe to group events
            group.NotificationClicked += Group_NotificationClicked;
            group.GroupDismissed += Group_Dismissed;

            // Add to dictionary (Phase 4.4 / G23 — locked).
            lock (_lock) _notificationGroups[groupKey] = group;

            // Position the group
            var targetLocation = CalculateGroupPosition(group);
            group.Location = targetLocation;
            group.Show();
            group.BringToFront();

            // Play sound
            if (newData.PlaySound)
            {
                BeepNotificationSound.PlaySound(newData.Type, newData.CustomSoundPath);
            }
        }

        private Point CalculateGroupPosition(BeepNotificationGroup group)
        {
            // Similar to notification positioning, but account for groups
            var workingArea = (_targetScreen ?? Screen.PrimaryScreen)?.WorkingArea ?? Rectangle.Empty;
            
            int currentOffset = _screenMargin;
            
            // Account for existing groups and notifications
            foreach (var existingGroup in _notificationGroups.Values)
            {
                if (existingGroup == group)
                    break;
                    
                if (_defaultPosition == NotificationPosition.BottomRight ||
                    _defaultPosition == NotificationPosition.BottomLeft ||
                    _defaultPosition == NotificationPosition.BottomCenter)
                {
                    currentOffset += existingGroup.Height + _notificationSpacing;
                }
            }

            return _defaultPosition switch
            {
                NotificationPosition.BottomRight => new Point(
                    workingArea.Right - group.Width - _screenMargin,
                    workingArea.Bottom - currentOffset - group.Height
                ),
                NotificationPosition.BottomLeft => new Point(
                    workingArea.Left + _screenMargin,
                    workingArea.Bottom - currentOffset - group.Height
                ),
                _ => new Point(
                    workingArea.Right - group.Width - _screenMargin,
                    workingArea.Bottom - currentOffset - group.Height
                )
            };
        }

        private void DismissNotificationInternal(BeepNotification notification)
        {
            if (notification == null)
                return;

            // Phase 4.4 / G23 — _animators dictionary read under lock; the
            // returned animator is safe to use outside the lock because it is
            // owned only by this notification and disposed under the same lock
            // in CleanupNotification.
            BeepNotificationAnimator? animator;
            lock (_lock)
            {
                if (!_animators.TryGetValue(notification, out animator))
                    animator = null;
            }

            if (animator != null)
            {
                var animation = _defaultAnimation;
                animator.AnimateHide(notification, animation, () =>
                {
                    CleanupNotification(notification);
                });
            }
            else
            {
                CleanupNotification(notification);
            }
        }

        /// <summary>
        /// Returns a defensive snapshot of currently-live <see cref="BeepNotification"/>
        /// instances (skipping already-disposed ones). Optional predicate filters in-place.
        /// Used by every batch method so iteration is safe even if a member ticks/lifecycle.
        /// </summary>
        private List<BeepNotification> SnapshotLive(Func<BeepNotification, bool> predicate = null)
        {
            List<BeepNotification> snapshot;
            if (predicate == null)
            {
                snapshot = new List<BeepNotification>(_activeNotifications.Count);
                foreach (var n in _activeNotifications)
                {
                    if (n != null && !n.IsDisposed) snapshot.Add(n);
                }
            }
            else
            {
                snapshot = new List<BeepNotification>();
                foreach (var n in _activeNotifications)
                {
                    if (n != null && !n.IsDisposed && predicate(n)) snapshot.Add(n);
                }
            }
            return snapshot;
        }

        private void CleanupNotification(BeepNotification notification)
        {
            if (notification == null || notification.IsDisposed)
                return;

            // Unsubscribe FIRST so the dispose call cannot re-fire events into a half-cleaned manager
            notification.NotificationDismissed -= Notification_Dismissed;
            notification.ActionClicked -= Notification_ActionClicked;
            notification.NotificationClicked -= Notification_Clicked;

            // Add to history before cleanup. HistoryPanel is a control inside a
            // UI thread; we keep the call here (assumes UI-thread callers — no
            // exception thrown under worker-thread usage, since BeepNotificationHistory
            // handles Invoking internally).
            if (_trackHistory && notification.NotificationData != null)
            {
                HistoryPanel.AddNotification(notification.NotificationData);
            }

            // Phase 4.4 / G23 — three collection mutations under one lock:
            // _activeNotifications.Remove + _animators.Remove + dequeue.
            bool repopulate = false;
            lock (_lock)
            {
                _activeNotifications.Remove(notification);

                if (_animators.TryGetValue(notification, out var animator))
                {
                    animator.Dispose();
                    _animators.Remove(notification);
                }

                repopulate = _activeNotifications.Count > 0;
                if (_notificationQueue.Count > 0)
                {
                    var nextData = _notificationQueue.Dequeue();
                    // Mark for re-show AFTER we release the lock so we don't hold
                    // it across a UI-thread internal call.
                    RepositionNotificationsAnimated();
                    ShowNotificationInternal(nextData);
                    return;
                }
            }

            // Reposition remaining notifications BEFORE disposing to prevent flicker
            if (repopulate)
            {
                RepositionNotificationsAnimated();
            }

            // Dispose notification AFTER repositioning
            notification.Dispose();
        }

        private Point CalculateNotificationPosition(BeepNotification notification)
        {
            // Use smart positioning if enabled
            if (_smartPositioning)
            {
                return CalculateSmartPosition(notification);
            }

            var workingArea = (_targetScreen ?? Screen.PrimaryScreen)?.WorkingArea ?? Rectangle.Empty;
            
            // Calculate total height of existing notifications
            int currentOffset = _screenMargin;
            foreach (var existing in _activeNotifications)
            {
                if (existing == notification)
                    break;
                    
                if (_defaultPosition == NotificationPosition.TopLeft ||
                    _defaultPosition == NotificationPosition.TopCenter ||
                    _defaultPosition == NotificationPosition.TopRight)
                {
                    currentOffset += existing.Height + _notificationSpacing;
                }
                else if (_defaultPosition == NotificationPosition.BottomLeft ||
                         _defaultPosition == NotificationPosition.BottomCenter ||
                         _defaultPosition == NotificationPosition.BottomRight)
                {
                    currentOffset += existing.Height + _notificationSpacing;
                }
            }

            return _defaultPosition switch
            {
                NotificationPosition.TopLeft => new Point(
                    workingArea.Left + _screenMargin,
                    workingArea.Top + currentOffset
                ),
                NotificationPosition.TopCenter => new Point(
                    workingArea.Left + (workingArea.Width - notification.Width) / 2,
                    workingArea.Top + currentOffset
                ),
                NotificationPosition.TopRight => new Point(
                    workingArea.Right - notification.Width - _screenMargin,
                    workingArea.Top + currentOffset
                ),
                NotificationPosition.BottomLeft => new Point(
                    workingArea.Left + _screenMargin,
                    workingArea.Bottom - currentOffset - notification.Height
                ),
                NotificationPosition.BottomCenter => new Point(
                    workingArea.Left + (workingArea.Width - notification.Width) / 2,
                    workingArea.Bottom - currentOffset - notification.Height
                ),
                NotificationPosition.BottomRight => new Point(
                    workingArea.Right - notification.Width - _screenMargin,
                    workingArea.Bottom - currentOffset - notification.Height
                ),
                NotificationPosition.Center => new Point(
                    workingArea.Left + (workingArea.Width - notification.Width) / 2,
                    workingArea.Top + (workingArea.Height - notification.Height) / 2
                ),
                _ => new Point(
                    workingArea.Right - notification.Width - _screenMargin,
                    workingArea.Bottom - currentOffset - notification.Height
                )
            };
        }

        private void RepositionNotificationsAnimated()
        {
            // Prevent flickering by using SuspendLayout during repositioning
            foreach (var notification in _activeNotifications)
            {
                notification.SuspendLayout();
            }

            try
            {
                // Smoothly reposition remaining notifications
                foreach (var notification in _activeNotifications)
                {
                    var targetLocation = CalculateNotificationPosition(notification);

                    if (_animators.TryGetValue(notification, out var animator))
                    {
                        // Use simpler fade animation to avoid visual glitches
                        animator.Stop(); // Stop any existing animation
                        notification.Location = targetLocation; // Direct position - no animation on reposition
                    }
                    else
                    {
                        notification.Location = targetLocation;
                    }
                }
            }
            finally
            {
                // Resume layout and refresh all at once
                foreach (var notification in _activeNotifications)
                {
                    notification.ResumeLayout(false);
                }
            }
        }

        private Point CalculateSmartPosition(BeepNotification notification)
        {
            // Determine the best screen to use
            Screen targetScreen;
            
            if (_anchorForm != null && !_anchorForm.IsDisposed)
            {
                // Use screen where anchor form is located
                targetScreen = Screen.FromControl(_anchorForm);
            }
            else
            {
                // Use screen with active cursor
                targetScreen = Screen.FromPoint(Cursor.Position);
            }

            var workingArea = targetScreen.WorkingArea;

            // Calculate offset accounting for existing notifications
            int currentOffset = _screenMargin;
            foreach (var existing in _activeNotifications)
            {
                if (existing == notification)
                    break;

                if (_defaultPosition == NotificationPosition.TopLeft ||
                    _defaultPosition == NotificationPosition.TopCenter ||
                    _defaultPosition == NotificationPosition.TopRight)
                {
                    currentOffset += existing.Height + _notificationSpacing;
                }
                else if (_defaultPosition == NotificationPosition.BottomLeft ||
                         _defaultPosition == NotificationPosition.BottomCenter ||
                         _defaultPosition == NotificationPosition.BottomRight)
                {
                    currentOffset += existing.Height + _notificationSpacing;
                }
            }

            // Calculate position based on preferred location
            Point position = _defaultPosition switch
            {
                NotificationPosition.TopLeft => new Point(
                    workingArea.Left + _screenMargin,
                    workingArea.Top + currentOffset
                ),
                NotificationPosition.TopCenter => new Point(
                    workingArea.Left + (workingArea.Width - notification.Width) / 2,
                    workingArea.Top + currentOffset
                ),
                NotificationPosition.TopRight => new Point(
                    workingArea.Right - notification.Width - _screenMargin,
                    workingArea.Top + currentOffset
                ),
                NotificationPosition.BottomLeft => new Point(
                    workingArea.Left + _screenMargin,
                    workingArea.Bottom - currentOffset - notification.Height
                ),
                NotificationPosition.BottomCenter => new Point(
                    workingArea.Left + (workingArea.Width - notification.Width) / 2,
                    workingArea.Bottom - currentOffset - notification.Height
                ),
                NotificationPosition.BottomRight => new Point(
                    workingArea.Right - notification.Width - _screenMargin,
                    workingArea.Bottom - currentOffset - notification.Height
                ),
                NotificationPosition.Center => new Point(
                    workingArea.Left + (workingArea.Width - notification.Width) / 2,
                    workingArea.Top + (workingArea.Height - notification.Height) / 2
                ),
                _ => new Point(
                    workingArea.Right - notification.Width - _screenMargin,
                    workingArea.Bottom - currentOffset - notification.Height
                )
            };

            // Ensure notification stays within screen bounds
            position.X = Math.Max(workingArea.Left, Math.Min(position.X, workingArea.Right - notification.Width));
            position.Y = Math.Max(workingArea.Top, Math.Min(position.Y, workingArea.Bottom - notification.Height));

            return position;
        }
        #endregion

        #region Event Handlers

        private void Notification_Dismissed(object sender, NotificationEventArgs e)
        {
            if (sender is BeepNotification notification)
            {
                NotificationDismissed?.Invoke(this, e);
                DismissNotificationInternal(notification);
            }
        }

        private void Notification_ActionClicked(object? sender, NotificationEventArgs e)
        {
            NotificationActionClicked?.Invoke(this, e);
            
            // Auto-dismiss after action click unless specified otherwise
            if (sender is BeepNotification notification && !e.Cancel)
            {
                DismissNotificationInternal(notification);
            }
        }

        private void Notification_Clicked(object? sender, NotificationEventArgs e)
        {
            // Can be used for custom handling
        }

        private void Group_NotificationClicked(object? sender, NotificationEventArgs e)
        {
            // Forward group notification clicks
            NotificationActionClicked?.Invoke(this, e);
        }

        private void Group_Dismissed(object? sender, NotificationEventArgs e)
        {
            if (sender is BeepNotificationGroup group)
            {
                // Phase 4.4 / G23 — remove from dictionary under lock.
                string? groupKey;
                lock (_lock)
                {
                    groupKey = _notificationGroups.FirstOrDefault(kvp => kvp.Value == group).Key;
                    if (!string.IsNullOrEmpty(groupKey))
                    {
                        _notificationGroups.Remove(groupKey);
                    }
                }

                // Dispose group
                group.Dispose();

                // Reposition remaining
                RepositionNotificationsAnimated();
            }
        }

        #endregion

        #region Keyboard Navigation (Phase 5.4 / G13)

        private int _focusedIndex = -1;

        /// <summary>
        /// Cycle focus to the next notification in <see cref="_activeNotifications"/>.
        /// Order matches the manager's visual stack order. Wrap-around on reach end.
        /// Idempotent if no active notifications.
        /// </summary>
        public bool FocusNext()
        {
            var snapshot = SnapshotLive();
            if (snapshot.Count == 0) return false;

            _focusedIndex = (_focusedIndex + 1) % snapshot.Count;
            snapshot[_focusedIndex].Focus();
            return true;
        }

        /// <summary>
        /// Cycle focus to the previous notification in <see cref="_activeNotifications"/>.
        /// Wrap-around on reach start.
        /// </summary>
        public bool FocusPrevious()
        {
            var snapshot = SnapshotLive();
            if (snapshot.Count == 0) return false;

            _focusedIndex = _focusedIndex <= 0
                ? snapshot.Count - 1
                : _focusedIndex - 1;
            snapshot[_focusedIndex].Focus();
            return true;
        }

        /// <summary>
        /// Index of the currently focused notification, or -1. Useful for menu
        /// items or accessibility tools that want to highlight a row.
        /// </summary>
        public int FocusedIndex => _focusedIndex;

        #endregion

        #region Cleanup

        /// <summary>
        /// Cleanup all resources
        /// </summary>
        public void Dispose()
        {
            DismissAll();
            ClearQueue();

            if (_schedulerTimer != null)
            {
                _schedulerTimer.Stop();
                _schedulerTimer.Dispose();
                _schedulerTimer = null;
            }
            _scheduledNotifications?.Clear();
            _notificationGroups?.Clear();
            _templates?.Clear();

            if (_historyPanel != null)
            {
                _historyPanel.Dispose();
                _historyPanel = null;
            }

            // Dispose all animators (Phase 4.4 / G23 — locked snapshot).
            List<BeepNotificationAnimator> animatorSnapshot;
            lock (_lock) animatorSnapshot = new List<BeepNotificationAnimator>(_animators.Values);
            foreach (var animator in animatorSnapshot)
            {
                animator.Dispose();
            }
            lock (_lock) _animators.Clear();
        }
        #endregion
    }
}
