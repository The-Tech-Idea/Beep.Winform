using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes.Helpers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.PathPainters;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.Themes.ThemeContrastUtilities;

namespace TheTechIdea.Beep.Winform.Controls.ComboBoxes.Painters
{
    /// <summary>
    /// Role tokens used by <see cref="ComboBoxFieldPainterBase.GetIconTint"/> to pick the
    /// correct theme colour without introducing new <see cref="IBeepTheme"/> properties.
    /// </summary>
    internal enum BeepComboBoxIconRole
    {
        Chevron,
        ClearNormal,
        ClearHover,
        Error,
        Warning,
        Success,
        Spinner,
        ChipClose,
        Checkmark
    }
    /// <summary>
    /// Base implementation for combo box painters
    /// Provides common functionality for all variants
    /// </summary>
    internal abstract class ComboBoxFieldPainterBase : IComboBoxPainter
    {
        protected BeepComboBox _owner;
        protected IBeepTheme _theme;
        protected BeepComboBoxHelper _helper;
        protected ComboBoxVisualTokens _tokens;
        protected IComboBoxChipPainter _chipPainter;
        
        public virtual void Initialize(BeepComboBox owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _theme = theme;
            _helper = owner.Helper;
            _tokens = ComboBoxVisualTokenCatalog.Resolve(owner.ComboBoxType);
            _chipPainter = new ComboBoxChipPainter();
        }
        
        public virtual void Paint(Graphics g, BeepComboBox owner, ComboBoxRenderState state, ComboBoxLayoutSnapshot layout)
        {
            if (g == null || owner == null || layout.DrawingRect.Width <= 0 || layout.DrawingRect.Height <= 0)
                return;
            
            _owner = owner;
            _theme = owner._currentTheme;
            _helper = owner.Helper;
            _tokens = state.VisualTokens;
            
            // Set high-quality rendering
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Rectangle drawingRect = layout.DrawingRect;
            Rectangle textAreaRect = layout.TextAreaRect;
            Rectangle buttonRect = layout.DropdownButtonRect;
            Rectangle imageRect = layout.ImageRect;

            // ENH-24: horizontal mirror transform for RTL layout
            GraphicsState rtlState = null;
            if (owner.IsRtl && drawingRect.Width > 0)
            {
                rtlState = g.Save();
                g.TranslateTransform(drawingRect.X + drawingRect.Width, drawingRect.Y);
                g.ScaleTransform(-1f, 1f);
                g.TranslateTransform(-drawingRect.X, -drawingRect.Y);
            }
            
            // Draw in order
            // NOTE: Background and border are drawn by ClassicBaseControlPainter via base.DrawContent.
            // Do NOT call DrawBackground here — it would paint over the border.

            // ENH-23: skeleton shimmer — replaces all content while loading state is active
            if (owner.ShowSkeleton)
            {
                DrawSkeleton(g, owner, textAreaRect, buttonRect);
                if (rtlState != null) g.Restore(rtlState);
                return;
            }

         //   DrawBorder(g, drawingRect);

            // ENH-04: Read-only overlay (before text so it sits under the text)
            if (owner.IsReadOnly)
            {
                using var roBrush = new SolidBrush(_theme?.DisabledBackColor ?? ComboBoxPainterThemeHelpers.Sc(_owner.BackColor, SystemColors.Control));
                g.FillRectangle(roBrush, textAreaRect);
                using var hatch = new HatchBrush(HatchStyle.LightUpwardDiagonal,
                    Color.FromArgb(18, _theme?.DisabledForeColor ?? Color.Gray), Color.Transparent);
                g.FillRectangle(hatch, textAreaRect);
            }

            // ENH-03: Validation-state background tint
            if (!owner.IsReadOnly)
            {
                Color tintColor = owner.ValidationState switch
                {
                    BeepComboBoxValidationState.Warning => Color.FromArgb(20, _theme?.WarningColor ?? Color.Orange),
                    BeepComboBoxValidationState.Error   => Color.FromArgb(20, _theme?.ErrorColor   ?? Color.Red),
                    BeepComboBoxValidationState.Success => Color.FromArgb(20, _theme?.SuccessColor  ?? Color.Green),
                    _                                   => Color.Empty
                };
                if (tintColor != Color.Empty)
                {
                    using var vBrush = new SolidBrush(tintColor);
                    g.FillRectangle(vBrush, textAreaRect);
                }
            }

            DrawTextArea(g, textAreaRect);
            
            if (!imageRect.IsEmpty)
            {
                DrawLeadingImage(g, imageRect);
            }

            // ENH: Draw Chips OR text — chips replace the text area for multi-select.
            // Skip text entirely when the inline BeepTextBox editor is active — it
            // renders its own text and painting underneath causes flicker/doubling.
            bool hasChips = state.IsMultiSelect && layout.Chips != null && layout.Chips.Count > 0;
            if (hasChips)
            {
                _chipPainter?.PaintChips(g, owner, state, layout);
            }
            else if (!state.InlineEditorActive)
            {
                DrawText(g, textAreaRect);
            }

            DrawDecorations(g, drawingRect);

            // ENH-01: Clear button
            if (owner.ShowClearButton)
                DrawClearButton(g, owner);

            // ENH-03: Validation status icon (trailing, to the left of the clear button / dropdown button)
            DrawValidationIcon(g, owner, buttonRect);

            DrawDropdownButton(g, buttonRect);

            // ENH-14: Loading spinner overlay — drawn last so it sits on top of everything
            if (owner.IsLoading)
                DrawSpinner(g, owner, textAreaRect, buttonRect);

            // ENH-24: restore graphics state after RTL mirror
            if (rtlState != null) g.Restore(rtlState);
        }
        
