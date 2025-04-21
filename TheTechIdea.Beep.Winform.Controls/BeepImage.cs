using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using Svg;
using System.Drawing.Text;
using Timer = System.Windows.Forms.Timer;
using TheTechIdea.Beep.Vis.Modules;
using System.Diagnostics;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using System.Drawing.Imaging;





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
        #region "Fields"
        private Image regularImage;
        private bool isSvg = false;
        private string _advancedImagePath;
        private bool _flipX = false;
        private bool _flipY = false;

        // Property for the image path (SVG, PNG, JPG, BMP)
        protected string _imagepath;
        private ImageClipShape _clipShape = ImageClipShape.None;
        private GraphicsPath _customClipPath = null;
        private float _cornerRadius = 10f; // For rounded rectangle

        #endregion "Fields"
        #region "Properties"
        public SvgDocument svgDocument { get; private set; }

        [Category("Appearance")]
        [Description("Determines the shape used to clip the image")]
        public ImageClipShape ClipShape
        {
            get => _clipShape;
            set
            {
                _clipShape = value;
                Invalidate();
            }
        }

        [Category("Appearance")]
        [Description("Corner radius for RoundedRect shape")]
        public float CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = Math.Max(0, value);
                Invalidate();
            }
        }

        [Browsable(false)]
        public GraphicsPath CustomClipPath
        {
            get => _customClipPath;
            set
            {
                _customClipPath = value;
                if (value != null)
                    ClipShape = ImageClipShape.Custom;
                Invalidate();
            }
        }
        private int _baseSize = 50; // Default size
        private bool _grayscale = false;
        [Category("Effects")]
        public bool Grayscale
        {
            get => _grayscale;
            set { _grayscale = value; Invalidate(); }
        }
        private float _opacity = 1.0f;
        [Category("Effects")]
        [Description("Opacity from 0.0 (transparent) to 1.0 (opaque).")]
        public float Opacity
        {
            get => _opacity;
            set { _opacity = Math.Max(0, Math.Min(1, value)); Invalidate(); }
        }

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
        #endregion "Properties"

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

        public void ApplyThemeToSvg()
        {
            if (svgDocument == null)
            {
                MiscFunctions.SendLog("ApplyThemeToSvg: svgDocument is null");
                return;
            }

            // Decide what colors to apply based on your theme settings.
            Color actualFillColor, actualStrokeColor, actualbackcolor;
       //   _imageEmbededin = ImageEmbededin.Button;
            if ( _currentTheme != null)
            {
                switch (_imageEmbededin)
                {
                    case ImageEmbededin.TabPage:
                        actualFillColor = _currentTheme.TabForeColor;
                        actualStrokeColor = _currentTheme.TabForeColor;
                        actualbackcolor = _currentTheme.TabBackColor;
                        break;
                    case ImageEmbededin.AppBar:
                        actualFillColor = _currentTheme.AppBarForeColor;
                        actualStrokeColor = _currentTheme.AppBarForeColor;
                        actualbackcolor = _currentTheme.AppBarBackColor;
                        break;
                    case ImageEmbededin.Menu:
                    case ImageEmbededin.MenuBar:
                        actualFillColor = _currentTheme.MenuForeColor;
                        actualStrokeColor = _currentTheme.MenuForeColor;
                        actualbackcolor = _currentTheme.MenuBackColor;
                        break;
                    case ImageEmbededin.TreeView:
                        actualFillColor = _currentTheme.TreeForeColor;
                        actualStrokeColor = _currentTheme.TreeForeColor;
                        actualbackcolor = _currentTheme.TreeBackColor;
                        break;
                    case ImageEmbededin.SideBar:
                        actualFillColor = _currentTheme.SideMenuForeColor;
                        actualStrokeColor = _currentTheme.SideMenuForeColor;
                        actualbackcolor = _currentTheme.SideMenuBackColor;
                        break;
                    case ImageEmbededin.ListBox:
                    case ImageEmbededin.Form:
                    case ImageEmbededin.ListView:
                        actualFillColor = _currentTheme.ListForeColor;
                        actualStrokeColor = _currentTheme.ListForeColor;
                        actualbackcolor = _currentTheme.ListBackColor;
                        break;
                    case ImageEmbededin.Label:
                        actualFillColor = _currentTheme.LabelForeColor;
                        actualStrokeColor = _currentTheme.LabelForeColor;
                        actualbackcolor = _currentTheme.LabelBackColor;
                        break;
                    case ImageEmbededin.TextBox:
                        actualFillColor = _currentTheme.TextBoxForeColor;
                        actualStrokeColor = _currentTheme.TextBoxForeColor;
                        actualbackcolor = _currentTheme.TextBoxBackColor;
                        break;
                    case ImageEmbededin.ComboBox:
                        actualFillColor = _currentTheme.ComboBoxForeColor;
                        actualStrokeColor = _currentTheme.ComboBoxForeColor;
                        actualbackcolor = _currentTheme.ComboBoxBackColor;
                        break;
                    case ImageEmbededin.DataGridView:
                        actualFillColor = _currentTheme.GridHeaderForeColor;
                        actualStrokeColor = _currentTheme.GridHeaderForeColor;
                        actualbackcolor = _currentTheme.GridHeaderBackColor;
                        break;
                    case ImageEmbededin.Button:
                    default:
                        actualFillColor = ForeColor;
                        actualStrokeColor = ForeColor;
                        actualbackcolor = BackColor;
                        break;
                }
            }
            else
            {
                actualFillColor = ForeColor;
                actualStrokeColor = ForeColor;
                actualbackcolor = BackColor;
            }

          

            MiscFunctions.SendLog($"ApplyThemeToSvg: Applying fillColor={actualFillColor}, strokeColor={actualStrokeColor}");

            // Create SvgColourServer instances for fill and stroke
            var fillServer = new SvgColourServer(actualFillColor);
            var strokeServer = new SvgColourServer(actualStrokeColor);
            var backgroundServer = new SvgColourServer(actualbackcolor);
            //svgDocument.Fill = fillServer;
            //svgDocument.Stroke = strokeServer;
            // Set the default stroke width
            // check if background color is not transparent in svgdocument by checking property in file



            //if (svgDocument.Fill != null  )
            //{
            //    svgDocument.Fill = backgroundServer;

            //}
            svgDocument.StrokeWidth = new SvgUnit(2); // Optional: set stroke width
                                                      // Recursively process all SVG nodes to update their color properties
                                                      // ProcessNodes(svgDocument.Descendants(), fillServer, strokeServer);
                                                      //   LoadImage(_imagepath); // Reload the image to apply the theme
            foreach (var node in svgDocument.Children)
            {


                // Update color properties.
                // You can check the properties if you want to preserve "None" values, or update unconditionally.
                node.Fill = fillServer;
                node.Color = fillServer;

                node.Stroke = strokeServer;
                node.StrokeWidth = new SvgUnit(2); // Optional: set stroke width

                // Recurse into child nodes.
                ProcessNodes(node.Descendants(), fillServer, strokeServer);
            }
            svgDocument.FlushStyles();
            // Optional: Log out the updated SVG XML for debugging.
            string modifiedXml = svgDocument.GetXML();
            MiscFunctions.SendLog($"ApplyThemeToSvg: Modified SVG XML: {modifiedXml}");

            // Trigger a redraw.
            try
            {
           //     svgDocument.FlushStyles();
                Invalidate();
                Refresh();
                MiscFunctions.SendLog("ApplyThemeToSvg: Redraw triggered");
            }
            catch (Exception ex)
            {
                MiscFunctions.SendLog($"ApplyThemeToSvg: Redraw failed - {ex.Message}");
            }
        }

        /// <summary>
        /// Recursively updates the given SVG nodes with the provided fill and stroke color servers.
        /// </summary>
        private void ProcessNodes(IEnumerable<SvgElement> nodes, SvgPaintServer fillServer, SvgPaintServer strokeServer)
        {
            foreach (var node in nodes)
            {
               

                // Update color properties.
                // You can check the properties if you want to preserve "None" values, or update unconditionally.
                node.Fill = fillServer;
                node.Color = fillServer;
                
                node.Stroke = strokeServer;

         
                    node.StrokeWidth = new SvgUnit(2); // Optional: set stroke width

                // Recurse into child nodes.
                ProcessNodes(node.Descendants(), fillServer, strokeServer);
            }
        }


        #endregion "Theme Properties"
        #region "Image Drawing Methods"
        private GraphicsPath CreateClipPath(Rectangle bounds)
        {
            GraphicsPath path = new GraphicsPath();

            switch (_clipShape)
            {
                case ImageClipShape.Circle:
                    // Create a circle path (perfect circle inscribed in bounds)
                    int diameter = Math.Min(bounds.Width, bounds.Height);
                    int offsetX = (bounds.Width - diameter) / 2;
                    int offsetY = (bounds.Height - diameter) / 2;
                    path.AddEllipse(bounds.X + offsetX, bounds.Y + offsetY, diameter, diameter);
                    break;

                case ImageClipShape.RoundedRect:
                    // Create a rounded rectangle path
                    path = GetRoundedRectPath(bounds, (int)_cornerRadius);
                    break;

                case ImageClipShape.Ellipse:
                    // Create an ellipse that fills the bounds
                    path.AddEllipse(bounds);
                    break;

                case ImageClipShape.Diamond:
                    // Create a diamond shape
                    Point[] diamondPoints = new Point[4];
                    diamondPoints[0] = new Point(bounds.X + bounds.Width / 2, bounds.Y);                    // Top
                    diamondPoints[1] = new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height / 2);    // Right
                    diamondPoints[2] = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height);    // Bottom
                    diamondPoints[3] = new Point(bounds.X, bounds.Y + bounds.Height / 2);                   // Left
                    path.AddPolygon(diamondPoints);
                    break;

                case ImageClipShape.Triangle:
                    // Create an equilateral triangle
                    Point[] trianglePoints = new Point[3];
                    trianglePoints[0] = new Point(bounds.X + bounds.Width / 2, bounds.Y);                    // Top
                    trianglePoints[1] = new Point(bounds.X, bounds.Y + bounds.Height);                       // Bottom left
                    trianglePoints[2] = new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height);        // Bottom right
                    path.AddPolygon(trianglePoints);
                    break;

                case ImageClipShape.Hexagon:
                    // Create a hexagon shape
                    Point[] hexagonPoints = new Point[6];
                    int quarterHeight = bounds.Height / 4;
                    hexagonPoints[0] = new Point(bounds.X + bounds.Width / 2, bounds.Y);                    // Top
                    hexagonPoints[1] = new Point(bounds.X + bounds.Width, bounds.Y + quarterHeight);         // Top right
                    hexagonPoints[2] = new Point(bounds.X + bounds.Width, bounds.Y + 3 * quarterHeight);     // Bottom right
                    hexagonPoints[3] = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height);     // Bottom
                    hexagonPoints[4] = new Point(bounds.X, bounds.Y + 3 * quarterHeight);                    // Bottom left
                    hexagonPoints[5] = new Point(bounds.X, bounds.Y + quarterHeight);                        // Top left
                    path.AddPolygon(hexagonPoints);
                    break;

              
                // Fallthrough to default if no custom path

                case ImageClipShape.None:
                default:
                    // No clipping - just use the full rectangle
                    path.AddRectangle(bounds);
                    break;
            }

            return path;
        }

        /// <summary>
        /// Draws the image with clipping to the specified rectangle.
        /// </summary>
        /// <param name="g">The Graphics object to draw on</param>
        /// <param name="destRect">The destination rectangle in the target surface</param>
        /// <param name="drawRect">The source rectangle from the image to draw</param>
        public void Draw(Graphics g, Rectangle destRect, Rectangle drawRect)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var originalTransform = g.Transform;
            var originalClip = g.Clip;

            try
            {
                // Calculate rotation parameters
                float effectiveRotation = _manualRotationAngle + (IsSpinning ? _rotationAngle : 0);

                // Get center point of destination rectangle
                PointF center = new PointF(destRect.X + destRect.Width / 2f, destRect.Y + destRect.Height / 2f);

                // Apply clipping if needed
                if (ClipShape != ImageClipShape.None)
                {
                    using (GraphicsPath clipPath = CreateClipPath(destRect))
                    {
                        g.SetClip(clipPath);
                    }
                }

                // Set up transformation for rotation and flipping
                g.TranslateTransform(center.X, center.Y);

                if (_flipX || _flipY)
                {
                    g.ScaleTransform(_flipX ? -1 : 1, _flipY ? -1 : 1);
                }

                if (effectiveRotation != 0)
                {
                    g.RotateTransform(effectiveRotation);
                }

                g.TranslateTransform(-center.X, -center.Y);

                // Now draw the image
                if (isSvg && svgDocument != null)
                {
                    var imageSize = svgDocument.GetDimensions();

                    // Calculate the scaling factors
                    float scaleX = (float)destRect.Width / drawRect.Width;
                    float scaleY = (float)destRect.Height / drawRect.Height;

                    // Create a transformation matrix for the SVG
                    g.TranslateTransform(destRect.X - drawRect.X * scaleX, destRect.Y - drawRect.Y * scaleY);
                    g.ScaleTransform(scaleX, scaleY);

                    svgDocument.Draw(g);
                }
                else if (regularImage != null)
                {
                    // For bitmap images, we can use DrawImage with source and destination rectangles
                    if (_grayscale || _opacity < 1.0f)
                    {
                        using (ImageAttributes imageAttr = new ImageAttributes())
                        {
                            ColorMatrix colorMatrix = new ColorMatrix();

                            if (_grayscale)
                            {
                                // Grayscale conversion matrix
                                colorMatrix.Matrix00 = colorMatrix.Matrix11 = colorMatrix.Matrix22 = 0.299f;
                                colorMatrix.Matrix01 = colorMatrix.Matrix12 = 0.587f;
                                colorMatrix.Matrix02 = colorMatrix.Matrix21 = 0.114f;
                                colorMatrix.Matrix10 = colorMatrix.Matrix20 = 0;
                            }

                            // Apply opacity
                            colorMatrix.Matrix33 = _opacity;

                            imageAttr.SetColorMatrix(colorMatrix);

                            g.DrawImage(regularImage, destRect,
                                drawRect.X, drawRect.Y, drawRect.Width, drawRect.Height,
                                GraphicsUnit.Pixel, imageAttr);
                        }
                    }
                    else
                    {
                        g.DrawImage(regularImage, destRect,
                            drawRect.X, drawRect.Y, drawRect.Width, drawRect.Height,
                            GraphicsUnit.Pixel);
                    }
                }
            }
            finally
            {
                // Restore original graphics state
                g.Clip = originalClip;
                g.Transform = originalTransform;
            }
        }

        public void DrawImage(Graphics g, Rectangle imageRect)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var originalTransform = g.Transform;
            var originalCompositingMode = g.CompositingMode;
            var originalClip = g.Clip;

            try
            {
                // --- Animation Setup ---
                float rotation = _manualRotationAngle + (IsSpinning ? _rotationAngle : 0);
                float scale = (_isPulsing || _isBouncing) ? _pulseScale : 1.0f;
                float alpha = _isFading ? _fadeAlpha : 1.0f;
                int shakeOffset = _isShaking ? _shakeOffset : 0;

                PointF center = new PointF(
                    imageRect.X + imageRect.Width / 2f,
                    imageRect.Y + imageRect.Height / 2f
                );

                // --- Create clipping path if shape clipping is enabled ---
                if (ClipShape != ImageClipShape.None)
                {
                    using (GraphicsPath clipPath = CreateClipPath(imageRect))
                    {
                        // Create a matrix for transforming the clip path
                        using (Matrix clipMatrix = new Matrix())
                        {
                            // Apply the same transformations to the clip path as we'll apply to the image
                            clipMatrix.Translate(center.X, center.Y);
                            clipMatrix.Scale(scale, scale);
                            // We don't rotate the clip path - it stays in place
                            clipMatrix.Translate(-center.X, -center.Y);

                            // Apply the transformation to the clip path
                            clipPath.Transform(clipMatrix);

                            // Set the clip region
                            g.SetClip(clipPath);
                        }
                    }
                }

                // --- Apply Transforms ---
                g.TranslateTransform(center.X, center.Y);

                if (_flipX || _flipY)
                {
                    g.ScaleTransform(_flipX ? -1 : 1, _flipY ? -1 : 1);
                    g.TranslateTransform(_flipX ? -2 * center.X : 0, _flipY ? -2 * center.Y : 0);
                }

                g.ScaleTransform(scale, scale);
                g.RotateTransform(rotation);
                g.TranslateTransform(-center.X, -center.Y);

                if (shakeOffset != 0)
                {
                    g.TranslateTransform(shakeOffset, 0);
                }

                // --- Prepare drawing bounds ---
                RectangleF scaledBounds;
                SizeF imageSize;

                if (isSvg && svgDocument != null)
                {
                    imageSize = svgDocument.GetDimensions();
                    scaledBounds = GetScaledBounds(imageSize, imageRect);

                    if (scaledBounds.Width > 0 && scaledBounds.Height > 0)
                    {
                        g.TranslateTransform(scaledBounds.X, scaledBounds.Y);
                        g.ScaleTransform(scaledBounds.Width / imageSize.Width, scaledBounds.Height / imageSize.Height);

                        if (alpha < 1.0f)
                            g.CompositingMode = CompositingMode.SourceOver;

                        svgDocument.Draw(g);
                    }
                }
                else if (regularImage != null)
                {
                    imageSize = regularImage.Size;
                    scaledBounds = GetScaledBounds(imageSize, imageRect);

                    if (scaledBounds.Width > 0 && scaledBounds.Height > 0)
                    {
                        if (alpha < 1.0f || _grayscale)
                        {
                            ColorMatrix matrix = new ColorMatrix();

                            if (_grayscale)
                            {
                                // Grayscale matrix values
                                matrix.Matrix00 = matrix.Matrix11 = matrix.Matrix22 = 0.299f;
                                matrix.Matrix01 = matrix.Matrix12 = 0.587f;
                                matrix.Matrix02 = matrix.Matrix21 = 0.114f;
                                matrix.Matrix10 = matrix.Matrix20 = 0;
                            }

                            matrix.Matrix33 = alpha * _opacity; // Apply opacity

                            ImageAttributes attr = new ImageAttributes();
                            attr.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                            g.DrawImage(
                                regularImage,
                                new Rectangle(
                                    (int)scaledBounds.X,
                                    (int)scaledBounds.Y,
                                    (int)scaledBounds.Width,
                                    (int)scaledBounds.Height
                                ),
                                0,
                                0,
                                regularImage.Width,
                                regularImage.Height,
                                GraphicsUnit.Pixel,
                                attr
                            );
                            attr.Dispose();
                        }
                        else
                        {
                            g.DrawImage(
                                regularImage,
                                new RectangleF(
                                    scaledBounds.X,
                                    scaledBounds.Y,
                                    scaledBounds.Width,
                                    scaledBounds.Height
                                )
                            );
                        }
                    }
                }
            }
            finally
            {
                // Restore original graphics state
                g.Clip = originalClip;
                g.Transform = originalTransform;
                g.CompositingMode = originalCompositingMode;
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
        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            DrawImage(
               g,
               DrawingRect
           );
        }
        #endregion "Image Drawing Methods"
        #region "Preview Generation Methods"
        /// <summary>
        /// Applies the current clipping shape to an image and returns the result
        /// </summary>
        /// <param name="sourceImage">The source image to clip</param>
        /// <returns>A new image with the clipping shape applied</returns>
        public Image GenerateShapedImage(Image sourceImage)
        {
            if (sourceImage == null)
                return null;

            Bitmap result = new Bitmap(sourceImage.Width, sourceImage.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Create clip path based on the image dimensions
                Rectangle imageBounds = new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
                using (GraphicsPath clipPath = CreateClipPath(imageBounds))
                {
                    // Set clip region
                    g.SetClip(clipPath);

                    // Draw the image
                    g.DrawImage(sourceImage, 0, 0, sourceImage.Width, sourceImage.Height);
                }
            }

            return result;
        }

        /// <summary>
        /// Creates an image with the specified shape, color, and size
        /// </summary>
        public static Image CreateShapedImage(ImageClipShape shape, Color fillColor, Size size, float cornerRadius = 10f)
        {
            Bitmap result = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(result))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                Rectangle bounds = new Rectangle(0, 0, size.Width, size.Height);

                using (GraphicsPath path = new GraphicsPath())
                {
                    switch (shape)
                    {
                        case ImageClipShape.Circle:
                            int diameter = Math.Min(size.Width, size.Height);
                            int offsetX = (size.Width - diameter) / 2;
                            int offsetY = (size.Height - diameter) / 2;
                            path.AddEllipse(offsetX, offsetY, diameter, diameter);
                            break;

                        case ImageClipShape.RoundedRect:
                            // Helper method from BeepControl
                            AddRoundedRectangle(path, bounds, cornerRadius);
                            break;

                        case ImageClipShape.Ellipse:
                            path.AddEllipse(bounds);
                            break;

                        case ImageClipShape.Diamond:
                            Point[] diamondPoints = new Point[4];
                            diamondPoints[0] = new Point(bounds.Width / 2, 0);                    // Top
                            diamondPoints[1] = new Point(bounds.Width, bounds.Height / 2);        // Right
                            diamondPoints[2] = new Point(bounds.Width / 2, bounds.Height);        // Bottom
                            diamondPoints[3] = new Point(0, bounds.Height / 2);                   // Left
                            path.AddPolygon(diamondPoints);
                            break;

                        case ImageClipShape.Triangle:
                            Point[] trianglePoints = new Point[3];
                            trianglePoints[0] = new Point(bounds.Width / 2, 0);                   // Top
                            trianglePoints[1] = new Point(0, bounds.Height);                      // Bottom left
                            trianglePoints[2] = new Point(bounds.Width, bounds.Height);           // Bottom right
                            path.AddPolygon(trianglePoints);
                            break;

                        case ImageClipShape.Hexagon:
                            Point[] hexagonPoints = new Point[6];
                            int quarterHeight = bounds.Height / 4;
                            hexagonPoints[0] = new Point(bounds.Width / 2, 0);                     // Top
                            hexagonPoints[1] = new Point(bounds.Width, quarterHeight);              // Top right
                            hexagonPoints[2] = new Point(bounds.Width, 3 * quarterHeight);          // Bottom right
                            hexagonPoints[3] = new Point(bounds.Width / 2, bounds.Height);          // Bottom
                            hexagonPoints[4] = new Point(0, 3 * quarterHeight);                     // Bottom left
                            hexagonPoints[5] = new Point(0, quarterHeight);                         // Top left
                            path.AddPolygon(hexagonPoints);
                            break;

                        default:
                            path.AddRectangle(bounds);
                            break;
                    }

                    using (Brush brush = new SolidBrush(fillColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }

            return result;
        }

        // Helper method to add a rounded rectangle to a path
        private static void AddRoundedRectangle(GraphicsPath path, Rectangle bounds, float radius)
        {
            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return;
            }

            // Ensure radius isn't too large for the rectangle
            float diameter = radius * 2;
            float sizeX = bounds.Width;
            float sizeY = bounds.Height;

            if (diameter > sizeX) diameter = sizeX;
            if (diameter > sizeY) diameter = sizeY;

            radius = diameter / 2;

            // Define corner rectangles
            RectangleF topLeftCorner = new RectangleF(bounds.X, bounds.Y, diameter, diameter);
            RectangleF topRightCorner = new RectangleF(bounds.X + sizeX - diameter, bounds.Y, diameter, diameter);
            RectangleF bottomLeftCorner = new RectangleF(bounds.X, bounds.Y + sizeY - diameter, diameter, diameter);
            RectangleF bottomRightCorner = new RectangleF(bounds.X + sizeX - diameter, bounds.Y + sizeY - diameter, diameter, diameter);

            // Add arcs for corners
            path.StartFigure();
            path.AddArc(topLeftCorner, 180, 90);
            path.AddArc(topRightCorner, 270, 90);
            path.AddArc(bottomRightCorner, 0, 90);
            path.AddArc(bottomLeftCorner, 90, 90);
            path.CloseFigure();
        }

        #endregion "Preview Generation Methods"
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
                                svgDocument.CustomAttributes.Remove("style");
                                svgDocument.FlushStyles();
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
                svgDocument.CustomAttributes.Remove("style");
                svgDocument.FlushStyles();
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
                    if (IsSpinning)
                        _rotationAngle = (_rotationAngle + SpinSpeed) % 360;

                    if (_isPulsing)
                    {
                        _pulseScale += 0.01f * _pulseDirection;
                        if (_pulseScale > 1.1f || _pulseScale < 0.9f)
                            _pulseDirection *= -1;
                    }

                    if (_isBouncing)
                    {
                        _pulseScale += 0.04f * _pulseDirection;
                        if (_pulseScale > 1.2f || _pulseScale < 0.8f)
                            _pulseDirection *= -1;
                    }

                    if (_isFading)
                    {
                        _fadeAlpha += 0.05f * _fadeDirection;
                        if (_fadeAlpha <= 0.4f || _fadeAlpha >= 1.0f)
                            _fadeDirection *= -1;
                    }

                    if (_isShaking)
                    {
                        _shakeOffset += 1 * _shakeDirection;
                        if (Math.Abs(_shakeOffset) > 3)
                            _shakeDirection *= -1;
                    }
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
            _pulseScale = 1.0f;
            _fadeAlpha = 1.0f;
            _shakeOffset = 0;

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
        #region "Rotate"
        public void Rotate90Clockwise()
        {
            if (isSvg && svgDocument != null)
                ManualRotationAngle = (ManualRotationAngle + 90f) % 360f;
            else if (regularImage != null)
                RotateImage(RotateFlipType.Rotate90FlipNone);
        }

        public void Rotate90CounterClockwise()
        {
            if (isSvg && svgDocument != null)
                ManualRotationAngle = (ManualRotationAngle - 90f + 360f) % 360f;
            else if (regularImage != null)
                RotateImage(RotateFlipType.Rotate270FlipNone);
        }

        public void Rotate180()
        {
            if (isSvg && svgDocument != null)
                ManualRotationAngle = (ManualRotationAngle + 180f) % 360f;
            else if (regularImage != null)
                RotateImage(RotateFlipType.Rotate180FlipNone);
        }

        public void FlipHorizontal()
        {
            if (isSvg && svgDocument != null)
            {
                _flipX = !_flipX;
                Invalidate();
            }
            else if (regularImage != null)
            {
                RotateImage(RotateFlipType.RotateNoneFlipX);
            }
        }

        public void FlipVertical()
        {
            if (isSvg && svgDocument != null)
            {
                _flipY = !_flipY;
                Invalidate();
            }
            else if (regularImage != null)
            {
                RotateImage(RotateFlipType.RotateNoneFlipY);
            }
        }

        /// <summary>
        /// Rotate or flip only regular images using built-in support
        /// </summary>
        public void RotateImage(RotateFlipType rotateFlipType)
        {
            if (regularImage != null)
            {
                regularImage.RotateFlip(rotateFlipType);
                Invalidate();
            }
            else
            {
                MessageBox.Show("Rotation is only supported for regular images (PNG, JPG, BMP).", "Rotate Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Resets all rotation and flip states
        /// </summary>
        public void ResetTransformations()
        {
            ManualRotationAngle = 0;
            _flipX = false;
            _flipY = false;
            if (regularImage != null)
            {
                // Reload the original image if needed
                if (!string.IsNullOrEmpty(ImagePath))
                {
                    LoadImage(ImagePath);
                }
            }
            Invalidate();
        }


        #endregion "Rotate"
        #region "Animation"
        [Category("Animation")]
        public bool IsPulsing
        {
            get => _isPulsing;
            set
            {
                _isPulsing = value;
                StartSpin(); // reuse same timer
            }
        }

        [Category("Animation")]
        public bool IsBouncing
        {
            get => _isBouncing;
            set
            {
                _isBouncing = value;
                StartSpin();
            }
        }

        [Category("Animation")]
        public bool IsFading
        {
            get => _isFading;
            set
            {
                _isFading = value;
                _fadeAlpha = 1.0f;
                _fadeDirection = -1;
                StartSpin();
            }
        }

        [Category("Animation")]
        public bool IsShaking
        {
            get => _isShaking;
            set
            {
                _isShaking = value;
                _shakeOffset = 0;
                _shakeDirection = 1;
                StartSpin();
            }
        }

        private bool _isPulsing = false;
        private bool _isBouncing = false;
        private bool _isShaking = false;
        private bool _isFading = false;

        private float _pulseScale = 1.0f;
        private int _pulseDirection = 1;

        private float _fadeAlpha = 1.0f;
        private int _fadeDirection = -1;

        private int _shakeOffset = 0;
        private int _shakeDirection = 1;

        #endregion "Animation"

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
    public enum ImageClipShape
    {
        None,       // No clipping (default rectangle)
        Circle,     // Perfect circle
        RoundedRect, // Rounded rectangle (uses BorderRadius)
        Ellipse,    // Oval/ellipse
        Diamond,    // Diamond shape
        Triangle,   // Triangle shape
        Hexagon,    // Hexagon shape
        Custom
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
        MenuBar,
    }
}
