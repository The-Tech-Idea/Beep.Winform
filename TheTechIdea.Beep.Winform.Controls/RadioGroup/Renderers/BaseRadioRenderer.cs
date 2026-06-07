using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Models;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers
{
    /// <summary>
    /// Minimal shared base for every <see cref="IRadioGroupRenderer"/>.
    /// Owns the paint-cycle lifecycle (owner / theme / control-style / font cache),
    /// resolves <see cref="RadioGroupColorTokens"/> once per paint, and provides
    /// the WCAG 2.2 / MD3 default implementations for focus ring and error overlay.
    /// Concrete renderers must implement only <see cref="RenderItem"/>,
    /// <see cref="MeasureItem"/>, <see cref="GetContentArea"/>, and
    /// <see cref="GetSelectorArea"/>; everything else has a sensible default.
    /// </summary>
    public abstract class BaseRadioRenderer : IRadioGroupRenderer, IFocusAwareRenderer, IImageAwareRenderer
    {
        #region Protected State

        protected BaseControl _owner;
        protected IBeepTheme _theme;
        protected Font _textFont;
        protected bool _ownsTextFont;

        protected RadioGroupColorTokens _tokens;

        #endregion

        #region Public Surface

        public virtual string StyleName => GetType().Name.Replace("RadioRenderer", string.Empty);
        public virtual string DisplayName => StyleName;
        public virtual bool SupportsMultipleSelection => true;

        public virtual bool AllowMultipleSelection { get; set; }

        public virtual BeepControlStyle ControlStyle { get; set; } = BeepControlStyle.Material3;
        public virtual bool UseThemeColors { get; set; } = true;

        public virtual Size MaxImageSize { get; set; } = new Size(24, 24);

        /// <summary>Resolved color tokens for the current paint cycle. Call <see cref="ResolveTokens"/> at the top of <see cref="RenderItem"/>.</summary>
        protected RadioGroupColorTokens Tokens => _tokens;

        #endregion

        #region DPI Helpers

        protected int S(int value) => _owner == null ? value : DpiScalingHelper.ScaleValue(value, _owner);
        protected float SF(float value) => _owner == null ? value : DpiScalingHelper.ScaleValue(value, _owner);

        #endregion

        #region Lifecycle

        public virtual void Initialize(BaseControl owner, IBeepTheme theme)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            UpdateTheme(theme);
        }

        public virtual void UpdateTheme(IBeepTheme theme)
        {
            _theme = theme;
            RefreshTextFont();
        }

        protected virtual void RefreshTextFont()
        {
            DisposeTextFont();
            _textFont = _owner?.Font ?? RadioGroupFontHelpers.GetItemFont(ControlStyle, isSelected: false, _theme);
            _ownsTextFont = _textFont != null && _owner?.Font != _textFont;
        }

        protected Font GetSubtextFont()
            => RadioGroupFontHelpers.GetSubtextFont(ControlStyle, _theme, _owner);

        protected Font GetLabelFont()
            => RadioGroupFontHelpers.GetLabelFont(ControlStyle, _theme, _owner);

        private void DisposeTextFont()
        {
            if (_ownsTextFont && _textFont != null)
            {
                try { _textFont.Dispose(); } catch { /* GDI race — swallow */ }
            }
            _textFont = null;
            _ownsTextFont = false;
        }

        #endregion

        #region Token Resolution (call from RenderItem)

        /// <summary>Resolves the MD3 token set for the current paint cycle. Call once at the top of <see cref="RenderItem"/>.</summary>
        protected void ResolveTokens()
            => _tokens = RadioGroupThemeHelpers.ResolveTokens(_theme, UseThemeColors, ControlStyle);

        #endregion

        #region Abstract Painting API

        public abstract void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state);

        public abstract Size MeasureItem(SimpleItem item, Graphics graphics);

        public abstract Rectangle GetContentArea(Rectangle itemRectangle);

        public abstract Rectangle GetSelectorArea(Rectangle itemRectangle);

        public virtual void RenderGroupDecorations(
            Graphics graphics, Rectangle groupRectangle,
            System.Collections.Generic.List<SimpleItem> items,
            System.Collections.Generic.List<Rectangle> itemRectangles,
            System.Collections.Generic.List<RadioItemState> states)
        {
        }

        /// <summary>
        /// Default header-label painter. Renders a non-interactive section label
        /// (uppercase, dim, slight indent) for items marked with
        /// <see cref="TheTechIdea.Beep.Winform.Controls.RadioGroup.IRadioGroupHeader"/>
        /// (or carrying an IRadioGroupHeader in their Tag).
        /// </summary>
        public virtual void DrawHeader(
            Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state)
        {
            if (graphics == null || item == null || _tokens == null) return;

            // Top divider line for visual separation
            using var pen = new Pen(_tokens.OutlineVariant, SF(1f));
            graphics.DrawLine(pen,
                rectangle.X + S(8), rectangle.Y + S(1),
                rectangle.Right - S(8), rectangle.Y + S(1));

            // Section label text (uppercase, secondary color)
            using var brush = new SolidBrush(_tokens.OnSurfaceVariant);
            using var font = new Font(_textFont?.FontFamily ?? System.Drawing.SystemFonts.DefaultFont.FontFamily,
                SF(10f), FontStyle.Bold, GraphicsUnit.Point);
            var fmt = new StringFormat { LineAlignment = StringAlignment.Center };
            graphics.DrawString(
                (item.Text ?? string.Empty).ToUpperInvariant(),
                font,
                brush,
                rectangle,
                fmt);
        }

        public virtual void Cleanup() => DisposeTextFont();

        #endregion

        #region Focus Ring (WCAG 2.2 SC 2.4.11)

        public virtual void DrawFocusRing(Graphics graphics, Rectangle itemRectangle, RadioItemState state)
        {
            if (graphics == null || _tokens == null) return;

            var focusRect = Rectangle.Inflate(itemRectangle, -S(1), -S(1));
            float penWidth = Math.Max(2f, SF(2.5f));

            using (var path = CreateRoundedRectanglePath(focusRect, S(4)))
            using (var pen = new Pen(_tokens.Primary, penWidth))
            {
                graphics.DrawPath(pen, path);
            }
        }

        #endregion

        #region Error Overlay

        /// <summary>Draws a 2px error-colored border around the item rect. Concrete renderers call this when <see cref="RadioItemState.IsError"/> is true.</summary>
        protected virtual void DrawErrorOverlay(Graphics graphics, Rectangle itemRectangle, int cornerRadius = 4)
        {
            if (graphics == null || _tokens == null) return;

            using (var path = CreateRoundedRectanglePath(itemRectangle, S(cornerRadius)))
            using (var pen = new Pen(_tokens.Error, SF(2f)))
            {
                graphics.DrawPath(pen, path);
            }
        }

        /// <summary>
        /// Draws a press-state visual on top of the item rect using <c>Tokens.PressStateLayer</c>.
        /// Concrete renderers call this from <c>RenderItem</c> when <c>state.IsPressed</c> is true.
        /// </summary>
        protected virtual void DrawPressedOverlay(Graphics graphics, Rectangle itemRectangle, int cornerRadius = 4)
        {
            if (graphics == null || _tokens == null) return;
            using (var brush = new SolidBrush(_tokens.PressStateLayer))
            using (var path = CreateRoundedRectanglePath(itemRectangle, S(cornerRadius)))
            {
                graphics.FillPath(brush, path);
            }
        }

        #endregion

        #region Shared Path Utility

        /// <summary>Creates a rounded-rectangle <see cref="GraphicsPath"/>. Shared utility for renderers and overlays.</summary>
        protected static GraphicsPath CreateRoundedRectanglePath(Rectangle rectangle, int cornerRadius)
        {
            var path = new GraphicsPath();

            if (cornerRadius <= 0)
            {
                path.AddRectangle(rectangle);
                return path;
            }

            int diameter = cornerRadius * 2;
            var arc = new Rectangle(rectangle.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = rectangle.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rectangle.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rectangle.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        #endregion

        #region Color Helpers

        /// <summary>Linearly interpolates two colors in sRGB. <paramref name="t"/> is clamped to [0,1].</summary>
        protected static Color LerpColor(Color a, Color b, float t)
        {
            if (t < 0f) t = 0f;
            else if (t > 1f) t = 1f;
            return Color.FromArgb(
                (int)Math.Round(a.A + (b.A - a.A) * t),
                (int)Math.Round(a.R + (b.R - a.R) * t),
                (int)Math.Round(a.G + (b.G - a.G) * t),
                (int)Math.Round(a.B + (b.B - a.B) * t));
        }

        #endregion
    }
}
