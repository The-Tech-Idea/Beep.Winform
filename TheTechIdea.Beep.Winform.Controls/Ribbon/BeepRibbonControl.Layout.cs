using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepRibbonControl
    {
        private bool ShouldRenderLargeButtons() => _layoutMode == RibbonLayoutMode.Classic && _density != RibbonDensity.Compact;

        private bool DetermineLayoutSize(IReadOnlyCollection<SimpleItem> commands, BeepRibbonGroup group)
        {
            if (_layoutMode == RibbonLayoutMode.Simplified) return false;
            if (!ShouldRenderLargeButtons()) return false;
            int available = GetAvailableGroupWidth(group);
            return commands.Sum(c => EstimateCommandWidth(c, true)) <= available || _density == RibbonDensity.Touch;
        }

        private int GetAvailableGroupWidth(BeepRibbonGroup group)
        {
            int width = group.DisplayRectangle.Width > 0 ? group.DisplayRectangle.Width : group.Width;
            return Math.Max(80, width - 12);
        }

        private int EstimateCommandWidth(SimpleItem command, bool useLargeButtons)
        {
            if (command.IsSeparator) return 10;
            if (useLargeButtons) return GetLargeItemWidth();
            string text = GetDisplayText(command);
            int textWidth = TextRenderer.MeasureText(text, BeepThemesManager.ToFont(_theme.CommandTypography)).Width;
            int iconWidth = string.IsNullOrWhiteSpace(command.ImagePath) ? 0 : GetIconSize(true) + 8;
            return Math.Max(52, textWidth + iconWidth + (command.Children.Count > 0 ? 14 : 0) + 18);
        }

        private int GetGroupHeight() => _density switch { RibbonDensity.Compact => 40, RibbonDensity.Touch => 56, _ => 48 };
        private int GetLargeItemWidth() => _density switch { RibbonDensity.Compact => 64, RibbonDensity.Touch => 84, _ => 72 };
        private int GetIconSize(bool small) => _density switch { RibbonDensity.Compact => small ? 14 : 16, RibbonDensity.Touch => small ? 18 : 22, _ => small ? 16 : 20 };

        private Image? CreateCommandImage(string? imagePath, bool small)
        {
            if (string.IsNullOrWhiteSpace(imagePath)) return null;
            int size = GetIconSize(small);
            var bmp = new Bitmap(size, size);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.Clear(Color.Transparent);
            using var clipPath = new GraphicsPath();
            clipPath.AddRectangle(new Rectangle(0, 0, size, size));
            StyledImagePainter.PaintWithTint(g, clipPath, imagePath, _theme.IconColor, 1f);
            _generatedImages.Add(bmp);
            return bmp;
        }

        private void ClearRibbonTree()
        {
            HideMinimizedPopup();
            _commandMap.Clear();
            _groupCommandNodes.Clear();
            _commandLookup.Clear();
            _tabStrip.Clear();
            _ribbonContentHost.Controls.Clear();
            DisposeGeneratedImages();
        }

        private void DisposeGeneratedImages()
        {
            foreach (var image in _generatedImages) image.Dispose();
            _generatedImages.Clear();
        }

        private void ApplyResponsiveLayout()
        {
            if (_isApplyingResponsiveLayout || _groupCommandNodes.Count == 0) return;
            _isApplyingResponsiveLayout = true;
            try
            {
                var groups = _groupCommandNodes.Keys.Where(g => !g.IsDisposed).ToList();
                foreach (var group in groups) group.Items.Clear();
                _commandMap.Clear();
                DisposeGeneratedImages();
                foreach (var group in groups)
                    if (_groupCommandNodes.TryGetValue(group, out var commands))
                        BuildGroupCommands(group, commands);
                RebuildQuickAccessToolbar();
            }
            finally { _isApplyingResponsiveLayout = false; }
        }

        private int EstimateOverflowButtonWidth() => 68;

        private int EstimateGalleryWidth(SimpleItem command, bool useLargeButtons)
        {
            int itemCount = command.Children.Count(c => !c.IsSeparator && c.IsVisible);
            itemCount = Math.Max(2, itemCount);
            int tileWidth = _density switch { RibbonDensity.Compact => useLargeButtons ? 88 : 68, RibbonDensity.Touch => useLargeButtons ? 110 : 92, _ => useLargeButtons ? 96 : 78 };
            int visibleTiles = useLargeButtons ? Math.Min(3, itemCount) : Math.Min(4, itemCount);
            return Math.Max(128, visibleTiles * tileWidth + 10);
        }

        public RibbonTab AddPage(string title)
        {
            var tab = _tabStrip.AddTab(title);
            tab.ContentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _theme.TabActiveBack,
                Visible = _tabStrip.Tabs.Count == 1
            };
            _ribbonContentHost.Controls.Add(tab.ContentPanel);
            return tab;
        }

        public BeepRibbonGroup AddGroup(RibbonTab tab, string title)
        {
            if (tab.ContentPanel == null)
            {
                tab.ContentPanel = new Panel { Dock = DockStyle.Fill, BackColor = _theme.TabActiveBack };
                _ribbonContentHost.Controls.Add(tab.ContentPanel);
            }
            var group = new BeepRibbonGroup { Text = title, Density = _density, Renderer = new BeepRibbonToolStripRenderer(this) };
            group.ApplyTheme(_theme);
            tab.ContentPanel.Controls.Add(group);
            return group;
        }
    }
}
