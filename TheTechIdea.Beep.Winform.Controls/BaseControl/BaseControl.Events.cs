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
            
            // Add null check to prevent exceptions during initialization
            _paint?.InvalidateRects();
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            // Mark rects dirty and let drawing ensure they’re updated
            _paint?.InvalidateRects();
            
            //// Only update material layout if helper is initialized
            //if (_materialHelper != null)
            //{
            //    UpdateMaterialLayout();
            //}
            
            // Simple region update
            UpdateControlRegion();
            
            // Ensure parent redraws badge area if size changes
            UpdateRegionForBadge();
        }

        private void UpdateControlRegion()
        {
            if (Width <= 0 || Height <= 0) return;

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

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Don't call base.OnPaintBackground(e) to prevent double background drawing
            // Background is handled in OnPaint through DrawContent
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (UseExternalBufferedGraphics)
            {
                BufferedGraphicsContext context = BufferedGraphicsManager.Current;
                using (BufferedGraphics buffer = context.Allocate(e.Graphics, this.ClientRectangle))
                {
                    Graphics g = buffer.Graphics;
                    if (IsChild && ParentBackColor != Color.Empty)
                    {
                        BackColor = ParentBackColor;
                    }
                    g.Clear(BackColor);
                    _externalDrawing?.PerformExternalDrawing(g, DrawingLayer.BeforeContent);
                    DrawContent(g);
                    DrawHitListIfNeeded(g);
                    _effects?.DrawOverlays(g);
                    buffer.Render(e.Graphics);
                }
            }
            else
            {
                Graphics g = e.Graphics;
                if (IsChild && ParentBackColor != Color.Empty)
                {
                    BackColor = ParentBackColor;
                }
                g.Clear(BackColor);
                _externalDrawing?.PerformExternalDrawing(g, DrawingLayer.BeforeContent);
                DrawContent(g);
                DrawHitListIfNeeded(g);
                _effects?.DrawOverlays(g);
            }
        }

        private void DrawHitListIfNeeded(Graphics g)
        {
            if (!AutoDrawHitListComponents || _hitTest?.HitList == null || _hitTest.HitList.Count == 0)
                return;

            int drawn = 0;
            var clip = ClientRectangle;
            foreach (var ht in _hitTest.HitList)
            {
                if (!ht.IsVisible || ht.uIComponent == null)
                    continue;
                if (!clip.IntersectsWith(ht.TargetRect))
                    continue;
                ht.uIComponent.Draw(g, ht.TargetRect);
                drawn++;
                if (MaxHitListDrawPerFrame > 0 && drawn >= MaxHitListDrawPerFrame)
                    break;
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