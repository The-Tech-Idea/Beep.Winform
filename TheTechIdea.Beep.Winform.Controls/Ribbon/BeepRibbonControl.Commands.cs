using TheTechIdea.Beep.Winform.Controls.Accessibility;
using TheTechIdea.Beep.Winform.Controls.Gallery;
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

        /// <summary>
        /// Fills a group's column grid, sending whatever the tab has no room for to a drop-down.
        /// </summary>
        /// <remarks>
        /// The ToolStrip version computed a provisional group width from the commands, then asked
        /// whether the commands fitted inside that width — a circular test, and the reason nearly
        /// every group chose small buttons and then overflowed most of them anyway. Here the group's
        /// width is an *output*: items are placed into columns, the group measures what it needs, and
        /// only a real shortfall against the width the tab can still give this group causes overflow.
        /// </remarks>
        private void BuildGroupCommands(BeepRibbonGroup group, IEnumerable<SimpleItem> commandNodes)
        {
            var commands = NormalizeSeparators(commandNodes.Where(IsVisibleNode));

            ForgetGroupCommandControls(group);
            group.ClearItems();

            if (commands.Count == 0)
            {
                group.PerformGroupLayout();
                return;
            }

            int budget = GetGroupWidthBudget(group);
            var overflow = FillGroup(group, commands, budget);

            if (overflow.Count > 0)
            {
                // Re-fill with room reserved for the overflow affordance itself. Without this the
                // "More" button was appended past the group's own width and then swallowed by the
                // toolbar's own chevron - two overflow mechanisms, neither of them visible to the
                // command map, the key tips or the accessibility audit.
                ForgetGroupCommandControls(group);
                group.ClearItems();
                overflow = FillGroup(group, commands, budget - OverflowButtonWidth);

                var overflowCommands = NormalizeSeparators(overflow);
                if (overflowCommands.Count > 0)
                {
                    var overflowButton = CreateOverflowButton(group, overflowCommands);
                    group.AddItem(overflowButton, RibbonItemSize.Small);
                }
            }

            group.PerformGroupLayout();
        }

        /// <summary>
        /// Adds commands until one does not fit, then returns that command and everything after it.
        /// </summary>
        private List<SimpleItem> FillGroup(BeepRibbonGroup group, List<SimpleItem> commands, int budget)
        {
            var overflow = new List<SimpleItem>();
            bool placedFirst = false;

            for (int index = 0; index < commands.Count; index++)
            {
                var command = commands[index];

                if (overflow.Count > 0)
                {
                    overflow.Add(command);
                    continue;
                }

                AddCommandToGroup(group, command, ResolveItemSize(command, !placedFirst));
                if (!command.IsSeparator) placedFirst = true;

                // A group always shows at least its first command, however narrow the tab is: a group
                // that is nothing but a "More" button tells the user nothing about what is inside it.
                if (index == 0 || group.MeasureContentWidth() <= budget) continue;

                var removed = group.RemoveLastItem();
                if (removed != null) _controlCommandMap.Remove(removed);
                overflow.Add(command);
            }

            return overflow;
        }

        private void AddCommandToGroup(BeepRibbonGroup group, SimpleItem command, RibbonItemSize size)
        {
            if (command.IsSeparator)
            {
                group.AddSeparator();
                return;
            }

            if (IsGalleryCommand(command))
            {
                AddGalleryToGroup(group, command, size);
                return;
            }

            if (command.Children.Count > 0)
            {
                var dropdown = CreateDropDownButton(group, command, size);
                group.AddItem(dropdown, size);
                return;
            }

            AddCommandButton(group, command, size);
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

        private void AddGalleryToGroup(BeepRibbonGroup group, SimpleItem command, RibbonItemSize size)
        {
            bool large = size == RibbonItemSize.Large;
            string galleryKey = GetCommandKey(command);
            var gallery = new BeepRibbonGallery
            {
                Compact = !large,
                EnableCategoryHeaders = true,
                EnableLargePreviewPopup = true,
                Width = EstimateGalleryWidth(command, large),
                Height = group.ContentBandHeight,
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

            gallery.ItemSelected += (_, e) =>
            {
                _galleryLastSelection[galleryKey] = GetCommandKey(e.Item);
                RecordSearchCommandUsage(e.Item);
                RaiseCommandInvoked(e.Item, gallery);
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

            // No ToolStripControlHost. The wrapper existed only so a Control could live inside a
            // toolbar; in a panel the gallery is simply a child, and the host's undisposed lifetime
            // (Items.Clear() never disposed it) goes with it.
            ConfigureCommandControl(gallery, command, AccessibleRole.List);
            group.AddItem(gallery, size);
            _controlCommandMap[gallery] = command;
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

        private void AddCommandButton(BeepRibbonGroup group, SimpleItem command, RibbonItemSize size)
        {
            string text = GetDisplayText(command);

            // The icon path goes straight to the button. BeepImage inside BeepButton renders and themes
            // the SVG itself, so the per-rebuild rasterise-to-Bitmap-then-dispose cycle that
            // ToolStripItem.Image forced is gone for every command in a group.
            var button = size == RibbonItemSize.Large
                ? group.AddLargeButton(text, command.ImagePath)
                : group.AddSmallButton(text, command.ImagePath);

            ConfigureCommandControl(button, command, RibbonAccessibilityHelper.GetCommandRole(command));
            button.IsSelectedOptionOn = command.IsCheckable;
            button.IsSelected = command.IsChecked;
            button.Click += (_, _) =>
            {
                if (command.IsCheckable) command.IsChecked = button.IsSelected;
                RaiseCommandInvoked(command, button);
            };
            _controlCommandMap[button] = command;
        }

        private BeepRibbonCommandButton CreateDropDownButton(BeepRibbonGroup group, SimpleItem command, RibbonItemSize size)
        {
            var button = group.NewCommandButton(GetDisplayText(command), command.ImagePath, size);
            button.ShowDropDownArrow = true;
            button.DropDownMenu = CreateCommandDropDown(group, command.Children);

            ConfigureCommandControl(button, command, AccessibleRole.ButtonDropDown);
            _controlCommandMap[button] = command;
            return button;
        }

        /// <summary>
        /// The affordance that holds what the tab had no room for.
        /// </summary>
        /// <remarks>
        /// The old "More" button was never registered in the command map, never had
        /// <c>ConfigureCommandItem</c> called on it and never had a Tag - so it had no accessible name,
        /// no role, no super-tooltip and no quick-access right click. It is a real command control now,
        /// and its own drop-down carries the overflowed commands.
        /// </remarks>
        private BeepRibbonCommandButton CreateOverflowButton(BeepRibbonGroup group, IEnumerable<SimpleItem> overflowNodes)
        {
            var button = group.NewCommandButton("More", null, RibbonItemSize.Small);
            button.ShowDropDownArrow = true;
            button.DropDownMenu = CreateCommandDropDown(group, overflowNodes);
            RibbonAccessibilityHelper.ApplyControlAccessibility(
                button,
                $"More {group.Text} commands",
                "Commands that did not fit the ribbon",
                AccessibleRole.ButtonDropDown);
            return button;
        }

        private ToolStripDropDownMenu CreateCommandDropDown(BeepRibbonGroup group, IEnumerable<SimpleItem> nodes)
        {
            var menu = new ToolStripDropDownMenu
            {
                ShowImageMargin = true,
                AutoClose = true,
                Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                ForeColor = _theme.Text,
                BackColor = _theme.GroupBack,
                Renderer = new BeepRibbonToolStripRenderer(this)
            };

            BuildDropDownMenu(menu.Items, nodes);
            group.TrackPopup(menu);
            return menu;
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

        /// <summary>
        /// Applies a command's state, tooltip, accessibility and personalisation wiring to a menu or
        /// toolbar item. Still needed by the quick access toolbar, the drop-down menus and the
        /// minimized popup, all of which remain ToolStrip-based.
        /// </summary>
        private void ConfigureCommandItem(ToolStripItem item, SimpleItem command)
        {
            item.Enabled = command.IsEnabled;
            item.Visible = command.IsVisible;
            item.ToolTipText = BuildToolTip(command);
            item.ForeColor = _theme.Text;
            var role = RibbonAccessibilityHelper.GetCommandRole(command, item);
            RibbonAccessibilityHelper.ApplyCommandAccessibility(item, command, GetDisplayText(command), role);
            item.Tag = command;

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

        /// <summary>The same contract as <see cref="ConfigureCommandItem"/>, for a hosted control.</summary>
        private void ConfigureCommandControl(Control control, SimpleItem command, AccessibleRole role)
        {
            control.Enabled = command.IsEnabled;
            control.Visible = command.IsVisible;
            control.Tag = command;

            RibbonAccessibilityHelper.ApplyControlAccessibility(
                control,
                GetDisplayText(command),
                RibbonAccessibilityHelper.BuildCommandDescription(command),
                role);

            if (control is BeepButton button)
            {
                // Two tooltips for one command is one too many: BaseControl's own rich tooltip stands
                // down while the ribbon's super-tooltip is the thing that shows.
                button.ToolTipText = BuildToolTip(command);
                button.EnableTooltip = !_useSuperToolTips;
            }

            if (_useSuperToolTips)
            {
                control.MouseHover -= CommandControl_MouseHover;
                control.MouseHover += CommandControl_MouseHover;
                control.MouseLeave -= CommandControl_MouseLeave;
                control.MouseLeave += CommandControl_MouseLeave;
            }
            else
            {
                control.MouseHover -= CommandControl_MouseHover;
                control.MouseLeave -= CommandControl_MouseLeave;
            }

            if ((_personalizationOptions & RibbonPersonalizationOptions.QuickAccess) != 0)
            {
                control.MouseUp -= CommandControl_MouseUp;
                control.MouseUp += CommandControl_MouseUp;
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

            ShowSuperTooltip(command, owner, item.Bounds);
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

        private void CommandControl_MouseHover(object? sender, EventArgs e)
        {
            if (!_useSuperToolTips) return;
            if (sender is not Control control) return;
            if (!TryResolveCommand(control, out var command)) return;

            // ToolStripItem.Owner has no Control equivalent - the parent is the owner, and the item's
            // bounds are already relative to it.
            var owner = control.Parent;
            if (owner == null) return;

            ShowSuperTooltip(command, owner, control.Bounds);
        }

        private void CommandControl_MouseLeave(object? sender, EventArgs e)
        {
            if (sender is not Control control) return;

            _hoveredTooltipCommand = null;
            _hoveredTooltipModel = null;
            if (control.Parent != null)
            {
                _superTooltip.Hide(control.Parent);
            }
        }

        private void ShowSuperTooltip(SimpleItem command, Control owner, Rectangle bounds)
        {
            var model = BuildSuperTooltipModel(command);
            if (model.IsEmpty)
            {
                return;
            }

            _hoveredTooltipCommand = command;
            _hoveredTooltipModel = model;
            int x = Math.Max(0, bounds.Left + 2);
            int y = Math.Max(0, bounds.Bottom + 2);
            _superTooltip.Show(owner, new Point(x, y), model, _superTooltipDurationMs);
        }

        private bool TryResolveCommand(Control control, out SimpleItem command)
        {
            if (_controlCommandMap.TryGetValue(control, out var mapped))
            {
                command = mapped;
                return true;
            }

            if (control.Tag is SimpleItem tagged)
            {
                command = tagged;
                return true;
            }

            command = null!;
            return false;
        }

        private void RaiseCommandInvoked(SimpleItem command, object source)
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

            ShowCommandContextMenu(command);
        }

        private void CommandControl_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if ((_personalizationOptions & RibbonPersonalizationOptions.QuickAccess) == 0) return;
            if (sender is not Control control) return;
            if (!TryResolveCommand(control, out var command)) return;

            ShowCommandContextMenu(command);
        }

        private void ShowCommandContextMenu(SimpleItem command)
        {
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
