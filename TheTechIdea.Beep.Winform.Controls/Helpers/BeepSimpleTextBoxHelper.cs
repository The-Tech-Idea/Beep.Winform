using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    /// <summary>
    /// Interface for textbox controls that can use the helper system
    /// </summary>
    public interface IBeepTextBox
    {
        // Core Text Properties
        string Text { get; set; }
        Font TextFont { get; set; }
        bool Multiline { get; set; }
        bool ReadOnly { get; set; }
        HorizontalAlignment TextAlignment { get; set; }
        string PlaceholderText { get; set; }
        Color PlaceholderTextColor { get; set; }
        
        // Selection Properties
        int SelectionStart { get; set; }
        int SelectionLength { get; set; }
        Color SelectionBackColor { get; set; }
        
        // Line Numbers
        bool ShowLineNumbers { get; set; }
        int LineNumberMarginWidth { get; set; }
        Color LineNumberForeColor { get; set; }
        Color LineNumberBackColor { get; set; }
        Font LineNumberFont { get; set; }
        
        // Image Properties
        string ImagePath { get; set; }
        Size MaxImageSize { get; set; }
        TextImageRelation TextImageRelation { get; set; }
        ContentAlignment ImageAlign { get; set; }
        BeepImage BeepImage { get; }
        
        // Masking
        TextBoxMaskFormat MaskFormat { get; set; }
        string CustomMask { get; set; }
        
        // Methods
        System.Collections.Generic.List<string> GetLines();
        string GetDisplayText();
        void ScrollToCaret();
        void Invalidate();
        new void Focus(); // Use 'new' to hide the base Control.Focus()
        
        // Control Properties
        Rectangle ClientRectangle { get; }
        bool Focused { get; }
        System.Windows.Forms.Padding Padding { get; }
        Point PointToScreen(Point point);
        Graphics CreateGraphics();
        
        // Events
        event EventHandler TextChanged;
    }

    /// <summary>
    /// Simple stub implementations for missing helper classes
    /// </summary>
    public class TextBoxPerformanceHelper
    {
        private Dictionary<string, object> _fontMetricsCache = new Dictionary<string, object>();
        
        public void InvalidateAllCaches() 
        {
            _fontMetricsCache.Clear();
        }
        
        public void InvalidateLineMetrics() 
        {
            _fontMetricsCache.Clear();
        }
        
        public void CacheFontMetrics(Graphics g, Font font) 
        {
            if (font == null) return;
            string key = $"{font.Name}_{font.Size}_{font.Style}";
            if (!_fontMetricsCache.ContainsKey(key))
            {
                _fontMetricsCache[key] = TextRenderer.MeasureText(g, "Ag", font).Height;
            }
        }
        
        public void CalculateLineMetrics(Graphics g, System.Collections.Generic.List<string> lines, Font font) 
        {
            CacheFontMetrics(g, font);
        }
        
        public float GetCachedLineHeight(Graphics g, Font font)
        {
            if (font == null) return 16f;
            string key = $"{font.Name}_{font.Size}_{font.Style}";
            if (_fontMetricsCache.TryGetValue(key, out object value))
            {
                return (float)(int)value;
            }
            
            int height = TextRenderer.MeasureText(g, "Ag", font).Height;
            _fontMetricsCache[key] = height;
            return height;
        }
    }

    public class TextBoxUndoRedoHelper
    {
        public class TextEditAction
        {
            public string OldText { get; set; }
            public string NewText { get; set; }
            public int OldCaretPosition { get; set; }
            public int NewCaretPosition { get; set; }
            public int OldSelectionStart { get; set; }
            public int NewSelectionStart { get; set; }
            public int OldSelectionLength { get; set; }
            public int NewSelectionLength { get; set; }
            public string ActionType { get; set; }
        }

        public event EventHandler<TextEditAction> UndoPerformed;
        public event EventHandler<TextEditAction> RedoPerformed;
        public bool IsUndoRedoOperation { get; set; }

        public void AddUndoAction(string actionType, string oldText, string newText, int oldCaret, int newCaret) { }
        public void Undo() { }
        public void Redo() { }
    }

    public class TextBoxCaretHelper : IDisposable
    {
        private readonly IBeepTextBox _textBox;
        private int _caretPosition = 0;
        private int _selectionStart = 0;
        private int _selectionLength = 0;
        private bool _caretVisible = false;
        private System.Windows.Forms.Timer _caretTimer;

        public TextBoxCaretHelper(IBeepTextBox textBox, TextBoxPerformanceHelper performance)
        {
            _textBox = textBox;
            _caretTimer = new System.Windows.Forms.Timer();
            _caretTimer.Interval = 500;
            _caretTimer.Tick += (s, e) => { _caretVisible = !_caretVisible; _textBox.Invalidate(); };
        }

        public int CaretPosition
        {
            get => _caretPosition;
            set => _caretPosition = Math.Max(0, Math.Min(value, _textBox.Text?.Length ?? 0));
        }

        public int SelectionStart
        {
            get => _selectionStart;
            set => _selectionStart = Math.Max(0, Math.Min(value, _textBox.Text?.Length ?? 0));
        }

        public int SelectionLength
        {
            get => _selectionLength;
            set => _selectionLength = Math.Max(0, value);
        }

        public bool HasSelection => _selectionLength > 0;
        public bool CaretVisible => _caretVisible;
        public string SelectedText => HasSelection && _textBox.Text != null && _selectionStart < _textBox.Text.Length
            ? _textBox.Text.Substring(_selectionStart, Math.Min(_selectionLength, _textBox.Text.Length - _selectionStart))
            : string.Empty;

        public void StartCaretTimer() 
        {
            _caretVisible = true;
            _caretTimer.Start();
            _textBox.Invalidate();
        }
        
        public void StopCaretTimer() 
        {
            _caretTimer.Stop();
            _caretVisible = false;
            _textBox.Invalidate();
        }
        
        public void MoveCaret(int direction, bool extend) 
        {
            int newPosition = Math.Max(0, Math.Min(_caretPosition + direction, _textBox.Text?.Length ?? 0));
            
            if (extend)
            {
                // Extend selection
                if (_selectionLength == 0)
                {
                    _selectionStart = _caretPosition;
                }
                
                int selStart = Math.Min(_selectionStart, newPosition);
                int selEnd = Math.Max(_selectionStart, newPosition);
                _selectionStart = selStart;
                _selectionLength = selEnd - selStart;
                
                // Sync with main control
                _textBox.SelectionStart = _selectionStart;
                _textBox.SelectionLength = _selectionLength;
            }
            else
            {
                ClearSelection();
                // Sync with main control
                _textBox.SelectionStart = newPosition;
                _textBox.SelectionLength = 0;
            }
            
            _caretPosition = newPosition;
            _selectionStart = newPosition;
            
            EnsureCaretVisible();
            _textBox.Invalidate();
        }
        
        public void MoveCaretVertical(int direction, bool extend) 
        {
            // Basic vertical movement for multiline
            var beepTextBox = _textBox as BeepSimpleTextBox;
            if (beepTextBox?.Multiline != true) return;
            
            var lines = beepTextBox.GetLines();
            if (lines.Count <= 1) return;
            
            // Find current line and position
            int currentPos = 0;
            int currentLine = 0;
            int columnIndex = _caretPosition;
            
            for (int i = 0; i < lines.Count; i++)
            {
                if (_caretPosition <= currentPos + lines[i].Length)
                {
                    currentLine = i;
                    columnIndex = _caretPosition - currentPos;
                    break;
                }
                currentPos += lines[i].Length + Environment.NewLine.Length;
            }
            
            // Move to target line
            int targetLine = Math.Max(0, Math.Min(currentLine + direction, lines.Count - 1));
            if (targetLine == currentLine) return;
            
            // Calculate new position
            int newPos = 0;
            for (int i = 0; i < targetLine; i++)
            {
                newPos += lines[i].Length + Environment.NewLine.Length;
            }
            newPos += Math.Min(columnIndex, lines[targetLine].Length);
            
            if (extend)
            {
                if (_selectionLength == 0)
                {
                    _selectionStart = _caretPosition;
                }
                
                int selStart = Math.Min(_selectionStart, newPos);
                int selEnd = Math.Max(_selectionStart, newPos);
                _selectionStart = selStart;
                _selectionLength = selEnd - selStart;
                
                // Sync with main control
                _textBox.SelectionStart = _selectionStart;
                _textBox.SelectionLength = _selectionLength;
            }
            else
            {
                ClearSelection();
                _selectionStart = newPos;
                
                // Sync with main control
                _textBox.SelectionStart = newPos;
                _textBox.SelectionLength = 0;
            }
            
            _caretPosition = newPos;
            EnsureCaretVisible();
            _textBox.Invalidate();
        }
        
        public void SelectAll() 
        { 
            if (_textBox.Text != null) 
            { 
                _selectionStart = 0; 
                _selectionLength = _textBox.Text.Length;
                _caretPosition = _textBox.Text.Length;
                
                // Sync with main control
                _textBox.SelectionStart = _selectionStart;
                _textBox.SelectionLength = _selectionLength;
                
                _textBox.Invalidate();
            } 
        }
        
        public void ClearSelection() 
        { 
            _selectionLength = 0; 
            
            // Sync with main control
            _textBox.SelectionLength = 0;
            
            _textBox.Invalidate();
        }
        
        public void RemoveSelectedText() 
        { 
            if (HasSelection)
            {
                string text = _textBox.Text ?? "";
                string newText = text.Remove(_selectionStart, Math.Min(_selectionLength, text.Length - _selectionStart));
                _textBox.Text = newText;
                _caretPosition = _selectionStart;
                
                // Clear selection in both helper and main control
                _selectionLength = 0;
                _textBox.SelectionStart = _selectionStart;
                _textBox.SelectionLength = 0;
                
                _textBox.Invalidate();
            }
        }
        
        public void EnsureCaretVisible() 
        {
            // Reset timer to make caret visible immediately
            _caretVisible = true;
            _caretTimer.Stop();
            _caretTimer.Start();
        }
        
        public int GetCaretPositionFromPoint(Point point, Graphics g, Rectangle textRect) 
        {
            string text = _textBox.Text ?? "";
            // Use displayed text for width (respect password masking)
            var beepTextBox = _textBox as BeepSimpleTextBox;
            if (beepTextBox != null)
            {
                if (beepTextBox.UseSystemPasswordChar && !string.IsNullOrEmpty(text))
                {
                    text = new string('•', text.Length);
                }
                else if (beepTextBox.PasswordChar != '\0' && !string.IsNullOrEmpty(text))
                {
                    text = new string(beepTextBox.PasswordChar, text.Length);
                }
            }
            
            Font font = _textBox.TextFont ?? new Font("Segoe UI", 9f);
            
            // Actual text bounds (account for image)
            Rectangle actualTextRect = GetActualTextRect(g, textRect);
            
            // Base X depends on alignment
            int baseX = actualTextRect.X;
            Size fullTextSize = Size.Empty;
            if (!string.IsNullOrEmpty(text))
            {
                fullTextSize = TextRenderer.MeasureText(g, text, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                if (_textBox.TextAlignment == HorizontalAlignment.Center)
                {
                    baseX = actualTextRect.X + Math.Max(0, (actualTextRect.Width - fullTextSize.Width) / 2);
                }
                else if (_textBox.TextAlignment == HorizontalAlignment.Right)
                {
                    baseX = actualTextRect.Right - fullTextSize.Width;
                }
            }
            
            int x = point.X;
            if (x < actualTextRect.X) x = actualTextRect.X;
            if (x > actualTextRect.Right) x = actualTextRect.Right;
            int relativeX = x - baseX;
            if (relativeX <= 0) return 0;
            
            if (string.IsNullOrEmpty(text)) return 0;
            
            // Walk characters to find closest caret index
            int bestIndex = 0;
            int bestDelta = int.MaxValue;
            for (int i = 0; i <= text.Length; i++)
            {
                string upTo = i == 0 ? string.Empty : text.Substring(0, i);
                Size size = TextRenderer.MeasureText(g, upTo, font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                int delta = Math.Abs(size.Width - relativeX);
                if (delta < bestDelta)
                {
                    bestDelta = delta;
                    bestIndex = i;
                }
            }
            return bestIndex;
        }
        
        /// <summary>
        /// Gets the actual text rectangle considering image layout
        /// </summary>
        private Rectangle GetActualTextRect(Graphics g, Rectangle textRect)
        {
            // Check if there's an image that affects layout using strict visibility
            var beepTextBox = _textBox as BeepSimpleTextBox;
            bool hasVisibleImage = _textBox.BeepImage != null && 
                                   !string.IsNullOrWhiteSpace(_textBox.ImagePath) && 
                                   (beepTextBox?.ImageVisible ?? false) &&
                                   _textBox.BeepImage.Visible;
            if (!hasVisibleImage)
                return textRect;
            
            // Compute image size with MaxImageSize constraints
            Size imageSize = Size.Empty;
            try
            {
                imageSize = _textBox.BeepImage.GetImageSize();
                Size maxSize = _textBox.MaxImageSize;
                if (imageSize.Width > maxSize.Width || imageSize.Height > maxSize.Height)
                {
                    float scaleFactor = Math.Min(
                        (float)maxSize.Width / imageSize.Width,
                        (float)maxSize.Height / imageSize.Height);
                    imageSize = new Size(
                        (int)(imageSize.Width * scaleFactor),
                        (int)(imageSize.Height * scaleFactor));
                }
            }
            catch
            {
                imageSize = _textBox.MaxImageSize;
            }
            
            // Include image margin if any
            Padding margin = beepTextBox != null ? beepTextBox.ImageMargin : new Padding(0);
            
            Rectangle adjustedTextRect = textRect;
            
            switch (_textBox.TextImageRelation)
            {
                case TextImageRelation.ImageBeforeText:
                    adjustedTextRect.X += imageSize.Width + margin.Horizontal + 4;
                    adjustedTextRect.Width -= imageSize.Width + margin.Horizontal + 4;
                    break;
                case TextImageRelation.TextBeforeImage:
                    adjustedTextRect.Width -= imageSize.Width + margin.Horizontal + 4;
                    break;
                case TextImageRelation.ImageAboveText:
                    adjustedTextRect.Y += imageSize.Height + margin.Vertical + 2;
                    adjustedTextRect.Height -= imageSize.Height + margin.Vertical + 2;
                    break;
                case TextImageRelation.TextAboveImage:
                    adjustedTextRect.Height -= imageSize.Height + margin.Vertical + 2;
                    break;
                case TextImageRelation.Overlay:
                default:
                    break;
            }
            
            return adjustedTextRect;
        }
        
        public void Dispose()
        {
            _caretTimer?.Dispose();
        }
    }

    public class TextBoxScrollingHelper : IDisposable
    {
        private readonly IBeepTextBox _textBox;
        public int ScrollOffsetX { get; set; } = 0;
        public int ScrollOffsetY { get; set; } = 0;
        public bool ShowVerticalScrollBar { get; set; } = true;
        public bool ShowHorizontalScrollBar { get; set; } = true;

        public TextBoxScrollingHelper(IBeepTextBox textBox, TextBoxPerformanceHelper performance)
        {
            _textBox = textBox;
        }

        public void UpdateContentSize() { }
        public void InvalidateViewport() { }
        public void HandleResize() { }
        public void HandleMouseWheel(MouseEventArgs e) { }
        public void ScrollToCaret(int caretPosition) { }

        public void Dispose() { }
    }

    /// <summary>
    /// Unified interface for all BeepSimpleTextBox helper functionality
    /// This is the main entry point that coordinates all helper classes
    /// </summary>
    public class BeepSimpleTextBoxHelper : IDisposable
    {
        #region "Helper Instances"
        
        public TextBoxPerformanceHelper Performance { get; private set; }
        public SmartAutoCompleteHelper AutoComplete { get; private set; }
        public TextBoxUndoRedoHelper UndoRedo { get; private set; }
        public TextBoxValidationHelper Validation { get; private set; }
        public TextBoxCaretHelper Caret { get; private set; }
        public TextBoxScrollingHelper Scrolling { get; private set; }
        public TextBoxDrawingHelper Drawing { get; private set; }
        public TextBoxAdvancedEditingHelper AdvancedEditing { get; private set; }
        
        #endregion
        
        #region "Fields"
        
        private readonly IBeepTextBox _textBox;
        private bool _disposed = false;
        
        #endregion
        
        #region "Constructor"
        
        public BeepSimpleTextBoxHelper(IBeepTextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
            
            InitializeHelpers();
            WireUpEvents();
        }
        
        #endregion
        
        #region "Initialization"
        
        private void InitializeHelpers()
        {
            // Initialize in dependency order
            Performance = new TextBoxPerformanceHelper();
            Validation = new TextBoxValidationHelper(_textBox);
            AutoComplete = new SmartAutoCompleteHelper(_textBox as Control);
            UndoRedo = new TextBoxUndoRedoHelper();
            Caret = new TextBoxCaretHelper(_textBox, Performance);
            Scrolling = new TextBoxScrollingHelper(_textBox, Performance);
            Drawing = new TextBoxDrawingHelper(_textBox);
            Drawing.SetPerformanceHelper(Performance);
            AdvancedEditing = new TextBoxAdvancedEditingHelper(_textBox);
        }
        
        private void WireUpEvents()
        {
            // Wire up cross-helper communication
            UndoRedo.UndoPerformed += OnUndoPerformed;
            UndoRedo.RedoPerformed += OnRedoPerformed;
            
            AutoComplete.AutoCompleteItemSelected += OnAutoCompleteItemSelected;
        }
        
        #endregion
        
        #region "Event Handlers"
        
        private void OnUndoPerformed(object sender, TextBoxUndoRedoHelper.TextEditAction action)
        {
            // Apply the undo action to the textbox
            _textBox.Text = action.OldText;
            Caret.CaretPosition = action.OldCaretPosition;
            Caret.SelectionStart = action.OldSelectionStart;
            Caret.SelectionLength = action.OldSelectionLength;
            
            // Ensure caret is visible after undo
            Caret.EnsureCaretVisible();
            Scrolling.ScrollToCaret(Caret.CaretPosition);
            
            _textBox.Invalidate();
        }
        
        private void OnRedoPerformed(object sender, TextBoxUndoRedoHelper.TextEditAction action)
        {
            // Apply the redo action to the textbox
            _textBox.Text = action.NewText;
            Caret.CaretPosition = action.NewCaretPosition;
            Caret.SelectionStart = action.NewSelectionStart;
            Caret.SelectionLength = action.NewSelectionLength;
            
            // Ensure caret is visible after redo
            Caret.EnsureCaretVisible();
            Scrolling.ScrollToCaret(Caret.CaretPosition);
            
            _textBox.Invalidate();
        }
        
        private void OnAutoCompleteItemSelected(object sender, string selectedItem)
        {
            // Record undo action before applying autocomplete
            string oldText = _textBox.Text;
            int oldCaret = Caret.CaretPosition;
            
            // Apply the autocomplete suggestion
            _textBox.Text = selectedItem;
            Caret.CaretPosition = selectedItem.Length;
            Caret.ClearSelection();
            
            // Add to undo history
            UndoRedo.AddUndoAction("AutoComplete", oldText, _textBox.Text, oldCaret, Caret.CaretPosition);
            
            _textBox.Invalidate();
        }
        
        #endregion
        
        #region "Unified Methods"
        
        /// <summary>
        /// Handles all text insertion with validation, undo tracking, and autocomplete
        /// </summary>
        public void InsertText(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            
            // Validate input
            if (!Validation.ValidateInput(text))
                return;
            
            // Record undo action
            string oldText = _textBox.Text;
            int oldCaret = Caret.CaretPosition;
            
            // Remove selected text first
            if (Caret.HasSelection)
            {
                Caret.RemoveSelectedText();
            }
            
            // Insert the text
            int insertPosition = Caret.CaretPosition;
            string newText = _textBox.Text.Insert(insertPosition, text);
            _textBox.Text = newText;
            
            // Update caret position in both helper and main control
            int newCaretPosition = insertPosition + text.Length;
            Caret.CaretPosition = newCaretPosition;
            Caret.SelectionStart = newCaretPosition;
            Caret.ClearSelection();
            
            // CRITICAL: Sync with main control's selection properties
            _textBox.SelectionStart = newCaretPosition;
            _textBox.SelectionLength = 0;
            
            // Apply formatting if needed
            if (Validation.IsApplyingMask)
            {
                string formattedText = Validation.ApplyMaskFormat(_textBox.Text);
                if (formattedText != _textBox.Text)
                {
                    _textBox.Text = formattedText;
                    // Adjust caret position after formatting
                    newCaretPosition = Math.Min(newCaretPosition, _textBox.Text.Length);
                    Caret.CaretPosition = newCaretPosition;
                    Caret.SelectionStart = newCaretPosition;
                    _textBox.SelectionStart = newCaretPosition;
                }
            }
            
            // Add to undo history
            if (!UndoRedo.IsUndoRedoOperation)
            {
                UndoRedo.AddUndoAction("Insert", oldText, _textBox.Text, oldCaret, Caret.CaretPosition);
            }
            
            // Trigger autocomplete for visible characters
            if (!char.IsControl(text[0]))
            {
                AutoComplete.TriggerSmartAutoComplete(_textBox.Text);
            }
            
            // Ensure caret is visible
            Scrolling.ScrollToCaret(Caret.CaretPosition);
            _textBox.Invalidate();
        }
        
        /// <summary>
        /// Handles character deletion with undo tracking
        /// </summary>
        public void DeleteText(int position, int length)
        {
            if (position < 0 || length <= 0 || position >= _textBox.Text.Length)
                return;
            
            // Record undo action
            string oldText = _textBox.Text;
            int oldCaret = Caret.CaretPosition;
            
            // Perform deletion
            length = Math.Min(length, _textBox.Text.Length - position);
            string newText = _textBox.Text.Remove(position, length);
            _textBox.Text = newText;
            
            // Update caret position in both helper and main control
            Caret.CaretPosition = position;
            Caret.SelectionStart = position;
            Caret.ClearSelection();
            
            // CRITICAL: Sync with main control's selection properties
            _textBox.SelectionStart = position;
            _textBox.SelectionLength = 0;
            
            // Add to undo history
            if (!UndoRedo.IsUndoRedoOperation)
            {
                UndoRedo.AddUndoAction("Delete", oldText, _textBox.Text, oldCaret, Caret.CaretPosition);
            }
            
            // Ensure caret is visible
            Scrolling.ScrollToCaret(Caret.CaretPosition);
            _textBox.Invalidate();
        }
        
        /// <summary>
        /// Invalidates all caches when text or font changes
        /// </summary>
        public void InvalidateAllCaches()
        {
            Performance.InvalidateAllCaches();
            Scrolling.InvalidateViewport();
            _textBox.Invalidate();
        }
        
        /// <summary>
        /// Updates all helpers when the textbox is resized
        /// </summary>
        public void HandleResize()
        {
            Performance.InvalidateLineMetrics();
            Scrolling.HandleResize();
            _textBox.Invalidate();
        }
        
        /// <summary>
        /// Handles focus gained
        /// </summary>
        public void HandleFocusGained()
        {
            Caret.StartCaretTimer();
        }
        
        /// <summary>
        /// Handles focus lost
        /// </summary>
        public void HandleFocusLost()
        {
            Caret.StopCaretTimer();
            // AutoComplete is handled in its own helper
        }
        
        /// <summary>
        /// Main drawing coordination method - now uses the enhanced TextBoxDrawingHelper
        /// </summary>
        public void DrawAll(Graphics graphics, Rectangle clientRect, Rectangle textRect)
        {
            // Use the enhanced drawing helper that handles image/text alignment like BeepButton
            Drawing.DrawAll(graphics, clientRect, textRect);
        }
        
        /// <summary>
        /// Handles mouse click for caret positioning and selection
        /// </summary>
        public void HandleMouseClick(MouseEventArgs e, Rectangle textRect)
        {
            using (Graphics g = _textBox.CreateGraphics())
            {
                int clickPosition = Caret.GetCaretPositionFromPoint(e.Location, g, textRect);
                
                // Update both helper and main control
                Caret.CaretPosition = clickPosition;
                Caret.SelectionStart = clickPosition;
                Caret.SelectionLength = 0;
                
                // CRITICAL: Sync with main control's selection properties
                _textBox.SelectionStart = clickPosition;
                _textBox.SelectionLength = 0;
                
                Caret.EnsureCaretVisible();
                _textBox.Invalidate();
            }
        }
        
        #endregion
        
        #region "Validation"
        
        /// <summary>
        /// Validates all data and returns comprehensive results
        /// </summary>
        public bool ValidateAllData(out string message)
        {
            return Validation.ValidateData(out message);
        }
        
        #endregion
        
        #region "IDisposable Implementation"
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                // Dispose all helpers
                AutoComplete?.Dispose();
                Caret?.Dispose();
                Scrolling?.Dispose();
                AdvancedEditing?.Dispose();
                
                // Unwire events
                if (UndoRedo != null)
                {
                    UndoRedo.UndoPerformed -= OnUndoPerformed;
                    UndoRedo.RedoPerformed -= OnRedoPerformed;
                }
                
                if (AutoComplete != null)
                {
                    AutoComplete.AutoCompleteItemSelected -= OnAutoCompleteItemSelected;
                }
                
                _disposed = true;
            }
        }
        
        #endregion
    }
}