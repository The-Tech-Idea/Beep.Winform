using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules.Wizards;

namespace TheTechIdea.Beep.Winform.Controls
{
    public enum WizardViewType
    {
        TopConnectedCircles,
        TopBreadcrumb,
        SideList
    }

    public partial class BeepWizard : BeepControl
    {
        public event EventHandler<NodeChangeEventArgs> NodeChanging;
        public event EventHandler<NodeChangeEventArgs> NodeChanged;
        public event EventHandler FinishRequested;
        public event EventHandler CancelRequested;

        public string Title { get; set; }
        public string Description { get; set; }

        private readonly List<IWizardNode> _nodes = new();
        public IReadOnlyList<IWizardNode> Nodes => _nodes.AsReadOnly();
        public IWizardNode EntryForm => _nodes.Count > 0 ? _nodes[0] : null;
        public IWizardNode LastForm => _nodes.Count > 0 ? _nodes[^1] : null;
        public int CurrentIdx => _currentIndex;
        public int Count => _nodes.Count;

        public WizardViewType ViewType { get; set; } = WizardViewType.TopConnectedCircles;

        private Panel _stepPanel;
        private Panel _progressPanel;
        private Panel _contentPanel;
        private Panel _footerPanel;

        private int _currentIndex = 0;

        private BeepButton btnNext;
        private BeepButton btnPrevious;
        private BeepButton btnFinish;
        private BeepButton btnCancel;

        public BeepWizard()
        {
            DoubleBuffered = true;
            ConnectedCircleWizard();
        }

        public void InitWizardForm()
        {
            Controls.Clear();
            ConnectedCircleWizard();
        }

        public void Show(IWizardNode node)
        {
            int index = _nodes.IndexOf(node);
            if (index >= 0)
            {
                _currentIndex = index;
                ShowCurrentNode();
                UpdateProgressIndicators();
            }
        }

        private void ConnectedCircleWizard()
        {
            _stepPanel = new Panel { Dock = DockStyle.Left, Width = 180, Padding = new Padding(10) };
            _progressPanel = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
            _progressPanel.Paint += ProgressPanel_Paint;
            _contentPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            _footerPanel = new Panel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(10) };

            btnPrevious = new BeepButton { Text = "Previous", Width = 100 };
            btnNext = new BeepButton { Text = "Next", Width = 100 };
            btnFinish = new BeepButton { Text = "Finish", Width = 100, Visible = false };
            btnCancel = new BeepButton { Text = "Cancel", Width = 100 };

            btnPrevious.Click += (s, e) => Previous();
            btnNext.Click += (s, e) => Next();
            btnFinish.Click += (s, e) => FinishRequested?.Invoke(this, EventArgs.Empty);
            btnCancel.Click += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);

