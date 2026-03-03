using TheTechIdea.Beep.Winform.Controls.ThemeManagement;
using TheTechIdea.Beep.Winform.Controls;
using System.Drawing.Drawing2D;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using System.IO;

namespace TheTechIdea.Beep.Winform.Controls.Tooltips
{
    public sealed class RibbonSuperTooltip : IDisposable
    {
        private const int DefaultMaxWidth = 340;
        private const int RichMaxWidth = 440;
        private const int PreviewWidth = 112;
        private const int PreviewHeight = 64;
        private const int PreviewGap = 10;

        private readonly ToolTip _toolTip = new()
        {
            OwnerDraw = true,
            ShowAlways = true,
            UseFading = true,
            UseAnimation = true,
            InitialDelay = 250,
            ReshowDelay = 80,
            AutoPopDelay = 12000
        };

        private readonly Dictionary<Control, RibbonSuperTooltipModel> _modelByControl = [];
        private RibbonTheme _theme = new();

        public RibbonSuperTooltip()
        {
            _toolTip.Popup += ToolTip_Popup;
            _toolTip.Draw += ToolTip_Draw;
        }

        public void ApplyTheme(RibbonTheme theme)
        {
            _theme = theme ?? new RibbonTheme();
        }

        public void Show(Control owner, Point location, RibbonSuperTooltipModel model, int durationMs = 9000)
        {
            if (owner == null || model == null || model.IsEmpty)
            {
                return;
            }

            _modelByControl[owner] = model;
            _toolTip.Show(model.ToPlainText(), owner, location, Math.Max(1200, durationMs));
        }

        public void Hide(Control owner)
        {
            if (owner == null)
            {
                return;
            }

            _toolTip.Hide(owner);
        }

        private void ToolTip_Popup(object? sender, PopupEventArgs e)
        {
            if (e.AssociatedControl == null || !_modelByControl.TryGetValue(e.AssociatedControl, out var model))
            {
                return;
            }

            using var g = e.AssociatedControl.CreateGraphics();
            using var baseTitle = BeepThemesManager.ToFont(_theme.CommandTypography);
            using var titleFont = new Font(baseTitle, FontStyle.Bold);
            using var bodyFont = BeepThemesManager.ToFont(_theme.CommandTypography);
            using var footFont = BeepThemesManager.ToFont(_theme.GroupTypography);

            bool hasPreview = !string.IsNullOrWhiteSpace(model.ThumbnailPath);
            int maxWidth = hasPreview ? RichMaxWidth : DefaultMaxWidth;
            int width = 30;
            int height = 14;
            bool hasImage = !string.IsNullOrWhiteSpace(model.ImagePath);
            int textLeftOffset = hasImage ? 24 : 0;
            int previewReserve = hasPreview ? PreviewWidth + PreviewGap : 0;
            int textMaxWidth = Math.Max(120, maxWidth - textLeftOffset - previewReserve - 20);

            if (!string.IsNullOrWhiteSpace(model.Title))
            {
                var size = TextRenderer.MeasureText(g, model.Title, titleFont, new Size(textMaxWidth, int.MaxValue), TextFormatFlags.WordBreak);
                width = Math.Max(width, size.Width);
                height += size.Height + 4;
            }

            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                var size = TextRenderer.MeasureText(g, model.Description, bodyFont, new Size(textMaxWidth, int.MaxValue), TextFormatFlags.WordBreak);
                width = Math.Max(width, size.Width);
                height += size.Height + 6;
            }

            if (!string.IsNullOrWhiteSpace(model.Shortcut))
            {
                var shortcut = $"Shortcut: {model.Shortcut}";
                var size = TextRenderer.MeasureText(g, shortcut, footFont, new Size(textMaxWidth, int.MaxValue), TextFormatFlags.WordBreak);
                width = Math.Max(width, size.Width);
                height += size.Height + 4;
            }

