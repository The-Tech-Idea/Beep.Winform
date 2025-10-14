using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.ProgressBars.Painters
{
    // Painter now renders the linear style directly; owner no longer draws.
    internal sealed class LinearProgressPainter : IProgressPainter
    {
        public string Key => nameof(ProgressPainterKind.Linear);

        public void Paint(Graphics g, Rectangle bounds, IBeepTheme theme, BeepProgressBar owner, IReadOnlyDictionary<string, object> parameters)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Apply owner border thickness inset
            var rect = bounds;
            rect.Inflate(-owner.BorderThickness, -owner.BorderThickness);
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Helper to clamp radius by the rectangle size
            static int ClampCorner(Rectangle r, int desired)
                => desired <= 0 ? 0 : System.Math.Min(desired, System.Math.Min(r.Width, r.Height) / 2);

            // Background (rounded or not)
            if (owner.IsRounded && owner.BorderRadius > 0)
            {
                int cr = ClampCorner(rect, owner.BorderRadius);
                using var path = ControlPaintHelper.GetRoundedRectPath(rect, cr);
                using var backFill = new SolidBrush(owner.BackColor);
                g.FillPath(backFill, path);
            }
            else
            {
                using var backFill = new SolidBrush(owner.BackColor);
                g.FillRectangle(backFill, rect);
            }

            // Secondary progress
            if (owner.SecondaryProgress > owner.Minimum)
            {
                float secondaryWidthF = (float)(owner.SecondaryProgress - owner.Minimum) / System.Math.Max(1, owner.Maximum - owner.Minimum) * rect.Width;
                int secondaryWidth = System.Math.Max(1, (int)System.Math.Round(secondaryWidthF));
                if (secondaryWidth > 0)
                {
                    var secondaryRect = new Rectangle(rect.X, rect.Y, System.Math.Min(secondaryWidth, rect.Width), rect.Height);
                    if (owner.IsRounded && owner.BorderRadius > 0)
                    {
                        int cr2 = ClampCorner(secondaryRect, owner.BorderRadius);
                        using var spath = ControlPaintHelper.GetRoundedRectPath(secondaryRect, cr2);
                        using var sbrush = new SolidBrush(owner.SecondaryProgressColor);
                        g.FillPath(sbrush, spath);
                    }
                    else
                    {
                        using var sbrush = new SolidBrush(owner.SecondaryProgressColor);
                        g.FillRectangle(sbrush, secondaryRect);
                    }
                }
            }

            // Primary progress
            float pct = owner.DisplayProgressPercentageAccessor; // 0..1 animated percentage
            pct = System.Math.Max(0f, System.Math.Min(1f, pct));
            int progressWidth = System.Math.Max(1, (int)System.Math.Round(pct * rect.Width));
            if (progressWidth > 0)
            {
                var progressRect = new Rectangle(rect.X, rect.Y, System.Math.Min(progressWidth, rect.Width), rect.Height);
                // Clip for rounded to keep corner radius on the fill end
                Region? oldClip = null;
                if (owner.IsRounded && owner.BorderRadius > 0)
                {
                    oldClip = g.Clip; // copy
                    int crp = ClampCorner(progressRect, owner.BorderRadius);
                    using var clipPath = ControlPaintHelper.GetRoundedRectPath(progressRect, crp);
                    g.SetClip(clipPath, CombineMode.Replace);
                }

                switch (owner.ProgressBarStyle)
                {
                    case ProgressBarStyle.Flat:
                        using (var brush = new SolidBrush(owner.ProgressColor)) g.FillRectangle(brush, progressRect);
                        break;
                    case ProgressBarStyle.Gradient:
                        using (var gradientBrush = new LinearGradientBrush(progressRect,
                                   Color.FromArgb(255, owner.ProgressColor),
                                   Color.FromArgb(220, owner.ProgressColor),
                                   LinearGradientMode.Vertical))
                        {
                            gradientBrush.GammaCorrection = true;
                            g.FillRectangle(gradientBrush, progressRect);
                        }
                        break;
                    case ProgressBarStyle.Striped:
                        using (var baseBrush = new SolidBrush(owner.ProgressColor)) g.FillRectangle(baseBrush, progressRect);
                        using (var stripeBrush = new HatchBrush(HatchStyle.LightUpwardDiagonal, Color.FromArgb(30, Color.White), Color.Transparent))
                            g.FillRectangle(stripeBrush, progressRect);
                        break;
                    case ProgressBarStyle.Animated:
                        using (var baseBrush = new SolidBrush(owner.ProgressColor)) g.FillRectangle(baseBrush, progressRect);
                        // simple shimmer using owner.AnimationOffset
                        using (var stripeBrush = new LinearGradientBrush(new Rectangle((int)owner.AnimationOffset, 0, progressRect.Width * 2, progressRect.Height),
                                   Color.FromArgb(0, 255, 255, 255), Color.FromArgb(60, 255, 255, 255), LinearGradientMode.Horizontal))
                        {
                            var blend = new ColorBlend
                            {
                                Colors = new[]
                                {
                                    Color.FromArgb(0, 255, 255, 255),
                                    Color.FromArgb(30, 255, 255, 255),
                                    Color.FromArgb(30, 255, 255, 255),
                                    Color.FromArgb(0, 255, 255, 255)
                                },
                                Positions = new[] { 0f, 0.4f, 0.6f, 1f }
                            };
                            stripeBrush.InterpolationColors = blend;
                            g.FillRectangle(stripeBrush, progressRect);
                        }
                        break;
                    case ProgressBarStyle.Segmented:
                        float segmentWidth = (float)rect.Width / System.Math.Max(1, owner.Segments);
                        int filled = (int)(pct * owner.Segments);
                        for (int i = 0; i < owner.Segments; i++)
                        {
                            var seg = new Rectangle((int)(rect.X + i * segmentWidth + 1), rect.Y + 1, System.Math.Max(1, (int)(segmentWidth - 2)), System.Math.Max(1, rect.Height - 2));
                            if (i < filled)
                            {
                                using var segBrush = new SolidBrush(owner.ProgressColor);
                                g.FillRectangle(segBrush, seg);
                            }
                            else if (i == filled && pct * owner.Segments % 1 > 0)
                            {
                                var pw = System.Math.Max(1, (int)(pct * owner.Segments % 1 * seg.Width));
                                using var segBrush = new SolidBrush(owner.ProgressColor);
                                g.FillRectangle(segBrush, new Rectangle(seg.X, seg.Y, pw, seg.Height));
                            }
                            using var pen = new Pen(owner.BackColor, 1);
                            g.DrawRectangle(pen, seg);
                        }
                        break;
                }

                // Glow effect
                if (owner.ShowGlowEffect && progressWidth > 0)
                {
                    float glowOpacity = owner.IsPulsating ? owner.GlowIntensity : 0.25f;
                    glowOpacity *= owner.IsPulsating ? 0.4f : 1f;
                    int gw = System.Math.Min(20, progressRect.Width);
                    if (gw > 0)
                    {
                        using var glowBrush = new LinearGradientBrush(new Rectangle(progressRect.Right - gw, progressRect.Y, gw, progressRect.Height),
                            Color.FromArgb((int)(255 * glowOpacity), 255, 255, 255),
                            Color.FromArgb(10, 255, 255, 255), LinearGradientMode.Horizontal);
                        g.FillRectangle(glowBrush, progressRect.Right - gw, progressRect.Y, gw, progressRect.Height);
                    }
                }

                // restore clip
                if (oldClip != null)
                {
                    g.Clip = oldClip;
                    oldClip.Dispose();
                }
            }

            // Border
            if (owner.IsRounded && owner.BorderRadius > 0)
            {
                int crb = ClampCorner(rect, owner.BorderRadius);
                using var borderPath = ControlPaintHelper.GetRoundedRectPath(rect, crb);
                using var borderPen = new Pen(theme.ProgressBarBorderColor != Color.Empty ? theme.ProgressBarBorderColor : theme.BorderColor, 1);
                g.DrawPath(borderPen, borderPath);
            }
            else
            {
                using var borderPen = new Pen(theme.ProgressBarBorderColor != Color.Empty ? theme.ProgressBarBorderColor : theme.BorderColor, 1);
                g.DrawRectangle(borderPen, rect);
            }

            // Text overlay
            if (owner.VisualMode != ProgressBarDisplayMode.NoText && rect.Height >= 12)
            {
                var text = owner.TextToDrawAccessor;
                if (!string.IsNullOrEmpty(text))
                {
                    using var font = owner.TextFont ?? new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Regular);
                    var sz = TextUtils.MeasureText(g, text, font);
                    if (sz.Width <= rect.Width - 4 && sz.Height <= rect.Height - 2)
                    {
                        var pt = new System.Drawing.PointF(rect.Left + (rect.Width - sz.Width) / 2, rect.Top + (rect.Height - sz.Height) / 2);
                        using var tb = new SolidBrush(owner.TextColor);
                        g.DrawString(text, font, tb, pt);
                    }
                }
            }
        }

        public void UpdateHitAreas(BeepProgressBar owner, Rectangle bounds, IBeepTheme theme, IReadOnlyDictionary<string, object> parameters, System.Action<string, Rectangle> register)
        {
            // No interactive parts by default
        }

        private static Color GetContrastColor(Color color)
        {
            int br = (int)System.Math.Sqrt(color.R * color.R * 0.299 + color.G * color.G * 0.587 + color.B * color.B * 0.114);
            return br > 130 ? Color.Black : Color.White;
        }
    }
}
