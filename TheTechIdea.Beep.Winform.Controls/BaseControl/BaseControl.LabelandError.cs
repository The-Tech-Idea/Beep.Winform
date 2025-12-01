using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;

namespace TheTechIdea.Beep.Winform.Controls.Base
{
    /// <summary>
    /// Specifies the horizontal location of the label relative to the control
    /// </summary>
    public enum LabelLocation
    {
        Left,
        Center,
        Right
    }

    /// <summary>
    /// Specifies the location of the message image relative to the control
    /// </summary>
    public enum ImageLocation
    {
        Top,
        Bottom,
        Left,
        Right
    }

    //  Layout Management and Calculation  extension for BaseControl (partial)
    public partial class BaseControl
    {
        /// <summary>
        /// Draws label and helper/error text on the parent control.
        /// Label is drawn above the control, error/helper text is drawn below.
        /// </summary>
        /// <param name="parentGraphics">Graphics object of the parent control</param>
        /// <param name="childBounds">Bounds of the child control in parent coordinates</param>
        /// <param name="owner">The BaseControl instance that owns the label/error</param>
        /// <param name="labelLocation">Location of the label relative to the control</param>
        /// <param name="imageLocation">Location of the message image relative to the control</param>
        /// <param name="messageImagePath">Path to the image to display (optional)</param>
        /// <param name="showImage">Whether to show the image</param>
        /// <param name="showHelperText">Whether to show helper text</param>
        /// <param name="showErrorText">Whether to show error text</param>
        private void DrawLabelAndHelperToParent(Graphics parentGraphics, Rectangle childBounds, Base.BaseControl owner, LabelLocation labelLocation, ImageLocation imageLocation, string messageImagePath = "", bool showImage = false, bool showHelperText = true, bool showErrorText = true)
        {
            if (parentGraphics == null || owner == null) return;

            const int labelSpacing = 4; // Space between label and control top
            const int errorSpacing = 4; // Space between control bottom and error text
            const int imageSize = 16; // Default image size
            const int imageSpacing = 4; // Space between image and text

            // Draw label above the control
            if (owner.LabelTextOn && !string.IsNullOrEmpty(owner.LabelText))
            {
                float labelSize = Math.Max(8f, owner.Font.Size - 1f);
                using var labelFont = new Font(owner.Font.FontFamily, labelSize, FontStyle.Regular);
                var labelHeight = TextRenderer.MeasureText(parentGraphics, "Ag", labelFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                
                // Calculate label Y position (above the control)
                int labelY = childBounds.Top - labelHeight - labelSpacing;
                
                // Calculate label X position based on labelLocation
                int labelX = childBounds.Left;
                int labelWidth = childBounds.Width;
                TextFormatFlags labelFlags = TextFormatFlags.Left | TextFormatFlags.EndEllipsis;
                
                switch (labelLocation)
                {
                    case LabelLocation.Left:
                        labelFlags = TextFormatFlags.Left | TextFormatFlags.EndEllipsis;
                        break;
                    case LabelLocation.Center:
                        labelFlags = TextFormatFlags.HorizontalCenter | TextFormatFlags.EndEllipsis;
                        break;
                    case LabelLocation.Right:
                        labelFlags = TextFormatFlags.Right | TextFormatFlags.EndEllipsis;
                        break;
                }

                // Adjust for image if shown
                int imageOffset = 0;
                if (showImage && !string.IsNullOrEmpty(messageImagePath) && imageLocation == ImageLocation.Left)
                {
                    imageOffset = imageSize + imageSpacing;
                }

                var labelRect = new Rectangle(
                    childBounds.Left + imageOffset, 
                    labelY, 
                    Math.Max(10, labelWidth - imageOffset), 
                    labelHeight
                );

                Color labelColor = string.IsNullOrEmpty(owner.ErrorText) ? owner.ForeColor : owner.ErrorColor;
                TextRenderer.DrawText(parentGraphics, owner.LabelText, labelFont, labelRect, labelColor, labelFlags);

                // Draw message image if provided and showImage is true
                if (showImage && !string.IsNullOrEmpty(messageImagePath))
                {
                    Rectangle imageRect = Rectangle.Empty;
                    switch (imageLocation)
                    {
                        case ImageLocation.Top:
                            imageRect = new Rectangle(
                                childBounds.Left,
                                labelY - imageSize / 2,
                                imageSize,
                                imageSize
                            );
                            break;
                        case ImageLocation.Left:
                            imageRect = new Rectangle(
                                childBounds.Left,
                                labelY + (labelHeight - imageSize) / 2,
                                imageSize,
                                imageSize
                            );
                            break;
                        case ImageLocation.Right:
                            imageRect = new Rectangle(
                                childBounds.Right - imageSize,
                                labelY + (labelHeight - imageSize) / 2,
                                imageSize,
                                imageSize
                            );
                            break;
                        case ImageLocation.Bottom:
                            // Image at bottom would be near error text area
                            break;
                    }

                    if (!imageRect.IsEmpty)
                    {
                        try
                        {
                            StyledImagePainter.Paint(parentGraphics, imageRect, messageImagePath);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error drawing message image: {ex.Message}");
                        }
                    }
                }
            }

            // Draw helper or error text below the control
            if (showHelperText || showErrorText)
            {
                string supporting = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorText : owner.HelperText;
                if (!string.IsNullOrEmpty(supporting))
                {
                    float supSize = Math.Max(8f, owner.Font.Size - 1f);
                    using var supportFont = new Font(owner.Font.FontFamily, supSize, FontStyle.Regular);
                    var supportHeight = TextRenderer.MeasureText(parentGraphics, "Ag", supportFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                    
                    // Calculate error/helper text Y position (below the control)
                    int supportY = childBounds.Bottom + errorSpacing;
                    
                    var supportRect = new Rectangle(
                        childBounds.Left + 6, 
                        supportY, 
                        Math.Max(10, childBounds.Width - 12), 
                        supportHeight
                    );
                    
                    Color supportColor = !string.IsNullOrEmpty(owner.ErrorText) ? owner.ErrorColor : owner.ForeColor;
                    TextRenderer.DrawText(parentGraphics, supporting, supportFont, supportRect, supportColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);

                    // Draw message image below if imageLocation is Bottom
                    if (showImage && !string.IsNullOrEmpty(messageImagePath) && imageLocation == ImageLocation.Bottom)
                    {
                        Rectangle imageRect = new Rectangle(
                            childBounds.Left,
                            supportY + (supportHeight - imageSize) / 2,
                            imageSize,
                            imageSize
                        );
                        try
                        {
                            StyledImagePainter.Paint(parentGraphics, imageRect, messageImagePath);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error drawing message image: {ex.Message}");
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Unified external drawing handler for label, helper/error text, and badge.
        /// This method matches the DrawExternalHandler signature and is called by the parent control.
        /// </summary>
        private void DrawAllExternalElements(Graphics parentGraphics, Rectangle childBounds)
        {
            if (parentGraphics == null) return;
            
            // Draw label and helper/error text (BeforeContent layer)
            bool hasLabel = LabelTextOn && !string.IsNullOrEmpty(LabelText);
            bool hasHelper = !string.IsNullOrEmpty(HelperText);
            bool hasError = !string.IsNullOrEmpty(ErrorText);
            
            if (hasLabel || hasHelper || hasError)
            {
                // Get current property values (with defaults if properties don't exist)
                LabelLocation labelLoc = GetLabelLocation();
                ImageLocation imageLoc = GetImageLocation();
                string messageImagePath = GetMessageImagePath();
                bool showImage = GetShowImage();
                bool showHelperText = GetShowHelperText();
                bool showErrorText = GetShowErrorText();
                
                DrawLabelAndHelperToParent(parentGraphics, childBounds, this, labelLoc, imageLoc, messageImagePath, showImage, showHelperText, showErrorText);
            }
            
            // Draw badge (AfterAll layer - drawn separately via parent's layer system)
            // Badge is handled separately because it needs to be on AfterAll layer
        }
        
        /// <summary>
        /// External drawing handler for label and helper/error text (legacy, kept for compatibility).
        /// This method matches the DrawExternalHandler signature and is called by the parent control.
        /// </summary>
        private void DrawLabelAndHelperExternally(Graphics parentGraphics, Rectangle childBounds)
        {
            if (parentGraphics == null) return;
            
            // Get current property values (with defaults if properties don't exist)
            LabelLocation labelLoc = GetLabelLocation();
            ImageLocation imageLoc = GetImageLocation();
            string messageImagePath = GetMessageImagePath();
            bool showImage = GetShowImage();
            bool showHelperText = GetShowHelperText();
            bool showErrorText = GetShowErrorText();
            
            DrawLabelAndHelperToParent(parentGraphics, childBounds, this, labelLoc, imageLoc, messageImagePath, showImage, showHelperText, showErrorText);
        }

        /// <summary>
        /// Registers all external drawings (label/helper/error and badge) with parent if parent supports it (is BaseControl).
        /// Called automatically when parent changes. Does NOT clear existing drawings - parent tracks them.
        /// Uses the generic helper methods from BaseControl to create handlers.
        /// </summary>
        private void RegisterExternalLabelHelperDrawer()
        {
            if (Parent == null) return;

            // Only register if parent supports external drawing and we have something to draw
            if (Parent is IExternalDrawingProvider newExternalDrawingProvider)
            {
                // Register badge if needed (AfterAll layer) - use helper method to create handler
                if (!string.IsNullOrEmpty(BadgeText))
                {
                    var badgeHandler = BaseControl.CreateBadgeDrawingHandler(
                        BadgeText, BadgeBackColor, BadgeForeColor, BadgeFont, BadgeShape);
                    newExternalDrawingProvider.AddChildExternalDrawing(this, badgeHandler, DrawingLayer.AfterAll);
                }
                
                // Register label/helper if we have something to draw (BeforeContent layer) - use helper method to create handler
                bool hasLabel = LabelTextOn && !string.IsNullOrEmpty(LabelText);
                bool hasHelper = !string.IsNullOrEmpty(HelperText);
                bool hasError = !string.IsNullOrEmpty(ErrorText);
                
                if (hasLabel || hasHelper || hasError)
                {
                    LabelLocation labelLoc = GetLabelLocation();
                    ImageLocation imageLoc = GetImageLocation();
                    string messageImagePath = GetMessageImagePath();
                    bool showImage = GetShowImage();
                    bool showHelperText = GetShowHelperText();
                    bool showErrorText = GetShowErrorText();
                    
                    var labelHandler = BaseControl.CreateLabelAndHelperDrawingHandler(
                        hasLabel ? LabelText : "",
                        hasHelper ? HelperText : "",
                        hasError ? ErrorText : "",
                        labelLoc,
                        imageLoc,
                        messageImagePath,
                        showImage,
                        showHelperText,
                        showErrorText,
                        string.IsNullOrEmpty(ErrorText) ? ForeColor : ErrorColor,
                        ErrorColor,
                        ForeColor,
                        Font);
                    newExternalDrawingProvider.AddChildExternalDrawing(this, labelHandler, DrawingLayer.BeforeContent);
                }
                
                try { Parent?.Invalidate(); } catch { }
            }
        }

        /// <summary>
        /// Updates all external drawing registrations (label, helper/error, and badge).
        /// Call this when LabelText, ErrorText, HelperText, or BadgeText properties change.
        /// Clears ALL external drawings for this child, then re-registers everything needed.
        /// Uses the generic helper methods from BaseControl to create handlers.
        /// </summary>
        public void UpdateExternalDrawing()
        {
            if (Parent is IExternalDrawingProvider externalDrawingProvider)
            {
                // Clear ALL external drawings for this child (parent tracks all drawings per child)
                externalDrawingProvider.ClearChildExternalDrawing(this);
                
                // Re-register badge if needed (AfterAll layer) - use helper method to create handler
                if (!string.IsNullOrEmpty(BadgeText))
                {
                    var badgeHandler = BaseControl.CreateBadgeDrawingHandler(
                        BadgeText, BadgeBackColor, BadgeForeColor, BadgeFont, BadgeShape);
                    externalDrawingProvider.AddChildExternalDrawing(this, badgeHandler, DrawingLayer.AfterAll);
                }
                
                // Re-register label/helper if we have something to draw (BeforeContent layer) - use helper method to create handler
                bool hasLabel = LabelTextOn && !string.IsNullOrEmpty(LabelText);
                bool hasHelper = !string.IsNullOrEmpty(HelperText);
                bool hasError = !string.IsNullOrEmpty(ErrorText);
                
                if (hasLabel || hasHelper || hasError)
                {
                    LabelLocation labelLoc = GetLabelLocation();
                    ImageLocation imageLoc = GetImageLocation();
                    string messageImagePath = GetMessageImagePath();
                    bool showImage = GetShowImage();
                    bool showHelperText = GetShowHelperText();
                    bool showErrorText = GetShowErrorText();
                    
                    var labelHandler = BaseControl.CreateLabelAndHelperDrawingHandler(
                        hasLabel ? LabelText : "",
                        hasHelper ? HelperText : "",
                        hasError ? ErrorText : "",
                        labelLoc,
                        imageLoc,
                        messageImagePath,
                        showImage,
                        showHelperText,
                        showErrorText,
                        string.IsNullOrEmpty(ErrorText) ? ForeColor : ErrorColor,
                        ErrorColor,
                        ForeColor,
                        Font);
                    externalDrawingProvider.AddChildExternalDrawing(this, labelHandler, DrawingLayer.BeforeContent);
                }
                
                try { Parent?.Invalidate(); } catch { }
            }
        }
        
        /// <summary>
        /// Updates the external label/helper drawing registration (legacy method name, kept for compatibility).
        /// </summary>
        [Obsolete("Use UpdateExternalDrawing() instead")]
        public void UpdateExternalLabelHelperDrawing()
        {
            UpdateExternalDrawing();
        }

        /// <summary>
        /// Removes all external drawings (label/helper/error and badge) from parent.
        /// </summary>
        public void RemoveExternalDrawing()
        {
            if (Parent is IExternalDrawingProvider externalDrawingProvider)
            {
                externalDrawingProvider.ClearChildExternalDrawing(this);
                try { Parent?.Invalidate(); } catch { }
            }
        }
        
        /// <summary>
        /// Removes external label/helper drawing from parent (legacy method name, kept for compatibility).
        /// </summary>
        [Obsolete("Use RemoveExternalDrawing() instead")]
        public void RemoveExternalLabelHelperDrawing()
        {
            RemoveExternalDrawing();
        }

        // Helper methods to get property values (with defaults if properties don't exist)
        private LabelLocation GetLabelLocation()
        {
            try
            {
                var prop = GetType().GetProperty("LabelLocation");
                if (prop != null && prop.PropertyType == typeof(LabelLocation))
                {
                    return (LabelLocation)prop.GetValue(this);
                }
            }
            catch { }
            return LabelLocation.Left;
        }

        private ImageLocation GetImageLocation()
        {
            try
            {
                var prop = GetType().GetProperty("ImageLocation");
                if (prop != null && prop.PropertyType == typeof(ImageLocation))
                {
                    return (ImageLocation)prop.GetValue(this);
                }
            }
            catch { }
            return ImageLocation.Top;
        }

        private string GetMessageImagePath()
        {
            try
            {
                var prop = GetType().GetProperty("MessageImagePath");
                if (prop != null && prop.PropertyType == typeof(string))
                {
                    return (string)prop.GetValue(this) ?? "";
                }
            }
            catch { }
            return "";
        }

        private bool GetShowImage()
        {
            try
            {
                var prop = GetType().GetProperty("ShowImage");
                if (prop != null && prop.PropertyType == typeof(bool))
                {
                    return (bool)prop.GetValue(this);
                }
            }
            catch { }
            return false;
        }

        private bool GetShowHelperText()
        {
            try
            {
                var prop = GetType().GetProperty("ShowHelperText");
                if (prop != null && prop.PropertyType == typeof(bool))
                {
                    return (bool)prop.GetValue(this);
                }
            }
            catch { }
            return true;
        }

        private bool GetShowErrorText()
        {
            try
            {
                var prop = GetType().GetProperty("ShowErrorText");
                if (prop != null && prop.PropertyType == typeof(bool))
                {
                    return (bool)prop.GetValue(this);
                }
            }
            catch { }
            return true;
        }
    }
}
