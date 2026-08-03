using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;
using TheTechIdea.Beep.Winform.Controls.Docking.Runtime;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// The manager's relationship with the host form and with the controls it parents panels into.
    /// </summary>
    /// <remarks>
    /// Everything here answers "which Windows control is this panel actually a child of, and who is
    /// listening to the host". That is a genuinely separate concern from where a panel sits in the
    /// layout tree: the tree says a panel belongs to the Left group, this decides whether the
    /// control lives on a <see cref="BeepDockspace"/> or directly on the form, and keeps the host's
    /// event subscriptions balanced.
    /// <para>
    /// A change about parenting, hosting or host-form events belongs in this file. A change about
    /// where a panel sits relative to others does not.
    /// </para>
    /// </remarks>
    public partial class BeepDockingManager
    {
        /// <summary>
        /// Attaches the manager to a host form and activates the Win32 runtime path.
        /// Call this once from the form's Load event (or constructor after InitializeComponent).
        /// Mirrors KryptonDockingManager.ManageControl(Control).
        /// </summary>
        /// <param name="hostForm">The form that will host the docking layout.</param>
        public void ManageControl(Form hostForm)
        {
            if (hostForm == null) throw new ArgumentNullException(nameof(hostForm));
            if (_hostForm == hostForm)
            {
                if (!IsDesignHosted)
                {
                    RegisterDesignerCreatedPanels(hostForm);
                    RecalculateLayout();
                }
                return;
            }

            DetachHostFormHandlers();
            _hostForm = hostForm;

            if (IsDesignHosted)
            {
                Debug.WriteLine("[BeepDockingManager] ManageControl — design-time, Win32 deferred.");
                return;
            }

            AttachHostFormHandlers(hostForm);
            InitializeSubsystems();
            _layoutController.ContainerBounds = hostForm.DisplayRectangle;
            CreateAutoHideStrips();
            RegisterDesignerCreatedPanels(hostForm);

            // Apply a layout deserialized from the host *.Designer.cs (populated into the backing
            // LayoutDefinition during InitializeComponent) now that the panels are registered.
            if (_layoutDefinition != null && !_layoutDefinition.IsEmpty)
            {
                MaterializeFromDefinition(_layoutDefinition);
            }
            else
            {
                RecalculateLayout();
            }

            Debug.WriteLine($"[BeepDockingManager] ManageControl — attached to: {hostForm.Name}");
        }

        /// <summary>Subscribes to host resize so the layout engine tracks the client area.</summary>
        private void AttachHostFormHandlers(Form hostForm)
        {
            if (hostForm == null)
                return;

            _hostLayoutChangedHandler ??= OnHostFormLayoutChanged;
            hostForm.Resize += _hostLayoutChangedHandler;
            hostForm.ClientSizeChanged += _hostLayoutChangedHandler;

            _hostKeyDownHandler ??= OnHostFormKeyDown;
            _hostKeyUpHandler ??= OnHostFormKeyUp;
            hostForm.KeyPreview = true;
            hostForm.KeyDown += _hostKeyDownHandler;
            hostForm.KeyUp += _hostKeyUpHandler;
        }

        private void DetachHostFormHandlers()
        {
            if (_hostForm == null)
                return;

            if (_hostLayoutChangedHandler != null)
            {
                _hostForm.Resize -= _hostLayoutChangedHandler;
                _hostForm.ClientSizeChanged -= _hostLayoutChangedHandler;
            }

            if (_hostKeyDownHandler != null)
            {
                _hostForm.KeyDown -= _hostKeyDownHandler;
            }

            if (_hostKeyUpHandler != null)
            {
                _hostForm.KeyUp -= _hostKeyUpHandler;
            }
        }

        private void OnHostFormLayoutChanged(object sender, EventArgs e)
        {
            if (_hostForm == null || _hostForm.IsDisposed || _layoutController == null || _disposed || IsDesignHosted)
                return;

            _layoutController.ContainerBounds = GetLayoutClientBounds(_hostForm.DisplayRectangle);

            if (_updateDepth > 0)
                return;

            ApplyLayout();
        }

        /// <summary>
        /// Ensures <paramref name="panel"/> is parented to the correct dockspace (or host form as
        /// fallback). Called after state transitions that leave the panel orphaned (float→dock,
        /// auto-hide→restore, hidden→shown, closed→reopen). Activates the panel in the dockspace
        /// when <paramref name="makeActive"/> is true.
        /// </summary>
        private void EnsurePanelHosted(DockPanel panel, bool makeActive = false)
        {
            if (panel == null || _hostForm == null) return;
            var targetDs = FindDockspaceAt(_hostForm, panel.DockPosition);
            if (targetDs != null && panel.Parent != targetDs)
            {
                if (panel.Parent != null)
                    panel.Parent.Controls.Remove(panel);
                targetDs.Controls.Add(panel);
                if (makeActive)
                    targetDs.ActivePanelKey = panel.Key;
                targetDs.LayoutPanels();
            }
            else if (panel.Parent == null)
            {
                _hostForm.Controls.Add(panel);
            }
        }

        /// <summary>Removes the panel from its current parent (dockspace or host form), re-laying out
        /// the dockspace if needed. No-op when panel or its parent is null.</summary>
        private static void DetachPanelFromParent(DockPanel panel)
        {
            if (panel?.Parent == null) return;
            if (panel.Parent is BeepDockspace ds)
            {
                ds.Controls.Remove(panel);
                ds.LayoutPanels();
                ds.Invalidate();
            }
            else
            {
                panel.Parent.Controls.Remove(panel);
            }
        }
    }
}
