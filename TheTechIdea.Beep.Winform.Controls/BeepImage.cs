﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Reflection;
using Svg;

using System.Windows.Forms.Design;
using TheTechIdea.Beep.Winform.Controls.UIEditor;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepImage))]
    [Category("Beep Controls")]
    [Designer(typeof(BeepImageDesigner))]
    public class BeepImage : BeepControl
    {
       
  
       
        private ImageSelectorImporterForm form;
        private SvgDocument svgDocument;
        private Image regularImage;
        private bool isSvg = false;
        private string _advancedImagePath;
        // Property for the image path (SVG, PNG, JPG, BMP)
        protected string _imagepath;
        
        public BeepImage()
        {
           // ImageSelector.SetSelector();
        }


        #region "Properties"
       

        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load")]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string ImagePath
        {
            get => _imagepath;
            set
            {
               

                _imagepath = value;
               // Console.WriteLine("Loading Image");
                if (!string.IsNullOrEmpty(_imagepath))
                {
                    //Use ImageSelector to select and process the image path
                    //if (!isinit)
                    //{
                    //  //  string processedPath = ImageSelector.SelectImage(_imagepath);
                    //    if (!string.IsNullOrEmpty(processedPath))
                    //    {
                    //        _imagepath = processedPath;
                    //    }

                    //}
                 //   isinit = false;
                   // Console.WriteLine("Loading Image");
                    LoadImage(_imagepath);  // Use the final processed path for the image
                  //  Console.WriteLine("Apply Theme to  Image");
                    ApplyTheme();
                    Invalidate();
                }
                else
                {
                    ClearImage();
                    Invalidate();
                }
            }
        }

        private ImageScaleMode _scaleMode = ImageScaleMode.KeepAspectRatio;
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ImageScaleMode ScaleMode
        {
            get => _scaleMode;
            set
            {
                _scaleMode = value;
                Invalidate(); // Repaint when the scale mode changes
            }
        }
        bool _applyThemeOnImage = false;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Appearance")]
        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
                _applyThemeOnImage = value;
                ApplyTheme();
                Invalidate();
            }
        }

        private bool _isstillimage = false;
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Appearance")]
        public bool IsStillImage { get { return _isstillimage; } set { _isstillimage = value; } }

        public bool HasImage
        {
            get
            {
                return (isSvg && svgDocument != null) || regularImage != null;
            }
        }
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category("Appearance")]
        public Size GetImageSize()
        {
            if (isSvg && svgDocument != null)
            {
                var dimensions = svgDocument.GetDimensions();
                return new Size((int)dimensions.Width, (int)dimensions.Height);
            }
            else if (regularImage != null)
            {
                return regularImage.Size;
            }
            return Size.Empty;
        }

      
        /// <summary>
        /// Gets or sets the image displayed in the control.
        /// Supports both regular images and SVG files.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The image displayed on the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Image Image
        {
            get => regularImage;
            set
            {
                if (value == null)
                {
                    ClearImage();  // Clear both SVG and regular images
                    return;
                }

                // If the image has a Tag that indicates it's an SVG path, handle it as an SVG
                if (value.Tag is string path && IsSvgPath(path))
                {
                    LoadSvg(path);
                }
                else
                {
                    regularImage = value;
                    svgDocument = null;  // Clear any loaded SVG
                    Invalidate();  // Trigger repaint
                }
            }
        }
        #endregion "Propoerties"
        #region "Theme Properties"
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme != null)
            {
                HoverBackColor = _currentTheme.ButtonHoverBackColor; // Hover background color
                HoverForeColor = _currentTheme.ButtonHoverForeColor; // Hover foreground color
                PressedBackColor = _currentTheme.ButtonActiveBackColor; // Pressed background color
                PressedForeColor = _currentTheme.ButtonActiveForeColor; // Pressed foreground color
                FocusBackColor = _currentTheme.ButtonActiveBackColor; // Focus background color
                FocusForeColor = _currentTheme.ButtonActiveForeColor; // Focus foreground color
                
               // ForeColor = _currentTheme.ButtonForeColor; // Default foreground color
               
                
                if(_applyThemeOnImage)
                {
                    ApplyThemeToSvg();
                }
                Invalidate();
            }
        }

        public  void ApplyThemeToSvg()
        {

            if (svgDocument != null && _currentTheme != null)
            {
                Color fillColor;
                Color strokeColor;

                // Determine the appropriate colors based on the current state (hover, pressed, or default)
                if (IsPressed)
                {
                    fillColor = _currentTheme.ButtonActiveBackColor;
                    strokeColor = _currentTheme.ButtonActiveForeColor;
                }
                else if (IsHovered)
                {
                    fillColor = _currentTheme.ButtonHoverBackColor;
                    strokeColor = _currentTheme.ButtonHoverForeColor;
                }
                else
                {
                    fillColor = _currentTheme.BackgroundColor;
                    strokeColor = ForeColor    ;
                }
                // Apply colors recursively to all elements
               // ApplyColorsToElement(svgDocument, fillColor, strokeColor);
                // Apply the selected colors to the SVG elements
                foreach (var element in svgDocument.Descendants())
                {
                    if (element is SvgVisualElement visualElement)
                    {
                        visualElement.Stroke = new SvgColourServer(strokeColor);
                        visualElement.Fill = new SvgColourServer(fillColor);
                        //visualElement.CustomAttributes["stroke-width"] = "1"; // Explicitly set stroke width
                        //visualElement.CustomAttributes["stroke-linecap"] = "round"; // Customize stroke if needed
                        //visualElement.CustomAttributes["stroke-linejoin"] = "round";
                    }
                }

                // Invalidate the control to trigger a repaint after applying the theme
                Invalidate();
            }      

        }
        private void ApplyColorsToElement(SvgElement element, Color fillColor, Color strokeColor)
        {
            if (element is SvgVisualElement visualElement)
            {
                visualElement.Fill = new SvgColourServer(fillColor);
                visualElement.Stroke = new SvgColourServer(strokeColor);
            }
            foreach (var child in element.Children)
            {
                ApplyColorsToElement(child, fillColor, strokeColor);
            }
        }
        #endregion "Theme Properties"
        #region "Image Drawing Methods"
        public void DrawImage(Graphics g, Rectangle imageRect)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (isSvg && svgDocument != null)
            {
                var imageSize = svgDocument.GetDimensions();
                var scaleFactor = GetScaleFactor(new SizeF(imageSize.Width, imageSize.Height), imageRect.Size);

                // Ensure the content fits within DrawingRect
                scaleFactor = Math.Max(scaleFactor, GetScaleFactor(imageSize, new Size(imageRect.Width, imageRect.Height)));

                // Apply scaling and positioning
                if (scaleFactor > 0)
                {
                    g.TranslateTransform(
                        imageRect.X + (imageRect.Width - imageSize.Width * scaleFactor) / 2,
                        imageRect.Y + (imageRect.Height - imageSize.Height * scaleFactor) / 2);
                    g.ScaleTransform(scaleFactor, scaleFactor);

                    svgDocument.Draw(g);
                    g.ResetTransform(); // Reset transformations
                }
            }
            else if (regularImage != null)
            {
                // Ensure the image fits within DrawingRect
                if (imageRect.Width > 0 && imageRect.Height > 0)
                {
                    var scaledBounds = GetScaledBounds(new SizeF(regularImage.Width, regularImage.Height), imageRect);

                    // Scale up if the image is smaller than DrawingRect
                    var scaleFactor = GetScaleFactor(new SizeF(regularImage.Width, regularImage.Height), new Size(imageRect.Width, imageRect.Height));
                    if (scaleFactor > 1)
                    {
                        var scaledSize = new SizeF(regularImage.Width * scaleFactor, regularImage.Height * scaleFactor);
                        var xOffset = imageRect.X + (imageRect.Width - scaledSize.Width) / 2;
                        var yOffset = imageRect.Y + (imageRect.Height - scaledSize.Height) / 2;
                        scaledBounds = new RectangleF(xOffset, yOffset, scaledSize.Width, scaledSize.Height);
                    }

                    g.DrawImage(regularImage, scaledBounds);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            if (isSvg && svgDocument != null)
            {
                var imageSize = svgDocument.GetDimensions();
                var scaleFactor = GetScaleFactor(new SizeF(imageSize.Width, imageSize.Height), DrawingRect.Size);

                // Ensure scaling up to fit DrawingRect
                scaleFactor = Math.Max(scaleFactor, GetScaleFactor(imageSize, new Size(DrawingRect.Width, DrawingRect.Height)));

                // Apply transformations for centering and scaling
                e.Graphics.TranslateTransform(
                    DrawingRect.X + (DrawingRect.Width - imageSize.Width * scaleFactor) / 2,
                    DrawingRect.Y + (DrawingRect.Height - imageSize.Height * scaleFactor) / 2);
                e.Graphics.ScaleTransform(scaleFactor, scaleFactor);

                svgDocument.Draw(e.Graphics);
                e.Graphics.ResetTransform();
            }
            else if (regularImage != null)
            {
                var scaledBounds = GetScaledBounds(new SizeF(regularImage.Width, regularImage.Height), DrawingRect);

                // Scale up if the image is smaller than DrawingRect
                var scaleFactor = GetScaleFactor(new SizeF(regularImage.Width, regularImage.Height), new Size(DrawingRect.Width, DrawingRect.Height));
                if (scaleFactor > 1)
                {
                    var scaledSize = new SizeF(regularImage.Width * scaleFactor, regularImage.Height * scaleFactor);
                    var xOffset = DrawingRect.X + (DrawingRect.Width - scaledSize.Width) / 2;
                    var yOffset = DrawingRect.Y + (DrawingRect.Height - scaledSize.Height) / 2;
                    scaledBounds = new RectangleF(xOffset, yOffset, scaledSize.Width, scaledSize.Height);
                }

                e.Graphics.DrawImage(regularImage, scaledBounds);
            }
        }

        public bool IsSvgPath(string path)
        {
            return Path.GetExtension(path)?.ToLower() == ".svg";
        }
        /// <summary>
        /// Load the image from the provided path (checks if it's a file path or embedded resource).
        /// </summary>
        private bool LoadImage(string path)
        {
            bool retval = false;
            try
            {
                if (IsEmbeddedResource(path))
                {
                    // Attempt to load from embedded resources
                    retval= LoadImageFromEmbeddedResource(path);
                }
                else
                {
                    if (File.Exists(path))
                    {
                        // Load from file system
                        retval= LoadImageFromFile(path);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image: {ex.Message}");
                return false;
            }
           return retval;
        }
        /// <summary>
        /// Load an image from the file system (SVG, PNG, JPG, BMP)
        /// </summary>
        private bool LoadImageFromFile(string path)
        {
            bool retval = false;
            string extension = Path.GetExtension(path).ToLower();
            switch (extension)
            {
                case ".svg":
                    retval= LoadSvg(path);
                    break;
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                    retval= LoadRegularImage(path);
                    break;
                default:
                    Console.WriteLine("Unsupported image format. Supported formats are: SVG, PNG, JPG, BMP.");
                    break;
            }
            
            return retval;
        }
        public bool IsEmbeddedResource(string path)
        {
            // check if path has more than one dot
            if (path.Split('.').Length > 2)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// Load an image from the embedded resources (checks the current assembly).
        /// </summary>
        public bool LoadImageFromEmbeddedResource(string resourcePath)
        {
            try
            {
                // Attempt to load the resource from the current assembly first
                var assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(resourcePath);

                // If not found, try to load from the calling assembly (useful when referenced by other projects)
                if (stream == null)
                {
                    assembly = Assembly.GetCallingAssembly();
                    stream = assembly.GetManifestResourceStream(resourcePath);
                }

                if (stream != null)
                {
                    // Determine the image type by file extension
                    string extension = Path.GetExtension(resourcePath).ToLower();
                    if (extension == ".svg")
                    {
                        svgDocument = SvgDocument.Open<SvgDocument>(stream);
                        isSvg = true;
                    }
                    else
                    {
                        regularImage = Image.FromStream(stream);
                        isSvg = false;
                    }

                    ApplyTheme(); // Apply theme after loading
                    Invalidate(); // Trigger a repaint
                    return true;
                }
                else
                {
                    Console.WriteLine($"Embedded resource not found: {resourcePath}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading embedded resource: {ex.Message}");
                return false;
            }
        }

        public void ClearImage()
        {
            svgDocument = null;
            regularImage?.Dispose();
            regularImage = null;
            isSvg = false;
        }
        public bool LoadSvg(string svgPath)
        {
            try
            {
                DisposeImages();
                svgDocument = SvgDocument.Open(svgPath);
                isSvg = true;
                Invalidate(); // Trigger repaint
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading SVG: {ex.Message}", "SVG Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        public bool LoadRegularImage(string imagePath)
        {
            try
            {
                DisposeImages();
                regularImage = Image.FromFile(imagePath);
                isSvg = false;
                regularImage.Tag = imagePath; // Store path for future reference
                Invalidate(); // Trigger repaint
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        private void DisposeImages()
        {
            regularImage?.Dispose();
            regularImage = null;
            svgDocument = null;
        }
      
        #endregion "Image Drawing Methods"
        #region "Designer Support"

        private void ClearImages()
        {
            svgDocument = null;
            regularImage?.Dispose();
            regularImage = null;
            Invalidate();  // Repaint the control to reflect the cleared state
        }
        public float GetScaleFactor(SizeF imageSize, Size targetSize)
        {
            float scaleX = targetSize.Width / imageSize.Width;
            float scaleY = targetSize.Height / imageSize.Height;

            return _scaleMode switch
            {
                ImageScaleMode.Stretch => 1.0f,
                ImageScaleMode.KeepAspectRatioByWidth => scaleX,
                ImageScaleMode.KeepAspectRatioByHeight => scaleY,
                _ => Math.Min(scaleX, scaleY) // Default to KeepAspectRatio
            };
        }


        public RectangleF GetScaledBounds(SizeF imageSize, Rectangle targetRect)
        {
            float scale = GetScaleFactor(imageSize, targetRect.Size);

            if (scale <= 0)
                return RectangleF.Empty;

            float newWidth = imageSize.Width * scale;
            float newHeight = imageSize.Height * scale;

            float xOffset = targetRect.X + (targetRect.Width - newWidth) / 2;  // Center the image horizontally
            float yOffset = targetRect.Y + (targetRect.Height - newHeight) / 2; // Center the image vertically

            return new RectangleF(xOffset, yOffset, newWidth, newHeight);
        }

        private string ProcessImagePath()
        {
            string selectedPath = string.Empty;

            try
            {
                // Attempt to open ImageSelectorImporterForm
                form.PreviewImage(_imagepath);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        // If dialog result is OK, capture the selected image path
                        selectedPath = form.SelectedImagePath;
                        if (!string.IsNullOrEmpty(selectedPath))
                        {
                            return selectedPath;  // Valid path obtained
                        }
                    }
                
            }
            catch (Exception ex)
            {
                // Show error dialog or handle the exception as needed
                MessageBox.Show($"Error processing image path: {ex.Message}", "Process Image Path Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Return empty if no valid path was obtained
            return string.Empty;
        }


        public float GetScaleFactor(SizeF imageSize)
        {
            float availableWidth = ClientSize.Width;
            float availableHeight = ClientSize.Height;

            switch (_scaleMode)
            {
                case ImageScaleMode.Stretch:
                    return 1.0f; // No scaling, just stretch
                case ImageScaleMode.KeepAspectRatioByWidth:
                    return availableWidth / imageSize.Width;
                case ImageScaleMode.KeepAspectRatioByHeight:
                    return availableHeight / imageSize.Height;
                case ImageScaleMode.KeepAspectRatio:
                default:
                    float scaleX = availableWidth / imageSize.Width;
                    float scaleY = availableHeight / imageSize.Height;
                    return Math.Min(scaleX, scaleY); // Maintain aspect ratio
            }
        }
        public RectangleF GetScaledBounds(SizeF imageSize)
        {
            float controlWidth = DrawingRect.Width;
            float controlHeight = DrawingRect.Height;

            float scaleX = controlWidth / imageSize.Width;
            float scaleY = controlHeight / imageSize.Height;
            float scale = Math.Min(scaleX, scaleY);

            float newWidth = imageSize.Width * scale;
            float newHeight = imageSize.Height * scale;

            float xOffset = (controlWidth - newWidth) / 2;
            float yOffset = (controlHeight - newHeight) / 2;

            return new RectangleF(xOffset, yOffset, newWidth, newHeight);
        }
        #endregion "Designer Support"
        #region Event Handlers for Hover and Pressed State
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            Invalidate(); // Force redraw when parent changes
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
            {
                Invalidate(); // Force redraw when visibility changes
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (IsStillImage)
            {
                return;
            }
             base.OnMouseLeave(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (IsStillImage)
            {
                return;
            }
            base.OnMouseLeave(e);
          
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                IsPressed = true;
               // ApplyTheme();  // Reapply theme to reflect pressed state
               // Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (IsStillImage)
            {
                return;
            }
            base.OnMouseUp(e);
            IsPressed = false;
          //  ApplyTheme();  // Reapply theme to reflect released state
         //   Invalidate();
        }

        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeImages();
            }
            base.Dispose(disposing);
        }

    }
    public enum ImageScaleMode
    {
        None,
        Stretch,
        KeepAspectRatio,
        KeepAspectRatioByWidth,
        KeepAspectRatioByHeight
    }
}
