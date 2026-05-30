using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Painters
{
    /// <summary>
    /// Theme/metrics provider for the docking system. Extracts the palette, fonts, and layout
    /// metrics from the active Beep theme and exposes them to the docking element renderers
    /// (caption/tab/splitter/strip), which own all painting. This adapter contains no GDI drawing.
    /// </summary>
    public class DockingPainterAdapter : IDockingPainter
    {
        private Color _backgroundColor = Color.FromArgb(240, 240, 240);
        private Color _foregroundColor = Color.FromArgb(33, 33, 33);
        private Color _borderColor = Color.FromArgb(200, 200, 200);
        private Color _hoverColor = Color.FromArgb(245, 245, 245);
        private Color _selectedColor = Color.FromArgb(0, 122, 255);
        private Color _disabledColor = Color.FromArgb(150, 150, 150);

        // Optional theme-supplied fonts; fall back to shared BeepFontManager fonts when unset.
        private Font _uiFont;
        private Font _tabFont;

        private int _tabStripHeight = 30;
        private int _splitterWidth = 5;

        private bool _disposed;

        /// <summary>
        /// Creates an adapter with default colours. Call <see cref="UpdateFromTheme()"/> to apply
        /// the active Beep theme.
        /// </summary>
        public DockingPainterAdapter()
        {
        }

        /// <summary>
        /// Updates the adapter's colours and fonts from the active Beep theme. Call when created
        /// and whenever <c>BeepThemesManager.ThemeChanged</c> fires.
        /// </summary>
        public void UpdateFromTheme()
        {
            UpdateFromTheme(BeepThemesManager.CurrentTheme ?? BeepThemesManager.GetDefaultTheme());
        }

        /// <summary>
        /// Updates adapter colours and fonts from a specific Beep theme.
        /// </summary>
        public void UpdateFromTheme(IBeepTheme theme)
        {
            var colors = DockingThemeColors.FromTheme(theme, useThemeColors: true);
            _backgroundColor = colors.PanelBackColor;
            _foregroundColor = colors.PanelForeColor;
            _borderColor = colors.BorderColor;
            _hoverColor = colors.HoverBackColor;
            _selectedColor = colors.ActiveTabBackColor;
            _disabledColor = theme?.DisabledForeColor ?? _disabledColor;

            if (theme != null)
            {
                // Null results fall back to BeepFontManager via the property getters.
                UIFont = BeepThemesManager.ToFont(theme.BodyMedium);
                TabFont = BeepThemesManager.ToFont(theme.TabFont);
            }

            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Applies explicit theme colours to the adapter. Called by
        /// <see cref="BeepDockingManager.ApplyTheme(Color, Color, Color, Color, Color)"/> when the
        /// host application's theme changes (Krypton-style update-and-invalidate).
        /// </summary>
        public void ApplyTheme(Color background, Color foreground, Color border,
                               Color hover, Color accent)
        {
            _backgroundColor = background;
            _foregroundColor = foreground;
            _borderColor     = border;
            _hoverColor      = hover;
            _selectedColor   = accent;

            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raised whenever <see cref="ApplyTheme"/> or <see cref="UpdateFromTheme()"/> change colours.
        /// Docking surfaces subscribe and call Invalidate() to repaint.
        /// </summary>
        public event EventHandler ThemeChanged;

        #region Color Properties

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => _backgroundColor = value;
        }

        public Color ForegroundColor
        {
            get => _foregroundColor;
            set => _foregroundColor = value;
        }

        public Color BorderColor
        {
            get => _borderColor;
            set => _borderColor = value;
        }

        public Color HoverColor
        {
            get => _hoverColor;
            set => _hoverColor = value;
        }

        public Color SelectedColor
        {
            get => _selectedColor;
            set => _selectedColor = value;
        }

        public Color DisabledColor
        {
            get => _disabledColor;
            set => _disabledColor = value;
        }

        #endregion

        #region Font Properties

        public Font UIFont
        {
            get => _uiFont ?? BeepFontManager.DefaultFont;
            set => _uiFont = value;
        }

        public Font TabFont
        {
            get => _tabFont ?? BeepFontManager.DefaultFont;
            set => _tabFont = value;
        }

        #endregion

        #region Layout Properties

        public int TabStripHeight
        {
            get => _tabStripHeight;
            set => _tabStripHeight = Math.Max(20, value);
        }

        public int SplitterWidth
        {
            get => _splitterWidth;
            set => _splitterWidth = Math.Max(3, value);
        }

        #endregion

        #region Cache Management

        public void InvalidateCache()
        {
            // Palette/metrics holder only; nothing cached to invalidate.
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_disposed)
                return;

            // Fonts are shared (BeepFontManager) or theme-owned; do not dispose here.
            _disposed = true;
        }

        #endregion
    }
}
