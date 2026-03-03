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
                // Draw icon only, centered
                var item = context.Items[i];
                var iconRect = new Rectangle(r.Left + (r.Width - 20) / 2, r.Top + (r.Height - 20) / 2, 20, 20);
                context.ImagePainter.ImagePath = string.IsNullOrEmpty(item.ImagePath) ? context.DefaultImagePath : item.ImagePath;
                context.ImagePainter.ImageEmbededin = ImageEmbededin.Button;
                var prevFill = context.ImagePainter.FillColor;
                var prevApplyTheme = context.ImagePainter.ApplyThemeOnImage;
                context.ImagePainter.ApplyThemeOnImage = false;
                context.ImagePainter.FillColor = isSelected ? ResolveAccent(context) : (isHovered ? ResolveHoverFore(context) : ResolveBarFore(context));
                context.ImagePainter.DrawImage(context.Graphics, iconRect);
                context.ImagePainter.ApplyThemeOnImage = prevApplyTheme;
                context.ImagePainter.FillColor = prevFill;

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