            if (!string.IsNullOrWhiteSpace(model.Footer))
            {
                var size = TextRenderer.MeasureText(g, model.Footer, footFont, new Size(textMaxWidth, int.MaxValue), TextFormatFlags.WordBreak);
                width = Math.Max(width, size.Width);
                height += size.Height + 4;
            }

            int previewHeight = hasPreview ? PreviewHeight : 0;
            if (hasPreview && !string.IsNullOrWhiteSpace(model.ThumbnailCaption))
            {
                var captionSize = TextRenderer.MeasureText(g, model.ThumbnailCaption, footFont, new Size(PreviewWidth, int.MaxValue), TextFormatFlags.WordBreak);
                previewHeight += captionSize.Height + 4;
            }

            height = Math.Max(height, 16 + previewHeight);
            int finalWidth = Math.Min(maxWidth, width + 20 + textLeftOffset + previewReserve);
            e.ToolTipSize = new Size(Math.Max(180, finalWidth), Math.Max(52, height + 8));
        }

        private void ToolTip_Draw(object? sender, DrawToolTipEventArgs e)
        {
            if (e.AssociatedControl == null || !_modelByControl.TryGetValue(e.AssociatedControl, out var model))
            {
                e.DrawBackground();
                e.DrawBorder();
                e.DrawText();
                return;
            }

            using var baseTitle = BeepThemesManager.ToFont(_theme.CommandTypography);
            using var titleFont = new Font(baseTitle, FontStyle.Bold);
            using var bodyFont = BeepThemesManager.ToFont(_theme.CommandTypography);
            using var footFont = BeepThemesManager.ToFont(_theme.GroupTypography);

            using var backBrush = new SolidBrush(_theme.TabActiveBack);
            using var borderPen = new Pen(_theme.GroupBorder);
            using var titleBrush = new SolidBrush(_theme.Text);
            using var bodyBrush = new SolidBrush(_theme.Text);
            using var footBrush = new SolidBrush(ControlPaint.Dark(_theme.Text, .25f));

            e.Graphics.FillRectangle(backBrush, e.Bounds);
            e.Graphics.DrawRectangle(borderPen, e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1);

            int x = e.Bounds.X + 8;
            int y = e.Bounds.Y + 6;
            int width = e.Bounds.Width - 16;
            bool hasPreview = !string.IsNullOrWhiteSpace(model.ThumbnailPath);
            bool hasImage = !string.IsNullOrWhiteSpace(model.ImagePath);
            int previewReserve = hasPreview ? PreviewWidth + PreviewGap : 0;
            int textWidth = Math.Max(96, width - previewReserve);
            Rectangle previewRect = Rectangle.Empty;
            Rectangle previewCaptionRect = Rectangle.Empty;
            if (hasPreview)
            {
                int previewX = e.Bounds.Right - 8 - PreviewWidth;
                previewRect = new Rectangle(previewX, e.Bounds.Y + 8, PreviewWidth, PreviewHeight);
                previewCaptionRect = new Rectangle(previewX, previewRect.Bottom + 2, PreviewWidth, 36);
            }

            int textX = x;
            if (hasImage)
            {
                using var icon = CreateTooltipImage(model.ImagePath!, 16);
                if (icon != null)
                {
                    e.Graphics.DrawImage(icon, new Rectangle(x, y + 1, 16, 16));
                    textX += 22;
                    textWidth -= 22;
                }
            }

            if (!string.IsNullOrWhiteSpace(model.Title))
            {
                var titleRect = new Rectangle(textX, y, textWidth, 48);
                TextRenderer.DrawText(e.Graphics, model.Title, titleFont, titleRect, titleBrush.Color, TextFormatFlags.WordBreak | TextFormatFlags.Left);
                y += TextRenderer.MeasureText(e.Graphics, model.Title, titleFont, new Size(textWidth, int.MaxValue), TextFormatFlags.WordBreak).Height + 3;
            }

            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                var bodyRect = new Rectangle(textX, y, textWidth, 120);
                TextRenderer.DrawText(e.Graphics, model.Description, bodyFont, bodyRect, bodyBrush.Color, TextFormatFlags.WordBreak | TextFormatFlags.Left);
                y += TextRenderer.MeasureText(e.Graphics, model.Description, bodyFont, new Size(textWidth, int.MaxValue), TextFormatFlags.WordBreak).Height + 5;
            }

