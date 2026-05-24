using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;
using Microsoft.DotNet.DesignTools.Designers;
using TheTechIdea.Beep.Winform.Controls.Docking;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Infrastructure;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Designers
{
    /// <summary>
    /// Designer for the dockspace surface. Header clicks are routed to the
    /// control so tabs can activate pages at design time.
    /// </summary>
    internal sealed class BeepDockspaceDesigner : ParentControlDesigner
    {
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_MOUSEMOVE = 0x0200;

        private BeepDockspace Dockspace => (BeepDockspace)Component;
        private IDesignerHost _designerHost;
        private IComponentChangeService _changeService;
        private BehaviorService _behaviorService;
        private Adorner _headerAdorner;
        private DockspaceHeaderGlyph _headerGlyph;
        private DockPanel _dragPanel;
        private Point _dragStartScreen;
        private bool _draggingTab;
        private bool _resizingFromDesigner;

        public override SelectionRules SelectionRules =>
            SelectionRules.Visible | SelectionRules.Moveable | SelectionRules.AllSizeable;

        protected override bool GetHitTest(Point point)
        {
            Point client = Dockspace.PointToClient(point);
            return client.Y >= 0 && client.Y <= BeepDockspace.HeaderHeight;
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
            _changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            if (_changeService != null)
                _changeService.ComponentChanged += OnComponentChanged;

            _behaviorService = GetService(typeof(BehaviorService)) as BehaviorService;
            if (_behaviorService != null)
            {
                _headerAdorner = new Adorner();
                _headerGlyph = new DockspaceHeaderGlyph(this, _behaviorService);
                _headerAdorner.Glyphs.Add(_headerGlyph);
                _behaviorService.Adorners.Add(_headerAdorner);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_changeService != null)
                    _changeService.ComponentChanged -= OnComponentChanged;

                if (_behaviorService != null && _headerAdorner != null)
                    _behaviorService.Adorners.Remove(_headerAdorner);

                _headerAdorner = null;
                _headerGlyph = null;
                _behaviorService = null;
                _changeService = null;
                _designerHost = null;
                _dragPanel = null;
            }

            base.Dispose(disposing);
        }

        protected override void WndProc(ref Message m)
        {
            if (Dockspace != null)
            {
                if (m.Msg == WM_LBUTTONDOWN)
                {
                    Point clientPoint = ClientPointFromMessage(m);
                    if (HandleDesignerHeaderMouseDown(clientPoint))
                        return;
                }

                if (m.Msg == WM_MOUSEMOVE && _dragPanel != null)
                {
                    HandleDesignerHeaderMouseMove(Control.MousePosition);
                }

                if (m.Msg == WM_LBUTTONUP && _dragPanel != null)
                {
                    HandleDesignerHeaderMouseUp(Control.MousePosition);
                }
            }

            base.WndProc(ref m);
        }

        private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            if (_resizingFromDesigner || IsDesignerLoading || !ReferenceEquals(e.Component, Dockspace) || e.Member == null)
                return;

            if (e.Member.Name != nameof(Control.Bounds) && e.Member.Name != nameof(Control.Size))
                return;

            if (!IsSizeChange(e))
                return;

            try
            {
                _resizingFromDesigner = true;
                BeepDockingDesignerWiring.ResizeDockspace(Dockspace, AsServiceProvider);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepDockspaceDesigner] Resize failed: {ex.Message}");
            }
            finally
            {
                _resizingFromDesigner = false;
            }
        }

        private static bool IsSizeChange(ComponentChangedEventArgs e)
        {
            if (e.Member?.Name == nameof(Control.Size))
                return true;

            if (e.OldValue is Rectangle oldBounds && e.NewValue is Rectangle newBounds)
                return oldBounds.Size != newBounds.Size;

            return true;
        }

        private static Point ClientPointFromMessage(Message m)
        {
            long raw = m.LParam.ToInt64();
            int value = unchecked((int)raw);
            int x = unchecked((short)(value & 0xFFFF));
            int y = unchecked((short)((value >> 16) & 0xFFFF));
            return new Point(x, y);
        }

        private void SelectPanelInDesigner(DockPanel panel)
        {
            if (panel == null)
                return;

            ISelectionService selection = GetService(typeof(ISelectionService)) as ISelectionService;
            selection?.SetSelectedComponents(new object[] { panel }, SelectionTypes.Replace);
        }

        private bool HandleDesignerHeaderMouseDown(Point clientPoint)
        {
            if (Dockspace == null || clientPoint.Y < 0 || clientPoint.Y > BeepDockspace.HeaderHeight)
                return false;

            _dragPanel = Dockspace.HitTestTabAt(clientPoint);
            if (_dragPanel != null)
            {
                _dragStartScreen = Control.MousePosition;
                _draggingTab = false;

                Dockspace.ActivatePanel(_dragPanel);
                SelectPanelInDesigner(_dragPanel);
                _behaviorService?.Invalidate();
                return true;
            }

            return Dockspace.HandleHeaderMouseDown(clientPoint, MouseButtons.Left);
        }

        private bool HandleDesignerHeaderMouseMove(Point screenPoint)
        {
            if (_dragPanel == null)
                return false;

            Size delta = new Size(Math.Abs(screenPoint.X - _dragStartScreen.X), Math.Abs(screenPoint.Y - _dragStartScreen.Y));
            if (!_draggingTab &&
                (delta.Width > SystemInformation.DragSize.Width || delta.Height > SystemInformation.DragSize.Height))
            {
                _draggingTab = true;
                Dockspace.Cursor = Cursors.SizeAll;
            }

            return true;
        }

        private bool HandleDesignerHeaderMouseUp(Point screenPoint)
        {
            if (_dragPanel == null)
                return false;

            try
            {
                if (_draggingTab)
                    BeepDockingDesignerWiring.DropPanelAt(_dragPanel, screenPoint, AsServiceProvider);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepDockspaceDesigner] Tab drag/drop failed: {ex.Message}");
            }
            finally
            {
                Dockspace.Cursor = Cursors.Default;
                _dragPanel = null;
                _draggingTab = false;
                _behaviorService?.Invalidate();
            }

            return true;
        }

        private IServiceProvider AsServiceProvider => new DockspaceServiceProviderAdapter(base.GetService);

        private bool IsDesignerLoading => _designerHost?.Loading == true;

        private sealed class DockspaceHeaderGlyph : Glyph
        {
            private readonly BeepDockspaceDesigner _designer;
            private readonly BehaviorService _behaviorService;

            public DockspaceHeaderGlyph(BeepDockspaceDesigner designer, BehaviorService behaviorService)
                : base(new DockspaceHeaderBehavior(designer))
            {
                _designer = designer;
                _behaviorService = behaviorService;
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (_designer.Dockspace == null || _behaviorService == null)
                        return Rectangle.Empty;

                    Rectangle controlBounds = _behaviorService.ControlRectInAdornerWindow(_designer.Dockspace);
                    return new Rectangle(controlBounds.Left, controlBounds.Top, controlBounds.Width, BeepDockspace.HeaderHeight);
                }
            }

            public Point ToDockspaceClient(Point adornerPoint)
            {
                Rectangle bounds = Bounds;
                return new Point(adornerPoint.X - bounds.Left, adornerPoint.Y - bounds.Top);
            }

            public override Cursor GetHitTest(Point p) =>
                Bounds.Contains(p) ? Cursors.Hand : null;

            public override void Paint(PaintEventArgs pe)
            {
            }
        }

        private sealed class DockspaceHeaderBehavior : Behavior
        {
            private readonly BeepDockspaceDesigner _designer;

            public DockspaceHeaderBehavior(BeepDockspaceDesigner designer)
            {
                _designer = designer;
            }

            public override bool OnMouseDown(Glyph g, MouseButtons button, Point mouseLoc)
            {
                if (button != MouseButtons.Left || g is not DockspaceHeaderGlyph glyph)
                    return false;

                return _designer.HandleDesignerHeaderMouseDown(glyph.ToDockspaceClient(mouseLoc));
            }

            public override bool OnMouseMove(Glyph g, MouseButtons button, Point mouseLoc) =>
                _designer.HandleDesignerHeaderMouseMove(Control.MousePosition);

            public override bool OnMouseUp(Glyph g, MouseButtons button) =>
                _designer.HandleDesignerHeaderMouseUp(Control.MousePosition);
        }

        private sealed class DockspaceServiceProviderAdapter : IServiceProvider
        {
            private readonly Func<Type, object> _getter;
            internal DockspaceServiceProviderAdapter(Func<Type, object> getter) => _getter = getter;
            public object GetService(Type serviceType) => _getter(serviceType);
        }
    }
}
