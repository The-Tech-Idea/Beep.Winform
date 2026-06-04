using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Badges;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Base
{
    public enum LabelLocation { Left, Center, Right }
    public enum ImageLocation { Top, Bottom, Left, Right }

    public partial class BaseControl
    {
        private PropertyInfo? _cachedLabelLocationProp;
        private PropertyInfo? _cachedImageLocationProp;
        private PropertyInfo? _cachedMessageImagePathProp;
        private PropertyInfo? _cachedShowImageProp;
        private PropertyInfo? _cachedShowHelperTextProp;
        private PropertyInfo? _cachedShowErrorTextProp;

        internal void DrawLabelAndHelperToParent(Graphics parentGraphics, Rectangle childBounds, Base.BaseControl owner, LabelLocation labelLocation, ImageLocation imageLocation, string messageImagePath = "", bool showImage = false, bool showHelperText = true, bool showErrorText = true)
        {
            if (parentGraphics == null || owner == null) return;

            const int labelSpacing = 4;
            const int errorSpacing = 4;
            const int imageSize = 16;
            const int imageSpacing = 4;

            if (owner.FloatingLabelOn && !string.IsNullOrEmpty(owner._floatingLabel))
            {
                float labelSize = Math.Max(7f, owner.Font.Size - 2f);
                using var labelFont = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                var labelHeight = TextRenderer.MeasureText(parentGraphics, "Ag", labelFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;

                int labelY = childBounds.Top - labelHeight - labelSpacing;
                TextFormatFlags labelFlags = labelLocation switch
                {
                    LabelLocation.Center => TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis,
                    LabelLocation.Right => TextFormatFlags.Right | TextFormatFlags.EndEllipsis,
                    _ => TextFormatFlags.Left | TextFormatFlags.EndEllipsis
                };

                int imageOffset = (showImage && !string.IsNullOrEmpty(messageImagePath) && imageLocation == ImageLocation.Left)
                    ? imageSize + imageSpacing : 0;

                var labelRect = new Rectangle(childBounds.Left + imageOffset, labelY, Math.Max(10, childBounds.Width - imageOffset), labelHeight);
                Color labelColor = string.IsNullOrEmpty(owner.ErrorText) ? owner.ForeColor : owner.ErrorColor;
                TextRenderer.DrawText(parentGraphics, owner.LabelText, labelFont, labelRect, labelColor, labelFlags);

                if (showImage && !string.IsNullOrEmpty(messageImagePath))
                {
                    Rectangle imageRect = imageLocation switch
                    {
                        ImageLocation.Top => new Rectangle(childBounds.Left, labelY - imageSize / 2, imageSize, imageSize),
                        ImageLocation.Left => new Rectangle(childBounds.Left, labelY + (labelHeight - imageSize) / 2, imageSize, imageSize),
                        ImageLocation.Right => new Rectangle(childBounds.Right - imageSize, labelY + (labelHeight - imageSize) / 2, imageSize, imageSize),
                        _ => Rectangle.Empty
                    };
                    if (!imageRect.IsEmpty)
                    {
                        try { StyledImagePainter.Paint(parentGraphics, imageRect, messageImagePath); }
                        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Error drawing message image: {ex.Message}"); }
                    }
                }
            }

            if (showHelperText || showErrorText)
            {
                string supporting = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
                if (!string.IsNullOrEmpty(supporting))
                {
                    float supSize = Math.Max(7f, owner.Font.Size - 2f);
                    using var supportFont = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                    var supportHeight = TextRenderer.MeasureText(parentGraphics, "Ag", supportFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    int supportY = childBounds.Bottom + errorSpacing;
                    var supportRect = new Rectangle(childBounds.Left + 6, supportY, Math.Max(10, childBounds.Width - 12), supportHeight);
                    Color supportColor = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorColor : owner.ForeColor;
                    TextRenderer.DrawText(parentGraphics, supporting, supportFont, supportRect, supportColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

                    if (showImage && !string.IsNullOrEmpty(messageImagePath) && imageLocation == ImageLocation.Bottom)
                    {
                        Rectangle imageRect = new Rectangle(childBounds.Left, supportY + (supportHeight - imageSize) / 2, imageSize, imageSize);
                        try { StyledImagePainter.Paint(parentGraphics, imageRect, messageImagePath); }
                        catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Error drawing message image: {ex.Message}"); }
                    }
                }
            }
        }

        public void UpdateExternalDrawing()
        {
            if (Parent is not IExternalDrawingProvider provider) return;

            provider.ClearChildExternalDrawing(this);

            bool hasAny = (LabelTextOn && !string.IsNullOrEmpty(LabelText))
                       || !string.IsNullOrEmpty(HelperText)
                       || !string.IsNullOrEmpty(ErrorText)
                       || _validationIcon != ValidationState.None
                       || _showIndicatorLine
                       || !string.IsNullOrEmpty(_customIconPath);

            if (!hasAny) return;

            provider.AddChildExternalDrawing(this, CreateCombinedExternalHandler(this), DrawingLayer.AfterAll);
            try { Parent?.Invalidate(); } catch { }
        }

        public void RemoveExternalDrawing()
        {
            if (Parent is IExternalDrawingProvider externalDrawingProvider)
            {
                externalDrawingProvider.ClearChildExternalDrawing(this);
                try { Parent?.Invalidate(); } catch { }
            }
        }

        private LabelLocation GetLabelLocation()
        {
            try
            {
                _cachedLabelLocationProp ??= GetType().GetProperty("LabelLocation");
                return _cachedLabelLocationProp is not null && _cachedLabelLocationProp.PropertyType == typeof(LabelLocation)
                    ? (LabelLocation)_cachedLabelLocationProp.GetValue(this) : LabelLocation.Left;
            }
            catch { return LabelLocation.Left; }
        }

        private ImageLocation GetImageLocation()
        {
            try
            {
                _cachedImageLocationProp ??= GetType().GetProperty("ImageLocation");
                return _cachedImageLocationProp is not null && _cachedImageLocationProp.PropertyType == typeof(ImageLocation)
                    ? (ImageLocation)_cachedImageLocationProp.GetValue(this) : ImageLocation.Top;
            }
            catch { return ImageLocation.Top; }
        }

        private string GetMessageImagePath()
        {
            try
            {
                _cachedMessageImagePathProp ??= GetType().GetProperty("MessageImagePath");
                return (string?)_cachedMessageImagePathProp?.GetValue(this) ?? "";
            }
            catch { return ""; }
        }

        private bool GetShowImage()
        {
            try
            {
                _cachedShowImageProp ??= GetType().GetProperty("ShowImage");
                return (bool?)_cachedShowImageProp?.GetValue(this) ?? false;
            }
            catch { return false; }
        }

        private bool GetShowHelperText()
        {
            try
            {
                _cachedShowHelperTextProp ??= GetType().GetProperty("ShowHelperText");
                return (bool?)_cachedShowHelperTextProp?.GetValue(this) ?? true;
            }
            catch { return true; }
        }

        private bool GetShowErrorText()
        {
            try
            {
                _cachedShowErrorTextProp ??= GetType().GetProperty("ShowErrorText");
                return (bool?)_cachedShowErrorTextProp?.GetValue(this) ?? true;
            }
            catch { return true; }
        }
    }
}
