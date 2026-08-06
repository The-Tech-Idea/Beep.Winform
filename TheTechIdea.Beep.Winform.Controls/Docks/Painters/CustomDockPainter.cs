using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Docks.Painters
{
    /// <summary>
    /// The painter for <see cref="DockStyle.Custom"/>: every visual decision comes from
    /// <see cref="DockConfig"/>, with no per-style opinions of its own.
    /// </summary>
    /// <remarks>
    /// <c>DockStyle.Custom</c>'s doc comment has always said "Custom style using DockConfig
    /// properties", and that is precisely what did not happen. It was absent from
    /// <c>DockPainterFactory</c>, so it silently got <c>AppleDockPainter</c>; it was absent from the
    /// metrics table, so it silently got Apple's numbers. A user who selected Custom and set every
    /// property they could find got an Apple dock with some of their values ignored, and no error
    /// anywhere.
    ///
    /// This painter deliberately declares no <c>StyleBackgroundColor</c>, no
    /// <c>StyleBackgroundOpacity</c> and no <c>IsNamedPalette</c>: having no opinion is the point, so
    /// the config's values - and the theme, when asked for - are what reach the surface.
    /// </remarks>
    public sealed class CustomDockPainter : DockPainterBase
    {
        public override void PaintDockBackground(Graphics g, Rectangle bounds, DockConfig config, IBeepTheme theme)
        {
            if (!config.ShowBackground)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var path = CreateRoundedPath(bounds, config.CornerRadius))
            {
                var background = ResolveBackground(config, theme, Color.FromArgb(240, 240, 240));
                using (var brush = new SolidBrush(background))
                {
                    g.FillPath(brush, path);
                }

                if (config.ShowBorder)
                {
                    var border = ResolveBorder(config, theme, Color.FromArgb(200, 200, 200));
                    using (var pen = new Pen(border, 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        protected override void PaintDockItemCore(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (itemState == null)
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            var state = GetInteractionState(itemState);
            Color? fill = state switch
            {
                DockInteractionState.Pressed => config.SelectedColor ?? Color.FromArgb(60, 0, 0, 0),
                DockInteractionState.Selected => config.SelectedColor ?? Color.FromArgb(40, 0, 0, 0),
                DockInteractionState.Hovered => config.HoverColor ?? Color.FromArgb(30, 255, 255, 255),
                DockInteractionState.Focused => config.HoverColor ?? Color.FromArgb(20, 255, 255, 255),
                _ => null
            };

            if (fill.HasValue)
            {
                using var path = CreateRoundedPath(itemState.Bounds, Math.Max(2, config.CornerRadius / 2));
                using var brush = new SolidBrush(fill.Value);
                g.FillPath(brush, path);
            }

            PaintItemIcon(g, itemState, config, theme, itemState.CurrentOpacity);
        }

        public override void PaintIndicator(Graphics g, DockItemState itemState, DockConfig config, IBeepTheme theme)
        {
            if (itemState == null || config.IndicatorStyle == DockIndicatorStyle.None)
                return;

            if (!itemState.IsSelected && !(itemState.IsRunning && config.ShowRunningIndicator))
                return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = itemState.Bounds;
            int size = Math.Max(3, config.ItemSize / 14);
            using var brush = new SolidBrush(config.IndicatorColor);

            switch (config.IndicatorStyle)
            {
                case DockIndicatorStyle.Line:
                    g.FillRectangle(brush, bounds.Left + bounds.Width / 4, bounds.Bottom + 2,
                        bounds.Width / 2, size);
                    break;

                case DockIndicatorStyle.Border:
                    using (var pen = new Pen(config.IndicatorColor, size))
                    using (var path = CreateRoundedPath(bounds, Math.Max(2, config.CornerRadius / 2)))
                    {
                        g.DrawPath(pen, path);
                    }
                    break;

                default:
                    g.FillEllipse(brush, bounds.Left + (bounds.Width - size) / 2, bounds.Bottom + 2, size, size);
                    break;
            }
        }
    }
}
