using System;
using System.Drawing;
using System.Drawing.Drawing2D;

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
            _paint.UpdateRects();
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            SuspendLayout();
            base.OnResize(e);
            _paint.UpdateRects();
            
            if (IsHandleCreated)
            {
                BeginInvoke((MethodInvoker)delegate { UpdateControlRegion(); });
            }
            else
            {
                UpdateControlRegion();
            }
            
            // Ensure parent redraws badge area if size changes
            UpdateRegionForBadge();
            ResumeLayout();
        }

        private void UpdateControlRegion()
        {
            if (Width <= 0 || Height <= 0) return;

            Region controlRegion;
            var regionRect = new Rectangle(0, 0, Width, Height);

            if (IsRounded)
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

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Handled in OnPaint
        }

        protected override  void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Console.WriteLine("OnPaint called");
            // Use the current BufferedGraphicsContext to allocate a buffer
          
            // Use built-in DoubleBuffered painting. Avoid disposing global BufferedGraphicsContext.
            var g = e.Graphics;
                if (IsChild)
                {
                    BackColor = ParentBackColor;
                }

                // Clear the entire buffer with the control's BackColor
                g.Clear(BackColor);
                // External drawing - before content
                _externalDrawing.PerformExternalDrawing(g, DrawingLayer.BeforeContent);
            Console.WriteLine("After external drawing");
            DrawContent(g);
            Console.WriteLine("After DrawContent");
            // Draw hit area components (parity with BeepControl)
            if (_hitTest?.HitList != null)
            {
                foreach (var hitTest in _hitTest.HitList)
                {
                    if (hitTest.IsVisible && hitTest.uIComponent != null)
                    {
                        hitTest.uIComponent.Draw(g, hitTest.TargetRect);
                    }
                }
            }

            // Effects and overlays
            _effects.DrawOverlays(g);

            // External drawing - after content
            _externalDrawing.PerformExternalDrawing(g, 
                DrawingLayer.AfterContent);

            // External drawing - after all
            _externalDrawing.PerformExternalDrawing(g, DrawingLayer.AfterAll);

         
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
                            beepParent.ClearChildExternalDrawing(this);
                            Parent.Invalidate();
                        }
                    }
                }
            }

            // Register external badge drawing with new parent, if any
            if (Parent is BaseControl newBeepParent)
            {
                if (!string.IsNullOrEmpty(BadgeText))
                {
                    newBeepParent.AddChildExternalDrawing(this, DrawBadgeExternally, DrawingLayer.AfterAll);
                    // Mark for redraw on parent
                    UpdateRegionForBadge();
                    Parent.Invalidate();
                }
            }
        }
        #endregion

        #region Event Invokers for Helper Classes

        /// <summary>
        /// Invoke property validation event for helper classes
        /// </summary>
        internal void InvokePropertyValidate(BeepComponentEventArgs args)
        {
            PropertyValidate?.Invoke(this, args);
        }

        /// <summary>
        /// Invoke value changed event for helper classes
        /// </summary>
        internal void InvokeOnValueChanged(BeepComponentEventArgs args)
        {
            OnValueChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Invoke linked value changed event for helper classes
        /// </summary>
        internal void InvokeOnLinkedValueChanged(BeepComponentEventArgs args)
        {
            OnLinkedValueChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Invoke property changed event for helper classes
        /// </summary>
        internal void InvokePropertyChanged(BeepComponentEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Invoke submit changes event for helper classes
        /// </summary>
        internal void InvokeSubmitChanges(BeepComponentEventArgs args)
        {
            SubmitChanges?.Invoke(this, args);
        }

        #endregion
    }
}