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

        public BeepRibbonGroup()
        {
            GripStyle = ToolStripGripStyle.Hidden;
            Dock = DockStyle.Top;
            RenderMode = ToolStripRenderMode.ManagerRenderMode;
            Stretch = true;
            AutoSize = false;
            Height = 48;
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
            Height = _density switch
            {
                RibbonDensity.Compact => 40,
                RibbonDensity.Touch => 56,
                _ => 48
            };

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

            Padding = new Padding(itemSpacing, verticalPadding, itemSpacing, verticalPadding);

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
    }
}
