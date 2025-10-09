using System.Drawing;

namespace TheTechIdea.Beep.Winform.Controls.Forms.ModernForm
{
        internal sealed class BeepiFormProLayoutManager
        {
            private readonly BeepiFormPro _owner;
            public Rectangle CaptionRect { get; private set; }
            public Rectangle ContentRect { get; private set; }
            public Rectangle BottomRect { get; private set; }
            public Rectangle LeftRect { get; private set; }
            public Rectangle RightRect { get; private set; }

            // Caption zones (for flexible alignment)
            public Rectangle LeftZoneRect { get; private set; }
            public Rectangle CenterZoneRect { get; private set; }
            public Rectangle RightZoneRect { get; private set; }

            // System button rects
            public Rectangle IconRect { get; private set; }
            public Rectangle TitleRect { get; private set; }
            public Rectangle ThemeButtonRect { get; private set; }
            public Rectangle StyleButtonRect { get; private set; }
            public Rectangle SearchButtonRect { get; private set; }
            public Rectangle ProfileButtonRect { get; private set; }
            public Rectangle MailButtonRect { get; private set; }
            public Rectangle CustomActionButtonRect { get; private set; }
            public Rectangle MinimizeButtonRect { get; private set; }
            public Rectangle MaximizeButtonRect { get; private set; }
            public Rectangle CloseButtonRect { get; private set; }

            public BeepiFormProLayoutManager(BeepiFormPro owner) { _owner = owner; }

