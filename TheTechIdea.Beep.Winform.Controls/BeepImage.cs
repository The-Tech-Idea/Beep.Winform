using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using Svg;
using System.Drawing.Text;
using Timer = System.Windows.Forms.Timer;
using TheTechIdea.Beep.Vis.Modules;




namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(BeepImage))]
    [Category("Beep Controls")]
    [DisplayName("Beep Image")]
    [Description("A control that displays an image (SVG, PNG, JPG, BMP).")]
    //[Designer(typeof(TheTechIdea.Beep.Winform.Controls.Design.BeepImageDesigner))]
    public class BeepImage : BeepControl
    {
        public SvgDocument svgDocument { get; private set; }
        private Image regularImage;
        private bool isSvg = false;
        private string _advancedImagePath;
        // Property for the image path (SVG, PNG, JPG, BMP)
        protected string _imagepath;
        Color fillColor;
        Color strokeColor;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("fill color")]
        public Color FillColor
        {
            get => fillColor;
            set
            {
                fillColor = value;
                Invalidate();
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("stroke color")]
        public Color StrokeColor
        {
            get => strokeColor;
            set
            {
                strokeColor = value;
                Invalidate();
            }
        }
        public BeepImage()
        {
            //// Enable double buffering and optimized painting
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);
            UpdateStyles();
            if (Width <= 0 || Height <= 0) // Ensure size is only set if not already defined
            {
                Width = 100;
                Height = 100;
            }
            BoundProperty = "ImagePath";
            fillColor = Color.Black;
            strokeColor = Color.Black;
            // ImageSelector.SetSelector();
        }
        #region "Properties"

        private ImageEmbededin _imageEmbededin = ImageEmbededin.Button;
        [Category("Appearance")]
        [Description("Indicates where the image is embedded.")]
        public ImageEmbededin ImageEmbededin
        {
            get => _imageEmbededin;
            set
            {
                _imageEmbededin = value;
                Invalidate();
            }
        }

        private float _manualRotationAngle = 0; // Manual rotation angle
        private bool _allowManualRotation = true; // Allows toggling between manual and spinning


        public float ManualRotationAngle
        {
            get => _manualRotationAngle;
            set
            {
                if (IsSpinning)
                {
                    Console.WriteLine("Warning: Spinner is active. Manual rotation may combine with spinning.");
                    // Option 1: Combine rotation
                    _manualRotationAngle = value; // Allows combining angles
                                                  // Option 2: Block manual rotation
                                                  // return; // Ignore the new value
                }
                else
                {
                    _manualRotationAngle = value;
                }

                Invalidate(); // Trigger redraw
            }
        }


        [Browsable(true)]
        [Category("Behavior")]
        [Description("Allows manual rotation when spinning is disabled.")]
        public bool AllowManualRotation
        {
            get => _allowManualRotation;
            set
            {
                if (_allowManualRotation != value)
                {
                    _allowManualRotation = value;
                    if (!_allowManualRotation)
                        _manualRotationAngle = 0; // Reset manual rotation if disabled
                    Invalidate();
                }
            }
        }


        private Timer _spinTimer;
        private float _rotationAngle;
        private bool _isSpinning = false;

        [Category("Behavior")]
        [Description("Indicates whether the image should spin.")]
        public bool IsSpinning
        {
            get => _isSpinning;
            set
            {
                if (_isSpinning == value)
                    return; // Skip if the state isn't changing

                _isSpinning = value;

                if (_isSpinning)
                {
                    StartSpin(); // Start or ensure spinning continues
                }
                else
                {
                    StopSpin(); // Stop spinning if requested
                }
            }
        }
        

        [Category("Behavior")]
        [Description("Sets the speed of the spin in degrees per frame.")]
        public float SpinSpeed { get; set; } = 5f;

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load.")]
        public string ImagePath
        {
            get => _imagepath;
            set
            {


                _imagepath = value;
                // Console.WriteLine("Loading ImagePath");
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
                    // Console.WriteLine("Loading ImagePath");
                    LoadImage(_imagepath);  // Use the final processed path for the image
                                            //  Console.WriteLine("Apply Theme to  ImagePath");
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
        private SvgPaintServer tmpfillcolor;
        private SvgPaintServer tmpstrokecolor;

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
          //  base.ApplyTheme();
            if (_currentTheme != null)
            {
                HoverBackColor = _currentTheme.ButtonHoverBackColor; // Hover background color
                HoverForeColor = _currentTheme.ButtonHoverForeColor; // Hover foreground color
                PressedBackColor = _currentTheme.ButtonActiveBackColor; // Pressed background color
                PressedForeColor = _currentTheme.ButtonActiveForeColor; // Pressed foreground color
                FocusBackColor = _currentTheme.ButtonActiveBackColor; // Focus background color
                FocusForeColor = _currentTheme.ButtonActiveForeColor; // Focus foreground color

                // ForeColor = _currentTheme.ButtonForeColor; // Default foreground color

                BackColor = _currentTheme.ButtonBackColor;
                switch (_imageEmbededin)
                {
                    case ImageEmbededin.ListBox:
                    case ImageEmbededin.Form:
                    case ImageEmbededin.Button:
                    case ImageEmbededin.ListView:
                        
                        BackColor = _currentTheme.ButtonBackColor;
                        break;
                    case ImageEmbededin.Label:

                        BackColor = _currentTheme.LabelBackColor;
                        break;
                    case ImageEmbededin.TextBox:

                        BackColor = _currentTheme.TextBoxBackColor;
                        break;
                    case ImageEmbededin.ComboBox:

                        BackColor = _currentTheme.ComboBoxBackColor;
                        break;
                    case ImageEmbededin.DataGridView:

                        BackColor = _currentTheme.GridBackColor;
                        break;
                    default:
                      
                        BackColor = _currentTheme.BackColor;
                        break;


                }
                if (_applyThemeOnImage)
                {
                    ApplyThemeToSvg();
                }
                if (IsChild && Parent != null)
                {
                    parentbackcolor = Parent.BackColor;
                }
                Invalidate();
            }
        }

        public void ApplyThemeToSvg()
        {

            if (svgDocument != null && _currentTheme != null)
            {
                
                switch (_imageEmbededin)
                {
                    case ImageEmbededin.ListBox:
                    case ImageEmbededin.Form:
                    case ImageEmbededin.Button:
                    case ImageEmbededin.ListView:
                        strokeColor  = _currentTheme.ButtonForeColor;
                        fillColor = _currentTheme.ButtonBackColor;
                        break;
                    case ImageEmbededin.Label:
                        strokeColor = _currentTheme.LabelForeColor;
                        fillColor  = _currentTheme.LabelBackColor;
                        break;
                    case ImageEmbededin.TextBox:
                        strokeColor = _currentTheme.TextBoxForeColor;
                        fillColor  = _currentTheme.TextBoxBackColor;
                        break;
                    case ImageEmbededin.ComboBox:
                        strokeColor  = _currentTheme.ComboBoxForeColor;
                        fillColor = _currentTheme.ComboBoxBackColor;
                        break;
                    case ImageEmbededin.DataGridView:
                        strokeColor  = _currentTheme.GridForeColor;
                        fillColor = _currentTheme.GridBackColor;
                        break;
                   default:
                        strokeColor = _currentTheme.ButtonForeColor;
                        fillColor  = _currentTheme.ButtonBackColor;
                        break;


                }
                //Determine the appropriate colors based on the current state(hover, pressed, or default)
                if (IsPressed)
                {
                    strokeColor  = _currentTheme.ButtonActiveBackColor;
                    fillColor = _currentTheme.ButtonActiveForeColor;
                }
                else if (IsHovered)
                {
                    strokeColor  = _currentTheme.ButtonActiveBackColor;
                    fillColor = _currentTheme.ButtonHoverForeColor;
                }
              
                foreach (var element in svgDocument.Descendants())
                {
                    if (element is SvgVisualElement visualElement)
                    {
                        // store current colors for future reference from current svgdocument
                       tmpfillcolor = visualElement.Fill;
                        tmpstrokecolor = visualElement.Stroke;
                    
                    }
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
                       
                    }
                }

                // Invalidate the control to trigger a repaint after applying the theme
                Invalidate();
            }

        }
      
        #endregion "Theme Properties"
        #region "Image Drawing Methods"
      
        public void DrawImage(Graphics g, Rectangle imageRect)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Save the current transformation state
            var originalTransform = g.Transform;

            try
            {
                float effectiveRotation = _manualRotationAngle + (IsSpinning ? _rotationAngle : 0);
                PointF center = new PointF(imageRect.X + imageRect.Width / 2f, imageRect.Y + imageRect.Height / 2f);

                // Apply rotation transformations
                g.TranslateTransform(center.X, center.Y);
                g.RotateTransform(effectiveRotation);
                g.TranslateTransform(-center.X, -center.Y);

                if (isSvg && svgDocument != null)
                {
                    var imageSize = svgDocument.GetDimensions();
                    var scaledBounds = GetScaledBounds(new SizeF(imageSize.Width, imageSize.Height), imageRect);

                    if (scaledBounds.Width > 0 && scaledBounds.Height > 0)
                    {
                        g.TranslateTransform(scaledBounds.X, scaledBounds.Y);
                        g.ScaleTransform(scaledBounds.Width / imageSize.Width, scaledBounds.Height / imageSize.Height);
                        svgDocument.Draw(g);
                    }
                }
                else if (regularImage != null)
                {
                    var scaledBounds = GetScaledBounds(new SizeF(regularImage.Width, regularImage.Height), imageRect);

                    if (scaledBounds.Width > 0 && scaledBounds.Height > 0)
                    {
                        g.DrawImage(
                            regularImage,
                            new Rectangle((int)scaledBounds.X, (int)scaledBounds.Y, (int)scaledBounds.Width, (int)scaledBounds.Height),
                            0,
                            0,
                            regularImage.Width,
                            regularImage.Height,
                            GraphicsUnit.Pixel
                        );
                    }
                }
            }
            finally
            {
                // Restore the original transformation state
                g.Transform = originalTransform;
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawingRect.Inflate(-1, -1); // Adjust for border thickness
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            // Fill the background with BackColor
           
            // Use spin functionality if enabled
            DrawImage(
                e.Graphics,
                DrawingRect
            );
            DrawBadge(e.Graphics);
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
                    retval = LoadImageFromEmbeddedResource(path);
                }
                else
                {
                    if (File.Exists(path))
                    {
                        // Load from file system
                        retval = LoadImageFromFile(path);
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
                    retval = LoadSvg(path);
                    break;
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                    retval = LoadRegularImage(path);
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
                Stream stream = null;

                // Try to load the resource from all loaded assemblies
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    stream = assembly.GetManifestResourceStream(resourcePath);
                    if (stream != null)
                    {
                        break; // Stop searching once we find the resource
                    }
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
                MessageBox.Show($"Error loading image: {ex.Message}", "ImagePath Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        public float GetScaleFactor(SizeF imageSize, Size targetSize)
        {
            float scaleX = targetSize.Width / imageSize.Width;
            float scaleY = targetSize.Height / imageSize.Height;

            return _scaleMode switch
            {
                ImageScaleMode.Stretch => Math.Min(scaleX, scaleY), // Fit within bounds, stretching as needed
                ImageScaleMode.KeepAspectRatio => Math.Min(scaleX, scaleY), // Maintain aspect ratio
                ImageScaleMode.KeepAspectRatioByWidth => scaleX,
                ImageScaleMode.KeepAspectRatioByHeight => scaleY,
                _ => 1.0f // Default to no scaling
            };
        }



        public RectangleF GetScaledBounds(SizeF imageSize, Rectangle targetRect)
        {
            float scaleFactor = GetScaleFactor(imageSize, targetRect.Size);

            float newWidth = imageSize.Width * scaleFactor;
            float newHeight = imageSize.Height * scaleFactor;

            float xOffset = targetRect.X + (targetRect.Width - newWidth) / 2;  // Center horizontally
            float yOffset = targetRect.Y + (targetRect.Height - newHeight) / 2; // Center vertically

            return new RectangleF(xOffset, yOffset, newWidth, newHeight);
        }


        //private string ProcessImagePath()
        //{
        //    string selectedPath = string.Empty;

        //    try
        //    {
        //        // Attempt to open ImageSelectorImporterForm
        //        form.PreviewImage(_imagepath);
        //        if (form.ShowDialog() == System.Windows.Forms.BeepDialogResult.OK)
        //            {
        //                // If dialog result is OK, capture the selected image path
        //                selectedPath = form.SelectedImagePath;
        //                if (!string.IsNullOrEmpty(selectedPath))
        //                {
        //                    return selectedPath;  // Valid path obtained
        //                }
        //            }

        //    }
        //    catch (Exception ex)
        //    {
        //        // Config error dialog or handle the exception as needed
        //        MessageBox.Config($"Error processing image path: {ex.Message}", "Process ImagePath Path Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }

        //    // Return empty if no valid path was obtained
        //    return string.Empty;
        //}


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
        //protected override void OnParentChanged(EventArgs e)
        //{
        //    base.OnParentChanged(e);
        //    Invalidate(); // Force redraw when parent changes
        //}

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
        #region "Spin and Animations"
        private void StartSpin()
        {
            // Check if the spinner is already running
            if (_spinTimer != null && _spinTimer.Enabled)
                return; // Do nothing if the spinner is already active
                        // Optionally reset manual rotation when spinning starts
            _manualRotationAngle = 0; // Reset if needed
            // Initialize the timer if it doesn't exist
            if (_spinTimer == null)
            {
                _spinTimer = new Timer { Interval = 30 }; // Adjust interval for desired speed
                _spinTimer.Tick += (s, e) =>
                {
                    // Increment the rotation angle and keep it within 0-360 degrees
                    _rotationAngle = (_rotationAngle + SpinSpeed) % 360;
                    Invalidate(); // Redraw the control to reflect the new angle
                };
            }

            // Start the timer
            _spinTimer.Start();
        }


        private void StopSpin()
        {
            if (!IsSpinning) return; // If not spinning, do nothing

            _isSpinning = false;
            _spinTimer?.Stop();

            // Reset the rotation angle
            _rotationAngle = 0;
            _manualRotationAngle = 0; // Optionally reset manual rotation too

            Invalidate(new Rectangle(0, 0, Width, Height)); // Redraw the entire control

        }



        #endregion "Spin and Animations"
        #region "IBeep UI Component Implementation"
        public override void SetValue(object value)
        {
            if (value != null)
            {
                ImagePath = value.ToString();
            }
        }
        public override object GetValue()
        {
            return ImagePath;
        }
        public override void ClearValue()
        {
            ImagePath = "";
        }
        public override bool ValidateData(out string messege)
        {
            messege = "";
            return true;
        }
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            Draw(graphics, rectangle);
        }

        #endregion "IBeep UI Component Implementation"
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeImages();
                _spinTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

    }
   public enum ImageEmbededin
    {
        Button,
        Form,
        Label,
        TextBox,
        ComboBox,
        ListBox,
        DataGridView,
        TreeView,
        ListView,
        Panel,
        GroupBox,
        TabControl,
        TabPage,
        AppBar,
        SideBar,
        Menu,
    }
}
