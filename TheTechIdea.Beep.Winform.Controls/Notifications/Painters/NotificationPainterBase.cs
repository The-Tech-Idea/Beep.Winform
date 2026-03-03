using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.FontManagement;

namespace TheTechIdea.Beep.Winform.Controls.Notifications.Painters
{
    /// <summary>
    /// Abstract base for all notification painters.
    /// <para>
    /// Fonts are sourced exclusively from <see cref="BeepFontManager.ToFont"/>;
    /// no inline <c>new Font(…)</c> is allowed.  All pixel values go through
    /// <see cref="DpiScalingHelper.ScaleValue"/> with the <see cref="OwnerControl"/>
    /// reference — never cached float fields.
    /// </para>
    /// </summary>
    public abstract class NotificationPainterBase : INotificationPainter
    {
        // ── Theme-sourced fonts (set via RefreshFonts) ────────────────────────
        // Never null after RefreshFonts; fallback is SystemFonts.DefaultFont.

        /// <summary>Font for the notification title.</summary>
        public Font TitleFont { get; protected set; }

        /// <summary>Font for the notification message body.</summary>
        public Font MessageFont { get; protected set; }

        /// <summary>Font for action buttons.</summary>
        public Font ButtonFont { get; protected set; }

        /// <summary>Font for captions / timestamps.</summary>
        public Font CaptionFont { get; protected set; }

        // ── Legacy compatibility shim (old code used TextFont) ────────────────
        /// <summary>Alias kept for callers that set TextFont directly (body font).</summary>
        public Font TextFont
        {
            get => MessageFont;
            set => MessageFont = value ?? SystemFonts.DefaultFont;
        }

        /// <summary>Owner WinForms control – used for DPI scaling throughout.</summary>
        public Control OwnerControl { get; set; }

        // ── Font refresh ──────────────────────────────────────────────────────

        /// <summary>
        /// Called by <c>BeepNotification.ApplyTheme()</c> whenever the theme changes.
        /// Sources all fonts from <see cref="BeepFontManager.ToFont"/>.
        /// </summary>
        public void RefreshFonts(IBeepTheme theme)
        {
            if (theme == null) return;

            TitleFont   = BeepFontManager.ToFont(theme.TitleSmall)   ?? SystemFonts.DefaultFont;
            MessageFont = BeepFontManager.ToFont(theme.BodyMedium)   ?? SystemFonts.DefaultFont;
            ButtonFont  = BeepFontManager.ToFont(theme.ButtonStyle)  ?? SystemFonts.DefaultFont;
            CaptionFont = BeepFontManager.ToFont(theme.CaptionStyle) ?? SystemFonts.DefaultFont;
        }

        // ── Abstract paint contract ───────────────────────────────────────────

        // Painters that override the full Draw method leave these as no-ops.
        public virtual void PaintBackground(Graphics g, Rectangle bounds, NotificationData data) { }
        public virtual void PaintIcon(Graphics g, Rectangle iconRect, NotificationData data) { }
        public virtual void PaintTitle(Graphics g, Rectangle titleRect, string title, NotificationData data) { }
        public virtual void PaintMessage(Graphics g, Rectangle messageRect, string message, NotificationData data) { }
        public virtual void PaintActions(Graphics g, Rectangle actionsRect, NotificationAction[] actions, NotificationData data) { }
        public virtual void PaintCloseButton(Graphics g, Rectangle closeRect, bool hovered, NotificationData data) { }
        public virtual void PaintProgressBar(Graphics g, Rectangle progressRect, float progress, NotificationData data) { }

        // ── Shared default implementations ────────────────────────────────────

