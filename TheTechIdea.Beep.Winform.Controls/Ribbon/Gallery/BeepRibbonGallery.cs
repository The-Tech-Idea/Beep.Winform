using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Gallery
{
    public sealed class BeepRibbonGallery : UserControl
    {
        private readonly FlowLayoutPanel _tilesPanel = new()
        {
            Dock = DockStyle.Fill,
            WrapContents = true,
            AutoScroll = true,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(2)
        };

        private readonly RibbonGalleryRenderer _renderer = new();
        private readonly List<Image> _images = [];
        private readonly Dictionary<Button, SimpleItem> _buttonMap = [];
        private readonly List<SimpleItem> _items = [];
        private readonly ContextMenuStrip _moreMenu = new();
        private readonly List<string> _recentKeys = [];
        private readonly HashSet<string> _pinnedKeys = new(StringComparer.OrdinalIgnoreCase);
        private RibbonTheme _theme = new();
        private RibbonDensity _density = RibbonDensity.Comfortable;
        private bool _compact = true;
        private bool _enableCategoryHeaders = true;
        private bool _enableLargePreviewPopup = true;
        private bool _showAllItemsInPopup;
        private int _maxRecentItems = 8;
        private int _keyboardSelectionIndex = -1;
        private string? _selectedKey;
        private bool _tabNavigatesOutOfGallery = true;
        private bool _childTileTabStops;

        public event EventHandler<RibbonGalleryItemSelectedEventArgs>? ItemSelected;
        public event EventHandler<RibbonGalleryStateChangedEventArgs>? StateChanged;

        public bool Compact
        {
            get => _compact;
            set
            {
                if (_compact == value) return;
                _compact = value;
                RefreshTiles();
            }
        }

        public bool EnableCategoryHeaders
        {
            get => _enableCategoryHeaders;
            set
            {
                if (_enableCategoryHeaders == value) return;
                _enableCategoryHeaders = value;
                RefreshTiles();
            }
        }

        public bool EnableLargePreviewPopup
        {
            get => _enableLargePreviewPopup;
            set => _enableLargePreviewPopup = value;
        }

        public bool ShowAllItemsInPopup
        {
            get => _showAllItemsInPopup;
            set
            {
                if (_showAllItemsInPopup == value) return;
                _showAllItemsInPopup = value;
                RefreshTiles();
            }
        }

        public int MaxRecentItems
        {
            get => _maxRecentItems;
            set => _maxRecentItems = Math.Max(1, value);
        }

        public bool TabNavigatesOutOfGallery
        {
            get => _tabNavigatesOutOfGallery;
            set => _tabNavigatesOutOfGallery = value;
        }

        public bool ChildTileTabStops
        {
            get => _childTileTabStops;
            set
            {
                if (_childTileTabStops == value) return;
                _childTileTabStops = value;
                RefreshTiles();
            }
        }

        public BeepRibbonGallery()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            Controls.Add(_tilesPanel);
            BorderStyle = BorderStyle.None;
            MinimumSize = new Size(120, 28);
            TabStop = true;
            AccessibleRole = AccessibleRole.List;
            AccessibleName = "Ribbon gallery";
            AccessibleDescription = "Select gallery command options.";
            _tilesPanel.TabStop = false;
            _tilesPanel.AccessibleRole = AccessibleRole.List;
            _tilesPanel.AccessibleName = "Gallery tiles";
            KeyDown += BeepRibbonGallery_KeyDown;
            PreviewKeyDown += BeepRibbonGallery_PreviewKeyDown;
            Enter += (_, __) =>
            {
                FocusSelectedTileControl();
                if (!_tilesPanel.ContainsFocus)
                {
                    _tilesPanel.Focus();
                }
            };
        }

        public void SetItems(IEnumerable<SimpleItem> items)
        {
            _items.Clear();
            if (items != null)
            {
                _items.AddRange(items.Where(i => i != null && i.IsVisible && !i.IsSeparator));
            }

            TrimStateToCurrentItems();
            RefreshTiles();
        }

        public void SetPinnedKeys(IEnumerable<string>? keys)
        {
            _pinnedKeys.Clear();
            if (keys != null)
            {
                foreach (var key in keys.Where(k => !string.IsNullOrWhiteSpace(k)))
                {
                    _pinnedKeys.Add(key);
                }
            }

            TrimStateToCurrentItems();
            RefreshTiles();
        }

        public void SetRecentKeys(IEnumerable<string>? keys)
        {
            _recentKeys.Clear();
            if (keys != null)
            {
                foreach (var key in keys.Where(k => !string.IsNullOrWhiteSpace(k)))
                {
                    if (_recentKeys.Contains(key, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    _recentKeys.Add(key);
                    if (_recentKeys.Count >= _maxRecentItems)
                    {
                        break;
                    }
                }
            }

            TrimStateToCurrentItems();
            RefreshTiles();
        }

        public IReadOnlyList<string> GetPinnedKeys()
        {
            return _pinnedKeys.ToList();
        }

        public IReadOnlyList<string> GetRecentKeys()
        {
            return _recentKeys.ToList();
        }

        public void ApplyTheme(RibbonTheme theme, RibbonDensity density)
        {
            _theme = theme ?? new RibbonTheme();
            _density = density;
            _renderer.ApplyTheme(_theme, _density);
            BackColor = _theme.GroupBack;
            _tilesPanel.BackColor = _theme.GroupBack;
            RefreshTiles();
        }

        public void SetSelected(SimpleItem? item)
        {
            _selectedKey = item == null ? null : GetItemKey(item);
            UpdateTileStyles();
        }

        private void RefreshTiles()
        {
            ClearTiles();
            var ordered = GetOrderedItems();
            var tileSize = _renderer.GetTileSize(_compact);
            int visibleCount = _showAllItemsInPopup
                ? ordered.Count
                : (_compact ? 4 : 6);

            foreach (var item in ordered.Take(visibleCount))
            {
                var button = CreateTileButton(item, tileSize);
                _buttonMap[button] = item;
                button.TabIndex = _tilesPanel.Controls.Count;
                _tilesPanel.Controls.Add(button);
            }

            if (ordered.Count > visibleCount)
            {
                var more = new Button
                {
                    AutoSize = false,
                    Size = tileSize,
                    Text = "...",
                    TextAlign = ContentAlignment.MiddleCenter,
                    Margin = new Padding(2),
                    UseVisualStyleBackColor = false,
                    TabStop = _childTileTabStops,
                    AccessibleRole = AccessibleRole.PushButton,
                    AccessibleName = "More gallery options",
                    AccessibleDescription = "Open additional gallery commands."
                };
                _renderer.StyleButton(more, false);
                more.Click += (_, __) => ShowMoreMenu(more, visibleCount);
                more.KeyDown += TileButton_KeyDown;
                more.PreviewKeyDown += TileButton_PreviewKeyDown;
                more.TabIndex = _tilesPanel.Controls.Count;
                _tilesPanel.Controls.Add(more);
            }

            UpdateTileStyles();
            UpdateKeyboardSelectionFromCurrent();
            FocusSelectedTileControl();
        }

        private void ShowMoreMenu(Button anchor, int startIndex)
        {
            _moreMenu.Items.Clear();
            _moreMenu.Font = _renderer.Theme != null
                ? BeepThemesManager.ToFont(_renderer.Theme.CommandTypography)
                : anchor.Font;

            var ordered = GetOrderedItems();
            var remaining = ordered.Skip(Math.Max(0, startIndex)).ToList();
            if (remaining.Count == 0 && !_enableLargePreviewPopup)
            {
                return;
            }

            var pinned = remaining.Where(i => _pinnedKeys.Contains(GetItemKey(i))).ToList();
            var recent = remaining.Where(i =>
                    !_pinnedKeys.Contains(GetItemKey(i)) &&
                    _recentKeys.Contains(GetItemKey(i), StringComparer.OrdinalIgnoreCase))
                .ToList();
            var pinnedOrRecent = new HashSet<string>(pinned.Select(GetItemKey), StringComparer.OrdinalIgnoreCase);
            foreach (var recentItem in recent)
            {
                pinnedOrRecent.Add(GetItemKey(recentItem));
            }
            var others = remaining.Where(i => !pinnedOrRecent.Contains(GetItemKey(i))).ToList();

            AddMoreMenuSection("Pinned", pinned);
            AddMoreMenuSection("Recent", recent);

            if (_enableCategoryHeaders)
            {
                var grouped = others
                    .GroupBy(GetItemCategory)
                    .OrderBy(g => g.Key, StringComparer.CurrentCultureIgnoreCase);
                foreach (var group in grouped)
                {
                    AddMoreMenuSection(group.Key, group.ToList());
                }
            }
            else
            {
                AddMoreMenuSection("Items", others);
            }

            if (_enableLargePreviewPopup && _items.Count > 0)
            {
                if (_moreMenu.Items.Count > 0)
                {
                    _moreMenu.Items.Add(new ToolStripSeparator());
                }

                var previewItem = new ToolStripMenuItem("Open Gallery Preview...");
                previewItem.Click += (_, __) => ShowLargePreviewDialog();
                _moreMenu.Items.Add(previewItem);
            }

            if (_moreMenu.Items.Count > 0)
            {
                _moreMenu.Show(anchor, new Point(0, anchor.Height));
                var first = _moreMenu.Items.OfType<ToolStripMenuItem>().FirstOrDefault(i => i.Enabled);
                first?.Select();
            }
        }

        private void AddMoreMenuSection(string header, List<SimpleItem> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            if (_moreMenu.Items.Count > 0)
            {
                _moreMenu.Items.Add(new ToolStripSeparator());
            }

            var head = new ToolStripMenuItem(header)
            {
                Enabled = false,
                Font = BeepThemesManager.ToFont(_theme.GroupTypography)
            };
            _moreMenu.Items.Add(head);

            foreach (var item in items)
            {
                var menuItem = new ToolStripMenuItem(GetDisplayText(item))
                {
                    Tag = item,
                    Checked = string.Equals(GetItemKey(item), _selectedKey, StringComparison.OrdinalIgnoreCase),
                    Font = BeepThemesManager.ToFont(_theme.CommandTypography)
                };
                menuItem.Click += (_, __) => SelectItem(item, true);
                menuItem.MouseUp += (_, e) =>
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        var anchor = menuItem.Owner as Control ?? this;
                        ShowItemContextMenu(anchor, item, e.Location);
                    }
                };
                _moreMenu.Items.Add(menuItem);
            }
        }

        private Button CreateTileButton(SimpleItem item, Size tileSize)
        {
            var button = new Button
            {
                AutoSize = false,
                Size = tileSize,
                Text = GetTileButtonText(item),
                Tag = item,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(2),
                UseVisualStyleBackColor = false,
                TabStop = _childTileTabStops,
                AccessibleRole = AccessibleRole.PushButton,
                AccessibleName = GetDisplayText(item),
                AccessibleDescription = BuildTileAccessibleDescription(item),
                AccessibleDefaultActionDescription = "Select gallery option"
            };

            if (!_compact && !string.IsNullOrWhiteSpace(item.ImagePath))
            {
                var image = CreateImage(item.ImagePath);
                if (image != null)
                {
                    button.Image = image;
                    button.ImageAlign = ContentAlignment.MiddleLeft;
                    button.TextImageRelation = TextImageRelation.ImageBeforeText;
                }
            }

            button.Click += TileButton_Click;
            button.MouseUp += TileButton_MouseUp;
            button.KeyDown += TileButton_KeyDown;
            button.PreviewKeyDown += TileButton_PreviewKeyDown;
            return button;
        }

        private void TileButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button button || !_buttonMap.TryGetValue(button, out var item))
            {
                return;
            }

            Focus();
            SelectItem(item, true);
        }

        private void TileButton_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            if (sender is not Button button || !_buttonMap.TryGetValue(button, out var item))
            {
                return;
            }

            ShowItemContextMenu(button, item, e.Location);
        }

        private void ShowItemContextMenu(Control anchor, SimpleItem item, Point location)
        {
            var menu = new ContextMenuStrip
            {
                Font = BeepThemesManager.ToFont(_theme.CommandTypography)
            };

            string key = GetItemKey(item);
            bool pinned = _pinnedKeys.Contains(key);
            if (pinned)
            {
                menu.Items.Add("Unpin", null, (_, __) => SetPinned(item, false));
            }
            else
            {
                menu.Items.Add("Pin", null, (_, __) => SetPinned(item, true));
            }

            menu.Items.Add("Select", null, (_, __) => SelectItem(item, true));

            if (_enableLargePreviewPopup)
            {
                menu.Items.Add("Open Gallery Preview...", null, (_, __) => ShowLargePreviewDialog());
            }

            menu.Closed += (_, __) => menu.Dispose();
            menu.Show(anchor, location);
        }

        private void SetPinned(SimpleItem item, bool pinned)
        {
            string key = GetItemKey(item);
            if (pinned)
            {
                _pinnedKeys.Add(key);
            }
            else
            {
                _pinnedKeys.Remove(key);
            }

            RaiseStateChanged();
            RefreshTiles();
        }

        private void SelectItem(SimpleItem item, bool raiseEvent)
        {
            _selectedKey = GetItemKey(item);
            RegisterRecent(item);
            UpdateTileStyles();
            UpdateKeyboardSelectionFromCurrent();
            if (raiseEvent)
            {
                ItemSelected?.Invoke(this, new RibbonGalleryItemSelectedEventArgs(item));
            }
        }

        private void RegisterRecent(SimpleItem item)
        {
            string key = GetItemKey(item);
            int existing = _recentKeys.FindIndex(k => k.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (existing >= 0)
            {
                _recentKeys.RemoveAt(existing);
            }

            _recentKeys.Insert(0, key);
            while (_recentKeys.Count > _maxRecentItems)
            {
                _recentKeys.RemoveAt(_recentKeys.Count - 1);
            }

            RaiseStateChanged();
        }

        private void UpdateTileStyles()
        {
            foreach (var kv in _buttonMap)
            {
                bool selected = string.Equals(GetItemKey(kv.Value), _selectedKey, StringComparison.OrdinalIgnoreCase);
                kv.Key.Text = GetTileButtonText(kv.Value);
                _renderer.StyleButton(kv.Key, selected);
                kv.Key.AccessibleDescription = BuildTileAccessibleDescription(kv.Value, selected);
            }
        }

        private string GetTileButtonText(SimpleItem item)
        {
            string text = GetDisplayText(item);
            return _pinnedKeys.Contains(GetItemKey(item)) ? $"* {text}" : text;
        }

        private List<SimpleItem> GetOrderedItems()
        {
            var source = _items.Where(i => i != null && i.IsVisible && !i.IsSeparator).ToList();
            if (source.Count == 0)
            {
                return [];
            }

            var pinned = source.Where(i => _pinnedKeys.Contains(GetItemKey(i))).ToList();
            var recent = source
                .Where(i => !_pinnedKeys.Contains(GetItemKey(i)))
                .OrderBy(i => GetRecentIndex(GetItemKey(i)))
                .Where(i => GetRecentIndex(GetItemKey(i)) >= 0)
                .ToList();

            var skipKeys = new HashSet<string>(pinned.Select(GetItemKey), StringComparer.OrdinalIgnoreCase);
            foreach (var recentItem in recent)
            {
                skipKeys.Add(GetItemKey(recentItem));
            }

            var remaining = source.Where(i => !skipKeys.Contains(GetItemKey(i))).ToList();
            return pinned.Concat(recent).Concat(remaining).ToList();
        }

        private int GetRecentIndex(string itemKey)
        {
            return _recentKeys.FindIndex(k => k.Equals(itemKey, StringComparison.OrdinalIgnoreCase));
        }

        private void ShowLargePreviewDialog()
        {
            using var dialog = new Form
            {
                Text = "Gallery Preview",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(720, 460),
                MinimizeBox = false,
                MaximizeBox = false,
                FormBorderStyle = FormBorderStyle.SizableToolWindow,
                BackColor = _theme.GroupBack,
                ForeColor = _theme.Text,
                Font = BeepThemesManager.ToFont(_theme.CommandTypography)
            };
            dialog.KeyPreview = true;

            var preview = new BeepRibbonGallery
            {
                Dock = DockStyle.Fill,
                Compact = false,
                ShowAllItemsInPopup = true,
                EnableCategoryHeaders = _enableCategoryHeaders,
                EnableLargePreviewPopup = false,
                MaxRecentItems = _maxRecentItems
            };
            preview.SetItems(_items);
            preview.SetPinnedKeys(_pinnedKeys);
            preview.SetRecentKeys(_recentKeys);
            preview.ApplyTheme(_theme, _density);
            preview.SetSelected(GetSelectedItem());
            preview.ItemSelected += (_, e) =>
            {
                _selectedKey = GetItemKey(e.Item);
                SetPinnedKeys(preview.GetPinnedKeys());
                SetRecentKeys(preview.GetRecentKeys());
                UpdateTileStyles();
                ItemSelected?.Invoke(this, new RibbonGalleryItemSelectedEventArgs(e.Item));
                dialog.Close();
            };
            preview.StateChanged += (_, __) =>
            {
                SetPinnedKeys(preview.GetPinnedKeys());
                SetRecentKeys(preview.GetRecentKeys());
            };

            dialog.Controls.Add(preview);
            dialog.Shown += (_, __) => preview.Focus();
            var owner = FindForm();
            if (owner != null)
            {
                dialog.ShowDialog(owner);
            }
            else
            {
                dialog.ShowDialog();
            }
        }

        private SimpleItem? GetSelectedItem()
        {
            if (string.IsNullOrWhiteSpace(_selectedKey))
            {
                return null;
            }

            return _items.FirstOrDefault(i => GetItemKey(i).Equals(_selectedKey, StringComparison.OrdinalIgnoreCase));
        }

        private void TrimStateToCurrentItems()
        {
            var keys = _items.Select(GetItemKey).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var toRemovePinned = _pinnedKeys.Where(k => !keys.Contains(k)).ToList();
            foreach (var key in toRemovePinned)
            {
                _pinnedKeys.Remove(key);
            }

            _recentKeys.RemoveAll(k => !keys.Contains(k));
        }

        private string GetItemCategory(SimpleItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.GroupName))
            {
                return item.GroupName.Trim();
            }

            string category = item.Category.ToString();
            if (!string.IsNullOrWhiteSpace(category) &&
                !category.Equals("None", StringComparison.OrdinalIgnoreCase) &&
                !category.Equals("0", StringComparison.OrdinalIgnoreCase))
            {
                return category;
            }

            return "Items";
        }

        private Image? CreateImage(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return null;
            }

            int size = _density switch
            {
                RibbonDensity.Compact => 14,
                RibbonDensity.Touch => 20,
                _ => 16
            };

            var bmp = new Bitmap(size, size);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.Clear(Color.Transparent);
            using var clipPath = new GraphicsPath();
            clipPath.AddRectangle(new Rectangle(0, 0, size, size));
            StyledImagePainter.PaintWithTint(g, clipPath, imagePath, _theme.IconColor, 1f);
            _images.Add(bmp);
            return bmp;
        }

        private void ClearTiles()
        {
            foreach (var button in _buttonMap.Keys.ToList())
            {
                button.Click -= TileButton_Click;
                button.MouseUp -= TileButton_MouseUp;
                button.KeyDown -= TileButton_KeyDown;
                button.PreviewKeyDown -= TileButton_PreviewKeyDown;
                button.Dispose();
            }

            foreach (var button in _tilesPanel.Controls.OfType<Button>())
            {
                button.KeyDown -= TileButton_KeyDown;
                button.PreviewKeyDown -= TileButton_PreviewKeyDown;
            }

            _buttonMap.Clear();
            _tilesPanel.Controls.Clear();
            DisposeImages();
        }

        private void DisposeImages()
        {
            foreach (var image in _images)
            {
                image.Dispose();
            }
            _images.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _moreMenu.Dispose();
                ClearTiles();
            }
            base.Dispose(disposing);
        }

        private void RaiseStateChanged()
        {
            StateChanged?.Invoke(this, new RibbonGalleryStateChangedEventArgs(GetPinnedKeys(), GetRecentKeys()));
        }

        private void BeepRibbonGallery_KeyDown(object? sender, KeyEventArgs e)
        {
            HandleKeyboardInput(e);
        }

        private void TileButton_KeyDown(object? sender, KeyEventArgs e)
        {
            HandleKeyboardInput(e);
        }

        private void BeepRibbonGallery_PreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                e.IsInputKey = true;
            }
        }

        private void TileButton_PreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                e.IsInputKey = true;
            }
        }

        private void HandleKeyboardInput(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Down)
            {
                MoveKeyboardSelection(1);
                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Up)
            {
                MoveKeyboardSelection(-1);
                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                ActivateKeyboardSelection();
                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                if (_moreMenu.Visible)
                {
                    _moreMenu.Close();
                }
                else
                {
                    var host = FindForm();
                    if (host != null && host.Modal && host.Text.Contains("Gallery", StringComparison.OrdinalIgnoreCase))
                    {
                        host.Close();
                    }
                }

                e.Handled = true;
                return;
            }

            if (e.KeyCode == Keys.Tab && _tabNavigatesOutOfGallery)
            {
                bool backward = e.Shift;
                if (MoveFocusOutOfGallery(backward))
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            var key = keyData & Keys.KeyCode;
            if (key is Keys.Left or Keys.Right or Keys.Up or Keys.Down or Keys.Enter or Keys.Escape or Keys.Tab)
            {
                return true;
            }

            return base.IsInputKey(keyData);
        }

        private void MoveKeyboardSelection(int delta)
        {
            var items = GetOrderedItems();
            if (items.Count == 0)
            {
                return;
            }

            if (_keyboardSelectionIndex < 0 || _keyboardSelectionIndex >= items.Count)
            {
                _keyboardSelectionIndex = 0;
            }
            else
            {
                _keyboardSelectionIndex += delta;
                if (_keyboardSelectionIndex < 0)
                {
                    _keyboardSelectionIndex = items.Count - 1;
                }
                else if (_keyboardSelectionIndex >= items.Count)
                {
                    _keyboardSelectionIndex = 0;
                }
            }

            _selectedKey = GetItemKey(items[_keyboardSelectionIndex]);
            UpdateTileStyles();
            FocusSelectedTileControl();
        }

        private void ActivateKeyboardSelection()
        {
            var items = GetOrderedItems();
            if (items.Count == 0)
            {
                return;
            }

            if (_keyboardSelectionIndex < 0 || _keyboardSelectionIndex >= items.Count)
            {
                UpdateKeyboardSelectionFromCurrent();
            }

            if (_keyboardSelectionIndex >= 0 && _keyboardSelectionIndex < items.Count)
            {
                SelectItem(items[_keyboardSelectionIndex], true);
            }
        }

        private void UpdateKeyboardSelectionFromCurrent()
        {
            var items = GetOrderedItems();
            if (items.Count == 0)
            {
                _keyboardSelectionIndex = -1;
                return;
            }

            if (string.IsNullOrWhiteSpace(_selectedKey))
            {
                _keyboardSelectionIndex = 0;
                return;
            }

            int index = items.FindIndex(i => GetItemKey(i).Equals(_selectedKey, StringComparison.OrdinalIgnoreCase));
            _keyboardSelectionIndex = index >= 0 ? index : 0;
        }

        private void FocusSelectedTileControl()
        {
            if (_buttonMap.Count == 0)
            {
                return;
            }

            if (!_childTileTabStops)
            {
                if (CanFocus)
                {
                    Focus();
                }
                return;
            }

            if (!string.IsNullOrWhiteSpace(_selectedKey))
            {
                var selected = GetButtonForItemKey(_selectedKey);
                if (selected != null && selected.CanFocus)
                {
                    selected.Focus();
                    _tilesPanel.ScrollControlIntoView(selected);
                    return;
                }
            }

            var fallback = _tilesPanel.Controls.OfType<Button>().FirstOrDefault();
            if (fallback != null && fallback.CanFocus)
            {
                fallback.Focus();
                _tilesPanel.ScrollControlIntoView(fallback);
            }
        }

        private bool MoveFocusOutOfGallery(bool backward)
        {
            Control current = GetFocusedChild(this) ?? this;

            var parent = Parent;
            if (parent != null && parent.SelectNextControl(current, !backward, true, true, true))
            {
                return true;
            }

            var form = FindForm();
            if (form != null)
            {
                return form.SelectNextControl(current, !backward, true, true, true);
            }

            return false;
        }

        private static Control? GetFocusedChild(Control root)
        {
            if (!root.ContainsFocus)
            {
                return null;
            }

            foreach (Control child in root.Controls)
            {
                if (child.Focused)
                {
                    return child;
                }

                var nested = GetFocusedChild(child);
                if (nested != null)
                {
                    return nested;
                }
            }

            return root.Focused ? root : null;
        }

        private Button? GetButtonForItemKey(string key)
        {
            foreach (var kv in _buttonMap)
            {
                if (GetItemKey(kv.Value).Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    return kv.Key;
                }
            }

            return null;
        }

        private string BuildTileAccessibleDescription(SimpleItem item, bool selected = false)
        {
            var parts = new List<string>();
            string category = GetItemCategory(item);
            if (!string.IsNullOrWhiteSpace(category))
            {
                parts.Add(category);
            }

            if (!string.IsNullOrWhiteSpace(item.ToolTip))
            {
                parts.Add(item.ToolTip);
            }

            if (!string.IsNullOrWhiteSpace(item.ShortcutText))
            {
                parts.Add($"Shortcut {item.ShortcutText}");
            }

            if (_pinnedKeys.Contains(GetItemKey(item)))
            {
                parts.Add("Pinned");
            }

            if (selected)
            {
                parts.Add("Selected");
            }

            return string.Join(". ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        private static string GetDisplayText(SimpleItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.DisplayField)) return item.DisplayField;
            if (!string.IsNullOrWhiteSpace(item.Text)) return item.Text;
            return item.Name ?? string.Empty;
        }

        private static string GetItemKey(SimpleItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.ActionID)) return item.ActionID;
            if (!string.IsNullOrWhiteSpace(item.ReferenceID)) return item.ReferenceID;
            if (!string.IsNullOrWhiteSpace(item.GuidId)) return item.GuidId;
            if (!string.IsNullOrWhiteSpace(item.Name)) return item.Name;
            if (!string.IsNullOrWhiteSpace(item.Text)) return item.Text;
            return $"{item.MenuID}:{item.MenuName}:{item.ItemType}";
        }
    }

    public sealed class RibbonGalleryItemSelectedEventArgs : EventArgs
    {
        public RibbonGalleryItemSelectedEventArgs(SimpleItem item)
        {
            Item = item;
        }

        public SimpleItem Item { get; }
    }

    public sealed class RibbonGalleryStateChangedEventArgs : EventArgs
    {
        public RibbonGalleryStateChangedEventArgs(IReadOnlyList<string> pinnedKeys, IReadOnlyList<string> recentKeys)
        {
            PinnedKeys = pinnedKeys;
            RecentKeys = recentKeys;
        }

        public IReadOnlyList<string> PinnedKeys { get; }
        public IReadOnlyList<string> RecentKeys { get; }
    }
}
