using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepiForm
    {
        // Layout helpers extracted to partial to keep main class slim
        public virtual void AdjustControls()
        {
            if (InDesignHost) return;
            Rectangle adjustedClientArea = GetAdjustedClientRectangle();
            foreach (Control control in Controls)
            {
                if (control.Dock == DockStyle.Fill)
                {
                    control.Bounds = adjustedClientArea;
                }
                else if (control.Dock == DockStyle.Top)
                {
                    control.Bounds = new Rectangle(adjustedClientArea.Left, adjustedClientArea.Top, adjustedClientArea.Width, control.Height);
                    adjustedClientArea.Y += control.Height;
                    adjustedClientArea.Height -= control.Height;
                }
                else if (control.Dock == DockStyle.Bottom)
                {
                    control.Bounds = new Rectangle(adjustedClientArea.Left, adjustedClientArea.Bottom - control.Height, adjustedClientArea.Width, control.Height);
                    adjustedClientArea.Height -= control.Height;
                }
                else if (control.Dock == DockStyle.Left)
                {
                    control.Bounds = new Rectangle(adjustedClientArea.Left, adjustedClientArea.Top, control.Width, adjustedClientArea.Height);
                    adjustedClientArea.X += control.Width;
                    adjustedClientArea.Width -= control.Width;
                }
                else if (control.Dock == DockStyle.Right)
                {
                    control.Bounds = new Rectangle(adjustedClientArea.Right - control.Width, adjustedClientArea.Top, control.Width, adjustedClientArea.Height);
                    adjustedClientArea.Width -= control.Width;
                }
                else
                {
                    control.Left = Math.Max(control.Left, adjustedClientArea.Left);
                    control.Top = Math.Max(control.Top, adjustedClientArea.Top);
                    int maxWidth = adjustedClientArea.Right - control.Left;
                    int maxHeight = adjustedClientArea.Bottom - control.Top;
                    control.Width = Math.Min(control.Width, maxWidth);
                    control.Height = Math.Min(control.Height, maxHeight);
                }
            }
        }

        public Rectangle GetAdjustedClientRectangle()
        {
            var extra = new Padding(0);
            ComputeExtraNonClientPadding(ref extra);
            int effectiveBorder =
                (Padding.Left >= _borderThickness && Padding.Right >= _borderThickness &&
                 Padding.Top >= _borderThickness && Padding.Bottom >= _borderThickness) ? 0 : _borderThickness;

            int adjustedWidth = Math.Max(0, ClientSize.Width - (2 * effectiveBorder) - extra.Left - extra.Right);
            int adjustedHeight = Math.Max(0, ClientSize.Height - (2 * effectiveBorder) - extra.Top - extra.Bottom);
            return new Rectangle(extra.Left + effectiveBorder, extra.Top + effectiveBorder, adjustedWidth, adjustedHeight);
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                var extra = new Padding(0);
                ComputeExtraNonClientPadding(ref extra); // adds CaptionHeight when ShowCaptionBar = true
                int effectiveBorder =
                    (Padding.Left >= _borderThickness && Padding.Right >= _borderThickness &&
                     Padding.Top >= _borderThickness && Padding.Bottom >= _borderThickness) ? 0 : _borderThickness;

                int adjustedWidth = Math.Max(0, ClientSize.Width - (effectiveBorder * 2) - extra.Left - extra.Right);
                int adjustedHeight = Math.Max(0, ClientSize.Height - (effectiveBorder * 2) - extra.Top - extra.Bottom);

                return new Rectangle(effectiveBorder + extra.Left,
                                     effectiveBorder + extra.Top,
                                     adjustedWidth,
                                     adjustedHeight);
            }
        }
    }
}
