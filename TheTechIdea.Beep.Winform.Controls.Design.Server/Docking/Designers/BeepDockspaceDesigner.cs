using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
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
    internal sealed class BeepDockspaceDesigner : ParentControlDesigner, IMessageFilter
    {
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_LBUTTONDBLCLK = 0x0203;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_MOUSEMOVE = 0x0200;

        private BeepDockspace Dockspace => (BeepDockspace)Component;
        private IDesignerHost _designerHost;
        private IComponentChangeService _changeService;
        private ISelectionService _selectionService;
        private DockPanel _dragPanel;
        private Point _dragStartScreen;
        private Point _dragStartClient;
        private bool _draggingTab;
        private bool _resizingFromDesigner;
        private bool _selectingPanelFromTab;

        public override SelectionRules SelectionRules =>
            SelectionRules.Visible | SelectionRules.Moveable | SelectionRules.AllSizeable;

        protected override bool GetHitTest(Point point)
        {
            if (Dockspace == null)
                return false;

            if (IsHeaderPoint(point))
                return true;

            Point client = Dockspace.PointToClient(point);
            return IsHeaderPoint(client);
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
            _changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            if (_changeService != null)
                _changeService.ComponentChanged += OnComponentChanged;

            _selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
            if (_selectionService != null)
                _selectionService.SelectionChanged += OnSelectionChanged;

            Dockspace.PanelSelectionRequested += OnPanelSelectionRequested;
            Dockspace.MouseDown += OnDockspaceMouseDown;
            Application.AddMessageFilter(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Dockspace.PanelSelectionRequested -= OnPanelSelectionRequested;
                Dockspace.MouseDown -= OnDockspaceMouseDown;
                Application.RemoveMessageFilter(this);

                if (_changeService != null)
                    _changeService.ComponentChanged -= OnComponentChanged;

                if (_selectionService != null)
                    _selectionService.SelectionChanged -= OnSelectionChanged;

                _changeService = null;
                _selectionService = null;
                _designerHost = null;
                _dragPanel = null;
            }

            base.Dispose(disposing);
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (Dockspace == null || Dockspace.IsDisposed || !Dockspace.IsHandleCreated)
                return false;

            Point screenPoint = ScreenPointFromMessage(m);

            if (m.Msg == WM_MOUSEMOVE && _dragPanel != null)
                return HandleDesignerHeaderMouseMove(screenPoint);

            if (m.Msg == WM_LBUTTONUP && _dragPanel != null)
                return HandleDesignerHeaderMouseUp(screenPoint);

            if (m.Msg != WM_LBUTTONDOWN &&
                m.Msg != WM_RBUTTONDOWN &&
                m.Msg != WM_LBUTTONDBLCLK)
            {
                return false;
            }

            Point clientPoint = Dockspace.PointToClient(screenPoint);
            if (!IsHeaderPoint(clientPoint))
                return false;

            if (m.Msg == WM_LBUTTONDBLCLK)
                return Dockspace.HandleHeaderDoubleClick(clientPoint, MouseButtons.Left);

            MouseButtons button = m.Msg == WM_RBUTTONDOWN ? MouseButtons.Right : MouseButtons.Left;
            return HandleDesignerHeaderMouseDown(clientPoint, button);
        }

        protected override void WndProc(ref Message m)
        {
            if (Dockspace != null)
            {
                if (m.Msg == WM_LBUTTONDOWN)
                {
                    Point clientPoint = ClientPointFromMousePosition(m);
                    if (HandleDesignerHeaderMouseDown(clientPoint, MouseButtons.Left))
                        return;
                }

                if (m.Msg == WM_RBUTTONDOWN)
                {
                    Point clientPoint = ClientPointFromMousePosition(m);
                    if (HandleDesignerHeaderMouseDown(clientPoint, MouseButtons.Right))
                        return;
                }

                if (m.Msg == WM_LBUTTONDBLCLK)
                {
                    Point clientPoint = ClientPointFromMousePosition(m);
                    if (Dockspace.HandleHeaderDoubleClick(clientPoint, MouseButtons.Left))
                        return;
                }

                if (m.Msg == WM_MOUSEMOVE && _dragPanel != null)
                {
                    HandleDesignerHeaderMouseMove(Control.MousePosition);
                    return;
                }

                if (m.Msg == WM_LBUTTONUP && _dragPanel != null)
                {
                    HandleDesignerHeaderMouseUp(Control.MousePosition);
                    return;
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

        private static Point ScreenPointFromMessage(Message m)
        {
            Point clientPoint = ClientPointFromMessage(m);
            Control target = Control.FromHandle(m.HWnd);
            return target?.PointToScreen(clientPoint) ?? Control.MousePosition;
        }

        private bool IsHeaderPoint(Point clientPoint) =>
            clientPoint.X >= 0 &&
            clientPoint.X < Dockspace.Width &&
            clientPoint.Y >= 0 &&
            clientPoint.Y <= BeepDockspace.HeaderHeight;

        private Point ClientPointFromMousePosition(Message m)
        {
            Point clientPoint = Dockspace.PointToClient(ScreenPointFromMessage(m));
            return Dockspace.ClientRectangle.Contains(clientPoint)
                ? clientPoint
                : ClientPointFromMessage(m);
        }

        private void OnPanelSelectionRequested(object sender, DockPanel panel)
        {
            SelectPanelInDesigner(panel);
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            if (_selectingPanelFromTab ||
                !ReferenceEquals(_selectionService?.PrimarySelection, Dockspace))
            {
                return;
            }

            Point clientPoint = Dockspace.PointToClient(Control.MousePosition);
            DockPanel tabPanel = Dockspace.HitTestTabAt(clientPoint);
            if (tabPanel != null)
                ActivateAndSelectTab(tabPanel, false);
        }

        private void OnDockspaceMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left ||
                e.Location.Y < 0 ||
                e.Location.Y > BeepDockspace.HeaderHeight)
            {
                return;
            }

            DockPanel tabPanel = Dockspace.HitTestTabAt(e.Location);
            if (tabPanel == null)
                return;

            ActivateAndSelectTab(tabPanel, true);
        }

        private void SelectPanelInDesigner(DockPanel panel)
        {
            if (panel == null)
                return;

            ISelectionService selection = GetService(typeof(ISelectionService)) as ISelectionService;
            selection?.SetSelectedComponents(new object[] { panel }, SelectionTypes.Replace);
        }

        private bool HandleDesignerHeaderMouseDown(Point clientPoint, MouseButtons button)
        {
            if (Dockspace == null || clientPoint.Y < 0 || clientPoint.Y > BeepDockspace.HeaderHeight)
                return false;

            if (button == MouseButtons.Right)
                return Dockspace.HandleHeaderMouseDown(clientPoint, MouseButtons.Right);

            if (button != MouseButtons.Left)
                return false;

            _dragPanel = Dockspace.HitTestTabAt(clientPoint);
            if (_dragPanel != null)
            {
                ActivateAndSelectTab(_dragPanel, true, clientPoint);
                return true;
            }

            return Dockspace.HandleHeaderMouseDown(clientPoint, MouseButtons.Left);
        }

        private void ActivateAndSelectTab(DockPanel panel, bool startDrag) =>
            ActivateAndSelectTab(panel, startDrag, Dockspace.PointToClient(Control.MousePosition));

        private void ActivateAndSelectTab(DockPanel panel, bool startDrag, Point dragStartClient)
        {
            if (panel == null)
                return;

            if (startDrag)
            {
                _dragPanel = panel;
                _dragStartClient = dragStartClient;
                _dragStartScreen = Dockspace.PointToScreen(dragStartClient);
                _draggingTab = false;
                Dockspace.Capture = true;
            }

            try
            {
                _selectingPanelFromTab = true;
                Dockspace.ActivatePanel(panel);
                SelectPanelInDesigner(panel);
            }
            finally
            {
                _selectingPanelFromTab = false;
            }
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
                {
                    DockspacePageDragCancelEventArgs args =
                        Dockspace.RaiseBeforePageDrag(_dragPanel, screenPoint, _dragStartClient, Dockspace);

                    if (!args.Cancel)
                        BeepDockingDesignerWiring.DropPanelAt(_dragPanel, screenPoint, AsServiceProvider);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BeepDockspaceDesigner] Tab drag/drop failed: {ex.Message}");
            }
            finally
            {
                Dockspace.Capture = false;
                Dockspace.Cursor = Cursors.Default;
                _dragPanel = null;
                _draggingTab = false;
            }

            return true;
        }

        private IServiceProvider AsServiceProvider => new DockspaceServiceProviderAdapter(base.GetService);

        private bool IsDesignerLoading => _designerHost?.Loading == true;

        private sealed class DockspaceServiceProviderAdapter : IServiceProvider
        {
            private readonly Func<Type, object> _getter;
            internal DockspaceServiceProviderAdapter(Func<Type, object> getter) => _getter = getter;
            public object GetService(Type serviceType) => _getter(serviceType);
        }
    }
}
