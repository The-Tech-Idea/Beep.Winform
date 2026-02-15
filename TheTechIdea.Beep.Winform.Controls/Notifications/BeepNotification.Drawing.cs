using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Notifications.Helpers;
using TheTechIdea.Beep.Winform.Controls.Notifications.Painters;

namespace TheTechIdea.Beep.Winform.Controls.Notifications
{
    public partial class BeepNotification
    {
        #region Private Methods - Visuals

        private void UpdateNotificationVisuals()
        {
            if (_notificationData == null)
                return;

            // Use NotificationThemeHelpers for consistent color management
            var colors = NotificationThemeHelpers.GetColorsForType(
                _notificationData.Type,
                null, // Theme can be passed if available
                _notificationData.CustomBackColor,
                _notificationData.CustomForeColor,
                null,
                _notificationData.IconTint
            );
            
            if (_notificationData.CustomBackColor.HasValue)
            {
                BackColor = _notificationData.CustomBackColor.Value;
            }
            else
            {
                BackColor = colors.BackColor;
            }

            if (_notificationData.CustomForeColor.HasValue)
            {
                ForeColor = _notificationData.CustomForeColor.Value;
            }
            else
            {
                ForeColor = colors.ForeColor;
            }

            BorderColor = colors.BorderColor;

            // Update icon path and tint
            _iconPath = !string.IsNullOrEmpty(_notificationData.IconPath)
                ? _notificationData.IconPath
                : NotificationData.GetDefaultIconForType(_notificationData.Type);

            _iconTint = _notificationData.IconTint ?? colors.IconColor;
        }

        private void RecalculateLayout()
        {
            if (_notificationData == null)
                return;

            // Use NotificationLayoutHelpers for layout calculation
            Rectangle content = ClientRectangle;
            if (content.IsEmpty || content.Width <= 0 || content.Height <= 0)
                return;

            int padding = NotificationStyleHelpers.GetRecommendedPadding(_notificationData.Layout, this);
            int spacing = NotificationStyleHelpers.GetRecommendedSpacing(_notificationData.Layout, this);
            int iconSize = NotificationStyleHelpers.GetRecommendedIconSize(_notificationData.Layout, this);

            bool hasIcon = !string.IsNullOrEmpty(_notificationData.IconPath) ||
                          _notificationData.Type != NotificationType.Custom;

            // Use layout helper to calculate metrics
            var metrics = NotificationLayoutHelpers.CalculateLayout(
                content,
                _notificationData.Layout,
                hasIcon,
                !string.IsNullOrEmpty(_notificationData.Title),
                !string.IsNullOrEmpty(_notificationData.Message),
                _notificationData.Actions != null && _notificationData.Actions.Length > 0,
                _notificationData.ShowCloseButton,
                _notificationData.ShowProgressBar,
                iconSize,
                padding,
                spacing,
                this
            );

            _iconRect = metrics.IconRect;
            _titleRect = metrics.TitleRect;
            _messageRect = metrics.MessageRect;
            _actionsRect = metrics.ActionsRect;
            _closeButtonRect = metrics.CloseButtonRect;
            _progressBarRect = metrics.ProgressBarRect;

            // Adjust height if needed
            int requiredHeight = Math.Max(_titleRect.Bottom, _messageRect.Bottom) + padding;
            if (!_actionsRect.IsEmpty)
                requiredHeight += _actionsRect.Height + spacing;
            if (_notificationData.ShowProgressBar)
                requiredHeight += ScaledProgressBarHeight;

            if (Height < requiredHeight)
            {
                Height = Math.Min(requiredHeight, MaximumSize.Height);
            }
        }
        #endregion

        #region Override - Drawing

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RecalculateLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_notificationData == null || _painter == null)
                return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            RecalculateLayout();

            // Use painter to draw notification elements
            _painter.PaintBackground(g, ClientRectangle, _notificationData);

            if (!_iconRect.IsEmpty && !string.IsNullOrEmpty(_iconPath))
            {
                _painter.PaintIcon(g, _iconRect, _notificationData);
            }

            if (!_closeButtonRect.IsEmpty)
            {
                bool isHovered = _closeButtonRect.Contains(PointToClient(Cursor.Position));
                _painter.PaintCloseButton(g, _closeButtonRect, isHovered, _notificationData);
            }

            if (!_titleRect.IsEmpty && !string.IsNullOrEmpty(_notificationData.Title))
            {
                _painter.PaintTitle(g, _titleRect, _notificationData.Title, _notificationData);
            }

            if (!_messageRect.IsEmpty && !string.IsNullOrEmpty(_notificationData.Message))
            {
                _painter.PaintMessage(g, _messageRect, _notificationData.Message, _notificationData);
            }

            if (!_actionsRect.IsEmpty && _notificationData.Actions != null)
            {
                _painter.PaintActions(g, _actionsRect, _notificationData.Actions, _notificationData);
            }

            if (!_progressBarRect.IsEmpty)
            {
                _painter.PaintProgressBar(g, _progressBarRect, _progressPercentage, _notificationData);
            }
        }
        #endregion
    }
}
