using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Buttons;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.TextFields;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    /// <summary>
    /// History panel for viewing dismissed notifications
    /// Shows a list of past notifications with filtering and search
    /// </summary>
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("Notification history panel with filtering and search")]
    public class BeepNotificationHistory : BaseControl
    {
        #region Private Fields
        private readonly List<NotificationHistoryItem> _history;

        // Phase 2: chrome now composed of Beep child controls (G3).
        // _filterCombo / _statusFilterCombo stay as standard ComboBox because
        // BeepComboBox uses a SimpleItem data model and overlay popup, which
        // would change the wiring contract for callers of SelectedIndex.
        private readonly BeepPanel _headerPanel;
        private readonly BeepLabel _titleLabel;
        private readonly BeepTextBox _searchBox;
        private readonly ComboBox _filterCombo;
        private readonly ComboBox _statusFilterCombo;
        private readonly BeepButton _clearButton;
        private readonly BeepButton _markAllReadButton;

        // Phase 2 completion: items are now BeepPanel child rows inside
        // _itemsHost (Dock=Fill, AutoScroll=true). No manual GDI paint remains.
        private Panel _itemsHost;
        private readonly List<BeepPanel> _itemPanels = new();

        private int _itemHeight = 60;
        private int _maxHistorySize = 100;
        private NotificationType? _filterType = null;
        private bool? _filterReadStatus = null;
        private readonly System.Windows.Forms.Timer _searchDebounceTimer;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of BeepNotificationHistory
        /// </summary>
        public BeepNotificationHistory()
        {
            _history = new List<NotificationHistoryItem>();

            // BaseControl configuration
            ControlStyle = BeepControlStyle.Material3;
            IsRounded = true;
            BorderRadius = DpiScalingHelper.ScaleValue(8, this);
            ShowShadow = true;
            ShadowOffset = DpiScalingHelper.ScaleValue(2, this);
            ShowAllBorders = true;
            BorderThickness = 1;
            MinimumSize = new Size(DpiScalingHelper.ScaleValue(300, this), DpiScalingHelper.ScaleValue(400, this));
            Size = new Size(DpiScalingHelper.ScaleValue(350, this), DpiScalingHelper.ScaleValue(500, this));

            // Header panel — BeepPanel hosts the chrome so it inherits theme.
            // (Manual GDI paint replaced by RebuildItems child controls)
            _headerPanel = new BeepPanel
            {
                Dock = DockStyle.Top,
                Height = DpiScalingHelper.ScaleValue(100, this),
                UseThemeColors = true
            };

            // Title — BeepLabel + theme typography via BeepFontManager
            _titleLabel = new BeepLabel
            {
                Text = "Notification History",
                Location = new Point(DpiScalingHelper.ScaleValue(12, this), DpiScalingHelper.ScaleValue(12, this)),
                AutoSize = true,
                UseThemeColors = true
            };

            // Search — BeepTextBox supplies placeholder text + theming
            _searchBox = new BeepTextBox
            {
                PlaceholderText = "Search notifications...",
                Location = new Point(DpiScalingHelper.ScaleValue(12, this), DpiScalingHelper.ScaleValue(40, this)),
                Width = DpiScalingHelper.ScaleValue(200, this),
                Height = DpiScalingHelper.ScaleValue(24, this),
                UseThemeColors = true
            };
            _searchBox.TextChanged += SearchBox_TextChanged;

            // Filter combos — WinForms ComboBox stays because BeepComboBox
            // requires SimpleItem data model + custom overlay popup.
            _filterCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(DpiScalingHelper.ScaleValue(220, this), DpiScalingHelper.ScaleValue(40, this)),
                Width = DpiScalingHelper.ScaleValue(100, this),
                Height = DpiScalingHelper.ScaleValue(24, this)
            };
            _filterCombo.Items.Add("All");
            _filterCombo.Items.Add("Info");
            _filterCombo.Items.Add("Success");
            _filterCombo.Items.Add("Warning");
            _filterCombo.Items.Add("Error");
            _filterCombo.Items.Add("System");
            _filterCombo.SelectedIndex = 0;
            _filterCombo.SelectedIndexChanged += FilterCombo_SelectedIndexChanged;

            _statusFilterCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(DpiScalingHelper.ScaleValue(12, this), DpiScalingHelper.ScaleValue(70, this)),
                Width = DpiScalingHelper.ScaleValue(100, this),
                Height = DpiScalingHelper.ScaleValue(24, this)
            };
            _statusFilterCombo.Items.Add("All");
            _statusFilterCombo.Items.Add("Read");
            _statusFilterCombo.Items.Add("Unread");
            _statusFilterCombo.SelectedIndex = 0;
            _statusFilterCombo.SelectedIndexChanged += StatusFilterCombo_SelectedIndexChanged;

            // Action buttons — BeepButton for theme integration
            _clearButton = new BeepButton
            {
                Text = "Clear",
                Location = new Point(DpiScalingHelper.ScaleValue(120, this), DpiScalingHelper.ScaleValue(70, this)),
                Width = DpiScalingHelper.ScaleValue(80, this),
                Height = DpiScalingHelper.ScaleValue(24, this),
                UseThemeColors = true
            };
            _clearButton.Click += ClearButton_Click;

            _markAllReadButton = new BeepButton
            {
                Text = "Mark All Read",
                Location = new Point(DpiScalingHelper.ScaleValue(210, this), DpiScalingHelper.ScaleValue(70, this)),
                Width = DpiScalingHelper.ScaleValue(100, this),
                Height = DpiScalingHelper.ScaleValue(24, this),
                UseThemeColors = true
            };
            _markAllReadButton.Click += MarkAllReadButton_Click;

            _headerPanel.Controls.Add(_titleLabel);
            _headerPanel.Controls.Add(_searchBox);
            _headerPanel.Controls.Add(_filterCombo);
            _headerPanel.Controls.Add(_statusFilterCombo);
            _headerPanel.Controls.Add(_clearButton);
            _headerPanel.Controls.Add(_markAllReadButton);

            // Items host — Dock=Fill with AutoScroll so rows overflow into a
            // scrollable region. Each row is a BeepPanel with Dock=Top matching
            // ItemHeight.
            _itemsHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoScroll = true
            };
            _itemsHost.Scroll += (s, e) => { /* AutoScroll handles scroll events internally */ };

            // Add controls — order affects Z-order / docking priority.
            this.Controls.Add(_itemsHost);
            this.Controls.Add(_headerPanel);

            _searchDebounceTimer = new System.Windows.Forms.Timer { Interval = 250 };
            _searchDebounceTimer.Tick += SearchDebounceTimer_Tick;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Maximum number of notifications to keep in history
        /// </summary>
        [Category("Behavior")]
        [Description("Maximum number of notifications to store")]
        [DefaultValue(100)]
        public int MaxHistorySize
        {
            get => _maxHistorySize;
            set => _maxHistorySize = Math.Max(10, value);
        }

        /// <summary>
        /// Height of each history item in pixels
        /// </summary>
        [Category("Appearance")]
        [Description("Height of each notification item")]
        [DefaultValue(60)]
        public int ItemHeight
        {
            get => _itemHeight;
            set
            {
                _itemHeight = Math.Max(40, value);
                RebuildItems();
            }
        }

        /// <summary>
        /// Number of items in history
        /// </summary>
        [Browsable(false)]
        public int Count => _history.Count;
        #endregion

        #region Public Methods
        /// <summary>
        /// Add a notification to history
        /// </summary>
        public void AddNotification(NotificationData data)
        {
            if (data == null)
                return;

            // First-time lazy load: only read the file once per instance.
            if (!_loaded) Load();

            var item = new NotificationHistoryItem
            {
                Data = data,
                DismissedAt = DateTime.Now
            };

            _history.Insert(0, item); // Add to top

            // Trim history if needed
            while (_history.Count > _maxHistorySize)
            {
                _history.RemoveAt(_history.Count - 1);
            }

            RebuildItems();
            ScheduleAutoSave();
        }

        /// <summary>
        /// Clear all history. If persistence is enabled the on-disk file is
        /// removed as well so the cleared state survives process restarts.
        /// </summary>
        public void ClearHistory()
        {
            _history.Clear();
            RebuildItems();

            // Persisted clear: remove the file outright so subsequent
            // loads find an empty history rather than the stale state.
            try
            {
                if (!string.IsNullOrEmpty(_persistenceFilePath) && File.Exists(_persistenceFilePath))
                    File.Delete(_persistenceFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepNotificationHistory] ClearHistory file delete failed: {ex.Message}");
            }

            // Cancel any pending debounced save (history is empty now).
            _autoSaveTimer?.Stop();
        }

        /// <summary>
        /// Get filtered history items
        /// </summary>
        public List<NotificationHistoryItem> GetFilteredItems()
        {
            var query = _history.AsEnumerable();

            // Apply type filter
            if (_filterType.HasValue)
            {
                query = query.Where(h => h.Data.Type == _filterType.Value);
            }

            if (_filterReadStatus.HasValue)
            {
                query = query.Where(h => h.Data.IsRead == _filterReadStatus.Value);
            }

            // Apply search filter
            var searchText = _searchBox?.Text?.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(h =>
                    h.Data.Title?.ToLower().Contains(searchText) == true ||
                    h.Data.Message?.ToLower().Contains(searchText) == true);
            }

            return query.ToList();
        }
        #endregion

        #region Items rebuild (Phase 2 completion — child controls)
        private void RebuildItems()
        {
            if (_itemsHost == null) return;

            // Get the live filtered list (respects type/status/search filters)
            var items = GetFilteredItems();

            _itemsHost.SuspendLayout();
            try
            {
                // Trim rows that exceed the filtered list
                while (_itemPanels.Count > items.Count)
                {
                    var lastIndex = _itemPanels.Count - 1;
                    var stale = _itemPanels[lastIndex];
                    _itemsHost.Controls.Remove(stale);
                    stale.Dispose();
                    _itemPanels.RemoveAt(lastIndex);
                }

                // Add/update rows
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (i < _itemPanels.Count)
                    {
                        UpdateHistoryRow(_itemPanels[i], item);
                    }
                    else
                    {
                        var row = CreateHistoryRow();
                        UpdateHistoryRow(row, item);
                        row.Click += (s, e) => HistoryItemClicked?.Invoke(this, item);
                        _itemsHost.Controls.Add(row);
                        _itemPanels.Add(row);
                    }
                }
            }
            finally
            {
                _itemsHost.ResumeLayout();
            }

            // Show empty-state label if no rows
            if (items.Count == 0)
            {
                HideEmptyState();    // Remove stale placeholder
                ShowEmptyState();    // Add new placeholder
            }
            else
            {
                HideEmptyState();    // Remove placeholder when items exist
            }
        }

        private BeepPanel CreateHistoryRow()
        {
            var row = new BeepPanel
            {
                Dock = DockStyle.Top,
                Height = _itemHeight,
                UseThemeColors = false,
                Padding = new Padding(
                    DpiScalingHelper.ScaleValue(12, this),
                    DpiScalingHelper.ScaleValue(4, this),
                    DpiScalingHelper.ScaleValue(12, this),
                    DpiScalingHelper.ScaleValue(4, this)),
                Cursor = Cursors.Hand
            };

            var inner = new BeepPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            row.Controls.Add(inner);

            // Timestamp — top-right
            inner.Controls.Add(new BeepLabel
            {
                Dock = DockStyle.Top,
                Height = DpiScalingHelper.ScaleValue(14, this),
                TextAlign = ContentAlignment.TopRight,
                AutoSize = false,
                AutoEllipsis = true,
                UseThemeColors = true,
                ForeColor = Color.FromArgb(120, this.ForeColor),
                TabStop = false,
                Tag = "_time"
            });
            // Title — single line, bold
            inner.Controls.Add(new BeepLabel
            {
                Dock = DockStyle.Top,
                Height = DpiScalingHelper.ScaleValue(18, this),
                AutoEllipsis = true,
                UseThemeColors = true,
                TabStop = false,
                Tag = "_title"
            });
            // Message — fill remaining
            inner.Controls.Add(new BeepLabel
            {
                Dock = DockStyle.Fill,
                AutoEllipsis = true,
                UseThemeColors = true,
                ForeColor = Color.FromArgb(180, this.ForeColor),
                TabStop = false,
                Tag = "_msg"
            });

            return row;
        }

        private void UpdateHistoryRow(BeepPanel row, NotificationHistoryItem item)
        {
            // Type-tinted background
            row.BackColor = ResolveTypeTint(item.Data.Type);

            BeepLabel title = null, msg = null, ts = null;
            foreach (Control c in row.Controls)
            {
                if (c is BeepPanel inner)
                {
                    foreach (Control ic in inner.Controls)
                    {
                        if (ic.Tag is string tag)
                        {
                            if (tag == "_title") title = (BeepLabel)ic;
                            else if (tag == "_msg") msg = (BeepLabel)ic;
                            else if (tag == "_time") ts = (BeepLabel)ic;
                        }
                    }
                }
            }

            if (title != null) title.Text = item.Data.Title ?? "Notification";
            if (msg   != null) msg.Text   = item.Data.Message ?? string.Empty;
            if (ts    != null) ts.Text    = GetTimeAgo(item.DismissedAt);
        }

        private Color ResolveTypeTint(NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => Color.FromArgb(20, 76, 175, 80),
                NotificationType.Warning => Color.FromArgb(20, 255, 152, 0),
                NotificationType.Error   => Color.FromArgb(20, 244, 67, 54),
                NotificationType.Info    => Color.FromArgb(20, 33, 150, 243),
                NotificationType.System  => Color.FromArgb(20, 158, 158, 158),
                _                        => Color.FromArgb(20, 128, 128, 128)
            };
        }

        private void ShowEmptyState()
        {
            // Add a centered empty-state label. HideEmptyState is called first
            // by the caller to remove any previous placeholder.
            var label = new BeepLabel
            {
                Text = "No notifications in history",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                UseThemeColors = true,
                ForeColor = Color.FromArgb(120, this.ForeColor)
            };

            _itemsHost.Controls.Add(label);
            label.Tag = "_empty";
        }

        private void HideEmptyState()
        {
            for (int i = _itemsHost.Controls.Count - 1; i >= 0; i--)
            {
                if (_itemsHost.Controls[i] is BeepLabel lbl && lbl.Tag as string == "_empty")
                {
                    _itemsHost.Controls.RemoveAt(i);
                    lbl.Dispose();
                    break;
                }
            }
        }
        #endregion

        #region Event Handlers
        private void SearchBox_TextChanged(object? sender, EventArgs e)
        {
            _searchDebounceTimer?.Stop();
            _searchDebounceTimer?.Start();
        }

        private void SearchDebounceTimer_Tick(object? sender, EventArgs e)
        {
            _searchDebounceTimer?.Stop();
            RebuildItems();
        }

        private void FilterCombo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            _filterType = _filterCombo.SelectedIndex switch
            {
                1 => NotificationType.Info,
                2 => NotificationType.Success,
                3 => NotificationType.Warning,
                4 => NotificationType.Error,
                5 => NotificationType.System,
                _ => null
            };

            RebuildItems();
        }

        private void StatusFilterCombo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            _filterReadStatus = _statusFilterCombo.SelectedIndex switch
            {
                1 => true,
                2 => false,
                _ => null
            };

            RebuildItems();
        }

        public event EventHandler<NotificationHistoryItem>? HistoryItemClicked;

        private void MarkAllReadButton_Click(object? sender, EventArgs e)
        {
            foreach (var item in _history)
            {
                if (item.Data != null && !item.Data.IsRead)
                {
                    item.Data.IsRead = true;
                    item.Data.ReadTimestamp = DateTime.Now;
                }
            }
            RebuildItems();
        }

        private void ClearButton_Click(object? sender, EventArgs e)
        {
            ClearHistory();
        }

        #region Persistence (Phase 7 / G14)
        private string _persistenceFilePath;
        private System.Windows.Forms.Timer _autoSaveTimer;
        private bool _loaded;
        private static readonly JsonSerializerOptions s_jsonOpts = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IncludeFields = true
        };

        /// <summary>
        /// File path used by <see cref="Save"/> / <see cref="Load"/> for
        /// history persistence. Default: <c>%AppData%\TheTechIdea\Beep\notifications.json</c>.
        /// Set to <c>null</c> or empty to disable persistence entirely.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PersistenceFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_persistenceFilePath))
                {
                    var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    _persistenceFilePath = Path.Combine(appData, "TheTechIdea", "Beep", "notifications.json");
                }
                return _persistenceFilePath;
            }
            set => _persistenceFilePath = value;
        }

        /// <summary>
        /// Persist the current history to <see cref="PersistenceFilePath"/>.
        /// Trims to the last <c>MaxHistorySize</c> items. If the path is
        /// empty or null, this is a no-op. Errors are swallowed (logged only
        /// so a read-only AppData directory does not crash the app).
        /// </summary>
        public void Save()
        {
            var path = _persistenceFilePath;     // honor caller-set null/empty without auto-defaulting
            if (string.IsNullOrEmpty(path)) return;

            try
            {
                // Cap to MaxHistorySize so the file does not grow unboundedly.
                var items = _history.Take(MaxHistorySize).ToList();
                var dto = new PersistedHistoryDto
                {
                    Version = 2,
                    SavedAt = DateTime.UtcNow,
                    Items = items.Select(item => new PersistedItemDto
                    {
                        Data = item.Data,
                        DismissedAt = item.DismissedAt,
                        WasActioned = item.WasActioned
                    }).ToList()
                };

                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonSerializer.Serialize(dto, s_jsonOpts);
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepNotificationHistory] Save failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Load history from <see cref="PersistenceFilePath"/>. Replaces current
        /// in-memory history with the on-disk version. No-op on missing file
        /// or empty path. Multi-call-safe: a second Load replaces the first.
        /// </summary>
        public void Load()
        {
            var path = _persistenceFilePath;     // same convention as Save
            if (string.IsNullOrEmpty(path)) return;
            if (!File.Exists(path)) return;

            try
            {
                var json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json)) return;

                var dto = JsonSerializer.Deserialize<PersistedHistoryDto>(json, s_jsonOpts);
                if (dto?.Items == null) return;

                _history.Clear();
                foreach (var item in dto.Items)
                {
                    if (item?.Data == null) continue;
                    _history.Add(new NotificationHistoryItem
                    {
                        Data = item.Data,
                        DismissedAt = item.DismissedAt,
                        WasActioned = item.WasActioned
                    });
                }

                RebuildItems();
                _loaded = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepNotificationHistory] Load failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Schedule a debounced save (10 seconds from now). Restarting the
        /// timer cancels a previously-pending save — bursts of history items
        /// coalesce into a single write. Also blocks saves while the file
        /// path is empty.
        /// </summary>
        private void ScheduleAutoSave()
        {
            if (string.IsNullOrEmpty(_persistenceFilePath)) return;

            if (_autoSaveTimer == null)
            {
                _autoSaveTimer = new System.Windows.Forms.Timer { Interval = 10000 };
                _autoSaveTimer.Tick += (s, e) =>
                {
                    _autoSaveTimer.Stop();
                    Save();
                };
            }

            _autoSaveTimer.Stop();
            _autoSaveTimer.Start();
        }

        /// <summary>
        /// Serialization DTO. Versioned so future migration code can branch
        /// on <see cref="Version"/>. Uses own DTO type so we can evolve
        /// the persistence shape without touching <see cref="NotificationData"/>
        /// or <see cref="NotificationHistoryItem"/> directly.
        /// </summary>
        private sealed class PersistedHistoryDto
        {
            public int Version { get; set; }
            public DateTime SavedAt { get; set; }
            public List<PersistedItemDto> Items { get; set; } = new();
        }

        private sealed class PersistedItemDto
        {
            public NotificationData Data { get; set; }
            public DateTime DismissedAt { get; set; }
            public bool WasActioned { get; set; }
        }
        #endregion

        #region Helper Methods
        #endregion

        private string GetTimeAgo(DateTime time)
        {
            var span = DateTime.Now - time;

            if (span.TotalMinutes < 1)
                return "Just now";
            if (span.TotalMinutes < 60)
                return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24)
                return $"{(int)span.TotalHours}h ago";
            if (span.TotalDays < 7)
                return $"{(int)span.TotalDays}d ago";

            return time.ToString("MMM d");
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            RebuildItems();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _itemHeight = DpiScalingHelper.ScaleValue(60, this);
            BorderRadius = DpiScalingHelper.ScaleValue(8, this);
            ShadowOffset = DpiScalingHelper.ScaleValue(2, this);
            MinimumSize = DpiScalingHelper.ScaleSize(new Size(300, 400), this);
            Size = DpiScalingHelper.ScaleSize(new Size(350, 500), this);
            RebuildItems();
            Invalidate();
        }

        /// <summary>
        /// Cleanup resources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _textFont?.Dispose();
                _headerPanel?.Dispose();
                _itemsHost?.Dispose();
                _titleLabel?.Dispose();
                _searchBox?.Dispose();
                _filterCombo?.Dispose();
                _clearButton?.Dispose();
                _statusFilterCombo?.Dispose();
                _markAllReadButton?.Dispose();
                _searchDebounceTimer?.Dispose();

                // Persistence (Phase 7): flush any pending debounced save
                // synchronously so the latest state reaches disk before the
                // process disposes the panel. Cancel after to avoid the timer
                // firing after this control is gone.
                if (_autoSaveTimer != null)
                {
                    _autoSaveTimer.Stop();
                    Save();
                    _autoSaveTimer.Dispose();
                    _autoSaveTimer = null;
                }
            }
            base.Dispose(disposing);
        }
        #endregion
    }

    /// <summary>
    /// Represents a notification in history
    /// </summary>
    public class NotificationHistoryItem
    {
        /// <summary>The notification data</summary>
        public NotificationData Data { get; set; } = new NotificationData();
        
        /// <summary>When the notification was dismissed</summary>
        public DateTime DismissedAt { get; set; }
        
        /// <summary>Was the notification acted upon</summary>
        public bool WasActioned { get; set; }
    }
}
