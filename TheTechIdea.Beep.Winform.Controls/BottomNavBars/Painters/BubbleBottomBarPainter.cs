using System.Drawing;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Models;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal class BubbleBottomBarPainter : BaseBottomBarPainter
    {
        public override string Name => "Bubble";
        public override void Paint(BottomBarPainterContext context)
        {
            base.CalculateLayout(context);
            using (var b = new SolidBrush(Color.FromArgb(250, 250, 250)))
            {
                context.Graphics.FillRectangle(b, context.Bounds);
            }

            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0; i < rects.Count; i++)
            {
                var r = rects[i];
                // Draw bubble when selected
                if (i == context.SelectedIndex)
                {
                    var bubble = new Rectangle(r.Left + 6, r.Top + 4, r.Width - 12, r.Height - 8);
                    // apply minor scale on selection using animation phase
                    float scale = 1.0f + 0.08f * context.AnimationPhase;
                    int w = (int)(bubble.Width * scale);
                    int h = (int)(bubble.Height * scale);
                    var centerX = bubble.Left + bubble.Width / 2;
                    var centerY = bubble.Top + bubble.Height / 2;
                    var scBubble = new Rectangle(centerX - w / 2, centerY - h / 2, w, h);
                    using (var gp = new GraphicsPath())
                    using (var brush = new SolidBrush(Color.FromArgb(36, context.AccentColor)))
                    {
                        gp.AddEllipse(scBubble);
                        context.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        context.Graphics.FillPath(brush, gp);
                        context.Graphics.SmoothingMode = SmoothingMode.Default;
                    }
                }

                    // For selected item, tint icon to accent color
                    var prevFill = context.ImagePainter.FillColor;
                    context.ImagePainter.FillColor = context.AccentColor;
                    var item = context.Items[i];
                    PaintMenuItem(context.Graphics, item, r, context);
                    context.ImagePainter.FillColor = prevFill;
                }
                else
                {
                    var item = context.Items[i];
                    PaintMenuItem(context.Graphics, item, r, context);
            }
        }

        public override void RegisterHitAreas(BottomBarPainterContext context)
        {
            if (context == null || context.HitTest == null) return;
            var selected = context.SelectedIndex;
            if (selected < 0 || selected >= context.Items.Count) return;
            var r = _layoutHelper.GetItemRect(selected);
            var bubble = new Rectangle(r.Left + 6, r.Top + 4, r.Width - 12, r.Height - 8);
            float scale = 1.0f + 0.08f * context.AnimationPhase;
            int w = (int)(bubble.Width * scale);
            int h = (int)(bubble.Height * scale);
            var centerX = bubble.Left + bubble.Width / 2;
            var centerY = bubble.Top + bubble.Height / 2;
            var scBubble = new Rectangle(centerX - w / 2, centerY - h / 2, w, h);
            context.HitTest.AddHitArea($"BottomBarItem_{selected}", scBubble, null, () => context.OnItemClicked?.Invoke(selected, MouseButtons.Left));
        }
    }
}
