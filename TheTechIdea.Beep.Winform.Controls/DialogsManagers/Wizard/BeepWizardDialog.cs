using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.DialogsManagers.Wizard.Painters;

namespace TheTechIdea.Beep.Winform.Controls.DialogsManagers.Wizard
{
    public class BeepWizardDialog : Form
    {
        private readonly Panel _contentHost = new Panel { Dock = DockStyle.Fill };
        private readonly BeepButton _back = new BeepButton { Text = "Back", UseThemeColors = true };
        private readonly BeepButton _next = new BeepButton { Text = "Next", UseThemeColors = true };
        private readonly BeepButton _finish = new BeepButton { Text = "Finish", UseThemeColors = true };
        private readonly BeepButton _cancel = new BeepButton { Text = "Cancel", UseThemeColors = true };
        private int _index;

        public List<WizardPage> Pages { get; } = new();
        public event EventHandler<int>? PageChanged;
        public event EventHandler? Finished;
        public event EventHandler? Cancelled;

        public BeepWizardDialog()
        {
            Text = "Wizard";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(740, 520);
            Controls.Add(_contentHost);
            _back.Click += (s, e) => ChangePage(_index - 1);
            _next.Click += (s, e) => ChangePage(_index + 1);
            _finish.Click += (s, e) => { Finished?.Invoke(this, EventArgs.Empty); DialogResult = DialogResult.OK; Close(); };
            _cancel.Click += (s, e) => { Cancelled?.Invoke(this, EventArgs.Empty); DialogResult = DialogResult.Cancel; Close(); };
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ChangePage(0);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var header = WizardLayoutHelper.GetHeaderRect(ClientRectangle);
            using var titleBrush = new SolidBrush(Color.FromArgb(25, 25, 25));
            string title = _index >= 0 && _index < Pages.Count ? Pages[_index].Title : "Wizard";
            e.Graphics.DrawString(title, Font, titleBrush, header);
            WizardPainter.PaintStepDots(e.Graphics, header, Pages.Count, _index);
            LayoutButtons();
        }

        private void LayoutButtons()
        {
            var b = WizardLayoutHelper.GetButtonsRect(ClientRectangle);
            _cancel.SetBounds(b.Right - 90, b.Y, 80, b.Height);
            _finish.SetBounds(_cancel.Left - 90, b.Y, 80, b.Height);
            _next.SetBounds(_finish.Left - 90, b.Y, 80, b.Height);
            _back.SetBounds(_next.Left - 90, b.Y, 80, b.Height);
            EnsureControl(_back);
            EnsureControl(_next);
            EnsureControl(_finish);
            EnsureControl(_cancel);
            _back.Enabled = _index > 0;
            _next.Enabled = _index < Pages.Count - 1;
            _finish.Enabled = _index == Pages.Count - 1;
        }

        private void EnsureControl(Control c)
        {
            if (!Controls.Contains(c)) Controls.Add(c);
        }

        private void ChangePage(int target)
        {
            if (target < 0 || target >= Pages.Count) return;
            if (_index >= 0 && _index < Pages.Count && Pages[_index].Validate != null && !Pages[_index].Validate!.Invoke())
            {
                return;
            }
            _index = target;
            _contentHost.Controls.Clear();
            var page = Pages[_index];
            if (page.Content != null)
            {
                page.Content.Dock = DockStyle.Fill;
                _contentHost.Controls.Add(page.Content);
            }
            PageChanged?.Invoke(this, _index);
            Invalidate();
        }
    }
}
