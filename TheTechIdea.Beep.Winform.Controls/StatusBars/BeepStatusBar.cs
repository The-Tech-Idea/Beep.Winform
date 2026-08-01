using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Badges;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.StatusBars
{
    /// <summary>
    /// A status line: a message on the left and right-aligned status segments.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The WPF library has had a <c>BeepStatusBar</c> for some time; WinForms had
    /// the theme contract (<c>StatusBarBackColor</c> and friends, defined by every
    /// theme) but no control to consume it, so anything needing a status line had
    /// to fall back to <c>System.Windows.Forms.StatusStrip</c> and lose the Beep
    /// chrome. This closes that parity gap.
    /// </para>
    /// <para>
    /// Deliberately not Forms-specific. It renders a message and a set of keyed
    /// segments; the caller decides that a segment says "Record 2 of 5". Keeping
    /// it general is also why it does not reference the Forms contracts — this
    /// assembly consumes <c>DataManagementModels</c> as a NuGet package, so
    /// depending on types from the working tree would be a version trap.
    /// </para>
    /// </remarks>
    [ToolboxItem(true)]
    [DisplayName("Beep Status Bar")]
    [Category("Beep Controls")]
    [Description("A themed status line: message text plus right-aligned status segments.")]
    public class BeepStatusBar : BaseControl
    {
        private readonly List<BeepStatusSegment> _segments = new();
        private string _message = string.Empty;

        // Severity reuses the Badges enum rather than declaring a fourth
        // near-identical severity enum in this assembly. Same shape
        // (None/Error/Warning/Info/Success), same library, already public.
        private ValidationState _messageSeverity = ValidationState.None;

        private int _segmentSpacing = 18;
        private int _horizontalPadding = 10;

        public BeepStatusBar()
        {
            Height = 26;
            Dock = DockStyle.Bottom;
            IsChild = false;
            DoubleBuffered = true;
        }

        #region Properties

        [Browsable(true)]
        [Category("Beep")]
        [Description("The message shown on the left of the status line.")]
        [DefaultValue("")]
        public string Message
        {
            get => _message;
            set
            {
                _message = value ?? string.Empty;
                Invalidate();
            }
        }

        [Browsable(true)]
        [Category("Beep")]
        [Description("Severity of the current message; selects the message colour.")]
        [DefaultValue(ValidationState.None)]
        public ValidationState MessageSeverity
        {
            get => _messageSeverity;
            set
            {
                _messageSeverity = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IReadOnlyList<BeepStatusSegment> Segments => _segments;

        [Browsable(true)]
        [Category("Beep")]
        [Description("Gap in pixels between right-aligned segments.")]
        [DefaultValue(18)]
        public int SegmentSpacing
        {
            get => _segmentSpacing;
            set { _segmentSpacing = Math.Max(0, value); Invalidate(); }
        }

        [Browsable(true)]
        [Category("Beep")]
        [Description("Left and right inset for the status line's contents.")]
        [DefaultValue(10)]
        public int HorizontalPadding
        {
            get => _horizontalPadding;
            set { _horizontalPadding = Math.Max(0, value); Invalidate(); }
        }

        #endregion

        #region API

        /// <summary>Shows a message at the given severity.</summary>
        public void SetMessage(string message, ValidationState severity = ValidationState.Info)
        {
            _message = message ?? string.Empty;
            _messageSeverity = string.IsNullOrEmpty(_message) ? ValidationState.None : severity;
            Invalidate();
        }

        /// <summary>Clears the message, leaving the segments untouched.</summary>
        public void ClearMessage()
        {
            _message = string.Empty;
            _messageSeverity = ValidationState.None;
            Invalidate();
        }

        /// <summary>
        /// Adds or updates the segment stored under <paramref name="key"/>.
        /// Keyed rather than indexed so a caller can refresh "record position"
        /// without knowing what else is on the bar.
        /// </summary>
        public void SetSegment(string key, string text, ValidationState severity = ValidationState.None)
        {
            if (string.IsNullOrWhiteSpace(key)) return;

            var existing = _segments.FirstOrDefault(s =>
                string.Equals(s.Key, key, StringComparison.OrdinalIgnoreCase));

            if (existing is null)
            {
                _segments.Add(new BeepStatusSegment(key, text, severity));
            }
            else
            {
                existing.Text = text;
                existing.Severity = severity;
            }

            Invalidate();
        }

        /// <summary>Removes a segment. A key that is not present is not an error.</summary>
        public void RemoveSegment(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return;
            var removed = _segments.RemoveAll(s =>
                string.Equals(s.Key, key, StringComparison.OrdinalIgnoreCase));
            if (removed > 0) Invalidate();
        }

        /// <summary>Removes every segment.</summary>
        public void ClearSegments()
        {
            if (_segments.Count == 0) return;
            _segments.Clear();
            Invalidate();
        }

        /// <summary>Reads a segment's text back, or null when the key is absent.</summary>
        public string GetSegmentText(string key) =>
            _segments.FirstOrDefault(s =>
                string.Equals(s.Key, key, StringComparison.OrdinalIgnoreCase))?.Text;

        #endregion

        #region Painting

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (_currentTheme == null) return;

            BackColor = _currentTheme.StatusBarBackColor;
            ForeColor = _currentTheme.StatusBarForeColor;
            BorderColor = _currentTheme.StatusBarBorderColor;
            Invalidate();
        }

        protected override void DrawContent(Graphics g)
        {
            base.DrawContent(g);
            if (g == null) return;

            var rect = DrawingRect;
            if (rect.Width <= 0 || rect.Height <= 0) return;

            using (var back = new SolidBrush(BackColor))
                g.FillRectangle(back, rect);

            // A single hairline along the top edge is what separates a status
            // line from the content above it.
            using (var pen = new Pen(BorderColor))
                g.DrawLine(pen, rect.Left, rect.Top, rect.Right - 1, rect.Top);

            const TextFormatFlags flags =
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine |
                TextFormatFlags.EndEllipsis |
                TextFormatFlags.NoPrefix;

            // Segments first, right to left, so the message knows how much room
            // is left and truncates instead of overlapping them.
            var rightEdge = rect.Right - _horizontalPadding;
            for (var i = _segments.Count - 1; i >= 0; i--)
            {
                var segment = _segments[i];
                if (string.IsNullOrEmpty(segment.Text)) continue;

                var size = TextRenderer.MeasureText(g, segment.Text, Font, Size.Empty, flags);
                var segmentRect = new Rectangle(
                    rightEdge - size.Width, rect.Top, size.Width, rect.Height);

                if (segmentRect.Left <= rect.Left + _horizontalPadding) break;

                TextRenderer.DrawText(g, segment.Text, Font, segmentRect,
                    ColorFor(segment.Severity), flags);

                rightEdge = segmentRect.Left - _segmentSpacing;
            }

            if (string.IsNullOrEmpty(_message)) return;

            var messageWidth = rightEdge - (rect.Left + _horizontalPadding);
            if (messageWidth <= 0) return;

            var messageRect = new Rectangle(
                rect.Left + _horizontalPadding, rect.Top, messageWidth, rect.Height);

            TextRenderer.DrawText(g, _message, Font, messageRect,
                ColorFor(_messageSeverity), flags);
        }

        private Color ColorFor(ValidationState severity)
        {
            if (_currentTheme == null) return ForeColor;

            return severity switch
            {
                ValidationState.Error => _currentTheme.ErrorColor,
                ValidationState.Warning => _currentTheme.WarningColor,
                ValidationState.Success => _currentTheme.SuccessColor,
                _ => ForeColor
            };
        }

        #endregion
    }

    /// <summary>One right-aligned cell on a <see cref="BeepStatusBar"/>.</summary>
    public class BeepStatusSegment
    {
        public BeepStatusSegment(string key, string text, ValidationState severity = ValidationState.None)
        {
            Key = key;
            Text = text;
            Severity = severity;
        }

        /// <summary>Stable identifier used to update this segment in place.</summary>
        public string Key { get; }

        public string Text { get; set; }

        public ValidationState Severity { get; set; }
    }
}
