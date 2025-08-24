using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Icons;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.TextFields.Helpers;

namespace TheTechIdea.Beep.Winform.Controls.TextFields
{
    public partial class BeepMaterialTextField
    {
        #region Text Manipulation Methods

        /// <summary>
        /// Clears all text from the control
        /// </summary>
        public void Clear()
        {
            if (!_readOnly)
            {
                Text = string.Empty;
                SelectionStart = 0;
                SelectionLength = 0;
                Invalidate();
            }
        }

        /// <summary>
        /// Selects all text in the control
        /// </summary>
        public void SelectAll()
        {
            if (!string.IsNullOrEmpty(_text))
            {
                SelectionStart = 0;
                SelectionLength = _text.Length;
                Invalidate();
            }
        }

        /// <summary>
        /// Copies selected text to clipboard
        /// </summary>
        public void Copy()
        {
            string selectedText = SelectedText;
            if (!string.IsNullOrEmpty(selectedText))
            {
                Clipboard.SetText(selectedText);
            }
        }

        /// <summary>
        /// Cuts selected text to clipboard
        /// </summary>
        public void Cut()
        {
            if (!_readOnly && !string.IsNullOrEmpty(SelectedText))
            {
                Copy();
                _materialHelper?.RemoveSelectedText();
            }
        }

        /// <summary>
        /// Pastes text from clipboard
        /// </summary>
        public void Paste()
        {
            if (!_readOnly && Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText();
                _materialHelper?.InsertText(clipboardText);
            }
        }

        /// <summary>
        /// Undoes the last text operation
        /// </summary>
        public void Undo()
        {
            // Basic undo implementation - would be enhanced with actual undo/redo logic
            // _advancedEditingHelper?.Undo();
        }

        /// <summary>
        /// Redoes the last undone operation
        /// </summary>
        public void Redo()
        {
            // Basic redo implementation - would be enhanced with actual undo/redo logic
            // _advancedEditingHelper?.Redo();
        }

        #endregion

        #region AutoComplete Methods

        /// <summary>
        /// Add an item to the autocomplete list
        /// </summary>
        public void AddAutoCompleteItem(string item)
        {
            _autoCompleteHelper?.AddAutoCompleteItem(item);
        }

        /// <summary>
        /// Add multiple items to the autocomplete list
        /// </summary>
        public void AddAutoCompleteItems(IEnumerable<string> items)
        {
            _autoCompleteHelper?.AddAutoCompleteItems(items);
        }

        /// <summary>
        /// Clear all autocomplete items
        /// </summary>
        public void ClearAutoCompleteItems()
        {
            _autoCompleteHelper?.ClearAutoCompleteItems();
        }

        /// <summary>
        /// Add smart autocomplete items with popularity scoring
        /// </summary>
        public void AddSmartAutoCompleteItems(IEnumerable<string> items, int popularity = 1)
        {
            _autoCompleteHelper?.AddSmartAutoCompleteItems(items, popularity);
        }

        #endregion

        #region Advanced Text Editing Methods

        /// <summary>
        /// Show find and replace dialog
        /// </summary>
        public void ShowFindReplaceDialog()
        {
            _advancedEditingHelper?.ShowFindReplaceDialog();
        }

        /// <summary>
        /// Find all occurrences of text
        /// </summary>
        public List<int> FindAll(string searchText, bool caseSensitive = false, bool wholeWord = false)
        {
            return _advancedEditingHelper?.FindAll(searchText, caseSensitive, wholeWord) ?? 
                   new List<int>();
        }

        /// <summary>
        /// Replace all occurrences of text
        /// </summary>
        public int ReplaceAll(string searchText, string replaceText, bool caseSensitive = false, bool wholeWord = false)
        {
            return _advancedEditingHelper?.ReplaceAll(searchText, replaceText, caseSensitive, wholeWord) ?? 0;
        }

        /// <summary>
        /// Go to specific line number
        /// </summary>
        public void GoToLine(int lineNumber)
        {
            _advancedEditingHelper?.GoToLine(lineNumber);
        }

        /// <summary>
        /// Toggle bookmark at current line
        /// </summary>
        public void ToggleBookmark()
        {
            int lineNumber = GetCurrentLineNumber();
            _advancedEditingHelper?.ToggleBookmark(lineNumber);
        }

