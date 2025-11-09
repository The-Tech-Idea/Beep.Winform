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
        /// Get the notification history panel (creates if needed)
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
        /// Show a notification with full data model
        /// </summary>
        public void Show(NotificationData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // Check Do Not Disturb mode
            if (_doNotDisturbMode && data.Priority < _doNotDisturbMinPriority)
            {
                // Queue notification for later (when DND is disabled)
                _notificationQueue.Enqueue(data);
                return;
            }

            // If at max capacity, queue it
            if (_activeNotifications.Count >= _maxVisibleNotifications)
            {
                _notificationQueue.Enqueue(data);
                return;
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
            var notifications = _activeNotifications.ToList();
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
            var notifications = _activeNotifications
                .Where(n => n.NotificationData?.Type == type)
                .ToList();

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
            var notifications = _activeNotifications
                .Where(n => n.NotificationData?.Priority == priority)
                .ToList();

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
            var notifications = _activeNotifications
                .Where(n => n.NotificationData?.Priority != priority)
                .ToList();

            foreach (var notification in notifications)
            {
                DismissNotificationInternal(notification);
            }
        }

        /// <summary>
        /// Clear the notification queue without showing
        /// </summary>
        public void ClearQueue()
        {
            _notificationQueue.Clear();
        }

        /// <summary>
        /// Dismiss all and clear queue
        /// </summary>
        public void Clear()
        {
            DismissAll();
            ClearQueue();
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

            // Add to active list
            _activeNotifications.Add(notification);

            // Play sound if requested
            if (data.PlaySound)
            {
                BeepNotificationSound.PlaySound(data.Type, data.CustomSoundPath);
            }

            // Get target position
            var targetLocation = CalculateNotificationPosition(notification);

            // Create animator
            var animator = new BeepNotificationAnimator();
            _animators[notification] = animator;

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

            if (_notificationGroups.TryGetValue(groupKey, out var group))
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

            // Add to dictionary
            _notificationGroups[groupKey] = group;

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

            // Get animator
            if (_animators.TryGetValue(notification, out var animator))
            {
                // Animate out
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

        private void CleanupNotification(BeepNotification notification)
        {
            if (notification == null)
                return;

            // Add to history before cleanup
            if (_trackHistory && notification.NotificationData != null)
            {
                HistoryPanel.AddNotification(notification.NotificationData);
            }

            // Unsubscribe from events
            notification.NotificationDismissed -= Notification_Dismissed;
            notification.ActionClicked -= Notification_ActionClicked;
            notification.NotificationClicked -= Notification_Clicked;

            // Remove from active list FIRST
            _activeNotifications.Remove(notification);

            // Dispose animator
            if (_animators.TryGetValue(notification, out var animator))
            {
                animator.Dispose();
                _animators.Remove(notification);
            }

            // Reposition remaining notifications BEFORE disposing to prevent flicker
            if (_activeNotifications.Count > 0)
            {
                RepositionNotificationsAnimated();
            }

            // Dispose notification AFTER repositioning
            notification.Dispose();

            // Show next queued notification if any
            if (_notificationQueue.Count > 0)
            {
                var nextData = _notificationQueue.Dequeue();
                ShowNotificationInternal(nextData);
            }
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
                // Remove from groups dictionary
                var groupKey = _notificationGroups.FirstOrDefault(kvp => kvp.Value == group).Key;
                if (!string.IsNullOrEmpty(groupKey))
                {
                    _notificationGroups.Remove(groupKey);
                }

                // Dispose group
                group.Dispose();

                // Reposition remaining
                RepositionNotificationsAnimated();
            }
        }
        #endregion

        #region Cleanup

        /// <summary>
        /// Cleanup all resources
        /// </summary>
        public void Dispose()
        {
            DismissAll();
            ClearQueue();
            
            // Dispose all animators
            foreach (var animator in _animators.Values)
            {
                animator.Dispose();
            }
            _animators.Clear();
        }
        #endregion
    }
}
