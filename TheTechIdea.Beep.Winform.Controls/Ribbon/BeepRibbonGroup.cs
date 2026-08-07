using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepRibbonGroup : ToolStrip
    {
        private RibbonDensity _density = RibbonDensity.Comfortable;
        private RibbonTheme? _theme;
        private int _minTouchTargetWidth = 44;
        private bool _popupOpen;

        public event EventHandler? PopupOpened;
        public event EventHandler? PopupClosed;

        public RibbonDensity Density
        {
            get => _density;
            set
            {
                if (_density == value) return;
                _density = value;
                ApplyDensity();
            }
        }

        /// <summary>Height of the item area: three 22px rows, the standard ribbon group content.</summary>
        /// <remarks>
        /// From Fluent.Ribbon's group template, whose inter-group separator is 55px and whose recurring
        /// row unit is 22. The control previously used 40/48/56 by density — all below the 66px three
        /// rows need, which is why every command collapsed to a single line of text and most of them
        /// went to the overflow menu.
        /// </remarks>
        public const int ContentHeight = 66;

        /// <summary>Height of the caption strip beneath the items, carrying <see cref="Control.Text"/>.</summary>
        public const int CaptionHeight = 22;

        /// <summary>Item-area height for a density. One place, so nothing can disagree about it.</summary>
        internal static int ContentFor(RibbonDensity density) => density switch
        {
            RibbonDensity.Compact => 44,   // two rows
            RibbonDensity.Touch => 90,     // three roomier rows
            _ => ContentHeight,            // three 22px rows
        };

        public BeepRibbonGroup()
        {
            GripStyle = ToolStripGripStyle.Hidden;

            // Left, not Top. Groups sit side by side across a ribbon tab; docked Top they stacked
            // vertically, each stretched to the full width of the ribbon, so on a tab with three groups
            // only the first was inside the content panel's height and the rest were pushed off the
            // bottom entirely. Stretch has to go with it - it is what made each group span the whole
            // width - and the group now takes its width from its own items.
            Dock = DockStyle.Left;
            RenderMode = ToolStripRenderMode.ManagerRenderMode;
            Stretch = false;
            AutoSize = false;
            Height = ContentHeight + CaptionHeight;
            Width = 120;

            // Reserve the caption strip out of the item area, or the items lay out over the top of it.
            Padding = new Padding(3, 2, 3, CaptionHeight);
        }

        [System.ComponentModel.Category("Layout")]
        [System.ComponentModel.Description("Minimum touch target width in pixels")]
        [System.ComponentModel.DefaultValue(44)]
        public int MinTouchTargetWidth
        {
            get => _minTouchTargetWidth;
            set { _minTouchTargetWidth = System.Math.Max(32, value); ApplyMetrics(); }
        }

        [System.ComponentModel.Browsable(false)]
        public bool IsPopupOpen
        {
            get => _popupOpen;
            set
            {
                if (_popupOpen != value)
                {
                    _popupOpen = value;
                    if (value) OnPopupOpened(System.EventArgs.Empty);
                    else OnPopupClosed(System.EventArgs.Empty);
                }
            }
        }

        public void CloseChildPopup()
        {
            if (_popupOpen)
            {
                IsPopupOpen = false;
            }
        }

        protected virtual void OnPopupOpened(System.EventArgs e) => PopupOpened?.Invoke(this, e);
        protected virtual void OnPopupClosed(System.EventArgs e) => PopupClosed?.Invoke(this, e);

        public void ApplyTheme(RibbonTheme theme)
        {
            _theme = theme;
            BackColor = theme.GroupBack;
            ForeColor = theme.Text;
            Font = BeepThemesManager.ToFont(theme.CommandTypography);
            ApplyMetrics();
        }

        public ToolStripButton AddLargeButton(string text, Image? image = null)
        {
            var btn = new ToolStripButton(text, image)
            {
                ImageScaling = ToolStripItemImageScaling.None,
                TextImageRelation = TextImageRelation.ImageAboveText,
                AutoSize = false,
                Width = GetLargeButtonWidth(),
                Height = Height,
                Font = Font
            };
            Items.Add(btn);
            ApplyMetrics();
            return btn;
        }

        public ToolStripButton AddSmallButton(string text, Image? image = null)
        {
            var btn = new ToolStripButton(text, image)
            {
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                AutoSize = true,
                Font = Font
            };
            Items.Add(btn);
            ApplyMetrics();
            return btn;
        }

        private int GetLargeButtonWidth()
        {
            return _density switch
            {
                RibbonDensity.Compact => 64,
                RibbonDensity.Touch => 84,
                _ => 72
            };
        }

        private void ApplyDensity()
        {
            // Content plus the caption strip. These were the bare 40/48/56 content heights, which left
            // no room for the caption and silently undid what the constructor set.
            Height = ContentFor(_density) + CaptionHeight;

            foreach (ToolStripItem item in Items)
            {
                if (item is ToolStripButton button)
                {
                    button.Height = Height;
                    if (button.TextImageRelation == TextImageRelation.ImageAboveText)
                    {
                        button.Width = GetLargeButtonWidth();
                    }
                }
            }

            ApplyMetrics();
        }

        private void ApplyMetrics()
        {
            int itemSpacing = Math.Max(1, _theme?.ItemSpacing ?? 4);
            int groupSpacing = Math.Max(itemSpacing + 1, _theme?.GroupSpacing ?? 8);
            int verticalPadding = _density switch
            {
                RibbonDensity.Compact => 1,
                RibbonDensity.Touch => 4,
                _ => 2
            };

            // The bottom padding must keep reserving the caption strip. This assigned plain
            // verticalPadding, so every call to ApplyMetrics - which runs from ApplyTheme, from
            // ApplyDensity and from each Add*Button - quietly handed the caption's rows back to the
            // items, which then drew straight over the group title.
            Padding = new Padding(itemSpacing, verticalPadding, itemSpacing, verticalPadding + CaptionHeight);

            foreach (ToolStripItem item in Items)
            {
                if (item is ToolStripSeparator separator)
                {
                    int horizontal = Math.Max(2, groupSpacing / 2);
                    separator.Margin = new Padding(horizontal, Math.Max(2, verticalPadding + 2), horizontal, Math.Max(2, verticalPadding + 2));
                }
                else
                {
                    int horizontal = Math.Max(1, itemSpacing / 2);
                    item.Margin = new Padding(horizontal, 1, horizontal, 1);
                }
            }
        }

        /// <summary>
        /// Draws the group's caption strip and its right-hand separator.
        /// </summary>
        /// <remarks>
        /// <see cref="Control.Text"/> was set on every group by <c>AddGroup</c> and never rendered
        /// anywhere, so a ribbon tab showed three unlabelled boxes of commands. The caption is what
        /// makes a group read as a group, and the separator is what stops adjacent ones merging into
        /// one grey band.
        /// </remarks>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Width <= 0 || Height <= CaptionHeight) return;

            var g = e.Graphics;
            var captionRect = new Rectangle(0, Height - CaptionHeight, Width, CaptionHeight);

            Color fore = _theme?.DisabledText ?? SystemColors.GrayText;
            var font = BeepThemesManager.ToFont(_theme?.GroupTypography) ?? Font;

            TextRenderer.DrawText(g, Text ?? string.Empty, font, captionRect, fore,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);

            // Separator on the trailing edge, inset so it stops short of the caption baseline - the
            // same treatment Fluent.Ribbon gives its 55px inter-group separator.
            Color line = _theme?.GroupBorder ?? SystemColors.ControlDark;
            using var pen = new Pen(Color.FromArgb(90, line));
            g.DrawLine(pen, Width - 1, 4, Width - 1, Height - CaptionHeight - 2);
        }
    }
}
