using System;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    public partial class BeepNotification
    {
        #region Public Methods

        /// <summary>
        /// Show the notification and start auto-dismiss timer if duration > 0
        /// </summary>
        public new void Show()
        {
            base.Show();
            _progressPercentage = 100f;

            if (_notificationData != null && _notificationData.Duration > 0)
            {
                _startTime = DateTime.Now;
                _remainingDuration = _notificationData.Duration;
                _autoDismissTimer.Start();
                
                if (_notificationData.ShowProgressBar)
                {
                    _progressTimer.Start();
                }
            }
        }

        /// <summary>
        /// Dismiss the notification
        /// </summary>
        public void Dismiss()
        {
            StopTimers();
            
            var args = new NotificationEventArgs
            {
                Notification = _notificationData
            };
            
            NotificationDismissed?.Invoke(this, args);
            
            if (!args.Cancel)
            {
                Visible = false;
            }
        }

        /// <summary>
        /// Pause auto-dismiss timer
        /// </summary>
        public void Pause()
        {
            if (!_isPaused && _autoDismissTimer.Enabled)
            {
                _isPaused = true;
                var elapsed = (DateTime.Now - _startTime).TotalMilliseconds;
                _remainingDuration = Math.Max(0, _notificationData.Duration - (int)elapsed);
                _autoDismissTimer.Stop();
                _progressTimer.Stop();
            }
        }

        /// <summary>
        /// Resume auto-dismiss timer
        /// </summary>
        public void Resume()
        {
            if (_isPaused && _remainingDuration > 0)
            {
                _isPaused = false;
                _startTime = DateTime.Now;
                _autoDismissTimer.Start();
                _progressTimer.Start();
            }
        }
        #endregion

        #region Private Methods - Timers

        private void AutoDismissTimer_Tick(object sender, EventArgs e)
        {
            if (_isPaused)
                return;

            var elapsed = (DateTime.Now - _startTime).TotalMilliseconds;
            
            if (elapsed >= _remainingDuration)
            {
                Dismiss();
            }
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (_isPaused || _notificationData == null || _notificationData.Duration <= 0)
                return;

            var elapsed = (DateTime.Now - _startTime).TotalMilliseconds;
            _progressPercentage = Math.Max(0, 100f - (float)(elapsed / _notificationData.Duration * 100));
            Invalidate();
        }

        private void StopTimers()
        {
            _autoDismissTimer?.Stop();
            _progressTimer?.Stop();
            _isPaused = false;
        }

        private void BeepNotification_MouseEnter(object sender, EventArgs e)
        {
            if (_notificationData != null && _notificationData.PauseOnHover)
            {
                Pause();
            }
        }

        private void BeepNotification_MouseLeave(object sender, EventArgs e)
        {
            if (_notificationData != null && _notificationData.PauseOnHover)
            {
                Resume();
            }
        }
        #endregion
    }
}
