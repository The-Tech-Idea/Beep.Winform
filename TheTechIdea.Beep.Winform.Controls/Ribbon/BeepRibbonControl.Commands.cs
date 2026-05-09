using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Accessibility;
using TheTechIdea.Beep.Winform.Controls.Gallery;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls.Tooltips;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        public void BuildFromSimpleItems()
        {
            BuildFromSimpleItems(_commandItems);
        }

        public void BuildFromSimpleItems(IEnumerable<SimpleItem>? tabNodes)
        {
            SuspendLayout();
            try
            {
                ClearRibbonTree();
                if (tabNodes == null)
                {
                    ApplyTheme();
                    ApplyMinimizedState();
                    return;
                }

                if (ReferenceEquals(tabNodes, _commandItems))
                {
                    EnsureCustomizationDefaultsCaptured();
                }

                if (ReferenceEquals(tabNodes, _commandItems) && _pendingCustomizationState != null)
                {
                    ApplyTabStates(_pendingCustomizationState.Tabs);
                    _pendingCustomizationState = null;
                }

                var tabList = tabNodes.Where(IsVisibleNode).ToList();
                RebuildCommandLookup(tabList);

                foreach (var tabNode in tabList)
                {
                    var page = AddPage(GetDisplayText(tabNode));
                    page.Tag = tabNode;

                    if (_layoutMode == RibbonLayoutMode.Simplified)
                    {
                        var mergedNodes = new List<SimpleItem>();
                        foreach (var groupNode in tabNode.Children.Where(IsVisibleNode))
                        {
                            if (mergedNodes.Count > 0)
                            {
                                mergedNodes.Add(new SimpleItem { IsSeparator = true });
                            }

                            mergedNodes.AddRange(groupNode.Children.Where(IsVisibleNode));
                        }

                        var mergedGroup = AddGroup(page, "Commands");
                        mergedGroup.Tag = tabNode;
                        _groupCommandNodes[mergedGroup] = mergedNodes;
                        BuildGroupCommands(mergedGroup, mergedNodes);
                    }
                    else
                    {
                        foreach (var groupNode in tabNode.Children.Where(IsVisibleNode))
                        {
                            var group = AddGroup(page, GetDisplayText(groupNode));
                            group.Tag = groupNode;
                            group.Density = _density;
                            group.ApplyTheme(_theme);
                            var commands = groupNode.Children.Where(IsVisibleNode).ToList();
                            _groupCommandNodes[group] = commands;
                            BuildGroupCommands(group, commands);
                        }
                    }
                }

                ApplyResponsiveLayout();
                RebuildQuickAccessToolbar();
                if (_searchMode != RibbonSearchMode.Off && !string.IsNullOrWhiteSpace(_searchBox.Text))
                {
                    RunLocalSearch(_searchBox.Text);
                }
                ApplyTheme();
                ApplyMinimizedState();
            }
            finally
            {
                ResumeLayout();
            }
        }

        private void BuildGroupCommands(BeepRibbonGroup group, IEnumerable<SimpleItem> commandNodes)
        {
            var commands = commandNodes.Where(IsVisibleNode).ToList();
            commands = NormalizeSeparators(commands);
            if (commands.Count == 0)
            {
                return;
            }

            bool useLargeButtons = DetermineLayoutSize(commands, group);
            int available = GetAvailableGroupWidth(group);
            int reservedOverflowWidth = EstimateOverflowButtonWidth();
            int used = 0;
            var overflow = new List<SimpleItem>();

            foreach (var command in commands)
            {
                int commandWidth = EstimateCommandWidth(command, useLargeButtons);
                bool fits = used + commandWidth <= available;
                bool reserveOverflow = used + commandWidth <= available - reservedOverflowWidth;

                if (!fits || (_layoutMode == RibbonLayoutMode.Simplified && !reserveOverflow))
                {
                    overflow.Add(command);
                    continue;
                }

                AddCommandToGroup(group, command, useLargeButtons);
                used += commandWidth;
            }

            if (overflow.Count > 0)
            {
                var overflowCommands = NormalizeSeparators(overflow);
                if (overflowCommands.Count > 0)
                {
                    var overflowButton = CreateOverflowButton(overflowCommands);
                    group.Items.Add(overflowButton);
                }
            }
        }

        private void AddCommandToGroup(BeepRibbonGroup group, SimpleItem command, bool useLargeButtons)
        {
            if (command.IsSeparator)
            {
                group.Items.Add(new ToolStripSeparator());
                return;
            }

            if (IsGalleryCommand(command))
            {
                AddGalleryToGroup(group, command, useLargeButtons);
                return;
            }

            if (command.Children.Count > 0)
            {
                var dropdown = CreateDropDownButton(command, useLargeButtons);
                group.Items.Add(dropdown);
                return;
            }

            AddCommandButton(group, command, useLargeButtons);
        }

        private bool IsGalleryCommand(SimpleItem command)
        {
            if (command.Children.Count < 2)
            {
                return false;
            }

            static bool ContainsGalleryToken(string? value)
            {
                return !string.IsNullOrWhiteSpace(value) &&
                       value.Contains("gallery", StringComparison.OrdinalIgnoreCase);
            }

            return ContainsGalleryToken(command.Text) ||
                   ContainsGalleryToken(command.DisplayField) ||
                   ContainsGalleryToken(command.Name) ||
                   ContainsGalleryToken(command.ToolTip) ||
                   ContainsGalleryToken(command.ItemType.ToString());
        }

        private void AddGalleryToGroup(BeepRibbonGroup group, SimpleItem command, bool useLargeButtons)
        {
            string galleryKey = GetCommandKey(command);
            var gallery = new BeepRibbonGallery
            {
                Compact = !useLargeButtons,
                EnableCategoryHeaders = true,
                EnableLargePreviewPopup = true,
                Width = EstimateGalleryWidth(command, useLargeButtons),
                Height = Math.Max(28, GetGroupHeight() - 6),
                Margin = new Padding(0),
                TabStop = true
            };

            gallery.ApplyTheme(_theme, _density);
            gallery.RightToLeft = _ribbonRightToLeft ? RightToLeft.Yes : RightToLeft.No;
            RibbonAccessibilityHelper.ApplyControlAccessibility(
                gallery,
                $"{GetDisplayText(command)} gallery",
                BuildToolTip(command),
                AccessibleRole.List);
            gallery.SetItems(GetGalleryItems(command));
            gallery.SetSelected(GetGallerySelectedItem(command));
            if (_galleryPinnedKeys.TryGetValue(galleryKey, out var pinnedKeys))
            {
                gallery.SetPinnedKeys(pinnedKeys);
            }
            else
            {
                gallery.SetPinnedKeys(GetGalleryPinnedKeysFromMetadata(command));
            }

            if (_galleryRecentKeys.TryGetValue(galleryKey, out var recentKeys))
            {
                gallery.SetRecentKeys(recentKeys);
            }
            else
            {
                gallery.SetRecentKeys(GetGalleryRecentKeysFromMetadata(command));
            }

            var host = new ToolStripControlHost(gallery)
            {
                AutoSize = false,
                Width = gallery.Width,
                Height = Math.Max(30, GetGroupHeight() - 2),
                Margin = new Padding(1),
                Padding = Padding.Empty
            };

            gallery.ItemSelected += (_, e) =>
            {
                _galleryLastSelection[galleryKey] = GetCommandKey(e.Item);
                RecordSearchCommandUsage(e.Item);
                RaiseCommandInvoked(e.Item, host);
            };
            gallery.StateChanged += (_, e) =>
            {
                _galleryPinnedKeys[galleryKey] = e.PinnedKeys
                    .Where(k => !string.IsNullOrWhiteSpace(k))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
                _galleryRecentKeys[galleryKey] = e.RecentKeys
                    .Where(k => !string.IsNullOrWhiteSpace(k))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Take(12)
                    .ToList();
            };

            ConfigureCommandItem(host, command);
            group.Items.Add(host);
            _commandMap[host] = command;
        }

        private IEnumerable<SimpleItem> GetGalleryItems(SimpleItem command)
        {
            var items = command.Children.Where(IsVisibleNode).ToList();
            if (items.Count <= 1)
            {
                return items;
            }

            string galleryKey = GetCommandKey(command);
            if (!_galleryLastSelection.TryGetValue(galleryKey, out var selectedKey) ||
                string.IsNullOrWhiteSpace(selectedKey))
            {
                return items;
            }

            int index = items.FindIndex(i => GetCommandKey(i).Equals(selectedKey, StringComparison.OrdinalIgnoreCase));
            if (index <= 0)
            {
                return items;
            }

            var selected = items[index];
            items.RemoveAt(index);
            items.Insert(0, selected);
            return items;
        }

        private SimpleItem? GetGallerySelectedItem(SimpleItem command)
        {
            string galleryKey = GetCommandKey(command);
            if (!_galleryLastSelection.TryGetValue(galleryKey, out var selectedKey))
            {
                return null;
            }

            return command.Children.FirstOrDefault(c =>
                GetCommandKey(c).Equals(selectedKey, StringComparison.OrdinalIgnoreCase));
        }

        private static IEnumerable<string> GetGalleryPinnedKeysFromMetadata(SimpleItem command)
        {
            return command.Children
                .Where(c =>
                    c.IsChecked ||
                    (!string.IsNullOrWhiteSpace(c.BadgeText) &&
                     c.BadgeText.Contains("pin", StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(c.SubText3) &&
                     c.SubText3.Contains("pin", StringComparison.OrdinalIgnoreCase)))
                .Select(GetCommandKey)
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }

        private static IEnumerable<string> GetGalleryRecentKeysFromMetadata(SimpleItem command)
        {
            return command.Children
                .Where(c =>
                    (!string.IsNullOrWhiteSpace(c.BadgeText) &&
                     c.BadgeText.Contains("recent", StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(c.SubText2) &&
                     c.SubText2.Contains("recent", StringComparison.OrdinalIgnoreCase)))
                .Select(GetCommandKey)
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(10);
        }

        private void AddCommandButton(BeepRibbonGroup group, SimpleItem command, bool useLargeButtons)
        {
            string text = GetDisplayText(command);
            Image? image = CreateCommandImage(command.ImagePath, !useLargeButtons);
            ToolStripButton button = useLargeButtons
                ? group.AddLargeButton(text, image)
                : group.AddSmallButton(text, image);

            ConfigureCommandItem(button, command);
            button.CheckOnClick = command.IsCheckable;
            button.Checked = command.IsChecked;
            button.Click += (_, __) => RaiseCommandInvoked(command, button);
            _commandMap[button] = command;
        }

        private ToolStripDropDownButton CreateDropDownButton(SimpleItem command, bool useLargeButtons)
        {
            Image? image = CreateCommandImage(command.ImagePath, !useLargeButtons);
            var button = new ToolStripDropDownButton(GetDisplayText(command), image)
            {
                ImageScaling = useLargeButtons ? ToolStripItemImageScaling.None : ToolStripItemImageScaling.SizeToFit,
                TextImageRelation = useLargeButtons ? TextImageRelation.ImageAboveText : TextImageRelation.ImageBeforeText,
                AutoSize = !useLargeButtons,
                Width = useLargeButtons ? GetLargeItemWidth() : 0,
                Height = GetGroupHeight(),
                Font = BeepThemesManager.ToFont(_theme.CommandTypography)
            };

            ConfigureCommandItem(button, command);
            BuildDropDownMenu(button.DropDownItems, command.Children);
            _commandMap[button] = command;
            return button;
        }

        private ToolStripDropDownButton CreateOverflowButton(IEnumerable<SimpleItem> overflowNodes)
        {
            var button = new ToolStripDropDownButton("More")
            {
                AutoSize = true,
                Height = GetGroupHeight(),
                Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                ForeColor = _theme.Text
            };
            BuildDropDownMenu(button.DropDownItems, overflowNodes);
            return button;
        }

        private void BuildDropDownMenu(ToolStripItemCollection parent, IEnumerable<SimpleItem> nodes)
        {
            foreach (var node in nodes.Where(IsVisibleNode))
            {
                if (node.IsSeparator)
                {
                    parent.Add(new ToolStripSeparator());
                    continue;
                }

                var item = new ToolStripMenuItem(GetDisplayText(node), CreateCommandImage(node.ImagePath, true))
                {
                    Enabled = node.IsEnabled,
                    Checked = node.IsChecked,
                    CheckOnClick = node.IsCheckable,
                    Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                };
                ConfigureCommandItem(item, node);
                item.Click += (_, __) => RaiseCommandInvoked(node, item);
                parent.Add(item);
                _commandMap[item] = node;

                if (node.Children.Count > 0)
                {
                    BuildDropDownMenu(item.DropDownItems, node.Children);
                }
            }
        }

        private void ConfigureCommandItem(ToolStripItem item, SimpleItem command)
        {
            item.Enabled = command.IsEnabled;
            item.Visible = command.IsVisible;
            item.ToolTipText = BuildToolTip(command);
            item.ForeColor = _theme.Text;
            var role = RibbonAccessibilityHelper.GetCommandRole(command, item);
            RibbonAccessibilityHelper.ApplyCommandAccessibility(item, command, GetDisplayText(command), role);
            item.Tag = command;
            if (item is ToolStripControlHost host && host.Control != null)
            {
                RibbonAccessibilityHelper.ApplyControlAccessibility(
                    host.Control,
                    GetDisplayText(command),
                    BuildToolTip(command),
                    AccessibleRole.Grouping);
                host.Control.TabStop = true;
            }

            if (_useSuperToolTips)
            {
                item.MouseHover -= CommandItem_MouseHover;
                item.MouseHover += CommandItem_MouseHover;
                item.MouseLeave -= CommandItem_MouseLeave;
                item.MouseLeave += CommandItem_MouseLeave;
            }
            else
            {
                item.MouseHover -= CommandItem_MouseHover;
                item.MouseLeave -= CommandItem_MouseLeave;
            }

            if ((_personalizationOptions & RibbonPersonalizationOptions.QuickAccess) != 0)
            {
                item.MouseUp -= CommandItem_MouseUp;
                item.MouseUp += CommandItem_MouseUp;
            }
        }

        private static string BuildToolTip(SimpleItem command)
        {
            if (string.IsNullOrWhiteSpace(command.ShortcutText))
            {
                return command.ToolTip;
            }

            if (string.IsNullOrWhiteSpace(command.ToolTip))
            {
                return command.ShortcutText;
            }

            return $"{command.ToolTip} ({command.ShortcutText})";
        }

        private RibbonSuperTooltipModel BuildSuperTooltipModel(SimpleItem command)
        {
            var model = _superTooltipModelProvider?.Invoke(command) ?? RibbonSuperTooltipModel.FromSimpleItem(command);
            if (string.IsNullOrWhiteSpace(model.Description))
            {
                model.Description = BuildToolTip(command);
            }
            return model;
        }

        private void CommandItem_MouseHover(object? sender, EventArgs e)
        {
            if (!_useSuperToolTips)
            {
                return;
            }

            if (sender is not ToolStripItem item)
            {
                return;
            }

            if (!_commandMap.TryGetValue(item, out var command))
            {
                if (item.Tag is not SimpleItem taggedCommand)
                {
                    return;
                }
                command = taggedCommand;
            }

            var owner = item.Owner;
            if (owner == null)
            {
                return;
            }

            var model = BuildSuperTooltipModel(command);
            if (model.IsEmpty)
            {
                return;
            }

            _hoveredTooltipCommand = command;
            _hoveredTooltipModel = model;
            int x = Math.Max(0, item.Bounds.Left + 2);
            int y = Math.Max(0, item.Bounds.Bottom + 2);
            _superTooltip.Show(owner, new Point(x, y), model, _superTooltipDurationMs);
        }

        private void CommandItem_MouseLeave(object? sender, EventArgs e)
        {
            if (sender is not ToolStripItem item)
            {
                return;
            }

            _hoveredTooltipCommand = null;
            _hoveredTooltipModel = null;
            if (item.Owner != null)
            {
                _superTooltip.Hide(item.Owner);
            }
        }

        private void RaiseCommandInvoked(SimpleItem command, ToolStripItem source)
        {
            HideSearchResultsDropDown();
            HideMinimizedPopup();
            HideKeyTips();
            CommandInvoked?.Invoke(this, new RibbonCommandInvokedEventArgs(command, source));
        }

        private void CommandItem_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if ((_personalizationOptions & RibbonPersonalizationOptions.QuickAccess) == 0) return;
            if (sender is not ToolStripItem item) return;
            if (!_commandMap.TryGetValue(item, out var command))
            {
                if (item.Tag is not SimpleItem taggedCommand) return;
                command = taggedCommand;
            }

            string commandKey = GetCommandKey(command);
            bool inQuickAccess = _quickAccessCommandKeys.Contains(commandKey, StringComparer.OrdinalIgnoreCase);
            var menu = new ContextMenuStrip();

            if (inQuickAccess)
            {
                menu.Items.Add("Remove from Quick Access Toolbar", null, (_, __) => RemoveCommandFromQuickAccess(commandKey));
            }
            else
            {
                menu.Items.Add("Add to Quick Access Toolbar", null, (_, __) => AddCommandToQuickAccess(commandKey));
            }

            if ((_personalizationOptions & (RibbonPersonalizationOptions.RibbonTabs | RibbonPersonalizationOptions.RibbonGroups | RibbonPersonalizationOptions.CommandOrder)) != 0)
            {
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(CreateCustomizeRibbonMenuItem());
            }

            menu.Closed += (_, __) => menu.Dispose();
            menu.Show(Cursor.Position);
        }
    }
}
