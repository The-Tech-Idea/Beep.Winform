using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Models
{
    /// <summary>
    /// A named arrangement of the docking host — Rider's and Visual Studio's <i>Window Layouts</i>,
    /// Blender's <i>Workspaces</i>, Eclipse's <i>Perspectives</i>.
    /// </summary>
    /// <remarks>
    /// A perspective is deliberately nothing more than a <see cref="DockLayoutDefinition"/> with a
    /// name. Everything persistence already guarantees — schema versioning, degradation for panels
    /// that no longer exist, hidden-panel membership — applies to a perspective unchanged, because
    /// it goes through the same materialiser rather than a parallel one.
    /// </remarks>
    public sealed class DockPerspective
    {
        /// <summary>Name the perspective is applied by. Compared ordinal, case-insensitively.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>The arrangement itself.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DockLayoutDefinition Layout { get; set; } = new DockLayoutDefinition();

        /// <summary>
        /// True for the perspective <see cref="BeepDockingManager.ApplyDefaultPerspective"/> restores.
        /// At most one perspective carries this; setting it on another clears the previous.
        /// </summary>
        public bool IsDefault { get; set; }

        public override string ToString()
            => IsDefault ? $"{Name} (default)" : Name;
    }
}
