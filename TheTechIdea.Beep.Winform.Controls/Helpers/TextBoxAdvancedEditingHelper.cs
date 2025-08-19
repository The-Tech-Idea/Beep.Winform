using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    /// <summary>
    /// Advanced text editing features similar to DevExpress TextEdit
    /// Provides syntax highlighting, text formatting, advanced find/replace, and more
    /// </summary>
    public class TextBoxAdvancedEditingHelper
    {
        private readonly IBeepTextBox _textBox;
        private Dictionary<string, TextStyle> _syntaxStyles;
        private List<TextBookmark> _bookmarks;
        private FindReplaceDialog _findDialog;
        private bool _syntaxHighlightingEnabled;
        private SyntaxLanguage _syntaxLanguage;
        
        public event EventHandler<TextFormattedEventArgs> TextFormatted;
        public event EventHandler<BookmarkEventArgs> BookmarkAdded;
        public event EventHandler<BookmarkEventArgs> BookmarkRemoved;
        
        public TextBoxAdvancedEditingHelper(IBeepTextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
            InitializeFeatures();
        }
        
        #region Properties
        
        public bool SyntaxHighlightingEnabled
        {
            get => _syntaxHighlightingEnabled;
            set
            {
                _syntaxHighlightingEnabled = value;
                if (value) ApplySyntaxHighlighting();
            }
        }
        
        public SyntaxLanguage SyntaxLanguage
        {
            get => _syntaxLanguage;
            set
            {
                _syntaxLanguage = value;
                LoadSyntaxRules();
                if (_syntaxHighlightingEnabled) ApplySyntaxHighlighting();
            }
        }
        
        public List<TextBookmark> Bookmarks => _bookmarks;
        
        public bool ShowLineNumbers { get; set; } = false;
        public bool ShowWhitespace { get; set; } = false;
        public bool WordWrapEnabled { get; set; } = false;
        public bool AutoIndentEnabled { get; set; } = true;
        public bool BracketMatchingEnabled { get; set; } = true;
        public bool CodeFoldingEnabled { get; set; } = false;
        
        #endregion
        
        #region Initialization
        
        private void InitializeFeatures()
        {
            _syntaxStyles = new Dictionary<string, TextStyle>();
            _bookmarks = new List<TextBookmark>();
            _syntaxLanguage = SyntaxLanguage.PlainText;
            
            LoadDefaultStyles();
        }
        
        private void LoadDefaultStyles()
        {
            _syntaxStyles["keyword"] = new TextStyle
            {
                ForeColor = Color.Blue,
                FontStyle = FontStyle.Bold
            };
            
            _syntaxStyles["string"] = new TextStyle
            {
                ForeColor = Color.DarkRed,
                FontStyle = FontStyle.Regular
            };
            
            _syntaxStyles["comment"] = new TextStyle
            {
                ForeColor = Color.Green,
                FontStyle = FontStyle.Italic
            };
            
            _syntaxStyles["number"] = new TextStyle
            {
                ForeColor = Color.DarkMagenta,
                FontStyle = FontStyle.Regular
            };
            
            _syntaxStyles["operator"] = new TextStyle
            {
                ForeColor = Color.DarkCyan,
                FontStyle = FontStyle.Bold
            };
        }
        
        private void LoadSyntaxRules()
        {
            // Load language-specific syntax rules
            switch (_syntaxLanguage)
            {
                case SyntaxLanguage.CSharp:
                    LoadCSharpSyntax();
                    break;
                case SyntaxLanguage.SQL:
                    LoadSQLSyntax();
                    break;
                case SyntaxLanguage.XML:
                    LoadXMLSyntax();
                    break;
                case SyntaxLanguage.JSON:
                    LoadJSONSyntax();
                    break;
            }
        }
        
        #endregion
        
        #region Syntax Highlighting
        
        public void ApplySyntaxHighlighting()
        {
            if (!_syntaxHighlightingEnabled) return;
            
            string text = _textBox.Text;
            if (string.IsNullOrEmpty(text)) return;
            
            var highlights = GetSyntaxHighlights(text);
            ApplyHighlights(highlights);
        }
        
        private List<TextHighlight> GetSyntaxHighlights(string text)
        {
            var highlights = new List<TextHighlight>();
            
            switch (_syntaxLanguage)
            {
                case SyntaxLanguage.CSharp:
                    highlights.AddRange(HighlightCSharp(text));
                    break;
                case SyntaxLanguage.SQL:
                    highlights.AddRange(HighlightSQL(text));
                    break;
                case SyntaxLanguage.XML:
                    highlights.AddRange(HighlightXML(text));
                    break;
                case SyntaxLanguage.JSON:
                    highlights.AddRange(HighlightJSON(text));
                    break;
            }
            
            return highlights;
        }
        
        private List<TextHighlight> HighlightCSharp(string text)
        {
            var highlights = new List<TextHighlight>();
            
            // C# Keywords
            string[] keywords = { "class", "public", "private", "static", "void", "int", "string", 
                                "bool", "if", "else", "for", "while", "return", "new", "this", 
                                "using", "namespace", "var", "const", "readonly" };
            
            foreach (string keyword in keywords)
            {
                highlights.AddRange(HighlightPattern(text, $@"\b{keyword}\b", "keyword"));
            }
            
            // String literals
            highlights.AddRange(HighlightPattern(text, @""".*?""", "string"));
            
            // Comments
            highlights.AddRange(HighlightPattern(text, @"//.*$", "comment", RegexOptions.Multiline));
            highlights.AddRange(HighlightPattern(text, @"/\*.*?\*/", "comment", RegexOptions.Singleline));
            
            // Numbers
            highlights.AddRange(HighlightPattern(text, @"\b\d+\.?\d*\b", "number"));
            
            return highlights;
        }
        
        private List<TextHighlight> HighlightSQL(string text)
        {
            var highlights = new List<TextHighlight>();
            
            // SQL Keywords
            string[] keywords = { "SELECT", "FROM", "WHERE", "INSERT", "UPDATE", "DELETE", 
                                "CREATE", "ALTER", "DROP", "TABLE", "INDEX", "JOIN", 
                                "INNER", "LEFT", "RIGHT", "ON", "AS", "GROUP", "BY", 
                                "ORDER", "HAVING", "UNION", "AND", "OR", "NOT" };
            
            foreach (string keyword in keywords)
            {
                highlights.AddRange(HighlightPattern(text, $@"\b{keyword}\b", "keyword", RegexOptions.IgnoreCase));
            }
            
            // String literals
            highlights.AddRange(HighlightPattern(text, @"'.*?'", "string"));
            
            // Comments
            highlights.AddRange(HighlightPattern(text, @"--.*$", "comment", RegexOptions.Multiline));
            
            return highlights;
        }
        
        private List<TextHighlight> HighlightXML(string text)
        {
            var highlights = new List<TextHighlight>();
            
            // XML Tags
            highlights.AddRange(HighlightPattern(text, @"<[^>]+>", "keyword"));
            
            // XML Comments
            highlights.AddRange(HighlightPattern(text, @"<!--.*?-->", "comment", RegexOptions.Singleline));
            
            // Attribute values
            highlights.AddRange(HighlightPattern(text, @""".*?""", "string"));
            
            return highlights;
        }
        
        private List<TextHighlight> HighlightJSON(string text)
        {
            var highlights = new List<TextHighlight>();
            
            // JSON Keys
            highlights.AddRange(HighlightPattern(text, @"""[^""]+""(?=\s*:)", "keyword"));
            
            // JSON String values
            highlights.AddRange(HighlightPattern(text, @"""[^""]+""(?!\s*:)", "string"));
            
            // JSON Numbers
            highlights.AddRange(HighlightPattern(text, @"\b\d+\.?\d*\b", "number"));
            
            // JSON Booleans and null
            highlights.AddRange(HighlightPattern(text, @"\b(true|false|null)\b", "keyword"));
            
            return highlights;
        }
        
        private List<TextHighlight> HighlightPattern(string text, string pattern, string styleName, 
            RegexOptions options = RegexOptions.None)
        {
            var highlights = new List<TextHighlight>();
            var regex = new Regex(pattern, options);
            var matches = regex.Matches(text);
            
            foreach (Match match in matches)
            {
                highlights.Add(new TextHighlight
                {
                    Start = match.Index,
                    Length = match.Length,
                    StyleName = styleName
                });
            }
            
            return highlights;
        }
        
        private void ApplyHighlights(List<TextHighlight> highlights)
        {
            // This would be implemented to apply highlights to the actual text
            // For now, store highlights for rendering
            TextFormatted?.Invoke(this, new TextFormattedEventArgs(highlights));
        }
        
        #endregion
        
        #region Advanced Text Features
        
        public void ShowFindReplaceDialog()
        {
            if (_findDialog == null || _findDialog.IsDisposed)
            {
                _findDialog = new FindReplaceDialog(_textBox);
            }
            
            _findDialog.Show();
        }
        
        public List<int> FindAll(string searchText, bool caseSensitive = false, bool wholeWord = false)
        {
            var positions = new List<int>();
            if (string.IsNullOrEmpty(searchText)) return positions;
            
            string text = _textBox.Text;
            StringComparison comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            
            if (wholeWord)
            {
                var pattern = $@"\b{Regex.Escape(searchText)}\b";
                var options = caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                var matches = Regex.Matches(text, pattern, options);
                
                foreach (Match match in matches)
                {
                    positions.Add(match.Index);
                }
            }
            else
            {
                int index = 0;
                while ((index = text.IndexOf(searchText, index, comparison)) != -1)
                {
                    positions.Add(index);
                    index += searchText.Length;
                }
            }
            
            return positions;
        }
        
        public int ReplaceAll(string searchText, string replaceText, bool caseSensitive = false, bool wholeWord = false)
        {
            var positions = FindAll(searchText, caseSensitive, wholeWord);
            if (positions.Count == 0) return 0;
            
            // Replace from end to beginning to maintain positions
            positions.Reverse();
            
            string text = _textBox.Text;
            foreach (int position in positions)
            {
                text = text.Remove(position, searchText.Length).Insert(position, replaceText);
            }
            
            _textBox.Text = text;
            return positions.Count;
        }
        
        public void GoToLine(int lineNumber)
        {
            var control = _textBox as BeepSimpleTextBox;
            if (control == null || !control.Multiline) return;
            
            var lines = control.GetLines();
            if (lineNumber < 1 || lineNumber > lines.Count) return;
            
            // Calculate position for the line
            int position = 0;
            for (int i = 0; i < lineNumber - 1; i++)
            {
                position += lines[i].Length + Environment.NewLine.Length;
            }
            
            control.SelectionStart = position;
            control.ScrollToCaret();
        }
        
        public void ToggleBookmark(int lineNumber)
        {
            var existing = _bookmarks.FirstOrDefault(b => b.LineNumber == lineNumber);
            if (existing != null)
            {
                _bookmarks.Remove(existing);
                BookmarkRemoved?.Invoke(this, new BookmarkEventArgs(existing));
            }
            else
            {
                var bookmark = new TextBookmark
                {
                    LineNumber = lineNumber,
                    Text = GetLineText(lineNumber),
                    Created = DateTime.Now
                };
                _bookmarks.Add(bookmark);
                BookmarkAdded?.Invoke(this, new BookmarkEventArgs(bookmark));
            }
        }
        
        public void NextBookmark()
        {
            var control = _textBox as BeepSimpleTextBox;
            if (control == null || _bookmarks.Count == 0) return;
            
            int currentLine = GetCurrentLineNumber();
            var nextBookmark = _bookmarks
                .Where(b => b.LineNumber > currentLine)
                .OrderBy(b => b.LineNumber)
                .FirstOrDefault() ?? _bookmarks.OrderBy(b => b.LineNumber).First();
            
            GoToLine(nextBookmark.LineNumber);
        }
        
        public void PreviousBookmark()
        {
            var control = _textBox as BeepSimpleTextBox;
            if (control == null || _bookmarks.Count == 0) return;
            
            int currentLine = GetCurrentLineNumber();
            var prevBookmark = _bookmarks
                .Where(b => b.LineNumber < currentLine)
                .OrderByDescending(b => b.LineNumber)
                .FirstOrDefault() ?? _bookmarks.OrderByDescending(b => b.LineNumber).First();
            
            GoToLine(prevBookmark.LineNumber);
        }
        
        public void FormatDocument()
        {
            switch (_syntaxLanguage)
            {
                case SyntaxLanguage.CSharp:
                    FormatCSharpCode();
                    break;
                case SyntaxLanguage.XML:
                    FormatXMLCode();
                    break;
                case SyntaxLanguage.JSON:
                    FormatJSONCode();
                    break;
                case SyntaxLanguage.SQL:
                    FormatSQLCode();
                    break;
            }
        }
        
        public void CommentSelection()
        {
            var control = _textBox as BeepSimpleTextBox;
            if (control == null) return;
            
            string commentPrefix = GetCommentPrefix();
            if (string.IsNullOrEmpty(commentPrefix)) return;
            
            int start = control.SelectionStart;
            int length = control.SelectionLength;
            
            if (length == 0)
            {
                // Comment current line
                CommentCurrentLine(commentPrefix);
            }
            else
            {
                // Comment selected lines
                CommentSelectedLines(commentPrefix, start, length);
            }
        }
        
        public void UncommentSelection()
        {
            var control = _textBox as BeepSimpleTextBox;
            if (control == null) return;
            
            string commentPrefix = GetCommentPrefix();
            if (string.IsNullOrEmpty(commentPrefix)) return;
            
            int start = control.SelectionStart;
            int length = control.SelectionLength;
            
            if (length == 0)
            {
                // Uncomment current line
                UncommentCurrentLine(commentPrefix);
            }
            else
            {
                // Uncomment selected lines
                UncommentSelectedLines(commentPrefix, start, length);
            }
        }
        
        #endregion
        
        #region Helper Methods
        
        private void LoadCSharpSyntax()
        {
            // Load C# specific syntax rules
        }
        
        private void LoadSQLSyntax()
        {
            // Load SQL specific syntax rules
        }
        
        private void LoadXMLSyntax()
        {
            // Load XML specific syntax rules
        }
        
        private void LoadJSONSyntax()
        {
            // Load JSON specific syntax rules
        }
        
        private void FormatCSharpCode()
        {
            // Basic C# formatting
            string text = _textBox.Text;
            text = text.Replace("{", "{\n").Replace("}", "\n}");
            _textBox.Text = text;
        }
        
        private void FormatXMLCode()
        {
            // Basic XML formatting
            string text = _textBox.Text;
            // Simple XML formatting logic
            _textBox.Text = text;
        }
        
        private void FormatJSONCode()
        {
            // Basic JSON formatting
            try
            {
                string text = _textBox.Text;
                // Would use JSON parser for proper formatting
                _textBox.Text = text;
            }
            catch
            {
                // Invalid JSON, leave as is
            }
        }
        
        private void FormatSQLCode()
        {
            // Basic SQL formatting
            string text = _textBox.Text;
            // Simple SQL formatting logic
            _textBox.Text = text;
        }
        
        private string GetCommentPrefix()
        {
            return _syntaxLanguage switch
            {
                SyntaxLanguage.CSharp => "//",
                SyntaxLanguage.SQL => "--",
                SyntaxLanguage.XML => "<!--",
                _ => "//"
            };
        }
        
        private void CommentCurrentLine(string commentPrefix)
        {
            // Implementation for commenting current line
        }
        
        private void CommentSelectedLines(string commentPrefix, int start, int length)
        {
            // Implementation for commenting selected lines
        }
        
        private void UncommentCurrentLine(string commentPrefix)
        {
            // Implementation for uncommenting current line
        }
        
        private void UncommentSelectedLines(string commentPrefix, int start, int length)
        {
            // Implementation for uncommenting selected lines
        }
        
        private int GetCurrentLineNumber()
        {
            var control = _textBox as BeepSimpleTextBox;
            if (control == null) return 1;
            
            // Calculate current line number
            return 1; // Placeholder
        }
        
        private string GetLineText(int lineNumber)
        {
            var control = _textBox as BeepSimpleTextBox;
            if (control == null) return string.Empty;
            
            var lines = control.GetLines();
            return lineNumber > 0 && lineNumber <= lines.Count ? lines[lineNumber - 1] : string.Empty;
        }
        
        #endregion
        
        #region Cleanup
        
        public void Dispose()
        {
            _findDialog?.Dispose();
            _syntaxStyles?.Clear();
            _bookmarks?.Clear();
        }
        
        #endregion
    }
    
    #region Supporting Classes and Enums
    
    public enum SyntaxLanguage
    {
        PlainText,
        CSharp,
        SQL,
        XML,
        JSON,
        JavaScript,
        Python,
        CSS,
        HTML
    }
    
    public class TextStyle
    {
        public Color ForeColor { get; set; } = Color.Black;
        public Color BackColor { get; set; } = Color.Transparent;
        public FontStyle FontStyle { get; set; } = FontStyle.Regular;
        public bool Underline { get; set; } = false;
        public bool Strikethrough { get; set; } = false;
    }
    
    public class TextHighlight
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public string StyleName { get; set; }
        public Color? CustomForeColor { get; set; }
        public Color? CustomBackColor { get; set; }
    }
    
    public class TextBookmark
    {
        public int LineNumber { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public object Tag { get; set; }
    }
    
    public class TextFormattedEventArgs : EventArgs
    {
        public List<TextHighlight> Highlights { get; }
        
        public TextFormattedEventArgs(List<TextHighlight> highlights)
        {
            Highlights = highlights;
        }
    }
    
    public class BookmarkEventArgs : EventArgs
    {
        public TextBookmark Bookmark { get; }
        
        public BookmarkEventArgs(TextBookmark bookmark)
        {
            Bookmark = bookmark;
        }
    }
    
    // Placeholder for Find/Replace dialog
    public class FindReplaceDialog : Form
    {
        private readonly IBeepTextBox _textBox;
        
        public FindReplaceDialog(IBeepTextBox textBox)
        {
            _textBox = textBox;
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            Text = "Find and Replace";
            Size = new Size(400, 300);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
        }
    }
    
    #endregion
}