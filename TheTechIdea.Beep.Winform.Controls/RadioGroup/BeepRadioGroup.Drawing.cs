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
            // Skip heavy operations in design mode
            if (DesignMode || !IsHandleCreated) return;
            
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
                    IsEnabled = !IsItemDisabled(item.Text), // Per-item disabled state
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
            // Design-time placeholder rendering
            if (DesignMode)
            {
                PaintDesignTimePlaceholder(g);
                return;
            }
            
            base.DrawContent(g);
            if (_currentRenderer == null || _items == null || _items.Count == 0)
                return;

            // Update states before drawing
            UpdateItemStates();
            if (UseThemeColors && _currentTheme != null)
            {
                g.Clear(_currentTheme.SideMenuBackColor);
            }
            else
            {
                // Paint background based on selected Style
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
        
        /// <summary>
        /// Simple placeholder rendering for Visual Studio Designer
        /// </summary>
        private void PaintDesignTimePlaceholder(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(245, 245, 247)), ClientRectangle);
            using (var pen = new Pen(Color.FromArgb(209, 213, 219), 1))
            {
                g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }
            using (var font = new Font("Segoe UI", 10))
            using (var brush = new SolidBrush(Color.FromArgb(100, 100, 100)))
            {
                g.DrawString("BeepRadioGroup (Design Mode)", font, brush, 10, 10);
                g.DrawString($"Style: {RadioGroupStyle}", font, brush, 10, 30);
                g.DrawString($"Items: {_items?.Count ?? 0}", font, brush, 10, 50);
            }
        }
        #endregion
    }
}