        /// <summary>
        /// Go to next bookmark
        /// </summary>
        public void NextBookmark()
        {
            _advancedEditingHelper?.NextBookmark();
        }

        /// <summary>
        /// Go to previous bookmark
        /// </summary>
        public void PreviousBookmark()
        {
            _advancedEditingHelper?.PreviousBookmark();
        }

        /// <summary>
        /// Format the document according to syntax language
        /// </summary>
        public void FormatDocument()
        {
            _advancedEditingHelper?.FormatDocument();
        }

        /// <summary>
        /// Comment the selected lines
        /// </summary>
        public void CommentSelection()
        {
            _advancedEditingHelper?.CommentSelection();
        }

        /// <summary>
        /// Uncomment the selected lines
        /// </summary>
        public void UncommentSelection()
        {
            _advancedEditingHelper?.UncommentSelection();
        }

        /// <summary>
        /// Apply syntax highlighting to the text
        /// </summary>
        public void ApplySyntaxHighlighting()
        {
            _advancedEditingHelper?.ApplySyntaxHighlighting();
        }

        #endregion

        #region Material Design Methods

        /// <summary>
        /// Start the label floating animation
        /// </summary>
        public void StartLabelAnimation(bool shouldFloat)
        {
            _materialHelper?.StartLabelAnimation(shouldFloat);
        }

        /// <summary>
        /// Start the focus animation
        /// </summary>
        public void StartFocusAnimation(bool isFocused)
        {
            _materialHelper?.StartFocusAnimation(isFocused);
        }

        /// <summary>
        /// Start ripple effect animation
        /// </summary>
        public void StartRippleAnimation(Point clickPosition)
        {
            if (EnableRippleEffect)
            {
                _materialHelper?.StartRippleAnimation(clickPosition);
            }
        }

        /// <summary>
        /// Update the Material Design layout
        /// </summary>
        public void UpdateMaterialLayout()
        {
            _materialHelper?.UpdateLayout();
        }

        /// <summary>
        /// Update icon visibility and positioning
        /// </summary>
        public void UpdateIcons()
        {
            _materialHelper?.UpdateIcons();
        }

        /// <summary>
        /// Apply Material Design variant styling
        /// </summary>
        public void ApplyVariant(MaterialTextFieldVariant variant)
        {
            _variant = variant;
            MaterialBorderVariant = variant;
            _materialHelper?.ApplyVariant(variant);
            Invalidate();
        }

        /// <summary>
        /// Apply Material Design density
        /// </summary>
        public void ApplyDensity(MaterialTextFieldDensity density)
        {
            Density = density;
        }

        /// <summary>
        /// Set prefix text
        /// </summary>
        public void SetPrefixText(string prefixText)
        {
            PrefixText = prefixText;
        }

        /// <summary>
        /// Set suffix text
        /// </summary>
        public void SetSuffixText(string suffixText)
        {
            SuffixText = suffixText;
        }

        /// <summary>
        /// Get current character count for counter display
        /// </summary>
        public string GetCharacterCountText()
        {
            if (_maxLength > 0 && _showCharacterCounter)
            {
                return $"{_text.Length}/{_maxLength}";
            }
            return _showCharacterCounter ? _text.Length.ToString() : string.Empty;
        }

        #endregion

        #region Icon Methods

        /// <summary>
        /// Set the leading icon using Svgs
        /// </summary>
        public void SetLeadingIcon(string svgPath)
        {
            LeadingIconPath = svgPath;
        }

        /// <summary>
        /// Set the trailing icon using Svgs
        /// </summary>
        public void SetTrailingIcon(string svgPath)
        {
            TrailingIconPath = svgPath;
        }

        /// <summary>
        /// Clear the leading icon
        /// </summary>
        public void ClearLeadingIcon()
        {
            LeadingIconPath = string.Empty;
        }

        /// <summary>
        /// Clear the trailing icon
        /// </summary>
        public void ClearTrailingIcon()
        {
            TrailingIconPath = string.Empty;
        }

