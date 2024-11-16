using TheTechIdea.Beep.Vis.Modules;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Category("Containers")]
    public class BeepPanel : BeepControl
    {


        private string _titleText = "Panel Title";
        private bool _showTitle = true;
        private bool _showTitleLine = true;
        private bool _titleLineFullWidth = true;
        private Color _titleLineColor = Color.Gray;
        private int _titleLineThickness = 2;
        private int _titleBottomY = 30;
        private ContentAlignment _titleAlignment = ContentAlignment.TopLeft;

        #region "Scrolling Properties"
        // Scrolling properties
        private bool _enableScrolling = false;
        private int _scrollOffset = 0;
        private int _scrollSpeed = 1;
        private Timer _scrollTimer;
        private int _scrollInterval = 10;
        private int _scrollDirection = 1;
        private VScrollBar _verticalScrollBar;
        private HScrollBar _horizontalScrollBar;
        #endregion "Scrolling Properties"

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Title Bottom Location Y")]
        public int TitleBottomY
        {
            get => _titleBottomY;
            set
            {
                _titleBottomY = value;
              
            }
        }
        // Title and Title Line properties
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The text displayed as the title.")]
        public string TitleText
        {
            get => _titleText;
            set
            {
                _titleText = value;
                Invalidate(); // Redraw when this property changes
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show a line below the title.")]
        public bool ShowTitleLine
        {
            get => _showTitleLine;
            set
            {
                _showTitleLine = value;
                Invalidate(); // Redraw when this property changes
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color of the line below the title.")]
        public Color TitleLineColor
        {
            get => _titleLineColor;
            set
            {
                _titleLineColor = value;
                Invalidate(); // Redraw when this property changes
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Thickness of the line below the title.")]
        public int TitleLineThickness
        {
            get => _titleLineThickness;
            set
            {
                _titleLineThickness = value;
                Invalidate(); // Redraw when this property changes
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Determines if the title is shown.")]
        public bool ShowTitle
        {
            get => _showTitle;
            set
            {
                _showTitle = value;
                Invalidate(); // Trigger repaint
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Alignment of the title text within the panel.")]
        public ContentAlignment TitleAlignment
        {
            get => _titleAlignment;
            set
            {
                _titleAlignment = value;
                Invalidate(); // Trigger repaint
            }
        }
        // Property to control whether the title line should span the full width or just the title width
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Draw the title line with full width or just below the title.")]

        public bool ShowTitleLineinFullWidth
        {
            get => _titleLineFullWidth;
            set
            {
                _titleLineFullWidth = value;
                Invalidate(); // Redraw when this property changes
            }
        }


        // Constructor
        public BeepPanel()
        {
            ApplyTheme();
            
            
        }
      
        public override void ApplyTheme()
        {
             Console.WriteLine("Applying Theme on Simple Panel");
            BackColor = _currentTheme.BackgroundColor;
            ForeColor = _currentTheme.TitleForColor;
            foreach (Control ctrl in Controls)
            {
                ApplyThemeToControl(ctrl);
                //if (ctrl is BeepButton)
                //{
                //    //  Console.WriteLine("Applying Theme to Button");
                //    ((BeepButton)ctrl).ApplyThemeOnImage = true;
                //    ((BeepButton)ctrl).Theme = Theme;
                //}
                //else if (ctrl is BeepLabel)
                //{
                //    ((BeepLabel)ctrl).Theme = Theme;
                //}
            }
           // Invalidate();
        }
        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //    base.OnPaintBackground(e); // Draw base background elements (gradient, etc.)
        //}
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // Draw base elements (border, shadow, etc.)

            // Draw title text if enabled
            if (_showTitle && !string.IsNullOrEmpty(_titleText))
            {
                DrawTitle(e.Graphics);
            }
        }

  
        protected override void OnControlAdded(ControlEventArgs e)
    {
        base.OnControlAdded(e);
        // Adjust layout for any new controls if necessary
    }
      
        private void DrawTitle(Graphics graphics)
        {
            // Use BeepThemesManager to fetch the theme-based font, or fall back to default font
            Font fontToUse = BeepThemesManager.ToFont(_currentTheme?.TitleStyle) ?? Font;
            

            SizeF titleSize = graphics.MeasureString(_titleText, fontToUse);
            int padding = 0; // Adjusted padding for shadow and borders
            PointF titlePosition = new PointF(DrawingRect.Left + padding, DrawingRect.Top + padding);

            // Adjust title position based on alignment setting within DrawingRect
            switch (_titleAlignment)
            {
                case ContentAlignment.TopRight:
                    titlePosition = new PointF(DrawingRect.Right - titleSize.Width - padding, DrawingRect.Top + padding);
                    break;
                case ContentAlignment.TopLeft:
                    titlePosition = new PointF(DrawingRect.Left + padding, DrawingRect.Top + padding);
                    break;
                case ContentAlignment.TopCenter:
                    titlePosition = new PointF(DrawingRect.Left + (DrawingRect.Width - titleSize.Width) / 2, DrawingRect.Top + padding);
                    break;
            }

            using (var brush = new SolidBrush(_currentTheme.TitleForColor))
            {
                if(_showTitle && !string.IsNullOrEmpty(TitleText))
                {
                    graphics.DrawString(_titleText, fontToUse, brush, titlePosition);
                    
                    if (ShowTitleLine)
                    {
                        DrawTitleLine(graphics);
                    }
                        
                }
               
            }
        }
        public void CalculateTitleBottomY()
        {
            using (Graphics g = CreateGraphics())
            {
                // Use the current theme font or default font
                Font fontToUse = BeepThemesManager.ToFont(_currentTheme?.TitleStyle) ?? Font;
                SizeF titleSize = g.MeasureString(TitleText, fontToUse);
                // Calculate Y position relative to DrawingRect to align line below title text
                int lineY = DrawingRect.Top + BorderThickness + (int)titleSize.Height + 8; // Adjust based on title height and padding

                TitleBottomY = lineY + 5;
            }
        }

        private void DrawTitleLine(Graphics graphics)
        {
            // Use the current theme font or default font
            Font fontToUse = BeepThemesManager.ToFont(_currentTheme?.TitleStyle) ?? Font;
            SizeF titleSize = graphics.MeasureString(TitleText, fontToUse);
            // Calculate Y position relative to DrawingRect to align line below title text
            int lineY = DrawingRect.Top + BorderThickness + (int)titleSize.Height + 8; // Adjust based on title height and padding

            // Measure the width of the title text if ShowTitleLineinFullWidth is false
            int lineStartX = DrawingRect.Left + BorderThickness;
            int lineEndX = ShowTitleLineinFullWidth
                ? DrawingRect.Right - BorderThickness // Full width
                : lineStartX + (int)titleSize.Width + 10; // Title width + padding
           // TitleBottomY = lineY + 5;
            // Set line color and thickness, using the current theme or default if unavailable
            using (var pen = new Pen(_currentTheme?.TitleForColor ?? Color.Gray, TitleLineThickness))
            {
                // Draw line based on the selected width option
                graphics.DrawLine(pen, lineStartX, lineY, lineEndX, lineY);
            }
            // Update the TitleBottomY to position items below the line
            _titleBottomY = lineY + _titleLineThickness + 5; // Adjusted for the line's thickness and extra padding
        }
   
        //protected override void OnResize(EventArgs e)
        //{
        //    base.OnResize(e);
        //    Invalidate(); // Redraw on resize to adjust title positioning
        //}
    }
}
