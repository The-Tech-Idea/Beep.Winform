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

            var colors = NotificationThemeHelpers.GetColorsForType(
                _notificationData.Type,
                null,
                _notificationData.CustomBackColor,
                _notificationData.CustomForeColor,
                null,
                _notificationData.IconTint
            );

            BackColor   = _notificationData.CustomBackColor ?? colors.BackColor;
            ForeColor   = _notificationData.CustomForeColor ?? colors.ForeColor;
            BorderColor = colors.BorderColor;

            _iconPath = !string.IsNullOrEmpty(_notificationData.IconPath)
                ? _notificationData.IconPath
                : NotificationData.GetDefaultIconForType(_notificationData.Type);

            _iconTint = _notificationData.IconTint ?? colors.IconColor;
        }

        /// <summary>
        /// Recalculates all layout rectangles.
        /// Passes the painter's theme fonts so text heights are measured correctly –
        /// this is the fix for text size / alignment not matching notification size.
        /// Must NOT be called from inside OnPaint.
        /// </summary>
        private void RecalculateLayout()
        {
            if (_notificationData == null)
                return;

            Rectangle content = ClientRectangle;
            if (content.IsEmpty || content.Width <= 0 || content.Height <= 0)
                return;

            int padding  = NotificationStyleHelpers.GetRecommendedPadding(_notificationData.Layout, this);
            int spacing  = NotificationStyleHelpers.GetRecommendedSpacing(_notificationData.Layout, this);
            int iconSize = NotificationStyleHelpers.GetRecommendedIconSize(_notificationData.Layout, this);

            bool hasIcon   = !string.IsNullOrEmpty(_notificationData.IconPath)
                             || _notificationData.Type != NotificationType.Custom;
            bool hasTitle  = !string.IsNullOrEmpty(_notificationData.Title);
            bool hasMsg    = !string.IsNullOrEmpty(_notificationData.Message);
            bool hasActions = _notificationData.Actions != null && _notificationData.Actions.Length > 0;

            // Retrieve theme fonts from painter so layout heights are accurate.
            Font titleFont   = null;
            Font messageFont = null;
            if (_painter is NotificationPainterBase pb)
            {
                titleFont   = pb.TitleFont;
                messageFont = pb.MessageFont;
            }

            var metrics = NotificationLayoutHelpers.CalculateLayout(
                content,
                _notificationData.Layout,
                hasIcon,
                hasTitle,
                hasMsg,
                hasActions,
                _notificationData.ShowCloseButton,
                _notificationData.ShowProgressBar,
                iconSize,
                padding,
                spacing,
                this,
                titleFont,
                messageFont
            );

            _iconRect        = metrics.IconRect;
            _titleRect       = metrics.TitleRect;
            _messageRect     = metrics.MessageRect;
            _actionsRect     = metrics.ActionsRect;
            _closeButtonRect = metrics.CloseButtonRect;
            _progressBarRect = metrics.ProgressBarRect;

            // Grow height so all content fits
            int progH        = _notificationData.ShowProgressBar ? ScaledProgressBarHeight : 0;
            int required     = padding;
            if (!_titleRect.IsEmpty)   required = Math.Max(required, _titleRect.Bottom   + padding);
            if (!_messageRect.IsEmpty) required = Math.Max(required, _messageRect.Bottom + padding);
            if (!_actionsRect.IsEmpty) required = Math.Max(required, _actionsRect.Bottom + padding);
            required += progH;

            if (Height < required)
                Height = Math.Min(required, MaximumSize.Height > 0 ? MaximumSize.Height : required);
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
            g.SmoothingMode      = SmoothingMode.AntiAlias;
            g.TextRenderingHint  = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.InterpolationMode  = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode    = PixelOffsetMode.HighQuality;

            // Layout is pre-calculated in RecalculateLayout (called by OnSizeChanged)
            // — do NOT call RecalculateLayout here to avoid layout thrash on every paint tick.

            _painter.PaintBackground(g, ClientRectangle, _notificationData);

            if (!_iconRect.IsEmpty && !string.IsNullOrEmpty(_iconPath))
                _painter.PaintIcon(g, _iconRect, _notificationData);

            if (!_closeButtonRect.IsEmpty)
            {
                bool hovered = _closeButtonRect.Contains(PointToClient(Cursor.Position));
                _painter.PaintCloseButton(g, _closeButtonRect, hovered, _notificationData);
            }

            if (!_titleRect.IsEmpty && !string.IsNullOrEmpty(_notificationData.Title))
                _painter.PaintTitle(g, _titleRect, _notificationData.Title, _notificationData);

            if (!_messageRect.IsEmpty && !string.IsNullOrEmpty(_notificationData.Message))
                _painter.PaintMessage(g, _messageRect, _notificationData.Message, _notificationData);

            if (!_actionsRect.IsEmpty && _notificationData.Actions != null)
                _painter.PaintActions(g, _actionsRect, _notificationData.Actions, _notificationData);

            if (!_progressBarRect.IsEmpty)
                _painter.PaintProgressBar(g, _progressBarRect, _progressPercentage, _notificationData);
        }

        #endregion
    }
}
