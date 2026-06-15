using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        /// <summary>
        /// Shows the minimized popup menu for the specified tab index.
        /// Displays all groups and commands for the selected tab when ribbon is minimized.
        /// </summary>
        private void ShowMinimizedPopupForTabIndex(int tabIndex)
        {
            if (!_isMinimized || tabIndex < 0 || tabIndex >= _tabStrip.Tabs.Count) return;
            HideMinimizedPopup();

            var tabNode = _tabStrip.Tabs[tabIndex].Tag as SimpleItem;
            if (tabNode == null) return;

            foreach (var groupNode in tabNode.Children.Where(IsVisibleNode))
            {
                var groupCommands = NormalizeSeparators(groupNode.Children.Where(IsVisibleNode));
                if (groupCommands.Count == 0)
                {
                    continue;
                }

                var groupItem = new ToolStripMenuItem(GetDisplayText(groupNode))
                {
                    Font = BeepThemesManager.ToFont(_theme.GroupTypography),
                    ForeColor = _theme.Text,
                    BackColor = _theme.GroupBack
                };

                BuildMinimizedPopupMenu(groupItem.DropDownItems, groupCommands);
                _minimizedTabPopup.Items.Add(groupItem);
            }

            if (_minimizedTabPopup.Items.Count == 0)
            {
                return;
            }

            _minimizedTabPopup.Renderer = new BeepRibbonToolStripRenderer(this);
            _minimizedTabPopup.Closed -= MinimizedTabPopup_Closed;
            _minimizedTabPopup.Closed += MinimizedTabPopup_Closed;

            var rect = _tabStrip.GetTabRect(tabIndex);
            _minimizedTabPopup.Show(_tabStrip, new Point(rect.Left, _tabStrip.Height));
        }

        /// <summary>
        /// Recursively builds the minimized popup menu from a collection of command nodes.
        /// </summary>
        private void BuildMinimizedPopupMenu(ToolStripItemCollection parent, IEnumerable<SimpleItem> nodes)
        {
            foreach (var node in nodes.Where(IsVisibleNode))
            {
                if (node.IsSeparator)
                {
                    parent.Add(new ToolStripSeparator());
                    continue;
                }

                var item = new ToolStripMenuItem(GetDisplayText(node), CreateTransientCommandImage(node.ImagePath, true))
                {
                    Enabled = node.IsEnabled,
                    Checked = node.IsChecked,
                    CheckOnClick = node.IsCheckable,
                    Font = BeepThemesManager.ToFont(_theme.CommandTypography),
                };

                ConfigureCommandItem(item, node);
                item.Click += (_, __) =>
                {
                    RaiseCommandInvoked(node, item);
                    HideMinimizedPopup();
                };
                parent.Add(item);
                _commandMap[item] = node;

                if (node.Children.Count > 0)
                {
                    BuildMinimizedPopupMenu(item.DropDownItems, node.Children);
                }
            }
        }

        /// <summary>
        /// Creates a transient command image for minimized popup rendering.
        /// </summary>
        private Image? CreateTransientCommandImage(string? imagePath, bool small)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return null;
            }

            int size = GetIconSize(small);
            var bmp = new Bitmap(size, size);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.Clear(Color.Transparent);
            using var clipPath = new GraphicsPath();
            clipPath.AddRectangle(new Rectangle(0, 0, size, size));
            StyledImagePainter.PaintWithTint(g, clipPath, imagePath, _theme.IconColor, 1f);
            _minimizedGeneratedImages.Add(bmp);
            return bmp;
        }

        /// <summary>
        /// Handles closure of the minimized popup menu.
        /// </summary>
        private void MinimizedTabPopup_Closed(object? sender, ToolStripDropDownClosedEventArgs e)
        {
            CleanupMinimizedPopup();
        }

        /// <summary>
        /// Hides the minimized popup and cleans up resources.
        /// </summary>
        private void HideMinimizedPopup()
        {
            if (_minimizedTabPopup.Visible)
            {
                _minimizedTabPopup.Close();
                return;
            }

            CleanupMinimizedPopup();
        }

        /// <summary>
        /// Cleans up minimized popup resources and command mappings.
        /// </summary>
        private void CleanupMinimizedPopup()
        {
            RemovePopupMappings(_minimizedTabPopup.Items);
            _minimizedTabPopup.Items.Clear();
            DisposeMinimizedImages();
        }

        /// <summary>
        /// Removes command mappings from the given popup items collection.
        /// </summary>
        private void RemovePopupMappings(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                _commandMap.Remove(item);
                if (item is ToolStripDropDownItem dropDownItem && dropDownItem.DropDownItems.Count > 0)
                {
                    RemovePopupMappings(dropDownItem.DropDownItems);
                }
            }
        }

        /// <summary>
        /// Disposes all transient images created for the minimized popup.
        /// </summary>
        private void DisposeMinimizedImages()
        {
            foreach (var image in _minimizedGeneratedImages)
            {
                image.Dispose();
            }
            _minimizedGeneratedImages.Clear();
        }

        /// <summary>
        /// Calculates the height of the ribbon when in minimized state.
        /// </summary>
        private int CalculateMinimizedHeight()
        {
            int qatHeight = Math.Max(_quickAccess.Height, _quickAccess.PreferredSize.Height);
            return qatHeight + _contextHeader.Height + _tabStrip.Height + 4;
        }

        /// <summary>
        /// Applies the minimized/expanded state to the ribbon control.
        /// Adjusts height and manages the popup visibility accordingly.
        /// </summary>
        private void ApplyMinimizedState()
        {
            if (_isMinimized)
            {
                _expandedRibbonHeight = Math.Max(_expandedRibbonHeight, Height);
                Height = CalculateMinimizedHeight();
            }
            else
            {
                HideMinimizedPopup();
                int minimumExpandedHeight = CalculateMinimizedHeight() + 18;
                if (_expandedRibbonHeight < minimumExpandedHeight)
                {
                    _expandedRibbonHeight = minimumExpandedHeight;
                }
                Height = _expandedRibbonHeight;
            }

            _tabStrip.Invalidate();
            _contextHeader.Invalidate();
        }

        /// <summary>
        /// Toggles the minimized state of the ribbon if allowed by <see cref="AllowMinimize"/>.
        /// </summary>
        public void ToggleMinimized()
        {
            if (!_allowMinimize) return;
            IsMinimized = !IsMinimized;
        }
    }
}
