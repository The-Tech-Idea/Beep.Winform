using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking
{
    /// <summary>
    /// Named layout perspectives — several complete arrangements, switched by name.
    /// </summary>
    /// <remarks>
    /// The arrangement a developer wants for writing code is not the one they want for debugging or
    /// reviewing a diff. This is the same feature as Rider's and Visual Studio's <i>Window
    /// Layouts</i>, Blender's <i>Workspaces</i> and Eclipse's <i>Perspectives</i>.
    /// <para>
    /// Everything here routes through <see cref="CaptureDefinition"/> and
    /// <see cref="MaterializeFromDefinition"/> rather than a parallel storage mechanism, so a
    /// perspective inherits schema versioning, degradation for panels that no longer exist, and
    /// hidden-panel membership without restating any of it.
    /// </para>
    /// </remarks>
    public partial class BeepDockingManager
    {
        private readonly List<DockPerspective> _perspectives = new List<DockPerspective>();
        private string _activePerspectiveName;

        /// <summary>The stored perspectives, in the order they were saved.</summary>
        [Browsable(false)]
        public IReadOnlyList<DockPerspective> Perspectives => _perspectives;

        /// <summary>
        /// Name of the perspective currently applied, or <c>null</c> if the arrangement did not come
        /// from one.
        /// </summary>
        [Browsable(false)]
        public string ActivePerspectiveName => _activePerspectiveName;

        /// <summary>Raised after a perspective is applied. Exactly once per switch.</summary>
        public event EventHandler<DockPerspective> PerspectiveApplied;

        /// <summary>Raised after the set of stored perspectives changes.</summary>
        public event EventHandler PerspectivesChanged;

        /// <summary>Finds a perspective by name, or <c>null</c>.</summary>
        public DockPerspective GetPerspective(string name)
            => string.IsNullOrWhiteSpace(name)
               ? null
               : _perspectives.FirstOrDefault(
                     p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Captures the current arrangement under <paramref name="name"/>, replacing any perspective
        /// already stored under it, and makes it the active perspective.
        /// </summary>
        /// <returns>The stored perspective, or <c>null</c> if the name was blank.</returns>
        public DockPerspective SavePerspective(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var existing = GetPerspective(name);
            if (existing != null)
            {
                existing.Layout = CaptureDefinition();
            }
            else
            {
                existing = new DockPerspective { Name = name, Layout = CaptureDefinition() };
                _perspectives.Add(existing);
            }

            _activePerspectiveName = existing.Name;
            OnPerspectivesChanged();
            return existing;
        }

        /// <summary>
        /// Switches to the perspective stored under <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Perspective to apply.</param>
        /// <param name="captureCurrent">
        /// When true (the default) the current arrangement is first captured back into the active
        /// perspective, so adjustments made since the last switch are not lost. Pass false for a
        /// deliberately pristine restore that discards them.
        /// </param>
        /// <remarks>
        /// Capture-before-switch is the part products get wrong first: without it a user who nudges
        /// a splitter, switches away and switches back finds their adjustment silently gone. It only
        /// captures into a perspective that is actually active — an arrangement that did not come
        /// from a perspective has nowhere to be captured to, and inventing a slot for it would
        /// create perspectives the user never asked for.
        /// </remarks>
        public bool ApplyPerspective(string name, bool captureCurrent = true)
        {
            var target = GetPerspective(name);
            if (target == null)
                return false;

            if (captureCurrent)
            {
                // Applying the perspective you are already in, having captured your changes into
                // it, would be a no-op that still tears the layout down and rebuilds it. Skip it.
                if (string.Equals(_activePerspectiveName, target.Name, StringComparison.OrdinalIgnoreCase))
                {
                    CaptureIntoActivePerspective();
                    return true;
                }

                CaptureIntoActivePerspective();
            }

            MaterializeFromDefinition(target.Layout);
            _activePerspectiveName = target.Name;

            PerspectiveApplied?.Invoke(this, target);
            return true;
        }

        /// <summary>
        /// Discards adjustments made since the active perspective was last saved and restores it as
        /// stored.
        /// </summary>
        public bool RevertPerspective()
            => _activePerspectiveName != null &&
               ApplyPerspectiveIgnoringActive(_activePerspectiveName);

        /// <summary>Removes a stored perspective. The arrangement on screen is untouched.</summary>
        public bool DeletePerspective(string name)
        {
            var target = GetPerspective(name);
            if (target == null)
                return false;

            _perspectives.Remove(target);

            if (string.Equals(_activePerspectiveName, target.Name, StringComparison.OrdinalIgnoreCase))
                _activePerspectiveName = null;

            OnPerspectivesChanged();
            return true;
        }

        /// <summary>
        /// Marks <paramref name="name"/> as the default, clearing the flag on any other. At most one
        /// perspective is the default.
        /// </summary>
        public bool SetDefaultPerspective(string name)
        {
            var target = GetPerspective(name);
            if (target == null)
                return false;

            foreach (var p in _perspectives)
                p.IsDefault = ReferenceEquals(p, target);

            OnPerspectivesChanged();
            return true;
        }

        /// <summary>
        /// Restores the perspective marked default, discarding the current arrangement.
        /// </summary>
        /// <remarks>
        /// Pristine by design: "reset my layout" means the arrangement the user is trying to escape
        /// must not be captured on the way out.
        /// </remarks>
        public bool ApplyDefaultPerspective()
        {
            var fallback = _perspectives.FirstOrDefault(p => p.IsDefault);
            return fallback != null && ApplyPerspective(fallback.Name, captureCurrent: false);
        }

        /// <summary>
        /// Applies a perspective by position — Visual Studio's <c>Ctrl+Alt+1..9</c>.
        /// </summary>
        public bool ApplyPerspectiveByIndex(int index)
            => index >= 0 && index < _perspectives.Count &&
               ApplyPerspective(_perspectives[index].Name);

        /// <summary>Re-applies a perspective even when it is already active.</summary>
        private bool ApplyPerspectiveIgnoringActive(string name)
        {
            var target = GetPerspective(name);
            if (target == null)
                return false;

            MaterializeFromDefinition(target.Layout);
            _activePerspectiveName = target.Name;

            PerspectiveApplied?.Invoke(this, target);
            return true;
        }

        /// <summary>Writes the current arrangement back into the active perspective, if any.</summary>
        private void CaptureIntoActivePerspective()
        {
            var active = GetPerspective(_activePerspectiveName);
            if (active != null)
                active.Layout = CaptureDefinition();
        }

        private void OnPerspectivesChanged()
            => PerspectivesChanged?.Invoke(this, EventArgs.Empty);
    }
}
