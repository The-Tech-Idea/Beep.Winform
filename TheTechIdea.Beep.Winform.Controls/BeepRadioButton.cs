using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Provides data for the SelectedValueChanged event.
    /// </summary>
    public class RadioButtonSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// The index of the option that was selected.
        /// </summary>
        public int SelectedIndex { get; }

        /// <summary>
        /// The value of the option that was selected.
        /// </summary>
        public string SelectedValue { get; }

        public RadioButtonSelectedEventArgs(int index, string value)
        {
            SelectedIndex = index;
            SelectedValue = value;
        }
    }
    public enum RadioButtonOrientation { Horizontal, Vertical }
    /// <summary>
    /// A control that displays a list of options as radio buttons.
    /// All drawing is performed inside the DrawingRect.
    /// When one option is clicked, the SelectedValue property is updated
    /// and the SelectedValueChanged event is raised.
    /// </summary>
    [ToolboxItem(true)]
    [DisplayName("Beep Radio Button")]
    [Category("Beep Controls")]
    [Description("A control that displays a list of radio button options. When one option is selected, it updates the SelectedValue property and fires an event.")]
    public class BeepRadioButton : BeepControl
    {
        #region Fields

        // A designer–editable list of options.
        private BindingList<string> _options = new BindingList<string>();

        // The currently selected option.
        private string _selectedValue = string.Empty;

        // Used for hit testing and highlighting; stored in absolute coordinates.
        private List<Rectangle> _optionRectangles = new List<Rectangle>();

        // The index of the option currently under the mouse pointer.
        private int _hoverIndex = -1;
        // Orientation for layout.
        private RadioButtonOrientation _orientation = RadioButtonOrientation.Vertical;
        #endregion

        #region Events

        /// <summary>
        /// Occurs when the selected value changes.
        /// </summary>
        [Category("Action")]
        [Description("Occurs when the selected value changes.")]
        public event EventHandler<RadioButtonSelectedEventArgs> SelectedValueChanged;

        #endregion

        #region Properties
      


        private Font _textFont = new Font("Arial", 10);
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
        {
            get => _textFont;
            set
            {

                _textFont = value;
                UseThemeFont = false;
                Font = _textFont;
               
                Invalidate();


            }
        }
        /// <summary>
        /// Gets the list of options displayed by the control.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Data")]
        [Description("The list of options displayed by the control.")]
        public BindingList<string> Options
        {
            get { return _options; }
            set
            {
                _options = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the selected option.
        /// </summary>
        [Category("Data")]
        [Description("The currently selected option.")]
        public string SelectedValue
        {
            get { return _selectedValue; }
            set
            {
                if (_selectedValue != value)
                {
                    _selectedValue = value;
                    OnSelectedValueChanged(new RadioButtonSelectedEventArgs(SelectedIndex, _selectedValue));
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets the index of the selected option. Returns -1 if no option is selected.
        /// </summary>
        [Browsable(false)]
        public int SelectedIndex
        {
            get
            {
                if (string.IsNullOrEmpty(_selectedValue))
                    return -1;
                return _options.IndexOf(_selectedValue);
            }
        }
        /// <summary>
        /// Gets or sets the layout orientation for the radio button options.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Determines if the options are laid out vertically (stacked) or horizontally (side-by-side).")]
        [DefaultValue(RadioButtonOrientation.Vertical)]
        public RadioButtonOrientation Orientation
        {
            get { return _orientation; }
            set
            {
                if (_orientation != value)
                {
                    _orientation = value;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Constructor

        public BeepRadioButton()
        {
            // Enable double buffering for smooth drawing.
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            // Set default appearance.
            this.BackColor = Color.White;
            this.ForeColor = Color.Black;
            this.Font = new Font("Segoe UI", 9);

            // Repaint when the options collection changes.
            _options.ListChanged += (s, e) => { Invalidate(); };
        }

        #endregion

        #region Event Raisers

        protected virtual void OnSelectedValueChanged(RadioButtonSelectedEventArgs e)
        {
            SelectedValueChanged?.Invoke(this, e);
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Use DrawingRect as the drawing area.
            Rectangle drawRect = this.DrawingRect;

            // Set the clip to the DrawingRect.
            g.SetClip(drawRect);
            // Translate the coordinate system so that (0,0) corresponds to drawRect.Location.
            g.TranslateTransform(drawRect.X, drawRect.Y);

            // (Optional) Clear the drawing area. Uncomment if needed.
            // using (SolidBrush backBrush = new SolidBrush(this.BackColor))
            // {
            //     g.FillRectangle(backBrush, 0, 0, drawRect.Width, drawRect.Height);
            // }

            // Clear previous hit–testing rectangles.
           // _optionRectangles.Clear();

            if (_orientation == RadioButtonOrientation.Vertical)
            {
                // Vertical layout: options are stacked.
                int itemHeight = 30;           // Height per option.
                int indicatorSize = 16;        // Size of the radio indicator (circle).
                int paddingLeft = 10;          // Left padding inside DrawingRect.
                int paddingTop = 5;            // Top padding inside DrawingRect.
                int spacing = 5;               // Space between the radio indicator and the text.

                for (int i = 0; i < _options.Count; i++)
                {
                    int top = paddingTop + i * itemHeight;
                    // Create a rectangle for the entire option area (in local coordinates).
                    Rectangle localItemRect = new Rectangle(0, top, drawRect.Width, itemHeight);
                    // Convert local coordinates to absolute coordinates for hit testing.
                    Rectangle absoluteItemRect = new Rectangle(drawRect.X, drawRect.Y + top, drawRect.Width, itemHeight);
                    _optionRectangles.Add(absoluteItemRect);

                    // Highlight the option if the mouse is over it.
                    if (i == _hoverIndex)
                    {
                        using (SolidBrush brush = new SolidBrush(Color.LightGray))
                        {
                            g.FillRectangle(brush, localItemRect);
                        }
                    }

                    // Draw the radio indicator.
                    Rectangle indicatorRect = new Rectangle(paddingLeft, top + (itemHeight - indicatorSize) / 2, indicatorSize, indicatorSize);
                    using (Pen pen = new Pen(this.ForeColor, 2))
                    {
                        g.DrawEllipse(pen, indicatorRect);
                    }
                    // If this option is selected, draw a filled inner circle.
                    if (_options[i] == _selectedValue)
                    {
                        int innerSize = indicatorSize / 2;
                        Rectangle innerRect = new Rectangle(
                            indicatorRect.X + (indicatorSize - innerSize) / 2,
                            indicatorRect.Y + (indicatorSize - innerSize) / 2,
                            innerSize,
                            innerSize);
                        using (SolidBrush brush = new SolidBrush(this.ForeColor))
                        {
                            g.FillEllipse(brush, innerRect);
                        }
                    }

                    // Draw the option text.
                    Rectangle textRect = new Rectangle(indicatorRect.Right + spacing, top, drawRect.Width - (indicatorRect.Right + spacing), itemHeight);
                    TextRenderer.DrawText(g, _options[i], this.Font, textRect, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                }
            }
            else // Horizontal layout.
            {
                int indicatorSize = 16;   // Size of the radio indicator (circle).
                int padding = 5;          // Padding for each option.
                int spacing = 5;          // Gap between options.
                int x = padding;          // Starting x–coordinate within drawRect.

                // For horizontal layout, each option's width is determined by its text size.
                for (int i = 0; i < _options.Count; i++)
                {
                    string opt = _options[i];
                    Size textSize = TextRenderer.MeasureText(opt, this.Font);
                    // Calculate item width: indicator + spacing + text + extra padding on both sides.
                    int itemWidth = indicatorSize + spacing + textSize.Width + 2 * padding;
                    // Create the local rectangle for this option.
                    Rectangle localItemRect = new Rectangle(x, 0, itemWidth, drawRect.Height);
                    // Convert to absolute coordinates for hit testing.
                    Rectangle absoluteItemRect = new Rectangle(drawRect.X + localItemRect.X, drawRect.Y + localItemRect.Y, localItemRect.Width, localItemRect.Height);
                    _optionRectangles.Add(absoluteItemRect);

                    // Highlight if hovered.
                    if (i == _hoverIndex)
                    {
                        using (SolidBrush brush = new SolidBrush(Color.LightGray))
                        {
                            g.FillRectangle(brush, localItemRect);
                        }
                    }

                    // Draw the radio indicator at the left of the item.
                    Rectangle indicatorRect = new Rectangle(padding, (drawRect.Height - indicatorSize) / 2, indicatorSize, indicatorSize);
                    using (Pen pen = new Pen(this.ForeColor, 2))
                    {
                        g.DrawEllipse(pen, indicatorRect);
                    }
                    if (_options[i] == _selectedValue)
                    {
                        int innerSize = indicatorSize / 2;
                        Rectangle innerRect = new Rectangle(
                            indicatorRect.X + (indicatorSize - innerSize) / 2,
                            indicatorRect.Y + (indicatorSize - innerSize) / 2,
                            innerSize,
                            innerSize);
                        using (SolidBrush brush = new SolidBrush(this.ForeColor))
                        {
                            g.FillEllipse(brush, innerRect);
                        }
                    }

                    // Draw the text next to the indicator.
                    Rectangle textRect = new Rectangle(indicatorRect.Right + spacing, 0, textSize.Width + 2 * padding, drawRect.Height);
                    TextRenderer.DrawText(g, opt, this.Font, textRect, this.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

                    // Update x for the next option.
                    x += itemWidth + spacing;
                }
            }

            // Reset the transformation and clipping.
            g.ResetTransform();
            g.ResetClip();
        }


        #endregion

        #region Mouse Interaction

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int newHover = -1;
            // _optionRectangles are stored in absolute coordinates.
            for (int i = 0; i < _optionRectangles.Count; i++)
            {
                if (_optionRectangles[i].Contains(e.Location))
                {
                    newHover = i;
                    break;
                }
            }
            if (newHover != _hoverIndex)
            {
                _hoverIndex = newHover;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoverIndex = -1;
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            for (int i = 0; i < _optionRectangles.Count; i++)
            {
                if (_optionRectangles[i].Contains(e.Location))
                {
                    SelectedValue = _options[i];
                    break;
                }
            }
        }

        #endregion
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            this.BackColor = _currentTheme.PanelBackColor;
            this.ForeColor = _currentTheme.LabelForeColor;
            if (UseThemeFont)
            {
                _textFont = BeepThemesManager.ToFont(_currentTheme.LabelSmall);
            }

            Font = _textFont;
        }
    }
}
