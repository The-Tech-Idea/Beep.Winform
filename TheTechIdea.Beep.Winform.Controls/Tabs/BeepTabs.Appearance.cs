using System.Drawing;
using System.Collections.Generic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Painters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTabs
    {
        private readonly Dictionary<TabStyle, ITabPainter> _painters = new();

        private ITabPainter GetPainter(TabStyle style)
        {
            if (!_painters.TryGetValue(style, out ITabPainter painter))
            {
                painter = style switch
                {
                    TabStyle.Underline => new UnderlineTabPainter(this),
                    TabStyle.Capsule => new CapsuleTabPainter(this),
                    TabStyle.Minimal => new MinimalTabPainter(this),
                    TabStyle.Segmented => new SegmentedTabPainter(this),
                    TabStyle.Card => new CardTabPainter(this),
                    TabStyle.Button => new ButtonTabPainter(this),
                    _ => new ClassicTabPainter(this)
                };

                _painters[style] = painter;
            }

            return painter;
        }

        private void UpdatePainter()
        {
            _painter = GetPainter(_tabStyle);
            if (_painter is BaseTabPainter basePainter)
            {
                _painter.Theme = _currentTheme;
                basePainter.TextFont = _textFont;
            }
            else if (_painter != null)
            {
                _painter.Theme = _currentTheme;
            }
        }

        public bool ShouldShowTabText(int tabIndex)
        {
            int selectedIndex = GetHostedSourceSelectedIndex();
            return _tabTextVisibility switch
            {
                TabLabelVisibility.Always => true,
                TabLabelVisibility.SelectedOnly => tabIndex == selectedIndex,
                TabLabelVisibility.IconsOnly => false,
                TabLabelVisibility.Never => false,
                _ => true
            };
        }

        public virtual void ApplyTheme()
        {
            if (_currentTheme == null)
            {
                BackColor = Color.FromArgb(240, 240, 245);
                ForeColor = Color.FromArgb(33, 37, 41);
                return;
            }

            BackColor = TabThemeHelpers.GetTabControlBackgroundColor(_currentTheme, true);
            ForeColor = TabThemeHelpers.GetTabTextColor(_currentTheme, true);
           // Font = BeepThemesManager.ToFont(_currentTheme.TabFont);
            _textFont = TabFontHelpers.ResolveSafeFont(BeepThemesManager.ToFont(_currentTheme.TabFont), this);

            if (_painter is BaseTabPainter basePainter)
            {
                _painter.Theme = _currentTheme;
                basePainter.TextFont = _textFont;
            }
            else if (_painter != null)
            {
                _painter.Theme = _currentTheme;
            }

            ApplyThemeToHostedSource(_currentTheme, Theme);
            RefreshHeaderLayoutState();
        }
    }
}