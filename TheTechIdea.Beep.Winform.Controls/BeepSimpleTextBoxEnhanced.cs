using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Desktop.Common.Util;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [Description("A modern, feature-rich text box control with advanced capabilities using helper classes.")]
    [DisplayName("Beep Simple TextBox Enhanced")]
    [Category("Beep Controls")]
    public class BeepSimpleTextBoxEnhanced : BeepControl, IBeepTextBox
    {
        #region "Helper Instance"
        
        /// <summary>
        /// The main helper that coordinates all functionality
        /// </summary>
        private BeepSimpleTextBoxHelper _helper;
        
        #endregion
        
        #region "Core Fields"
        
        private BeepImage _beepImage;
        private string _placeholderText = "Enter Text Here";
        private Rectangle _textRect;
        private List<string> _lines = new List<string>();
        private bool _isInitializing = true;
        
        #endregion
        
        #region "Properties - Core Text"
        
        public override string Text
        {
            get => _text;
            set
            {
                if (_isInitializing || (value != null && value.StartsWith("beepSimpleTextBox")))
                {
                    _text = "";
                    return;
                }

                if (_text != value)
                {
                    _text = value ?? "";
                    UpdateLines();
                    _helper?.InvalidateAllCaches();
                    TextChanged?.Invoke(this, EventArgs.Empty);
                    Invalidate();
                }
            }
        }
        
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Placeholder text shown when textbox is empty.")]
        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                _placeholderText = value;
                Invalidate();
            }
        }
        
        #endregion
        
        #region "Properties - Appearance"
        
        private Font _textFont = new Font("Arial", 10);
        [Browsable(true)]
        [MergableProperty(true)]
        [Category("Appearance")]
        [Description("Text Font displayed in the control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont
        {
            get => _textFont;
            set
            {
                if (_textFont != value)
                {
                    _textFont = value;
                    UseThemeFont = false;
                    _helper?.InvalidateAllCaches();
                    Invalidate();
                }
            }
        }
        
        private Color _placeholderTextColor = Color.Gray;
        [Browsable(true)]
        [Category("Appearance")]
        public Color PlaceholderTextColor
        {
            get => _placeholderTextColor;
            set
            {
                _placeholderTextColor = value;
                _currentTheme.TextBoxPlaceholderColor = value;
                Invalidate();
            }
        }
        
        #endregion
        
        #region "Properties - Behavior"
        
        private bool _multiline = false;
        [Browsable(true)]
        [Category("Behavior")]
        public bool Multiline
        {
            get => _multiline;
            set
            {
                _multiline = value;
                if (value)
                {
                    AcceptsReturn = true;
                }
                UpdateLines();
                _helper?.InvalidateAllCaches();
                Invalidate();
            }
        }
        
        private bool _readOnly = false;
        [Browsable(true)]
        [Category("Behavior")]
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                _readOnly = value;
                Invalidate();
            }
        }
        
        private bool _acceptsReturn = false;
        [Browsable(true)]
        [Category("Behavior")]
        public bool AcceptsReturn
        {
            get => _acceptsReturn;
            set
            {
                _acceptsReturn = value;
                if (_multiline && !value)
                {
                    _acceptsReturn = true;
                }
            }
        }
        
        private bool _acceptsTab = false;
        [Browsable(true)]
        [Category("Behavior")]
        public bool AcceptsTab
        {
            get => _acceptsTab;
            set => _acceptsTab = value;
        }
        
        private HorizontalAlignment _textAlignment = HorizontalAlignment.Left;
        [Browsable(true)]
        [Category("Appearance")]
        public HorizontalAlignment TextAlignment
        {
            get => _textAlignment;
            set
            {
                _textAlignment = value;
                Invalidate();
            }
        }
        
        #endregion
        
        #region "Properties - Validation and Masking"
        
        [Browsable(true)]
        [Category("Validation")]
        [Description("Specifies the mask format for the text box.")]
        [DefaultValue(TextBoxMaskFormat.None)]
        public TextBoxMaskFormat MaskFormat
        {
            get => _helper?.Validation?.MaskFormat ?? TextBoxMaskFormat.None;
            set
            {
                if (_helper?.Validation != null)
                {
                    _helper.Validation.MaskFormat = value;
                    Invalidate();
                }
            }
        }
        
        [Browsable(true)]
        [Category("Validation")]
        public string CustomMask
        {
            get => _helper?.Validation?.CustomMask ?? string.Empty;
            set
            {
                if (_helper?.Validation != null)
                {
                    _helper.Validation.CustomMask = value;
                    Invalidate();
                }
            }
        }
        
        [Browsable(true)]
        [Category("Validation")]
        public string DateFormat
        {
            get => _helper?.Validation?.DateFormat ?? "MM/dd/yyyy";
            set
            {
                if (_helper?.Validation != null)
                {
                    _helper.Validation.DateFormat = value;
                    Invalidate();
                }
            }
        }
        
        #endregion
        
        #region "Properties - AutoComplete"
        
        [Browsable(true)]
        [Category("AutoComplete")]
        public AutoCompleteMode AutoCompleteMode
        {
            get => _helper?.AutoComplete?.AutoCompleteMode ?? AutoCompleteMode.None;
            set
            {
                if (_helper?.AutoComplete != null)
                {
                    _helper.AutoComplete.AutoCompleteMode = value;
                }
            }
        }
        
        [Browsable(true)]
        [Category("AutoComplete")]
        public AutoCompleteSource AutoCompleteSource
        {
            get => _helper?.AutoComplete?.AutoCompleteSource ?? AutoCompleteSource.None;
            set
            {
                if (_helper?.AutoComplete != null)
                {
                    _helper.AutoComplete.AutoCompleteSource = value;
                }
            }
        }
        
        [Browsable(true)]
        [Category("AutoComplete")]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get => _helper?.AutoComplete?.AutoCompleteCustomSource;
            set
            {
                if (_helper?.AutoComplete != null)
                {
                    _helper.AutoComplete.AutoCompleteCustomSource = value;
                }
            }
        }
        
        #endregion
        
        #region "Properties - Line Numbers"
        
        private bool _showLineNumbers = false;
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Shows or hides line numbers on the left side of the textbox.")]
        public bool ShowLineNumbers
        {
            get => _showLineNumbers;
            set
            {
                _showLineNumbers = value;
                UpdateDrawingRect();
                Invalidate();
            }
        }
        
        private int _lineNumberMarginWidth = 40;
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(40)]
        [Description("Width of the line number margin.")]
        public int LineNumberMarginWidth
        {
            get => _lineNumberMarginWidth;
            set
            {
                _lineNumberMarginWidth = Math.Max(20, value);
                if (_showLineNumbers)
                {
                    UpdateDrawingRect();
                    Invalidate();
                }
            }
        }
        
        private Color _lineNumberForeColor = Color.Gray;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Foreground color for line numbers.")]
        public Color LineNumberForeColor
        {
            get => _lineNumberForeColor;
            set
            {
                _lineNumberForeColor = value;
                if (_showLineNumbers)
                    Invalidate();
            }
        }
        
        private Color _lineNumberBackColor = Color.FromArgb(240, 240, 240);
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Background color for line number margin.")]
        public Color LineNumberBackColor
        {
            get => _lineNumberBackColor;
            set
            {
                _lineNumberBackColor = value;
                if (_showLineNumbers)
                    Invalidate();
            }
        }
        
        private Font _lineNumberFont;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Font used for line numbers.")]
        public Font LineNumberFont
        {
            get => _lineNumberFont ?? TextFont;
            set
            {
                _lineNumberFont = value;
                if (_showLineNumbers)
                    Invalidate();
            }
        }
        
        #endregion
        
        #region "Properties - Scrolling"
        
        [Browsable(true)]
        [Category("Scrolling")]
        [DefaultValue(true)]
        [Description("Shows or hides the vertical scrollbar.")]
        public bool ShowVerticalScrollBar
        {
            get => _helper?.Scrolling?.ShowVerticalScrollBar ?? true;
            set
            {
                if (_helper?.Scrolling != null)
                {
                    _helper.Scrolling.ShowVerticalScrollBar = value;
                    Invalidate();
                }
            }
        }
        
        [Browsable(true)]
        [Category("Scrolling")]
        [DefaultValue(true)]
        [Description("Shows or hides the horizontal scrollbar.")]
        public bool ShowHorizontalScrollBar
        {
            get => _helper?.Scrolling?.ShowHorizontalScrollBar ?? true;
            set
            {
                if (_helper?.Scrolling != null)
                {
                    _helper.Scrolling.ShowHorizontalScrollBar = value;
                    Invalidate();
                }
            }
        }
        
        [Browsable(false)]
        [Description("Gets the current horizontal scroll offset.")]
        public int ScrollOffsetX => _helper?.Scrolling?.ScrollOffsetX ?? 0;
        
        [Browsable(false)]
        [Description("Gets the current vertical scroll offset.")]
        public int ScrollOffsetY => _helper?.Scrolling?.ScrollOffsetY ?? 0;
        
        #endregion
        
        #region "Properties - Selection and Caret"
        
        [Browsable(false)]
        public int SelectionStart
        {
            get => _helper?.Caret?.SelectionStart ?? 0;
            set
            {
                if (_helper?.Caret != null)
                {
                    _helper.Caret.SelectionStart = value;
                    Invalidate();
                }
            }
        }
        
        [Browsable(false)]
        public int SelectionLength
        {
            get => _helper?.Caret?.SelectionLength ?? 0;
            set
            {
                if (_helper?.Caret != null)
                {
                    _helper.Caret.SelectionLength = value;
                    Invalidate();
                }
            }
        }
        
        [Browsable(false)]
        public string SelectedText => _helper?.Caret?.SelectedText ?? string.Empty;
        
        public Color SelectionBackColor { get; set; } = SystemColors.Highlight;
        
        #endregion
        
        #region "Properties - Image"
        
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Path to the image file to display in the textbox.")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ImagePath
        {
            get => _beepImage?.ImagePath ?? "";
            set
            {
                if (_beepImage != null)
                {
                    _beepImage.ImagePath = value;
                    _beepImage.Visible = !string.IsNullOrEmpty(value);
                    Invalidate();
                }
            }
        }
        
        private Size _maxImageSize = new Size(20, 20);
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Size of the image displayed in the textbox.")]
        public Size MaxImageSize
        {
            get => _maxImageSize;
            set
            {
                _maxImageSize = value;
                if (_beepImage != null)
                {
                    _beepImage.Size = _maxImageSize;
                }
                Invalidate();
            }
        }
        
        private TextImageRelation _textImageRelation = TextImageRelation.ImageBeforeText;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Relationship between text and image.")]
        public TextImageRelation TextImageRelation
        {
            get => _textImageRelation;
            set
            {
                _textImageRelation = value;
                Invalidate();
            }
        }
        
        private ContentAlignment _imageAlign = ContentAlignment.MiddleLeft;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Align image within the BeepSimpleTextBox.")]
        public ContentAlignment ImageAlign
        {
            get => _imageAlign;
            set
            {
                _imageAlign = value;
                Invalidate();
            }
        }
        
        public BeepImage BeepImage => _beepImage;
        
        #endregion
        
        #region "Events"
        
        public new event EventHandler TextChanged;
        public event EventHandler SearchTriggered;
        
        #endregion
        
        #region "Constructor"
        
        public BeepSimpleTextBoxEnhanced() : base()
        {
            _isInitializing = true;
            
            InitializeComponents();
            SetControlStyles();
            InitializeProperties();
            
            _helper = new BeepSimpleTextBoxHelper(this);
            
            UpdateDrawingRect();
            UpdateLines();
            
            _isInitializing = false;
        }
        
        #endregion
        
        #region "Initialization"
        
        private void InitializeComponents()
        {
            _beepImage = new BeepImage()
            {
                Size = _maxImageSize,
                ShowAllBorders = false,
                IsBorderAffectedByTheme = false,
                IsShadowAffectedByTheme = false,
                IsChild = true,
                ClipShape = ImageClipShape.None,
                Visible = false
            };
        }
        
        private void SetControlStyles()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.Selectable |
                     ControlStyles.StandardClick |
                     ControlStyles.StandardDoubleClick, true);
            SetStyle(ControlStyles.ContainerControl, false);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }
        
        private void InitializeProperties()
        {
            _text = string.Empty;
            BoundProperty = "Text";
            BorderRadius = 3;
            ShowAllBorders = true;
            IsShadowAffectedByTheme = false;
            IsBorderAffectedByTheme = false;
            TabStop = true;
        }
        
        protected override Size DefaultSize => new Size(200, 32);
        
        #endregion
        
        #region "Event Overrides"
        
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _helper?.HandleFocusGained();
            Invalidate();
        }
        
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            _helper?.HandleFocusLost();
            Invalidate();
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            _helper?.HandleMouseClick(e, _textRect);
        }
        
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            _helper?.Scrolling?.HandleMouseWheel(e);
        }
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateDrawingRect();
            _helper?.HandleResize();
        }
        
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _textFont = Font;
            _helper?.InvalidateAllCaches();
            Invalidate();
        }
        
        #endregion
        
        #region "Key Handling"
        
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.Back:
                case Keys.Delete:
                case Keys.Enter:
                case Keys.Tab:
                    return true;
                case Keys.Up:
                case Keys.Down:
                    return _multiline;
                default:
                    return base.IsInputKey(keyData);
            }
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            
            if (_readOnly) return;
            
            // Handle undo/redo
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.Z:
                        if (e.Shift)
                            _helper?.UndoRedo?.Redo();
                        else
                            _helper?.UndoRedo?.Undo();
                        e.Handled = true;
                        return;
                    case Keys.Y:
                        _helper?.UndoRedo?.Redo();
                        e.Handled = true;
                        return;
                    case Keys.A:
                        _helper?.Caret?.SelectAll();
                        e.Handled = true;
                        return;
                    case Keys.C:
                        Copy();
                        e.Handled = true;
                        return;
                    case Keys.V:
                        Paste();
                        e.Handled = true;
                        return;
                    case Keys.X:
                        Cut();
                        e.Handled = true;
                        return;
                }
            }
            
            // Handle navigation
            switch (e.KeyCode)
            {
                case Keys.Left:
                    _helper?.Caret?.MoveCaret(-1, e.Shift);
                    e.Handled = true;
                    break;
                case Keys.Right:
                    _helper?.Caret?.MoveCaret(1, e.Shift);
                    e.Handled = true;
                    break;
                case Keys.Up:
                    if (_multiline)
                    {
                        _helper?.Caret?.MoveCaretVertical(-1, e.Shift);
                        e.Handled = true;
                    }
                    break;
                case Keys.Down:
                    if (_multiline)
                    {
                        _helper?.Caret?.MoveCaretVertical(1, e.Shift);
                        e.Handled = true;
                    }
                    break;
                case Keys.Home:
                    // Handle Home key logic
                    e.Handled = true;
                    break;
                case Keys.End:
                    // Handle End key logic
                    e.Handled = true;
                    break;
                case Keys.Back:
                    HandleBackspace();
                    e.Handled = true;
                    break;
                case Keys.Delete:
                    HandleDelete();
                    e.Handled = true;
                    break;
                case Keys.Enter:
                    if (_multiline && _acceptsReturn)
                    {
                        _helper?.InsertText(Environment.NewLine);
                    }
                    else
                    {
                        SearchTriggered?.Invoke(this, EventArgs.Empty);
                    }
                    e.Handled = true;
                    break;
                case Keys.Tab:
                    if (_acceptsTab)
                    {
                        _helper?.InsertText("\t");
                        e.Handled = true;
                    }
                    break;
            }
        }
        
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            
            if (_readOnly)
            {
                e.Handled = true;
                return;
            }
            
            if (char.IsControl(e.KeyChar))
            {
                return; // Already handled in OnKeyDown
            }
            
            _helper?.InsertText(e.KeyChar.ToString());
            e.Handled = true;
        }
        
        #endregion
        
        #region "Text Operations"
        
        private void HandleBackspace()
        {
            if (_helper?.Caret?.HasSelection == true)
            {
                _helper.Caret.RemoveSelectedText();
            }
            else if (_helper?.Caret?.CaretPosition > 0)
            {
                _helper.DeleteText(_helper.Caret.CaretPosition - 1, 1);
            }
        }
        
        private void HandleDelete()
        {
            if (_helper?.Caret?.HasSelection == true)
            {
                _helper.Caret.RemoveSelectedText();
            }
            else if (_helper?.Caret?.CaretPosition < _text.Length)
            {
                _helper.DeleteText(_helper.Caret.CaretPosition, 1);
            }
        }
        
        public void Copy()
        {
            if (!string.IsNullOrEmpty(SelectedText))
            {
                Clipboard.SetText(SelectedText);
            }
        }
        
        public void Cut()
        {
            if (!string.IsNullOrEmpty(SelectedText) && !_readOnly)
            {
                Copy();
                _helper?.Caret?.RemoveSelectedText();
            }
        }
        
        public void Paste()
        {
            if (!_readOnly && Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText();
                _helper?.InsertText(clipboardText);
            }
        }
        
        public void SelectAll()
        {
            _helper?.Caret?.SelectAll();
        }
        
        public void Clear()
        {
            if (!_readOnly)
            {
                Text = "";
                if (_helper?.Caret != null)
                {
                    _helper.Caret.CaretPosition = 0;
                    _helper.Caret.SelectionStart = 0;
                    _helper.Caret.SelectionLength = 0;
                }
                Invalidate();
            }
        }
        
        #endregion
        
        #region "Drawing"
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _helper?.DrawAll(e.Graphics, ClientRectangle, _textRect);
        }
        
        #endregion
        
        #region "Helper Methods"
        
        private void UpdateDrawingRect()
        {
            _textRect = ClientRectangle;
            _textRect.Inflate(-Padding.Horizontal / 2, -Padding.Vertical / 2);
            
            if (_showLineNumbers && _multiline)
            {
                _textRect.X += _lineNumberMarginWidth;
                _textRect.Width -= _lineNumberMarginWidth;
            }
        }
        
        private void UpdateLines()
        {
            _lines.Clear();
            
            if (string.IsNullOrEmpty(_text))
            {
                _lines.Add("");
                return;
            }
            
            if (_multiline)
            {
                string[] splitLines = _text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                _lines.AddRange(splitLines);
            }
            else
            {
                string singleLineText = _text.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                _lines.Add(singleLineText);
            }
            
            if (_lines.Count == 0)
                _lines.Add("");
        }
        
        /// <summary>
        /// Gets the display text (handles placeholder and password characters)
        /// </summary>
        public string GetDisplayText()
        {
            if (string.IsNullOrEmpty(_text) ||
                _text.StartsWith("beepSimpleTextBox") ||
                _text.Equals(this.Name))
            {
                return PlaceholderText ?? "Enter Text Here";
            }
            
            return _text;
        }
        
        /// <summary>
        /// Gets the current lines collection
        /// </summary>
        public List<string> GetLines() => _lines;
        
        /// <summary>
        /// Scrolls to ensure the caret is visible
        /// </summary>
        public void ScrollToCaret()
        {
            _helper?.Scrolling?.ScrollToCaret(_helper.Caret?.CaretPosition ?? 0);
        }
        
        #endregion
        
        #region "IBeepComponent Implementation"
        
        public bool ValidateData(out string message)
        {
            return _helper?.ValidateAllData(out message) ?? TheTechIdea.Beep.Desktop.Common.Util.EntityHelper.ValidateData(MaskFormat, Text, CustomMask, out message);
        }
        
        public override void SetValue(object value)
        {
            Text = value?.ToString() ?? string.Empty;
        }
        
        public override object GetValue()
        {
            return Text;
        }
        
        public override void ClearValue()
        {
            Text = string.Empty;
        }
        
        #endregion
        
        #region "Cleanup"
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _helper?.Dispose();
            }
            base.Dispose(disposing);
        }
        
        #endregion

        #region "IBeepTextBox Implementation"
        
        // Implement Focus method to satisfy interface
        void IBeepTextBox.Focus()
        {
            base.Focus();
        }
        
        #endregion
    }
}