            var footerFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                FlowDirection = FlowDirection.RightToLeft
            };
            footerFlow.Controls.AddRange(new Control[] { btnCancel, btnFinish, btnNext, btnPrevious });

            _footerPanel.Controls.Add(footerFlow);

            Controls.AddRange(new Control[] { _contentPanel, _footerPanel, _progressPanel, _stepPanel });

            UpdateStepList();
        }

        private void UpdateStepList()
        {
            _stepPanel.Controls.Clear();
            for (int i = 0; i < _nodes.Count; i++)
            {
                var btn = new BeepButton
                {
                    Text = _nodes[i].Name,
                    Dock = DockStyle.Top,
                    Tag = i,
                    Height = 40,
                    Margin = new Padding(0, 0, 0, 5)
                };
                btn.Click += (s, e) =>
                {
                    var targetIndex = (int)((Control)s).Tag;
                    var args = new NodeChangeEventArgs { CurrentNode = _nodes[_currentIndex], ToNode = _nodes[targetIndex] };
                    NodeChanging?.Invoke(this, args);
                    if (!args.Cancel)
                    {
                        _currentIndex = targetIndex;
                        ShowCurrentNode();
                        UpdateConnectedCircleIndicators();
                        NodeChanged?.Invoke(this, args);
                    }
                };
                _stepPanel.Controls.Add(btn);
            }
            UpdateConnectedCircleIndicators();
        }

        private void UpdateProgressIndicators()
        {
            switch (ViewType)
            {
                case WizardViewType.TopConnectedCircles:
                    UpdateConnectedCircleIndicators();
                    break;
            }
        }

        private void UpdateConnectedCircleIndicators()
        {
            _progressPanel.Controls.Clear();
            _progressPanel.Invalidate();
            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            for (int i = 0; i < _nodes.Count; i++)
            {
                var circle = new BeepCircularButton
                {
                    Text = (i + 1).ToString(),
                    Width = 30,
                    Height = 30,
                    BorderRadius = 15,
                    Margin = new Padding(10, 0, 0, 0),
                    IsHovered = (i == _currentIndex),
                    Tag = i
                };
                flow.Controls.Add(circle);
            }
            _progressPanel.Controls.Add(flow);
        }

        private void ProgressPanel_Paint(object sender, PaintEventArgs e)
        {
            if (ViewType != WizardViewType.TopConnectedCircles) return;
            var g = e.Graphics;
            var buttons = new List<BeepCircularButton>();
            foreach (Control c in _progressPanel.Controls)
            {
                if (c is FlowLayoutPanel f)
                {
                    foreach (Control b in f.Controls)
                    {
                        if (b is BeepCircularButton cb)
                        {
                            buttons.Add(cb);
                        }
                    }
                }
            }

            for (int i = 0; i < buttons.Count - 1; i++)
            {
                var b1 = buttons[i];
                var b2 = buttons[i + 1];
                var p1 = b1.Parent.PointToScreen(new Point(b1.Left + b1.Width, b1.Top + b1.Height / 2));
                var p2 = b2.Parent.PointToScreen(new Point(b2.Left, b2.Top + b2.Height / 2));
                p1 = _progressPanel.PointToClient(p1);
                p2 = _progressPanel.PointToClient(p2);
                g.DrawLine(Pens.Gray, p1, p2);
            }
        }

        public void AddStep(IWizardNode node)
        {
            _nodes.Add(node);
            UpdateStepList();
            if (_nodes.Count == 1)
                ShowCurrentNode();
        }

        public void Next() => MoveNext();

        public void Previous() => MovePrevious();

        public void MoveNext()
        {
            if (_currentIndex < _nodes.Count - 1 && _nodes[_currentIndex].CanMoveNext)
            {
                var args = new NodeChangeEventArgs { CurrentNode = _nodes[_currentIndex], ToNode = _nodes[_currentIndex + 1] };
                NodeChanging?.Invoke(this, args);
                if (!args.Cancel)
                {
                    _currentIndex++;
                    ShowCurrentNode();
                    UpdateProgressIndicators();
                    NodeChanged?.Invoke(this, args);
                }
            }
        }

        public void MovePrevious()
        {
            if (_currentIndex > 0 && _nodes[_currentIndex].CanMovePrevious)
            {
                var args = new NodeChangeEventArgs { CurrentNode = _nodes[_currentIndex], ToNode = _nodes[_currentIndex - 1] };
                NodeChanging?.Invoke(this, args);
                if (!args.Cancel)
                {
                    _currentIndex--;
                    ShowCurrentNode();
                    UpdateProgressIndicators();
                    NodeChanged?.Invoke(this, args);
                }
            }
        }

        private void ShowCurrentNode()
        {
            _contentPanel.Controls.Clear();
            var node = _nodes[_currentIndex];
            if (node?.Page is Control ctrl)
            {
                ctrl.Dock = DockStyle.Fill;
                _contentPanel.Controls.Add(ctrl);
            }

            btnPrevious.Enabled = node.CanMovePrevious;
            btnNext.Enabled = node.CanMoveNext;
            btnFinish.Visible = node.CanFinish;
            btnCancel.Enabled = node.CanCancel;
        }

        public class NodeChangeEventArgs : EventArgs
        {
            public IWizardNode CurrentNode { get; set; }
            public IWizardNode ToNode { get; set; }
            public bool Cancel { get; set; } = false;
        }
    }
}