        public virtual int GetPreferredButtonWidth()
        {
            return _tokens?.ButtonWidth ?? 24;
        }
        
        public virtual Padding GetPreferredPadding()
        {
            return _tokens?.InnerPadding ?? new Padding(4);
        }

        protected int ScaleX(int logicalPixels) => _owner?.ScaleLogicalX(logicalPixels) ?? logicalPixels;
        protected int ScaleY(int logicalPixels) => _owner?.ScaleLogicalY(logicalPixels) ?? logicalPixels;
        
        #region Abstract/Virtual Methods - Override in derived classes
        
        /// <summary>
        /// Draw the background of the combo box.
        /// Fills the DrawingRect with the correct ComboBox-specific theme colour.
        /// Override in derived classes to add type-specific fills (e.g., Material Filled tint).
        /// </summary>
        protected virtual void DrawBackground(Graphics g, Rectangle rect)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;
            // Paint correct ComboBox background directly from theme.
            // Do NOT use _owner.BackColor — base.DrawContent() overwrites it
            // with the generic theme BackColor every paint frame.
            Color bgColor = _theme?.ComboBoxBackColor ?? Color.Empty;
            if (bgColor == Color.Empty) bgColor = _theme?.BackColor ?? ComboBoxPainterThemeHelpers.Sc(_owner.BackColor, SystemColors.Window);
            if (bgColor == Color.Empty) bgColor = ComboBoxPainterThemeHelpers.Sc(_owner.BackColor, SystemColors.Window);
            var brush = PaintersFactory.GetSolidBrush(bgColor);
            g.FillRectangle(brush, rect);
        }
        
        /// <summary>
        /// Draw the border of the combo box.
        /// <para><b>NOTE</b>: This method is intentionally <b>NOT called</b> by the Paint pipeline.
        /// Border rendering is delegated to <c>BaseControl</c>'s global border-painter system
        /// (Shadcn / Radix / NextJS / Linear painters in <c>Styling/BorderPainters/</c>).
        /// This virtual hook is reserved for per-type border customisation if that system is
        /// ever bypassed for specific <see cref="ComboBoxType"/> values.</para>
        /// </summary>
        protected virtual void DrawBorder(Graphics g, Rectangle rect) { /* reserved – see NOTE above */ }
        
