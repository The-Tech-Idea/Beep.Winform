using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Docking.Infrastructure
{
    /// <summary>
    /// Design-time wiring helpers for the Beep docking system.
    /// Follows the same pattern as BeepMdiDesignerWiring:
    /// all mutations go through IComponentChangeService so the designer
    /// records undo steps and regenerates .designer.cs correctly.
    /// </summary>
    internal static class BeepDockingDesignerWiring
    {
        /// <summary>
        /// Creates a new DockPanel component, assigns a unique key and manager,
        /// and registers it with the designer host so it appears in .designer.cs.
        /// </summary>
        public static DockPanel AddPanel(
            BeepDockingManager manager,
            DockPosition position,
            IServiceProvider services)
        {
            IDesignerHost host = services?.GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (host == null || manager == null) return null;

            using (DesignerTransaction tx = host.CreateTransaction("Add Dock Panel"))
            {
                DockPanel panel = host.CreateComponent(typeof(DockPanel)) as DockPanel;
                if (panel == null) { tx.Cancel(); return null; }

                // Generate a unique key using the component name assigned by the host
                string key = panel.Site?.Name ?? "panel" + Guid.NewGuid().ToString("N").Substring(0, 6);

                SetProperty(panel, nameof(DockPanel.Key), key, services);
                SetProperty(panel, nameof(DockPanel.Title), key, services);
                SetProperty(panel, nameof(DockPanel.DockPosition), position, services);
                SetProperty(panel, nameof(DockPanel.Manager), manager, services);

                Form hostForm = GetHostForm(manager, services);
                if (hostForm != null)
                {
                    BeepDockspace dockspace = GetOrCreateDockspace(manager, position, services);
                    AddDockspaceToHostForm(hostForm, dockspace, services);
                    AddPanelToDockspace(dockspace, panel, services);
                    RefreshHostLayout(manager, services);
                }

                SelectComponent(panel, services);
                tx.Commit();
                return panel;
            }
        }

        /// <summary>
        /// Removes a DockPanel component from the designer host container,
        /// which removes it from .designer.cs.
        /// </summary>
        public static void RemovePanel(DockPanel panel, IServiceProvider services)
        {
            IDesignerHost host = services?.GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (host == null || panel == null) return;

            BeepDockingManager manager = panel.Manager;
            using (DesignerTransaction tx = host.CreateTransaction("Remove Dock Panel"))
            {
                RemovePanelFromParent(panel, services);
                host.DestroyComponent(panel);
                if (manager != null)
                    RefreshHostLayout(manager, services);

                tx.Commit();
            }
        }

        public static void MovePanel(DockPanel panel, DockPosition position, IServiceProvider services)
        {
            IDesignerHost host = services?.GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (host == null || panel == null) return;

            using (DesignerTransaction tx = host.CreateTransaction("Move Dock Panel"))
            {
                SetProperty(panel, nameof(DockPanel.DockPosition), position, services);
                BeepDockspace dockspace = GetOrCreateDockspace(panel.Manager, position, services);
                if (dockspace != null)
                    AddPanelToDockspace(dockspace, panel, services);
                RefreshHostLayout(panel.Manager, services);
                SelectComponent(panel, services);
                tx.Commit();
            }
        }

        public static void StackPanel(DockPanel panel, DockPanel target, IServiceProvider services)
        {
            IDesignerHost host = services?.GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (host == null || panel == null || target == null || ReferenceEquals(panel, target)) return;

            using (DesignerTransaction tx = host.CreateTransaction("Stack Dock Panel"))
            {
                if (!ReferenceEquals(panel.Manager, target.Manager))
                    SetProperty(panel, nameof(DockPanel.Manager), target.Manager, services);

                SetProperty(panel, nameof(DockPanel.DockPosition), target.DockPosition, services);
                if (target.Parent is BeepDockspace targetDockspace)
                    AddPanelToDockspace(targetDockspace, panel, services);
                MovePanelNear(panel, target, services);
                RefreshHostLayout(target.Manager, services);
                SelectComponent(panel, services);
                tx.Commit();
            }
        }

        public static void MovePanelInStack(DockPanel panel, int delta, IServiceProvider services)
        {
            IDesignerHost host = services?.GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (host == null || panel?.Manager == null || delta == 0) return;

            using (DesignerTransaction tx = host.CreateTransaction("Reorder Dock Panel Stack"))
            {
                var stack = GetStackPanels(panel, services);
                int current = stack.IndexOf(panel);
                if (current >= 0)
                {
                    int next = Math.Max(0, Math.Min(stack.Count - 1, current + delta));
                    if (current != next)
                    {
                        stack.RemoveAt(current);
                        stack.Insert(next, panel);
                        ApplyStackOrder(stack, services);
                    }
                }

                RefreshHostLayout(panel.Manager, services);
                SelectComponent(panel, services);
                tx.Commit();
            }
        }

        public static DockPanel GetPreviousPanel(DockPanel panel, IServiceProvider services)
        {
            if (panel?.Manager == null) return null;
            var panels = GetStackPanels(panel, services);
            int index = panels.IndexOf(panel);
            return index > 0 ? panels[index - 1] : null;
        }

        public static DockPanel GetNextPanel(DockPanel panel, IServiceProvider services)
        {
            if (panel?.Manager == null) return null;
            var panels = GetStackPanels(panel, services);
            int index = panels.IndexOf(panel);
            return index >= 0 && index < panels.Count - 1 ? panels[index + 1] : null;
        }

        public static DockPanel GetSelectedPanel(IServiceProvider services)
        {
            ISelectionService selection = services?.GetService(typeof(ISelectionService)) as ISelectionService;
            return selection?.PrimarySelection as DockPanel;
        }

        public static void ResizePanel(DockPanel panel, IServiceProvider services)
        {
            if (panel?.Manager == null)
                return;

            BeepDockspace dockspace = panel.Parent as BeepDockspace;
            DockPosition position = dockspace?.DockPosition ?? panel.DockPosition;
            int extent = ResolvePanelResizeExtent(panel, dockspace, position);
            ApplyPreferredExtent(GetResizeStack(panel, dockspace, services), position, extent, services);
            RefreshHostLayout(panel.Manager, services);
            SelectComponent(panel, services);
        }

        public static void ResizeDockspace(BeepDockspace dockspace, IServiceProvider services)
        {
            if (dockspace?.Manager == null)
                return;

            var panels = dockspace.Panels.ToList();
            if (panels.Count == 0)
                return;

            int extent = ResolveDockspaceResizeExtent(dockspace);
            ApplyPreferredExtent(panels, dockspace.DockPosition, extent, services);
            RefreshHostLayout(dockspace.Manager, services);

            DockPanel active = dockspace.ActivePanel ?? panels.FirstOrDefault();
            if (active != null)
                SelectComponent(active, services);
        }

        public static void DropPanelAt(DockPanel panel, Point screenPoint, IServiceProvider services)
        {
            if (panel?.Manager == null)
                return;

            IDesignerHost host = services?.GetService(typeof(IDesignerHost)) as IDesignerHost;
            Form hostForm = GetHostForm(panel.Manager, services);
            if (host == null || hostForm == null)
                return;

            foreach (BeepDockspace dockspace in host.Container.Components
                         .OfType<BeepDockspace>()
                         .Where(space => ReferenceEquals(space.Manager, panel.Manager)))
            {
                Rectangle screenBounds = new Rectangle(dockspace.PointToScreen(Point.Empty), dockspace.Size);
                if (!screenBounds.Contains(screenPoint))
                    continue;

                Point clientPoint = dockspace.PointToClient(screenPoint);
                DockPanel target = dockspace.HitTestTabAt(clientPoint)
                                   ?? dockspace.ActivePanel
                                   ?? dockspace.Panels.FirstOrDefault();

                if (target != null && !ReferenceEquals(target, panel))
                    StackPanel(panel, target, services);
                else
                    MovePanel(panel, dockspace.DockPosition, services);

                return;
            }

            Point hostPoint = hostForm.PointToClient(screenPoint);
            MovePanel(panel, ResolveDockPositionFromPoint(hostPoint, hostForm.DisplayRectangle), services);
        }

        /// <summary>
        /// Returns all DockPanel components in the designer container that
        /// belong to the given manager.
        /// </summary>
        public static System.Collections.Generic.List<DockPanel> GetPanelsFor(
            BeepDockingManager manager,
            IServiceProvider services)
        {
            IDesignerHost host = services?.GetService(typeof(IDesignerHost)) as IDesignerHost;
            return host?.Container.Components
                .OfType<DockPanel>()
                .Where(p => p.Manager == manager)
                .ToList()
                ?? new System.Collections.Generic.List<DockPanel>();
        }

        /// <summary>
        /// Sets a property on a component through TypeDescriptor so the
        /// IComponentChangeService records the change for undo and .designer.cs codegen.
        /// Mirrors BeepMdiDesignerWiring.SetProperty exactly.
        /// </summary>
        public static void SetProperty(object target, string name, object value, IServiceProvider services)
        {
            PropertyDescriptor property = TypeDescriptor.GetProperties(target)[name];
            if (property == null || property.IsReadOnly) return;

            IComponentChangeService changes = services?.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            changes?.OnComponentChanging(target, property);
            property.SetValue(target, value);
            changes?.OnComponentChanged(target, property, null, value);
        }

        public static void RefreshHostLayout(BeepDockingManager manager, IServiceProvider services)
        {
            Form hostForm = GetHostForm(manager, services);
            if (manager == null || hostForm == null) return;

            Rectangle remaining = hostForm.DisplayRectangle;
            if (remaining.Width <= 0 || remaining.Height <= 0)
                remaining = new Rectangle(0, 0, 800, 450);

            var panels = GetPanelsFor(manager, services);

            var left = panels.Where(p => p.DockPosition == DockPosition.Left).OrderBy(p => p.TabIndex).ToList();
            if (left.Count > 0)
            {
                int width = ClampExtent(left.Max(p => p.PreferredWidth), remaining.Width, 80);
                SetStackBounds(manager, DockPosition.Left, left, new Rectangle(remaining.Left, remaining.Top, width, remaining.Height), services);
                remaining = new Rectangle(remaining.Left + width, remaining.Top, Math.Max(0, remaining.Width - width), remaining.Height);
            }

            var right = panels.Where(p => p.DockPosition == DockPosition.Right).OrderBy(p => p.TabIndex).ToList();
            if (right.Count > 0)
            {
                int width = ClampExtent(right.Max(p => p.PreferredWidth), remaining.Width, 80);
                SetStackBounds(manager, DockPosition.Right, right, new Rectangle(remaining.Right - width, remaining.Top, width, remaining.Height), services);
                remaining = new Rectangle(remaining.Left, remaining.Top, Math.Max(0, remaining.Width - width), remaining.Height);
            }

            var top = panels.Where(p => p.DockPosition == DockPosition.Top).OrderBy(p => p.TabIndex).ToList();
            if (top.Count > 0)
            {
                int height = ClampExtent(top.Max(p => p.PreferredHeight), remaining.Height, 60);
                SetStackBounds(manager, DockPosition.Top, top, new Rectangle(remaining.Left, remaining.Top, remaining.Width, height), services);
                remaining = new Rectangle(remaining.Left, remaining.Top + height, remaining.Width, Math.Max(0, remaining.Height - height));
            }

            var bottom = panels.Where(p => p.DockPosition == DockPosition.Bottom).OrderBy(p => p.TabIndex).ToList();
            if (bottom.Count > 0)
            {
                int height = ClampExtent(bottom.Max(p => p.PreferredHeight), remaining.Height, 60);
                SetStackBounds(manager, DockPosition.Bottom, bottom, new Rectangle(remaining.Left, remaining.Bottom - height, remaining.Width, height), services);
                remaining = new Rectangle(remaining.Left, remaining.Top, remaining.Width, Math.Max(0, remaining.Height - height));
            }

            SetStackBounds(manager, DockPosition.Fill, panels.Where(p => p.DockPosition == DockPosition.Fill).OrderBy(p => p.TabIndex).ToList(), remaining, services);
        }

        private static int ClampExtent(int requested, int available, int minimum)
        {
            int safeAvailable = Math.Max(minimum, available);
            return Math.Min(Math.Max(minimum, requested), safeAvailable);
        }

        private static int ResolvePanelResizeExtent(DockPanel panel, BeepDockspace dockspace, DockPosition position)
        {
            switch (position)
            {
                case DockPosition.Left:
                case DockPosition.Right:
                    return Math.Max(80, panel.Width > 0 ? panel.Width : dockspace?.Width ?? panel.PreferredWidth);
                case DockPosition.Top:
                case DockPosition.Bottom:
                    int contentHeight = panel.Height > 0 ? panel.Height : Math.Max(0, (dockspace?.Height ?? panel.PreferredHeight) - BeepDockspace.HeaderHeight);
                    return Math.Max(60, contentHeight + BeepDockspace.HeaderHeight);
                default:
                    return 0;
            }
        }

        private static int ResolveDockspaceResizeExtent(BeepDockspace dockspace)
        {
            switch (dockspace.DockPosition)
            {
                case DockPosition.Left:
                case DockPosition.Right:
                    return Math.Max(80, dockspace.Width);
                case DockPosition.Top:
                case DockPosition.Bottom:
                    return Math.Max(60, dockspace.Height);
                default:
                    return 0;
            }
        }

        private static IList<DockPanel> GetResizeStack(DockPanel panel, BeepDockspace dockspace, IServiceProvider services)
        {
            if (dockspace != null)
                return dockspace.Panels.ToList();

            return GetPanelsFor(panel.Manager, services)
                .Where(p => p.DockPosition == panel.DockPosition)
                .ToList();
        }

        private static void ApplyPreferredExtent(IList<DockPanel> panels, DockPosition position, int extent, IServiceProvider services)
        {
            if (panels == null || panels.Count == 0 || extent <= 0)
                return;

            foreach (DockPanel panel in panels)
            {
                switch (position)
                {
                    case DockPosition.Left:
                    case DockPosition.Right:
                        SetProperty(panel, nameof(DockPanel.PreferredWidth), extent, services);
                        break;
                    case DockPosition.Top:
                    case DockPosition.Bottom:
                        SetProperty(panel, nameof(DockPanel.PreferredHeight), extent, services);
                        break;
                }
            }
        }

        private static DockPosition ResolveDockPositionFromPoint(Point point, Rectangle bounds)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
                return DockPosition.Fill;

            int horizontalBand = Math.Max(80, bounds.Width / 4);
            int verticalBand = Math.Max(60, bounds.Height / 4);

            if (point.X <= bounds.Left + horizontalBand)
                return DockPosition.Left;
            if (point.X >= bounds.Right - horizontalBand)
                return DockPosition.Right;
            if (point.Y <= bounds.Top + verticalBand)
                return DockPosition.Top;
            if (point.Y >= bounds.Bottom - verticalBand)
                return DockPosition.Bottom;

            return DockPosition.Fill;
        }

        private static void SetStackBounds(
            BeepDockingManager manager,
            DockPosition position,
            IList<DockPanel> panels,
            Rectangle bounds,
            IServiceProvider services)
        {
            if (panels == null || panels.Count == 0)
                return;

            BeepDockspace dockspace = GetOrCreateDockspace(manager, position, services);
            Form hostForm = GetHostForm(manager, services);
            AddDockspaceToHostForm(hostForm, dockspace, services);
            SetProperty(dockspace, nameof(Control.Bounds), bounds, services);
            SetProperty(dockspace, nameof(BeepDockspace.DockPosition), position, services);
            SetProperty(dockspace, nameof(BeepDockspace.Manager), manager, services);

            foreach (DockPanel panel in panels)
            {
                AddPanelToDockspace(dockspace, panel, services);
            }

            dockspace.SyncPages();
        }

        private static Form GetHostForm(BeepDockingManager manager, IServiceProvider services)
        {
            IDesignerHost host = services?.GetService(typeof(IDesignerHost)) as IDesignerHost;
            return host?.RootComponent as Form ?? manager?.HostForm;
        }

        private static BeepDockspace GetOrCreateDockspace(
            BeepDockingManager manager,
            DockPosition position,
            IServiceProvider services)
        {
            IDesignerHost host = services?.GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (host == null || manager == null)
                return null;

            BeepDockspace dockspace = host.Container.Components
                .OfType<BeepDockspace>()
                .FirstOrDefault(space => ReferenceEquals(space.Manager, manager) && space.DockPosition == position);

            if (dockspace != null)
                return dockspace;

            dockspace = host.CreateComponent(typeof(BeepDockspace)) as BeepDockspace;
            if (dockspace == null)
                return null;

            SetProperty(dockspace, nameof(BeepDockspace.Manager), manager, services);
            SetProperty(dockspace, nameof(BeepDockspace.DockPosition), position, services);
            return dockspace;
        }

        private static void AddDockspaceToHostForm(Form hostForm, BeepDockspace dockspace, IServiceProvider services)
        {
            if (hostForm == null || dockspace == null || dockspace.Parent == hostForm)
                return;

            if (dockspace.Parent != null)
                RemoveControlFromParent(dockspace, services);

            IComponentChangeService changes = services?.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            PropertyDescriptor controlsProperty = TypeDescriptor.GetProperties(hostForm)[nameof(Control.Controls)];
            changes?.OnComponentChanging(hostForm, controlsProperty);
            hostForm.Controls.Add(dockspace);
            dockspace.BringToFront();
            changes?.OnComponentChanged(hostForm, controlsProperty, null, hostForm.Controls);
        }

        private static void AddPanelToDockspace(BeepDockspace dockspace, DockPanel panel, IServiceProvider services)
        {
            if (dockspace == null || panel == null || panel.Parent == dockspace)
                return;

            if (panel.Parent != null)
                RemoveControlFromParent(panel, services);

            IComponentChangeService changes = services?.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            PropertyDescriptor controlsProperty = TypeDescriptor.GetProperties(dockspace)[nameof(Control.Controls)];
            changes?.OnComponentChanging(dockspace, controlsProperty);
            dockspace.Controls.Add(panel);
            changes?.OnComponentChanged(dockspace, controlsProperty, null, dockspace.Controls);
            dockspace.ActivatePanel(panel);
        }

        private static void SelectComponent(IComponent component, IServiceProvider services)
        {
            if (component is DockPanel panel)
            {
                if (panel.Parent is BeepDockspace dockspace)
                    dockspace.ActivatePanel(panel);
                else
                    panel.BringToFront();

                InvalidateStack(panel);
            }

            ISelectionService selection = services?.GetService(typeof(ISelectionService)) as ISelectionService;
            selection?.SetSelectedComponents(new[] { component }, SelectionTypes.Replace);
        }

        internal static void ActivateDesignPanel(DockPanel panel)
        {
            if (panel == null)
                return;

            if (panel.Parent is BeepDockspace dockspace)
                dockspace.ActivatePanel(panel);
            else
                panel.BringToFront();

            InvalidateStack(panel);
        }

        private static void InvalidateStack(DockPanel panel)
        {
            if (panel?.Parent == null)
                return;

            Control stackParent = panel.Parent;
            foreach (DockPanel stackPanel in stackParent.Controls
                         .OfType<DockPanel>()
                         .Where(p => ReferenceEquals(p.Manager, panel.Manager) &&
                                     p.DockPosition == panel.DockPosition))
            {
                stackPanel.Invalidate();
            }

            stackParent.Invalidate();
        }

        private static List<DockPanel> GetStackPanels(DockPanel panel, IServiceProvider services)
        {
            return GetPanelsFor(panel.Manager, services)
                .Where(p => p.DockPosition == panel.DockPosition)
                .OrderBy(p => p.TabIndex)
                .ToList();
        }

        private static void ApplyStackOrder(IList<DockPanel> stack, IServiceProvider services)
        {
            for (int i = 0; i < stack.Count; i++)
                SetProperty(stack[i], nameof(Control.TabIndex), i, services);
        }

        private static void MovePanelNear(DockPanel panel, DockPanel target, IServiceProvider services)
        {
            var stack = GetStackPanels(target, services);
            stack.Remove(panel);
            int targetIndex = stack.IndexOf(target);
            stack.Insert(targetIndex < 0 ? stack.Count : targetIndex + 1, panel);
            ApplyStackOrder(stack, services);
        }

        private static void RemovePanelFromParent(DockPanel panel, IServiceProvider services)
        {
            if (panel == null)
                return;

            RemoveControlFromParent(panel, services);
        }

        private static void RemoveControlFromParent(Control control, IServiceProvider services)
        {
            Control parent = control?.Parent;
            if (parent == null || control == null)
                return;

            IComponentChangeService changes = services?.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            PropertyDescriptor controlsProperty = TypeDescriptor.GetProperties(parent)[nameof(Control.Controls)];
            changes?.OnComponentChanging(parent, controlsProperty);
            parent.Controls.Remove(control);
            changes?.OnComponentChanged(parent, controlsProperty, null, parent.Controls);
        }
    }
}
