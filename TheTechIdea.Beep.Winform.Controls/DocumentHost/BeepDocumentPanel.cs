using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Tokens;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// A themed Panel that acts as the content area for one document inside a
    /// <see cref="BeepDocumentHost"/>.  Inherits <see cref="Panel"/> rather than
    /// BaseControl to avoid ContainerControl focus/WM_ERASEBKGND conflicts.
    /// </summary>
    [ToolboxItem(false)]   // Not intended as a stand-alone designer drop target
    [Description("Content panel hosting a single document within BeepDocumentHost.")]
    public class BeepDocumentPanel : Panel
    {
        // ─────────────────────────────────────────────────────────────────────
        // Fields
        // ─────────────────────────────────────────────────────────────────────

        private IBeepTheme _theme;
        private string _themeName = string.Empty;

        // ─────────────────────────────────────────────────────────────────────
        // Properties
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Unique identifier that matches its corresponding <see cref="BeepDocumentTab.Id"/>.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DocumentId { get; }

        private string _documentTitle = "Document";
        /// <summary>Human-readable title displayed in the tab for this panel.</summary>
        [DefaultValue("Document")]
        [Description("Title shown in the document tab.")]
        public string DocumentTitle
        {
            get => _documentTitle;
            set { _documentTitle = value ?? "Document"; }
        }

        private bool _isModified;
        /// <summary>When true, the tab strip shows a dirty indicator for this document.</summary>
        [DefaultValue(false)]
        [Description("Marks the document as having unsaved changes.")]
        public bool IsModified
        {
            get => _isModified;
            set { _isModified = value; ModifiedChanged?.Invoke(this, EventArgs.Empty); }
        }

        private bool _canClose = true;
        /// <summary>When false the close button is hidden in the tab.</summary>
        [DefaultValue(true)]
        [Description("Allows the user to close this document.")]
        public bool CanClose
        {
            get => _canClose;
            set { _canClose = value; }
        }

        private string? _iconPath;
        /// <summary>Path to an icon image displayed in the tab (same convention as BeepButton.ImagePath).</summary>
        [DefaultValue(null)]
        [Description("Path to the icon shown on the document tab.")]
        public string? IconPath
        {
            get => _iconPath;
            set { _iconPath = value; }
        }

        private string? _documentCategory;
        /// <summary>Optional category label used to group this document's tab in the overflow dropdown.</summary>
        [DefaultValue(null)]
        [Description("Category label for grouping tabs; shown as a separator in the overflow dropdown.")]
        public string? DocumentCategory
        {
            get => _documentCategory;
            set => _documentCategory = value;
        }

        private bool _showStatusBar;
        private string _statusLeft   = string.Empty;
        private string _statusCentre = string.Empty;
        private string _statusRight  = string.Empty;

        /// <summary>When true, a 22-pixel three-segment status bar is rendered at the bottom of the panel.</summary>
        [DefaultValue(false)]
        [Description("Show a themed tri-segment status bar at the bottom of the panel.")]
        public bool ShowStatusBar
        {
            get => _showStatusBar;
            set { _showStatusBar = value; Invalidate(); }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Status bar helpers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Sets the left-aligned segment text of the status bar and repaints.</summary>
        public void SetStatusLeft(string text)
        {
            _statusLeft = text ?? string.Empty;
            if (_showStatusBar) Invalidate();
        }

        /// <summary>Sets the centre-aligned segment text of the status bar and repaints.</summary>
        public void SetStatusCentre(string text)
        {
            _statusCentre = text ?? string.Empty;
            if (_showStatusBar) Invalidate();
        }

        /// <summary>
        /// Sets the right-aligned segment text of the status bar and repaints.
        /// Optionally prepend <paramref name="gitBranch"/> (e.g. "main") with a space separator.
        /// </summary>
        public void SetStatusRight(string text, string? gitBranch = null)
        {
            _statusRight = string.IsNullOrEmpty(gitBranch)
                ? (text ?? string.Empty)
                : $"{gitBranch}  {text ?? string.Empty}";
            if (_showStatusBar) Invalidate();
        }

        /// <summary>Name of the current Beep theme.</summary>
        [DefaultValue("")]
        [Description("Beep theme name applied to this control.")]
        public string ThemeName
        {
            get => _themeName;
            set
            {
                _themeName = value ?? string.Empty;
                _theme = BeepThemesManager.GetTheme(_themeName)
                         ?? BeepThemesManager.GetDefaultTheme();
                ApplyThemeColors();
                Invalidate();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Events
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Raised when <see cref="IsModified"/> changes.</summary>
        public event EventHandler? ModifiedChanged;

        // ─────────────────────────────────────────────────────────────────────
        // Constructor
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Creates a new panel with an auto-generated <see cref="DocumentId"/>.</summary>
        public BeepDocumentPanel() : this(Guid.NewGuid().ToString(), "Document") { }

        /// <summary>Creates a new panel with the specified id and title.</summary>
        public BeepDocumentPanel(string documentId, string title)
        {
            DocumentId = documentId ?? Guid.NewGuid().ToString();
            _documentTitle = title ?? "Document";

            // Theme
            _theme = BeepThemesManager.GetTheme(BeepThemesManager.CurrentThemeName)
                     ?? BeepThemesManager.GetDefaultTheme();
            _themeName = BeepThemesManager.CurrentThemeName ?? string.Empty;
            BeepThemesManager.ThemeChanged += OnGlobalThemeChanged;

            // Panel defaults
            AutoScroll = true;
            Dock = DockStyle.None;

            // Double buffer via reflection (Panel doesn't expose DoubleBuffered publicly)
            typeof(Panel).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
                ?.SetValue(this, true);

            ApplyThemeColors();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Theme wiring
        // ─────────────────────────────────────────────────────────────────────

        private void OnGlobalThemeChanged(object? sender, ThemeChangeEventArgs e)
        {
            ThemeName = e.NewThemeName;
        }

        private void ApplyThemeColors()
        {
            if (_theme == null) return;
            BackColor = _theme.BackgroundColor;
            ForeColor = _theme.ForeColor;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Painting
        // ─────────────────────────────────────────────────────────────────────

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);    // Panel handles child clipping; we overlay only decorations.

            if (_theme == null) return;

            var g = e.Graphics;

            // Optional tri-segment status bar at bottom
            if (_showStatusBar)
            {
                int barH = DocTokens.StatusBarHeight;   // 22 px logical
                int barY = Height - barH;

                // Background: primary colour shifted slightly darker for visual separation
                Color barBack = ShiftLightness(_theme.PrimaryColor, -0.10f);
                using (var barBr = new SolidBrush(barBack))
                    g.FillRectangle(barBr, 0, barY, Width, barH);

                // Text on status bar — contrast-adaptive colour
                Color barFore = ContrastColor(barBack);
                using var statusFont = new Font(Font.FontFamily, 8f, FontStyle.Regular, GraphicsUnit.Point);
                using var fgBr       = new SolidBrush(barFore);

                var baseFmt = new StringFormat { LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.NoWrap };

                const int hPad = 8;
                var barRectF = new RectangleF(hPad, barY, Width - hPad * 2, barH);

                if (!string.IsNullOrEmpty(_statusLeft))
                {
                    baseFmt.Alignment = StringAlignment.Near;
                    g.DrawString(_statusLeft, statusFont, fgBr, barRectF, baseFmt);
                }

                if (!string.IsNullOrEmpty(_statusCentre))
                {
                    baseFmt.Alignment = StringAlignment.Center;
                    g.DrawString(_statusCentre, statusFont, fgBr, barRectF, baseFmt);
                }

                if (!string.IsNullOrEmpty(_statusRight))
                {
                    baseFmt.Alignment = StringAlignment.Far;
                    g.DrawString(_statusRight, statusFont, fgBr, barRectF, baseFmt);
                }

                baseFmt.Dispose();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Colour helpers for status bar
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Shifts the lightness of <paramref name="c"/> by <paramref name="delta"/> (-1..1).</summary>
        private static Color ShiftLightness(Color c, float delta)
        {
            float r_ = c.R / 255f, g_ = c.G / 255f, b_ = c.B / 255f;
            float max = Math.Max(r_, Math.Max(g_, b_));
            float min = Math.Min(r_, Math.Min(g_, b_));
            float l = (max + min) / 2f;
            float s = 0f, h = 0f, d = max - min;
            if (d > 0f)
            {
                s = l > 0.5f ? d / (2f - max - min) : d / (max + min);
                if (max == r_)       h = (g_ - b_) / d + (g_ < b_ ? 6f : 0f);
                else if (max == g_)  h = (b_ - r_) / d + 2f;
                else                 h = (r_ - g_) / d + 4f;
                h /= 6f;
            }
            l = Math.Max(0f, Math.Min(1f, l + delta));
            if (s == 0f)
            {
                int v = (int)(l * 255f);
                return Color.FromArgb(c.A, v, v, v);
            }
            float q2 = l < 0.5f ? l * (1f + s) : l + s - l * s;
            float p2 = 2f * l - q2;
            return Color.FromArgb(c.A,
                (int)(HueChannel(p2, q2, h + 1f / 3f) * 255f),
                (int)(HueChannel(p2, q2, h)           * 255f),
                (int)(HueChannel(p2, q2, h - 1f / 3f) * 255f));
        }

        private static float HueChannel(float p, float q, float t)
        {
            if (t < 0f) t += 1f;
            if (t > 1f) t -= 1f;
            if (t < 1f / 6f) return p + (q - p) * 6f * t;
            if (t < 1f / 2f) return q;
            if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
            return p;
        }

        /// <summary>Returns Black or White whichever contrasts better against <paramref name="bg"/>.</summary>
        private static Color ContrastColor(Color bg)
        {
            double y = 0.2126 * bg.R + 0.7152 * bg.G + 0.0722 * bg.B;
            return y > 128.0 ? Color.Black : Color.White;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Dispose
        // ─────────────────────────────────────────────────────────────────────

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
            base.Dispose(disposing);
        }
    }
}