        /// <summary>
        /// Draw the dropdown button with separator line and state-aware arrow.
        /// Override in derived classes to customize (e.g., skip separator).
        /// </summary>
        protected virtual void DrawDropdownButton(Graphics g, Rectangle buttonRect)
        {
            if (buttonRect.IsEmpty) return;

            // Subtle button fill for hover/open states to improve affordance.
            if (_owner.Enabled && (_owner.IsButtonHovered || _owner.IsDropdownOpen))
            {
                Color baseHover = _theme?.ComboBoxHoverBackColor != Color.Empty
                    ? _theme.ComboBoxHoverBackColor
                    : (_theme?.PrimaryColor ?? Color.Empty);
                if (baseHover != Color.Empty)
                {
                    int alpha = _owner.IsDropdownOpen ? 70 : 44;
                    using var hoverPath = GetRoundedRectPath(
                        Rectangle.Inflate(buttonRect, -ScaleX(1), -ScaleY(1)),
                        Math.Max(ScaleX(3), Math.Min(buttonRect.Width, buttonRect.Height) / 4));
                    g.FillPath(PaintersFactory.GetSolidBrush(Color.FromArgb(alpha, baseHover)), hoverPath);
                }
            }

            // Draw subtle separator line between text area and button
            if (ShowButtonSeparator)
            {
                int separatorMargin = ScaleY(6);
                Color separatorColor = Color.FromArgb(100, _theme?.BorderColor ?? Color.Gray);
                var sepPen = PaintersFactory.GetPen(separatorColor, 1f);
                g.DrawLine(sepPen, buttonRect.Left, buttonRect.Top + separatorMargin,
                           buttonRect.Left, buttonRect.Bottom - separatorMargin);
            }

            // ENH-02: animated SVG chevron (rotate by _owner.ChevronAngle)
            // ChevronAngle == 0 when closed, 180 when open.
            Color arrowColor = GetArrowColor();
            // Size icon relative to the dropdown button so it remains legible across
            // styles and DPI values (instead of a fixed small logical size).
            int maxByButton = Math.Min(buttonRect.Width, buttonRect.Height) - ScaleX(8);
            int preferred = Math.Max(ScaleX(14), (int)Math.Round(Math.Min(buttonRect.Width, buttonRect.Height) * 0.45f));
            int iconSize = Math.Min(preferred, maxByButton);
            if (iconSize > 4)
            {
                int x = buttonRect.X + (buttonRect.Width  - iconSize) / 2;
                int y = buttonRect.Y + (buttonRect.Height - iconSize) / 2;
                var iconRect = new Rectangle(x, y, iconSize, iconSize);
                // Chevron "down" SVG; rotation drives open/close direction.
                DrawSvgIcon(g, iconRect, SvgsUI.ChevronDown, arrowColor,
                    rotationDeg: _owner?.ChevronAngle ?? 0f);
            }
            else
            {
                // Fallback to drawn arrow when control is too small for an SVG icon
                DrawDropdownArrow(g, buttonRect, arrowColor, _owner?.IsDropdownOpen ?? false);
            }
        }

        /// <summary>
        /// Whether to draw a vertical separator line between text and dropdown button.
        /// Override to return false in painters that don't want a separator (Rounded, Filled, Borderless, etc.).
        /// </summary>
        protected virtual bool ShowButtonSeparator => _tokens?.ShowButtonSeparator ?? true;
        
        #endregion
        
        #region Common Drawing Methods
        
        /// <summary>
        /// Returns the correct arrow color based on control state (disabled, focused, hovered, normal).
        /// </summary>
        protected virtual Color GetArrowColor()
        {
            if (!_owner.Enabled)
            {
                Color disabledBase = _theme?.ForeColor ?? Color.Gray;
                return Color.FromArgb(115, disabledBase.R, disabledBase.G, disabledBase.B);
            }
            if (_owner.Focused)
            {
                return _theme?.ComboBoxHoverBorderColor != Color.Empty
                    ? _theme.ComboBoxHoverBorderColor
                    : (_theme?.PrimaryColor ?? Color.Black);
            }
            if (_owner.IsButtonHovered)
            {
                return _theme?.SecondaryColor ?? Color.Black;
            }
            // Normal state: slightly muted secondary color
            Color normalBase = _theme?.SecondaryColor ?? Color.Gray;
            return Color.FromArgb(180, normalBase.R, normalBase.G, normalBase.B);
        }
        
