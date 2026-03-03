using System;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.CommandPalette
{
    public sealed class CommandAction
    {
        public string Text { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public string ShortcutText { get; set; } = string.Empty;
        public Action? Action { get; set; }
        public bool IsFavorite { get; set; }
    }
}
