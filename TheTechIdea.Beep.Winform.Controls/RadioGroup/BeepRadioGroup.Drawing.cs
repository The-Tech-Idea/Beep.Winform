using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Helpers;
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
            _layoutDirty = true;

            // Prevent stale hover/focus state after item source changes.
            _hitTestHelper.ResetInteractionState();

            if (_disabledItems.Count > 0)
            {
                var existing = new HashSet<string>(_items.Where(i => !string.IsNullOrEmpty(i?.Text)).Select(i => i.Text));
                _disabledItems.RemoveWhere(value => !existing.Contains(value));
            }
            
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
            
            RequestVisualRefresh();
        }

        private void UpdateLayout()
        {
            if (!_layoutDirty)
            {
                return;
            }

            if (_items == null || _items.Count == 0)
            {
                _itemRectangles.Clear();
                _visibleIndices.Clear();
                _layoutDirty = false;
                UpdateScrollBar();
                return;
            }

            // Use DrawingRect instead of full Size for layout calculations
            var containerRect = DrawingRect;
            if (containerRect.IsEmpty)
                containerRect = new Rectangle(Point.Empty, Size);

            // Make container rect relative (start from 0,0 since we'll offset later)
            containerRect = new Rectangle(Point.Empty, containerRect.Size);

            // Always measure ALL items so hit testing + state work on the full list.
            _itemRectangles = _layoutHelper.CalculateItemRectangles(_items, containerRect);
            _layoutHelper.ApplyAlignment(_itemRectangles, containerRect);

            // Then compute which subset is visible in the current scroll window.
            // Hit test uses the full _itemRectangles; DrawContent uses _visibleIndices.
            if (IsVirtualized)
            {
                UpdateScrollBar();
                _scrollOffset = Math.Min(_scrollOffset, Math.Max(0, _items.Count - 1));
                _visibleIndices.Clear();
                int rightInset = _vScroll?.Visible == true ? _vScroll.Width : 0;
                // Window in absolute container coords: items whose rect falls inside
                // [_scrollOffset, _scrollOffset + containerRect.Height].
                int visibleTop = _scrollOffset;
                int visibleBottom = _scrollOffset + containerRect.Height;
                for (int i = 0; i < _items.Count; i++)
                {
                    var r = _itemRectangles[i];
                    if (r.Bottom >= visibleTop && r.Top <= visibleBottom)
                    {
                        if (r.Right > containerRect.Width - rightInset)
                        {
                            _itemRectangles[i] = new Rectangle(r.X, r.Y,
                                Math.Max(0, containerRect.Width - rightInset - r.X),
                                r.Height);
                        }
                        _visibleIndices.Add(i);
                    }
                }
            }
            else
            {
                _visibleIndices.Clear();
                for (int i = 0; i < _items.Count; i++) _visibleIndices.Add(i);
            }
            _layoutDirty = false;
        }

        private void UpdateScrollBar()
        {
            if (_vScroll == null) return;
            if (!IsVirtualized)
            {
                _vScroll.Visible = false;
                _vScroll.Value = 0;
                return;
            }
            _vScroll.Visible = true;
            int totalContentHeight = _layoutHelper.CalculateTotalSize(_items, new Size(Width - _vScroll.Width, 0)).Height;
            int visibleHeight = Height;
            int max = Math.Max(0, totalContentHeight - visibleHeight);
            int oldMax = _vScroll.Maximum;
            _vScroll.Minimum = 0;
            _vScroll.Maximum = max;
            _vScroll.LargeChange = Math.Max(1, visibleHeight);
            if (oldMax == 0) _vScroll.Value = 0;
        }

        private void UpdateItemStates(bool notifyAccessibility = true)
        {
            _itemStates.Clear();

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                bool isHeader = item.IsHeader();
                var state = new RadioItemState
                {
                    IsSelected = !isHeader && _stateHelper.IsSelected(item),
                    IsHovered = !isHeader && (_hitTestHelper.HoveredIndex == i || _hitTestHelper.PressedIndex == i),
                    IsFocused = !isHeader && _hitTestHelper.FocusedIndex == i,
                    IsPressed = !isHeader && _hitTestHelper.PressedIndex == i,
                    IsEnabled = !isHeader && !IsItemDisabled(item.Text),
                    IsError = _hasValidationError,
                    AnimationProgress = GetAnimationProgress(i, state: _stateHelper.IsSelected(item)),
                    Index = i
                };

                _itemStates.Add(state);
            }

            if (notifyAccessibility)
            {
                UpdateAccessibilityMetadata();
            }
        }

        private float GetAnimationProgress(int index, bool state)
        {
            if (_animationProgress == null) return 1f;
            return _animationProgress.TryGetValue(index, out var p) ? p : 1f;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            // Ensure DrawingRect is updated
            UpdateDrawingRect();
            _layoutDirty = true;
            UpdateLayout();
            _hitTestHelper.UpdateItems(_items, _itemRectangles);
            RequestVisualRefresh();
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
            _suppressAccessibilityNotifications = true;
            UpdateItemStates(notifyAccessibility: false);
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

            // Iterate either the full list (non-virtualized) or the visible window.
            // When virtualized, translate the entire render by -_scrollOffset on Y so
            // only the visible window appears in the drawing bounds.
            // _visibleIndices is always populated in UpdateLayout — either with all
            // indices (non-virtualized) or the visible window (virtualized).
            var indices = _visibleIndices;
            System.Drawing.Drawing2D.Matrix? oldTransform = null;
            if (IsVirtualized && _scrollOffset != 0)
            {
                oldTransform = g.Transform;
                g.TranslateTransform(0, -_scrollOffset);
            }

            for (int k = 0; k < indices.Count; k++)
            {
                int i = indices[k];
                if (i < 0 || i >= _items.Count || i >= _itemRectangles.Count || i >= _itemStates.Count)
                    continue;

                // Offset item rectangles to be relative to DrawingRect
                var adjustedRect = new Rectangle(
                    _itemRectangles[i].X + drawingBounds.X,
                    _itemRectangles[i].Y + drawingBounds.Y,
                    _itemRectangles[i].Width,
                    _itemRectangles[i].Height
                );

                // In virtualized mode, _visibleIndices already filters; the g.TranslateTransform
                // above shifts the entire draw by -_scrollOffset so the visible window falls
                // inside drawingBounds. In non-virtualized mode, every item is in indices.
                // Either way, just draw.
                // Header items are drawn as non-interactive section labels
                if (_items[i].IsHeader())
                {
                    _currentRenderer.DrawHeader(g, _items[i], adjustedRect, _itemStates[i]);
                }
                else
                {
                    // Search filter: dim non-matching items
                    if (!MatchesSearch(_items[i]))
                    {
                        DrawDimmedItem(g, _items[i], adjustedRect, _itemStates[i]);
                    }
                    else
                    {
                        _currentRenderer.RenderItem(g, _items[i], adjustedRect, _itemStates[i]);
                    }

                    // Pass 2: focus ring (only for selectable items that are focused)
                    if (_itemStates[i].IsFocused && _currentRenderer is IFocusAwareRenderer focusRenderer)
                    {
                        focusRenderer.DrawFocusRing(g, adjustedRect, _itemStates[i]);
                    }
                }
            }

            if (oldTransform != null)
            {
                g.Transform = oldTransform;
            }
            _suppressAccessibilityNotifications = false;
        }

        /// <summary>
        /// Draws the item at 35% alpha when it does not match the current <see cref="SearchText"/>.
        /// The renderer is still called so the visual is preserved, but the result is dimmed.
        /// Uses a cached <see cref="System.Drawing.Imaging.ImageAttributes"/> to avoid per-paint
        /// allocation when the user is actively typing into the search box.
        /// </summary>
        private void DrawDimmedItem(Graphics g, SimpleItem item, Rectangle rect, RadioItemState state)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // Render to an off-screen bitmap at 35% alpha so the item stays in place
            // but reads as visually filtered out.
            using var bmp = new Bitmap(rect.Width, rect.Height, g);
            using (var gBmp = Graphics.FromImage(bmp))
            {
                gBmp.SmoothingMode = g.SmoothingMode;
                gBmp.TranslateTransform(-rect.X, -rect.Y);
                _currentRenderer!.RenderItem(gBmp, item, rect, state);
            }
            _dimmedImageAttributes ??= CreateDimmedImageAttributes();
            g.DrawImage(bmp,
                new Rectangle(rect.X, rect.Y, rect.Width, rect.Height),
                0, 0, rect.Width, rect.Height,
                GraphicsUnit.Pixel,
                _dimmedImageAttributes);
        }

        private static System.Drawing.Imaging.ImageAttributes CreateDimmedImageAttributes()
        {
            var attrs = new System.Drawing.Imaging.ImageAttributes();
            var matrix = new System.Drawing.Imaging.ColorMatrix { Matrix33 = 0.35f };
            attrs.SetColorMatrix(matrix);
            return attrs;
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
            using (var font = RadioGroupFontHelpers.GetItemFont(Style, false, _currentTheme))
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
