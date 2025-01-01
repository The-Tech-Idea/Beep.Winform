﻿using TheTechIdea.Beep.Vis.Modules;
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
        // We'll keep everything the same, only adjusting the logic in DrawTitle and DrawTitleLine
        const int startyoffset = 0;
        private string _titleText = "Panel Title";
        private bool _showTitle = true;
        private bool _showTitleLine = true;
        private bool _titleLineFullWidth = true;
        private Color _titleLineColor = Color.Gray;
        private int _titleLineThickness = 2;

   

        private int _titleBottomY = startyoffset;
        private ContentAlignment _titleAlignment = ContentAlignment.TopLeft;

        int padding = 2; // Adjusted padding for top, left, etc.

        #region "Scrolling"
        private bool _enableScrolling = false;
        private int _scrollOffset = 0;
        private int _scrollSpeed = 1;
        private Timer _scrollTimer;
        private int _scrollInterval = 10;
        private int _scrollDirection = 1;
        private VScrollBar _verticalScrollBar;
        private HScrollBar _horizontalScrollBar;
        #endregion

        #region "Public Properties"

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Title Bottom Location Y")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TitleBottomY
        {
            get => _titleBottomY;
            set
            {
                _titleBottomY = value;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The text displayed as the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string TitleText
        {
            get => _titleText;
            set
            {
                _titleText = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Show a line below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitleLine
        {
            get => _showTitleLine;
            set
            {
                _showTitleLine = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Color of the line below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TitleLineColor
        {
            get => _titleLineColor;
            set
            {
                _titleLineColor = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Thickness of the line below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TitleLineThickness
        {
            get => _titleLineThickness;
            set
            {
                _titleLineThickness = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Determines if the title is shown.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitle
        {
            get => _showTitle;
            set
            {
                _showTitle = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Alignment of the title text within the panel.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ContentAlignment TitleAlignment
        {
            get => _titleAlignment;
            set
            {
                _titleAlignment = value;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Draw the title line with full width or just below the title.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowTitleLineinFullWidth
        {
            get => _titleLineFullWidth;
            set
            {
                _titleLineFullWidth = value;
                Invalidate();
            }
        }

        #endregion

        #region "Constructor"
        public BeepPanel()
        {
            ApplyTheme();
            IsChild = false;
            this.Size = new Size(400, 300);
        }
        #endregion

        public override void ApplyTheme()
        {
            // We'll keep your logic, no changes
            BackColor = _currentTheme.PanelBackColor;
            ForeColor = _currentTheme.TitleForColor;
            Font = BeepThemesManager.ToFont(_currentTheme.TitleMedium);
            foreach (Control ctrl in Controls)
            {
                // if you want to apply theme to child controls, do so here
            }
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            // base draws the beepcontrol background/border if any
            base.OnPaint(e);

            // Reset TitleBottomY each time we paint. We'll recalc it if we draw a title.
            _titleBottomY = startyoffset;

            // If ShowTitle is true and TitleText is not empty, draw the title
            if (_showTitle && !string.IsNullOrEmpty(_titleText))
            {
                DrawTitle(e.Graphics);
            }
        }
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            // If you need to re-layout child controls, do so
        }
        private void DrawTitle(Graphics g)
        {
            // Use a title font from your theme, or fallback
         
            // Update DrawingRect before measuring or positioning
            UpdateDrawingRect();

            // measure how big the text is
            SizeF titleSize = g.MeasureString(_titleText, Font);

            // We'll define a "textTop" for vertical. It's typically DrawingRect.Top + some padding
            float textTop = DrawingRect.Top + padding;
            // We'll define textLeft based on TitleAlignment
            float textLeft = 0;

            switch (_titleAlignment)
            {
                case ContentAlignment.TopLeft:
                    textLeft = DrawingRect.Left + padding;
                    break;
                case ContentAlignment.TopCenter:
                    textLeft = DrawingRect.Left + (DrawingRect.Width - titleSize.Width) / 2f;
                    break;
                case ContentAlignment.TopRight:
                    textLeft = DrawingRect.Right - titleSize.Width - padding;
                    break;
                    // If you plan to support MiddleLeft, MiddleCenter, etc. you'd handle them similarly,
                    // but your code suggests you only do top alignments
            }

            // Draw the title text
            using (Brush brush = new SolidBrush(_currentTheme.TitleForColor))
            {
                g.DrawString(_titleText, Font, brush, textLeft, textTop);
            }

            // The bottom of the drawn text
            float textBottomY = textTop + titleSize.Height;

            if (_showTitleLine)
            {
                // We'll draw a line below the text
                // line Y is a bit below the text
                // e.g. lineY = (int)(textBottomY + 2) if you want a small gap
                int lineY = (int)(textBottomY + 2);

                // If ShowTitleLineinFullWidth => from DrawingRect.Left + _borderThickness
                // else from textLeft => textLeft + titleSize.Width
                int lineStartX = ShowTitleLineinFullWidth
                    ? (DrawingRect.Left + BorderThickness)
                    : (int)textLeft;
                int lineEndX = ShowTitleLineinFullWidth
                    ? (DrawingRect.Right - BorderThickness)
                    : (int)(textLeft + titleSize.Width);

                using (var pen = new Pen(_titleLineColor, _titleLineThickness))
                {
                    g.DrawLine(pen, lineStartX, lineY, lineEndX, lineY);
                }
                
                // TitleBottomY is the line's bottom => lineY + lineThickness + some extra
                _titleBottomY = lineY + _titleLineThickness + padding;
            }
            else
            {
                // no line => just set TitleBottomY to textBottom plus some padding
                _titleBottomY = (int)(textBottomY + padding);
            }
        }
    }
}