            public void Calculate()
            {
                var r = _owner.ClientRectangle;

                // Pull metrics from painter if available; otherwise pick defaults per FormStyle
                _owner.FormPainterMetrics = FormPainterMetrics.DefaultFor(_owner.FormStyle, _owner.CurrentTheme);
                if (_owner.ActivePainter is TheTechIdea.Beep.Winform.Controls.Forms.ModernForm.Painters.IFormPainterMetricsProvider provider)
                {
                    var pm = provider.GetMetrics(_owner);
                    if (pm != null) _owner.FormPainterMetrics = pm;
                }

                // Sync painter-controlled visibility flags to owner toggles if desired
                bool showTheme = _owner.ShowThemeButton || _owner.FormPainterMetrics.ShowThemeButton;
                bool showStyle = _owner.ShowStyleButton || _owner.FormPainterMetrics.ShowStyleButton;
                bool showSearch = _owner.FormPainterMetrics.ShowSearchButton;
                bool showProfile = _owner.FormPainterMetrics.ShowProfileButton;
                bool showMail = _owner.FormPainterMetrics.ShowMailButton;

                // DPI-aware caption height: use metrics.CaptionHeight when caption is shown
                int captionH = _owner.ShowCaptionBar
                    ? System.Math.Max(_owner.ScaleDpi(_owner.FormPainterMetrics.CaptionHeight), (int)(_owner.Font.Height * _owner.FormPainterMetrics.FontHeightMultiplier))
                    : 0;

                int bottomH = 0; // start at 0; regions can use Bottom dock
                int borderWidth = _owner.ScaleDpi(_owner.FormPainterMetrics.BorderWidth);

                CaptionRect = new Rectangle(r.Left, r.Top, r.Width, captionH);
                BottomRect = new Rectangle(r.Left, r.Bottom - bottomH, r.Width, bottomH);
                LeftRect = new Rectangle(r.Left, CaptionRect.Bottom, borderWidth, r.Height - captionH - bottomH);
                RightRect = new Rectangle(r.Right - borderWidth, CaptionRect.Bottom, borderWidth, r.Height - captionH - bottomH);
                ContentRect = Rectangle.FromLTRB(r.Left + borderWidth, CaptionRect.Bottom, r.Right - borderWidth, r.Bottom - bottomH);

                if (!_owner.ShowCaptionBar)
                {
                    // No caption bar - all buttons are empty
                    IconRect = Rectangle.Empty;
                    TitleRect = Rectangle.Empty;
                    ThemeButtonRect = Rectangle.Empty;
                    StyleButtonRect = Rectangle.Empty;
                    SearchButtonRect = Rectangle.Empty;
                    ProfileButtonRect = Rectangle.Empty;
                    MailButtonRect = Rectangle.Empty;
                    CustomActionButtonRect = Rectangle.Empty;
                    MinimizeButtonRect = Rectangle.Empty;
                    MaximizeButtonRect = Rectangle.Empty;
                    CloseButtonRect = Rectangle.Empty;
                    LeftZoneRect = Rectangle.Empty;
                    CenterZoneRect = Rectangle.Empty;
                    RightZoneRect = Rectangle.Empty;
                    return;
                }

                // System metrics
                int buttonWidth = _owner.ScaleDpi(_owner.FormPainterMetrics.ButtonWidth);
                int spacing = _owner.ScaleDpi(_owner.FormPainterMetrics.ButtonSpacing);
                int buttonY = 0;
                int buttonHeight = captionH;

                bool macLike = _owner.FormStyle == FormStyle.MacOS;

                // Compute widths for groups
                int sysButtonsCount = 3; // minimize, maximize, close (order varies)
                int sysButtonsWidth = sysButtonsCount * buttonWidth;

                int extrasCount = (showTheme ? 1 : 0) + (showStyle ? 1 : 0) + (showSearch ? 1 : 0) + (showProfile ? 1 : 0) + (showMail ? 1 : 0) + ((!showTheme && !showStyle) ? 1 : 0);
                int extrasWidth = extrasCount * buttonWidth;

                if (!macLike)
                {
                    // Windows/Fluent/Material layout
                    // Right zone contains system buttons and extras
                    int rightZoneWidth = sysButtonsWidth + (spacing) + extrasWidth;
                    RightZoneRect = new Rectangle(r.Right - rightZoneWidth, r.Top, rightZoneWidth, captionH);

                    // Place from right edge
                    int currentX = r.Width;

                    // System buttons (rightmost): Close, Maximize, Minimize
                    CloseButtonRect = new Rectangle(currentX - buttonWidth, buttonY, buttonWidth, buttonHeight); currentX -= buttonWidth;
                    MaximizeButtonRect = new Rectangle(currentX - buttonWidth, buttonY, buttonWidth, buttonHeight); currentX -= buttonWidth;
                    MinimizeButtonRect = new Rectangle(currentX - buttonWidth, buttonY, buttonWidth, buttonHeight); currentX -= buttonWidth;

                    // Gap before extras
                    currentX -= spacing;

                    // Extras (to the left of system buttons)
                    ThemeButtonRect = showTheme ? new Rectangle(currentX - buttonWidth, buttonY, buttonWidth, buttonHeight) : Rectangle.Empty; if (showTheme) currentX -= buttonWidth;
                    StyleButtonRect = showStyle ? new Rectangle(currentX - buttonWidth, buttonY, buttonWidth, buttonHeight) : Rectangle.Empty; if (showStyle) currentX -= buttonWidth;
                    MailButtonRect = showMail ? new Rectangle(currentX - buttonWidth, buttonY, buttonWidth, buttonHeight) : Rectangle.Empty; if (showMail) currentX -= buttonWidth;
                    ProfileButtonRect = showProfile ? new Rectangle(currentX - buttonWidth, buttonY, buttonWidth, buttonHeight) : Rectangle.Empty; if (showProfile) currentX -= buttonWidth;
                    SearchButtonRect = showSearch ? new Rectangle(currentX - buttonWidth, buttonY, buttonWidth, buttonHeight) : Rectangle.Empty; if (showSearch) currentX -= buttonWidth;
                    CustomActionButtonRect = (!showTheme && !showStyle) ? new Rectangle(currentX - buttonWidth, buttonY, buttonWidth, buttonHeight) : Rectangle.Empty; if (!showTheme && !showStyle) currentX -= buttonWidth;

                    // Left zone: icon
                    int iconSize = System.Math.Min(captionH - _owner.ScaleDpi(_owner.FormPainterMetrics.IconLeftPadding), _owner.ScaleDpi(_owner.FormPainterMetrics.IconSize));
                    int iconY = (captionH - iconSize) / 2;
                    IconRect = new Rectangle(_owner.ScaleDpi(_owner.FormPainterMetrics.IconLeftPadding), iconY, iconSize, iconSize);
                    LeftZoneRect = new Rectangle(0, r.Top, IconRect.Right + _owner.ScaleDpi(_owner.FormPainterMetrics.TitleLeftPadding), captionH);

                    // Center zone: between left zone and right zone
                    CenterZoneRect = Rectangle.FromLTRB(LeftZoneRect.Right, r.Top, RightZoneRect.Left, r.Top + captionH);

                    // Title centered within center zone
                    int titleWidth = System.Math.Max(0, CenterZoneRect.Width - _owner.ScaleDpi(_owner.FormPainterMetrics.TitleLeftPadding) * 2);
                    int titleX = CenterZoneRect.Left + (CenterZoneRect.Width - titleWidth) / 2;
                    TitleRect = new Rectangle(titleX, 0, titleWidth, captionH);
                }
                else
                {
                    // macOS-like layout
                    // Left zone: system buttons on the left
                    int leftMargin = _owner.ScaleDpi(_owner.FormPainterMetrics.IconLeftPadding);
                    int leftZoneWidth = leftMargin + sysButtonsWidth;
                    LeftZoneRect = new Rectangle(r.Left, r.Top, leftZoneWidth, captionH);

                    // Place system buttons: Close (leftmost), Minimize, Maximize (mac order close, min, max)
                    int lx = LeftZoneRect.Left + leftMargin;
                    CloseButtonRect = new Rectangle(lx, buttonY, buttonWidth, buttonHeight); lx += buttonWidth;
                    MinimizeButtonRect = new Rectangle(lx, buttonY, buttonWidth, buttonHeight); lx += buttonWidth;
                    MaximizeButtonRect = new Rectangle(lx, buttonY, buttonWidth, buttonHeight);

                    // Right zone: extras aligned to the right
                    int rightZoneWidth = extrasWidth + (extrasWidth > 0 ? spacing : 0);
                    RightZoneRect = new Rectangle(r.Right - rightZoneWidth, r.Top, rightZoneWidth, captionH);

                    int rx = r.Width;
                    // Extras from right edge inward
                    ThemeButtonRect = showTheme ? new Rectangle(rx - buttonWidth, buttonY, buttonWidth, buttonHeight) : Rectangle.Empty; if (showTheme) rx -= buttonWidth;
                    StyleButtonRect = showStyle ? new Rectangle(rx - buttonWidth, buttonY, buttonWidth, buttonHeight) : Rectangle.Empty; if (showStyle) rx -= buttonWidth;
                    MailButtonRect = showMail ? new Rectangle(rx - buttonWidth, buttonY, buttonWidth, buttonHeight) : Rectangle.Empty; if (showMail) rx -= buttonWidth;
                    ProfileButtonRect = showProfile ? new Rectangle(rx - buttonWidth, buttonY, buttonWidth, buttonHeight) : Rectangle.Empty; if (showProfile) rx -= buttonWidth;
                    SearchButtonRect = showSearch ? new Rectangle(rx - buttonWidth, buttonY, buttonWidth, buttonHeight) : Rectangle.Empty; if (showSearch) rx -= buttonWidth;
                    CustomActionButtonRect = (!showTheme && !showStyle) ? new Rectangle(rx - buttonWidth, buttonY, buttonWidth, buttonHeight) : Rectangle.Empty; if (!showTheme && !showStyle) rx -= buttonWidth;

                    // Center zone is between left and right zones
                    CenterZoneRect = Rectangle.FromLTRB(LeftZoneRect.Right, r.Top, RightZoneRect.Left, r.Top + captionH);

                    // Icon and Title centered as a group: center the title area, and place icon just left of it
                    int iconSize = System.Math.Min(captionH - _owner.ScaleDpi(_owner.FormPainterMetrics.IconLeftPadding), _owner.ScaleDpi(_owner.FormPainterMetrics.IconSize));
                    int iconY = (captionH - iconSize) / 2;

                    int titleWidth = System.Math.Max(0, CenterZoneRect.Width - (iconSize + spacing) - _owner.ScaleDpi(_owner.FormPainterMetrics.TitleLeftPadding) * 2);
                    int centeredTitleX = CenterZoneRect.Left + (CenterZoneRect.Width - titleWidth) / 2;
                    TitleRect = new Rectangle(centeredTitleX, 0, titleWidth, captionH);

                    // Icon to the immediate left of the title
                    int iconX = System.Math.Max(LeftZoneRect.Right + spacing, TitleRect.Left - spacing - iconSize);
                    IconRect = new Rectangle(iconX, iconY, iconSize, iconSize);
                }
            }
        }
}
