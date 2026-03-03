using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Painters
{
    public abstract class DialogPainterBase : IDialogPainter
    {
        protected const int DefaultPadding = 20;
        protected const int DefaultIconSize = 48;
        protected const int DefaultIconMargin = 15;
        protected const int DefaultTitleSpacing = 12;
        protected const int DefaultContentSpacing = 10;
        protected const int DefaultButtonHeight = 36;
        protected const int DefaultButtonWidth = 90;
        protected const int DefaultButtonSpacing = 10;
        protected const int DefaultMinWidth = 300;
        protected const int DefaultMaxWidth = 600;
        protected const int DefaultMinHeight = 150;

        public abstract void Paint(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme);
        public abstract void PaintBackground(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme);
        public abstract void PaintBorder(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme);
        public abstract void PaintShadow(Graphics g, Rectangle bounds, DialogConfig config);
        public abstract void PaintIcon(Graphics g, Rectangle iconRect, DialogConfig config, IBeepTheme theme);
        public abstract void PaintTitle(Graphics g, Rectangle titleRect, DialogConfig config, IBeepTheme theme);
        public abstract void PaintMessage(Graphics g, Rectangle messageRect, DialogConfig config, IBeepTheme theme);
        public abstract void PaintButtons(Graphics g, Rectangle buttonBounds, DialogConfig config, IBeepTheme theme);

        public virtual Size CalculateSize(Graphics g, DialogConfig config)
        {
            if (config == null) return new Size(DefaultMinWidth, DefaultMinHeight);
            if (config.CustomSize.HasValue) return config.CustomSize.Value;

            int padding = Scale(DefaultPadding, config);
            int iconMargin = Scale(DefaultIconMargin, config);
            int titleSpacing = Scale(DefaultTitleSpacing, config);
            int contentSpacing = Scale(DefaultContentSpacing, config);
            int buttonHeight = Scale(DefaultButtonHeight, config);
            int buttonSpacing = Scale(DefaultButtonSpacing, config);
            int maxWidth = Math.Max(config.MinWidth, config.MaxWidth);

            int width = padding * 2;
            int height = padding * 2;
            int iconSize = Math.Max(Scale(DefaultIconSize, config), config.IconSize);
            int contentWidth = 0;

            if (config.ShowIcon)
            {
                width += iconSize + iconMargin;
            }

            if (!string.IsNullOrEmpty(config.Title))
            {
                var titleSize = DialogHelpers.MeasureDialogText(g, config.Title, GetTitleFont(config), maxWidth - width);
                contentWidth = Math.Max(contentWidth, (int)Math.Ceiling(titleSize.Width));
                height += (int)Math.Ceiling(titleSize.Height) + titleSpacing;
            }

            if (!string.IsNullOrEmpty(config.Message))
            {
                var messageSize = DialogHelpers.MeasureDialogText(g, config.Message, GetMessageFont(config), maxWidth - width);
                contentWidth = Math.Max(contentWidth, (int)Math.Ceiling(messageSize.Width));
                height += Math.Min(config.MaxContentHeight, (int)Math.Ceiling(messageSize.Height)) + contentSpacing;
            }

            if (!string.IsNullOrEmpty(config.Details))
            {
                var detailsSize = DialogHelpers.MeasureDialogText(g, config.Details, GetDetailsFont(config), maxWidth - width);
                contentWidth = Math.Max(contentWidth, (int)Math.Ceiling(detailsSize.Width));
                height += Math.Min(config.MaxContentHeight / 2, (int)Math.Ceiling(detailsSize.Height)) + contentSpacing;
            }

            if (config.CustomControl != null && !config.CustomControlFillsDialog)
            {
                contentWidth = Math.Max(contentWidth, config.CustomControl.Width);
                height += Math.Min(config.MaxContentHeight, config.CustomControl.Height) + contentSpacing;
            }

            if (config.Buttons.Length > 0)
            {
                int maxButtonWidth = Scale(DefaultButtonWidth, config);
                var buttonFont = GetButtonFont(config);
                foreach (var button in config.Buttons)
                {
                    int bw = DialogHelpers.CalculateButtonWidth(g, DialogStyleAdapter.GetButtonText(button), buttonFont);
                    maxButtonWidth = Math.Max(maxButtonWidth, bw);
                }
                var buttonAreaSize = DialogHelpers.CalculateButtonAreaSize(config.Buttons.Length, config.ButtonLayout, maxButtonWidth, buttonHeight, buttonSpacing);
                contentWidth = Math.Max(contentWidth, buttonAreaSize.Width);
                height += buttonAreaSize.Height;
            }

            width += contentWidth;

            var style = DialogStyleAdapter.GetBeepControlStyle(config);
            int border = (int)Math.Ceiling(StyleBorders.GetBorderWidth(style));
            width += border * 2;
            height += border * 2;
            if ((config.ShowShadow || config.EnableShadow) && StyleShadows.HasShadow(style))
            {
                width += StyleShadows.GetShadowBlur(style) + Math.Abs(StyleShadows.GetShadowOffsetX(style));
                height += StyleShadows.GetShadowBlur(style) + Math.Abs(StyleShadows.GetShadowOffsetY(style));
            }

            return new Size(
                Math.Max(config.MinWidth, Math.Min(width, config.MaxWidth)),
                Math.Max(Scale(DefaultMinHeight, config), height));
        }

        public virtual DialogLayout CalculateLayout(Rectangle bounds, DialogConfig config)
        {
            var layout = new DialogLayout();
            var style = DialogStyleAdapter.GetBeepControlStyle(config);
            int borderWidth = (int)Math.Ceiling(StyleBorders.GetBorderWidth(style));
            int shadowOffset = config.ShowShadow && StyleShadows.HasShadow(style) ? StyleShadows.GetShadowBlur(style) : 0;
            int padding = Scale(DefaultPadding, config);
            int iconMargin = Scale(DefaultIconMargin, config);
            int titleSpacing = Scale(DefaultTitleSpacing, config);
            int contentSpacing = Scale(DefaultContentSpacing, config);
            int buttonHeight = Scale(DefaultButtonHeight, config);
            int buttonSpacing = Scale(DefaultButtonSpacing, config);
            int iconSize = Math.Max(Scale(DefaultIconSize, config), config.IconSize);

            layout.ContentArea = new Rectangle(
                bounds.X + borderWidth,
                bounds.Y + borderWidth,
                bounds.Width - (borderWidth * 2) - shadowOffset,
                bounds.Height - (borderWidth * 2) - shadowOffset);

            int currentY = layout.ContentArea.Top + padding;
            int currentX = layout.ContentArea.Left + padding;
            int contentWidth = layout.ContentArea.Width - (padding * 2);

            if (config.ShowIcon)
            {
                layout.IconRect = new Rectangle(currentX, currentY, iconSize, iconSize);
                currentX += iconSize + iconMargin;
                contentWidth -= iconSize + iconMargin;
            }

            if (!string.IsNullOrEmpty(config.Title))
            {
                var titleSize = TextUtils.MeasureText(null, config.Title, GetTitleFont(config), contentWidth);
                layout.TitleRect = new Rectangle(currentX, currentY, contentWidth, (int)Math.Ceiling(titleSize.Height));
                currentY += layout.TitleRect.Height + titleSpacing;
            }

            currentX = layout.ContentArea.Left + padding + (config.ShowIcon ? iconSize + iconMargin : 0);
            if (!string.IsNullOrEmpty(config.Message))
            {
                var messageSize = TextUtils.MeasureText(null, config.Message, GetMessageFont(config), contentWidth);
                layout.MessageRect = new Rectangle(currentX, currentY, contentWidth, Math.Min(config.MaxContentHeight, (int)Math.Ceiling(messageSize.Height)));
                currentY += layout.MessageRect.Height + contentSpacing;
            }

            if (!string.IsNullOrEmpty(config.Details))
            {
                var detailsSize = TextUtils.MeasureText(null, config.Details, GetDetailsFont(config), contentWidth);
                layout.DetailsRect = new Rectangle(currentX, currentY, contentWidth, Math.Min(config.MaxContentHeight / 2, (int)Math.Ceiling(detailsSize.Height)));
                currentY += layout.DetailsRect.Height + contentSpacing;
            }

            if (config.CustomControl != null)
            {
                int ch = Math.Min(config.MaxContentHeight, config.CustomControl.Height);
                layout.CustomControlRect = new Rectangle(currentX, currentY, config.CustomControlFillsDialog ? contentWidth : config.CustomControl.Width, ch);
                currentY += ch + contentSpacing;
            }

            if (config.Buttons.Length > 0)
            {
                int maxButtonWidth = Scale(DefaultButtonWidth, config);
                var buttonFont = GetButtonFont(config);
                foreach (var button in config.Buttons)
                {
                    maxButtonWidth = Math.Max(maxButtonWidth, DialogHelpers.CalculateButtonWidth(null, DialogStyleAdapter.GetButtonText(button), buttonFont));
                }

                var buttonAreaSize = DialogHelpers.CalculateButtonAreaSize(config.Buttons.Length, config.ButtonLayout, maxButtonWidth, buttonHeight, buttonSpacing);
                layout.ButtonAreaRect = new Rectangle(layout.ContentArea.Left, layout.ContentArea.Bottom - buttonAreaSize.Height - padding, layout.ContentArea.Width, buttonAreaSize.Height);
                layout.ButtonRects = DialogHelpers.CalculateButtonPositions(layout.ButtonAreaRect, config.Buttons.Length, config.ButtonLayout, maxButtonWidth, buttonHeight, buttonSpacing);
            }

            if (config.ShowCloseButton)
            {
                layout.CloseButtonRect = new Rectangle(layout.ContentArea.Right - Scale(30, config), layout.ContentArea.Top + Scale(5, config), Scale(25, config), Scale(25, config));
            }

            return layout;
        }

        protected virtual Font GetTitleFont(DialogConfig config)
        {
            if (config.TitleFont != null) return config.TitleFont;
            var theme = BeepThemesManager.CurrentTheme;
            if (theme?.TitleStyle != null) return BeepThemesManager.ToFont(theme.TitleStyle);
            return BeepFontManager.GetCachedFont(BeepFontManager.DefaultFontName, 12f, FontStyle.Bold);
        }

        protected virtual Font GetMessageFont(DialogConfig config)
        {
            if (config.MessageFont != null) return config.MessageFont;
            var theme = BeepThemesManager.CurrentTheme;
            if (theme?.BodyStyle != null) return BeepThemesManager.ToFont(theme.BodyStyle);
            return BeepFontManager.GetCachedFont(BeepFontManager.DefaultFontName, 10f, FontStyle.Regular);
        }

        protected virtual Font GetDetailsFont(DialogConfig config)
        {
            if (config.DetailsFont != null) return config.DetailsFont;
            var theme = BeepThemesManager.CurrentTheme;
            if (theme?.CaptionStyle != null) return BeepThemesManager.ToFont(theme.CaptionStyle);
            return BeepFontManager.GetCachedFont(BeepFontManager.DefaultFontName, 9f, FontStyle.Regular);
        }

        protected virtual Font GetButtonFont(DialogConfig config)
        {
            if (config.ButtonFont != null) return config.ButtonFont;
            var theme = BeepThemesManager.CurrentTheme;
            if (theme?.DialogOkButtonFont != null) return BeepThemesManager.ToFont(theme.DialogOkButtonFont);
            if (theme?.ButtonStyle != null) return BeepThemesManager.ToFont(theme.ButtonStyle);
            return BeepFontManager.GetCachedFont(BeepFontManager.DefaultFontName, 9f, FontStyle.Regular);
        }

        protected GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0) { path.AddRectangle(bounds); return path; }
            int diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
            var arc = new Rectangle(bounds.X, bounds.Y, diameter, diameter);
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter; path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter; path.AddArc(arc, 0, 90);
            arc.X = bounds.Left; path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected virtual string GetIconPath(DialogConfig config)
        {
            if (!string.IsNullOrEmpty(config.IconPath)) return config.IconPath;
            return config.IconType switch
            {
                BeepDialogIcon.Information => Svgs.Information,
                BeepDialogIcon.Warning => Svgs.InfoWarning,
                BeepDialogIcon.Error => Svgs.Error,
                BeepDialogIcon.Question => Svgs.Question,
                BeepDialogIcon.Success => Svgs.CheckCircle,
                _ => Svgs.Information
            };
        }

        protected virtual string GetButtonText(BeepDialogButtons button, DialogConfig config)
        {
            if (config?.CustomButtonLabels != null && config.CustomButtonLabels.TryGetValue(button, out var customLabel))
            {
                return customLabel;
            }

            return button switch
            {
                BeepDialogButtons.Ok => "OK",
                BeepDialogButtons.Cancel => "Cancel",
                BeepDialogButtons.Yes => "Yes",
                BeepDialogButtons.No => "No",
                BeepDialogButtons.Abort => "Abort",
                BeepDialogButtons.Retry => "Retry",
                BeepDialogButtons.Ignore => "Ignore",
                BeepDialogButtons.Close => "Close",
                BeepDialogButtons.Help => "Help",
                _ => button.ToString()
            };
        }

        protected static int Scale(int value, DialogConfig config)
        {
            if (config.CustomControl != null)
            {
                return Math.Max(1, DpiScalingHelper.ScaleValue(value, config.CustomControl));
            }
            return value;
        }
    }
}
