using TheTechIdea.Beep.Vis.Modules;
using System.IO;

namespace TheTechIdea.Beep.Winform.Controls.Tooltips
{
    public sealed class RibbonSuperTooltipModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Shortcut { get; set; } = string.Empty;
        public string Footer { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public string? ThumbnailPath { get; set; }
        public string ThumbnailCaption { get; set; } = string.Empty;

        public bool IsEmpty =>
            string.IsNullOrWhiteSpace(Title) &&
            string.IsNullOrWhiteSpace(Description) &&
            string.IsNullOrWhiteSpace(Shortcut) &&
            string.IsNullOrWhiteSpace(Footer) &&
            string.IsNullOrWhiteSpace(ThumbnailPath);

        public string ToPlainText()
        {
            var lines = new List<string>();
            if (!string.IsNullOrWhiteSpace(Title)) lines.Add(Title);
            if (!string.IsNullOrWhiteSpace(Description)) lines.Add(Description);
            if (!string.IsNullOrWhiteSpace(Shortcut)) lines.Add($"Shortcut: {Shortcut}");
            if (!string.IsNullOrWhiteSpace(Footer)) lines.Add(Footer);
            if (!string.IsNullOrWhiteSpace(ThumbnailCaption)) lines.Add(ThumbnailCaption);
            return string.Join(Environment.NewLine, lines);
        }

        public static RibbonSuperTooltipModel FromSimpleItem(SimpleItem item)
        {
            if (item == null)
            {
                return new RibbonSuperTooltipModel();
            }

            string title = !string.IsNullOrWhiteSpace(item.DisplayField)
                ? item.DisplayField
                : (!string.IsNullOrWhiteSpace(item.Text) ? item.Text : item.Name ?? string.Empty);

            string description = item.ToolTip ?? item.Description ?? item.SubText ?? string.Empty;
            string footer = item.SubText2 ?? item.SubText3 ?? string.Empty;
            string? thumbnailPath = null;
            string thumbnailCaption = string.Empty;

            if (TryParsePreviewMetadata(item.SubText3, out var previewPath, out var previewCaption))
            {
                thumbnailPath = previewPath;
                thumbnailCaption = previewCaption;
                footer = item.SubText2 ?? string.Empty;
            }
            else if (IsImageLocation(item.Uri))
            {
                thumbnailPath = item.Uri;
            }

            return new RibbonSuperTooltipModel
            {
                Title = title,
                Description = description,
                Shortcut = item.ShortcutText ?? string.Empty,
                Footer = footer,
                ImagePath = item.ImagePath,
                ThumbnailPath = thumbnailPath,
                ThumbnailCaption = thumbnailCaption
            };
        }

        private static bool TryParsePreviewMetadata(string? raw, out string thumbnailPath, out string caption)
        {
            thumbnailPath = string.Empty;
            caption = string.Empty;
            if (string.IsNullOrWhiteSpace(raw))
            {
                return false;
            }

            bool foundPath = false;
            string[] segments = raw.Split('|', StringSplitOptions.RemoveEmptyEntries);
            foreach (var segment in segments)
            {
                string token = segment.Trim();
                if (TryReadTokenValue(token, "preview", out var previewValue) ||
                    TryReadTokenValue(token, "thumbnail", out previewValue) ||
                    TryReadTokenValue(token, "thumb", out previewValue))
                {
                    thumbnailPath = previewValue;
                    foundPath = !string.IsNullOrWhiteSpace(previewValue);
                    continue;
                }

                if (TryReadTokenValue(token, "caption", out var captionValue))
                {
                    caption = captionValue;
                }
            }

            return foundPath;
        }

        private static bool TryReadTokenValue(string raw, string key, out string value)
        {
            value = string.Empty;
            if (string.IsNullOrWhiteSpace(raw))
            {
                return false;
            }

            if (raw.StartsWith(key + ":", StringComparison.OrdinalIgnoreCase))
            {
                value = raw[(key.Length + 1)..].Trim();
                return !string.IsNullOrWhiteSpace(value);
            }

            if (raw.StartsWith(key + "=", StringComparison.OrdinalIgnoreCase))
            {
                value = raw[(key.Length + 1)..].Trim();
                return !string.IsNullOrWhiteSpace(value);
            }

            return false;
        }

        private static bool IsImageLocation(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string path = value.Trim();
            int queryIndex = path.IndexOfAny(['?', '#']);
            if (queryIndex >= 0)
            {
                path = path[..queryIndex];
            }

            string ext = Path.GetExtension(path);
            if (string.IsNullOrWhiteSpace(ext))
            {
                return false;
            }

            return ext.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                   ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                   ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                   ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase) ||
                   ext.Equals(".gif", StringComparison.OrdinalIgnoreCase) ||
                   ext.Equals(".webp", StringComparison.OrdinalIgnoreCase) ||
                   ext.Equals(".svg", StringComparison.OrdinalIgnoreCase);
        }
    }
}
