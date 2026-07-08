using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Text.Json;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Accessibility;
using TheTechIdea.Beep.Winform.Controls.Backstage;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.Customization;
using TheTechIdea.Beep.Winform.Controls.Gallery;
using TheTechIdea.Beep.Winform.Controls.Rendering;
using TheTechIdea.Beep.Winform.Controls.Search;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Tokens;
using TheTechIdea.Beep.Winform.Controls.Tooltips;
using TheTechIdea.Beep.Winform.Controls.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        private void InitializeBackstageLayout()
        {
            _backstageSplit.SplitterDistance = 180;
            _backstageSplit.Panel1MinSize = 140;
            _backstageSplit.Panel2MinSize = 260;

            _backstageSplit.Panel1.Controls.Clear();
            _backstageSplit.Panel1.Controls.Add(_backstageNavList);

            _backstageContentHost.Controls.Clear();
            _backstageContentHost.Controls.Add(_backstageActions);
            _backstageContentHost.Controls.Add(_backstageFooter);
            _backstageContentHost.Controls.Add(_backstageTitle);

            _backstageSplit.Panel2.Controls.Clear();
            _backstageSplit.Panel2.Controls.Add(_backstageContentHost);

            _backstagePanelContent.Controls.Clear();
            _backstagePanelContent.Controls.Add(_backstageSplit);

            _backstageNavList.SelectedItemChanged -= BackstageNavList_SelectedItemChanged;
            _backstageNavList.SelectedItemChanged += BackstageNavList_SelectedItemChanged;
            _backstageActions.SizeChanged -= BackstageActions_SizeChanged;
            _backstageActions.SizeChanged += BackstageActions_SizeChanged;
            BuildBackstageFooterActions();
        }

        public void BuildBackstageFromSimpleItems()
        {
            BuildBackstageFromSimpleItems(_backstageItems);
        }

        public void BuildBackstageFromSimpleItems(IEnumerable<SimpleItem>? sectionNodes)
        {
            _backstageSectionMap.Clear();
            _backstageNavList.Items.Clear();
            ClearBackstageActions();
            _activeBackstageIndex = -1;

            if (sectionNodes == null)
            {
                _backstageTitle.Text = string.Empty;
                return;
            }

            int index = 0;
            foreach (var section in sectionNodes.Where(IsVisibleNode))
            {
                _backstageSectionMap[index] = section;
                _backstageNavList.Items.Add(GetDisplayText(section));
                index++;
            }

            if (_backstageNavList.Items.Count > 0)
            {
                _backstageNavList.SelectedIndex = 0;
            }
            else
            {
                _backstageTitle.Text = string.Empty;
            }
        }

        public void LoadStandardBackstageTemplate(string applicationTitle = "Application", bool replaceExistingSections = true)
        {
            var sections = RibbonBackstageTemplateBuilder.CreateStandardTemplate(applicationTitle);
            if (replaceExistingSections)
            {
                _backstageItems.Clear();
            }

            foreach (var section in sections)
            {
                _backstageItems.Add(section);
            }
        }

        public void BindBackstageRecentItems(IEnumerable<RibbonRecentItemModel>? items, bool clearCurrent = true)
        {
            if (clearCurrent)
            {
                _backstageRecentItems.Clear();
                _backstagePinnedItems.Clear();
            }

            if (items == null)
            {
                return;
            }

            foreach (var model in items)
            {
                if (model == null)
                {
                    continue;
                }

                var node = model.ToSimpleItem();
                if (model.IsPinned)
                {
                    PinBackstageItem(node);
                }
                else
                {
                    AddBackstageRecentItem(node);
                }
            }
        }

        public void AddBackstageFooterAction(SimpleItem item)
        {
            if (item == null)
            {
                return;
            }

            _backstageFooterItems.Add(item);
        }

        public void ClearBackstageFooterActionItems()
        {
            _backstageFooterItems.Clear();
        }

        public void AddBackstageRecentItem(SimpleItem item, bool pin = false)
        {
            if (item == null)
            {
                return;
            }

            string key = GetMergeKey(item);
            RemoveBackstageItemByKey(_backstageRecentItems, key);

            if (pin)
            {
                PinBackstageItem(item);
                return;
            }

            if (string.IsNullOrWhiteSpace(item.SubText3))
            {
                item.SubText3 = DateTime.UtcNow.ToString("O");
            }

            _backstageRecentItems.Insert(0, item);
            while (_backstageRecentItems.Count > _backstageRecentLimit)
            {
                _backstageRecentItems.RemoveAt(_backstageRecentItems.Count - 1);
            }
        }

        public bool PinBackstageItem(SimpleItem item)
        {
            if (item == null)
            {
                return false;
            }

            string key = GetMergeKey(item);
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            RemoveBackstageItemByKey(_backstagePinnedItems, key);
            RemoveBackstageItemByKey(_backstageRecentItems, key);
            _backstagePinnedItems.Insert(0, item);
            return true;
        }

        public bool UnpinBackstageItem(string itemKey, bool moveToRecent = true)
        {
            int index = FindBackstageItemIndexByKey(_backstagePinnedItems, itemKey);
            if (index < 0)
            {
                return false;
            }

            var item = _backstagePinnedItems[index];
            _backstagePinnedItems.RemoveAt(index);
            if (moveToRecent)
            {
                AddBackstageRecentItem(item, false);
            }
            return true;
        }

        public void ClearBackstageRecentItems()
        {
            _backstageRecentItems.Clear();
        }

        public void ClearBackstagePinnedItems()
        {
            _backstagePinnedItems.Clear();
        }

        public void ShowBackstageSection(int index)
        {
            if (!_backstageSectionMap.TryGetValue(index, out var section))
            {
                return;
            }

            _activeBackstageIndex = index;
            _backstageTitle.Text = GetDisplayText(section);
            ClearBackstageActions();

            foreach (var action in GetBackstageActions(section))
            {
                if (action.IsSeparator)
                {
                    var sep = new Label
                    {
                        Height = 1,
                        Width = Math.Max(80, _backstageActions.ClientSize.Width - 24),
                        Margin = new Padding(4, 8, 4, 8),
                        BackColor = _theme.GroupBorder
                    };
                    _backstageActions.Controls.Add(sep);
                    continue;
                }

                var row = CreateBackstageActionRow(action);
                _backstageActions.Controls.Add(row);
            }

            BackstageSectionChanged?.Invoke(this, new BackstageSectionChangedEventArgs(section, index));
        }

        private string GetBackstageActionButtonText(SimpleItem action)
        {
            string text = GetDisplayText(action);
            if (IsBackstageSectionHeader(action))
            {
                return text;
            }

            return text;
        }

        private Panel CreateBackstageActionRow(SimpleItem action)
        {
            var row = new Panel
            {
                Width = Math.Max(120, _backstageActions.ClientSize.Width - 30),
                Height = 36,
                Margin = new Padding(4),
                Padding = new Padding(0),
                BackColor = Color.Transparent,
                Tag = action
            };

            var button = new BeepButton
            {
                IsChild = true,
                Dock = DockStyle.Fill,
                Text = GetBackstageActionButtonText(action),
                TextAlign = ContentAlignment.MiddleLeft,
                UseThemeColors = true,
                AccessibleName = GetDisplayText(action),
                AccessibleDescription = BuildToolTip(action),
                Tag = action
            };
            button.Click += BackstageActionButton_Click;
            button.MouseUp += BackstageActionButton_MouseUp;
            RibbonAccessibilityHelper.ApplyControlAccessibility(
                button,
                GetDisplayText(action),
                BuildToolTip(action),
                AccessibleRole.PushButton);

            if (_backstageShowTimestamps && TryGetBackstageItemTimestamp(action, out var openedUtc))
            {
                var stamp = new Label
                {
                    Dock = DockStyle.Right,
                    Width = 88,
                    TextAlign = ContentAlignment.MiddleRight,
                    ForeColor = ColorUtils.ShiftLuminance(_theme.Text, -0.25f),
                    BackColor = Color.Transparent,
                    Font = BeepThemesManager.ToFont(_theme.GroupTypography),
                    Text = FormatBackstageTimestamp(action, openedUtc),
                    Tag = action,
                    AccessibleName = $"Last opened for {GetDisplayText(action)}",
                    AccessibleDescription = openedUtc.ToLocalTime().ToString("G")
                };
                RibbonAccessibilityHelper.ApplyControlAccessibility(
                    stamp,
                    stamp.AccessibleName,
                    stamp.AccessibleDescription,
                    AccessibleRole.StaticText);
                row.Controls.Add(stamp);
            }

            if (!string.IsNullOrWhiteSpace(action.ImagePath))
            {
                var image = CreateBackstageImage(action.ImagePath);
                if (image != null)
                {
                    button.Image = image;
                    button.ImageAlign = ContentAlignment.MiddleLeft;
                    button.TextImageRelation = TextImageRelation.ImageBeforeText;
                }
            }

            if (!IsBackstageSectionHeader(action))
            {
                var pinButton = new BeepButton
                {
                    IsChild = true,
                    Dock = DockStyle.Right,
                    Width = 30,
                    UseThemeColors = true,
                    Text = IsPinnedBackstageItem(action) ? "★" : "☆",
                    Tag = action,
                    TextAlign = ContentAlignment.MiddleCenter,
                    AccessibleName = $"Pin {GetDisplayText(action)}",
                    AccessibleDescription = "Toggle pinned state for this backstage item"
                };
                pinButton.Click += BackstageInlinePinButton_Click;
                RibbonAccessibilityHelper.ApplyControlAccessibility(
                    pinButton,
                    pinButton.AccessibleName,
                    pinButton.AccessibleDescription,
                    AccessibleRole.CheckButton);
                row.Controls.Add(pinButton);
            }

            row.Controls.Add(button);

            return row;
        }

        private void BackstageInlinePinButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button pinButton || pinButton.Tag is not SimpleItem action)
            {
                return;
            }

            string key = GetMergeKey(action);
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            if (IsPinnedBackstageItem(action))
            {
                UnpinBackstageItem(key, moveToRecent: true);
            }
            else
            {
                PinBackstageItem(action);
            }

            if (_activeBackstageIndex >= 0)
            {
                ShowBackstageSection(_activeBackstageIndex);
            }
        }

        private List<SimpleItem> GetBackstageActions(SimpleItem section)
        {
            var actions = section.Children.Where(IsVisibleNode).ToList();
            string sectionName = GetDisplayText(section);

            bool includeRecent = sectionName.Equals("Open", StringComparison.OrdinalIgnoreCase) ||
                                 sectionName.Equals("Recent", StringComparison.OrdinalIgnoreCase) ||
                                 sectionName.Contains("Recent", StringComparison.OrdinalIgnoreCase);

            if (!includeRecent)
            {
                return actions;
            }

            if (_backstagePinnedItems.Count > 0)
            {
                if (actions.Count > 0) actions.Add(new SimpleItem { IsSeparator = true });
                actions.Add(new SimpleItem { Text = "Pinned", IsEnabled = false, IsVisible = true });
                actions.AddRange(_backstagePinnedItems.Where(IsVisibleNode));
            }

            if (_backstageRecentItems.Count > 0)
            {
                if (actions.Count > 0) actions.Add(new SimpleItem { IsSeparator = true });
                actions.Add(new SimpleItem { Text = "Recent", IsEnabled = false, IsVisible = true });
                actions.AddRange(_backstageRecentItems.Where(IsVisibleNode));
            }

            return actions;
        }

        private void BackstageActions_SizeChanged(object? sender, EventArgs e)
        {
            int width = Math.Max(120, _backstageActions.ClientSize.Width - 30);
            foreach (var panel in _backstageActions.Controls.OfType<Panel>())
            {
                panel.Width = width;
            }

            foreach (var separator in _backstageActions.Controls.OfType<Label>())
            {
                separator.Width = Math.Max(80, _backstageActions.ClientSize.Width - 24);
            }

            foreach (var button in _backstageActions.Controls.OfType<Button>())
            {
                button.Width = width;
            }
        }

        private void BuildBackstageFooterActions()
        {
            ClearBackstageFooterActions();

            foreach (var item in _backstageFooterItems.Where(IsVisibleNode))
            {
                int width = Math.Clamp(TextRenderer.MeasureText(GetDisplayText(item), BeepThemesManager.ToFont(_theme.CommandTypography)).Width + 34, 96, 180);
                var button = new Button
                {
                    AutoSize = false,
                    Height = 30,
                    Width = width,
                    Margin = new Padding(4, 3, 4, 3),
                    Text = GetDisplayText(item),
                    TextAlign = ContentAlignment.MiddleCenter,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = _theme.TabActiveBack,
                    ForeColor = _theme.Text,
                    Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                    Tag = item
                };
                button.FlatAppearance.BorderColor = _theme.GroupBorder;
                button.FlatAppearance.MouseDownBackColor = _theme.GroupBack;
                button.FlatAppearance.MouseOverBackColor = _theme.HoverBack;
                RibbonAccessibilityHelper.ApplyControlAccessibility(
                    button,
                    GetDisplayText(item),
                    BuildToolTip(item),
                    AccessibleRole.PushButton);
                if (!string.IsNullOrWhiteSpace(item.ImagePath))
                {
                    var image = CreateBackstageImage(item.ImagePath);
                    if (image != null)
                    {
                        button.Image = image;
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        button.TextImageRelation = TextImageRelation.ImageBeforeText;
                        button.TextAlign = ContentAlignment.MiddleRight;
                        button.Padding = new Padding(6, 0, 6, 0);
                    }
                }
                button.Click += BackstageFooterButton_Click;
                _backstageFooter.Controls.Add(button);
            }
        }

        private void ClearBackstageFooterActions()
        {
            var controls = _backstageFooter.Controls.Cast<Control>().ToList();
            foreach (var control in controls)
            {
                if (control is Button button)
                {
                    button.Click -= BackstageFooterButton_Click;
                }
                control.Dispose();
            }
            _backstageFooter.Controls.Clear();
        }

        private void BackstageFooterButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button button || button.Tag is not SimpleItem action)
            {
                return;
            }

            BackstageCommandInvoked?.Invoke(this, new BackstageCommandInvokedEventArgs(_backstageFooterSection, action));
        }

        private void BackstageNavList_SelectedItemChanged(object? sender, SelectedItemChangedEventArgs e)
        {
            if (_backstageNavList.SelectedIndex < 0)
            {
                return;
            }

            ShowBackstageSection(_backstageNavList.SelectedIndex);
        }

        private void BackstageActionButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button button || button.Tag is not SimpleItem action) return;
            if (_activeBackstageIndex < 0 || !_backstageSectionMap.TryGetValue(_activeBackstageIndex, out var section)) return;
            if (IsBackstageSectionHeader(action)) return;

            if (action.Children.Count > 0)
            {
                var menu = new ContextMenuStrip();
                BuildBackstageChildMenu(menu.Items, section, action.Children);
                menu.Closed += (_, __) => menu.Dispose();
                var screenPoint = button.PointToScreen(new Point(button.Width - 2, button.Height));
                menu.Show(screenPoint);
                return;
            }

            BackstageCommandInvoked?.Invoke(this, new BackstageCommandInvokedEventArgs(section, action));
        }

        private void BackstageActionButton_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (sender is not Button button || button.Tag is not SimpleItem action) return;
            if (IsBackstageSectionHeader(action)) return;

            string key = GetMergeKey(action);
            bool isPinned = IsPinnedBackstageItem(action);
            bool isRecent = IsRecentBackstageItem(action);
            var menu = new ContextMenuStrip();
            menu.Font = BeepThemesManager.ToFont(_theme.CommandTypography);

            if (isPinned)
            {
                menu.Items.Add("Unpin from list", null, (_, __) => UnpinBackstageItem(key, moveToRecent: true));
            }
            else
            {
                menu.Items.Add("Pin to list", null, (_, __) => PinBackstageItem(action));
            }

            if (!isRecent)
            {
                menu.Items.Add("Add to recent", null, (_, __) => AddBackstageRecentItem(action));
            }
            else
            {
                menu.Items.Add("Remove from recent", null, (_, __) => RemoveBackstageItemByKey(_backstageRecentItems, key));
            }

            menu.Closed += (_, __) => menu.Dispose();
            menu.Show(button, e.Location);
        }

        private static bool IsBackstageSectionHeader(SimpleItem action)
        {
            if (action == null)
            {
                return false;
            }

            if (!action.IsEnabled && action.Children.Count == 0)
            {
                string text = GetDisplayText(action);
                return text.Equals("Pinned", StringComparison.OrdinalIgnoreCase) ||
                       text.Equals("Recent", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private bool IsPinnedBackstageItem(SimpleItem action)
        {
            string key = GetMergeKey(action);
            return FindBackstageItemIndexByKey(_backstagePinnedItems, key) >= 0;
        }

        private bool IsRecentBackstageItem(SimpleItem action)
        {
            string key = GetMergeKey(action);
            return FindBackstageItemIndexByKey(_backstageRecentItems, key) >= 0;
        }

        private static bool TryGetBackstageItemTimestamp(SimpleItem action, out DateTime openedUtc)
        {
            openedUtc = default;
            if (action == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(action.SubText3) &&
                DateTime.TryParse(action.SubText3, out var parsed))
            {
                openedUtc = parsed.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(parsed, DateTimeKind.Utc)
                    : parsed.ToUniversalTime();
                return true;
            }

            return false;
        }

        private string FormatBackstageTimestamp(SimpleItem action, DateTime openedUtc)
        {
            if (_backstageTimestampFormatter != null)
            {
                try
                {
                    string custom = _backstageTimestampFormatter.Invoke(action, openedUtc);
                    if (!string.IsNullOrWhiteSpace(custom))
                    {
                        return custom.Trim();
                    }
                }
                catch
                {
                }
            }

            if (!_backstageUseRelativeTimestamps)
            {
                return openedUtc.ToLocalTime().ToString("g");
            }

            return FormatRelativeTime(openedUtc);
        }

        private static string FormatRelativeTime(DateTime openedUtc)
        {
            var elapsed = DateTime.UtcNow - openedUtc;
            if (elapsed < TimeSpan.Zero) elapsed = TimeSpan.Zero;

            if (elapsed.TotalMinutes < 1) return "now";
            if (elapsed.TotalHours < 1) return $"{Math.Max(1, (int)elapsed.TotalMinutes)}m";
            if (elapsed.TotalDays < 1) return $"{Math.Max(1, (int)elapsed.TotalHours)}h";
            if (elapsed.TotalDays < 7) return $"{Math.Max(1, (int)elapsed.TotalDays)}d";
            if (elapsed.TotalDays < 30) return $"{Math.Max(1, (int)(elapsed.TotalDays / 7))}w";
            if (elapsed.TotalDays < 365) return $"{Math.Max(1, (int)(elapsed.TotalDays / 30))}mo";
            return $"{Math.Max(1, (int)(elapsed.TotalDays / 365))}y";
        }

        private void BuildBackstageChildMenu(ToolStripItemCollection parent, SimpleItem section, IEnumerable<SimpleItem> nodes)
        {
            foreach (var node in nodes.Where(IsVisibleNode))
            {
                if (node.IsSeparator)
                {
                    parent.Add(new ToolStripSeparator());
                    continue;
                }

                var item = new ToolStripMenuItem(GetDisplayText(node), CreateBackstageImage(node.ImagePath))
                {
                    Enabled = node.IsEnabled,
                    Checked = node.IsChecked,
                    CheckOnClick = node.IsCheckable,
                    Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                    ToolTipText = BuildToolTip(node)
                };
                item.Click += (_, __) => BackstageCommandInvoked?.Invoke(this, new BackstageCommandInvokedEventArgs(section, node));
                parent.Add(item);
                RibbonAccessibilityHelper.ApplyCommandAccessibility(item, node, GetDisplayText(node), AccessibleRole.MenuItem);

                if (node.Children.Count > 0)
                {
                    BuildBackstageChildMenu(item.DropDownItems, section, node.Children);
                }
            }
        }

        private Image? CreateBackstageImage(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return null;
            }

            int size = 16;
            var bmp = new Bitmap(size, size);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.Clear(Color.Transparent);
            using var clipPath = new GraphicsPath();
            clipPath.AddRectangle(new Rectangle(0, 0, size, size));
            StyledImagePainter.PaintWithTint(g, clipPath, imagePath, _theme.IconColor, 1f);
            _backstageGeneratedImages.Add(bmp);
            return bmp;
        }

        private void ClearBackstageActions()
        {
            var controls = _backstageActions.Controls.Cast<Control>().ToList();
            foreach (var control in controls)
            {
                foreach (var button in EnumerateButtons(control))
                {
                    button.Click -= BackstageActionButton_Click;
                    button.MouseUp -= BackstageActionButton_MouseUp;
                    button.Click -= BackstageInlinePinButton_Click;
                }
                control.Dispose();
            }
            _backstageActions.Controls.Clear();
            DisposeBackstageImages();
        }

        private static IEnumerable<Button> EnumerateButtons(Control root)
        {
            if (root is Button ownButton)
            {
                yield return ownButton;
            }

            foreach (Control child in root.Controls)
            {
                foreach (var nested in EnumerateButtons(child))
                {
                    yield return nested;
                }
            }
        }

        private void DisposeBackstageImages()
        {
            foreach (var image in _backstageGeneratedImages)
            {
                image.Dispose();
            }
            _backstageGeneratedImages.Clear();
        }

        private void BackstageButton_DropDownOpening(object? sender, EventArgs e)
        {
            BeginBackstageOpenTransition();
        }

        private void BackstageDropDown_Closed(object? sender, ToolStripDropDownClosedEventArgs e)
        {
            _backstageTransitionTimer.Stop();
            _backstageHost.Size = _backstagePanelContent.Size;
            _backstageDropDown.Size = _backstagePanelContent.Size;
        }

        private void BeginBackstageOpenTransition()
        {
            if (!ShouldAnimateTransitions() || !_enableBackstageTransitions)
            {
                var size = _backstagePanelContent.Size;
                _backstageHost.Size = size;
                _backstageDropDown.Size = size;
                return;
            }

            _backstageTransitionEffectiveDurationMs = GetEffectiveTransitionDurationMs(_backstageTransitionDurationMs, forBackstage: true);
            _backstageTransitionTimer.Interval = Math.Clamp(_backstageTransitionEffectiveDurationMs / 12, 10, 24);
            _backstageTransitionTimer.Stop();
            _backstageTransitionTargetSize = _backstagePanelContent.Size;
            float widthFactor = _density switch
            {
                RibbonDensity.Compact => 0.72f,
                RibbonDensity.Touch => 0.82f,
                _ => 0.76f
            };
            float heightFactor = _density switch
            {
                RibbonDensity.Compact => 0.76f,
                RibbonDensity.Touch => 0.84f,
                _ => 0.78f
            };
            if (_resolvedStylePreset == RibbonStylePreset.HighContrast)
            {
                widthFactor = 0.88f;
                heightFactor = 0.90f;
            }
            _backstageTransitionStartSize = new Size(
                Math.Max(360, (int)(_backstageTransitionTargetSize.Width * widthFactor)),
                Math.Max(220, (int)(_backstageTransitionTargetSize.Height * heightFactor)));

            _backstageHost.Size = _backstageTransitionStartSize;
            _backstageDropDown.Size = _backstageTransitionStartSize;
            _backstageTransitionStartUtc = DateTime.UtcNow;
            _backstageTransitionTimer.Start();
        }

        private void BackstageTransitionTimer_Tick(object? sender, EventArgs e)
        {
            double elapsed = (DateTime.UtcNow - _backstageTransitionStartUtc).TotalMilliseconds;
            double duration = Math.Max(1, _backstageTransitionEffectiveDurationMs);
            double t = Math.Clamp(elapsed / duration, 0, 1);
            // Smooth-step easing for subtle transition.
            double eased = t * t * (3 - 2 * t);

            int width = _backstageTransitionStartSize.Width +
                        (int)Math.Round((_backstageTransitionTargetSize.Width - _backstageTransitionStartSize.Width) * eased);
            int height = _backstageTransitionStartSize.Height +
                         (int)Math.Round((_backstageTransitionTargetSize.Height - _backstageTransitionStartSize.Height) * eased);

            var size = new Size(Math.Max(100, width), Math.Max(100, height));
            _backstageHost.Size = size;
            _backstageDropDown.Size = size;

            if (t >= 1)
            {
                _backstageTransitionTimer.Stop();
                _backstageHost.Size = _backstageTransitionTargetSize;
                _backstageDropDown.Size = _backstageTransitionTargetSize;
            }
        }
    }
}
