using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers;
using TheTechIdea.Beep.Winform.Controls.Styling;
using Point = System.Drawing.Point;

namespace TheTechIdea.Beep.Winform.Controls.RadioGroup
{
    public partial class BeepRadioGroup
    {
        #region Layout and Rendering
        private void UpdateItemsAndLayout()
        {
            // Update helpers with new data
            _stateHelper.UpdateItems(_items);
            
            // Calculate layout
            UpdateLayout();
            
            // Update states
            UpdateItemStates();
            
            // Update hit testing
            _hitTestHelper.UpdateItems(_items, _itemRectangles);
            
            // Auto-size control if needed
            if (AutoSize)
            {
                var totalSize = _layoutHelper.CalculateTotalSize(_items, MaximumSize);
                Size = totalSize;
            }
            
            Invalidate();
        }

        private void UpdateLayout()
        {
            if (_items == null || _items.Count == 0)
            {
                _itemRectangles.Clear();
                return;
            }

            // Use DrawingRect instead of full Size for layout calculations
            var containerRect = DrawingRect;
            if (containerRect.IsEmpty)
                containerRect = new Rectangle(Point.Empty, Size);
            
            // Make container rect relative (start from 0,0 since we'll offset later)
            containerRect = new Rectangle(Point.Empty, containerRect.Size);
            
            _itemRectangles = _layoutHelper.CalculateItemRectangles(_items, containerRect);
            
            // Apply alignment
            _layoutHelper.ApplyAlignment(_itemRectangles, containerRect);
        }

        private void UpdateItemStates()
        {
            _itemStates.Clear();
            
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var state = new RadioItemState
                {
                    IsSelected = _stateHelper.IsSelected(item),
                    IsHovered = _hitTestHelper.HoveredIndex == i,
                    IsFocused = _hitTestHelper.FocusedIndex == i,
                    IsEnabled = true, // Could be per item later
                    Index = i
                };
                
                _itemStates.Add(state);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            // Ensure DrawingRect is updated
            UpdateDrawingRect();
            
            UpdateLayout();
            _hitTestHelper.UpdateItems(_items, _itemRectangles);
            Invalidate();
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            if (_currentRenderer == null || _items == null || _items.Count == 0)
                return;

            // Update states before drawing
            UpdateItemStates();
            if (UseThemeColors && _currentTheme != null)
            {
                BackColor = _currentTheme.SideMenuBackColor;
                g.Clear(BackColor);
            }
            else
            {
                // Paint background based on selected style
                BeepStyling.PaintStyleBackground(g, DrawingRect, Style);
            }
            // Use DrawingRect from BaseControl for proper bounds
            var drawingBounds = DrawingRect;
            if (drawingBounds.IsEmpty)
                drawingBounds = ClientRectangle;

            // Draw group decorations first
            _currentRenderer.RenderGroupDecorations(g, drawingBounds, _items, _itemRectangles, _itemStates);

            // Draw each item within the drawing bounds
            for (int i = 0; i < Math.Min(_items.Count, Math.Min(_itemRectangles.Count, _itemStates.Count)); i++)
            {
                // Offset item rectangles to be relative to DrawingRect
                var adjustedRect = new Rectangle(
                    _itemRectangles[i].X + drawingBounds.X,
                    _itemRectangles[i].Y + drawingBounds.Y,
                    _itemRectangles[i].Width,
                    _itemRectangles[i].Height
                );

                // Clip to drawing bounds
                if (adjustedRect.IntersectsWith(drawingBounds))
                {
                    _currentRenderer.RenderItem(g, _items[i], adjustedRect, _itemStates[i]);
                }
            }
        }
        #endregion
    }
}