        protected virtual void DrawTextArea(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;

            // State-aware overlays on top of the background drawn by ClassicBaseControlPainter.
            if (_owner.Focused && _owner.Enabled)
            {
                Color focusColor = _theme?.PrimaryColor ?? Color.Empty;
                if (focusColor != Color.Empty)
                {
                    var brush = PaintersFactory.GetSolidBrush(PathPainterHelpers.WithAlphaIfNotEmpty(focusColor, 10));
                    g.FillRectangle(brush, textAreaRect);
                }
            }
            else if (_owner.IsControlHovered && _owner.Enabled)
            {
                var hoverColor = _theme?.ComboBoxHoverBackColor ?? Color.Empty;
                Color fillColor;
                if (hoverColor != Color.Empty)
                {
                    fillColor = PathPainterHelpers.WithAlphaIfNotEmpty(hoverColor, 40);
                }
                else
                {
                    fillColor = Color.FromArgb(18, _theme?.ForeColor ?? Color.Black);
                }
                var brush = PaintersFactory.GetSolidBrush(fillColor);
                g.FillRectangle(brush, textAreaRect);
            }
        }
        
        protected virtual void DrawText(Graphics g, Rectangle textAreaRect)
        {
            if (textAreaRect.IsEmpty) return;
            
            string displayText = _helper.GetDisplayText();
            if (string.IsNullOrEmpty(displayText)) return;
            
            Color textColor;
            if (!_owner.Enabled)
            {
                textColor = _theme?.DisabledForeColor ?? Color.FromArgb(158, 158, 158);
            }
            else if (_helper.IsShowingPlaceholder())
            {
                Color placeholderColor = _theme?.TextBoxPlaceholderColor ?? Color.Empty;
                if (placeholderColor != Color.Empty)
                {
                    textColor = placeholderColor;
                }
                else
                {
                    Color baseColor = _theme?.SecondaryColor ?? _theme?.ForeColor ?? Color.Gray;
                    textColor = Color.FromArgb(128, baseColor.R, baseColor.G, baseColor.B);
                }

                var bgColor = _theme?.ComboBoxBackColor ?? _theme?.TextBoxBackColor ?? ComboBoxPainterThemeHelpers.Sc(_owner.BackColor, SystemColors.Window);
                if (ThemeContrastHelper.ContrastRatio(textColor, bgColor) < 2.8)
                {
                    textColor = ThemeContrastHelper.AdjustForegroundToContrast(textColor, bgColor, 2.8);
                }
            }
            else
            {
                textColor = _helper.GetTextColor();
            }
            
            Font textFont = _owner.TextFont ?? BeepThemesManager.ToFont(_theme?.LabelFont) ?? SystemFonts.DefaultFont;
            
            // Calculate text bounds with padding — horizontal inset from tokens,
            // vertical centering computed from measured text height so it sits
            // exactly in the middle regardless of DPI or font size.
            int horizontalInset = ScaleX(_tokens?.TextInset ?? 6);
            int availW = Math.Max(1, textAreaRect.Width - (horizontalInset * 2));

            Size textSize = TextRenderer.MeasureText(g, displayText, textFont, new Size(availW, int.MaxValue),
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

            int textY = textAreaRect.Y + Math.Max(0, (textAreaRect.Height - textSize.Height) / 2);
            var textBounds = new Rectangle(
                textAreaRect.X + horizontalInset,
                textY,
                availW,
                textSize.Height);
            
            // Draw text
            TextFormatFlags flags = TextFormatFlags.Left | 
                                   TextFormatFlags.VerticalCenter | 
                                   TextFormatFlags.EndEllipsis |
                                   TextFormatFlags.NoPrefix;
            
            TextRenderer.DrawText(g, displayText, textFont, textBounds, textColor, flags);
        }
        
        protected virtual void DrawLeadingImage(Graphics g, Rectangle imageRect)
        {
            if (imageRect.IsEmpty) return;
            
            string imagePath = null;
            
            // Prioritize LeadingImagePath over LeadingIconPath
            if (!string.IsNullOrEmpty(_owner.LeadingImagePath))
            {
                imagePath = _owner.LeadingImagePath;
            }
            else if (!string.IsNullOrEmpty(_owner.LeadingIconPath))
            {
                imagePath = _owner.LeadingIconPath;
            }
            
            if (string.IsNullOrEmpty(imagePath)) return;
            
            try
            {
                // Use StyledImagePainter for consistent image rendering.
                if (!_owner.Enabled)
                {
                    StyledImagePainter.PaintDisabled(g, imageRect, imagePath, _theme?.DisabledBackColor ?? _owner.BackColor);
                }
                else
                {
                    var style = BeepStyling.CurrentControlStyle;
                    StyledImagePainter.Paint(g, imageRect, imagePath, style);
                }
            }
            catch
            {
                // Fallback: draw a placeholder icon
                DrawPlaceholderIcon(g, imageRect);
            }
        }
        
        protected void DrawPlaceholderIcon(Graphics g, Rectangle iconRect)
        {
            var brush = PaintersFactory.GetSolidBrush(PathPainterHelpers.WithAlphaIfNotEmpty(_theme?.SecondaryColor ?? Color.Empty, 150));
            var smallRect = iconRect;
            smallRect.Inflate(-ScaleX(4), -ScaleY(4));
            g.FillEllipse(brush, smallRect);
        }
        
        /// <param name="isOpen">When true the chevron points up (dropdown open); false = down (closed).</param>
        protected void DrawDropdownArrow(Graphics g, Rectangle buttonRect, Color arrowColor, bool isOpen = false)
        {
            // Draw a modern chevron with rounded caps and DPI-aware geometry.
            var arrowSize = Math.Max(ScaleX(3), Math.Min(buttonRect.Width, buttonRect.Height) / 5);
            var arrowHalfHeight = Math.Max(ScaleY(2), arrowSize / 2);
            var centerX = buttonRect.Left + buttonRect.Width / 2;
            var centerY = buttonRect.Top + buttonRect.Height / 2;

            // Flip chevron direction based on dropdown state
            Point[] arrowPoints = isOpen
                ? new[]
                  {
                      new Point(centerX - arrowSize, centerY + arrowHalfHeight),
                      new Point(centerX, centerY - arrowHalfHeight),
                      new Point(centerX + arrowSize, centerY + arrowHalfHeight)
                  }
                : new[]
                  {
                      new Point(centerX - arrowSize, centerY - arrowHalfHeight),
                      new Point(centerX, centerY + arrowHalfHeight),
                      new Point(centerX + arrowSize, centerY - arrowHalfHeight)
                  };

            float stroke = Math.Max(1f, _owner?.ScaleLogicalX(2) ?? 2);
            var pen = (System.Drawing.Pen)PaintersFactory.GetPen(arrowColor, stroke).Clone();
            try
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;
                g.DrawLines(pen, arrowPoints);
            }
            finally
            {
                pen.Dispose();
            }
        }
        
