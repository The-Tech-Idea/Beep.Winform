using System;
using System.ComponentModel;

using System.Drawing.Drawing2D;
using System.Drawing.Text;


namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [DisplayName("Beep Numeric UpDown")]
    [Description("A custom numeric up-down control with Beep theming.")]
    public class BeepNumericUpDown : BeepControl
    {
        private BeepButton _decrementButton;
        private BeepButton _incrementButton;
        private BeepTextBox _valueTextBox;

        private decimal _minimumValue = 0m;
        private decimal _maximumValue = 1000m;
        private decimal _incrementValue = 1m;
        private decimal _value = 0m;
        #region "Properties"
        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the minimum numeric value.")]
        public decimal MinimumValue
        {
            get => _minimumValue;
            set
            {
                _minimumValue = value;
                if (_value < _minimumValue)
                    _value = _minimumValue;
                UpdateTextBox();
            }
        }

        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the maximum numeric value.")]
        public decimal MaximumValue
        {
            get => _maximumValue;
            set
            {
                _maximumValue = value;
                if (_value > _maximumValue)
                    _value = _maximumValue;
                UpdateTextBox();
            }
        }

        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the amount by which the value changes.")]
        public decimal IncrementValue
        {
            get => _incrementValue;
            set => _incrementValue = value;
        }

        [Browsable(true)]
        [Category("Numeric Settings")]
        [Description("Specifies the current numeric value.")]
        public decimal Value
        {
            get => _value;
            set
            {
                // 🔹 If MinimumValue is null, ignore lower bound check.
                // 🔹 If MaximumValue is null, ignore upper bound check.
                bool withinRange =
                    (_minimumValue == decimal.MinValue || value >= _minimumValue) &&
                    (_maximumValue == decimal.MaxValue || value <= _maximumValue);

                if (!withinRange)
                    throw new ArgumentOutOfRangeException($"Value must be between {_minimumValue} and {_maximumValue}.");

                _value = value;
                UpdateTextBox();
                Invalidate();
            }
        }

        #endregion
       
        public BeepNumericUpDown() : base()
        {
            // Enable smooth resizing
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.Size = DefaultSize;
            Margin = new Padding(0);
        
            // Initialize child controls
            InitializeControls();
        }
        protected override Size DefaultSize => new Size(100, _valueTextBox?.PreferredHeight ?? 20);
        private void InitializeControls()
        {
            // Decrement Button (Left)
            _decrementButton = new BeepButton
            {
                Text = "-",
                IsFrameless = true,
                AutoSize = false  // We'll size it manually
            };
            _decrementButton.Click += DecrementButton_Click;

            // Increment Button (Right)
            _incrementButton = new BeepButton
            {
                Text = "+",
                IsFrameless = true,
                AutoSize = false  // We'll size it manually
            };
            _incrementButton.Click += IncrementButton_Click;

            // Value TextBox (Center)
            _valueTextBox = new BeepTextBox
            {
                TextAlignment = HorizontalAlignment.Center,
                IsFrameless = true,
                OnlyDigits = true,
                // If your BeepTextBox supports vertical centering, set it here.
                // Otherwise, consider making the TextBox tall enough so that the text appears centered.
            };
            _valueTextBox.TextChanged += ValueTextBox_TextChanged;

            // Add controls to the parent
            Controls.Add(_decrementButton);
            Controls.Add(_valueTextBox);
            Controls.Add(_incrementButton);
            UpdateDrawingRect();
            // Update layout initially
            UpdateLayout(DrawingRect);
        }
        private void UpdateLayout(Rectangle rect)
        {
            _valueTextBox.AutoSize = false; // Disable auto-sizing for manual layout.
            // First update your drawing rectangle.
            
           
            int padding = 1; // uniform padding on all sides

            // Get the preferred size of the text box.
            Size preferredSize = _valueTextBox.GetPreferredSize(Size.Empty);
            int preferredHeight = preferredSize.Height;

            // Calculate available width and height inside the DrawingRect.
            int availableWidth = rect.Width - 2 * padding;
            int availableHeight = rect.Height - 2 * padding;

            // Determine the height for child controls:
            // Use the smaller of the available height and the preferred height.
            // This will effectively "limit" the height to the preferred height.
            int elementHeight = rect.Height - (2 * padding);

            // Center the children vertically if there's extra space.
            int elementY = rect.Top + padding + ((availableHeight - elementHeight) / 2);

            // Define your desired minimum sizes.
            const int desiredButtonWidth = 15; // desired width for the buttons
            const int minTextBoxWidth = 10;      // minimum width for the textbox (adjust as needed)

            // Choose a button width:
            // For a square button, we base it on the available height.
            int buttonWidth = Math.Min(desiredButtonWidth, availableHeight);

            // Compute the remaining width for the textbox.
            // Layout: left padding + left button + padding + textbox + padding + right button + right padding.
            int textBoxWidth = availableWidth - (2 * buttonWidth) - (2 * padding);

            // If the available width is too narrow (i.e. textBoxWidth is less than its minimum),
            // adjust the button widths down so that the textbox gets at least minTextBoxWidth.
            if (textBoxWidth < minTextBoxWidth)
            {
                buttonWidth = Math.Max(0, (availableWidth - (2 * padding) - minTextBoxWidth) / 2);
                textBoxWidth = Math.Max(0, availableWidth - (2 * buttonWidth) - (2 * padding));
            }

            // Layout the Decrement Button (left side):
            int leftButtonX = rect.Left + padding;
            _decrementButton.Location = new Point(leftButtonX, elementY);
            _decrementButton.Size = new Size(buttonWidth, elementHeight);

            // Layout the TextBox (center):
            int textBoxX = leftButtonX + buttonWidth + padding;
            _valueTextBox.Location = new Point(textBoxX, elementY);
            _valueTextBox.Size = new Size(textBoxWidth, elementHeight);

            // Layout the Increment Button (right side):
            int rightButtonX = textBoxX + textBoxWidth + padding;
            _incrementButton.Location = new Point(rightButtonX, elementY);
            _incrementButton.Size = new Size(buttonWidth, elementHeight);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateDrawingRect();
            UpdateLayout(DrawingRect);
            Invalidate(); // force visual redraw too
        }
      
        public override void Draw(Graphics graphics, Rectangle rectangle)
        {
            UpdateLayout(rectangle);
            // Enable anti-aliasing for smooth rendering
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Fill the background with the theme's panel background color
            //using (SolidBrush backgroundBrush = new SolidBrush(_currentTheme.PanelBackColor))
            //{
            //    graphics.FillRectangle(backgroundBrush, rectangle);
            //}

            // Draw the border if needed
            //if (BorderThickness > 0)
            //{
            //    using (Pen borderPen = new Pen(_currentTheme.BorderColor, BorderThickness))
            //    {
            //        graphics.DrawRectangle(borderPen, rectangle);
            //    }
            //}

            // Define rectangles for each child control
            Rectangle decrementButtonRect = new Rectangle(_decrementButton.Left, _decrementButton.Top, _decrementButton.Width, _decrementButton.Height);
            Rectangle valueTextBoxRect = new Rectangle(_valueTextBox.Left, _valueTextBox.Top, _valueTextBox.Width, _valueTextBox.Height);
            Rectangle incrementButtonRect = new Rectangle(_incrementButton.Left, _incrementButton.Top, _incrementButton.Width, _incrementButton.Height);

            //// Draw the decrement button
            //if (_decrementButton != null)
            //{
            //    _decrementButton.Draw(graphics, decrementButtonRect);
            //}

            // Draw the text box
            if (_valueTextBox?.Visible == true && _valueTextBox.Width > 0 && _valueTextBox.Height > 0)
            {
                _valueTextBox.Draw(graphics, valueTextBoxRect);
            }


            //// Draw the increment button
            //if (_incrementButton != null)
            //{
            //    _incrementButton.Draw(graphics, incrementButtonRect);
            //}
        }
        #region "Event Handlers"
        private void IncrementButton_Click(object sender, EventArgs e)
        {
            decimal newValue = _value + _incrementValue;

            if (_maximumValue == decimal.MaxValue || newValue <= _maximumValue)
            {
                _value = newValue;
            }
            else
            {
                _value = _maximumValue;
            }

            UpdateTextBox();
            Invalidate();
        }

        private void DecrementButton_Click(object sender, EventArgs e)
        {
            decimal newValue = _value - _incrementValue;

            if (_minimumValue == decimal.MinValue || newValue >= _minimumValue)
            {
                _value = newValue;
            }
            else
            {
                _value = _minimumValue;
            }

            UpdateTextBox();
            Invalidate();
        }



        private void ValueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(_valueTextBox.Text, out decimal newValue))
            {
                // 🔹 Allow unrestricted values if no explicit limits are set.
                bool withinRange =
                    (_minimumValue == decimal.MinValue || newValue >= _minimumValue) &&
                    (_maximumValue == decimal.MaxValue || newValue <= _maximumValue);

                if (withinRange)
                {
                    _value = newValue;
                    Invalidate();
                    return;
                }
            }

            // 🔹 If the new value is invalid or out of range, revert to the last valid value.
            UpdateTextBox();
        }
        private void UpdateTextBox()
        {
            if (_valueTextBox == null) return;

            string newText = _value.ToString();
            if (_valueTextBox.Text != newText)
            {
                _valueTextBox.TextChanged -= ValueTextBox_TextChanged; // Prevent recursion
                _valueTextBox.Text = newText;
                _valueTextBox.TextChanged += ValueTextBox_TextChanged;
            }
        }

        #endregion


        public override void SetValue(object value)
        {
            if (value == null) return;

            decimal parsedValue = 0m;

            if (value is int intValue)
                parsedValue = Convert.ToDecimal(intValue);
            else if (value is long longValue)
                parsedValue = Convert.ToDecimal(longValue);
            else if (value is float floatValue)
                parsedValue = Convert.ToDecimal(floatValue);
            else if (value is double doubleValue)
                parsedValue = Convert.ToDecimal(doubleValue);
            else if (value is decimal decimalValue)
                parsedValue = decimalValue;
            else if (decimal.TryParse(value.ToString(), out decimal tempParsed))
                parsedValue = tempParsed;

            // 🔹 If no limits are set, allow any value.
            if (_minimumValue == decimal.MinValue && _maximumValue == decimal.MaxValue)
            {
                Value = parsedValue;
            }
            else
            {
                // 🔹 Apply restrictions only if limits are set.
                Value = Math.Max(_minimumValue, Math.Min(parsedValue, _maximumValue));
            }
        }



        public override object GetValue()
        {
            return Value;
        }
    }
}
