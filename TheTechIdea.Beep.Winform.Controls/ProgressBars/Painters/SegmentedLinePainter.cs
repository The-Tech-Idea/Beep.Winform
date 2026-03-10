using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Helpers;
using TheTechIdea.Beep.Winform.Controls.ProgressBars.Models;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    internal sealed class SegmentedLinePainter : IProgressPainter, IProgressPainterV2
    {
        public string Key => nameof(ProgressPainterKind.Segmented);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> p)
        {
            g.SmoothingMode =SmoothingMode.AntiAlias;
            int segments = ProgressPainterParameterContracts.GetInt(p, "Segments", owner.Segments);
            int pad = ProgressBarDpiHelpers.Scale(owner, 6);
            int thickness = ProgressBarDpiHelpers.Scale(owner, 6);
            var line = new Rectangle(bounds.X + pad, bounds.Y + bounds.Height/2 - thickness/2, bounds.Width - pad*2, thickness);
            using (var back = new SolidBrush(Color.FromArgb(owner.Enabled ? 30 : 18, theme.CardTextForeColor))) g.FillRectangle(back, line);
            float pct = owner.DisplayProgressPercentageAccessor;
            int filledW = (int)(line.Width * pct);
            var activeColor = theme.PrimaryColor.IsEmpty ? Color.DodgerBlue : theme.PrimaryColor;
            if (!owner.Enabled)
            {
                activeColor = Color.FromArgb(120, activeColor);
            }
            using (var fore = new SolidBrush(activeColor))
                g.FillRectangle(fore, new Rectangle(line.X, line.Y, filledW, line.Height));

            // segment ticks
            using var pen = new Pen(Color.FromArgb(owner.Enabled ? 120 : 90, theme.CardTextForeColor), ProgressBarDpiHelpers.Scale(owner, 1));
            for (int i = 1; i < segments; i++)
            {
                int x = line.X + (line.Width * i) / segments;
                g.DrawLine(pen, x, line.Y, x, line.Bottom);
            }
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> p, Action<string, Rectangle> register)
        {
            int segments = ProgressPainterParameterContracts.GetInt(p, "Segments", owner.Segments);
            int pad = ProgressBarDpiHelpers.Scale(owner, 6);
            int thickness = ProgressBarDpiHelpers.Scale(owner, 6);
            var line = new Rectangle(bounds.X + pad, bounds.Y + bounds.Height/2 - thickness/2, bounds.Width - pad*2, thickness);
            for (int i = 0; i < segments; i++)
            {
                int x1 = line.X + (line.Width * i) / segments;
                int x2 = line.X + (line.Width * (i+1)) / segments;
                int hitPad = ProgressBarDpiHelpers.Scale(owner, 4);
                register($"Step:{i}", new Rectangle(x1, line.Y-hitPad, x2-x1, line.Height + hitPad*2));
            }
        }

        public void Paint(Graphics g, ProgressPainterContext context, BeepProgressBar owner)
        {
            if (context == null)
            {
                return;
            }

            Paint(g, context.Bounds, context.Theme, owner, context.Parameters);
        }

        public void UpdateHitAreas(ProgressPainterContext context, BeepProgressBar owner, Action<string, Rectangle> register)
        {
            if (context == null)
            {
                return;
            }

            UpdateHitAreas(owner, context.Bounds, context.Theme, context.Parameters, register);
        }
    }
}