        /// <summary>
        /// Override to draw type-specific decorations after text has been painted.
        /// </summary>
        protected virtual void DrawDecorations(Graphics g, Rectangle drawingRect) { }

        protected ComboBoxInteractionState GetInteractionState()
        {
            if (_owner == null || !_owner.Enabled) return ComboBoxInteractionState.Disabled;
            if (_owner.HasError) return ComboBoxInteractionState.Error;
            if (_owner.IsDropdownOpen) return ComboBoxInteractionState.Open;
            if (_owner.Focused) return ComboBoxInteractionState.Focused;
            if (_owner.IsControlHovered || _owner.IsButtonHovered) return ComboBoxInteractionState.Hover;
            return ComboBoxInteractionState.Normal;
        }
        
        #endregion

        #region Shared Path Helpers

        /// <summary>
        /// Creates a standard full-rounded rectangle path shared by all painter variants.
        /// Caller is responsible for disposing the returned path.
        /// </summary>
        protected GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            int diameter = radius * 2;
            var arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        #endregion

        #region Icon Helpers (ENH-01 – ENH-04)

        /// <summary>
        /// Returns the theme colour that should tint an icon depending on its semantic role.
        /// Uses only existing <see cref="IBeepTheme"/> properties – no new properties needed.
        /// </summary>
        protected Color GetIconTint(BeepComboBoxIconRole role, IBeepTheme theme, BeepComboBox owner)
            => role switch
            {
                BeepComboBoxIconRole.Chevron     => owner?.IsButtonHovered == true
                                                     ? (theme?.ComboBoxHoverForeColor ?? Color.Gray)
                                                     : (theme?.ComboBoxForeColor ?? Color.Gray),
                BeepComboBoxIconRole.ClearNormal => theme?.ComboBoxForeColor  ?? Color.Gray,
                BeepComboBoxIconRole.ClearHover  => theme?.ComboBoxHoverForeColor ?? Color.Gray,
                BeepComboBoxIconRole.Error        => theme?.ErrorColor ?? Color.Red,
                BeepComboBoxIconRole.Warning      => theme?.WarningColor ?? Color.Orange,
                BeepComboBoxIconRole.Success      => theme?.SuccessColor ?? Color.Green,
                BeepComboBoxIconRole.Spinner      => Color.FromArgb(178, theme?.ComboBoxForeColor ?? Color.Gray),
                BeepComboBoxIconRole.ChipClose    => theme?.ComboBoxSelectedForeColor ?? Color.Gray,
                BeepComboBoxIconRole.Checkmark    => theme?.ComboBoxSelectedForeColor ?? Color.Gray,
                _                                 => theme?.ComboBoxForeColor ?? Color.Gray
            };

