using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Tabs.Helpers;
using TheTechIdea.Beep.Winform.Controls.Tabs.Hosts;
using TheTechIdea.Beep.Winform.Controls.Tabs.Models;
using TheTechIdea.Beep.Winform.Controls.Tabs.Painters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public partial class BeepTabs
    {
        private readonly BeepTabHeaderHost _headerHost = new();

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Only clear the content area; the header is drawn in OnPaint.
            Rectangle content = DisplayRectangle;
            if (content.Width > 0 && content.Height > 0 && e.ClipRectangle.IntersectsWith(content))
            {
                using var brush = new SolidBrush(BackColor);
                e.Graphics.FillRectangle(brush, content);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (IsDisposed || Disposing || !IsHandleCreated)
            {
                return;
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            Rectangle headerRegion = BeepTabLayoutHelper.GetHeaderBounds(this);
            if (!headerRegion.IsEmpty && e.ClipRectangle.IntersectsWith(headerRegion))
            {
                _painter?.PaintHeaderBackground(e.Graphics, headerRegion);
                DrawTabHeaders(e.Graphics);
                DrawHeaderSelectionIndicator(e.Graphics);
            }

            DrawErrorOverlay(e.Graphics);
        }

        private void DrawErrorOverlay(Graphics g)
        {
            if (string.IsNullOrEmpty(_lastError))
                return;

            Rectangle bounds = ClientRectangle;
            if (bounds.Width < 4 || bounds.Height < 4)
                return;

            // BT-01: Error overlay with theme-derived colors
            using Brush overlayBrush = new SolidBrush(Color.FromArgb(180, _currentTheme?.ErrorColor ?? Color.FromArgb(220, 0, 0)));
            g.FillRectangle(overlayBrush, bounds);

            Rectangle textBounds = Rectangle.Inflate(bounds, -4, -4);
            Font errorFont = BeepThemesManager.ToFont(_currentTheme?.BodySmall) ?? SystemFonts.DefaultFont;
            TextRenderer.DrawText(g, _lastError, errorFont, textBounds, Color.White,
                TextFormatFlags.WordBreak | TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        private void DrawTabHeaders(Graphics graphics)
        {
            if (GetHostedSourceItemCount() == 0)
            {
                return;
            }

            SyncHeaderSurface(graphics);
            _headerHost.RenderLegacyHeader(graphics, CreateHeaderRenderRequest());
        }

        private void InvalidateHeader()
        {
            if (!IsHandleCreated || IsDisposed || Disposing)
            {
                return;
            }

            Rectangle headerBounds = BeepTabLayoutHelper.GetHeaderBounds(this);
            if (headerBounds.IsEmpty)
            {
                return;
            }

            headerBounds.Inflate(2, 2);
            headerBounds = Rectangle.Intersect(ClientRectangle, headerBounds);
            if (!headerBounds.IsEmpty)
            {
                Invalidate(headerBounds, invalidateChildren: false);
            }
        }

        private void InvalidateHeaderSelectionChange(int oldSelectedIndex, int newSelectedIndex)
        {
            // Simply invalidate the entire header region.
            // The header is only ~30 px tall; recalculating exact tab rectangles
            // via CreateGraphics() is far more expensive than painting the whole strip.
            InvalidateHeader();
        }

        private void SyncHeaderSurface(Graphics graphics)
        {
            SyncHeaderHostFromOwner(graphics);
            _headerHost.SyncSnapshot();
        }

        private void SyncHeaderHostFromOwner(Graphics graphics)
        {
            BeepTabHeaderAction[] headerActions = GetHeaderActionSlots(graphics);
            _headerHost.SetActionSlots(headerActions);
            _headerHost.SetOverflowState(GetHeaderOverflowState(graphics, headerActions));
        }

        private BeepTabHeaderRenderRequest CreateHeaderRenderRequest()
        {
            ITabPainter primaryPainter = _painter ?? GetPainter(_tabStyle);
            primaryPainter.Theme = _currentTheme;

            ITabPainter? transitionFromPainter = null;
            ITabPainter? transitionToPainter = null;
            if (_styleTransitionProgress > 0f && _transitionFrom != _transitionTo)
            {
                transitionFromPainter = GetPainter(_transitionFrom);
                transitionToPainter = GetPainter(_transitionTo);
                transitionFromPainter.Theme = _currentTheme;
                transitionToPainter.Theme = _currentTheme;
            }

            return new BeepTabHeaderRenderRequest
            {
                PrimaryPainter = primaryPainter,
                TransitionFromPainter = transitionFromPainter,
                TransitionToPainter = transitionToPainter,
                TransitionProgress = _styleTransitionProgress
            };
        }

        private void BeepTabs_DrawItem(object sender, DrawItemEventArgs e)
        {
        }
    }
}