        /// <summary>
        /// Default background: rounded rect, type colour, optional border.
        /// </summary>
        protected virtual void DrawBackground(Graphics g, Rectangle bounds,
            Color backColor, Color borderColor, int radius)
        {
            using var path = CreateRoundedPath(bounds, radius);
            using var bg   = new SolidBrush(backColor);
            g.FillPath(bg, path);
            if (borderColor != Color.Transparent)
            {
                using var pen = new Pen(borderColor, 1f);
                g.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Default title rendering using <see cref="TitleFont"/> (from theme).
        /// Uses <see cref="TextRenderer"/> for consistent ClearType output.
        /// </summary>
        protected virtual void DrawTitle(Graphics g, Rectangle rect, string title, Color color)
        {
            if (rect.IsEmpty || string.IsNullOrEmpty(title)) return;
            Font f = TitleFont ?? SystemFonts.DefaultFont;
            TextRenderer.DrawText(g, title, f, rect, color,
                TextFormatFlags.Left | TextFormatFlags.Top |
                TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
        }

        /// <summary>
        /// Default message rendering using <see cref="MessageFont"/> (from theme).
        /// </summary>
        protected virtual void DrawMessage(Graphics g, Rectangle rect, string message, Color color)
        {
            if (rect.IsEmpty || string.IsNullOrEmpty(message)) return;
            Font f = MessageFont ?? SystemFonts.DefaultFont;
            TextRenderer.DrawText(g, message, f, rect, color,
                TextFormatFlags.Left | TextFormatFlags.Top |
                TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
        }

        // ── Icon helpers ──────────────────────────────────────────────────────

        /// <summary>
        /// Paints a tinted icon via <see cref="StyledImagePainter"/>.
        /// Always uses string paths — never raw Image objects.
        /// </summary>
        protected void DrawIcon(Graphics g, Rectangle iconRect, string imagePath, Color tintColor,
            int cornerRadius = 0)
        {
            if (iconRect.IsEmpty || string.IsNullOrEmpty(imagePath)) return;
            int r = cornerRadius > 0 ? cornerRadius : S(4);
            StyledImagePainter.PaintWithTint(g, iconRect, imagePath, tintColor, 1f, r);
        }

        /// <summary>
        /// Paints a circle-badge icon: coloured disc background + centred icon.
        /// Used by Material3, Elevated, MaterialYou styles.
        /// </summary>
        protected void DrawCircleBadgeIcon(Graphics g, Rectangle iconRect,
            string imagePath, Color bgColor, Color iconColor)
        {
            if (iconRect.IsEmpty || string.IsNullOrEmpty(imagePath)) return;

            // Disc background
            using (var bg = new SolidBrush(bgColor))
                g.FillEllipse(bg, iconRect);

            // Icon centred inside disc
            int inset = S(5);
            var inner = new Rectangle(
                iconRect.X + inset, iconRect.Y + inset,
                iconRect.Width - inset * 2, iconRect.Height - inset * 2);
            StyledImagePainter.PaintWithTint(g, inner, imagePath, iconColor, 1f, 0);
        }

        /// <summary>
        /// Paints a small solid colour dot (used by StatusBar and StripeDashboard styles).
        /// </summary>
        protected void DrawTypeDot(Graphics g, Rectangle dotRect, Color color)
        {
            using var brush = new SolidBrush(color);
            g.FillEllipse(brush, dotRect);
        }

        /// <summary>
        /// Paints an embedded body image via <see cref="StyledImagePainter"/>.
        /// </summary>
        protected void DrawBodyImage(Graphics g, Rectangle imageRect, string imagePath, int radius)
        {
            if (imageRect.IsEmpty || string.IsNullOrEmpty(imagePath)) return;
            StyledImagePainter.Paint(g, imageRect, imagePath,
                TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.Material3);
        }

        // ── Default INotificationPainter implementations ──────────────────────

        void INotificationPainter.PaintBackground(Graphics g, Rectangle bounds, NotificationData data)
        {
            var opts    = CreateRenderOptions(data);
            var colors  = GetColorsForType(data.Type, opts);
            int radius  = data.CornerRadiusOverride > 0
                ? data.CornerRadiusOverride
                : NotificationStyleHelpers.GetRecommendedBorderRadius(
                    TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.Material3);

            DrawBackground(g, bounds, colors.BackColor, colors.BorderColor, radius);

            // Accent stripe (left edge)
            if (data.ShowAccentStripe)
            {
                int stripeW = S(4);
                Color sc    = data.AccentStripeColor ?? colors.IconColor;
                using var b = new SolidBrush(sc);
                g.FillRectangle(b,
                    bounds.X, bounds.Y + radius,
                    stripeW, bounds.Height - radius * 2);
            }
        }

        void INotificationPainter.PaintIcon(Graphics g, Rectangle iconRect, NotificationData data)
        {
            var opts   = CreateRenderOptions(data);
            var colors = GetColorsForType(data.Type, opts);
            string ip  = !string.IsNullOrEmpty(data.IconPath)
                ? data.IconPath
                : NotificationData.GetDefaultIconForType(data.Type);
            Color tint = data.IconTint ?? colors.IconColor;
            DrawIcon(g, iconRect, ip, tint);
        }

        void INotificationPainter.PaintTitle(Graphics g, Rectangle titleRect, string title, NotificationData data)
        {
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));
            DrawTitle(g, titleRect, title, colors.ForeColor);
        }

        void INotificationPainter.PaintMessage(Graphics g, Rectangle messageRect, string message, NotificationData data)
        {
            var colors   = GetColorsForType(data.Type, CreateRenderOptions(data));
            Color msgCol = Color.FromArgb(185, colors.ForeColor);
            DrawMessage(g, messageRect, message, msgCol);
        }

        void INotificationPainter.PaintActions(Graphics g, Rectangle actionsRect,
            NotificationAction[] actions, NotificationData data)
        {
            if (actionsRect.IsEmpty || actions == null || actions.Length == 0) return;

            int btnSpacing = S(8);
            int btnWidth   = (actionsRect.Width - btnSpacing * (actions.Length - 1)) / actions.Length;
            int x          = actionsRect.X;

            foreach (var action in actions)
            {
                var btnRect = new Rectangle(x, actionsRect.Y, btnWidth, actionsRect.Height);
                PaintActionButtonDefault(g, btnRect, action, data);
                x += btnWidth + btnSpacing;
            }
        }

        void INotificationPainter.PaintCloseButton(Graphics g, Rectangle closeRect, bool hovered, NotificationData data)
        {
            if (closeRect.IsEmpty) return;
            var colors   = GetColorsForType(data.Type, CreateRenderOptions(data));
            Color cc     = hovered ? Color.FromArgb(220, colors.ForeColor) : Color.FromArgb(130, colors.ForeColor);
            float penW   = S(2);
            int   pad    = S(5);

            if (hovered)
            {
                using var hb = new SolidBrush(Color.FromArgb(25, colors.ForeColor));
                g.FillEllipse(hb, closeRect);
            }

            using var pen = new Pen(cc, penW);
            pen.StartCap = pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            g.DrawLine(pen,
                closeRect.X + pad, closeRect.Y + pad,
                closeRect.Right - pad, closeRect.Bottom - pad);
            g.DrawLine(pen,
                closeRect.Right - pad, closeRect.Y + pad,
                closeRect.X + pad, closeRect.Bottom - pad);
        }

        void INotificationPainter.PaintProgressBar(Graphics g, Rectangle progressRect,
            float progress, NotificationData data)
        {
            if (progressRect.IsEmpty) return;
            var colors = GetColorsForType(data.Type, CreateRenderOptions(data));

            using (var bg = new SolidBrush(Color.FromArgb(40, colors.IconColor)))
                g.FillRectangle(bg, progressRect);

            int pw = (int)(progressRect.Width * (progress / 100f));
            if (pw > 0)
            {
                var fillRect = new Rectangle(progressRect.X, progressRect.Y, pw, progressRect.Height);
                using var fill = new SolidBrush(colors.IconColor);
                g.FillRectangle(fill, fillRect);
            }
        }

        // ── Private button helper ─────────────────────────────────────────────

        private void PaintActionButtonDefault(Graphics g, Rectangle rect,
            NotificationAction action, NotificationData data)
        {
            var colors   = GetColorsForType(data.Type, CreateRenderOptions(data));
            Color btnCol = action.CustomColor ??
                           (action.IsPrimary ? colors.IconColor : Color.FromArgb(140, colors.ForeColor));
            bool hovered = rect.Contains(Cursor.Position);
            int  radius  = S(4);

            if (hovered)
            {
                using var hb = new SolidBrush(Color.FromArgb(25, btnCol));
                using var hp = CreateRoundedPath(rect, radius);
                g.FillPath(hb, hp);
            }

            using var pen  = new Pen(btnCol, 1f);
            using var path = CreateRoundedPath(rect, radius);
            g.DrawPath(pen, path);

            Font f = ButtonFont ?? SystemFonts.DefaultFont;
            TextRenderer.DrawText(g, action.Text, f, rect, btnCol,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        // ── Geometry helpers ──────────────────────────────────────────────────

        /// <summary>Creates a rounded-rectangle GraphicsPath.</summary>
        protected static GraphicsPath CreateRoundedPath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0) { path.AddRectangle(bounds); return path; }

            int d = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
            path.AddArc(bounds.X,                    bounds.Y,                    d, d, 180, 90);
            path.AddArc(bounds.Right - d,             bounds.Y,                    d, d, 270, 90);
            path.AddArc(bounds.Right - d,             bounds.Bottom - d,           d, d,   0, 90);
            path.AddArc(bounds.X,                    bounds.Bottom - d,           d, d,  90, 90);
            path.CloseFigure();
            return path;
        }

        // ── Colour helper ─────────────────────────────────────────────────────

        /// <summary>
        /// Returns the four semantic colours for a notification type,
        /// respecting any custom overrides in <see cref="NotificationRenderOptions"/>.
        /// </summary>
        protected (Color BackColor, Color ForeColor, Color BorderColor, Color IconColor)
            GetColorsForType(NotificationType type, NotificationRenderOptions opts)
            => NotificationThemeHelpers.GetColorsForType(
                type, null,
                opts.CustomBackColor,
                opts.CustomForeColor,
                opts.CustomBorderColor,
                opts.CustomIconColor);

        /// <summary>Builds render options from a <see cref="NotificationData"/>.</summary>
        protected NotificationRenderOptions CreateRenderOptions(NotificationData data)
            => new NotificationRenderOptions
            {
                Type              = data.Type,
                Layout            = data.Layout,
                Priority          = data.Priority,
                CustomBackColor   = data.CustomBackColor,
                CustomForeColor   = data.CustomForeColor,
                CustomBorderColor = null,
                CustomIconColor   = data.IconTint,
                BorderRadius      = data.CornerRadiusOverride > 0
                    ? data.CornerRadiusOverride
                    : NotificationStyleHelpers.GetRecommendedBorderRadius(
                        TheTechIdea.Beep.Winform.Controls.Common.BeepControlStyle.Material3),
                Padding     = NotificationStyleHelpers.GetRecommendedPadding(data.Layout, OwnerControl),
                Spacing     = NotificationStyleHelpers.GetRecommendedSpacing(data.Layout, OwnerControl),
                IconSize    = NotificationStyleHelpers.GetRecommendedIconSize(data.Layout, OwnerControl),
                ShowCloseButton  = data.ShowCloseButton,
                ShowProgressBar  = data.ShowProgressBar,
            };

        // ── DPI shorthand ─────────────────────────────────────────────────────

        /// <summary>Scales <paramref name="value"/> using <see cref="OwnerControl"/> as the DPI reference.</summary>
        protected int S(int value) =>
            OwnerControl != null ? DpiScalingHelper.ScaleValue(value, OwnerControl) : value;

        /// <summary>Scales <paramref name="value"/> using <paramref name="owner"/> as the DPI reference.</summary>
        protected static int S(int value, Control owner) =>
            owner != null ? DpiScalingHelper.ScaleValue(value, owner) : value;
    }
}
