// BeepDocumentHostDesigner.DragDrop.cs
// Phase 03 — split out of BeepDocumentHostDesigner.cs.
//
// Drag-and-drop pipeline for the BeepDocumentHost designer:
//
//   • OnDragDrop / OnDragEnter / OnDragOver / OnDragLeave overrides.
//   • Routing toolbox drops onto the active design-time document surface
//     (creating one on demand when the host is empty).
//   • The 5-zone dock compass (Center / Left / Right / Top / Bottom) that
//     mimics DevExpress XtraTabbedView and Telerik RadDocking behaviour
//     during a tab-tear-out drag.
//   • Painting helpers (PaintDockCompass, DrawSplitPreview, DrawCompassArrow,
//     DrawCompassLabels, DrawPreviewLabel). PaintDockCompass is called from
//     OnPaintAdornments in the main partial when a drag is in progress.
//
// Hit-testing is screen-coordinate aware: drag args carry screen pixels and we
// project them into host client space via PointToClient before classifying.
// ─────────────────────────────────────────────────────────────────────────────
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.DotNet.DesignTools.Designers;
using TheTechIdea.Beep.Winform.Controls.DocumentHost;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    public partial class BeepDocumentHostDesigner
    {
        // ── State (used by both painter and drag handlers) ───────────────────

        private bool     _dragActive;
        private DockZone _dragZone = DockZone.None;

        /// <summary>5-zone dock compass classification used during drag.</summary>
        private enum DockZone { None, Center, Left, Right, Top, Bottom }

        /// <summary>
        /// Called at the end of Initialize() to opt into DragDrop on the host
        /// so it accepts toolbox drops at design-time.
        /// </summary>
        private void InitializeDockAdorner()
        {
            if (Component is BeepDocumentHost host)
                host.AllowDrop = true;
        }

        // ── Drag overrides ───────────────────────────────────────────────────

        protected override void OnDragEnter(DragEventArgs de)
        {
            base.OnDragEnter(de);

            if (IsToolboxDrag(de))
            {
                _dragActive = false;
                _dragZone = DockZone.None;
                InvalidateHost();
                return;
            }

            _dragActive = true;
            _dragZone   = DockZone.None;
            de.Effect   = DragDropEffects.Move;
            InvalidateHost();
        }

        protected override void OnDragOver(DragEventArgs de)
        {
            base.OnDragOver(de);

            if (IsToolboxDrag(de))
            {
                _dragActive = false;
                _dragZone = DockZone.None;
                InvalidateHost();
                return;
            }

            if (Component is BeepDocumentHost host)
            {
                var screenPt = new Point(de.X, de.Y);
                var clientPt = host.PointToClient(screenPt);
                _dragZone = HitTestCompass(host.ClientRectangle, clientPt);
                de.Effect = _dragZone == DockZone.None ? DragDropEffects.None : DragDropEffects.Move;
                InvalidateHost();
            }
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
            _dragActive = false;
            _dragZone   = DockZone.None;
            InvalidateHost();
        }

        /// <summary>
        /// Intercepts drag-drop from the toolbox or component tray and redirects
        /// the dropped control to the currently active design-time document panel
        /// rather than to the BeepDocumentHost root.
        ///
        /// When the host is empty (no documents yet) we deliberately do NOT bail
        /// out to base.OnDragDrop. Instead we promote the empty state into a
        /// first document on demand via EnsureDesignSurfaceForDroppedContent —
        /// exactly what commercial MDI hosts (DevExpress XtraTabbedView,
        /// Telerik RadDocking) do when a control is dropped on a bare host.
        /// Both paths funnel through the same panel-designer reflection redirect
        /// so the user gets identical "drop landed in the panel" UX in either
        /// case; the only difference is whether a transaction is wrapped around
        /// the panel auto-creation (it is — EnsureActiveDesignDocumentSurface
        /// already runs inside ExecuteDesignTimeDocumentsAction).
        /// </summary>
        protected override void OnDragDrop(DragEventArgs de)
        {
            bool toolboxDrag = IsToolboxDrag(de);

            if (toolboxDrag && Component is BeepDocumentHost hostForToolbox)
            {
                var activePanel = GetActiveDocumentPanel(hostForToolbox)
                                  ?? EnsureDesignSurfaceForDroppedContent();
                if (activePanel != null)
                {
                    var designerHost = GetDesignerHost();
                    if (designerHost?.GetDesigner(activePanel) is ControlDesigner panelDesigner)
                    {
                        panelDesigner.GetType()
                            .GetMethod("OnDragDrop",
                                System.Reflection.BindingFlags.Instance |
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Public,
                                null,
                                new[] { typeof(DragEventArgs) }, null)
                            ?.Invoke(panelDesigner, new object[] { de });
                        _dragActive = false;
                        _dragZone   = DockZone.None;
                        InvalidateHost();
                        return;
                    }
                }
            }

            base.OnDragDrop(de);

            if (!toolboxDrag && Component is BeepDocumentHost host)
            {
                var screenPt = new Point(de.X, de.Y);
                var clientPt = host.PointToClient(screenPt);
                var zone = HitTestCompass(host.ClientRectangle, clientPt);

                switch (zone)
                {
                    case DockZone.Left:
                    case DockZone.Right:
                        if (!string.IsNullOrEmpty(host.ActiveDocumentId))
                            ExecuteAction("Split Horizontal",
                                h => h.SplitDocumentHorizontal(h.ActiveDocumentId!));
                        break;

                    case DockZone.Top:
                    case DockZone.Bottom:
                        if (!string.IsNullOrEmpty(host.ActiveDocumentId))
                            ExecuteAction("Split Vertical",
                                h => h.SplitDocumentVertical(h.ActiveDocumentId!));
                        break;

                    // DockZone.Center → no split; tab dropped into existing group.
                }
            }

            _dragActive = false;
            _dragZone   = DockZone.None;
            InvalidateHost();
        }

        // ── Drag classification helpers ──────────────────────────────────────

        private static bool IsToolboxDrag(DragEventArgs de)
        {
            try
            {
                return de.Data
                    .GetFormats()
                    .Any(format => format.IndexOf("Toolbox", StringComparison.OrdinalIgnoreCase) >= 0);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Determines which compass zone contains <paramref name="pt"/>.</summary>
        private static DockZone HitTestCompass(Rectangle hostRc, Point pt)
        {
            const int ArrowSize = 28;
            int cx = hostRc.Width  / 2;
            int cy = hostRc.Height / 2;

            if (new Rectangle(cx - ArrowSize,     cy - ArrowSize,     ArrowSize * 2, ArrowSize * 2).Contains(pt)) return DockZone.Center;
            if (new Rectangle(cx - ArrowSize * 3, cy - ArrowSize,     ArrowSize * 2, ArrowSize * 2).Contains(pt)) return DockZone.Left;
            if (new Rectangle(cx + ArrowSize,     cy - ArrowSize,     ArrowSize * 2, ArrowSize * 2).Contains(pt)) return DockZone.Right;
            if (new Rectangle(cx - ArrowSize,     cy - ArrowSize * 3, ArrowSize * 2, ArrowSize * 2).Contains(pt)) return DockZone.Top;
            if (new Rectangle(cx - ArrowSize,     cy + ArrowSize,     ArrowSize * 2, ArrowSize * 2).Contains(pt)) return DockZone.Bottom;
            return DockZone.None;
        }

        private void InvalidateHost()
        {
            try { (Component as Control)?.Invalidate(); }
            catch { }
        }

        // ── Dock-compass painting ────────────────────────────────────────────

        /// <summary>Paints the docking compass when a drag is in progress.</summary>
        private void PaintDockCompass(Graphics g, BeepDocumentHost host)
        {
            if (!_dragActive) return;

            var rc = host.ClientRectangle;
            int cx = rc.Width  / 2;
            int cy = rc.Height / 2;
            const int Sz = 28;

            using var dimBrush = new SolidBrush(Color.FromArgb(35, 0, 0, 0));
            g.FillRectangle(dimBrush, rc);

            if (_dragZone != DockZone.None && _dragZone != DockZone.Center)
            {
                DrawSplitPreview(g, rc, _dragZone);
            }

            DrawCompassArrow(g, new Rectangle(cx - Sz,     cy - Sz,     Sz * 2, Sz * 2), "●", DockZone.Center);
            DrawCompassArrow(g, new Rectangle(cx - Sz * 3, cy - Sz,     Sz * 2, Sz * 2), "◄", DockZone.Left);
            DrawCompassArrow(g, new Rectangle(cx + Sz,     cy - Sz,     Sz * 2, Sz * 2), "►", DockZone.Right);
            DrawCompassArrow(g, new Rectangle(cx - Sz,     cy - Sz * 3, Sz * 2, Sz * 2), "▲", DockZone.Top);
            DrawCompassArrow(g, new Rectangle(cx - Sz,     cy + Sz,     Sz * 2, Sz * 2), "▼", DockZone.Bottom);

            DrawCompassLabels(g, rc);
        }

        private void DrawSplitPreview(Graphics g, Rectangle hostRc, DockZone zone)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            const int PreviewMargin = 40;
            var previewRc = new Rectangle(
                hostRc.X + PreviewMargin,
                hostRc.Y + PreviewMargin,
                hostRc.Width - (PreviewMargin * 2),
                hostRc.Height - (PreviewMargin * 2));

            using var previewPen = new Pen(Color.FromArgb(200, 100, 180, 255), 2f);
            using var fillBrush  = new SolidBrush(Color.FromArgb(40, 100, 180, 255));

            switch (zone)
            {
                case DockZone.Left:
                {
                    int splitLineX = previewRc.X + previewRc.Width / 2;
                    g.FillRectangle(fillBrush, previewRc.X, previewRc.Y, previewRc.Width / 2, previewRc.Height);
                    g.DrawLine(previewPen, splitLineX, previewRc.Y, splitLineX, previewRc.Y + previewRc.Height);
                    g.DrawRectangle(previewPen, previewRc);
                    DrawPreviewLabel(g, "Left Split Preview", previewRc);
                    break;
                }
                case DockZone.Right:
                {
                    int splitLineX = previewRc.X + previewRc.Width / 2;
                    g.FillRectangle(fillBrush, previewRc.X + previewRc.Width / 2, previewRc.Y, previewRc.Width / 2, previewRc.Height);
                    g.DrawLine(previewPen, splitLineX, previewRc.Y, splitLineX, previewRc.Y + previewRc.Height);
                    g.DrawRectangle(previewPen, previewRc);
                    DrawPreviewLabel(g, "Right Split Preview", previewRc);
                    break;
                }
                case DockZone.Top:
                {
                    int splitLineY = previewRc.Y + previewRc.Height / 2;
                    g.FillRectangle(fillBrush, previewRc.X, previewRc.Y, previewRc.Width, previewRc.Height / 2);
                    g.DrawLine(previewPen, previewRc.X, splitLineY, previewRc.X + previewRc.Width, splitLineY);
                    g.DrawRectangle(previewPen, previewRc);
                    DrawPreviewLabel(g, "Top Split Preview", previewRc);
                    break;
                }
                case DockZone.Bottom:
                {
                    int splitLineY = previewRc.Y + previewRc.Height / 2;
                    g.FillRectangle(fillBrush, previewRc.X, previewRc.Y + previewRc.Height / 2, previewRc.Width, previewRc.Height / 2);
                    g.DrawLine(previewPen, previewRc.X, splitLineY, previewRc.X + previewRc.Width, splitLineY);
                    g.DrawRectangle(previewPen, previewRc);
                    DrawPreviewLabel(g, "Bottom Split Preview", previewRc);
                    break;
                }
            }
        }

        private static void DrawPreviewLabel(Graphics g, string text, Rectangle previewRc)
        {
            using var font  = new Font("Segoe UI", 11f, FontStyle.Bold);
            using var brush = new SolidBrush(Color.FromArgb(200, 255, 255, 255));
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            var labelRc = new Rectangle(previewRc.X, previewRc.Y + 8, previewRc.Width, 24);
            g.DrawString(text, font, brush, labelRc, sf);
        }

        private void DrawCompassLabels(Graphics g, Rectangle hostRc)
        {
            int cx = hostRc.X + hostRc.Width / 2;
            int cy = hostRc.Y + hostRc.Height / 2;
            const int Distance = 100;

            using var font  = new Font("Segoe UI", 9f);
            using var brush = new SolidBrush(Color.FromArgb(200, 255, 255, 255));
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            if (_dragZone != DockZone.None)
            {
                string label = _dragZone switch
                {
                    DockZone.Center => "Drop here to add to group",
                    DockZone.Left   => "Split Left",
                    DockZone.Right  => "Split Right",
                    DockZone.Top    => "Split Top",
                    DockZone.Bottom => "Split Bottom",
                    _ => string.Empty
                };

                if (!string.IsNullOrEmpty(label))
                {
                    var labelRc = new Rectangle(cx - 80, cy + Distance, 160, 24);
                    g.DrawString(label, font, brush, labelRc, sf);
                }
            }
        }

        private void DrawCompassArrow(Graphics g, Rectangle rc, string symbol, DockZone zone)
        {
            bool active = zone == _dragZone;

            using var bgBrush = new SolidBrush(active
                ? Color.FromArgb(220, 0, 120, 215)
                : Color.FromArgb(160, 40, 40, 40));
            using var fg  = new SolidBrush(Color.White);
            using var pen = new Pen(Color.FromArgb(200, 255, 255, 255), 1.5f);
            using var fnt = new Font("Segoe UI", 13f, FontStyle.Bold);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillEllipse(bgBrush, rc);
            g.DrawEllipse(pen, rc);

            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(symbol, fnt, fg, rc, sf);
        }
    }
}
