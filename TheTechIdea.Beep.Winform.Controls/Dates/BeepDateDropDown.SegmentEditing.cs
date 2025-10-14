using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace TheTechIdea.Beep.Winform.Controls.Dates
{
    /// <summary>
    /// Handles segment-based editing for date/time components
    /// Allows clicking on day, month, year, hour, minute individually
    /// </summary>
    public partial class BeepDateDropDown
    {
        #region "Segment Fields"
        
        private List<DateSegment> _segments = new List<DateSegment>();
        private DateSegment _hoveredSegment = null;
        private DateSegment _activeSegment = null;
        private bool _enableSegmentEditing = true;
        
        #endregion
        
        #region "Segment Properties"
        
        /// <summary>
        /// Enable/disable segment-based editing (click on day, month, year individually)
        /// </summary>
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Allow clicking on individual date segments (day, month, year) for targeted editing.")]
        public bool EnableSegmentEditing
        {
            get => _enableSegmentEditing;
            set
            {
                _enableSegmentEditing = value;
                if (!value)
                {
                    _hoveredSegment = null;
                    _activeSegment = null;
                    _segments.Clear();
                }
                Invalidate();
            }
        }
        
        #endregion
        
        #region "Segment Classes"
        
        /// <summary>
        /// Represents a single editable segment of a date/time
        /// </summary>
        internal class DateSegment
        {
            public DateSegmentType Type { get; set; }
            public Rectangle Bounds { get; set; }
            public string Text { get; set; }
            public int StartPosition { get; set; }
            public int Length { get; set; }
            public bool IsStartDate { get; set; }  // For range mode
            
            public bool Contains(Point point) => Bounds.Contains(point);
        }
        
        /// <summary>
        /// Types of date/time segments
        /// </summary>
        internal enum DateSegmentType
        {
            Day,
            Month,
            Year,
            Hour,
            Minute,
            Second,
            AmPm,
            Separator
        }
        
        #endregion
        
        #region "Segment Calculation"
        
        /// <summary>
        /// Calculate segment boundaries based on current text and format
        /// </summary>
        private void CalculateSegments()
        {
            _segments.Clear();
            
            if (!_enableSegmentEditing || string.IsNullOrEmpty(Text))
                return;
            
            string format = GetCurrentFormat();
            string text = Text;
            
            // For range mode, split by separator
            if (_mode == Models.DatePickerMode.Range || _mode == Models.DatePickerMode.RangeWithTime)
            {
                string[] parts = text.Split(new[] { _dateSeparator }, StringSplitOptions.None);
                
                if (parts.Length >= 1)
                {
                    CalculateSegmentsForDateText(parts[0].Trim(), format, 0, true);
                }
                
                if (parts.Length >= 2)
                {
                    int separatorPos = parts[0].Length;
                    int secondDateStart = separatorPos + _dateSeparator.Length;
                    CalculateSegmentsForDateText(parts[1].Trim(), format, secondDateStart, false);
                }
            }
            else
            {
                CalculateSegmentsForDateText(text, format, 0, true);
            }
        }
        
        /// <summary>
        /// Calculate segments for a single date text
        /// </summary>
        private void CalculateSegmentsForDateText(string dateText, string format, int basePosition, bool isStartDate)
        {
            if (string.IsNullOrEmpty(dateText))
                return;
            
            // Parse format to identify segments
            // Common formats: MM/dd/yyyy, dd-MM-yyyy, yyyy.MM.dd HH:mm:ss
            
            int position = basePosition;
            int textIndex = 0;
            
            // Simple parser for common date formats
            for (int i = 0; i < format.Length && textIndex < dateText.Length; i++)
            {
                char formatChar = format[i];
                
                if (char.IsLetter(formatChar))
                {
                    // Start of a segment
                    string segmentPattern = new string(formatChar, 1);
                    int patternStart = i;
                    
                    // Extend pattern while same character
                    while (i + 1 < format.Length && format[i + 1] == formatChar)
                    {
                        segmentPattern += formatChar;
                        i++;
                    }
                    
                    // Determine segment type
                    DateSegmentType segmentType = GetSegmentType(formatChar);
                    
                    // Get the actual text for this segment
                    int segmentLength = segmentPattern.Length;
                    if (textIndex + segmentLength <= dateText.Length)
                    {
                        string segmentText = dateText.Substring(textIndex, segmentLength);
                        
                        // Calculate bounds (will be refined in OnPaint)
                        var segment = new DateSegment
                        {
                            Type = segmentType,
                            Text = segmentText,
                            StartPosition = position + textIndex,
                            Length = segmentLength,
                            IsStartDate = isStartDate
                        };
                        
                        _segments.Add(segment);
                        textIndex += segmentLength;
                    }
                }
                else
                {
                    // Separator character
                    if (textIndex < dateText.Length && dateText[textIndex] == formatChar)
                    {
                        textIndex++;
                    }
                }
            }
        }
        
        private DateSegmentType GetSegmentType(char formatChar)
        {
            switch (char.ToUpper(formatChar))
            {
                case 'D': return DateSegmentType.Day;
                case 'M': return DateSegmentType.Month;
                case 'Y': return DateSegmentType.Year;
                case 'H': return DateSegmentType.Hour;
                case 'N': // Minute (some formats use 'n')
                case 'S': return DateSegmentType.Minute;
                case 'T': return DateSegmentType.AmPm;
                default: return DateSegmentType.Separator;
            }
        }
        
        private string GetCurrentFormat()
        {
            switch (_mode)
            {
                case Models.DatePickerMode.Single:
                    return DateFormat;
                    
                case Models.DatePickerMode.SingleWithTime:
                    return DateTimeFormat;
                    
                case Models.DatePickerMode.Range:
                    return DateFormat;
                    
                case Models.DatePickerMode.RangeWithTime:
                    return DateTimeFormat;
                    
                default:
                    return "MM/dd/yyyy";
            }
        }
        
        #endregion
        
        #region "Segment Rendering"
        
        /// <summary>
        /// Update segment bounds based on actual text rendering
        /// </summary>
        private void UpdateSegmentBounds(Graphics g)
        {
            if (!_enableSegmentEditing || _segments.Count == 0)
                return;
            
            Rectangle textRect = GetTextRectangle();
            Font font = TextFont ?? Font;
            
            // Measure each segment
            int currentX = textRect.X;
            
            foreach (var segment in _segments)
            {
                // Get text up to this segment
                string textBefore = Text.Substring(0, segment.StartPosition);
                Size beforeSize = TextRenderer.MeasureText(g, textBefore, font);
                
                // Measure segment text
                Size segmentSize = TextRenderer.MeasureText(g, segment.Text, font);
                
                segment.Bounds = new Rectangle(
                    textRect.X + beforeSize.Width,
                    textRect.Y,
                    segmentSize.Width,
                    textRect.Height
                );
            }
        }
        
        /// <summary>
        /// Paint segment highlights
        /// </summary>
        private void PaintSegmentHighlights(Graphics g)
        {
            if (!_enableSegmentEditing)
                return;
            
            // Update bounds first
            UpdateSegmentBounds(g);
            
            // Highlight hovered segment
            if (_hoveredSegment != null)
            {
                PaintSegmentHighlight(g, _hoveredSegment, false);
            }
            
            // Highlight active segment
            if (_activeSegment != null && _activeSegment != _hoveredSegment)
            {
                PaintSegmentHighlight(g, _activeSegment, true);
            }
        }
        
        private void PaintSegmentHighlight(Graphics g, DateSegment segment, bool isActive)
        {
            if (segment == null || segment.Bounds.IsEmpty)
                return;
            
            // Create highlight rectangle with padding
            Rectangle highlightRect = segment.Bounds;
            highlightRect.Inflate(2, 2);
            
            // Choose color based on state
            Color highlightColor = isActive 
                ? Color.FromArgb(80, _currentTheme?.AccentColor ?? Color.DodgerBlue)
                : Color.FromArgb(40, _currentTheme?.AccentColor ?? Color.LightGray);
            
            using (SolidBrush brush = new SolidBrush(highlightColor))
            {
                g.FillRectangle(brush, highlightRect);
            }
            
            // Draw subtle border
            Color borderColor = isActive
                ? _currentTheme?.AccentColor ?? Color.DodgerBlue
                : Color.FromArgb(100, ForeColor);
            
            using (Pen pen = new Pen(borderColor, 1))
            {
                g.DrawRectangle(pen, highlightRect);
            }
        }
        
        #endregion
        
        #region "Segment Interaction"
        
        /// <summary>
        /// Handle mouse move for segment hover
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            if (!_enableSegmentEditing || _segments.Count == 0)
                return;
            
            // Find hovered segment
            DateSegment newHovered = _segments.FirstOrDefault(s => s.Contains(e.Location));
            
            if (newHovered != _hoveredSegment)
            {
                _hoveredSegment = newHovered;
                
                // Update cursor
                Cursor = _hoveredSegment != null ? Cursors.IBeam : Cursors.Default;
                
                // Show tooltip with segment type
                if (_hoveredSegment != null)
                {
                    string tooltip = GetSegmentTooltip(_hoveredSegment);
                    ToolTipText = tooltip;
                }
                else
                {
                    ToolTipText = string.Empty;
                }
                
                Invalidate();
            }
        }
        
        /// <summary>
        /// Handle mouse click on segment
        /// </summary>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            
            if (!_enableSegmentEditing || _segments.Count == 0)
                return;
            
            // Find clicked segment
            DateSegment clickedSegment = _segments.FirstOrDefault(s => s.Contains(e.Location));
            
            if (clickedSegment != null)
            {
                // Set as active segment
                _activeSegment = clickedSegment;
                
                // Select the segment text
                SelectionStart = clickedSegment.StartPosition;
                SelectionLength = clickedSegment.Length;
                
                Invalidate();
            }
        }
        
        /// <summary>
        /// Handle keyboard input for segment editing
        /// </summary>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (_enableSegmentEditing && _activeSegment != null)
            {
                // Handle segment-specific input
                if (HandleSegmentKeyPress(_activeSegment, e.KeyChar))
                {
                    e.Handled = true;
                    return;
                }
            }
            
            base.OnKeyPress(e);
        }
        
        private bool HandleSegmentKeyPress(DateSegment segment, char keyChar)
        {
            // Allow only valid input for segment type
            switch (segment.Type)
            {
                case DateSegmentType.Day:
                case DateSegmentType.Month:
                case DateSegmentType.Year:
                case DateSegmentType.Hour:
                case DateSegmentType.Minute:
                case DateSegmentType.Second:
                    // Only allow digits
                    return !char.IsDigit(keyChar) && !char.IsControl(keyChar);
                    
                case DateSegmentType.AmPm:
                    // Only allow A, P, M
                    char upper = char.ToUpper(keyChar);
                    return upper != 'A' && upper != 'P' && upper != 'M' && !char.IsControl(keyChar);
                    
                default:
                    return false;
            }
        }
        
        private string GetSegmentTooltip(DateSegment segment)
        {
            string prefix = segment.IsStartDate ? "Start " : "End ";
            
            switch (segment.Type)
            {
                case DateSegmentType.Day:
                    return $"{prefix}Day (DD)";
                case DateSegmentType.Month:
                    return $"{prefix}Month (MM)";
                case DateSegmentType.Year:
                    return $"{prefix}Year (YYYY)";
                case DateSegmentType.Hour:
                    return $"{prefix}Hour (HH)";
                case DateSegmentType.Minute:
                    return $"{prefix}Minute (mm)";
                case DateSegmentType.Second:
                    return $"{prefix}Second (ss)";
                case DateSegmentType.AmPm:
                    return "AM/PM";
                default:
                    return string.Empty;
            }
        }
        
        #endregion
        
        #region "Segment Navigation"
        
        /// <summary>
        /// Move to next segment (Tab key)
        /// </summary>
        private void MoveToNextSegment()
        {
            if (_segments.Count == 0)
                return;
            
            int currentIndex = _activeSegment != null 
                ? _segments.IndexOf(_activeSegment) 
                : -1;
            
            int nextIndex = (currentIndex + 1) % _segments.Count;
            _activeSegment = _segments[nextIndex];
            
            SelectionStart = _activeSegment.StartPosition;
            SelectionLength = _activeSegment.Length;
            
            Invalidate();
        }
        
        /// <summary>
        /// Move to previous segment (Shift+Tab)
        /// </summary>
        private void MoveToPreviousSegment()
        {
            if (_segments.Count == 0)
                return;
            
            int currentIndex = _activeSegment != null 
                ? _segments.IndexOf(_activeSegment) 
                : 0;
            
            int prevIndex = (currentIndex - 1 + _segments.Count) % _segments.Count;
            _activeSegment = _segments[prevIndex];
            
            SelectionStart = _activeSegment.StartPosition;
            SelectionLength = _activeSegment.Length;
            
            Invalidate();
        }
        
        /// <summary>
        /// Override Tab key handling for segment navigation
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (_enableSegmentEditing && _segments.Count > 0)
            {
                if (keyData == Keys.Tab)
                {
                    MoveToNextSegment();
                    return true;
                }
                else if (keyData == (Keys.Tab | Keys.Shift))
                {
                    MoveToPreviousSegment();
                    return true;
                }
            }
            
            return base.ProcessDialogKey(keyData);
        }
        
        #endregion
        
        #region "Helper Methods"
        
        /// <summary>
        /// Get the text rectangle (content area without borders and image)
        /// </summary>
        private Rectangle GetTextRectangle()
        {
            Rectangle bounds = ClientRectangle;
            
            // Account for borders
            int borderOffset = BorderWidth;
            bounds.Inflate(-borderOffset, -borderOffset);
            
            // Account for padding
            bounds.X += Padding.Left;
            bounds.Y += Padding.Top;
            bounds.Width -= Padding.Horizontal;
            bounds.Height -= Padding.Vertical;
            
            // Account for image if visible
            if (ImageVisible)
            {
                bounds.Width -= (MaxImageSize.Width + ImageMargin.Horizontal);
            }
            
            return bounds;
        }
        
        #endregion
    }
}
