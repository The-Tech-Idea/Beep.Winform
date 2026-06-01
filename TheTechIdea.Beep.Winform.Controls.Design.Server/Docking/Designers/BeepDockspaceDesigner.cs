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
    internal sealed class BeepDockspaceDesigner : ParentControlDesigner
    {
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_MOUSEMOVE = 0x0200;

        private BeepDockspace Dockspace => (BeepDockspace)Component;
        private IDesignerHost _designerHost;
        private IComponentChangeService _changeService;
        private ISelectionService _selectionService;
        private DockPanel _dragPanel;
        private Point _dragStartScreen;
        private bool _draggingTab;
        private bool _resizingFromDesigner;
        private bool _selectedByDesigner;

        public override SelectionRules SelectionRules =>
            SelectionRules.Visible | SelectionRules.AllSizeable;

        protected override bool GetHitTest(Point point)
        {
            Point client = Dockspace.PointToClient(point);
            if (client.Y < 0 || client.Y > BeepDockspace.HeaderHeight)
                return false;

            return Dockspace.HitTestTabAt(client) is null && _selectedByDesigner;
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
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
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

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            _selectedByDesigner = ReferenceEquals(_selectionService?.PrimarySelection, Dockspace);
        }

        protected override void WndProc(ref Message m)
        {
            if (Dockspace != null)
            {
                if (m.Msg == WM_LBUTTONDOWN)
                {
                    Point clientPoint = ClientPointFromMessage(m);
                    if (clientPoint.Y >= 0 && clientPoint.Y <= BeepDockspace.HeaderHeight)
                    {
                        DockPanel tab = Dockspace.HitTestTabAt(clientPoint);
                        if (tab != null)
                        {
                            Dockspace.ActivatePanel(tab);
                            _selectionService?.SetSelectedComponents(
                                new object[] { tab }, SelectionTypes.Replace);

                            _dragPanel = tab;
                            _dragStartScreen = Control.MousePosition;
                            _draggingTab = false;
                            return;
                        }

                        if (!_selectedByDesigner)
                        {
                            _selectionService?.SetSelectedComponents(
                                new object[] { Dockspace }, SelectionTypes.Replace);
                            return;
                        }

                        if (Dockspace.HandleHeaderMouseDown(clientPoint, MouseButtons.Left))
                            return;
                    }
                }

                if (m.Msg == WM_MOUSEMOVE && _dragPanel != null)
                {
                    Point screen = Control.MousePosition;
                    Size delta = new Size(Math.Abs(screen.X - _dragStartScreen.X), Math.Abs(screen.Y - _dragStartScreen.Y));
                    if (!_draggingTab &&
                        (delta.Width > SystemInformation.DragSize.Width || delta.Height > SystemInformation.DragSize.Height))
                    {
                        _draggingTab = true;
                        Dockspace.Cursor = Cursors.SizeAll;
                    }
                }

                if (m.Msg == WM_LBUTTONUP && _dragPanel != null)
                {
                    try
                    {
                        if (_draggingTab)
                            BeepDockingDesignerWiring.DropPanelAt(_dragPanel, Control.MousePosition, AsServiceProvider);
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
                    }
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
            {
                HandleDockspaceMove(e);
                return;
            }

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

        private void HandleDockspaceMove(ComponentChangedEventArgs e)
        {
            if (Dockspace.Manager == null || !(e.NewValue is Rectangle newBounds))
                return;

            Form hostForm = (_designerHost?.RootComponent as Form) ?? Dockspace.FindForm();
            if (hostForm == null)
                return;

            Point center = new Point(
                newBounds.Left + newBounds.Width / 2,
                newBounds.Top + newBounds.Height / 2);

            DockPosition resolved = BeepDockingDesignerWiring.ResolveDockPositionFromPoint(
                center, hostForm.DisplayRectangle);

            if (resolved != Dockspace.DockPosition)
                BeepDockingDesignerWiring.SetProperty(
                    Dockspace, nameof(BeepDockspace.DockPosition), resolved, AsServiceProvider);

            BeepDockingDesignerWiring.RefreshHostLayout(Dockspace.Manager, AsServiceProvider);
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
