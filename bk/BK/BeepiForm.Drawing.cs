using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// BeepiForm.Drawing.cs - OnPaint and Paint Methods
    /// </summary>
    public partial class BeepiForm
    {
        #region OnPaint Override
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (InDesignHost)
            {
                e.Graphics.Clear(BackColor);
                using var pen = new Pen(Color.Gray, 1) { Alignment = PenAlignment.Inset };
                e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));
                TextRenderer.DrawText(e.Graphics, Text, Font, new Point(8, 8), ForeColor);
                return;
            }

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            if (_inMoveOrResize) { g.Clear(BackColor); return; }

            if (UseHelperInfrastructure && _shadowGlow != null && _regionHelper != null && _overlayRegistry != null)
            {
                System.Diagnostics.Debug.WriteLine("[BeepiForm.OnPaint] Using helper infrastructure");
                var formPath = GetFormPath();
                using (formPath)
                {
                    if (WindowState != FormWindowState.Maximized)
                    {
                        _shadowGlow.PaintShadow(g, formPath);
                        _shadowGlow.PaintGlow(g, formPath);
                    }
                    // Paint a simple caption band first (path-only)
                    PaintStyleCaption(g);
                    using var backBrush = new SolidBrush(BackColor);
                    g.FillPath(backBrush, formPath);
                }
                _overlayRegistry.PaintOverlays(g);
            }
            else
            {
                PaintDirectly(g);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (InDesignHost) return;
            // Ensure geometry and region reflect the current window state (especially on maximize/restore)
            ApplyMaximizedWindowFix();
            MarkPathsDirty();
            if (UseHelperInfrastructure && _regionHelper != null)
            {
                // When maximized we want no rounded region; when restored, rebuild it
                if (WindowState == FormWindowState.Maximized)
                {
                    try { if (Region != null) { Region.Dispose(); Region = null; } } catch { }
                }
                else
                {
                    _regionHelper.InvalidateRegion();
                    _regionHelper.EnsureRegion(true);
                }
            }
            Invalidate();
        }
        #endregion

        #region Paint Helpers
        private void PaintDirectly(Graphics g)
        {
            if (InDesignHost) return;
            var formPath = GetFormPath();
            using (formPath)
            {
                if (WindowState != FormWindowState.Maximized)
                {
                    if (_shadowDepth > 0)
                    {
                        using var shadowPath = (GraphicsPath)formPath.Clone();
                        using var shadowBrush = new SolidBrush(_shadowColor);
                        g.TranslateTransform(_shadowDepth, _shadowDepth);
                        g.FillPath(shadowBrush, shadowPath);
                        g.TranslateTransform(-_shadowDepth, -_shadowDepth);
                    }
                    if (_enableGlow && _glowSpread > 0f)
                    {
                        using var glowPen = new Pen(_glowColor, _glowSpread)
                        {
                            LineJoin = LineJoin.Round,
                            Alignment = PenAlignment.Center
                        };
                        g.DrawPath(glowPen, formPath);
                    }
                }
                using var backBrush = new SolidBrush(BackColor);
                g.FillPath(backBrush, formPath);
                // Border is painted in non-client via WndProc
            }
        }

        public void BeginUpdate()
        {
            if (!InDesignHost)
                User32.SendMessage(Handle, User32.WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        }

        public void EndUpdate()
        {
            if (!InDesignHost)
            {
                User32.SendMessage(this.Handle, User32.WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
                this.Refresh();
            }
        }
        #endregion
    }
}
