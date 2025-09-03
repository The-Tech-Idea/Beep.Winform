using System.ComponentModel;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm
    {
        private bool _showRibbonPlaceholder = false;
        private int _ribbonHeight = 80;
        private bool _showQuickAccess = true;
        private BeepRibbonControl? _ribbon;

        [Category("Beep Ribbon")]
        [DefaultValue(false)]
        public bool ShowRibbonPlaceholder
        {
            get => _showRibbonPlaceholder;
            set { _showRibbonPlaceholder = value; if (value) EnsureRibbon(); Invalidate(); }
        }

        [Browsable(false)]
        public BeepRibbonControl? Ribbon => _ribbon;

        [Category("Beep Ribbon")]
        [DefaultValue(80)]
        public int RibbonHeight
        {
            get => _ribbonHeight;
            set { _ribbonHeight = Math.Max(40, value); if (_ribbon != null) _ribbon.Height = _ribbonHeight; Invalidate(); }
        }

        [Category("Beep Ribbon")]
        [DefaultValue(true)]
        public bool ShowQuickAccess
        {
            get => _showQuickAccess;
            set { _showQuickAccess = value; if (_ribbon != null) _ribbon.QuickAccess.Visible = value; Invalidate(); }
        }

        private void EnsureRibbon()
        {
            if (_ribbon != null) return;
            _ribbon = new BeepRibbonControl { Left = 0, Top = _captionHeight, Width = Width, Height = _ribbonHeight, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            _ribbon.QuickAccess.Visible = _showQuickAccess;
            _ribbon.ApplyThemeFromBeep(_currentTheme);
            Controls.Add(_ribbon);
            _ribbon.BringToFront();
        }

        partial void InitializeRibbonFeature()
        {
            RegisterPaddingProvider((ref Padding p) => { if (_showRibbonPlaceholder) p.Top += _ribbonHeight; });
            RegisterOverlayPainter(Ribbon_OnPaintOverlay);
        }

        private void Ribbon_OnPaintOverlay(Graphics g)
        {
            if (!_showRibbonPlaceholder) return;
            EnsureRibbon();
        }
    }
}
