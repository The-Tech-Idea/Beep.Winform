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

            // Background (rounded or not)
            if (owner.IsRounded && owner.BorderRadius > 0)
            {
                using var path = ControlPaintHelper.GetRoundedRectPath(rect, owner.BorderRadius);
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
                float secondaryWidth = (float)(owner.SecondaryProgress - owner.Minimum) / (owner.Maximum - owner.Minimum) * rect.Width;
                if (secondaryWidth > 0)
                {
                    var secondaryRect = new Rectangle(rect.X, rect.Y, (int)secondaryWidth, rect.Height);
                    if (owner.IsRounded && owner.BorderRadius > 0)
                    {
                        using var spath = ControlPaintHelper.GetRoundedRectPath(secondaryRect, owner.BorderRadius);
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
            float progressWidth = pct * rect.Width;
            if (progressWidth > 0)
            {
                var progressRect = new Rectangle(rect.X, rect.Y, (int)progressWidth, rect.Height);
                // Clip for rounded to keep corner radius on the fill end
                Region oldClip = g.Clip;
                if (owner.IsRounded && owner.BorderRadius > 0)
                {
                    using var clipPath = ControlPaintHelper.GetRoundedRectPath(progressRect, owner.BorderRadius);
                    g.SetClip(clipPath, CombineMode.Replace);
                }

                switch (owner.Style)
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
                        float segmentWidth = (float)rect.Width / owner.Segments;
                        int filled = (int)(pct * owner.Segments);
                        for (int i = 0; i < owner.Segments; i++)
                        {
                            var seg = new Rectangle((int)(rect.X + i * segmentWidth + 1), rect.Y + 1, (int)(segmentWidth - 2), rect.Height - 2);
                            if (i < filled)
                            {
                                using var segBrush = new SolidBrush(owner.ProgressColor);
                                g.FillRectangle(segBrush, seg);
                            }
                            else if (i == filled && pct * owner.Segments % 1 > 0)
                            {
                                var pw = (int)(pct * owner.Segments % 1 * seg.Width);
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
                    using var glowBrush = new LinearGradientBrush(new Rectangle(progressRect.Right - 20, progressRect.Y, 20, progressRect.Height),
                        Color.FromArgb((int)(255 * glowOpacity), 255, 255, 255),
                        Color.FromArgb(10, 255, 255, 255), LinearGradientMode.Horizontal);
                    g.FillRectangle(glowBrush, progressRect.Right - 20, progressRect.Y, 20, progressRect.Height);
                }

                // restore clip
                g.Clip = oldClip;
            }

            // Border
            if (owner.IsRounded && owner.BorderRadius > 0)
            {
                using var borderPath = ControlPaintHelper.GetRoundedRectPath(rect, owner.BorderRadius);
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
                    using var font = owner.TextFont ?? new Font("Segoe UI", 10f, FontStyle.Regular);
                    var sz = g.MeasureString(text, font);
                    if (sz.Width <= rect.Width - 4 && sz.Height <= rect.Height - 2)
                    {
                        var pt = new PointF(rect.Left + (rect.Width - sz.Width) / 2, rect.Top + (rect.Height - sz.Height) / 2);
                        // pick contrasting brush depending on fill coverage
                        if (pct > 0.5f)
                        {
                            using var tb = new SolidBrush(owner.TextColor);
                            g.DrawString(text, font, tb, pt);
                        }
                        else
                        {
                            using var cb = new SolidBrush(GetContrastColor(owner.ProgressColor));
                            g.DrawString(text, font, cb, pt);
                        }
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
            int br = (int)Math.Sqrt(color.R * color.R * 0.299 + color.G * color.G * 0.587 + color.B * color.B * 0.114);
            return br > 130 ? Color.Black : Color.White;
        }
    }
}