            if (!string.IsNullOrWhiteSpace(model.Shortcut))
            {
                string shortcut = $"Shortcut: {model.Shortcut}";
                var shortcutRect = new Rectangle(textX, y, textWidth, 28);
                TextRenderer.DrawText(e.Graphics, shortcut, footFont, shortcutRect, footBrush.Color, TextFormatFlags.WordBreak | TextFormatFlags.Left);
                y += TextRenderer.MeasureText(e.Graphics, shortcut, footFont, new Size(textWidth, int.MaxValue), TextFormatFlags.WordBreak).Height + 3;
            }

            if (!string.IsNullOrWhiteSpace(model.Footer))
            {
                var footRect = new Rectangle(textX, y, textWidth, 40);
                TextRenderer.DrawText(e.Graphics, model.Footer, footFont, footRect, footBrush.Color, TextFormatFlags.WordBreak | TextFormatFlags.Left);
            }

            if (hasPreview)
            {
                DrawPreviewSurface(e.Graphics, previewRect, previewCaptionRect, model, footBrush.Color, footFont);
            }
        }

        private Image? CreateTooltipImage(string imagePath, int size)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return null;
            }

            var bmp = new Bitmap(size, size);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.Clear(Color.Transparent);
            using var clipPath = new GraphicsPath();
            clipPath.AddRectangle(new Rectangle(0, 0, size, size));
            StyledImagePainter.PaintWithTint(g, clipPath, imagePath, _theme.IconColor, 1f);
            return bmp;
        }

        private void DrawPreviewSurface(Graphics g, Rectangle previewRect, Rectangle captionRect, RibbonSuperTooltipModel model, Color textColor, Font captionFont)
        {
            using var borderPen = new Pen(ControlPaint.Light(_theme.GroupBorder, .1f));
            using var backBrush = new SolidBrush(ControlPaint.Light(_theme.GroupBack, .04f));
            g.FillRectangle(backBrush, previewRect);
            g.DrawRectangle(borderPen, previewRect);

            using var preview = CreatePreviewImage(model.ThumbnailPath!, previewRect.Size);
            if (preview != null)
            {
                g.DrawImage(preview, previewRect);
            }
            else
            {
                using var fallback = CreateTooltipImage(model.ImagePath ?? model.ThumbnailPath!, 20);
                if (fallback != null)
                {
                    var iconRect = new Rectangle(
                        previewRect.X + (previewRect.Width - 20) / 2,
                        previewRect.Y + (previewRect.Height - 20) / 2,
                        20,
                        20);
                    g.DrawImage(fallback, iconRect);
                }
            }

            if (!string.IsNullOrWhiteSpace(model.ThumbnailCaption))
            {
                TextRenderer.DrawText(g, model.ThumbnailCaption, captionFont, captionRect, textColor, TextFormatFlags.WordBreak | TextFormatFlags.Left);
            }
        }

        private Image? CreatePreviewImage(string imagePath, Size targetSize)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return null;
            }

            if (TryLoadImageFromFile(imagePath, out var image))
            {
                return image;
            }

            int size = Math.Max(16, Math.Min(targetSize.Width, targetSize.Height));
            return CreateTooltipImage(imagePath, size);
        }

        private static bool TryLoadImageFromFile(string imagePath, out Image? image)
        {
            image = null;
            try
            {
                if (!File.Exists(imagePath))
                {
                    return false;
                }

                using var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var source = Image.FromStream(fs, useEmbeddedColorManagement: true, validateImageData: false);
                image = new Bitmap(source);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _toolTip.Popup -= ToolTip_Popup;
            _toolTip.Draw -= ToolTip_Draw;
            _toolTip.Dispose();
            _modelByControl.Clear();
        }
    }
}
