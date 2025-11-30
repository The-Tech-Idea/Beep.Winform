using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Painters;
using TheTechIdea.Beep.Winform.Controls.Styling;
using TheTechIdea.Beep.Winform.Controls.Styling.Borders;
using TheTechIdea.Beep.Winform.Controls.Styling.ImagePainters;
using TheTechIdea.Beep.Winform.Controls.ThemeManagement;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Forms
{
    /// <summary>
    /// Modern styled dialog form with full BeepControlStyle support
    /// </summary>
    public class BeepDialog : Form
    {
        #region Fields

        private DialogConfig _config;
        private IBeepTheme _theme;
        private BeepControlStyle _style;
        private IDialogPainter _painter;

        // Layout rectangles
        private Rectangle _iconRect;
        private Rectangle _titleRect;
        private Rectangle _messageRect;
        private Rectangle _detailsRect;
        private Rectangle _customControlRect;
        private Rectangle _buttonAreaRect;
        private Rectangle[] _buttonRects;
        private Rectangle _closeButtonRect;

        // State
        private int _hoveredButtonIndex = -1;
        private int _pressedButtonIndex = -1;
        private bool _closeButtonHovered = false;
        private bool _isDragging = false;
        private Point _dragOffset;

        // Constants
        private const int PADDING = 24;
        private const int ICON_SIZE = 48;
        private const int BUTTON_HEIGHT = 40;
        private const int BUTTON_MIN_WIDTH = 100;
        private const int BUTTON_SPACING = 12;
        private const int CLOSE_BUTTON_SIZE = 32;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the dialog configuration
        /// </summary>
        [Browsable(false)]
        public DialogConfig Config
        {
            get => _config;
            set
            {
                _config = value;
                ApplyConfig();
            }
        }

        /// <summary>
        /// Gets or sets the theme
        /// </summary>
        [Browsable(false)]
        public IBeepTheme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the control style
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        public BeepControlStyle ControlStyle
        {
            get => _style;
            set
            {
                _style = value;
                _painter = DialogPainterFactory.CreatePainter(_config);
                Invalidate();
            }
        }

        /// <summary>
        /// Gets the dialog result value (for input dialogs)
        /// </summary>
        public string ResultValue { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the selected item (for list dialogs)
        /// </summary>
        public object? ResultTag { get; private set; }

        #endregion

        #region Constructor

        public BeepDialog()
        {
            InitializeComponent();
        }

        public BeepDialog(DialogConfig config) : this()
        {
            _config = config ?? new DialogConfig();
            ApplyConfig();
        }

        private void InitializeComponent()
        {
            // Form settings
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowInTaskbar = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.DoubleBuffered = true;
            this.BackColor = Color.White;
            this.Size = new Size(400, 200);

            // Enable transparency
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            // Initialize defaults
            _config = new DialogConfig();
            _theme = BeepThemesManager.CurrentTheme;
            _style = BeepStyling.CurrentControlStyle;
            _painter = DialogPainterFactory.GetDefaultPainter();
            _buttonRects = Array.Empty<Rectangle>();
        }

        #endregion

        #region Configuration

        private void ApplyConfig()
        {
            if (_config == null)
                return;

            // Apply style
            _style = _config.Style;
            _painter = DialogPainterFactory.CreatePainter(_config);

            // Calculate size
            CalculateSize();
            CalculateLayout();

            // Apply position
            ApplyPosition();

            Invalidate();
        }

        private void CalculateSize()
        {
            using var g = CreateGraphics();

            int width = Math.Max(_config.MinWidth, 400);
            int height = PADDING * 2;

            // Title height
            if (!string.IsNullOrEmpty(_config.Title))
            {
                var titleFont = _config.TitleFont ?? new Font("Segoe UI", 14, FontStyle.Bold);
                var titleSize = g.MeasureString(_config.Title, titleFont, width - PADDING * 2 - (_config.ShowIcon ? ICON_SIZE + 16 : 0));
                height += (int)titleSize.Height + 8;
            }

            // Message height
            if (!string.IsNullOrEmpty(_config.Message))
            {
                var messageFont = _config.MessageFont ?? new Font("Segoe UI", 10, FontStyle.Regular);
                var messageSize = g.MeasureString(_config.Message, messageFont, width - PADDING * 2 - (_config.ShowIcon ? ICON_SIZE + 16 : 0));
                height += (int)messageSize.Height + 16;
            }

            // Details height
            if (!string.IsNullOrEmpty(_config.Details))
            {
                var detailsFont = _config.DetailsFont ?? new Font("Segoe UI", 9, FontStyle.Regular);
                var detailsSize = g.MeasureString(_config.Details, detailsFont, width - PADDING * 2);
                height += Math.Min((int)detailsSize.Height, 100) + 16;
            }

            // Custom control height
            if (_config.CustomControl != null)
            {
                height += Math.Max(_config.CustomControlMinHeight, _config.CustomControl.Height) + _config.CustomControlPadding * 2;
            }

            // Button area height
            if (_config.Buttons != null && _config.Buttons.Length > 0)
            {
                height += BUTTON_HEIGHT + PADDING;
            }

            // Icon affects minimum height
            if (_config.ShowIcon)
            {
                height = Math.Max(height, PADDING * 2 + ICON_SIZE + BUTTON_HEIGHT + PADDING);
            }

            // Apply custom size if specified
            if (_config.CustomSize.HasValue)
            {
                this.Size = _config.CustomSize.Value;
            }
            else
            {
                this.Size = new Size(
                    Math.Min(Math.Max(width, _config.MinWidth), _config.MaxWidth),
                    height
                );
            }
        }

        private void CalculateLayout()
        {
            int x = PADDING;
            int y = PADDING;
            int contentWidth = Width - PADDING * 2;

            // Close button
            if (_config.ShowCloseButton)
            {
                _closeButtonRect = new Rectangle(Width - CLOSE_BUTTON_SIZE - 8, 8, CLOSE_BUTTON_SIZE, CLOSE_BUTTON_SIZE);
            }

            // Icon
            if (_config.ShowIcon)
            {
                _iconRect = new Rectangle(x, y, ICON_SIZE, ICON_SIZE);
                x += ICON_SIZE + 16;
                contentWidth -= ICON_SIZE + 16;
            }

            // Title
            if (!string.IsNullOrEmpty(_config.Title))
            {
                using var g = CreateGraphics();
                var titleFont = _config.TitleFont ?? new Font("Segoe UI", 14, FontStyle.Bold);
                var titleSize = g.MeasureString(_config.Title, titleFont, contentWidth);
                _titleRect = new Rectangle(x, y, contentWidth, (int)titleSize.Height);
                y += (int)titleSize.Height + 8;
            }

            // Message
            if (!string.IsNullOrEmpty(_config.Message))
            {
                using var g = CreateGraphics();
                var messageFont = _config.MessageFont ?? new Font("Segoe UI", 10, FontStyle.Regular);
                var messageSize = g.MeasureString(_config.Message, messageFont, contentWidth);
                _messageRect = new Rectangle(x, y, contentWidth, (int)messageSize.Height);
                y += (int)messageSize.Height + 16;
            }

            // Details
            if (!string.IsNullOrEmpty(_config.Details))
            {
                using var g = CreateGraphics();
                var detailsFont = _config.DetailsFont ?? new Font("Segoe UI", 9, FontStyle.Regular);
                var detailsSize = g.MeasureString(_config.Details, detailsFont, Width - PADDING * 2);
                int detailsHeight = Math.Min((int)detailsSize.Height, 100);
                _detailsRect = new Rectangle(PADDING, y, Width - PADDING * 2, detailsHeight);
                y += detailsHeight + 16;
            }

            // Custom control
            if (_config.CustomControl != null)
            {
                int controlHeight = Math.Max(_config.CustomControlMinHeight, _config.CustomControl.Height);
                _customControlRect = new Rectangle(
                    PADDING + _config.CustomControlPadding,
                    y + _config.CustomControlPadding,
                    Width - PADDING * 2 - _config.CustomControlPadding * 2,
                    controlHeight
                );

                // Add the control
                _config.CustomControl.Location = _customControlRect.Location;
                _config.CustomControl.Size = _customControlRect.Size;
                if (!this.Controls.Contains(_config.CustomControl))
                {
                    this.Controls.Add(_config.CustomControl);
                }

                y += controlHeight + _config.CustomControlPadding * 2;
            }

            // Button area
            if (_config.Buttons != null && _config.Buttons.Length > 0)
            {
                _buttonAreaRect = new Rectangle(PADDING, Height - PADDING - BUTTON_HEIGHT, Width - PADDING * 2, BUTTON_HEIGHT);

                // Calculate button rectangles (right-aligned)
                var buttons = _config.ButtonOrder ?? _config.Buttons;
                _buttonRects = new Rectangle[buttons.Length];

                int totalButtonWidth = 0;
                using var g = CreateGraphics();
                var buttonFont = _config.ButtonFont ?? new Font("Segoe UI", 10, FontStyle.Regular);

                // Measure buttons
                int[] buttonWidths = new int[buttons.Length];
                for (int i = 0; i < buttons.Length; i++)
                {
                    string text = GetButtonText(buttons[i]);
                    var textSize = g.MeasureString(text, buttonFont);
                    buttonWidths[i] = Math.Max(BUTTON_MIN_WIDTH, (int)textSize.Width + 32);
                    totalButtonWidth += buttonWidths[i];
                }
                totalButtonWidth += (buttons.Length - 1) * BUTTON_SPACING;

                // Position buttons (right-aligned)
                int buttonX = _buttonAreaRect.Right - totalButtonWidth;
                for (int i = 0; i < buttons.Length; i++)
                {
                    _buttonRects[i] = new Rectangle(buttonX, _buttonAreaRect.Y, buttonWidths[i], BUTTON_HEIGHT);
                    buttonX += buttonWidths[i] + BUTTON_SPACING;
                }
            }
        }

        private void ApplyPosition()
        {
            switch (_config.Position)
            {
                case DialogPosition.CenterParent:
                    this.StartPosition = FormStartPosition.CenterParent;
                    break;
                case DialogPosition.CenterScreen:
                    this.StartPosition = FormStartPosition.CenterScreen;
                    break;
                case DialogPosition.Custom when _config.CustomLocation.HasValue:
                    this.StartPosition = FormStartPosition.Manual;
                    this.Location = _config.CustomLocation.Value;
                    break;
                default:
                    this.StartPosition = FormStartPosition.CenterParent;
                    break;
            }
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = new Rectangle(0, 0, Width, Height);

            // Paint using painter
            if (_painter != null)
            {
                _painter.Paint(g, bounds, _config, _theme);
            }
            else
            {
                // Fallback painting
                PaintFallback(g, bounds);
            }

            // Paint buttons with hover/pressed states
            PaintButtons(g);

            // Paint close button
            if (_config.ShowCloseButton)
            {
                PaintCloseButton(g);
            }
        }

        private void PaintFallback(Graphics g, Rectangle bounds)
        {
            int radius = StyleBorders.GetRadius(_style);

            // Background
            using (var path = GraphicsExtensions.GetRoundedRectPath(bounds, radius))
            using (var brush = new SolidBrush(_theme?.DialogBackColor ?? Color.White))
            {
                g.FillPath(brush, path);
            }

            // Border
            using (var path = GraphicsExtensions.GetRoundedRectPath(bounds, radius))
            using (var pen = new Pen(_theme?.BorderColor ?? Color.FromArgb(229, 231, 235), 1))
            {
                g.DrawPath(pen, path);
            }

            // Icon
            if (_config.ShowIcon && !_iconRect.IsEmpty)
            {
                var iconPath = GetIconPath(_config.IconType);
                if (!string.IsNullOrEmpty(iconPath))
                {
                    try
                    {
                        var iconColor = GetIconColor(_config.IconType);
                        StyledImagePainter.PaintWithTint(g, _iconRect, iconPath, iconColor, 0.9f, 8);
                    }
                    catch
                    {
                        // Draw fallback icon
                        using var brush = new SolidBrush(GetIconColor(_config.IconType));
                        g.FillEllipse(brush, _iconRect);
                    }
                }
            }

            // Title
            if (!string.IsNullOrEmpty(_config.Title) && !_titleRect.IsEmpty)
            {
                var titleFont = _config.TitleFont ?? new Font("Segoe UI", 14, FontStyle.Bold);
                using var brush = new SolidBrush(_theme?.DialogForeColor ?? Color.FromArgb(17, 24, 39));
                g.DrawString(_config.Title, titleFont, brush, _titleRect);
            }

            // Message
            if (!string.IsNullOrEmpty(_config.Message) && !_messageRect.IsEmpty)
            {
                var messageFont = _config.MessageFont ?? new Font("Segoe UI", 10, FontStyle.Regular);
                using var brush = new SolidBrush(Color.FromArgb(180, _theme?.DialogForeColor ?? Color.FromArgb(55, 65, 81)));
                using var sf = new StringFormat { Trimming = StringTrimming.Word };
                g.DrawString(_config.Message, messageFont, brush, _messageRect, sf);
            }

            // Details
            if (!string.IsNullOrEmpty(_config.Details) && !_detailsRect.IsEmpty)
            {
                var detailsFont = _config.DetailsFont ?? new Font("Segoe UI", 9, FontStyle.Regular);
                using var brush = new SolidBrush(Color.FromArgb(140, _theme?.DialogForeColor ?? Color.FromArgb(107, 114, 128)));
                using var sf = new StringFormat { Trimming = StringTrimming.Word };
                g.DrawString(_config.Details, detailsFont, brush, _detailsRect, sf);
            }
        }

        private void PaintButtons(Graphics g)
        {
            if (_buttonRects == null || _buttonRects.Length == 0)
                return;

            var buttons = _config.ButtonOrder ?? _config.Buttons ?? Array.Empty<BeepDialogButtons>();
            var buttonFont = _config.ButtonFont ?? new Font("Segoe UI", 10, FontStyle.Regular);

            for (int i = 0; i < Math.Min(buttons.Length, _buttonRects.Length); i++)
            {
                var rect = _buttonRects[i];
                var button = buttons[i];
                bool isPrimary = i == buttons.Length - 1;
                bool isHovered = i == _hoveredButtonIndex;
                bool isPressed = i == _pressedButtonIndex;

                PaintButton(g, rect, button, isPrimary, isHovered, isPressed, buttonFont);
            }
        }

        private void PaintButton(Graphics g, Rectangle rect, BeepDialogButtons button, bool isPrimary, bool isHovered, bool isPressed, Font font)
        {
            int radius = 8;

            // Determine colors based on state and type
            Color bgColor, fgColor, borderColor;

            if (isPrimary)
            {
                bgColor = GetPrimaryButtonColor();
                fgColor = Color.White;
                borderColor = bgColor;

                if (isPressed)
                    bgColor = DarkenColor(bgColor, 0.15f);
                else if (isHovered)
                    bgColor = LightenColor(bgColor, 0.1f);
            }
            else
            {
                bgColor = isPressed ? Color.FromArgb(229, 231, 235) :
                          isHovered ? Color.FromArgb(243, 244, 246) :
                          Color.White;
                fgColor = Color.FromArgb(55, 65, 81);
                borderColor = Color.FromArgb(209, 213, 219);
            }

            // Draw button
            using (var path = GraphicsExtensions.GetRoundedRectPath(rect, radius))
            {
                // Background
                using (var brush = new SolidBrush(bgColor))
                {
                    g.FillPath(brush, path);
                }

                // Border
                using (var pen = new Pen(borderColor, 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            // Text
            string text = GetButtonText(button);
            using (var brush = new SolidBrush(fgColor))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(text, font, brush, rect, sf);
            }
        }

        private void PaintCloseButton(Graphics g)
        {
            if (_closeButtonRect.IsEmpty)
                return;

            // Background on hover
            if (_closeButtonHovered)
            {
                using (var brush = new SolidBrush(Color.FromArgb(254, 226, 226)))
                using (var path = GraphicsExtensions.GetRoundedRectPath(_closeButtonRect, 6))
                {
                    g.FillPath(brush, path);
                }
            }

            // X icon
            var color = _closeButtonHovered ? Color.FromArgb(220, 38, 38) : Color.FromArgb(156, 163, 175);
            using (var pen = new Pen(color, 2))
            {
                int padding = 10;
                g.DrawLine(pen,
                    _closeButtonRect.Left + padding, _closeButtonRect.Top + padding,
                    _closeButtonRect.Right - padding, _closeButtonRect.Bottom - padding);
                g.DrawLine(pen,
                    _closeButtonRect.Right - padding, _closeButtonRect.Top + padding,
                    _closeButtonRect.Left + padding, _closeButtonRect.Bottom - padding);
            }
        }

        #endregion

        #region Mouse Events

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Dragging
            if (_isDragging && _config.AllowDrag)
            {
                this.Location = new Point(
                    e.X + this.Left - _dragOffset.X,
                    e.Y + this.Top - _dragOffset.Y
                );
                return;
            }

            // Button hover
            int oldHovered = _hoveredButtonIndex;
            _hoveredButtonIndex = -1;

            if (_buttonRects != null)
            {
                for (int i = 0; i < _buttonRects.Length; i++)
                {
                    if (_buttonRects[i].Contains(e.Location))
                    {
                        _hoveredButtonIndex = i;
                        break;
                    }
                }
            }

            // Close button hover
            bool oldCloseHovered = _closeButtonHovered;
            _closeButtonHovered = _closeButtonRect.Contains(e.Location);

            if (oldHovered != _hoveredButtonIndex || oldCloseHovered != _closeButtonHovered)
            {
                Invalidate();
            }

            // Cursor
            this.Cursor = (_hoveredButtonIndex >= 0 || _closeButtonHovered) ? Cursors.Hand : Cursors.Default;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                // Button press
                if (_hoveredButtonIndex >= 0)
                {
                    _pressedButtonIndex = _hoveredButtonIndex;
                    Invalidate();
                }
                // Start drag
                else if (_config.AllowDrag && e.Y < 50)
                {
                    _isDragging = true;
                    _dragOffset = e.Location;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _isDragging = false;

            if (e.Button == MouseButtons.Left)
            {
                // Close button click
                if (_closeButtonHovered && _config.ShowCloseButton)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    this.Close();
                    return;
                }

                // Button click
                if (_pressedButtonIndex >= 0 && _pressedButtonIndex == _hoveredButtonIndex)
                {
                    HandleButtonClick(_pressedButtonIndex);
                }

                _pressedButtonIndex = -1;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (_hoveredButtonIndex >= 0 || _closeButtonHovered)
            {
                _hoveredButtonIndex = -1;
                _closeButtonHovered = false;
                Invalidate();
            }
        }

        #endregion

        #region Keyboard Events

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                // Click default/primary button
                if (_buttonRects != null && _buttonRects.Length > 0)
                {
                    HandleButtonClick(_buttonRects.Length - 1); // Primary is last
                }
                return true;
            }

            if (keyData == Keys.Escape && _config.CloseOnEscape)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region Button Handling

        private void HandleButtonClick(int buttonIndex)
        {
            var buttons = _config.ButtonOrder ?? _config.Buttons ?? Array.Empty<BeepDialogButtons>();
            if (buttonIndex < 0 || buttonIndex >= buttons.Length)
                return;

            var button = buttons[buttonIndex];

            // Map button to dialog result
            this.DialogResult = button switch
            {
                BeepDialogButtons.Ok => System.Windows.Forms.DialogResult.OK,
                BeepDialogButtons.Cancel => System.Windows.Forms.DialogResult.Cancel,
                BeepDialogButtons.Yes => System.Windows.Forms.DialogResult.Yes,
                BeepDialogButtons.No => System.Windows.Forms.DialogResult.No,
                BeepDialogButtons.Abort => System.Windows.Forms.DialogResult.Abort,
                BeepDialogButtons.Retry => System.Windows.Forms.DialogResult.Retry,
                BeepDialogButtons.Ignore => System.Windows.Forms.DialogResult.Ignore,
                _ => System.Windows.Forms.DialogResult.OK
            };

            this.Close();
        }

        private string GetButtonText(BeepDialogButtons button)
        {
            // Check for custom label
            if (_config.CustomButtonLabels != null && _config.CustomButtonLabels.TryGetValue(button, out var customLabel))
            {
                return customLabel;
            }

            return button switch
            {
                BeepDialogButtons.Ok => "OK",
                BeepDialogButtons.Cancel => "Cancel",
                BeepDialogButtons.Yes => "Yes",
                BeepDialogButtons.No => "No",
                BeepDialogButtons.Abort => "Abort",
                BeepDialogButtons.Retry => "Retry",
                BeepDialogButtons.Ignore => "Ignore",
                BeepDialogButtons.Close => "Close",
                BeepDialogButtons.Help => "Help",
                _ => button.ToString()
            };
        }

        #endregion

        #region Helper Methods

        private string GetIconPath(BeepDialogIcon icon)
        {
            return icon switch
            {
                BeepDialogIcon.Information => "TheTechIdea.Beep.Winform.GFX.SVG.information.svg",
                BeepDialogIcon.Warning => "TheTechIdea.Beep.Winform.GFX.SVG.warning.svg",
                BeepDialogIcon.Error => "TheTechIdea.Beep.Winform.GFX.SVG.error.svg",
                BeepDialogIcon.Question => "TheTechIdea.Beep.Winform.GFX.SVG.question.svg",
                BeepDialogIcon.Success => "TheTechIdea.Beep.Winform.GFX.SVG.check.svg",
                _ => "TheTechIdea.Beep.Winform.GFX.SVG.information.svg"
            };
        }

        private Color GetIconColor(BeepDialogIcon icon)
        {
            return icon switch
            {
                BeepDialogIcon.Information => Color.FromArgb(59, 130, 246),
                BeepDialogIcon.Warning => Color.FromArgb(245, 158, 11),
                BeepDialogIcon.Error => Color.FromArgb(239, 68, 68),
                BeepDialogIcon.Question => Color.FromArgb(139, 92, 246),
                BeepDialogIcon.Success => Color.FromArgb(34, 197, 94),
                _ => Color.FromArgb(107, 114, 128)
            };
        }

        private Color GetPrimaryButtonColor()
        {
            if (_config.Preset != DialogPreset.None)
            {
                return _config.Preset switch
                {
                    DialogPreset.Success => Color.FromArgb(34, 197, 94),
                    DialogPreset.Danger => Color.FromArgb(239, 68, 68),
                    DialogPreset.Warning => Color.FromArgb(245, 158, 11),
                    DialogPreset.Question => Color.FromArgb(59, 130, 246),
                    _ => Color.FromArgb(59, 130, 246)
                };
            }

            return _theme?.DialogOkButtonBackColor ?? Color.FromArgb(59, 130, 246);
        }

        private Color LightenColor(Color color, float amount)
        {
            return Color.FromArgb(
                color.A,
                Math.Min(255, (int)(color.R + (255 - color.R) * amount)),
                Math.Min(255, (int)(color.G + (255 - color.G) * amount)),
                Math.Min(255, (int)(color.B + (255 - color.B) * amount))
            );
        }

        private Color DarkenColor(Color color, float amount)
        {
            return Color.FromArgb(
                color.A,
                Math.Max(0, (int)(color.R * (1 - amount))),
                Math.Max(0, (int)(color.G * (1 - amount))),
                Math.Max(0, (int)(color.B * (1 - amount)))
            );
        }

        #endregion
    }
}

