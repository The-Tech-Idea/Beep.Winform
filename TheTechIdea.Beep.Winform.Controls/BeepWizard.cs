
using System.ComponentModel;
using System.Text;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Vis.Modules.Wizards;
 
using TheTechIdea.Beep.Winform.Controls.Models;
 

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

        private string logopath = "logo.svg";

        [Browsable(true)]
        [Category("Appearance")]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Select the image file (SVG, PNG, JPG, etc.) to load.")]
        public string LogoPath
        {
            get => logopath;
            set
            {
                logopath = value;
                if (wizardImage != null)
                {
                    wizardImage.ImagePath = logopath;
                }
            }
        }

        public WizardViewType ViewType
        {
            get => _viewType;
            set
            {
                _viewType = value;
                UpdateViewLayout();
            }
        }

        private BeepPanel _topPanel;
        private BeepPanel _sidePanel;
        private BeepPanel _contentPanel;
        private BeepPanel _footerPanel;

        private BeepStepperBar stepperBar;
        private BeepStepperBreadCrumb breadCrumb;
        private BeepImage wizardImage;

        private int _currentIndex = 0;

        private BeepButton btnNext;
        private BeepButton btnPrevious;
        private BeepButton btnFinish;
        private BeepButton btnCancel;
        private WizardViewType _viewType = WizardViewType.TopBreadcrumb;

        private Control previousControl;
        private Timer transitionTimer;
        private int animationStep;
        private const int AnimationSteps = 10;
        private int slideDirection = 1; // 1 = forward, -1 = backward

        public BeepWizard():base()
        {
            DoubleBuffered = true;
            IsRoundedAffectedByTheme = true;
            IsShadowAffectedByTheme = true;
           
            ApplyThemeToChilds = true;
            InitWizardForm();
            InitSlideAnimation();
        }

        private void InitSlideAnimation()
        {
            transitionTimer = new Timer { Interval = 15 };
            transitionTimer.Tick += (s, e) => PerformSlideAnimation();
        }

        private void PerformSlideAnimation()
        {
            if (previousControl == null || _contentPanel.Controls.Count == 0) return;

            var newControl = _contentPanel.Controls[0];
            int shift = (_contentPanel.Width / AnimationSteps) * slideDirection;
            newControl.Left -= shift;
            previousControl.Left -= shift;
            animationStep++;

            if (animationStep >= AnimationSteps)
            {
                transitionTimer.Stop();
                _contentPanel.Controls.Remove(previousControl);
                previousControl.Dispose();
                previousControl = null;
                animationStep = 0;
            }
        }

        public void InitWizardForm()
        {
            Controls.Clear();

            _topPanel = new BeepPanel {IsRounded=false,IsRoundedAffectedByTheme=false, ShowTitle=false,  Dock = DockStyle.Top, Height = 80, Padding = new Padding(0) };
            _sidePanel = new BeepPanel { IsRounded = false, IsRoundedAffectedByTheme = false, ShowTitle = false, Dock = DockStyle.Left, Width = 180, Padding = new Padding(0) };
            _contentPanel = new BeepPanel { IsRounded = false, IsRoundedAffectedByTheme = false, ShowTitle = false, Dock = DockStyle.Fill, Padding = new Padding(0), AutoScroll = false };
            _footerPanel = new BeepPanel { IsRounded = false, IsRoundedAffectedByTheme = false, ShowTitle = false, Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(0) };

            btnPrevious = new BeepButton { IsRounded = false, IsRoundedAffectedByTheme = false, Text = "Previous", Width = 100 };
            btnNext = new BeepButton { IsRounded = false, IsRoundedAffectedByTheme = false, Text = "Next", Width = 100 };
            btnFinish = new BeepButton { IsRounded = false, IsRoundedAffectedByTheme = false, Text = "Finish", Width = 100, Visible = false };
            btnCancel = new BeepButton { IsRounded = false, IsRoundedAffectedByTheme = false, Text = "Cancel", Width = 100 };

            btnPrevious.Click += (s, e) => { slideDirection = -1; MovePrevious(); };
            btnNext.Click += (s, e) => { slideDirection = 1; MoveNext(); };
            btnFinish.Click += (s, e) => FinishRequested?.Invoke(this, EventArgs.Empty);
            btnCancel.Click += (s, e) => CancelRequested?.Invoke(this, EventArgs.Empty);

            btnPrevious.Margin = btnNext.Margin = btnFinish.Margin = btnCancel.Margin = new Padding(5, 0, 0, 0);

            var footerFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.RightToLeft
            };
            footerFlow.Controls.AddRange(new Control[] { btnCancel, btnFinish, btnNext, btnPrevious });
            _footerPanel.Controls.Add(footerFlow);

            Controls.AddRange(new Control[] { _contentPanel, _footerPanel, _topPanel, _sidePanel });

            UpdateViewLayout();
        }

        private void UpdateViewLayout()
        {
            _topPanel.Controls.Clear();
            _sidePanel.Controls.Clear();

            switch (ViewType)
            {
                case WizardViewType.TopConnectedCircles:
                    stepperBar = new BeepStepperBar
                    {
                        IsRounded = false,
                        IsRoundedAffectedByTheme = false,
                        IsFrameless = true,
                        IsChild = true,
                        Dock = DockStyle.Fill,
                        Orientation = Orientation.Horizontal,
                        ListItems = GetStepItems()
                    };
                    wizardImage = new BeepImage { Dock = DockStyle.Left, IsChild = true, Width = 80, ImagePath = logopath };
                    _topPanel.Controls.Add(stepperBar);
                    _sidePanel.Controls.Add(wizardImage);
                    break;

                case WizardViewType.TopBreadcrumb:
                    breadCrumb = new BeepStepperBreadCrumb
                    {
                        IsRounded = false,
                        IsRoundedAffectedByTheme = false,
                        IsFrameless = true,
                        IsChild = true,
                        Dock = DockStyle.Fill,
                        ListItems = GetStepItems()
                    };
                    wizardImage = new BeepImage {
                        IsRounded = false,
                        IsRoundedAffectedByTheme = false,
                        IsFrameless = true,
                        IsChild = true,
                        Dock = DockStyle.Left,  Width = 80, ImagePath = logopath };
                    _topPanel.Controls.Add(breadCrumb);
                    _sidePanel.Controls.Add(wizardImage);
                    break;

                case WizardViewType.SideList:
                    stepperBar = new BeepStepperBar
                    {
                        IsRounded = false,
                        IsRoundedAffectedByTheme = false,
                        IsFrameless = true,
                        IsChild = true,
                        Dock = DockStyle.Fill,
                        Orientation = Orientation.Vertical,
                        ListItems = GetStepItems()
                    };
                    wizardImage = new BeepImage { Dock = DockStyle.Fill, IsChild = true, Width = 80, ImagePath = logopath };
                    _sidePanel.Controls.Add(stepperBar);
                    _topPanel.Controls.Add(wizardImage);
                    break;
            }

            ShowCurrentNode();
        }

        private BindingList<SimpleItem> GetStepItems()
        {
            var items = new BindingList<SimpleItem>();
            for (int i = 0; i < _nodes.Count; i++)
            {
                items.Add(new SimpleItem { ID = i, Name = $"Step {i + 1}", Text = _nodes[i].Name, IsChecked = i <= _currentIndex });
            }
            return items;
        }

        private void ShowCurrentNode()
        {
            if (_nodes.Count <= 0) return;

            var node = _nodes[_currentIndex];
            if (node?.Page is Control ctrl)
            {
                ctrl.Dock = DockStyle.Fill;
                ctrl.Left = slideDirection > 0 ? _contentPanel.Width : -_contentPanel.Width;

                previousControl = _contentPanel.Controls.Count > 0 ? _contentPanel.Controls[0] : null;
                if (previousControl != null) previousControl.Dock = DockStyle.None;

                _contentPanel.Controls.Add(ctrl);
                animationStep = 0;
                transitionTimer.Start();
            }

            btnPrevious.Enabled = node.CanMovePrevious;
            btnNext.Enabled = node.CanMoveNext;
            btnFinish.Visible = node.CanFinish;
            btnCancel.Enabled = node.CanCancel;

            if (stepperBar != null) stepperBar.UpdateCurrentStep(_currentIndex);
            if (breadCrumb != null) breadCrumb.UpdateCurrentStep(_currentIndex);
        }

        public void AddStep(IWizardNode node)
        {
            _nodes.Add(node);
            UpdateViewLayout();
        }

        public void AddSteps(IEnumerable<IWizardNode> nodes)
        {
            foreach (var node in nodes)
            {
                _nodes.Add(node);
            }
            UpdateViewLayout();
        }

        public void RemoveStep(IWizardNode node)
        {
            if (_nodes.Contains(node))
            {
                _nodes.Remove(node);
                UpdateViewLayout();
            }
        }

        public void RemoveStep(int index)
        {
            if (index >= 0 && index < _nodes.Count)
            {
                _nodes.RemoveAt(index);
                UpdateViewLayout();
            }
        }

        public void ClearSteps()
        {
            _nodes.Clear();
            UpdateViewLayout();
        }

        public void Show() => ShowCurrentNode();

        public void Show(IWizardNode node)
        {
            if (_nodes.Count <= 0 || node == null) return;
            int index = _nodes.IndexOf(node);
            if (index >= 0 && index < _nodes.Count)
            {
                _currentIndex = index;
                ShowCurrentNode();
            }
        }

        public void MoveNext()
        {
            if (_nodes.Count <= 0) return;
            if (_currentIndex < _nodes.Count - 1 && _nodes[_currentIndex].CanMoveNext)
            {
                var args = new NodeChangeEventArgs { CurrentNode = _nodes[_currentIndex], ToNode = _nodes[_currentIndex + 1] };
                NodeChanging?.Invoke(this, args);
                if (!args.Cancel)
                {
                    _currentIndex++;
                    ShowCurrentNode();
                    NodeChanged?.Invoke(this, args);
                }
            }
        }

        public void MovePrevious()
        {
            if (_nodes.Count <= 0) return;
            if (_currentIndex > 0 && _nodes[_currentIndex].CanMovePrevious)
            {
                var args = new NodeChangeEventArgs { CurrentNode = _nodes[_currentIndex], ToNode = _nodes[_currentIndex - 1] };
                NodeChanging?.Invoke(this, args);
                if (!args.Cancel)
                {
                    _currentIndex--;
                    ShowCurrentNode();
                    NodeChanged?.Invoke(this, args);
                }
            }
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            if (stepperBar != null) stepperBar.Theme = Theme;
            if (breadCrumb != null) breadCrumb.Theme = Theme;
            if (wizardImage != null) wizardImage.Theme = Theme;
            if (btnNext != null) btnNext.Theme = Theme;
            if (btnPrevious != null) btnPrevious.Theme = Theme;
            if (btnFinish != null) btnFinish.Theme = Theme;
            if (btnCancel != null) btnCancel.Theme = Theme;

            _topPanel.BackColor = _currentTheme.AppBarTitleBackColor;
            _sidePanel.BackColor = _currentTheme.SideMenuBackColor;
            _contentPanel.BackColor = _currentTheme.CardBackColor;
            _footerPanel.BackColor = _currentTheme.SideMenuBackColor;
        }

        public class NodeChangeEventArgs : EventArgs
        {
            public IWizardNode CurrentNode { get; set; }
            public IWizardNode ToNode { get; set; }
            public bool Cancel { get; set; } = false;
        }
    }
}
