using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers;
using TheTechIdea.Beep.Winform.Controls.Base.Helpers.Painters;

using TheTechIdea.Beep.Winform.Controls.Helpers;
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
            if (IsDisposed || Disposing) return;
            
            // Invalidate parent background cache since our position changed
            InvalidateParentBackgroundCache();
            
            // Ensure parent redraws badge area when we move
            UpdateRegionForBadge();
        }
        #endregion

        #region Paint Pipeline
        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            if (IsDisposed || Disposing) return;

            // Keep painter layout in sync with padding changes
            EnsurePainter();
            _painter?.UpdateLayout(this);
            Invalidate();
        }
        
        /// <summary>
        /// Excludes child control bounds from painting to prevent flickering in container controls.
        /// DISABLED: This causes child controls to not paint initially - they need their own paint events
        /// </summary>
        private void ExcludeChildControlsFromClip(Graphics g)
        {
            // DISABLED - was causing child controls to not paint until hover/click
            // if (!IsContainerControl) return;
            // 
            // foreach (Control child in Controls)
            // {
            //     if (child.Visible)
            //     {
            //         g.ExcludeClip(child.Bounds);
            //     }
            // }
        }

        private void ExcludeConfiguredPaintRectangles(Graphics g)
        {
            if (g == null || _excludedPaintRectangles.Count == 0)
            {
                return;
            }

            Rectangle clientRect = ClientRectangle;
            for (int i = 0; i < _excludedPaintRectangles.Count; i++)
            {
                var rectangle = _excludedPaintRectangles[i];
                if (rectangle.Width <= 0 || rectangle.Height <= 0)
                {
                    continue;
                }

                var clipped = Rectangle.Intersect(clientRect, rectangle);
                if (clipped.Width > 0 && clipped.Height > 0)
                {
                    g.ExcludeClip(clipped);
                }
            }
        }
        
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Exclude child controls from clip region to prevent painting over them
            if (IsContainerControl || Controls.Count > 0)
            {
                ExcludeChildControlsFromClip(e.Graphics);
            }

            if (Parent != null && IsChild)
            {
                // Read-only snapshot for painting transparency — do NOT assign BackColor here
                // to avoid triggering IComponentChangeService and designer file writes on every frame.
                // BackColor is set once in ApplyTheme() and OnParentChanged().
                ParentBackColor = Parent.BackColor;
                BackColor = ParentBackColor;
            }

            if (IsTransparentBackground)
            {
                PaintParentBackground(e.Graphics);
                return;
            }
            base.OnPaintBackground(e);
            
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        /// <summary>
        /// Invalidates the cached parent background. Call when theme or style changes.
        /// </summary>
        public void InvalidateParentBackgroundCache()
        {
            _parentBackgroundCacheValid = false;
            _cachedParentBackground?.Dispose();
            _cachedParentBackground = null;
        }

        private void PaintParentBackground(Graphics g)
        {
            if (Parent == null || Width <= 0 || Height <= 0) return;

          

            // Use cached background if valid
            if (_parentBackgroundCacheValid && _cachedParentBackground != null)
            {
                g.DrawImageUnscaled(_cachedParentBackground, 0, 0);
                return;
            }

            // Cache is invalid - capture parent background using BitBlt
            // This only happens once (or when cache is invalidated), avoiding feedback loop
            
            // Create cache bitmap if needed
            if (_cachedParentBackground == null || 
                _cachedParentBackground.Width != Width || 
                _cachedParentBackground.Height != Height)
            {
                _cachedParentBackground?.Dispose();
                _cachedParentBackground = new Bitmap(Width, Height, g);
            }

            // Capture parent background to cache
            using (var cacheGraphics = Graphics.FromImage(_cachedParentBackground))
            {
                IntPtr cacheDc = cacheGraphics.GetHdc();
                IntPtr parentDc = IntPtr.Zero;
                
                try
                {
                    // Get parent's client DC (excludes title bar and borders)
                    parentDc = GetDC(Parent.Handle);
                    
                    if (parentDc != IntPtr.Zero)
                    {
                        // BitBlt from parent at our location (in parent's client coordinates)
                        // Location property is always relative to parent's client area
                        BitBlt(cacheDc, 0, 0, Width, Height, parentDc, Location.X, Location.Y, 0x00CC0020); // SRCCOPY
                    }
                }
                finally
                {
                    if (cacheDc != IntPtr.Zero)
                        cacheGraphics.ReleaseHdc(cacheDc);
                    if (parentDc != IntPtr.Zero)
                        ReleaseDC(Parent.Handle, parentDc);
                }
            }

            // Mark cache as valid
            _parentBackgroundCacheValid = true;

            // Draw the cached background
            g.DrawImageUnscaled(_cachedParentBackground, 0, 0);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            // Early out for safety during design-time removal/dispose
            if (IsDisposed || Disposing || !IsHandleCreated)
                return;
            base.OnPaint(e);
            
            // Exclude child controls from clip region to prevent painting over them
            if (IsContainerControl || Controls.Count > 0)
            {
                ExcludeChildControlsFromClip(e.Graphics);
            }
         
            // Always ensure clip rectangle is valid
            SafeDraw(e.Graphics);
        }

        // Safe drawing without buffering
        private void SafeDraw(Graphics g)
        {
            try
            {
              //  Console.WriteLine($"BaseControl.SafeDraw called for {Name} at {DateTime.Now:HH:mm:ss.fff}");
                ClearDrawingSurface(g);
                // Paint the inner area using the new PaintInnerShape method
                //SafePaintInnerShape(g);
              //  Console.WriteLine($"BaseControl.SafeDraw - after ClearDrawingSurface and SafePaintInnerShape for {Name} at {DateTime.Now:HH:mm:ss.fff}");
                // External drawing hooks BEFORE content
                SafeExternalDrawing(g, DrawingLayer.BeforeContent);
              //  Console.WriteLine($"BaseControl.SafeDraw - after BeforeContent external drawing for {Name} at {DateTime.Now:HH:mm:ss.fff}");
                // Main content and painter
                DrawContent(g);
              //  Console.WriteLine($"BaseControl.SafeDraw - after DrawContent for {Name} at {DateTime.Now:HH:mm:ss.fff}");
                 DrawLabelAndHelperUniversal(g);
               //  Console.WriteLine($"BaseControl.SafeDraw - after DrawLabelAndHelperUniversal for {Name} at {DateTime.Now:HH:mm:ss.fff}");
                // After-all external drawings
                SafeExternalDrawing(g, DrawingLayer.AfterAll);
              //  Console.WriteLine($"BaseControl.SafeDraw - after AfterAll external drawing for {Name} at {DateTime.Now:HH:mm:ss.fff}");
                // Effects
                SafeDrawEffects(g);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                System.Diagnostics.Debug.WriteLine($"BaseControl.SafeDraw error: {ex.Message}");
            }
        }

        private void ClearDrawingSurface(Graphics g)
        {
            if (g == null)
                return;

            if (!AllowBaseControlClear)
                return;

            if (!IsTransparentBackground)
            {
                if (IsContainerControl || Controls.Count > 0)
                {
                    ExcludeChildControlsFromClip(g);
                }
                g.Clear(BackColor);
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
            if (IsDisposed || Disposing) return;

            // Avoid updating regions/layout if dimensions aren't positive
            if (Width <= 0 || Height <= 0) return;

            try
            {
                // Invalidate parent background cache since our size changed
                if (IsTransparentBackground || (IsRounded && BorderRadius > 0))
                {
                    // Invalidate parent background cache since our size changed
                    InvalidateParentBackgroundCache(); bool isGrid = this is GridX.BeepGridPro;
                }
                if (!GridMode)
                {
                    UpdateDrawingRect();
                    UpdateControlRegion();
                    UpdateRegionForBadge();
                    Invalidate();
                }
                // For BeepGridPro, ResizeRedraw Style automatically triggers repaint
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
                // Custom-shape controls set their own BorderPath; skip painter to avoid overwrite
                if (!IsCustomShape)
                {
                    _painter?.UpdateLayout(this);
                }

                Region controlRegion;
                if (BorderPath != null)
                {
                    controlRegion = new Region(BorderPath);
                }
                else
                {
                    controlRegion = new Region(new Rectangle(0, 0, Width, Height));
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
                                badgePath.AddPath(GraphicsExtensions.GetRoundedRectPath(badgeRect, badgeRect.Height / 4), false);
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
            if (IsDisposed || Disposing) return;
            base.OnMouseEnter(e); 
            IsHovered = true; 
            _input.OnMouseEnter(); 
        }
        
        protected override void OnMouseLeave(EventArgs e) 
        { 
            if (IsDisposed || Disposing) return;
            base.OnMouseLeave(e); 
            IsHovered = false; 
            _input.OnMouseLeave(); 
        }
        
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e) 
        { 
            if (IsDisposed || Disposing) return;
            base.OnMouseMove(e); 
            _input.OnMouseMove(e.Location); 
        }
        
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e) 
        { 
            if (IsDisposed || Disposing) return;
            base.OnMouseDown(e); 
            if (e.Button == System.Windows.Forms.MouseButtons.Left) IsPressed = true; 
            _input.OnMouseDown(e); 
        }
        
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e) 
        { 
            if (IsDisposed || Disposing) return;
            base.OnMouseUp(e); 
            if (e.Button == System.Windows.Forms.MouseButtons.Left) IsPressed = false; 
            _input.OnMouseUp(e); 
        }
        
        protected override void OnMouseHover(EventArgs e) 
        { 
            if (IsDisposed || Disposing) return;
            base.OnMouseHover(e); 
            _input.OnMouseHover(); 
        }
        
        protected override void OnClick(EventArgs e) 
        { 
            if (IsDisposed || Disposing) return;
            base.OnClick(e); 
            _input.OnClick(); 
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (IsDisposed || Disposing) return;
            base.OnGotFocus(e);
            _input.OnGotFocus();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (IsDisposed || Disposing) return;
            base.OnLostFocus(e);
            _input.OnLostFocus();
        }

        // Key event handling
        protected override bool ProcessDialogKey(System.Windows.Forms.Keys keyData)
        {
            if (IsDisposed || Disposing) return false;
            if (_input.ProcessDialogKey(keyData))
                return true;
            return base.ProcessDialogKey(keyData);
        }
        #endregion

        #region Parent Change Handling
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (IsDisposed || Disposing) return;

        // Clear ALL external drawings for this child from old parent
        // Parent keeps track of all drawings per child, so we clear all at once
        var oldParent = Tag as Control;
        Tag = Parent;

        if (oldParent is IExternalDrawingProvider oldExternalDrawingProvider &&
            oldParent is Control oldParentControl &&
            !oldParentControl.IsDisposed &&
            !oldParentControl.Disposing)
        {
            oldExternalDrawingProvider.ClearChildExternalDrawing(this);
        }

            if (Parent == null)
            {
                // Try to clear from form if parent is null
                var form = FindForm();
                if (form != null && !form.IsDisposed && !form.Disposing)
                {
                    foreach (Control c in form.Controls)
                    {
                        if (c == null || c.IsDisposed || c.Disposing)
                            continue;

                        if (c is BaseControl beepParent && !beepParent.IsDisposed && !beepParent.Disposing)
                        {
                            try
                            {
                                beepParent.ClearChildExternalDrawing(this);
                                if (!beepParent.IsDisposed && !beepParent.Disposing)
                                {
                                    beepParent.Invalidate();
                                }
                            }
                            catch { }
                        }
                    }
                }
                return;
            }

            // Sync background color from new parent once (avoids doing it on every paint frame)
            if (IsChild && Parent != null)
            {
                ParentBackColor = ResolveUsableParentBackColor(Parent);
                BackColor = ParentBackColor;
            }

            // Now register all external drawings with new parent (parent tracks them per child)
            RegisterBadgeDrawer();
            RegisterExternalLabelHelperDrawer();
        }

        private Color ResolveUsableParentBackColor(Control parent)
        {
            if (GetStyle(ControlStyles.SupportsTransparentBackColor))
            {
                return parent.BackColor;
            }

            Control? current = parent;
            while (current != null)
            {
                Color candidate = current.BackColor;
                if (candidate.A == byte.MaxValue)
                {
                    return candidate;
                }

                current = current.Parent;
            }

            return SystemColors.Control;
        }
        
        /// <summary>
        /// Registers badge external drawing with parent. Does NOT clear - parent tracks all drawings per child.
        /// </summary>
        private void RegisterBadgeDrawer()
        {
            if (Parent == null) return;

            // Register external badge drawing with new parent, if any
            if (Parent is IExternalDrawingProvider newExternalDrawingProvider)
            {
                if (!string.IsNullOrEmpty(BadgeText))
                {
                    var badgeHandler = BaseControl.CreateBadgeDrawingHandler(
                        BadgeText, BadgeBackColor, BadgeForeColor, BadgeFont, BadgeShape);
                    newExternalDrawingProvider.AddChildExternalDrawing(this, badgeHandler, DrawingLayer.AfterAll);
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
                float labelSize = Math.Max(8f, TextFont.Size - 1f);
                using var lf = BeepFontManager.GetFont(TextFont.FontFamily.Name, labelSize, FontStyle.Regular);
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
