using System;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Models
{
    /// <summary>
    /// Which parts of an arrangement a save or load deals with.
    /// </summary>
    /// <remarks>
    /// The two are genuinely different things, and conflating them is what produces the behaviour
    /// where applying a saved arrangement to one window disturbs what the user had open in another.
    /// <list type="bullet">
    /// <item><b>Structure</b> — the shape: groups, split ratios, orientations, which panels belong
    /// where, which edges are auto-hidden, where floats sit. Two windows can sensibly share
    /// this.</item>
    /// <item><b>Session</b> — what the user was doing: which tab is active in each group, and which
    /// panels are hidden. Personal to one window and one moment.</item>
    /// </list>
    /// "Reset my layout but leave what I have open alone" is <see cref="Structure"/>; a normal
    /// save and restore is <see cref="All"/>.
    /// </remarks>
    [Flags]
    public enum DockLayoutScope
    {
        /// <summary>The arrangement's shape.</summary>
        Structure = 1,

        /// <summary>Active tabs and hidden panels.</summary>
        Session = 2,

        /// <summary>Both — the default for an ordinary save or restore.</summary>
        All = Structure | Session,
    }
}
