using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Vis.Modules.Wizards;

namespace TheTechIdea.Beep.Vis.Modules.Wizards
{
    public class WizardManager : IWizardManager
    {
        private readonly BeepWizard _wizard;

        public event EventHandler<WizardNodeChangingEventArgs> NodeChanging;
        public event EventHandler<WizardNodeChangedEventArgs> NodeChanged;
        public event EventHandler FinishRequested;
        public event EventHandler CancelRequested;

        public WizardManager(BeepWizard wizard)
        {
            _wizard = wizard ?? throw new ArgumentNullException(nameof(wizard));

            // Wire events from wizard
            _wizard.NodeChanging += (s, e) =>
            {
                var args = new WizardNodeChangingEventArgs(e.CurrentNode, e.ToNode);
                NodeChanging?.Invoke(this, args);
                e.Cancel = args.Cancel;
            };

            _wizard.NodeChanged += (s, e) =>
            {
                var args = new WizardNodeChangedEventArgs(e.CurrentNode, e.ToNode);
                NodeChanged?.Invoke(this, args);
            };

            _wizard.FinishRequested += (s, e) => FinishRequested?.Invoke(this, e);
            _wizard.CancelRequested += (s, e) => CancelRequested?.Invoke(this, e);
        }

        public string Title
        {
            get => _wizard.Title;
            set => _wizard.Title = value;
        }

        public string Description
        {
            get => _wizard.Description;
            set => _wizard.Description = value;
        }

        public LinkedList<IWizardNode> Nodes
        {
            get => new LinkedList<IWizardNode>(_wizard.Nodes);
            set => throw new NotSupportedException("Setting Nodes is not supported directly.");
        }

        public int Count => _wizard.Count;
        public IWizardNode EntryForm => _wizard.EntryForm;
        public IWizardNode LastForm => _wizard.LastForm;
        public int CurrentIdx => _wizard.CurrentIdx;

        public void InitWizardForm() => _wizard.InitWizardForm();
        public void Show() => _wizard.Show();
        public void Show(IWizardNode node) => _wizard.Show(node);
        public void Hide() => _wizard.Hide();
        public void MoveNext() => _wizard.MoveNext();
        public void MovePrevious() => _wizard.MovePrevious();
    }

    public class WizardNodeChangingEventArgs : CancelEventArgs
    {
        public IWizardNode From { get; }
        public IWizardNode To { get; }

        public WizardNodeChangingEventArgs(IWizardNode from, IWizardNode to)
        {
            From = from;
            To = to;
        }
    }

    public class WizardNodeChangedEventArgs : EventArgs
    {
        public IWizardNode From { get; }
        public IWizardNode To { get; }

        public WizardNodeChangedEventArgs(IWizardNode from, IWizardNode to)
        {
            From = from;
            To = to;
        }
    }
}
