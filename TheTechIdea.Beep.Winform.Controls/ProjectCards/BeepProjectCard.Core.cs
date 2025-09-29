using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.ProjectCards.Painters;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.ProjectCards
{
    public partial class BeepProjectCard : BaseControl
    {
        private readonly Dictionary<ProjectCardPainterKind, IProjectCardPainter> _painters = new();
        private ProjectCardPainterKind _painterKind = ProjectCardPainterKind.CompactProgress;
        private Dictionary<string, object> _parameters = new();

        private string _title = "Project Title";
        private string _subtitle = "Subtitle / Category";
        private float _progress = 48f;
        private string[] _tags = new[] { "Design", "UI", "Sprint" };
        private string _status = "In progress";
        private int _daysLeft = 7;

        // interactive area state
        private readonly Dictionary<string, Rectangle> _areaRects = new();
        private string _hoverArea = null;
        private string _pressedArea = null;

        public BeepProjectCard()
        {
            IsChild = false;
            Padding = new Padding(12);
            Size = new Size(320, 180);
            ApplyThemeToChilds = false;
            ApplyTheme();

            // design-time defaults in param bag
            Parameters[ParamTitle] = _title;
            Parameters[ParamSubtitle] = _subtitle;
            Parameters[ParamProgress] = _progress;
            Parameters[ParamTags] = _tags;
            Parameters[ParamStatus] = _status;
            Parameters[ParamDaysLeft] = _daysLeft;

            // register overlay drawer once
            AddChildExternalDrawing(this, DrawHoverPressedOverlay, DrawingLayer.AfterAll);
        }

        // Events for named areas
        public event EventHandler TitleClicked;
        public event EventHandler ProgressClicked;
        public event EventHandler<PillClickedEventArgs> PillClicked;
        public event EventHandler<AvatarClickedEventArgs> AvatarClicked;
        // Painter-specific optional events
        public event EventHandler StripeClicked;
        public event EventHandler ContentClicked;
        public event EventHandler OutlineClicked;
        public event EventHandler CardBodyClicked;

        [Category("Appearance")]
        public ProjectCardPainterKind PainterKind { get => _painterKind; set { _painterKind = value; Invalidate(); } }

        [Browsable(false)]
        public Dictionary<string, object> Parameters { get => _parameters; set { _parameters = value ?? new(); Invalidate(); } }

        public static readonly string ParamTitle = "Title";
        public static readonly string ParamSubtitle = "Subtitle";
        public static readonly string ParamDescription = "Description";
        public static readonly string ParamProgress = "Progress";
        public static readonly string ParamTags = "Tags";
        public static readonly string ParamAssignees = "Assignees"; // string[] image paths or initials
        public static readonly string ParamStatus = "Status";
        public static readonly string ParamDaysLeft = "DaysLeft";
        public static readonly string ParamIcon = "Icon"; // optional glyph
        public static readonly string ParamAccent = "AccentColor"; // optional Color

        [Category("Content")] public string Title { get => _title; set { _title = value; Invalidate(); } }
        [Category("Content")] public string Subtitle { get => _subtitle; set { _subtitle = value; Invalidate(); } }
        [Category("Content")] public float Progress { get => _progress; set { _progress = value; Invalidate(); } }
        [Category("Content")] public string Status { get => _status; set { _status = value; Invalidate(); } }
        [Category("Content")] public int DaysLeft { get => _daysLeft; set { _daysLeft = value; Invalidate(); } }
        [Category("Content")] public string[] Tags { get => _tags; set { _tags = value ?? System.Array.Empty<string>(); Invalidate(); } }

        public override void ApplyTheme()
        {
            if (_currentTheme == null) return;
            BackColor = _currentTheme.CardBackColor;
            ParentBackColor = _currentTheme.CardBackColor;
            Invalidate();
        }

        protected override void DrawContent(Graphics g)
        {
            var p = GetActivePainter();
            if (p == null) return;
            var rect = DrawingRect;
            rect = new Rectangle(rect.X + Padding.Left, rect.Y + Padding.Top, System.Math.Max(0, rect.Width - Padding.Horizontal), System.Math.Max(0, rect.Height - Padding.Vertical));
            if (rect.Width <= 0 || rect.Height <= 0) return;

            // sync convenience props
            if (!Parameters.ContainsKey(ParamTitle)) Parameters[ParamTitle] = _title;
            if (!Parameters.ContainsKey(ParamSubtitle)) Parameters[ParamSubtitle] = _subtitle;
            if (!Parameters.ContainsKey(ParamProgress)) Parameters[ParamProgress] = _progress;
            if (!Parameters.ContainsKey(ParamStatus)) Parameters[ParamStatus] = _status;
            if (!Parameters.ContainsKey(ParamDaysLeft)) Parameters[ParamDaysLeft] = _daysLeft;
            if (!Parameters.ContainsKey(ParamTags)) Parameters[ParamTags] = _tags;

            // painter draw
            p.Paint(g, rect, _currentTheme, this, Parameters);

            // interactive hit areas registration
            ClearHitList();
            _areaRects.Clear();
            p.UpdateHitAreas(this, rect, _currentTheme, Parameters, (name, r) =>
            {
                _areaRects[name] = r;
                AddHitArea(name, r, this, () => OnAreaClick(name));
            });
        }

        private void DrawHoverPressedOverlay(Graphics g, Rectangle childBounds)
        {
            if (string.IsNullOrEmpty(_hoverArea) && string.IsNullOrEmpty(_pressedArea)) return;
            var name = _pressedArea ?? _hoverArea;
            if (string.IsNullOrEmpty(name)) return;
            if (!_areaRects.TryGetValue(name, out var rect)) return;

            var fill = _pressedArea != null ? (_currentTheme.ProgressBarHoverForeColor.IsEmpty ? Color.FromArgb(60, Color.Black) : Color.FromArgb(80, _currentTheme.ProgressBarHoverForeColor))
                                            : (_currentTheme.ProgressBarHoverBackColor.IsEmpty ? Color.FromArgb(40, Color.Black) : Color.FromArgb(60, _currentTheme.ProgressBarHoverBackColor));
            var border = _currentTheme.ProgressBarHoverBorderColor.IsEmpty ? Color.FromArgb(120, Color.Black) : _currentTheme.ProgressBarHoverBorderColor;

            using var b = new SolidBrush(fill);
            using var p = new Pen(border, 1);
            g.FillRectangle(b, rect);
            g.DrawRectangle(p, rect);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            // Find hovered area
            string newHover = _areaRects.FirstOrDefault(kv => kv.Value.Contains(e.Location)).Key;
            if (newHover != _hoverArea)
            {
                _hoverArea = newHover;
                SetChildExternalDrawingRedraw(this, true);
                Invalidate();
            }
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoverArea = null;
            _pressedArea = null;
            SetChildExternalDrawingRedraw(this, true);
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _pressedArea = _areaRects.FirstOrDefault(kv => kv.Value.Contains(e.Location)).Key;
                if (_pressedArea != null)
                {
                    SetChildExternalDrawingRedraw(this, true);
                    Invalidate();
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_pressedArea != null)
            {
                _pressedArea = null;
                SetChildExternalDrawingRedraw(this, true);
                Invalidate();
            }
        }

        private void OnAreaClick(string name)
        {
            // area-name routing
            if (name.StartsWith("Pill:"))
            {
                var tag = name.Substring("Pill:".Length);
                PillClicked?.Invoke(this, new PillClickedEventArgs(tag));
                return;
            }
            if (name.StartsWith("Avatar:"))
            {
                int index = -1;
                var idxStr = name.Substring("Avatar:".Length);
                int.TryParse(idxStr, out index);
                AvatarClicked?.Invoke(this, new AvatarClickedEventArgs(index));
                return;
            }
            if (name.Contains("Progress"))
            {
                ProgressClicked?.Invoke(this, System.EventArgs.Empty);
                return;
            }
            if (name == "Title")
            {
                TitleClicked?.Invoke(this, System.EventArgs.Empty);
                return;
            }
            if (name == "Stripe")
            {
                StripeClicked?.Invoke(this, System.EventArgs.Empty);
                return;
            }
            if (name == "Content")
            {
                ContentClicked?.Invoke(this, System.EventArgs.Empty);
                return;
            }
            if (name == "Outline")
            {
                OutlineClicked?.Invoke(this, System.EventArgs.Empty);
                return;
            }
            if (name == "CardBody")
            {
                CardBodyClicked?.Invoke(this, System.EventArgs.Empty);
                return;
            }
        }
    }

    public sealed class PillClickedEventArgs : System.EventArgs
    {
        public PillClickedEventArgs(string tag) { Tag = tag; }
        public string Tag { get; }
    }

    public sealed class AvatarClickedEventArgs : System.EventArgs
    {
        public AvatarClickedEventArgs(int index) { Index = index; }
        public int Index { get; }
    }
}
