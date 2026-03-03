using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Common;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Models;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Painters;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Helpers;
using TheTechIdea.Beep.Winform.Controls.FontManagement;
using TheTechIdea.Beep.Winform.Controls.Helpers;
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
        private Rectangle _dragHandleRect;
        private readonly List<Control> _focusableControls = new();
        private int _focusedButtonIndex = -1;
        private Panel? _messageScrollPanel;
        private Label? _messageScrollLabel;
        private float[] _buttonHoverProgress = Array.Empty<float>();
        private float[] _buttonPressProgress = Array.Empty<float>();
        private float _closeHoverProgress;
        private Timer? _microInteractionTimer;

        // Constants
        private const int PADDING = 24;
        private const int ICON_SIZE = 48;
        private const int BUTTON_HEIGHT = 40;
        private const int BUTTON_MIN_WIDTH = 100;
        private const int BUTTON_SPACING = 12;
        private const int CLOSE_BUTTON_SIZE = 32;
        private const int DRAG_HANDLE_HEIGHT = 8;

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
            this.KeyPreview = true;
            this.AccessibleRole = AccessibleRole.Dialog;

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
            _microInteractionTimer = new Timer { Interval = 16 };
            _microInteractionTimer.Tick += (_, _) => AdvanceMicroInteractions();
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
            ApplyAccessibilityMetadata();
            BuildScrollableMessageIfNeeded();

            // Apply position
            ApplyPosition();

            Invalidate();
        }

        private void CalculateSize()
        {
            using var g = CreateGraphics();

            int padding = Scale(PADDING);
            int iconSize = ResolveIconSize();
            int width = Math.Max(_config.MinWidth, Scale(400));
            int height = padding * 2;

            // Title height
            if (!string.IsNullOrEmpty(_config.Title))
            {
                var titleFont = GetTitleFont();
                var titleSize = g.MeasureString(_config.Title, titleFont, width - padding * 2 - (_config.ShowIcon ? iconSize + Scale(16) : 0));
                height += (int)titleSize.Height + 8;
            }

            // Message height
            if (!string.IsNullOrEmpty(_config.Message))
            {
                var messageFont = GetMessageFont();
                var messageSize = g.MeasureString(_config.Message, messageFont, width - padding * 2 - (_config.ShowIcon ? iconSize + Scale(16) : 0));
                height += (int)messageSize.Height + 16;
            }

            // Details height
            if (!string.IsNullOrEmpty(_config.Details))
            {
                var detailsFont = GetDetailsFont();
                var detailsSize = g.MeasureString(_config.Details, detailsFont, width - padding * 2);
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
                height += Scale(BUTTON_HEIGHT) + padding;
            }

            // Icon affects minimum height
            if (_config.ShowIcon)
            {
                height = Math.Max(height, padding * 2 + iconSize + Scale(BUTTON_HEIGHT) + padding);
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
            int padding = Scale(PADDING);
            int iconSize = ResolveIconSize();
            int buttonHeight = Scale(BUTTON_HEIGHT);
            int minButtonWidth = Scale(BUTTON_MIN_WIDTH);
            int buttonSpacing = Scale(BUTTON_SPACING);
            int closeButtonSize = Scale(CLOSE_BUTTON_SIZE);
            int x = padding;
            int y = padding;
            int contentWidth = Width - padding * 2;
            _dragHandleRect = new Rectangle(padding, Scale(6), Math.Max(24, Width - (padding * 2)), Scale(DRAG_HANDLE_HEIGHT));

            // Close button
            if (_config.ShowCloseButton)
            {
                _closeButtonRect = new Rectangle(Width - closeButtonSize - Scale(8), Scale(8), closeButtonSize, closeButtonSize);
            }

            // Icon
            if (_config.ShowIcon)
            {
                _iconRect = new Rectangle(x, y, iconSize, iconSize);
                x += iconSize + Scale(16);
                contentWidth -= iconSize + Scale(16);
            }

            // Title
            if (!string.IsNullOrEmpty(_config.Title))
            {
                using var g = CreateGraphics();
                var titleFont = GetTitleFont();
                var titleSize = g.MeasureString(_config.Title, titleFont, contentWidth);
                _titleRect = new Rectangle(x, y, contentWidth, (int)titleSize.Height);
                y += (int)titleSize.Height + 8;
            }

            // Message
            if (!string.IsNullOrEmpty(_config.Message))
            {
                using var g = CreateGraphics();
                var messageFont = GetMessageFont();
                var messageSize = g.MeasureString(_config.Message, messageFont, contentWidth);
                _messageRect = new Rectangle(x, y, contentWidth, (int)messageSize.Height);
                y += (int)messageSize.Height + 16;
            }

            // Details
            if (!string.IsNullOrEmpty(_config.Details))
            {
                using var g = CreateGraphics();
                var detailsFont = GetDetailsFont();
                var detailsSize = g.MeasureString(_config.Details, detailsFont, Width - PADDING * 2);
                int detailsHeight = Math.Min((int)detailsSize.Height, 100);
                _detailsRect = new Rectangle(padding, y, Width - padding * 2, detailsHeight);
                y += detailsHeight + 16;
            }

            // Custom control
            if (_config.CustomControl != null)
            {
                int controlHeight = Math.Max(_config.CustomControlMinHeight, _config.CustomControl.Height);
                _customControlRect = new Rectangle(
                    padding + Scale(_config.CustomControlPadding),
                    y + Scale(_config.CustomControlPadding),
                    Width - padding * 2 - Scale(_config.CustomControlPadding) * 2,
                    controlHeight
                );

                // Add the control
                _config.CustomControl.Location = _customControlRect.Location;
                _config.CustomControl.Size = _customControlRect.Size;
                if (!this.Controls.Contains(_config.CustomControl))
                {
                    this.Controls.Add(_config.CustomControl);
                }

                y += controlHeight + Scale(_config.CustomControlPadding) * 2;
            }

            // Button area
            if (_config.Buttons != null && _config.Buttons.Length > 0)
            {
                _buttonAreaRect = new Rectangle(padding, Height - padding - buttonHeight, Width - padding * 2, buttonHeight);

                // Calculate button rectangles (right-aligned)
                var buttons = _config.ButtonOrder ?? _config.Buttons;
                _buttonRects = new Rectangle[buttons.Length];
                EnsureMicroInteractionArrays(buttons.Length);

                int totalButtonWidth = 0;
                using var g = CreateGraphics();
                var buttonFont = GetButtonFont();

                // Measure buttons
                int[] buttonWidths = new int[buttons.Length];
                for (int i = 0; i < buttons.Length; i++)
                {
                    string text = GetButtonText(buttons[i]);
                    var textSize = g.MeasureString(text, buttonFont);
                    buttonWidths[i] = Math.Max(minButtonWidth, (int)textSize.Width + Scale(32));
                    totalButtonWidth += buttonWidths[i];
                }
                totalButtonWidth += (buttons.Length - 1) * buttonSpacing;

                int buttonX = _config.ButtonLayout switch
                {
                    DialogButtonLayout.HorizontalCenter => _buttonAreaRect.Left + (_buttonAreaRect.Width - totalButtonWidth) / 2,
                    DialogButtonLayout.HorizontalLeft => _buttonAreaRect.Left,
                    DialogButtonLayout.Spread => _buttonAreaRect.Left,
                    _ => _buttonAreaRect.Right - totalButtonWidth
                };
                int spreadGap = buttonSpacing;
                if (_config.ButtonLayout == DialogButtonLayout.Spread && buttons.Length > 1)
                {
                    spreadGap = Math.Max(buttonSpacing, (_buttonAreaRect.Width - totalButtonWidth) / (buttons.Length - 1) + buttonSpacing);
                }
                for (int i = 0; i < buttons.Length; i++)
                {
                    _buttonRects[i] = new Rectangle(buttonX, _buttonAreaRect.Y, buttonWidths[i], buttonHeight);
                    buttonX += buttonWidths[i] + spreadGap;
                }
            }
        }

        private void ApplyPosition()
        {
            var owner = Owner;
            if (_config.PlacementStrategy != DialogPlacementStrategy.CenterOwner)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = DialogPlacementEngine.Place(owner, this.Size, _config.PlacementStrategy);
                return;
            }

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
                case DialogPosition.BottomRight:
                case DialogPosition.BottomLeft:
                case DialogPosition.BottomCenter:
                    if (owner != null)
                    {
                        this.StartPosition = FormStartPosition.Manual;
                        int y = owner.Bottom - Height - Scale(16);
                        int x = _config.Position switch
                        {
                            DialogPosition.BottomLeft => owner.Left + Scale(16),
                            DialogPosition.BottomRight => owner.Right - Width - Scale(16),
                            _ => owner.Left + ((owner.Width - Width) / 2)
                        };
                        this.Location = new Point(x, y);
                    }
                    else
                    {
                        this.StartPosition = FormStartPosition.CenterScreen;
                    }
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

            if (_config.AllowDrag && !_dragHandleRect.IsEmpty)
            {
                PaintDragHandle(g);
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
                var titleFont = GetTitleFont();
                using var brush = new SolidBrush(_theme?.DialogForeColor ?? Color.FromArgb(17, 24, 39));
                g.DrawString(_config.Title, titleFont, brush, _titleRect);
            }

            // Message
            if (!string.IsNullOrEmpty(_config.Message) && !_messageRect.IsEmpty)
            {
                var messageFont = GetMessageFont();
                using var brush = new SolidBrush(Color.FromArgb(180, _theme?.DialogForeColor ?? Color.FromArgb(55, 65, 81)));
                using var sf = new StringFormat { Trimming = StringTrimming.Word };
                g.DrawString(_config.Message, messageFont, brush, _messageRect, sf);
            }

            // Details
            if (!string.IsNullOrEmpty(_config.Details) && !_detailsRect.IsEmpty)
            {
                var detailsFont = GetDetailsFont();
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
            var buttonFont = GetButtonFont();

            for (int i = 0; i < Math.Min(buttons.Length, _buttonRects.Length); i++)
            {
                var rect = _buttonRects[i];
                var button = buttons[i];
                bool isPrimary = i == buttons.Length - 1;
                bool isHovered = i == _hoveredButtonIndex;
                bool isPressed = i == _pressedButtonIndex;

                PaintButton(g, i, rect, button, isPrimary, isHovered || _focusedButtonIndex == i, isPressed, buttonFont);
            }
        }

        private void PaintButton(Graphics g, int buttonIndex, Rectangle rect, BeepDialogButtons button, bool isPrimary, bool isHovered, bool isPressed, Font font)
        {
            int radius = Scale(8);
            float hover = buttonIndex >= 0 && buttonIndex < _buttonHoverProgress.Length ? _buttonHoverProgress[buttonIndex] : 0f;
            float press = buttonIndex >= 0 && buttonIndex < _buttonPressProgress.Length ? _buttonPressProgress[buttonIndex] : 0f;
            int depthOffset = (int)Math.Round(Scale(3) * press);
            rect = new Rectangle(rect.X, rect.Y + depthOffset, rect.Width, rect.Height);

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
                    bgColor = LightenColor(bgColor, 0.1f + (0.08f * hover));
            }
            else
            {
                bgColor = isPressed ? Color.FromArgb(229, 231, 235) :
                          isHovered ? Color.FromArgb(243, 244, 246) :
                          Color.White;
                fgColor = Color.FromArgb(55, 65, 81);
                borderColor = Color.FromArgb(209, 213, 219);
                if (hover > 0f)
                {
                    bgColor = LightenColor(bgColor, 0.06f * hover);
                }
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

                if (isHovered || hover > 0f)
                {
                    int alpha = (int)Math.Round(50 + (120 * hover));
                    using var focusPen = new Pen(Color.FromArgb(Math.Max(0, Math.Min(255, alpha)), _theme?.AccentColor ?? Color.DodgerBlue), 2);
                    g.DrawPath(focusPen, path);
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
            if (_closeButtonHovered || _closeHoverProgress > 0f)
            {
                int a = (int)Math.Round(120 * _closeHoverProgress);
                using (var brush = new SolidBrush(Color.FromArgb(a, 254, 226, 226)))
                using (var path = GraphicsExtensions.GetRoundedRectPath(_closeButtonRect, 6))
                {
                    g.FillPath(brush, path);
                }
            }

            // X icon
            var color = _closeButtonHovered || _closeHoverProgress > 0.05f
                ? Color.FromArgb(220, 38, 38)
                : Color.FromArgb(156, 163, 175);
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

        private void PaintDragHandle(Graphics g)
        {
            using var path = GraphicsExtensions.GetRoundedRectPath(_dragHandleRect, Scale(4));
            using var brush = new SolidBrush(Color.FromArgb(60, _theme?.BorderColor ?? Color.Gray));
            g.FillPath(brush, path);
        }

        #endregion

        #region Mouse Events

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Dragging
            if (_isDragging && _config.AllowDrag)
            {
                var proposed = new Point(
                    e.X + this.Left - _dragOffset.X,
                    e.Y + this.Top - _dragOffset.Y
                );
                this.Location = ApplySnapToOwnerEdges(proposed);
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
                StartMicroInteractionTimer();
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
                    StartMicroInteractionTimer();
                    Invalidate();
                }
                // Start drag
                else if (_config.AllowDrag && _dragHandleRect.Contains(e.Location))
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
                StartMicroInteractionTimer();
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
                StartMicroInteractionTimer();
                Invalidate();
            }
        }

        #endregion

        #region Keyboard Events

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab || keyData == (Keys.Shift | Keys.Tab))
            {
                MoveFocus(keyData == (Keys.Shift | Keys.Tab) ? -1 : 1);
                return true;
            }

            if (keyData == Keys.Left || keyData == Keys.Right)
            {
                MoveButtonFocus(keyData == Keys.Left ? -1 : 1);
                return true;
            }

            if (keyData == Keys.Enter)
            {
                if (_focusedButtonIndex >= 0)
                {
                    HandleButtonClick(_focusedButtonIndex);
                    return true;
                }

                // Click primary button by default
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

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            BuildFocusableControls();
            FocusPrimaryOrFirstControl();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (_messageScrollPanel != null)
            {
                Controls.Remove(_messageScrollPanel);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_microInteractionTimer != null)
                {
                    _microInteractionTimer.Stop();
                    _microInteractionTimer.Dispose();
                    _microInteractionTimer = null;
                }
                if (_messageScrollPanel != null)
                {
                    _messageScrollPanel.Dispose();
                    _messageScrollPanel = null;
                }
                if (_messageScrollLabel != null)
                {
                    _messageScrollLabel.Dispose();
                    _messageScrollLabel = null;
                }
            }
            base.Dispose(disposing);
        }

        private string GetIconPath(BeepDialogIcon icon)
        {
            return icon switch
            {
                BeepDialogIcon.Information => Svgs.Information,
                BeepDialogIcon.Warning => Svgs.InfoWarning,
                BeepDialogIcon.Error => Svgs.Error,
                BeepDialogIcon.Question => Svgs.Question,
                BeepDialogIcon.Success => Svgs.CheckCircle,
                _ => Svgs.Information
            };
        }

        private Color GetIconColor(BeepDialogIcon icon)
        {
            return icon switch
            {
                BeepDialogIcon.Information => _theme?.AccentColor ?? Color.FromArgb(59, 130, 246),
                BeepDialogIcon.Warning => _theme?.WarningColor ?? Color.FromArgb(245, 158, 11),
                BeepDialogIcon.Error => _theme?.ErrorColor ?? Color.FromArgb(239, 68, 68),
                BeepDialogIcon.Question => _theme?.AccentColor ?? Color.FromArgb(139, 92, 246),
                BeepDialogIcon.Success => _theme?.SuccessColor ?? Color.FromArgb(34, 197, 94),
                _ => Color.FromArgb(107, 114, 128)
            };
        }

        private Font GetTitleFont()
        {
            if (_config.TitleFont != null)
                return _config.TitleFont;
            if (_theme?.TitleStyle != null)
                return BeepThemesManager.ToFont(_theme.TitleStyle);

            return BeepFontManager.GetCachedFont(BeepFontManager.DefaultFontName, 14f, FontStyle.Bold);
        }

        private Font GetMessageFont()
        {
            if (_config.MessageFont != null)
                return _config.MessageFont;
            if (_theme?.BodyStyle != null)
                return BeepThemesManager.ToFont(_theme.BodyStyle);

            return BeepFontManager.GetCachedFont(BeepFontManager.DefaultFontName, 10f, FontStyle.Regular);
        }

        private Font GetDetailsFont()
        {
            if (_config.DetailsFont != null)
                return _config.DetailsFont;
            if (_theme?.CaptionStyle != null)
                return BeepThemesManager.ToFont(_theme.CaptionStyle);

            return BeepFontManager.GetCachedFont(BeepFontManager.DefaultFontName, 9f, FontStyle.Regular);
        }

        private Font GetButtonFont()
        {
            if (_config.ButtonFont != null)
                return _config.ButtonFont;
            if (_theme?.DialogOkButtonFont != null)
                return BeepThemesManager.ToFont(_theme.DialogOkButtonFont);
            if (_theme?.ButtonStyle != null)
                return BeepThemesManager.ToFont(_theme.ButtonStyle);

            return BeepFontManager.GetCachedFont(BeepFontManager.DefaultFontName, 10f, FontStyle.Regular);
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

        private void EnsureMicroInteractionArrays(int buttonCount)
        {
            if (_buttonHoverProgress.Length == buttonCount && _buttonPressProgress.Length == buttonCount)
            {
                return;
            }

            _buttonHoverProgress = new float[buttonCount];
            _buttonPressProgress = new float[buttonCount];
        }

        private void StartMicroInteractionTimer()
        {
            _microInteractionTimer?.Start();
        }

        private void AdvanceMicroInteractions()
        {
            bool changed = false;
            const float speed = 0.22f;

            for (int i = 0; i < _buttonHoverProgress.Length; i++)
            {
                float hoverTarget = (_hoveredButtonIndex == i || _focusedButtonIndex == i) ? 1f : 0f;
                float pressTarget = _pressedButtonIndex == i ? 1f : 0f;

                float nh = Lerp(_buttonHoverProgress[i], hoverTarget, speed);
                float np = Lerp(_buttonPressProgress[i], pressTarget, speed + 0.08f);
                changed |= Math.Abs(nh - _buttonHoverProgress[i]) > 0.001f || Math.Abs(np - _buttonPressProgress[i]) > 0.001f;
                _buttonHoverProgress[i] = nh;
                _buttonPressProgress[i] = np;
            }

            float closeTarget = _closeButtonHovered ? 1f : 0f;
            float nc = Lerp(_closeHoverProgress, closeTarget, speed);
            changed |= Math.Abs(nc - _closeHoverProgress) > 0.001f;
            _closeHoverProgress = nc;

            if (changed)
            {
                Invalidate();
            }
            else
            {
                _microInteractionTimer?.Stop();
            }
        }

        private static float Lerp(float current, float target, float speed)
        {
            return current + ((target - current) * speed);
        }

        private int Scale(int value) => DpiScalingHelper.ScaleValue(value, this);

        private int ResolveIconSize()
        {
            return _config.IconSizePreset switch
            {
                DialogIconSizePreset.Small => Scale(24),
                DialogIconSizePreset.Medium => Scale(32),
                DialogIconSizePreset.Large => Scale(48),
                DialogIconSizePreset.ExtraLarge => Scale(64),
                _ => Scale(Math.Max(16, _config.IconSize))
            };
        }

        private void BuildFocusableControls()
        {
            _focusableControls.Clear();
            foreach (Control control in Controls)
            {
                if (control.Visible && control.Enabled && control.TabStop && control.CanFocus)
                {
                    _focusableControls.Add(control);
                }
            }
        }

        private void FocusPrimaryOrFirstControl()
        {
            if (_focusableControls.Count > 0)
            {
                _focusableControls[0].Focus();
            }

            if (_buttonRects != null && _buttonRects.Length > 0)
            {
                _focusedButtonIndex = _buttonRects.Length - 1;
            }
        }

        private void MoveFocus(int direction)
        {
            BuildFocusableControls();
            if (_focusableControls.Count == 0)
            {
                return;
            }

            int current = _focusableControls.FindIndex(c => c.Focused);
            if (current < 0) current = 0;
            int next = (current + direction + _focusableControls.Count) % _focusableControls.Count;
            _focusableControls[next].Focus();
        }

        private void MoveButtonFocus(int direction)
        {
            if (_buttonRects == null || _buttonRects.Length == 0)
            {
                return;
            }

            if (_focusedButtonIndex < 0)
            {
                _focusedButtonIndex = _buttonRects.Length - 1;
            }

            _focusedButtonIndex = (_focusedButtonIndex + direction + _buttonRects.Length) % _buttonRects.Length;
            Invalidate();
        }

        private void ApplyAccessibilityMetadata()
        {
            AccessibleName = string.IsNullOrWhiteSpace(_config.Title) ? "Dialog" : _config.Title;
            AccessibleDescription = string.IsNullOrWhiteSpace(_config.Message) ? "Dialog content" : _config.Message;
            AccessibilityNotifyClients(AccessibleEvents.SystemForeground, -1);
        }

        private void BuildScrollableMessageIfNeeded()
        {
            if (_messageScrollPanel != null)
            {
                Controls.Remove(_messageScrollPanel);
                _messageScrollPanel.Dispose();
                _messageScrollPanel = null;
                _messageScrollLabel = null;
            }

            if (string.IsNullOrWhiteSpace(_config.Message) || _messageRect.IsEmpty)
            {
                return;
            }

            using var g = CreateGraphics();
            var messageSize = g.MeasureString(_config.Message, GetMessageFont(), _messageRect.Width);
            if (messageSize.Height <= _config.MaxContentHeight)
            {
                return;
            }

            _messageScrollPanel = new Panel
            {
                Location = _messageRect.Location,
                Size = new Size(_messageRect.Width, Math.Max(80, _config.MaxContentHeight)),
                AutoScroll = true,
                BackColor = Color.Transparent
            };
            _messageScrollLabel = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(Math.Max(20, _messageRect.Width - Scale(8)), 0),
                Text = _config.Message,
                ForeColor = Color.FromArgb(180, _theme?.DialogForeColor ?? Color.FromArgb(55, 65, 81))
            };
            _messageScrollPanel.Controls.Add(_messageScrollLabel);
            Controls.Add(_messageScrollPanel);
            _messageRect = Rectangle.Empty;
        }

        private Point ApplySnapToOwnerEdges(Point proposed)
        {
            if (!_config.SnapToOwnerEdges || Owner == null)
            {
                return proposed;
            }

            var snapped = proposed;
            int threshold = Scale(Math.Max(4, _config.SnapThreshold));
            if (Math.Abs(snapped.X - Owner.Left) <= threshold) snapped.X = Owner.Left;
            if (Math.Abs((snapped.X + Width) - Owner.Right) <= threshold) snapped.X = Owner.Right - Width;
            if (Math.Abs(snapped.Y - Owner.Top) <= threshold) snapped.Y = Owner.Top;
            if (Math.Abs((snapped.Y + Height) - Owner.Bottom) <= threshold) snapped.Y = Owner.Bottom - Height;
            return snapped;
        }

        #endregion
    }
}

