using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Base
{
    public partial class BaseControl
    {
        #region Location Change Handling
        protected override void OnLocationChanged(EventArgs e)
        {
            if (StaticNotMoving)
            {
                Location = _originalLocation;
                return;
            }
            base.OnLocationChanged(e);
            // Ensure parent redraws badge area when we move
            UpdateRegionForBadge();
        }
        #endregion

        #region Paint Pipeline
        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);

            // Keep painter layout in sync with padding changes
            EnsurePainter();
            _painter?.UpdateLayout(this);
            Invalidate();
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            // Early out for safety during design-time removal/dispose
            if (IsDisposed || !IsHandleCreated)
                return;

            // Add comprehensive validation
            if (e == null || e.Graphics == null || !IsReadyForDrawing)
            {
                base.OnPaint(e);
                return;
            }

            // Always ensure clip rectangle is valid
            var clipRect = EnsureValidRectangle(e.ClipRectangle);

            // Respect IsChild same as base BeepControl
            try
            {
                if (IsChild)
                {
                    ParentBackColor = Parent?.BackColor ?? SystemColors.Control;
                    BackColor = ParentBackColor;
                }
                else
                {
                    BackColor = _currentTheme?.BackColor ?? SystemColors.Control;
                }

                e.Graphics.Clear(BackColor);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                // Silently fail on color operations
                System.Diagnostics.Debug.WriteLine($"BaseControl.OnPaint color error: {ex.Message}");
            }

            if (UseExternalBufferedGraphics)
            {
                try
                {
                    BufferedGraphicsContext context = BufferedGraphicsManager.Current;
                    if (context != null)
                    {
                        using (BufferedGraphics buffer = context.Allocate(e.Graphics, EnsureValidRectangle(this.ClientRectangle)))
                        {
                            Graphics g = buffer.Graphics;

                            // Paint the inner area using the new PaintInnerShape method
                            SafePaintInnerShape(g);

                            // External drawing hooks BEFORE content
                            SafeExternalDrawing(g, DrawingLayer.BeforeContent);

                            // Main content and painter
                            DrawContent(g);

                            // After-all external drawings
                            SafeExternalDrawing(g, DrawingLayer.AfterAll);

                            // Effects
                            SafeDrawEffects(g);

                            buffer.Render(e.Graphics);
                        }
                    }
                }
                catch (Exception ex) when (ex is ArgumentException || ex is OutOfMemoryException)
                {
                    // Fall back to direct drawing if buffering fails
                    System.Diagnostics.Debug.WriteLine($"BaseControl.OnPaint buffered error: {ex.Message}");
                    SafeDraw(e.Graphics);
                }
            }
            else
            {
                SafeDraw(e.Graphics);
            }
        }

        // Safe drawing without buffering
        private void SafeDraw(Graphics g)
        {
            try
            {
                // Paint the inner area using the new PaintInnerShape method
                SafePaintInnerShape(g);

                // External drawing hooks BEFORE content
                SafeExternalDrawing(g, DrawingLayer.BeforeContent);

                // Main content and painter
                DrawContent(g);

                // After-all external drawings
                SafeExternalDrawing(g, DrawingLayer.AfterAll);

                // Effects
                SafeDrawEffects(g);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                System.Diagnostics.Debug.WriteLine($"BaseControl.SafeDraw error: {ex.Message}");
            }
        }

        private void SafePaintInnerShape(Graphics g)
        {
            try
            {
                PaintInnerShape(g, BackColor);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                System.Diagnostics.Debug.WriteLine($"BaseControl.SafePaintInnerShape error: {ex.Message}");
            }
        }

        private void SafeExternalDrawing(Graphics g, DrawingLayer layer)
        {
            try
            {
                _externalDrawing?.PerformExternalDrawing(g, layer);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                System.Diagnostics.Debug.WriteLine($"BaseControl.SafeExternalDrawing error: {ex.Message}");
            }
        }

        private void SafeDrawEffects(Graphics g)
        {
            try
            {
                _effects?.DrawOverlays(g);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                System.Diagnostics.Debug.WriteLine($"BaseControl.SafeDrawEffects error: {ex.Message}");
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Avoid updating regions/layout if dimensions aren't positive
            if (Width <= 0 || Height <= 0) return;

            try
            {
                UpdateDrawingRect();
                UpdateControlRegion();
                UpdateRegionForBadge();
                Invalidate();
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                System.Diagnostics.Debug.WriteLine($"BaseControl.OnResize error: {ex.Message}");
            }
        }

        private void UpdateControlRegion()
        {
            if (Width <= 0 || Height <= 0) return;

            try
            {
                Region controlRegion;
                var regionRect = new Rectangle(0, 0, Width, Height);

                if (IsRounded && BorderRadius > 0)
                {
                    using (var path = ControlPaintHelper.GetRoundedRectPath(regionRect, BorderRadius))
                    {
                        controlRegion = new Region(path);
                    }
                }
                else
                {
                    controlRegion = new Region(regionRect);
                }

                // Include badge area if present
                if (!string.IsNullOrEmpty(BadgeText))
                {
                    const int badgeSize = 22;
                    int badgeX = Width - badgeSize / 2;
                    int badgeY = -badgeSize / 2;
                    var badgeRect = new Rectangle(badgeX, badgeY, badgeSize, badgeSize);

                    using (var badgePath = new GraphicsPath())
                    {
                        switch (BadgeShape)
                        {
                            case BadgeShape.Circle:
                                badgePath.AddEllipse(badgeRect);
                                break;
                            case BadgeShape.Rectangle:
                                badgePath.AddRectangle(badgeRect);
                                break;
                            case BadgeShape.RoundedRectangle:
                                badgePath.AddPath(ControlPaintHelper.GetRoundedRectPath(badgeRect, badgeRect.Height / 4), false);
                                break;
                        }
                        controlRegion.Union(badgePath);
                    }
                }

                Region = controlRegion;
            }
            catch (Exception ex) when (ex is ArgumentException || ex is OutOfMemoryException)
            {
                System.Diagnostics.Debug.WriteLine($"BaseControl.UpdateControlRegion error: {ex.Message}");
                // Fallback to rectangular region
                try { Region = new Region(new Rectangle(0, 0, Width, Height)); }
                catch { /* last resort - leave region as-is */ }
            }
        }
        #endregion

        #region Mouse and Input Event Routing
        protected override void OnMouseEnter(EventArgs e) 
        { 
            base.OnMouseEnter(e); 
            IsHovered = true; 
            _input.OnMouseEnter(); 
        }
        
        protected override void OnMouseLeave(EventArgs e) 
        { 
            base.OnMouseLeave(e); 
            IsHovered = false; 
            _input.OnMouseLeave(); 
        }
        
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e) 
        { 
            base.OnMouseMove(e); 
            _input.OnMouseMove(e.Location); 
        }
        
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) 
        { 
            base.OnMouseDown(e); 
            if (e.Button == System.Windows.Forms.MouseButtons.Left) IsPressed = true; 
            _input.OnMouseDown(e); 
        }
        
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) 
        { 
            base.OnMouseUp(e); 
            if (e.Button == System.Windows.Forms.MouseButtons.Left) IsPressed = false; 
            _input.OnMouseUp(e); 
        }
        
        protected override void OnMouseHover(EventArgs e) 
        { 
            base.OnMouseHover(e); 
            _input.OnMouseHover(); 
        }
        
        protected override void OnClick(EventArgs e) 
        { 
            base.OnClick(e); 
            _input.OnClick(); 
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _input.OnGotFocus();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _input.OnLostFocus();
        }

        // Key event handling
        protected override bool ProcessDialogKey(System.Windows.Forms.Keys keyData)
        {
            if (_input.ProcessDialogKey(keyData))
                return true;
            return base.ProcessDialogKey(keyData);
        }
        #endregion

        #region Parent Change Handling
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            RegisterBadgeDrawer();
            //RegisterExternalLabelHelperDrawer();
        }
        private void RegisterBadgeDrawer()
        {
            var oldParent = Tag as Control;
            Tag = Parent;

            if (oldParent is BaseControl oldBeepParent)
            {
                oldBeepParent.ClearChildExternalDrawing(this);
            }

            if (Parent == null)
            {
                var form = FindForm();
                if (form != null)
                {
                    foreach (Control c in form.Controls)
                    {
                        if (c is BaseControl beepParent)
                        {
                            try
                            {
                                beepParent.ClearChildExternalDrawing(this);
                                // Parent is null here; invalidate the actual parent we cleared
                                beepParent.Invalidate();
                            }
                            catch { }
                        }
                    }
                }
                // Nothing else to register if no parent
                return;
            }

            // Register external badge drawing with new parent, if any
            if (Parent is BaseControl newBeepParent)
            {
                if (!string.IsNullOrEmpty(BadgeText))
                {
                    newBeepParent.AddChildExternalDrawing(this, DrawBadgeExternally, DrawingLayer.AfterAll);
                    // Mark for redraw on parent
                    UpdateRegionForBadge();
                    try { Parent?.Invalidate(); } catch { }
                }
            }
        }

        
     
        #endregion

        #region Event Invokers for Helper Classes

        /// <summary>
        /// Invoke property validation event for helper classes
        /// </summary>
        public void InvokePropertyValidate(BeepComponentEventArgs args)
        {
            PropertyValidate?.Invoke(this, args);
        }

        /// <summary>
        /// Invoke value changed event for helper classes
        /// </summary>
        public void InvokeOnValueChanged(BeepComponentEventArgs args)
        {
            OnValueChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Invoke linked value changed event for helper classes
        /// </summary>
        public void InvokeOnLinkedValueChanged(BeepComponentEventArgs args)
        {
            OnLinkedValueChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Invoke property changed event for helper classes
        /// </summary>
        public void InvokePropertyChanged(BeepComponentEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Invoke submit changes event for helper classes
        /// </summary>
        public void InvokeSubmitChanges(BeepComponentEventArgs args)
        {
            SubmitChanges?.Invoke(this, args);
        }

        /// <summary>
        /// Programmatically triggers the Click event. Used by painters and helpers.
        /// </summary>
        public void TriggerClick()
        {
            if (Enabled && Visible)
            {
                OnClick(EventArgs.Empty);
            }
        }

        #endregion

        #region Fallback label/helper drawing
        /// <summary>
        /// Draws LabelText above the top border and Helper/Error text below the bottom border
        /// for painters that don't explicitly handle it. Skips when Material painter is active
        /// because it already provides floating label and helper rendering.
        /// </summary>
        private void DrawLabelAndHelperUniversal(Graphics g)
        {
            if (g == null || _painter == null) return;

            // Skip for Material painter (handled inside its painter)
            if (_painter is Helpers.Painters.MaterialBaseControlPainter) return;

            // Nothing to draw
            bool hasLabel = !string.IsNullOrEmpty(LabelText);
            bool hasSupporting = !string.IsNullOrEmpty(ErrorText) || !string.IsNullOrEmpty(HelperText);
            if (!hasLabel && !hasSupporting) return;

            // Use painter border rect when available
            Rectangle border = _painter.BorderRect;
            if (border.Width <= 0 || border.Height <= 0) border = new Rectangle(0, 0, Width - 1, Height - 1);

            // Label
            if (hasLabel)
            {
                float labelSize = Math.Max(8f, Font.Size - 1f);
                using var lf = new Font(Font.FontFamily, labelSize, FontStyle.Regular);
                int labelHeight = TextRenderer.MeasureText(g, "Ag", lf, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Height;
                var labelRect = new Rectangle(border.Left + 6, Math.Max(0, border.Top - labelHeight - 2), Math.Max(10, border.Width - 12), labelHeight);
                Color labelColor = string.IsNullOrEmpty(ErrorText) ? ForeColor : ErrorColor;
                TextRenderer.DrawText(g, LabelText, lf, labelRect, labelColor, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }

            // Helper / Error
         
        }

        #endregion
    }
}