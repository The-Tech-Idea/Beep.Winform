using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.BottomNavBars.Painters
{
    internal class NotionMinimalBottomBarPainter : BaseBottomBarPainter
    {
        public override string Name => "NotionMinimal";
        public override void Paint(BottomBarPainterContext context)
        {
            base.CalculateLayout(context);
            using (var b = new SolidBrush(ResolveBarBack(context)))
            {
                context.Graphics.FillRectangle(b, context.Bounds);
            }

            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0; i < rects.Count; i++)
            {
                var r = rects[i];
                bool isSelected = i == context.SelectedIndex;
                bool isHovered = i == context.HoverIndex;
                if (isHovered && !isSelected)
                {
                    using var hoverBrush = new SolidBrush(Color.FromArgb(18, ResolveAccent(context)));
                    context.Graphics.FillRectangle(hoverBrush, new Rectangle(r.X + 6, r.Y + 6, Math.Max(0, r.Width - 12), Math.Max(0, r.Height - 12)));
                }
                // Icon and label through the shared item painter, so this style uses the 24px icon
                // and the caller's LabelPolicy like every other. It used to draw its own 20px icon
                // centred in the whole cell and never render a label at all - so "labels always" was
                // silently ignored here, and the reference's minimal bar (which does show captions)
                // could not be produced no matter how the control was configured.
                var item = context.Items[i];
                PaintMenuItem(context.Graphics, item, r, i, context);

                // Selected indicator is a subtle top border
                if (isSelected)
                {
                    // animate width using context.AnimationPhase
                    float phase = context.AnimationPhase;
                    int w = (int)(r.Width * (0.28f + 0.44f * phase));
                    var topRect = new Rectangle(r.Left + (r.Width - w) / 2, r.Top + 4, w, 4);
                    using (var sel = new SolidBrush(ResolveAccent(context)))
                    {
                        context.Graphics.FillRectangle(sel, topRect);
                    }
                }
            }
        }
    }
}
