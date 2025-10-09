using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TheTechIdea.Beep.Winform.Controls
{
    // BeepiForm partial: non-client border rendering and helpers
    public partial class BeepiForm
    {
        /// <summary>
        /// Paints the custom non-client border using window coordinates.
        /// Fills the NC band first to avoid background bleed, then delegates stroke to FormBorderPainter.
        /// </summary>
        private void PaintNonClientBorder()
        {
            if (InDesignHost || !IsHandleCreated || _borderPainter == null || !_drawCustomWindowBorder)
                return;

            IntPtr hdc = GetWindowDC(this.Handle);
            if (hdc == IntPtr.Zero)
                return;

            try
            {
                using (Graphics g = Graphics.FromHdc(hdc))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    if (_borderThickness > 0)
                    {
                        int t = _borderThickness;
                        // Use rounded shapes for NC fill so corners match the border geometry exactly
                        using var outerPath = GetWindowPath();
                        // Rebuild inner path with reduced radius instead of geometric scale (more accurate corners)
                        using var innerPath = new GraphicsPath();
                        {
                            int w = Math.Max(0, Width);
                            int h = Math.Max(0, Height);
                            int innerRadius = Math.Max(0, _borderRadius - _borderThickness);
                            // Use same Bezier rounded rectangle builder as geometry
                            using var temp = new GraphicsPath();
                            // Local function replicated from geometry helper logic
                            float k = 0.5522847498307936f;
                            float rf = innerRadius;
                            float ox = rf * k, oy = rf * k;
                            float left = 0, top = 0, right = w, bottom = h;
                            float tlx = left + rf, trx = right - rf, tly = top + rf, bry = bottom - rf;
                            temp.StartFigure();
                            temp.AddLine(new PointF(tlx, top + t), new PointF(trx, top + t));
                            temp.AddBezier(new PointF(trx, top + t), new PointF(trx + ox, top + t), new PointF(right - t, top + t + oy), new PointF(right - t, tly));
                            temp.AddLine(new PointF(right - t, tly), new PointF(right - t, bry));
                            temp.AddBezier(new PointF(right - t, bry), new PointF(right - t, bry + oy), new PointF(trx + ox, bottom - t), new PointF(trx, bottom - t));
                            temp.AddLine(new PointF(trx, bottom - t), new PointF(tlx, bottom - t));
                            temp.AddBezier(new PointF(tlx, bottom - t), new PointF(tlx - ox, bottom - t), new PointF(left + t, bry + oy), new PointF(left + t, bry));
                            temp.AddLine(new PointF(left + t, bry), new PointF(left + t, tly));
                            temp.AddBezier(new PointF(left + t, tly), new PointF(left + t, tly - oy), new PointF(tlx - ox, top + t), new PointF(tlx, top + t));
                            temp.CloseFigure();
                            innerPath.AddPath(temp, false);
                        }

                        using var ncRegion = new Region(outerPath);
                        ncRegion.Exclude(innerPath);
                        using var ncBrush = new SolidBrush(this.BackColor);
                        g.FillRegion(ncBrush, ncRegion);

                        // Blend top NC band with caption gradient to avoid a seam
                        // No caption gradient overlay here; NC band should remain uniform BackColor
                    }

                    // Style-specific border stroke (path-only, simple)
                    using var pathForStroke = GetWindowPath();
                    PaintStyleBorder(g, pathForStroke);
                }
            }
            finally
            {
                ReleaseDC(this.Handle, hdc);
            }
        }
    }
}
