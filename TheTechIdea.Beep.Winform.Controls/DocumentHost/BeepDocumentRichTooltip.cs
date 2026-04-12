// BeepDocumentRichTooltip.cs
// Borderless, topmost popup displayed when TabTooltipMode == Rich.
// Shows: bold title + modified badge, optional thumbnail with rounded corners,
// optional category label, optional body/file-path text, keyboard-hint footer.
// Themed via IBeepTheme; drop shadow via WS_EX_NOACTIVATE + CS_DROPSHADOW.
// ─────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DocumentHost.Tokens;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Owner-drawn borderless popup used as the "rich" tab tooltip.
    /// Show it with <see cref="ShowForTab"/> and close with <see cref="HidePopup"/>.
    /// </summary>
    internal sealed class BeepDocumentRichTooltip : Form
    {
        // ─────────────────────────────────────────────────────────────────────
        // Win32 constants for drop-shadow and no-activate
        // ─────────────────────────────────────────────────────────────────────
        private const int CS_DROPSHADOW   = 0x00020000;
        private const int WS_EX_NOACTIVATE = unchecked((int)0x08000000);
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        // ─────────────────────────────────────────────────────────────────────
        // Layout constants (logical pixels – scaled by DPI)
        // ─────────────────────────────────────────────────────────────────────
        private const int Pad        = 10;
        private const int ThumbW     = 200;
        private const int ThumbH     = 115;
        private const int MaxTextW   = 220;
        private const int BorderR    = 6;

        // ─────────────────────────────────────────────────────────────────────
        // State
        // ─────────────────────────────────────────────────────────────────────
        private string  _title       = string.Empty;
        private string  _body        = string.Empty;
        private string  _category    = string.Empty;  // e.g. "Source Files"
        private bool    _isModified;
        private Bitmap? _thumbnail;
        private IBeepTheme? _currentTheme;

        private const string FooterHint = "Ctrl+W to close \u00b7 Ctrl+click to pin";

        // ─────────────────────────────────────────────────────────────────────
        // Constructor
        // ─────────────────────────────────────────────────────────────────────
        public BeepDocumentRichTooltip()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.UserPaint, true);

            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar   = false;
            TopMost         = true;
            StartPosition   = FormStartPosition.Manual;
            BackColor       = Color.White;
            Size            = new Size(ScaleL(MaxTextW + Pad * 2), ScaleL(80));
        }

        // ─────────────────────────────────────────────────────────────────────
        // CreateParams – drop shadow + no-activate
        // ─────────────────────────────────────────────────────────────────────
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ClassStyle  |= CS_DROPSHADOW;
                cp.ExStyle     |= WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW;
                return cp;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Display the popup for the given tab near <paramref name="screenPt"/>.</summary>
        public void ShowForTab(BeepDocumentTab tab, Bitmap? thumbnail,
                               Point screenPt, IBeepTheme? theme)
        {
            _title      = tab.Title ?? string.Empty;
            _body       = tab.TooltipText ?? string.Empty;
            _category   = tab.DocumentCategory ?? string.Empty;
            _isModified = tab.IsModified;
            _thumbnail  = thumbnail;
            _currentTheme      = theme;

            RecalcSize();
            PositionNearPoint(screenPt);
            Invalidate();
            if (!Visible) Show();
        }

        /// <summary>Hide the popup without disposing it so it can be reused.</summary>
        public void HidePopup()
        {
            if (Visible) Hide();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Size calculation
        // ─────────────────────────────────────────────────────────────────────
        private void RecalcSize()
        {
            bool hasThumbnail = _thumbnail != null;
            bool hasBody      = !string.IsNullOrWhiteSpace(_body);
            bool hasCategory  = !string.IsNullOrWhiteSpace(_category);

            using Font titleFont  = new Font(Font.FontFamily, Font.Size + 1f, FontStyle.Bold,    Font.Unit);
            Font bodyFont   = Font;
            Font smallFont  = BeepFontManager.GetCachedFont(Font.FontFamily.Name, Font.SizeInPoints - 0.5f, FontStyle.Regular);
            using var g           = Graphics.FromHwnd(Handle);

            // Measure title (may append " ●" for modified)
            string displayTitle = _isModified ? _title + " ●" : _title;
            SizeF titleSize = g.MeasureString(displayTitle, titleFont, ScaleL(MaxTextW));

            // Measure body
            SizeF bodySize  = hasBody
                ? g.MeasureString(_body, bodyFont, ScaleL(MaxTextW))
                : SizeF.Empty;

            // Measure category badge
            SizeF catSize = hasCategory
                ? g.MeasureString(_category, smallFont, ScaleL(MaxTextW))
                : SizeF.Empty;

            // Footer
            SizeF footerSize = g.MeasureString(FooterHint, smallFont, ScaleL(MaxTextW));

            int textH = (int)Math.Ceiling(titleSize.Height)
                      + (hasBody     ? ScaleL(Pad / 2) + (int)Math.Ceiling(bodySize.Height)    : 0)
                      + (hasCategory ? ScaleL(Pad / 2) + (int)Math.Ceiling(catSize.Height)     : 0)
                      + ScaleL(Pad / 2) + (int)Math.Ceiling(footerSize.Height);

            int contentW = (int)Math.Max(titleSize.Width,
                           Math.Max(hasBody ? bodySize.Width : 0f,
                                    Math.Max(hasCategory ? catSize.Width : 0f, footerSize.Width)));

            int totalW, totalH;

            if (hasThumbnail)
            {
                totalW = ScaleL(Pad * 2) + ScaleL(ThumbW);
                int thumbH = ScaleL(ThumbH);
                int belowH = ScaleL(Pad) + textH + ScaleL(Pad);
                totalH     = thumbH + belowH;
                totalW = Math.Max(totalW, ScaleL(Pad * 2) + contentW);
            }
            else
            {
                totalW = ScaleL(Pad * 2) + Math.Max(contentW, ScaleL(120));
                totalH = ScaleL(Pad) + textH + ScaleL(Pad);
            }

            Size = new Size(totalW, totalH);
        }

        private void PositionNearPoint(Point screenPt)
        {
            // Prefer below-right; clamp to screen
            var screen  = Screen.FromPoint(screenPt).WorkingArea;
            int x       = screenPt.X + 12;
            int y       = screenPt.Y + 20;

            if (x + Width > screen.Right)   x = screenPt.X - Width - 4;
            if (y + Height > screen.Bottom) y = screenPt.Y - Height - 4;

            x = Math.Max(screen.Left, x);
            y = Math.Max(screen.Top,  y);

            Location = new Point(x, y);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Painting
        // ─────────────────────────────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Resolve colors
            Color back      = _currentTheme?.PanelBackColor     ?? Color.FromArgb(40, 44, 52);
            Color border    = _currentTheme?.BorderColor        ?? Color.FromArgb(80, 86, 100);
            Color titleFg   = _currentTheme?.ForeColor          ?? Color.White;
            Color bodyFg    = _currentTheme?.SecondaryTextColor ?? Color.FromArgb(180, 185, 200);
            Color footerFg  = Color.FromArgb(120, bodyFg.R, bodyFg.G, bodyFg.B);
            Color accentFg  = _currentTheme?.PrimaryColor       ?? Color.FromArgb(0, 122, 204);

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);

            // Background with rounded corners
            using (var path = RoundedRect(rect, ScaleL(BorderR)))
            using (var backBrush = new SolidBrush(back))
            {
                g.FillPath(backBrush, path);
                using var borderPen = new Pen(border);
                g.DrawPath(borderPen, path);
            }

            int y   = ScaleL(Pad);
            int pad = ScaleL(Pad);

            // Thumbnail with rounded corners
            if (_thumbnail != null)
            {
                int tw  = ScaleL(ThumbW);
                int th  = ScaleL(ThumbH);
                int tcr = ScaleL(DocTokens.ThumbnailCornerRadius);
                var thumbRect = new Rectangle(pad, pad, tw, th);
                using var clipPath = RoundedRect(thumbRect, tcr);
                g.SetClip(clipPath);
                g.DrawImage(_thumbnail, thumbRect);
                g.ResetClip();
                y = pad + th + ScaleL(Pad / 2);
            }

            using Font titleFont  = new Font(Font.FontFamily, Font.Size + 1f, FontStyle.Bold,    Font.Unit);
            Font smallFont  = BeepFontManager.GetCachedFont(Font.FontFamily.Name, Font.SizeInPoints - 0.5f, FontStyle.Regular);

            // Title + optional modified dot
            string displayTitle = _isModified ? _title + " ●" : _title;
            using (var titleBrush = new SolidBrush(titleFg))
            {
                var titleRect = new RectangleF(pad, y, Width - pad * 2, Font.Height * 2);
                g.DrawString(displayTitle, titleFont, titleBrush, titleRect);
                y += (int)g.MeasureString(displayTitle, titleFont, Width - pad * 2).Height + ScaleL(Pad / 4);
            }

            // Category badge
            if (!string.IsNullOrWhiteSpace(_category))
            {
                using var catBrush = new SolidBrush(accentFg);
                var catRect = new RectangleF(pad, y, Width - pad * 2, smallFont.Height + 2);
                g.DrawString(_category, smallFont, catBrush, catRect);
                y += (int)g.MeasureString(_category, smallFont, Width - pad * 2).Height + ScaleL(Pad / 4);
            }

            // Body / file-path text
            if (!string.IsNullOrWhiteSpace(_body))
            {
                using var bodyBrush = new SolidBrush(bodyFg);
                var bodyRect = new RectangleF(pad, y, Width - pad * 2, Height - y - pad - smallFont.Height - ScaleL(Pad / 2));
                g.DrawString(_body, Font, bodyBrush, bodyRect);
                y += (int)g.MeasureString(_body, Font, Width - pad * 2).Height + ScaleL(Pad / 4);
            }

            // Footer hint (always at the very bottom)
            using var footerBrush = new SolidBrush(footerFg);
            var footerRect = new RectangleF(pad, Height - pad - smallFont.Height, Width - pad * 2, smallFont.Height + ScaleL(2));
            using var footerFmt  = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far };
            g.DrawString(FooterHint, smallFont, footerBrush, footerRect, footerFmt);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────
        private static GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d    = radius * 2;
            path.AddArc(rect.X,                    rect.Y,                     d, d, 180, 90);
            path.AddArc(rect.Right - d,            rect.Y,                     d, d, 270, 90);
            path.AddArc(rect.Right - d,            rect.Bottom - d,            d, d,   0, 90);
            path.AddArc(rect.X,                    rect.Bottom - d,            d, d,  90, 90);
            path.CloseFigure();
            return path;
        }

        /// <summary>Scale a logical pixel value by the current DPI factor.</summary>
        private int ScaleL(int logical) =>
            (int)Math.Round(logical * (DeviceDpi / 96f));

        // ─────────────────────────────────────────────────────────────────────
        // Prevent the popup from stealing focus
        // ─────────────────────────────────────────────────────────────────────
        protected override bool ShowWithoutActivation => true;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            // Any mouse move over the popup itself closes it — keeps UX clean
            HidePopup();
        }
    }
}
