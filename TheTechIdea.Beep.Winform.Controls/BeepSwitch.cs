using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Report;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Specifies the orientation of the switch.
    /// Horizontal places labels on left/right;
    /// Vertical places labels on top/bottom.
    /// </summary>
    public enum SwitchOrientation
    {
        Horizontal,
        Vertical
    }

    [ToolboxItem(true)]
    [DisplayName("Beep Switch")]
    [Category("Beep Controls")]
    [Description("A cylindrical toggle switch control with customizable labels, images, and orientation.")]
    public class BeepSwitch : BeepControl
    {
        #region Fields

        private bool _checked = false;
        private SwitchOrientation _orientation = SwitchOrientation.Horizontal;
        private string _onLabel = "On";
        private string _offLabel = "Off";

        // Instead of direct Image fields, use two BeepImage instances.
        private string? _onImagePath;
        private string? _offImagePath;
        private BeepImage _onBeepImage;
        private BeepImage _offBeepImage;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the Checked property value changes.
        /// </summary>
        public event EventHandler? CheckedChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether the switch is On (Checked) or Off (Unchecked).
        /// </summary>
        [Category("Behavior")]
        [Description("Indicates whether the switch is On (Checked) or Off (Unchecked).")]
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    OnCheckedChanged();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Orientation of the switch (Horizontal or Vertical).
        /// </summary>
        [Category("Appearance")]
        [Description("Orientation of the switch (Horizontal or Vertical).")]
        [DefaultValue(SwitchOrientation.Horizontal)]
        public SwitchOrientation Orientation
        {
            get => _orientation;
            set
            {
                if (_orientation != value)
                {
                    _orientation = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Label text for the On state.
        /// </summary>
        [Category("Appearance")]
        [Description("Label for the On state.")]
        public string OnLabel
        {
            get => _onLabel;
            set
            {
                _onLabel = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Label text for the Off state.
        /// </summary>
        [Category("Appearance")]
        [Description("Label for the Off state.")]
        public string OffLabel
        {
            get => _offLabel;
            set
            {
                _offLabel = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Background image path for the On state.
        /// The image is loaded via a BeepImage instance and drawn clipped inside the capsule.
        /// </summary>
        [Category("Appearance")]
        [Description("Background image for the On state. This image is drawn and clipped inside the capsule track.")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string? OnImagePath
        {
            get => _onImagePath;
            set
            {
                _onImagePath = value;
                _onBeepImage.ImagePath = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Background image path for the Off state.
        /// The image is loaded via a BeepImage instance and drawn clipped inside the capsule.
        /// </summary>
        [Category("Appearance")]
        [Description("Background image for the Off state. This image is drawn and clipped inside the capsule track.")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string? OffImagePath
        {
            get => _offImagePath;
            set
            {
                _offImagePath = value;
                _offBeepImage.ImagePath = value;
                Invalidate();
            }
        }

        // Data binding properties (optional)
        [Browsable(true)]
        [Category("Data")]
        [Description("The property in the control to bind to the data source.")]
        public new string BoundProperty { get; set; } = "Checked";

        [Browsable(true)]
        [Category("Data")]
        [Description("The property in the data source to bind to.")]
        public new string DataSourceProperty { get; set; }

        [Browsable(true)]
        [Category("Data")]
        [Description("The linked property name.")]
        public new string LinkedProperty { get; set; }

        #endregion

        #region Constructor

        public BeepSwitch()
        {
            // Set default size and enable double buffering for smoother rendering.
            this.Width = 120;
            this.Height = 50;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            // Initialize the BeepImage instances.
            _onBeepImage = new BeepImage
            {
                IsSpinning = false,
                ScaleMode = Vis.Modules.ImageScaleMode.KeepAspectRatio
            };
            _offBeepImage = new BeepImage
            {
                IsSpinning = false,
                ScaleMode = Vis.Modules.ImageScaleMode.KeepAspectRatio
            };
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (Orientation == SwitchOrientation.Horizontal)
            {
                DrawHorizontalSwitch(g);
            }
            else
            {
                DrawVerticalSwitch(g);
            }
        }

        /// <summary>
        /// Draws the switch in horizontal orientation.
        /// Off label appears on the left and On label on the right.
        /// </summary>
        private void DrawHorizontalSwitch(Graphics g)
        {
            int padding = 8; // General padding

            // Measure label sizes.
            Size offLabelSize = TextRenderer.MeasureText(OffLabel, this.Font);
            Size onLabelSize = TextRenderer.MeasureText(OnLabel, this.Font);

            // Determine the track rectangle, reserving space for the labels.
            int trackX = offLabelSize.Width + padding * 2;
            int trackY = padding;
            int trackWidth = DrawingRect.Width - offLabelSize.Width - onLabelSize.Width - padding * 4;
            int trackHeight = DrawingRect.Height - padding * 2;
            Rectangle trackRect = new Rectangle(trackX, trackY, trackWidth, trackHeight);

            // Create a capsule (pill-shaped) path for the track.
            using (GraphicsPath trackPath = GetCapsulePath(trackRect, vertical: false))
            {
                // Draw the background image (clipped inside the capsule) if available.
                if (Checked && _onBeepImage.HasImage)
                {
                    g.SetClip(trackPath);
                    _onBeepImage.DrawImage(g, trackRect);
                    g.ResetClip();
                }
                else if (!Checked && _offBeepImage.HasImage)
                {
                    g.SetClip(trackPath);
                    _offBeepImage.DrawImage(g, trackRect);
                    g.ResetClip();
                }
                else
                {
                    // Otherwise, fill with a gradient based on the theme.
                    Color startColor, endColor;
                    if (Checked)
                    {
                        startColor = _currentTheme?.CheckBoxBackColor ?? Color.FromArgb(70, 70, 70);
                        endColor = _currentTheme?.CheckBoxForeColor ?? Color.FromArgb(45, 45, 45);
                    }
                    else
                    {
                        startColor = _currentTheme?.GradientStartColor ?? Color.White;
                        endColor = _currentTheme?.GradientEndColor ?? Color.LightGray;
                    }
                    using (LinearGradientBrush brush = new LinearGradientBrush(trackRect, startColor, endColor, LinearGradientMode.Horizontal))
                    {
                        g.FillPath(brush, trackPath);
                    }
                }
                // Draw the border of the track.
                using (Pen pen = new Pen(this.ForeColor, 1))
                {
                    g.DrawPath(pen, trackPath);
                }
            }

            // Draw the slider (the circular knob).
            int sliderDiameter = trackHeight - 4;
            int sliderY = trackRect.Y + 2;
            int sliderX = Checked ? (trackRect.Right - sliderDiameter - 2) : (trackRect.X + 2);
            Rectangle sliderRect = new Rectangle(sliderX, sliderY, sliderDiameter, sliderDiameter);
            using (GraphicsPath sliderPath = new GraphicsPath())
            {
                sliderPath.AddEllipse(sliderRect);
                using (PathGradientBrush pgb = new PathGradientBrush(sliderPath))
                {
                    pgb.CenterColor = Color.White;
                    pgb.SurroundColors = new Color[] { Color.LightGray };
                    g.FillEllipse(pgb, sliderRect);
                }
                using (Pen pen = new Pen(this.ForeColor, 1))
                {
                    g.DrawEllipse(pen, sliderRect);
                }
            }

            // Draw the labels.
            Rectangle offLabelRect = new Rectangle(padding, trackRect.Y, offLabelSize.Width, trackRect.Height);
            TextRenderer.DrawText(g, OffLabel, this.Font, offLabelRect, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            Rectangle onLabelRect = new Rectangle(trackRect.Right + padding, trackRect.Y, onLabelSize.Width, trackRect.Height);
            TextRenderer.DrawText(g, OnLabel, this.Font, onLabelRect, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        /// <summary>
        /// Draws the switch in vertical orientation.
        /// On label appears at the top and Off label at the bottom.
        /// </summary>
        private void DrawVerticalSwitch(Graphics g)
        {
            int padding = 8; // General padding

            // Measure label sizes.
            Size onLabelSize = TextRenderer.MeasureText(OnLabel, this.Font);
            Size offLabelSize = TextRenderer.MeasureText(OffLabel, this.Font);

            // Determine the track rectangle, leaving room for the labels.
            int trackY = onLabelSize.Height + padding * 2;
            int trackX = padding;
            int trackHeight = DrawingRect.Height - onLabelSize.Height - offLabelSize.Height - padding * 4;
            int trackWidth = DrawingRect.Width - padding * 2;
            Rectangle trackRect = new Rectangle(trackX, trackY, trackWidth, trackHeight);

            // Create a capsule path for the vertical track.
            using (GraphicsPath trackPath = GetCapsulePath(trackRect, vertical: true))
            {
                if (Checked && _onBeepImage.HasImage)
                {
                    g.SetClip(trackPath);
                    _onBeepImage.DrawImage(g, trackRect);
                    g.ResetClip();
                }
                else if (!Checked && _offBeepImage.HasImage)
                {
                    g.SetClip(trackPath);
                    _offBeepImage.DrawImage(g, trackRect);
                    g.ResetClip();
                }
                else
                {
                    Color startColor, endColor;
                    if (Checked)
                    {
                        startColor = _currentTheme?.CheckBoxBackColor ?? Color.FromArgb(70, 70, 70);
                        endColor = _currentTheme?.CheckBoxForeColor ?? Color.FromArgb(45, 45, 45);
                    }
                    else
                    {
                        startColor = _currentTheme?.GradientStartColor ?? Color.White;
                        endColor = _currentTheme?.GradientEndColor ?? Color.LightGray;
                    }
                    using (LinearGradientBrush brush = new LinearGradientBrush(trackRect, startColor, endColor, LinearGradientMode.Vertical))
                    {
                        g.FillPath(brush, trackPath);
                    }
                }
                using (Pen pen = new Pen(this.ForeColor, 1))
                {
                    g.DrawPath(pen, trackPath);
                }
            }

            // Draw the slider (toggle knob).
            int sliderDiameter = trackWidth - 4;
            int sliderX = trackRect.X + 2;
            int sliderY = Checked ? (trackRect.Y + 2) : (trackRect.Bottom - sliderDiameter - 2);
            Rectangle sliderRect = new Rectangle(sliderX, sliderY, sliderDiameter, sliderDiameter);
            using (GraphicsPath sliderPath = new GraphicsPath())
            {
                sliderPath.AddEllipse(sliderRect);
                using (PathGradientBrush pgb = new PathGradientBrush(sliderPath))
                {
                    pgb.CenterColor = Color.White;
                    pgb.SurroundColors = new Color[] { Color.LightGray };
                    g.FillEllipse(pgb, sliderRect);
                }
                using (Pen pen = new Pen(this.ForeColor, 1))
                {
                    g.DrawEllipse(pen, sliderRect);
                }
            }

            // Draw the labels.
            Rectangle onLabelRect = new Rectangle(0, padding, this.Width, onLabelSize.Height);
            TextRenderer.DrawText(g, OnLabel, this.Font, onLabelRect, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            Rectangle offLabelRect = new Rectangle(0, trackRect.Bottom + padding, this.Width, offLabelSize.Height);
            TextRenderer.DrawText(g, OffLabel, this.Font, offLabelRect, this.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        /// <summary>
        /// Creates a capsule-shaped GraphicsPath for the specified rectangle.
        /// </summary>
        /// <param name="rect">The rectangle for the capsule.</param>
        /// <param name="vertical">If true, creates a vertical capsule; otherwise horizontal.</param>
        private GraphicsPath GetCapsulePath(Rectangle rect, bool vertical)
        {
            GraphicsPath path = new GraphicsPath();
            if (vertical)
            {
                int radius = rect.Width / 2;
                // Top arc.
                path.AddArc(rect.X, rect.Y, rect.Width, 2 * radius, 180, 180);
                // Bottom arc.
                path.AddArc(rect.X, rect.Bottom - 2 * radius, rect.Width, 2 * radius, 0, 180);
            }
            else
            {
                int radius = rect.Height / 2;
                // Left arc.
                path.AddArc(rect.X, rect.Y, rect.Height, rect.Height, 90, 180);
                // Right arc.
                path.AddArc(rect.Right - rect.Height, rect.Y, rect.Height, rect.Height, 270, 180);
            }
            path.CloseFigure();
            return path;
        }

        #endregion

        #region Mouse Interaction

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            // Toggle the state when clicked.
            this.Checked = !this.Checked;
        }

        #endregion

        #region Event Raisers

        protected virtual void OnCheckedChanged()
        {
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Data Binding Methods

        public new void RefreshBinding()
        {
            if (DataContext != null && !string.IsNullOrEmpty(DataSourceProperty))
            {
                var propInfo = DataContext.GetType().GetProperty(DataSourceProperty);
                if (propInfo != null)
                {
                    this.Checked = (bool)propInfo.GetValue(DataContext);
                }
            }
        }

        public new void SetValue(object value)
        {
            if (value != null)
            {
                this.Checked = (bool)value;
            }
        }

        public new object GetValue()
        {
            return this.Checked;
        }

        public new void ClearValue()
        {
            this.Checked = false;
        }

        public new bool HasFilterValue()
        {
            return true;
        }

        public new AppFilter ToFilter()
        {
            return new AppFilter
            {
                FieldName = BoundProperty,
                FilterValue = this.Checked.ToString(),
                Operator = "="
            };
        }

        #endregion

        #region Theme Support

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            // Apply theme colors to the control.
            this.ForeColor = _currentTheme.CheckBoxForeColor;
            this.BackColor = _currentTheme.CheckBoxBackColor;
            Invalidate();
        }

        #endregion

        #region Disposal

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _onBeepImage?.Dispose();
                _offBeepImage?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