        /// <summary>
        /// Draws an SVG icon from <see cref="SvgsUI"/> inside <paramref name="rect"/>,
        /// optionally rotating it around its own centre.
        /// </summary>
        protected static void DrawSvgIcon(
            Graphics g, Rectangle rect, string svgPath,
            Color tint, float opacity = 1f, float rotationDeg = 0f)
        {
            if (rect.IsEmpty || string.IsNullOrEmpty(svgPath)) return;
            using var iconPath = new GraphicsPath();
            iconPath.AddRectangle(rect);
            if (Math.Abs(rotationDeg) > 0.1f)
            {
                var state = g.Save();
                float cx = rect.X + rect.Width  / 2f;
                float cy = rect.Y + rect.Height / 2f;
                g.TranslateTransform(cx, cy);
                g.RotateTransform(rotationDeg);
                g.TranslateTransform(-cx, -cy);
                StyledImagePainter.PaintWithTint(g, iconPath, svgPath, tint, opacity);
                g.Restore(state);
            }
            else
            {
                StyledImagePainter.PaintWithTint(g, iconPath, svgPath, tint, opacity);
            }
        }

        /// <summary>
        /// Draws the clear (×) button when <see cref="BeepComboBox.ShowClearButton"/> is active.
        /// </summary>
        protected virtual void DrawClearButton(Graphics g, BeepComboBox owner)
        {
            var rect = owner.ClearButtonRect;
            if (rect.IsEmpty) return;

            // Hover highlight
            if (owner.ClearButtonHovered)
            {
                using var hb = new SolidBrush(Color.FromArgb(30, _theme?.ComboBoxHoverForeColor ?? Color.Gray));
                int r = Math.Min(rect.Width, rect.Height) / 2;
                using var hp = GetRoundedRectPath(rect, r);
                g.FillPath(hb, hp);
            }

            var role = owner.ClearButtonHovered ? BeepComboBoxIconRole.ClearHover : BeepComboBoxIconRole.ClearNormal;
            Color tint = GetIconTint(role, _theme, owner);
            int iconSize = Math.Min(rect.Width, rect.Height) - ScaleX(6);
            iconSize = Math.Max(ScaleX(10), iconSize);
            int xOff = (rect.Width  - iconSize) / 2;
            int yOff = (rect.Height - iconSize) / 2;
            var iconRect = new Rectangle(rect.X + xOff, rect.Y + yOff, iconSize, iconSize);
            DrawSvgIcon(g, iconRect, SvgsUI.CircleX, tint, owner.Enabled ? 0.78f : 0.55f);
        }

        /// <summary>
        /// Draws a small status icon to the left of the dropdown button when
        /// <see cref="BeepComboBox.ValidationState"/> is Warning, Error, or Success.
        /// </summary>
        protected virtual void DrawValidationIcon(Graphics g, BeepComboBox owner, Rectangle buttonRect)
        {
            if (owner == null || owner.ValidationState == BeepComboBoxValidationState.None) return;

            int iconSize = Math.Min(buttonRect.Height - ScaleY(8), ScaleX(16));
            if (iconSize <= 0) return;

            // Place icon just to the left of the dropdown button (or clear button if both shown)
            int x = buttonRect.Left - iconSize - ScaleX(6);
            int y = buttonRect.Top  + (buttonRect.Height - iconSize) / 2;
            var vRect = new Rectangle(x, y, iconSize, iconSize);

            (string svgPath, BeepComboBoxIconRole role) = owner.ValidationState switch
            {
                BeepComboBoxValidationState.Error   => (SvgsUI.AlertCircle,    BeepComboBoxIconRole.Error),
                BeepComboBoxValidationState.Warning  => (SvgsUI.AlertTriangle,  BeepComboBoxIconRole.Warning),
                BeepComboBoxValidationState.Success  => (SvgsUI.Check,          BeepComboBoxIconRole.Success),
                _                                    => (string.Empty,           BeepComboBoxIconRole.Checkmark)
            };

            if (!string.IsNullOrEmpty(svgPath))
                DrawSvgIcon(g, vRect, svgPath, GetIconTint(role, _theme, owner));
        }

