using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Backstage
{
    public sealed class RibbonRecentItemModel
    {
        public string Key { get; set; } = string.Empty;
        public string DisplayText { get; set; } = string.Empty;
        public string? SubText { get; set; }
        public string? ToolTip { get; set; }
        public string? ImagePath { get; set; }
        public string? FilePath { get; set; }
        public DateTime? LastOpenedUtc { get; set; }
        public bool IsPinned { get; set; }

        public SimpleItem ToSimpleItem()
        {
            string text = string.IsNullOrWhiteSpace(DisplayText) ? (FilePath ?? Key) : DisplayText;
            string key = string.IsNullOrWhiteSpace(Key) ? (FilePath ?? text) : Key;
            string tooltip = ToolTip ?? FilePath ?? text;

            return new SimpleItem
            {
                Text = text,
                SubText = SubText,
                SubText3 = LastOpenedUtc.HasValue ? LastOpenedUtc.Value.ToString("O") : string.Empty,
                ToolTip = tooltip,
                ActionID = key,
                ReferenceID = key,
                Uri = FilePath,
                ImagePath = ImagePath,
                IsVisible = true,
                IsEnabled = true
            };
        }
    }
}
