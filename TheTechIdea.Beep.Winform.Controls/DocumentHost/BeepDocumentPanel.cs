using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Tokens;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    [ToolboxItem(false)]
    [Description("Content panel hosting a single document within BeepDocumentHost.")]
    public class BeepDocumentPanel : BaseControl
    {
        private string _documentTitle = "Document";
        private bool _isModified;
        private bool _canClose = true;
        private string? _iconPath;
        private string? _documentCategory;
        private bool _showStatusBar;
        private string _statusLeft = string.Empty;
        private string _statusCentre = string.Empty;
        private string _statusRight = string.Empty;
        // 5.2 — Lazy content loading
        private bool _isContentLoaded;

        public event EventHandler? ModifiedChanged;

        // ─────────────────────────────────────────────────────────────────────
        // 5.2 — Lazy content loading
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Gets whether <see cref="LoadContent"/> has been called at least once.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsContentLoaded => _isContentLoaded;

        /// <summary>
        /// Ensures content is loaded; calls <see cref="LoadContent"/> on the first
        /// activation.  Subsequent calls are no-ops until <see cref="UnloadContent"/>
        /// resets the flag.
        /// </summary>
        public void EnsureContentLoaded()
        {
            if (_isContentLoaded) return;
            _isContentLoaded = true;
            LoadContent();
        }

        /// <summary>
        /// Override in subclasses to perform deferred, one-time content initialisation.
        /// Called the first time the panel is activated via <see cref="EnsureContentLoaded"/>.
        /// The default implementation is empty.
        /// </summary>
        protected virtual void LoadContent() { }

        /// <summary>
        /// Unloads non-essential child controls to reclaim memory.  Override in
        /// subclasses to free expensive resources.  After unloading,
        /// <see cref="IsContentLoaded"/> is reset so the next activation reloads content
        /// via <see cref="LoadContent"/>.
        /// </summary>
        public virtual void UnloadContent()
        {
            _isContentLoaded = false;
            // Dispose and remove all child controls
            var toDispose = new System.Collections.Generic.List<Control>(Controls.Count);
            foreach (Control c in Controls) toDispose.Add(c);
            Controls.Clear();
            foreach (var c in toDispose) c.Dispose();
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DocumentId { get; }

        [DefaultValue("Document")]
        [Description("Title shown in the document tab.")]
        public string DocumentTitle
        {
            get => _documentTitle;
            set { _documentTitle = value ?? "Document"; }
        }

        [DefaultValue(false)]
        [Description("Marks the document as having unsaved changes.")]
        public bool IsModified
        {
            get => _isModified;
            set { _isModified = value; ModifiedChanged?.Invoke(this, EventArgs.Empty); }
        }

        [DefaultValue(true)]
        [Description("Allows the user to close this document.")]
        public bool CanClose
        {
            get => _canClose;
            set => _canClose = value;
        }

        [DefaultValue(null)]
        [Description("Path to the icon shown on the document tab.")]
        public string? IconPath
        {
            get => _iconPath;
            set => _iconPath = value;
        }

        [DefaultValue(null)]
        [Description("Category label for grouping tabs; shown as a separator in the overflow dropdown.")]
        public string? DocumentCategory
        {
            get => _documentCategory;
            set => _documentCategory = value;
        }

        [DefaultValue(false)]
        [Description("Show a themed tri-segment status bar at the bottom of the panel.")]
        public bool ShowStatusBar
        {
            get => _showStatusBar;
            set { _showStatusBar = value; Invalidate(); }
        }

        public void SetStatusLeft(string text)
        {
            _statusLeft = text ?? string.Empty;
            if (_showStatusBar) Invalidate();
        }

        public void SetStatusCentre(string text)
        {
            _statusCentre = text ?? string.Empty;
            if (_showStatusBar) Invalidate();
        }

        public void SetStatusRight(string text, string? gitBranch = null)
        {
            _statusRight = string.IsNullOrEmpty(gitBranch)
                ? (text ?? string.Empty)
                : $"{gitBranch}  {text ?? string.Empty}";
            if (_showStatusBar) Invalidate();
        }

        public BeepDocumentPanel() : this(Guid.NewGuid().ToString(), "Document") { }

        public BeepDocumentPanel(string documentId, string title)
        {
            DocumentId = documentId ?? Guid.NewGuid().ToString();
            _documentTitle = title ?? "Document";

            AutoScroll = true;
            Dock = DockStyle.None;
            DoubleBuffered = true;
        }

        protected override bool IsContainerControl => true;
        protected override bool AllowBaseControlClear => true;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (_currentTheme == null)
                _currentTheme = BeepThemesManager.GetTheme(BeepThemesManager.CurrentThemeName)
                    ?? BeepThemesManager.GetDefaultTheme();

            ApplyThemeColors();
        }

        private void ApplyThemeColors()
        {
            if (_currentTheme == null) return;
            BackColor = _currentTheme.BackgroundColor;
            ForeColor = _currentTheme.ForeColor;
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            ApplyThemeColors();
            Invalidate();
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);

            if (_currentTheme == null) return;

            if (_showStatusBar)
            {
                int barH = DpiScalingHelper.ScaleValue(DocTokens.StatusBarHeight, this);
                int barY = Height - barH;

                Color barBack = ShiftLightness(_currentTheme.PrimaryColor, -0.10f);
                using (var barBr = new SolidBrush(barBack))
                    g.FillRectangle(barBr, 0, barY, Width, barH);

                Color barFore = ContrastColor(barBack);
                var statusFont = BeepFontManager.GetCachedFont("Segoe UI", 8f, FontStyle.Regular);
                using var fgBr = new SolidBrush(barFore);

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
                if (max == r_) h = (g_ - b_) / d + (g_ < b_ ? 6f : 0f);
                else if (max == g_) h = (b_ - r_) / d + 2f;
                else h = (r_ - g_) / d + 4f;
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
                (int)(HueChannel(p2, q2, h) * 255f),
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

        private static Color ContrastColor(Color bg)
        {
            double y = 0.2126 * bg.R + 0.7152 * bg.G + 0.0722 * bg.B;
            return y > 128.0 ? Color.Black : Color.White;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                BeepThemesManager.ThemeChanged -= OnGlobalThemeChanged;
            base.Dispose(disposing);
        }

        private void OnGlobalThemeChanged(object? sender, ThemeChangeEventArgs e)
        {
            if (IsDisposed) return;
            Theme = e.NewThemeName;
        }
    }
}
