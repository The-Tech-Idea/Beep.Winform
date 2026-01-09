using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    public partial class BeepNotification
    {
        #region Override - Mouse Handling

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            // Check close button
            if (_closeButtonRect.Contains(e.Location))
            {
                Dismiss();
                return;
            }

            // Check action buttons
            if (!_actionsRect.IsEmpty && _notificationData.Actions != null)
            {
                int buttonWidth = (_actionsRect.Width - (PADDING * (_notificationData.Actions.Length - 1))) / _notificationData.Actions.Length;
                int x = _actionsRect.X;

                for (int i = 0; i < _notificationData.Actions.Length; i++)
                {
                    var buttonRect = new Rectangle(x, _actionsRect.Y, buttonWidth, _actionsRect.Height);
                    if (buttonRect.Contains(e.Location))
                    {
                        var action = _notificationData.Actions[i];
                        var args = new NotificationEventArgs
                        {
                            Notification = _notificationData,
                            Action = action
                        };

                        ActionClicked?.Invoke(this, args);
                        action.OnClick?.Invoke(_notificationData);
                        return;
                    }
                    x += buttonWidth + PADDING;
                }
            }

            // General notification click
            NotificationClicked?.Invoke(this, new NotificationEventArgs { Notification = _notificationData });
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            // Redraw if hovering over interactive elements
            bool needsRedraw = _closeButtonRect.Contains(e.Location) ||
                             (!_actionsRect.IsEmpty && _actionsRect.Contains(e.Location));
            
            if (needsRedraw)
            {
                Invalidate();
            }
        }

        private void BeepNotification_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    // Escape key dismisses notification
                    Dismiss();
                    e.Handled = true;
                    break;

                case Keys.Enter:
                case Keys.Space:
                    // Enter/Space triggers primary action or dismisses
                    if (_notificationData?.Actions != null && _notificationData.Actions.Length > 0)
                    {
                        var primaryAction = Array.Find(_notificationData.Actions, a => a.IsPrimary) 
                                          ?? _notificationData.Actions[0];
                        
                        var args = new NotificationEventArgs
                        {
                            Notification = _notificationData,
                            Action = primaryAction
                        };

                        ActionClicked?.Invoke(this, args);
                        primaryAction.OnClick?.Invoke(_notificationData);
                    }
                    else
                    {
                        Dismiss();
                    }
                    e.Handled = true;
                    break;

                case Keys.D1:
                case Keys.NumPad1:
                    TriggerActionByIndex(0);
                    e.Handled = true;
                    break;

                case Keys.D2:
                case Keys.NumPad2:
                    TriggerActionByIndex(1);
                    e.Handled = true;
                    break;

                case Keys.D3:
                case Keys.NumPad3:
                    TriggerActionByIndex(2);
                    e.Handled = true;
                    break;
            }
        }

        private void TriggerActionByIndex(int index)
        {
            if (_notificationData?.Actions == null || index >= _notificationData.Actions.Length)
                return;

            var action = _notificationData.Actions[index];
            var args = new NotificationEventArgs
            {
                Notification = _notificationData,
                Action = action
            };

            ActionClicked?.Invoke(this, args);
            action.OnClick?.Invoke(_notificationData);
        }
        #endregion

        #region Cleanup

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopTimers();
                _autoDismissTimer?.Dispose();
                _progressTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
