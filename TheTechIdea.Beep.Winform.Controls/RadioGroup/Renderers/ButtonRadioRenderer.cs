using System;
using System.Drawing;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers
{
    /// <summary>
    /// Button-group renderer: each item is a filled push-button (centred text, no radio
    /// indicator); the selected item is a solid Primary button with OnPrimary ink.
    /// Layout/measure delegate to <see cref="CardRadioRenderer"/>; visuals are its own -
    /// as a pure pass-through it rendered pixel-identical to Card and the style was a lie.
    /// </summary>
    public sealed class ButtonRadioRenderer : BaseRadioRenderer
    {
        private readonly CardRadioRenderer _inner = new CardRadioRenderer();

        public override string StyleName => "Button";
        public override string DisplayName => "Button Group";
        public override bool SupportsMultipleSelection => true;

        public override bool AllowMultipleSelection
        {
            get => _inner.AllowMultipleSelection;
            set => _inner.AllowMultipleSelection = value;
        }

        public override BeepControlStyle ControlStyle
        {
            get => _inner.ControlStyle;
            set => _inner.ControlStyle = value;
        }

        public override Size MaxImageSize
        {
            get => _inner.MaxImageSize;
            set => _inner.MaxImageSize = value;
        }

        public override void Initialize(BaseControl owner, IBeepTheme theme)
        {
            base.Initialize(owner, theme);
            _inner.Initialize(owner, theme);
        }

        public override void UpdateTheme(IBeepTheme theme)
        {
            base.UpdateTheme(theme);
            _inner.UpdateTheme(theme);
        }

        public override void RenderItem(Graphics graphics, SimpleItem item, Rectangle rectangle, RadioItemState state)
        {
            if (graphics == null || item == null) return;

            ResolveTokens();
            var t = Tokens;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var rect = Rectangle.Inflate(rectangle, -S(2), -S(2));
            Color fill = !state.IsEnabled ? t.DisabledContainer
                       : state.IsSelected ? t.Primary
                       : state.IsPressed ? t.PressStateLayer
                       : state.IsHovered ? t.HoverStateLayer
                       : t.SurfaceContainer;
            Color ink = !state.IsEnabled ? t.Disabled
                      : state.IsSelected ? t.OnPrimary
                      : t.OnSurface;
            Color border = state.IsSelected || state.IsFocused ? t.Primary : t.Outline;

            using (var path = CreateRoundedRectanglePath(rect, S(6)))
            {
                using (var brush = new SolidBrush(fill)) graphics.FillPath(brush, path);
                using (var pen = new Pen(border, state.IsSelected ? SF(2f) : SF(1f)))
                    graphics.DrawPath(pen, path);
            }

            using (var brush = new SolidBrush(ink))
            {
                var fmt = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                graphics.DrawString(item.Text ?? string.Empty, _textFont, brush, rect, fmt);
            }

            if (state.IsError) DrawErrorOverlay(graphics, rectangle, S(6));
        }

        public override Size MeasureItem(SimpleItem item, Graphics graphics) => _inner.MeasureItem(item, graphics);

        public override Rectangle GetContentArea(Rectangle itemRectangle) => _inner.GetContentArea(itemRectangle);

        public override Rectangle GetSelectorArea(Rectangle itemRectangle) => _inner.GetSelectorArea(itemRectangle);
    }
}
