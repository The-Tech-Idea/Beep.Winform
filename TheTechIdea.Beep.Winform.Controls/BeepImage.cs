using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using Svg;
using System.Drawing.Text;
using Timer = System.Windows.Forms.Timer;
using TheTechIdea.Beep.Vis.Modules;
using System.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.Helpers;




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

        private int _baseSize = 50; // Default size

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The default size of the image before scaling.")]
        public int BaseSize
        {
            get => _baseSize;
            set
            {
                _baseSize = value;
                if (Size.Width <= _baseSize) // If not scaled, apply base size
                {
                    Size = new Size(_baseSize, _baseSize);
                }
                Invalidate();
            }
        }
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
        private float _velocity = 0.0f;
        [Category("Behavior")]
        [Description("Sets the velocity of the spin in degrees per frame.")]
        public float Velocity
        {
            get => _velocity;
            set
            {
                _velocity = value;
                Invalidate(); // Repaint when the velocity changes
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
                    // Console.WriteLine("Warning: Spinner is active. Manual rotation may combine with spinning.");
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
                // Console.WriteLine("Loading ImagePath ...");
                if (!string.IsNullOrEmpty(_imagepath))
                {
                    // Console.WriteLine($"Loading ImagePath ....."+ _imagepath);
                    LoadImage(_imagepath);  // Use the final processed path for the image
                                            // Console.WriteLine("Finished  Image Path ......." + _imagepath);
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
        private float _scaleFactor = 1.0f;
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                _scaleFactor = value;
                Invalidate(); // Repaint when the scale factor changes
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
                PressedBackColor = _currentTheme.ButtonSelectedForeColor; // Pressed background color
                PressedForeColor = _currentTheme.ButtonSelectedForeColor; // Pressed foreground color
                FocusBackColor = _currentTheme.ButtonSelectedForeColor; // Focus background color
                FocusForeColor = _currentTheme.ButtonSelectedForeColor; // Focus foreground color

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
            if (svgDocument == null || _currentTheme == null)
            {
                // Log if SVG document or theme is null
                MiscFunctions.SendLog("ApplyThemeToSvg: svgDocument or _currentTheme is null");
                return;
            }

            // Determine fill & stroke colors based on the theme
            Color strokeColor, fillColor;
            switch (_imageEmbededin)
            {
                case ImageEmbededin.ListBox:
                case ImageEmbededin.Form:
                case ImageEmbededin.Button:
                case ImageEmbededin.ListView:
                    strokeColor = _currentTheme.ButtonForeColor;
                    fillColor = _currentTheme.ButtonBackColor;
                    break;
                case ImageEmbededin.Label:
                    strokeColor = _currentTheme.LabelForeColor;
                    fillColor = _currentTheme.LabelBackColor;
                    break;
                case ImageEmbededin.TextBox:
                    strokeColor = _currentTheme.TextBoxForeColor;
                    fillColor = _currentTheme.TextBoxBackColor;
                    break;
                case ImageEmbededin.ComboBox:
                    strokeColor = _currentTheme.ComboBoxForeColor;
                    fillColor = _currentTheme.ComboBoxBackColor;
                    break;
                case ImageEmbededin.DataGridView:
                    strokeColor = _currentTheme.GridForeColor;
                    fillColor = _currentTheme.GridBackColor;
                    break;
                default:
                    strokeColor = _currentTheme.ButtonForeColor;
                    fillColor = _currentTheme.ButtonBackColor;
                    break;
            }

            // Adjust for hover/pressed states
            if (IsPressed)
            {
                strokeColor = _currentTheme.ButtonSelectedForeColor;
                fillColor = _currentTheme.ButtonSelectedForeColor;
            }
            else if (IsHovered)
            {
                strokeColor = _currentTheme.ButtonSelectedForeColor;
                fillColor = _currentTheme.ButtonHoverForeColor;
            }

            // Log the colors being applied
            MiscFunctions.SendLog($"ApplyThemeToSvg: Applying fillColor={fillColor}, strokeColor={strokeColor} for ImageEmbededin={_imageEmbededin}");

            // Recursively apply colors to all SVG visual elements
            foreach (var element in svgDocument.Descendants())
            {
                // Apply to SvgVisualElement (e.g., paths, shapes)
                if (element is SvgVisualElement visualElement)
                {
                    // Log the element and its original colors
                    MiscFunctions.SendLog($"Before: {element.GetType().Name}, Fill={visualElement.Fill}, Stroke={visualElement.Stroke}");

                    // Store original colors
                    tmpfillcolor = visualElement.Fill;
                    tmpstrokecolor = visualElement.Stroke;

                    // Apply theme colors, preserving transparency if the original color was transparent
                    if (fillColor != Color.Empty)
                    {
                        visualElement.Fill = new SvgColourServer(fillColor);
                    }
                    else if (visualElement.Fill != null && visualElement.Fill != SvgPaintServer.None)
                    {
                        // Preserve the original fill if the theme color is empty
                        visualElement.Fill = tmpfillcolor;
                    }
                    else
                    {
                        visualElement.Fill = SvgPaintServer.None;
                    }

                    if (strokeColor != Color.Empty)
                    {
                        visualElement.Stroke = new SvgColourServer(strokeColor);
                    }
                    else if (visualElement.Stroke != null && visualElement.Stroke != SvgPaintServer.None)
                    {
                        // Preserve the original stroke if the theme color is empty
                        visualElement.Stroke = tmpstrokecolor;
                    }
                    else
                    {
                        visualElement.Stroke = SvgPaintServer.None;
                    }

                    // Log the updated colors
                    MiscFunctions.SendLog($"After: {element.GetType().Name}, Fill={visualElement.Fill}, Stroke={visualElement.Stroke}");
                }

                // Fix SvgText-specific attributes
                if (element is SvgText svgText)
                {
                    try
                    {
                        if (svgText.TextAnchor != SvgTextAnchor.Start &&
                            svgText.TextAnchor != SvgTextAnchor.Middle &&
                            svgText.TextAnchor != SvgTextAnchor.End)
                        {
                            svgText.TextAnchor = SvgTextAnchor.Start;
                        }
                    }
                    catch (Exception ex)
                    {
                        MiscFunctions.SendLog($"Error setting TextAnchor: {ex.Message}");
                        svgText.TextAnchor = SvgTextAnchor.Start;
                    }

                    try
                    {
                        if (svgText.StrokeOpacity < 0 || float.IsNaN(svgText.StrokeOpacity) || svgText.StrokeOpacity > 1)
                        {
                            svgText.StrokeOpacity = 1.0f;
                        }
                    }
                    catch (Exception ex)
                    {
                        MiscFunctions.SendLog($"Error setting StrokeOpacity: {ex.Message}");
                        svgText.StrokeOpacity = 1.0f;
                    }

                    try
                    {
                        if (svgText.Stroke == null || svgText.Stroke == SvgPaintServer.None)
                        {
                            svgText.Stroke = new SvgColourServer(Color.Black);
                        }
                    }
                    catch (Exception ex)
                    {
                        MiscFunctions.SendLog($"Error setting Stroke: {ex.Message}");
                        svgText.Stroke = new SvgColourServer(Color.Black);
                    }
                }
            }

            Invalidate();
        }


        #endregion "Theme Properties"
        #region "Image Drawing Methods"
        public void Draw(Graphics g, Rectangle destRect, Rectangle drawRect)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
  
            var originalTransform = g.Transform;
            try
            {
                float effectiveRotation = _manualRotationAngle + (IsSpinning ? _rotationAngle : 0);

                // Find the actual center of drawRect for proper positioning
                PointF center = new PointF(drawRect.X + drawRect.Width / 2f, drawRect.Y + drawRect.Height / 2f);

                // Translate to the new center for proper rotation
                g.TranslateTransform(center.X, center.Y);
                g.RotateTransform(effectiveRotation);
                g.TranslateTransform(-center.X, -center.Y);

                if (isSvg && svgDocument != null)
                {
                    var imageSize = svgDocument.GetDimensions();
                    var scaledBounds = GetScaledBounds(new SizeF(imageSize.Width, imageSize.Height), drawRect);

                    if (scaledBounds.Width > 0 && scaledBounds.Height > 0)
                    {
                        // Move image to its correct position inside drawRect
                        g.TranslateTransform(scaledBounds.X, scaledBounds.Y);
                        g.ScaleTransform(scaledBounds.Width / imageSize.Width, scaledBounds.Height / imageSize.Height);
                        svgDocument.Draw(g);
                    }
                }
                else if (regularImage != null)
                {
                    var scaledBounds = GetScaledBounds(new SizeF(regularImage.Width, regularImage.Height), drawRect);

                    if (scaledBounds.Width > 0 && scaledBounds.Height > 0)
                    {
                        g.DrawImage(regularImage, new RectangleF(
                            scaledBounds.X,
                            scaledBounds.Y,
                            scaledBounds.Width,
                            scaledBounds.Height
                        ));
                    }
                }
            }
            finally
            {
                g.Transform = originalTransform; // Restore graphics state
            }
        }

        public void DrawImage(Graphics g, Rectangle imageRect)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

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
         //   DrawingRect.Inflate(-1, -1); // Adjust for border thickness
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Fill the background with BackColor

            // Use spin functionality if enabled
            DrawImage(
                e.Graphics,
                DrawingRect
            );
            DrawBadge(e.Graphics);
        }
        #endregion "Image Drawing Methods"
        #region "Loading Images"
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
                
                // Console.WriteLine($"Loading image: {path}");
                if (IsEmbeddedResource(path))
                {
                    // Console.WriteLine("Loading from embedded resource 1"); 
                    // Attempt to load from embedded resources
                    bool isJustFileName = !path.Contains("\\") && !path.Contains("/") && path.Count(c => c == '.') == 1;
                    if (isJustFileName)
                    {
                       string newpath=ImageListHelper.GetImagePathFromName(path);
                        if (newpath != null)
                        {
                            path = newpath;
                        }
                    }
                    retval = LoadImageFromEmbeddedResource(path);
                    // Console.WriteLine("Loading from embedded resource 2");
                }
                else
                {
                    if (File.Exists(path))
                    {
                        // Console.WriteLine("Loading from file system");
                        // Load from file system
                        retval = LoadImageFromFile(path);
                        // Console.WriteLine("Loading from file system 2");
                    }
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error loading image: {ex.Message}");
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
                    // Console.WriteLine("Unsupported image format. Supported formats are: SVG, PNG, JPG, BMP.");
                    break;
            }

            return retval;
        }

        public bool IsEmbeddedResource(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            string normalizedPath = path.Trim().Replace("\\", ".").Replace("/", ".");

            // Check if it exists in any loaded assembly
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var resourceNames = assembly.GetManifestResourceNames();

                if (resourceNames.Any(name => name.Equals(normalizedPath, StringComparison.OrdinalIgnoreCase)))
                {
                    return true; // Found as an embedded resource
                }
            }
            // if it's just a file name no path,its an embedded resource 
            // Check if the path is just a filename (no path separators)
            // Check if the path is just a filename (no path separators)
            bool isJustFileName = !path.Contains("\\") && !path.Contains("/")  && path.Count(c => c == '.') == 1;
            if (isJustFileName)
            {
                return true; // It's likely an embedded resource
            }
            // If it's a valid file path, it's NOT an embedded resource
            if (File.Exists(path) || Directory.Exists(path))
            {
                return false;
            }

            // If it has no valid file extension, assume it's an embedded resource
            return string.IsNullOrEmpty(Path.GetExtension(path));
        }

        /// <summary>
        /// Load an image from the embedded resources (checks the current assembly).
        /// </summary>
        public bool LoadImageFromEmbeddedResource(string resourcePath)
        {
            try
            {
                Stream stream = null;
                string matchedResource = null;

                // Normalize resource path (remove starting dots or extra spaces)
                string normalizedResourcePath = resourcePath.Trim().Replace("\\", ".").Replace("/", ".");

                // Search all assemblies for the resource (case-insensitive)
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var resourceNames = assembly.GetManifestResourceNames();

                    // Perform a case-insensitive lookup
                    matchedResource = resourceNames
                        .FirstOrDefault(name => name.Equals(normalizedResourcePath, StringComparison.OrdinalIgnoreCase));

                    if (matchedResource != null)
                    {
                        stream = assembly.GetManifestResourceStream(matchedResource);
                        break;
                    }
                }

                if (stream != null)
                {
                    // Determine image type by file extension
                    string extension = Path.GetExtension(resourcePath).ToLower();
                    if (extension == ".svg")
                    {
                        // Read the stream into a string for sanitization
                        using (var reader = new StreamReader(stream))
                        {
                            string svgContent = reader.ReadToEnd();

                            // Sanitize invalid attribute values
                            svgContent = svgContent.Replace("text-anchor=\"left\"", "text-anchor=\"start\"")
                                                   .Replace("stroke-opacity=\"null\"", "stroke-opacity=\"1.0\"")
                                                   .Replace("stroke=\"null\"", "stroke=\"none\"");

                            // Convert the sanitized string back to a stream
                            using (var sanitizedStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(svgContent)))
                            {
                                svgDocument = SvgDocument.Open<SvgDocument>(sanitizedStream);
                                isSvg = true;
                            }
                        }
                    }
                    else
                    {
                        regularImage = Image.FromStream(stream);
                        isSvg = false;
                    }

                    // ApplyTheme(); // Apply theme after loading
                    Invalidate(); // Trigger a repaint
                    return true;
                }
                else
                {
                    // Console.WriteLine($"Embedded resource not found (case-insensitive): {resourcePath}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error loading embedded resource: {ex.Message}");
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
        public SvgDocument LoadSanitizedSvg(string svgFilePath)
        {
            // Read the raw SVG content
            string svgContent = File.ReadAllText(svgFilePath);

            // Replace invalid attribute values
            svgContent = svgContent.Replace("text-anchor=\"left\"", "text-anchor=\"start\"")
                                   .Replace("stroke-opacity=\"null\"", "stroke-opacity=\"1.0\"")
                                   .Replace("stroke=\"null\"", "stroke=\"none\"");

            // Parse the sanitized content into an SvgDocument
            return SvgDocument.FromSvg<SvgDocument>(svgContent);
        }
        public bool LoadSvg(string svgPath)
        {
            try
            {
                DisposeImages();
                svgDocument = LoadSanitizedSvg(svgPath);
                //  svgDocument = SvgDocument.Open(svgPath);
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
        #endregion "Loading Images"
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
            DrawingRect.Inflate(-1, -1); // Adjust for border thickness
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            // Fill the background with BackColor

            // Use spin functionality if enabled
            DrawImage(
                graphics,
                DrawingRect
            );
            DrawBadge(graphics);
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
        private void DisposeImages()
        {
            regularImage?.Dispose();
            regularImage = null;
            svgDocument = null;
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