        /// <summary>
        /// ENH-14: Draws an animated Loader SVG rotating around the dropdown button area
        /// and a translucent overlay over the text area to indicate loading state.
        /// </summary>
        protected virtual void DrawSpinner(Graphics g, BeepComboBox owner, Rectangle textAreaRect, Rectangle buttonRect)
        {
            if (owner == null) return;

            // Semi-transparent haze over the text area
            using (var hazeBrush = new SolidBrush(
                Color.FromArgb(90, _theme?.ComboBoxBackColor ?? ComboBoxPainterThemeHelpers.Sc(_owner.BackColor, SystemColors.Window))))
            {
                g.FillRectangle(hazeBrush, textAreaRect);
            }

            // Rotating Loader icon in the dropdown-button zone
            int spinSize = Math.Min(buttonRect.Width, buttonRect.Height) - ScaleX(8);
            if (spinSize <= 0) return;

            int cx = buttonRect.Left + (buttonRect.Width  - spinSize) / 2;
            int cy = buttonRect.Top  + (buttonRect.Height - spinSize) / 2;
            var spinRect = new Rectangle(cx, cy, spinSize, spinSize);

            DrawSvgIcon(g, spinRect, SvgsUI.CircleArrowRight,
                GetIconTint(BeepComboBoxIconRole.Spinner, _theme, owner),
                opacity:     0.75f,
                rotationDeg: owner.LoadingRotationAngle);
        }

        /// <summary>
        /// ENH-23: Draws an animated shimmer stripe over the text area to indicate a skeleton
        /// (placeholder) loading state. The stripe sweeps left-to-right driven by
        /// <see cref="BeepComboBox.SkeletonOffset"/> (0 → 1).
        /// </summary>
        protected virtual void DrawSkeleton(Graphics g, BeepComboBox owner,
            Rectangle textAreaRect, Rectangle buttonRect)
        {
            if (owner == null || textAreaRect.Width <= 0) return;

            // Base shimmer fill
            Color baseColor = _theme?.ComboBoxBackColor ?? ComboBoxPainterThemeHelpers.Sc(_owner.BackColor, SystemColors.Control);
            bool isDark = (baseColor.R * 0.299 + baseColor.G * 0.587 + baseColor.B * 0.114) < 128;
            Color shimBase = isDark ? Color.FromArgb(55, Color.White) : Color.FromArgb(55, Color.Gray);

            using (var baseBrush = new SolidBrush(shimBase))
                g.FillRectangle(baseBrush, textAreaRect);

            // Animated highlight band
            float offset = owner.SkeletonOffset;               // 0 → 1
            int totalW = textAreaRect.Width + buttonRect.Width;
            int shimW  = Math.Max(60, totalW / 3);
            int shimX  = textAreaRect.X + (int)(offset * (totalW + shimW)) - shimW;

            var shimRect = new Rectangle(shimX, textAreaRect.Y, shimW, textAreaRect.Height);
            var gradRect = new Rectangle(shimRect.X - 1, shimRect.Y, shimRect.Width + 2, shimRect.Height);

            if (gradRect.Width > 0 && gradRect.Height > 0)
            {
                Color shimHigh = isDark
                    ? Color.FromArgb(80, Color.White)
                    : Color.FromArgb(80, Color.LightGray);

                using var shimBrush = new LinearGradientBrush(
                    gradRect, Color.Transparent, shimHigh,
                    LinearGradientMode.Horizontal);
                shimBrush.SetBlendTriangularShape(0.5f);

                // Clip to text-area so shimmer doesn't bleed outside
                var clip = g.Clip;
                g.SetClip(textAreaRect, CombineMode.Intersect);
                g.FillRectangle(shimBrush, shimRect);
                g.Clip = clip;
            }
        }

        #endregion
    }

    internal static class ComboBoxPainterThemeHelpers
    {
        internal static Color Sc(Color refColor, Color lightColor)
        {
            return TheTechIdea.Beep.Winform.Controls.Helpers.ColorUtils.MapSystemColor(lightColor);
        }
    }
}

