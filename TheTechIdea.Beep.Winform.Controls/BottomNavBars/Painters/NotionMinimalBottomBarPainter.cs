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
            using (var b = new SolidBrush(Color.FromArgb(255, 255, 255)))
            {
                context.Graphics.FillRectangle(b, context.Bounds);
            }

            var rects = _layoutHelper.GetItemRectangles();
            for (int i = 0; i < rects.Count; i++)
            {
                var r = rects[i];
                // Draw icon only, centered
                var item = context.Items[i];
                var iconRect = new Rectangle(r.Left + (r.Width - 20) / 2, r.Top + (r.Height - 20) / 2, 20, 20);
                context.ImagePainter.ImagePath = string.IsNullOrEmpty(item.ImagePath) ? context.DefaultImagePath : item.ImagePath;
                context.ImagePainter.ImageEmbededin = BaseImage.ImageEmbededin.Button;
                context.ImagePainter.DrawImage(context.Graphics, iconRect);

                // Selected indicator is a subtle top border
                if (i == context.SelectedIndex)
                {
                    // animate width using context.AnimationPhase
                    float phase = context.AnimationPhase;
                    int w = (int)(r.Width * (0.28f + 0.44f * phase));
                    var topRect = new Rectangle(r.Left + (r.Width - w) / 2, r.Top + 4, w, 4);
                    using (var sel = new SolidBrush(context.AccentColor))
                    {
                        context.Graphics.FillRectangle(sel, topRect);
                    }
                }
            }
        }
    }
}
