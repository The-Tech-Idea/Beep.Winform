using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheTechIdea.Beep.Winform.Controls.ContextMenus.Painters
{
    internal class BaseContextMenuFucntions
    {
        public static void DrawIcon(Graphics g, Rectangle rect, string imagePath, bool disabled)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            try
            {
                if (disabled)
                {
                    using (var temp = new Bitmap(rect.Width, rect.Height))
                    {
                        using (var tempG = Graphics.FromImage(temp))
                        {
                            TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters.StyledImagePainter.Paint(
                                tempG, new Rectangle(0, 0, rect.Width, rect.Height), imagePath);
                        }
                        ControlPaint.DrawImageDisabled(g, temp, rect.X, rect.Y, Color.Transparent);
                    }
                }
                else
                {
                    TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters.StyledImagePainter.Paint(g, rect, imagePath);
                }
            }
            catch { }
        }
    }
}
