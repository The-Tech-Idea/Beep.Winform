using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.TextFields.Helpers;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Helper methods for BeepTextBox
    /// </summary>
    public partial class BeepTextBox
    {
        #region "Helper Methods"
        
        private void UpdateDrawingRect()
        {
            base.UpdateDrawingRect();

            Rectangle contentArea;
            if (PainterKind == BaseControlPainterKind.Material)
            {
                contentArea = GetMaterialContentRectangle();

                if (contentArea.Width <= 0 || contentArea.Height <= 0)
                {
                    contentArea = DrawingRect;
                }
            }
            else
            {
                contentArea = DrawingRect;
            }

            _textRect = contentArea;

            int borderOffset = Math.Max(0, _borderWidth);
            _textRect.Inflate(-(Padding.Horizontal / 2 + borderOffset), -(Padding.Vertical / 2 + borderOffset));
            
            if (_showLineNumbers && _multiline)
            {
                _textRect.X += _lineNumberMarginWidth;
                _textRect.Width = Math.Max(1, _textRect.Width - _lineNumberMarginWidth);
            }
            
            if (_showCharacterCount && _maxLength > 0)
            {
                try
                {
                    using (var g = CreateGraphics())
                    using (var font = new Font(_textFont.FontFamily, _textFont.Size * 0.8f))
                    {
                        var charCountHeight = (int)Math.Ceiling(TextUtils.MeasureText(g,"0/0", font).Height);
                        _textRect.Height = Math.Max(1, _textRect.Height - charCountHeight);
                    }
                }
                catch
                {
                    // Ignore
                }
            }
        }
        
        private void InvalidateLayout()
        {
            _needsLayoutUpdate = true;
            _delayedUpdateTimer?.Stop();
            _delayedUpdateTimer?.Start();
        }
        
        public string GetDisplayText()
        {
            string actualText = GetActualText();
            
            if (string.IsNullOrEmpty(actualText))
            {
                return string.IsNullOrEmpty(PlaceholderText) ? "" : PlaceholderText;
            }
            
            return actualText;
        }
        
        private string GetActualText()
        {
            if (string.IsNullOrEmpty(_text) ||
                _text.StartsWith("BeepTextBox") ||
                _text.Equals(Name) ||
                (_isInitializing && string.IsNullOrWhiteSpace(_text)))
            {
                return string.Empty;
            }
            
            if (_useSystemPasswordChar && !string.IsNullOrEmpty(_text))
            {
                return new string('•', _text.Length);
            }
            else if (_passwordChar != '\0' && !string.IsNullOrEmpty(_text))
            {
                return new string(_passwordChar, _text.Length);
            }
            
            return _text;
        }
        
        public System.Collections.Generic.List<string> GetLines() => new System.Collections.Generic.List<string>(_lines);
        
        private (int LineIndex, int ColumnIndex) GetLineInfoFromPosition(int position, System.Collections.Generic.List<string> lines)
        {
            if (lines.Count == 0) return (0, 0);
            if (position <= 0) return (0, 0);
            if (position >= _text.Length) return (lines.Count - 1, lines[lines.Count - 1].Length);

            int currentPos = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                if (position <= currentPos + lines[i].Length)
                {
                    return (i, position - currentPos);
                }
                currentPos += lines[i].Length;
                if (i < lines.Count - 1)
                {
                    currentPos += Environment.NewLine.Length;
                }
            }
            return (lines.Count - 1, lines[lines.Count - 1].Length);
        }
        
        private int GetPositionFromLineInfo(int lineIndex, int columnIndex, System.Collections.Generic.List<string> lines)
        {
            if (lineIndex >= lines.Count) return _text.Length;
            if (lineIndex < 0) return 0;

            int i = 0;
            int currentLine = 0;

            while (i < _text.Length && currentLine < lineIndex)
            {
                if (_text[i] == '\r')
                {
                    if (i + 1 < _text.Length && _text[i + 1] == '\n')
                    {
                        i += 2;
                    }
                    else
                    {
                        i++;
                    }
                    currentLine++;
                }
                else if (_text[i] == '\n')
                {
                    i++;
                    currentLine++;
                }
                else
                {
                    i++;
                }
            }

            int targetPosition = i + columnIndex;
            return Math.Min(targetPosition, _text.Length);
        }
        
        private void ValidateCaretPosition()
        {
            if (_helper?.Caret != null)
            {
                int textLength = _text?.Length ?? 0;
                
                if (_helper.Caret.CaretPosition > textLength)
                {
                    _helper.Caret.CaretPosition = textLength;
                }
                
                if (_helper.Caret.SelectionStart > textLength)
                {
                    _helper.Caret.SelectionStart = textLength;
                    _helper.Caret.SelectionLength = 0;
                }
                else if (_helper.Caret.SelectionStart + _helper.Caret.SelectionLength > textLength)
                {
                    _helper.Caret.SelectionLength = textLength - _helper.Caret.SelectionStart;
                }
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
                // Just split lines - do NOT perform word wrapping here
                string[] splitLines = _text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                _lines.AddRange(splitLines);
                
                // NOTE: Word wrapping is now handled during paint/draw operations
                // when we have a valid Graphics context, not during UpdateLines()
            }
            else
            {
                // For single line, replace all line breaks with spaces
                string singleLineText = _text.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                _lines.Add(singleLineText);
            }
            
            // Ensure we always have at least one line
            if (_lines.Count == 0)
                _lines.Add("");
        }
        
        private System.Collections.Generic.List<string> WrapLine(string line, Graphics g)
        {
            var result = new System.Collections.Generic.List<string>();
            if (string.IsNullOrEmpty(line))
            {
                result.Add("");
                return result;
            }
            
            // Add comprehensive safety checks
            if (g == null || _textFont == null || _textRect.Width <= 0)
            {
                result.Add(line);
                return result;
            }
            
            // Additional check for control state
            if (IsDisposed || !IsHandleCreated || _isInitializing)
            {
                result.Add(line);
                return result;
            }
            
            try
            {
                var lineSize = TextUtils.MeasureText(g,line, _textFont);
                if (lineSize.Width <= _textRect.Width)
                {
                    result.Add(line);
                    return result;
                }
            }
            catch (ArgumentException)
            {
                // MeasureString failed - return line as-is
                result.Add(line);
                return result;
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ObjectDisposedException)
            {
                // Graphics or font became invalid
                result.Add(line);
                return result;
            }
            
            var words = line.Split(' ');
            var currentLine = "";
            
            foreach (var word in words)
            {
                var testLine = string.IsNullOrEmpty(currentLine) ? word : $"{currentLine} {word}";
                
                try
                {
                    var testSize = TextUtils.MeasureText(g,testLine, _textFont);
                    
                    if (testSize.Width <= _textRect.Width)
                    {
                        currentLine = testLine;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(currentLine))
                        {
                            result.Add(currentLine);
                            currentLine = word;
                        }
                        else
                        {
                            try
                            {
                                if (TextUtils.MeasureText(g,word, _textFont).Width > _textRect.Width)
                                {
                                    result.AddRange(WrapLongWord(word, g));
                                    currentLine = "";
                                }
                                else
                                {
                                    result.Add(word);
                                    currentLine = "";
                                }
                            }
                            catch (ArgumentException)
                            {
                                // If word measurement fails, add it as-is
                                result.Add(word);
                                currentLine = "";
                            }
                        }
                    }
                }
                catch (ArgumentException)
                {
                    // MeasureString failed for testLine
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        result.Add(currentLine);
                    }
                    result.Add(word);
                    currentLine = "";
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is ObjectDisposedException)
                {
                    // Graphics became invalid mid-operation
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        result.Add(currentLine);
                    }
                    result.Add(word);
                    currentLine = "";
                }
            }
            
            if (!string.IsNullOrEmpty(currentLine))
            {
                result.Add(currentLine);
            }
            
            // Ensure we always return at least the original line
            if (result.Count == 0)
            {
                result.Add(line);
            }
            
            return result;
        }
        
        private System.Collections.Generic.List<string> WrapLongWord(string word, Graphics g)
        {
            var result = new System.Collections.Generic.List<string>();
            var currentPart = "";
            
            foreach (char c in word)
            {
                var testPart = currentPart + c;
                var testSize = TextUtils.MeasureText(g,testPart, _textFont);
                
                if (testSize.Width <= _textRect.Width)
                {
                    currentPart = testPart;
                }
                else
                {
                    if (!string.IsNullOrEmpty(currentPart))
                    {
                        result.Add(currentPart);
                        currentPart = c.ToString();
                    }
                    else
                    {
                        result.Add(c.ToString());
                        currentPart = "";
                    }
                }
            }
            
            if (!string.IsNullOrEmpty(currentPart))
            {
                result.Add(currentPart);
            }
            
            return result;
        }
        
        public override Size GetPreferredSize(Size proposedSize)
        {
            if (DisableMaterialConstraints)
            {
                using (Graphics g = CreateGraphics())
                {
                    var textSize = TextUtils.MeasureText(g,string.IsNullOrEmpty(Text) ? PlaceholderText : Text, TextFont);
                    int width = (int)Math.Ceiling(textSize.Width) + Padding.Horizontal + (BorderWidth * 2);
                    int height = (int)Math.Ceiling(textSize.Height) + Padding.Vertical + (BorderWidth * 2);
                    return new Size(width, height);
                }
            }

            return base.GetPreferredSize(proposedSize);
        }
        
        #endregion
        
        #region "IBeepComponent Implementation"
        
        public bool ValidateData(out string message)
        {
            return _helper?.ValidateAllData(out message) ?? EntityHelper.ValidateData(MaskFormat, Text, CustomMask, out message);
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
    }
}