        /// <summary>
        /// Toggle password visibility
        /// </summary>
        public void TogglePasswordVisibility()
        {
            UseSystemPasswordChar = !UseSystemPasswordChar;
            
            // Update trailing icon to reflect password visibility state
            if (!string.IsNullOrEmpty(TrailingIconPath))
            {
                TrailingIconPath = UseSystemPasswordChar 
                    ? Svgs.DoorOpen 
                    : Svgs.DoorClosed;
            }
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validate the current text against all validation rules
        /// </summary>
        public bool ValidateText(out string errorMessage)
        {
            if (_validationHelper != null)
            {
                return _validationHelper.ValidateData(out errorMessage);
            }
            else
            {
                errorMessage = string.Empty;
                return true;
            }
        }

        /// <summary>
        /// Validate specific input text
        /// </summary>
        public bool ValidateInput(string input)
        {
            return _validationHelper?.ValidateInput(input) ?? true;
        }

        /// <summary>
        /// Set validation error state
        /// </summary>
        public void SetError(string errorMessage)
        {
            ErrorText = errorMessage;
        }

        /// <summary>
        /// Clear validation error state
        /// </summary>
        public void ClearError()
        {
            ErrorText = string.Empty;
        }

        /// <summary>
        /// Check if field has validation errors
        /// </summary>
        public bool HasValidationError()
        {
            return _hasError || !string.IsNullOrEmpty(_errorText);
        }

        #endregion

        #region Drawing Methods

        /// <summary>
        /// Draw the control content to the specified graphics and rectangle
        /// </summary>
        public void Draw(Graphics graphics, Rectangle rectangle)
        {
            _drawingHelper?.DrawAll(graphics, rectangle);
        }

        /// <summary>
        /// Get the text rectangle for drawing calculations
        /// </summary>
        public Rectangle GetTextRectangle()
        {
            return _materialHelper?.GetTextRectangle() ?? ClientRectangle;
        }

        /// <summary>
        /// Get the label rectangle for drawing calculations
        /// </summary>
        public Rectangle GetLabelRectangle()
        {
            return _materialHelper?.GetLabelRectangle() ?? Rectangle.Empty;
        }

        /// <summary>
        /// Get the helper text rectangle for drawing calculations
        /// </summary>
        public Rectangle GetHelperTextRectangle()
        {
            return _materialHelper?.GetHelperTextRectangle() ?? Rectangle.Empty;
        }

        /// <summary>
        /// Scroll to ensure the caret is visible
        /// </summary>
        public void ScrollToCaret()
        {
            _materialHelper?.ScrollToCaret();
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Get the current line number (1-based)
        /// </summary>
        public int GetCurrentLineNumber()
        {
            return _materialHelper?.GetCurrentLineNumber() ?? 1;
        }

        /// <summary>
        /// Insert text at current caret position
        /// </summary>
        public void InsertText(string textToInsert)
        {
            if (!_readOnly && !string.IsNullOrEmpty(textToInsert))
            {
                _materialHelper?.InsertText(textToInsert);
            }
        }

        /// <summary>
        /// Get suitable size for text and image
        /// </summary>
        public Size GetSuitableSize()
        {
            var imageSize = _beepImage?.HasImage == true ? _beepImage.GetImageSize() : Size.Empty;
            return GetSuitableSizeForTextandImage(imageSize, MaxImageSize, TextImageRelation);
        }

        /// <summary>
        /// Update the control theme
        /// </summary>
        public override void ApplyTheme()
        {
            base.ApplyTheme();
            
            if (_currentTheme != null)
            {
                // Apply theme colors to Material Design properties
                PrimaryColor = _currentTheme.PrimaryColor;
                ErrorColor = _currentTheme.ErrorColor;
                SurfaceColor = _currentTheme.SurfaceColor;
                OutlineColor = _currentTheme.BorderColor;
                
                // Apply theme to image if enabled
                if (ApplyThemeToImage && _beepImage != null)
                {
                    _beepImage.Theme = Theme;
                    _beepImage.ApplyTheme();
                }
                
                // Apply theme to helpers
                _materialHelper?.ApplyTheme(_currentTheme);
            }
            
            Invalidate();
        }

        #endregion

        #region IBeepComponent Implementation

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
            Clear();
        }

        public override bool ValidateData(out string message)
        {
            if (IsRequired && string.IsNullOrWhiteSpace(Text))
            {
                message = "This field is required.";
                SetError(message);
                return false;
            }

            if (MaxLength > 0 && Text.Length > MaxLength)
            {
                message = $"Text length cannot exceed {MaxLength} characters.";
                SetError(message);
                return false;
            }

            bool isValid = ValidateText(out message);
            if (!isValid)
            {
                SetError(message);
            }
            else
            {
                ClearError();
            }

            return isValid;
        }

        #endregion

        #region Keyboard Handling Methods

        /// <summary>
        /// Handle special key combinations
        /// </summary>
        protected virtual bool ProcessKeyboardShortcut(Keys keyData)
        {
            if (_readOnly) return false;

            // Handle Ctrl combinations
            if ((keyData & Keys.Control) == Keys.Control)
            {
                switch (keyData & Keys.KeyCode)
                {
                    case Keys.A:
                        SelectAll();
                        return true;
                    case Keys.C:
                        Copy();
                        return true;
                    case Keys.V:
                        Paste();
                        return true;
                    case Keys.X:
                        Cut();
                        return true;
                    case Keys.Z:
                        if ((keyData & Keys.Shift) == Keys.Shift)
                            Redo();
                        else
                            Undo();
                        return true;
                    case Keys.Y:
                        Redo();
                        return true;
                    case Keys.F:
                        ShowFindReplaceDialog();
                        return true;
                }
            }

            return false;
        }

        #endregion

        #region Focus Management

        /// <summary>
        /// Focus the text field and position cursor
        /// </summary>
        public new void Focus()
        {
            base.Focus();
            _materialHelper?.EnsureCaretVisible();
        }

        /// <summary>
        /// Focus the text field and select all text
        /// </summary>
        public void FocusAndSelectAll()
        {
            Focus();
            SelectAll();
        }

        /// <summary>
        /// Focus the text field and position cursor at end
        /// </summary>
        public void FocusAtEnd()
        {
            Focus();
            SelectionStart = Text.Length;
            SelectionLength = 0;
        }

        /// <summary>
        /// Focus the text field and position cursor at beginning
        /// </summary>
        public void FocusAtBeginning()
        {
            Focus();
            SelectionStart = 0;
            SelectionLength = 0;
        }

        #endregion

        #region Search Box Style Methods

        /// <summary>
        /// Configure the text field to look like a search box
        /// </summary>
        public void ConfigureAsSearchBox(string searchIconPath = null, bool showClearButton = true)
        {
            // Enable search box styling
            SearchBoxStyle = true;
            Variant = MaterialTextFieldVariant.Standard;
            ShowFill = true;
            FillColor = Color.FromArgb(245, 245, 245);
            
            // Configure icons
            if (!string.IsNullOrEmpty(searchIconPath))
            {
                LeadingIconPath = searchIconPath;
            }
            else
            {
                LeadingIconPath = Svgs.Search; // Default search icon
            }
            
            ShowClearButton = showClearButton;
            LeadingIconClickable = true;
            TrailingIconClickable = true;
            
            // Configure placeholder
            if (string.IsNullOrEmpty(PlaceholderText))
            {
                PlaceholderText = "Search...";
            }
            
            // Adjust height for search box appearance
            if (Height < 40)
            {
                Height = 40;
            }
            
            UpdateDrawingRect();
            Invalidate();
        }

        /// <summary>
        /// Configure dual icons for the text field
        /// </summary>
        public void ConfigureDualIcons(string leadingIcon, string trailingIcon, 
                                      bool leadingClickable = true, bool trailingClickable = true)
        {
            LeadingIconPath = leadingIcon ?? string.Empty;
            TrailingIconPath = trailingIcon ?? string.Empty;
            LeadingIconClickable = leadingClickable;
            TrailingIconClickable = trailingClickable;
            
            UpdateDrawingRect();
            Invalidate();
        }

        /// <summary>
        /// Set curved border radius and update appearance
        /// </summary>
        public void SetCurvedRadius(int radius)
        {
            CurvedBorderRadius = Math.Max(0, radius);
            
            // Auto-adjust height if radius is very large
            if (radius > Height / 2)
            {
                Height = Math.Max(Height, radius * 2 + 16);
            }
            
            UpdateDrawingRect();
            Invalidate();
        }

        #endregion
    }
}
