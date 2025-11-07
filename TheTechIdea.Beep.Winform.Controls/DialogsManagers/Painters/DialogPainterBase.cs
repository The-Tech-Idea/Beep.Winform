using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.Shadows;


namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Painters
{
    /// <summary>
    /// Base class for dialog painters providing common functionality
    /// Includes proper BorderThickness and ShadowSize calculations
    /// </summary>
    public abstract class DialogPainterBase : IDialogPainter
    {
        #region Constants

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

        #endregion

        #region Abstract Methods

        public abstract void Paint(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme);
        public abstract void PaintBackground(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme);
        public abstract void PaintBorder(Graphics g, Rectangle bounds, DialogConfig config, IBeepTheme theme);
        public abstract void PaintShadow(Graphics g, Rectangle bounds, DialogConfig config);
        public abstract void PaintIcon(Graphics g, Rectangle iconRect, DialogConfig config, IBeepTheme theme);
        public abstract void PaintTitle(Graphics g, Rectangle titleRect, DialogConfig config, IBeepTheme theme);
        public abstract void PaintMessage(Graphics g, Rectangle messageRect, DialogConfig config, IBeepTheme theme);
        public abstract void PaintButtons(Graphics g, Rectangle buttonBounds, DialogConfig config, IBeepTheme theme);

        #endregion

        #region Size Calculation with BorderThickness and ShadowSize

        /// <summary>
        /// Calculate dialog size accounting for BorderThickness and ShadowSize from BeepControlStyle
        /// </summary>
        public virtual Size CalculateSize(Graphics g, DialogConfig config)
        {
            if (config == null)
                return new Size(DefaultMinWidth, DefaultMinHeight);

            // Start with custom size if specified
            if (config.CustomSize.HasValue)
                return config.CustomSize.Value;

            int width = DefaultPadding * 2;
            int height = DefaultPadding * 2;

            // Account for icon
            int iconWidth = 0;
            if (config.ShowIcon)
            {
                iconWidth = config.IconSize + DefaultIconMargin;
                width += iconWidth;
            }

            // Measure title
            int titleHeight = 0;
            int contentWidth = 0;

            if (!string.IsNullOrEmpty(config.Title))
            {
                var titleFont = GetTitleFont(config);
                var titleSize = DialogHelpers.MeasureDialogText(g, config.Title, titleFont, 
                    DefaultMaxWidth - width);
                titleHeight = (int)Math.Ceiling(titleSize.Height) + DefaultTitleSpacing;
                contentWidth = Math.Max(contentWidth, (int)Math.Ceiling(titleSize.Width));
                height += titleHeight;
            }

            // Measure message
            if (!string.IsNullOrEmpty(config.Message))
            {
                var messageFont = GetMessageFont(config);
                var messageSize = DialogHelpers.MeasureDialogText(g, config.Message, messageFont, 
                    DefaultMaxWidth - width);
                contentWidth = Math.Max(contentWidth, (int)Math.Ceiling(messageSize.Width));
                height += (int)Math.Ceiling(messageSize.Height) + DefaultContentSpacing;
            }

            // Measure details
            if (!string.IsNullOrEmpty(config.Details))
            {
                var detailsFont = GetDetailsFont(config);
                var detailsSize = DialogHelpers.MeasureDialogText(g, config.Details, detailsFont, 
                    DefaultMaxWidth - width);
                contentWidth = Math.Max(contentWidth, (int)Math.Ceiling(detailsSize.Width));
                height += (int)Math.Ceiling(detailsSize.Height) + DefaultContentSpacing;
            }

            // Custom control size
            if (config.CustomControl != null && !config.CustomControlFillsDialog)
            {
                contentWidth = Math.Max(contentWidth, config.CustomControl.Width);
                height += config.CustomControl.Height + DefaultContentSpacing;
            }

            // Button area
            var buttons = config.Buttons;
            if (buttons.Length > 0)
            {
                var buttonFont = GetButtonFont(config);
                int maxButtonWidth = DefaultButtonWidth;

                foreach (var button in buttons)
                {
                    var buttonText = DialogStyleAdapter.GetButtonText(button);
                    int btnWidth = DialogHelpers.CalculateButtonWidth(g, buttonText, buttonFont);
                    maxButtonWidth = Math.Max(maxButtonWidth, btnWidth);
                }

                var buttonAreaSize = DialogHelpers.CalculateButtonAreaSize(
                    buttons.Length, config.ButtonLayout, maxButtonWidth, 
                    DefaultButtonHeight, DefaultButtonSpacing);

                contentWidth = Math.Max(contentWidth, buttonAreaSize.Width);
                height += buttonAreaSize.Height;
            }

            width += contentWidth;

            // ✅ CRITICAL: Account for BeepControlStyle BorderThickness
            var beepStyle = DialogStyleAdapter.GetBeepControlStyle(config);
            int borderWidth = (int)Math.Ceiling(StyleBorders.GetBorderWidth(beepStyle));
            width += borderWidth * 2;  // Left + Right
            height += borderWidth * 2; // Top + Bottom

            // ✅ CRITICAL: Account for BeepControlStyle Shadow size
            if ((config.ShowShadow || config.EnableShadow) && StyleShadows.HasShadow(beepStyle))
            {
                int shadowBlur = StyleShadows.GetShadowBlur(beepStyle);
                int shadowOffsetX = Math.Abs(StyleShadows.GetShadowOffsetX(beepStyle));
                int shadowOffsetY = Math.Abs(StyleShadows.GetShadowOffsetY(beepStyle));

                // Add shadow space to prevent clipping
                width += shadowBlur + shadowOffsetX;
                height += shadowBlur + shadowOffsetY;
            }

            // Apply constraints
            width = Math.Max(config.MinWidth, Math.Min(width, config.MaxWidth));
            height = Math.Max(DefaultMinHeight, height);

            return new Size(width, height);
        }

        #endregion

        #region Layout Calculation

        /// <summary>
        /// Calculate layout for all dialog components
        /// </summary>
        public virtual DialogLayout CalculateLayout(Rectangle bounds, DialogConfig config)
        {
            var layout = new DialogLayout();
            var beepStyle = DialogStyleAdapter.GetBeepControlStyle(config);

            // Account for border and shadow
            int borderWidth = (int)Math.Ceiling(StyleBorders.GetBorderWidth(beepStyle));
            int shadowOffset = 0;

            if (config.ShowShadow && StyleShadows.HasShadow(beepStyle))
            {
                shadowOffset = StyleShadows.GetShadowBlur(beepStyle);
            }

            // Content area (excluding border and shadow)
            layout.ContentArea = new Rectangle(
                bounds.X + borderWidth,
                bounds.Y + borderWidth,
                bounds.Width - (borderWidth * 2) - shadowOffset,
                bounds.Height - (borderWidth * 2) - shadowOffset
            );

            int currentY = layout.ContentArea.Top + DefaultPadding;
            int currentX = layout.ContentArea.Left + DefaultPadding;
            int contentWidth = layout.ContentArea.Width - (DefaultPadding * 2);

            // Icon
            if (config.ShowIcon)
            {
                layout.IconRect = new Rectangle(
                    currentX,
                    currentY,
                    config.IconSize,
                    config.IconSize
                );
                currentX += config.IconSize + DefaultIconMargin;
                contentWidth -= (config.IconSize + DefaultIconMargin);
            }

            // Title
            if (!string.IsNullOrEmpty(config.Title))
            {
                var titleFont = GetTitleFont(config);
                var titleSize = TextUtils.MeasureText(null, config.Title, titleFont, contentWidth);

                layout.TitleRect = new Rectangle(
                    currentX,
                    currentY,
                    contentWidth,
                    (int)Math.Ceiling(titleSize.Height)
                );
                currentY += layout.TitleRect.Height + DefaultTitleSpacing;
            }

            // Reset X for message (full width)
            currentX = layout.ContentArea.Left + DefaultPadding;
            if (config.ShowIcon)
                currentX += config.IconSize + DefaultIconMargin;

            // Message
            if (!string.IsNullOrEmpty(config.Message))
            {
                var messageFont = GetMessageFont(config);
                var messageSize = TextUtils.MeasureText(null, config.Message, messageFont, contentWidth);

                layout.MessageRect = new Rectangle(
                    currentX,
                    currentY,
                    contentWidth,
                    (int)Math.Ceiling(messageSize.Height)
                );
                currentY += layout.MessageRect.Height + DefaultContentSpacing;
            }

            // Details
            if (!string.IsNullOrEmpty(config.Details))
            {
                var detailsFont = GetDetailsFont(config);
                var detailsSize = TextUtils.MeasureText(null, config.Details, detailsFont, contentWidth);

                layout.DetailsRect = new Rectangle(
                    currentX,
                    currentY,
                    contentWidth,
                    (int)Math.Ceiling(detailsSize.Height)
                );
                currentY += layout.DetailsRect.Height + DefaultContentSpacing;
            }

            // Custom control
            if (config.CustomControl != null)
            {
                layout.CustomControlRect = new Rectangle(
                    currentX,
                    currentY,
                    config.CustomControlFillsDialog ? contentWidth : config.CustomControl.Width,
                    config.CustomControl.Height
                );
                currentY += layout.CustomControlRect.Height + DefaultContentSpacing;
            }

            // Buttons
            var buttons = config.Buttons;
            if (buttons.Length > 0)
            {
                var buttonFont = GetButtonFont(config);
                int maxButtonWidth = DefaultButtonWidth;

                foreach (var button in buttons)
                {
                    var buttonText = DialogStyleAdapter.GetButtonText(button);
                    maxButtonWidth = Math.Max(maxButtonWidth, 
                        DialogHelpers.CalculateButtonWidth(null, buttonText, buttonFont));
                }

                var buttonAreaSize = DialogHelpers.CalculateButtonAreaSize(
                    buttons.Length, config.ButtonLayout, maxButtonWidth,
                    DefaultButtonHeight, DefaultButtonSpacing);

                layout.ButtonAreaRect = new Rectangle(
                    layout.ContentArea.Left,
                    layout.ContentArea.Bottom - buttonAreaSize.Height - DefaultPadding,
                    layout.ContentArea.Width,
                    buttonAreaSize.Height
                );

                layout.ButtonRects = DialogHelpers.CalculateButtonPositions(
                    layout.ButtonAreaRect, buttons.Length, config.ButtonLayout,
                    maxButtonWidth, DefaultButtonHeight, DefaultButtonSpacing);
            }

            // Close button (if shown)
            if (config.ShowCloseButton)
            {
                layout.CloseButtonRect = new Rectangle(
                    layout.ContentArea.Right - 30,
                    layout.ContentArea.Top + 5,
                    25,
                    25
                );
            }

            return layout;
        }

        #endregion

        #region Font Helpers

        protected virtual Font GetTitleFont(DialogConfig config)
        {
            if (config.TitleFont != null)
                return config.TitleFont;

            return new Font("Segoe UI", 12, FontStyle.Bold);
        }

        protected virtual Font GetMessageFont(DialogConfig config)
        {
            if (config.MessageFont != null)
                return config.MessageFont;

            return new Font("Segoe UI", 10, FontStyle.Regular);
        }

        protected virtual Font GetDetailsFont(DialogConfig config)
        {
            if (config.DetailsFont != null)
                return config.DetailsFont;

            return new Font("Segoe UI", 9, FontStyle.Regular);
        }

        protected virtual Font GetButtonFont(DialogConfig config)
        {
            if (config.ButtonFont != null)
                return config.ButtonFont;

            return new Font("Segoe UI", 9, FontStyle.Regular);
        }

        #endregion

        #region Path Helpers

        /// <summary>
        /// Create rounded rectangle path
        /// </summary>
        protected GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            int diameter = Math.Min(radius * 2, Math.Min(bounds.Width, bounds.Height));
            var arc = new Rectangle(bounds.X, bounds.Y, diameter, diameter);

            // Top-left
            path.AddArc(arc, 180, 90);

            // Top-right
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom-right
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom-left
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        #endregion
    }
}
