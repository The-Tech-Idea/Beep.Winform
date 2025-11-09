using System;
using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Toggle.Helpers
{
    /// <summary>
    /// Layout calculation helper for BeepToggle
    /// Manages track and thumb positioning for different toggle styles
    /// </summary>
    internal class BeepToggleLayoutHelper
    {
        private readonly BeepToggle _owner;
        private Rectangle _trackRect;
        private Rectangle _thumbRect;
        private Rectangle _onLabelRect;
        private Rectangle _offLabelRect;
        private Rectangle _iconRect;

        public BeepToggleLayoutHelper(BeepToggle owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        #region Properties

        /// <summary>
        /// Track (background) rectangle
        /// </summary>
        public Rectangle TrackRect => _trackRect;

        /// <summary>
        /// Thumb (slider) rectangle
        /// </summary>
        public Rectangle ThumbRect => _thumbRect;

        /// <summary>
        /// ON label rectangle
        /// </summary>
        public Rectangle OnLabelRect => _onLabelRect;

        /// <summary>
        /// OFF label rectangle
        /// </summary>
        public Rectangle OffLabelRect => _offLabelRect;

        /// <summary>
        /// Icon rectangle
        /// </summary>
        public Rectangle IconRect => _iconRect;

        #endregion

        #region Layout Calculation

        /// <summary>
        /// Update layout based on drawing rectangle and toggle style
        /// </summary>
        public void UpdateLayout(Rectangle drawingRect)
        {
            if (drawingRect.Width <= 0 || drawingRect.Height <= 0)
                return;

            switch (_owner.ToggleStyle)
            {
                case ToggleStyle.Classic:
                case ToggleStyle.iOS:
                case ToggleStyle.MaterialPill:
                    CalculatePillLayout(drawingRect);
                    break;

                case ToggleStyle.LabeledTrack:
                    CalculateLabeledTrackLayout(drawingRect);
                    break;

                case ToggleStyle.IconThumb:
                    CalculateIconThumbLayout(drawingRect);
                    break;

                case ToggleStyle.RectangularLabeled:
                case ToggleStyle.MaterialSquare:
                    CalculateRectangularLayout(drawingRect);
                    break;

                case ToggleStyle.Minimal:
                case ToggleStyle.MaterialSlider:
                    CalculateMinimalLayout(drawingRect);
                    break;

                case ToggleStyle.ButtonStyle:
                    CalculateButtonStyleLayout(drawingRect);
                    break;

                case ToggleStyle.CheckboxStyle:
                case ToggleStyle.MaterialCheckbox:
                    CalculateCheckboxLayout(drawingRect);
                    break;

                case ToggleStyle.MaterialSquareButton:
                    CalculateSquareButtonLayout(drawingRect);
                    break;

                default:
                    CalculatePillLayout(drawingRect);
                    break;
            }
        }

        private void CalculatePillLayout(Rectangle rect)
        {
            // Track is a pill shape (rounded rectangle)
            int trackHeight = Math.Min(rect.Height, rect.Height * 2 / 3);
            int trackWidth = Math.Max(rect.Width, trackHeight * 2);
            _trackRect = new Rectangle(
                rect.X,
                rect.Y + (rect.Height - trackHeight) / 2,
                trackWidth,
                trackHeight
            );

            // Thumb is a circle
            int thumbSize = (int)(trackHeight * 0.85f);
            int padding = (trackHeight - thumbSize) / 2;
            
            // Calculate thumb position based on animation
            int thumbX = (int)(_trackRect.X + padding + 
                (_trackRect.Width - thumbSize - padding * 2) * _owner.ThumbPosition);
            
            _thumbRect = new Rectangle(
                thumbX,
                _trackRect.Y + padding,
                thumbSize,
                thumbSize
            );

            // Labels (if needed)
            _onLabelRect = Rectangle.Empty;
            _offLabelRect = Rectangle.Empty;
        }

        private void CalculateLabeledTrackLayout(Rectangle rect)
        {
            // Similar to pill but with text inside track
            CalculatePillLayout(rect);

            if (_owner.ShowLabels)
            {
                int labelWidth = _trackRect.Width / 2 - 4;
                int labelHeight = _trackRect.Height - 4;
                
                _offLabelRect = new Rectangle(
                    _trackRect.X + 4,
                    _trackRect.Y + 2,
                    labelWidth,
                    labelHeight
                );

                _onLabelRect = new Rectangle(
                    _trackRect.X + _trackRect.Width / 2,
                    _trackRect.Y + 2,
                    labelWidth,
                    labelHeight
                );
            }
        }

        private void CalculateIconThumbLayout(Rectangle rect)
        {
            // Pill track
            CalculatePillLayout(rect);

            // Icon in thumb
            int iconSize = (int)(_thumbRect.Width * 0.6f);
            _iconRect = new Rectangle(
                _thumbRect.X + (_thumbRect.Width - iconSize) / 2,
                _thumbRect.Y + (_thumbRect.Height - iconSize) / 2,
                iconSize,
                iconSize
            );
        }

        private void CalculateRectangularLayout(Rectangle rect)
        {
            // Rectangular track
            int trackHeight = rect.Height - 4;
            _trackRect = new Rectangle(
                rect.X + 2,
                rect.Y + 2,
                rect.Width - 4,
                trackHeight
            );

            // Rectangular thumb with rounded corners
            int thumbWidth = _trackRect.Width / 2 - 4;
            int thumbHeight = trackHeight - 4;
            
            int thumbX = (int)(_trackRect.X + 2 + 
                (_trackRect.Width - thumbWidth - 4) * _owner.ThumbPosition);
            
            _thumbRect = new Rectangle(
                thumbX,
                _trackRect.Y + 2,
                thumbWidth,
                thumbHeight
            );

            // Labels on thumb or track
            if (_owner.ShowLabels)
            {
                _offLabelRect = new Rectangle(
                    _trackRect.X + 4,
                    _trackRect.Y,
                    thumbWidth,
                    trackHeight
                );

                _onLabelRect = new Rectangle(
                    _trackRect.Right - thumbWidth - 4,
                    _trackRect.Y,
                    thumbWidth,
                    trackHeight
                );
            }
        }

        private void CalculateMinimalLayout(Rectangle rect)
        {
            // Flat track
            int trackHeight = rect.Height / 3;
            _trackRect = new Rectangle(
                rect.X,
                rect.Y + (rect.Height - trackHeight) / 2,
                rect.Width,
                trackHeight
            );

            // Circle thumb
            int thumbSize = rect.Height - 4;
            int thumbX = (int)(rect.X + 
                (rect.Width - thumbSize) * _owner.ThumbPosition);
            
            _thumbRect = new Rectangle(
                thumbX,
                rect.Y + 2,
                thumbSize,
                thumbSize
            );
        }

        private void CalculateButtonStyleLayout(Rectangle rect)
        {
            // Two button areas
            int buttonWidth = rect.Width / 2;
            
            _offLabelRect = new Rectangle(
                rect.X,
                rect.Y,
                buttonWidth - 2,
                rect.Height
            );

            _onLabelRect = new Rectangle(
                rect.X + buttonWidth + 2,
                rect.Y,
                buttonWidth - 2,
                rect.Height
            );

            // Track indicates selection
            _trackRect = _owner.IsOn ? _onLabelRect : _offLabelRect;
            _thumbRect = Rectangle.Empty;
        }

        private void CalculateCheckboxLayout(Rectangle rect)
        {
            // Square checkbox
            int size = Math.Min(rect.Width, rect.Height);
            _trackRect = new Rectangle(
                rect.X,
                rect.Y + (rect.Height - size) / 2,
                size,
                size
            );

            // Icon inside
            int iconSize = (int)(size * 0.6f);
            _iconRect = new Rectangle(
                _trackRect.X + (size - iconSize) / 2,
                _trackRect.Y + (size - iconSize) / 2,
                iconSize,
                iconSize
            );

            _thumbRect = Rectangle.Empty;
        }

        private void CalculateSquareButtonLayout(Rectangle rect)
        {
            // Full rectangle as button
            _trackRect = new Rectangle(
                rect.X + 2,
                rect.Y + 2,
                rect.Width - 4,
                rect.Height - 4
            );

            // Icon in center
            int iconSize = Math.Min(_trackRect.Width, _trackRect.Height) / 2;
            _iconRect = new Rectangle(
                _trackRect.X + (_trackRect.Width - iconSize) / 2,
                _trackRect.Y + (_trackRect.Height - iconSize) / 2,
                iconSize,
                iconSize
            );

            _thumbRect = Rectangle.Empty;
        }

        #endregion
    }
}
