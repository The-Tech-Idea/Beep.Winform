using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Base.Helpers
{
    internal class ControlHitTestHelper
    {
        private readonly BaseControl _owner;

        public ControlHitTestHelper(BaseControl owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            HitList = new List<ControlHitTest>();
        }

        #region Properties
        public List<ControlHitTest> HitList { get; set; }
        public ControlHitTest HitTestControl { get;  set; }
        public bool HitAreaEventOn { get;  set; }
        #endregion

        #region Events
        public event EventHandler<ControlHitTestArgs> OnControlHitTest;
        public event EventHandler<ControlHitTestArgs> HitDetected;
        #endregion

        #region Hit Area Management
        public void AddHitTest(ControlHitTest hitTest)
        {
            var index = HitList.FindIndex(x => x.TargetRect == hitTest.TargetRect);
            if (index >= 0)
            {
                HitList[index] = hitTest;
            }
            else
            {
                HitList.Add(hitTest);
            }
        }

        public void AddHitArea(string name, IBeepUIComponent component = null, Action hitAction = null)
        {
            Rectangle targetRect = Rectangle.Empty;
            bool isVisible = true;
            bool isEnabled = true;

            if (component is Control control && control.Visible)
            {
                targetRect = new Rectangle(control.Location, control.Size);
                isVisible = control.Visible;
                isEnabled = control.Enabled;

                Action wrappedHitAction = hitAction != null ? () =>
                {
                    if (component != null)
                    {
                        component.SendMouseEvent(component, MouseEventType.Click, _owner.PointToScreen(targetRect.Location));
                    }
                    hitAction.Invoke();
                } : null;

                var hitTest = new ControlHitTest
                {
                    Name = name,
                    GuidID = Guid.NewGuid().ToString(),
                    TargetRect = targetRect,
                    uIComponent = component,
                    HitAction = wrappedHitAction,
                    IsVisible = isVisible,
                    IsEnabled = isEnabled
                };

                var index = HitList.FindIndex(x => x.Name == name);
                if (index >= 0)
                {
                    HitList[index] = hitTest;
                }
                else
                {
                    HitList.Add(hitTest);
                }
            }
        }

        public void AddHitArea(string name, Rectangle rect, IBeepUIComponent component = null, Action hitAction = null)
        {
            var hitTest = new ControlHitTest
            {
                Name = name,
                GuidID = Guid.NewGuid().ToString(),
                TargetRect = rect,
                uIComponent = component,
                HitAction = hitAction,
                IsVisible = true,
                IsEnabled = true
            };

            var index = HitList.FindIndex(x => x.Name == name);
            if (index >= 0)
            {
                HitList[index] = hitTest;
            }
            else
            {
                HitList.Add(hitTest);
            }
        }

        public void AddHitTest(Control childControl)
        {
            if (childControl == null)
                throw new ArgumentNullException(nameof(childControl));

            if (!_owner.Controls.Contains(childControl))
                throw new ArgumentException("The specified control is not a child of this control.", nameof(childControl));

            var hitTest = new ControlHitTest
            {
                Name = childControl.Name,
                GuidID = Guid.NewGuid().ToString(),
                TargetRect = childControl.Bounds,
                IsVisible = childControl.Visible,
                IsEnabled = childControl.Enabled,
            };

            var index = HitList.FindIndex(x => x.Name == childControl.Name);
            if (index >= 0)
            {
                HitList[index] = hitTest;
            }
            else
            {
                HitList.Add(hitTest);
            }
        }

        public void RemoveHitTest(ControlHitTest hitTest)
        {
            HitList.Remove(hitTest);
        }

        public void ClearHitList()
        {
            HitList.Clear();
        }

        public void UpdateHitTest(ControlHitTest hitTest)
        {
            var index = HitList.FindIndex(x => x.TargetRect == hitTest.TargetRect);
            if (index >= 0)
            {
                HitList[index] = hitTest;
            }
        }
        #endregion

        #region Hit Testing
        public bool HitTest(Point location)
        {
            if (HitList == null || !HitList.Any())
            {
                HitAreaEventOn = false;
                HitTestControl = null;
                return false;
            }
            HitTestControl = null;
            bool hitDetected = false;
            HitAreaEventOn = false;
            foreach (var hitTest in HitList)
            {
                hitTest.IsHit = false;
              
                if (hitTest?.TargetRect != null && hitTest.IsVisible && hitTest.IsEnabled && hitTest.TargetRect.Contains(location))
                {
                    hitTest.IsHit = true;
                    hitDetected = true;
                    HitAreaEventOn = true;
                    HitTestControl = hitTest;

                    OnControlHitTest?.Invoke(_owner, new ControlHitTestArgs(hitTest));
                    HitDetected?.Invoke(_owner, new ControlHitTestArgs(hitTest));
                    break;
                }
            }

            if (!hitDetected)
            {
                HitAreaEventOn = false;
                HitTestControl = null;
            }

            return hitDetected;
        }

        public bool HitTest(Point location, out ControlHitTest hitTest)
        {
            hitTest = null;
            foreach (var test in HitList)
            {
                if (test.TargetRect.Contains(location))
                {
                    hitTest = test;
                    return true;
                }
            }
            return false;
        }

        public bool HitTest(Rectangle rectangle, out ControlHitTest hitTest)
        {
            hitTest = null;
            foreach (var test in HitList)
            {
                if (test.TargetRect.IntersectsWith(rectangle))
                {
                    hitTest = test;
                    return true;
                }
            }
            return false;
        }

        public bool HitTestWithMouse()
        {
            if (!_owner.Visible || HitList == null || !HitList.Any())
            {
                HitAreaEventOn = false;
                HitTestControl = null;
                return false;
            }

            Point location = _owner.PointToClient(Cursor.Position);
            return HitTest(location);
        }
        #endregion

        #region Mouse Event Handling
        public void SendMouseEvent(IBeepUIComponent targetControl, MouseEventType eventType, Point screenLocation)
        {
            if (targetControl == null) return;
            
            Point clientPoint = screenLocation;
            if (targetControl is Control control)
            {
                clientPoint = control.PointToClient(screenLocation);
            }
            
            var args = new HitTestEventArgs(eventType, clientPoint);
            targetControl.ReceiveMouseEvent(args);
        }

        public void HandleMouseEnter(Point location)
        {
            if (HitTest(location) && HitTestControl != null && HitTestControl.uIComponent != null)
            {
                if (!HitTestControl.IsHovered)
                {
                    HitTestControl.IsHovered = true;
                    SendMouseEvent(HitTestControl.uIComponent, MouseEventType.MouseEnter, _owner.PointToScreen(location));
                    // Do NOT invoke HitAction here - only on Click to prevent duplicate actions
                }
       
            }
        }

        public void HandleMouseMove(Point location)
        {
            if (HitTest(location))
            {
                if (HitTestControl != null && HitTestControl.uIComponent != null)
                {
                    if (!HitTestControl.IsHovered)
                    {
                        HitTestControl.IsHovered = true;
                        SendMouseEvent(HitTestControl.uIComponent, MouseEventType.MouseMove, _owner.PointToScreen(location));
                        
                    }
                  
                   
                }
            }
            else
            {
                foreach (var hitTest in HitList)
                {
                    hitTest.IsHovered = false;
                }
            }
        }

        public void HandleMouseLeave()
        {
            HitAreaEventOn = false;

            if (HitTestControl != null && HitTestControl.uIComponent != null)
            {
                SendMouseEvent(HitTestControl.uIComponent, MouseEventType.MouseLeave, _owner.PointToScreen(Point.Empty));
            }
            
            HitTestControl = null;

            if (HitList != null)
            {
                foreach (var hitTest in HitList)
                {
                    hitTest.IsHit = false;
                    hitTest.IsHovered = false;
                    hitTest.IsPressed = false;
                }
            }
        }

        public void HandleClick(Point location)
        {
            if (_owner.IsDisposed) return;

            try
            {
                if (!HitTest(location)) return;

                if (HitTestControl == null) return;

                // If we have a UI component, forward the event; otherwise, just execute action
                if (HitTestControl.uIComponent != null)
                {
                    if (HitTestControl.uIComponent is Control control && control.IsDisposed) return;
                    SendMouseEvent(HitTestControl.uIComponent, MouseEventType.Click, _owner.PointToScreen(location));
                }

                HitTestControl.HitAction?.Invoke();
            }
            catch (ObjectDisposedException)
            {
                HitTestControl = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in HandleClick: {ex.Message}");
            }
        }

        public void HandleMouseDown(Point location, MouseEventArgs e)
        {
            if (HitTest(location))
            {
                if (HitTestControl != null)
                {
                    HitTestControl.IsPressed = true;
                    if (HitTestControl.uIComponent != null)
                    {
                        SendMouseEvent(HitTestControl.uIComponent, MouseEventType.MouseDown, _owner.PointToScreen(location));
                        // Do NOT invoke HitAction here - only on Click to prevent duplicate actions
                    }
                }
            }
        }

        public void HandleMouseUp(Point location, MouseEventArgs e)
        {
            if (HitTest(location))
            {
                if (HitTestControl != null)
                {
                    if (HitTestControl.uIComponent != null)
                    {
                        SendMouseEvent(HitTestControl.uIComponent, MouseEventType.MouseUp, _owner.PointToScreen(location));
                        // Do NOT invoke HitAction here - only on Click to prevent duplicate actions
                    }
                }
            }

            if (HitList != null)
            {
                foreach (var hitTest in HitList)
                {
                    hitTest.IsPressed = false;
                }
            }

            if (HitTestControl != null)
            {
                HitTestControl.IsHovered = true;
            }
        }

        public void HandleMouseHover(Point location)
        {
            if (HitTest(location) && HitTestControl != null && HitTestControl.uIComponent != null)
            {
                HitTestControl.IsHovered = true;
                SendMouseEvent(HitTestControl.uIComponent, MouseEventType.MouseHover, _owner.PointToScreen(location));
                // Do NOT invoke HitAction here - only on Click to prevent duplicate actions
            }
        }

        public void HandleGotFocus(Point location)
        {
            if (HitTestWithMouse() && HitTestControl != null && HitTestControl.uIComponent != null)
            {
                HitTestControl.IsFocused = true;
            }
        }

        public void HandleLostFocus()
        {
            if (HitList != null)
            {
                foreach (var hitTest in HitList)
                {
                    hitTest.IsFocused = false;
                    hitTest.IsHovered = hitTest.IsHit;
                }
            }
        }
        #endregion
    }
}
