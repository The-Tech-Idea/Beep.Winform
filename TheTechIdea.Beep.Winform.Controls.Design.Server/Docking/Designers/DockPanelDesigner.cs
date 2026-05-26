using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.ActionLists;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Infrastructure;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Designers
{
    /// <summary>
    /// Design-time support for a DockPanel component.
    ///
    /// Responsibilities (matching Krypton / DockPanelSuite component designer conventions):
    /// - Auto-assign a unique Key when a new component is first dropped
    /// - Expose all dock properties via a smart-tag action list
    /// - Provide designer verbs: Dock Left / Right / Top / Bottom / Fill / Remove
    /// </summary>
    internal sealed class DockPanelDesigner : ParentControlDesigner
    {
        private DockPanel _panel;
        private IDesignerHost _designerHost;
        private DesignerActionListCollection _actionLists;
        private DesignerVerbCollection _verbs;
        private IComponentChangeService _changeService;
        private ISelectionService _selectionService;
        private bool _snappingFromDesignerMove;
        private bool _selectedByDesigner;

        private DockPanel Panel => _panel ?? (_panel = (DockPanel)Component);

        // ── Smart tags ──────────────────────────────────────────────────────────
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actionLists == null)
                {
                    _actionLists = new DesignerActionListCollection();
                    _actionLists.Add(new DockPanelActionList(Component, this));
                }
                return _actionLists;
            }
        }

        // ── Verbs ───────────────────────────────────────────────────────────────
        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection
                    {
                        new DesignerVerb("Dock &Left",   (s, e) => ApplyDockPosition(DockPosition.Left)),
                        new DesignerVerb("Dock &Right",  (s, e) => ApplyDockPosition(DockPosition.Right)),
                        new DesignerVerb("Dock &Top",    (s, e) => ApplyDockPosition(DockPosition.Top)),
                        new DesignerVerb("Dock &Bottom", (s, e) => ApplyDockPosition(DockPosition.Bottom)),
                        new DesignerVerb("Dock &Fill",   (s, e) => ApplyDockPosition(DockPosition.Fill)),
                        new DesignerVerb("Stack with Previous Panel", (s, e) => StackWithPreviousPanel()),
                        new DesignerVerb("Stack with Next Panel",     (s, e) => StackWithNextPanel()),
                        new DesignerVerb("Move Earlier in Stack",     (s, e) => MoveEarlierInStack()),
                        new DesignerVerb("Move Later in Stack",       (s, e) => MoveLaterInStack()),
                        new DesignerVerb("&Remove Panel",(s, e) => RemovePanel())
                    };
                }
                return _verbs;
            }
        }

        // ── Lifecycle ────────────────────────────────────────────────────────────

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
            _changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            if (_changeService != null)
                _changeService.ComponentChanged += OnDesignerComponentChanged;

            _selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
            if (_selectionService != null)
                _selectionService.SelectionChanged += OnDesignerSelectionChanged;
        }

        /// <summary>
        /// Called once when the component is first added to the designer container.
        /// Auto-assigns a unique Key derived from the host-assigned component name.
        /// </summary>
        public override void InitializeNewComponent(IDictionary defaultValues)
        {
            base.InitializeNewComponent(defaultValues);

            if (string.IsNullOrEmpty(Panel.Key))
            {
                // Use the name already assigned by the designer host (e.g. "dockPanel1")
                string key = Panel.Site?.Name ?? "panel" + Guid.NewGuid().ToString("N").Substring(0, 6);
                SetPanelProperty(nameof(DockPanel.Key), key);
                SetPanelProperty(nameof(DockPanel.Title), key);
                Debug.WriteLine($"[DockPanelDesigner] InitializeNewComponent: key = {key}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_changeService != null)
                    _changeService.ComponentChanged -= OnDesignerComponentChanged;

                if (_selectionService != null)
                    _selectionService.SelectionChanged -= OnDesignerSelectionChanged;

                _panel = null;
                _designerHost = null;
                _changeService = null;
                _selectionService = null;
            }
            base.Dispose(disposing);
        }

        public override SelectionRules SelectionRules =>
            SelectionRules.Visible | SelectionRules.Moveable | SelectionRules.AllSizeable;

        protected override bool GetHitTest(Point point)
        {
            return _selectedByDesigner;
        }

        private const int WM_LBUTTONDOWN = 0x0201;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN && !_selectedByDesigner)
                _selectionService?.SetSelectedComponents(new object[] { Panel }, SelectionTypes.Replace);

            base.WndProc(ref m);
        }

        // ── Internal helpers used by DockPanelActionList ─────────────────────────

        /// <summary>
        /// Sets a property on the managed DockPanel through TypeDescriptor so
        /// the change is tracked for undo and .designer.cs regeneration.
        /// </summary>
        internal void SetPanelProperty(string propertyName, object value)
        {
            BeepDockingDesignerWiring.SetProperty(Panel, propertyName, value, AsServiceProvider);
            if (propertyName == nameof(DockPanel.DockPosition) ||
                propertyName == nameof(DockPanel.PreferredWidth) ||
                propertyName == nameof(DockPanel.PreferredHeight))
            {
                BeepDockingDesignerWiring.RefreshHostLayout(Panel.Manager, AsServiceProvider);
            }
        }

        // ── Private helpers ──────────────────────────────────────────────────────

        private void ApplyDockPosition(DockPosition position) =>
            BeepDockingDesignerWiring.MovePanel(Panel, position, AsServiceProvider);

        internal void MovePanel(DockPosition position) =>
            BeepDockingDesignerWiring.MovePanel(Panel, position, AsServiceProvider);

        internal void StackWithPreviousPanel()
        {
            DockPanel target = BeepDockingDesignerWiring.GetPreviousPanel(Panel, AsServiceProvider);
            if (target != null)
                BeepDockingDesignerWiring.StackPanel(Panel, target, AsServiceProvider);
        }

        internal void StackWithNextPanel()
        {
            DockPanel target = BeepDockingDesignerWiring.GetNextPanel(Panel, AsServiceProvider);
            if (target != null)
                BeepDockingDesignerWiring.StackPanel(Panel, target, AsServiceProvider);
        }

        internal void MoveEarlierInStack() =>
            BeepDockingDesignerWiring.MovePanelInStack(Panel, -1, AsServiceProvider);

        internal void MoveLaterInStack() =>
            BeepDockingDesignerWiring.MovePanelInStack(Panel, 1, AsServiceProvider);

        private void OnDesignerComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            if (_snappingFromDesignerMove || IsDesignerLoading || !ReferenceEquals(e.Component, Panel) || e.Member == null)
                return;

            string memberName = e.Member.Name;
            if (memberName != nameof(Control.Bounds) &&
                memberName != nameof(Control.Location) &&
                memberName != nameof(Control.Size))
            {
                return;
            }

            if (IsResizeChange(e))
                ResizeDockedPanelFromDesigner();
            else
                SnapMovedPanelToDockTarget();
        }

        private void OnDesignerSelectionChanged(object sender, EventArgs e)
        {
            _selectedByDesigner = ReferenceEquals(_selectionService?.PrimarySelection, Panel);
            if (_selectedByDesigner)
                BeepDockingDesignerWiring.ActivateDesignPanel(Panel);
        }

        private void SnapMovedPanelToDockTarget()
        {
            if (Panel?.Manager == null || Panel.Parent == null)
                return;

            try
            {
                _snappingFromDesignerMove = true;

                DockPanel target = FindPanelUnderCenter();
                if (target != null)
                {
                    BeepDockingDesignerWiring.StackPanel(Panel, target, AsServiceProvider);
                    return;
                }

                if (ShouldFloatPanel())
                {
                    BeepDockingDesignerWiring.FloatPanel(Panel, AsServiceProvider);
                    return;
                }

                BeepDockingDesignerWiring.MovePanel(Panel, ResolveNearestDockPosition(), AsServiceProvider);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DockPanelDesigner] Snap moved panel failed: {ex.Message}");
            }
            finally
            {
                _snappingFromDesignerMove = false;
            }
        }

        private bool ShouldFloatPanel()
        {
            Form hostForm = Panel.FindForm();
            if (hostForm == null)
                return false;

            Rectangle clientArea = hostForm.ClientRectangle;
            Point center = new Point(Panel.Left + Panel.Width / 2, Panel.Top + Panel.Height / 2);

            int margin = 40;
            return center.X < clientArea.Left - margin ||
                   center.X > clientArea.Right + margin ||
                   center.Y < clientArea.Top - margin ||
                   center.Y > clientArea.Bottom + margin;
        }

        private void ResizeDockedPanelFromDesigner()
        {
            if (Panel?.Manager == null)
                return;

            try
            {
                _snappingFromDesignerMove = true;
                BeepDockingDesignerWiring.ResizePanel(Panel, AsServiceProvider);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DockPanelDesigner] Resize panel failed: {ex.Message}");
            }
            finally
            {
                _snappingFromDesignerMove = false;
            }
        }

        private static bool IsResizeChange(ComponentChangedEventArgs e)
        {
            if (e.Member?.Name == nameof(Control.Size))
                return true;

            if (e.Member?.Name != nameof(Control.Bounds))
                return false;

            if (e.OldValue is Rectangle oldBounds && e.NewValue is Rectangle newBounds)
                return oldBounds.Size != newBounds.Size;

            return false;
        }

        private DockPanel FindPanelUnderCenter()
        {
            Point center = new Point(Panel.Left + Panel.Width / 2, Panel.Top + Panel.Height / 2);
            return BeepDockingDesignerWiring.GetPanelsFor(Panel.Manager, AsServiceProvider)
                .Where(p => !ReferenceEquals(p, Panel) && p.Parent == Panel.Parent && p.Bounds.Contains(center))
                .OrderByDescending(p => p.Bounds.Width * p.Bounds.Height)
                .FirstOrDefault();
        }

        private DockPosition ResolveNearestDockPosition()
        {
            Rectangle host = Panel.Parent?.ClientRectangle ?? Rectangle.Empty;
            if (host.Width <= 0 || host.Height <= 0)
                return Panel.DockPosition;

            Point center = new Point(Panel.Left + Panel.Width / 2, Panel.Top + Panel.Height / 2);
            int left = Math.Abs(center.X - host.Left);
            int right = Math.Abs(host.Right - center.X);
            int top = Math.Abs(center.Y - host.Top);
            int bottom = Math.Abs(host.Bottom - center.Y);

            int min = Math.Min(Math.Min(left, right), Math.Min(top, bottom));
            if (min == left) return DockPosition.Left;
            if (min == right) return DockPosition.Right;
            if (min == top) return DockPosition.Top;
            return DockPosition.Bottom;
        }

        private void RemovePanel()
        {
            try
            {
                BeepDockingDesignerWiring.RemovePanel(Panel, AsServiceProvider);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DockPanelDesigner] Error removing panel: {ex.Message}");
            }
        }

        private object GetService(Type serviceType) => base.GetService(serviceType);

        private IServiceProvider AsServiceProvider => new ServiceProviderAdapter(base.GetService);

        private bool IsDesignerLoading => _designerHost?.Loading == true;
    }

    file sealed class ServiceProviderAdapter : IServiceProvider
    {
        private readonly Func<Type, object> _getter;
        internal ServiceProviderAdapter(Func<Type, object> getter) => _getter = getter;
        public object GetService(Type serviceType) => _getter(serviceType);
    }
}
