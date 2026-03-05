using System;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.CommandPalette
{
    public sealed class CommandAction
    {
        /// <summary>Display name shown in the palette list.</summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>Category used to group/filter actions (e.g. "File", "Edit", "View").</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>SVG / PNG image path rendered as a small icon left of the label.</summary>
        public string IconPath { get; set; } = string.Empty;

        /// <summary>Keyboard shortcut hint displayed right-aligned in the row (e.g. "Ctrl+K").</summary>
        public string ShortcutText { get; set; } = string.Empty;

        /// <summary>Callback executed when the action is selected.</summary>
        public Action? Action { get; set; }

        /// <summary>Marks the action as a favourite (pinned at the top of the list).</summary>
        public bool IsFavorite { get; set; }
    }
}
