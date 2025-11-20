using System;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.DisplayContainers;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Addin;
using TheTechIdea.Beep.Vis.Modules;

namespace WinformSampleApp
{
    public class TabStylePresetsForm : Form
    {
        private BeepDisplayContainer2 _container2;
        private FlowLayoutPanel _panel;

        public TabStylePresetsForm()
        {
            Text = "Tab Style Presets Demo";
            Size = new Size(900, 600);

            _panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(8)
            };

            var btnClassic = new Button { Text = "Classic" };
            btnClassic.Click += (s, e) => { _container2.SetTabStylePreset(TabStyle.Classic); };

            var btnCapsule = new Button { Text = "Capsule" };
            btnCapsule.Click += (s, e) => { _container2.SetTabStylePreset(TabStyle.Capsule); };

            var btnUnderline = new Button { Text = "Underline" };
            btnUnderline.Click += (s, e) => { _container2.SetTabStylePreset(TabStyle.Underline); };

            var btnMinimal = new Button { Text = "Minimal" };
            btnMinimal.Click += (s, e) => { _container2.SetTabStylePreset(TabStyle.Minimal); };

            var btnSegmented = new Button { Text = "Segmented" };
            btnSegmented.Click += (s, e) => { _container2.SetTabStylePreset(TabStyle.Segmented); };

            _panel.Controls.Add(btnClassic);
            _panel.Controls.Add(btnCapsule);
            _panel.Controls.Add(btnUnderline);
            _panel.Controls.Add(btnMinimal);
            _panel.Controls.Add(btnSegmented);

            Controls.Add(_panel);

            _container2 = new BeepDisplayContainer2
            {
                Dock = DockStyle.Fill,
                TabHeight = 36
            };

            // Add sample add-in pages (simple labels)
            var addin1 = new System.Windows.Forms.Panel { BackColor = Color.White, Dock = DockStyle.Fill };
            var lbl1 = new Label { Text = "Home", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            addin1.Controls.Add(lbl1);
            _container2.AddControl("Home", new FakeAddin(addin1), ContainerTypeEnum.TabbedPanel);

            var addin2 = new System.Windows.Forms.Panel { BackColor = Color.WhiteSmoke, Dock = DockStyle.Fill };
            var lbl2 = new Label { Text = "Products", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            addin2.Controls.Add(lbl2);
            _container2.AddControl("Products", new FakeAddin(addin2), ContainerTypeEnum.TabbedPanel);

            var addin3 = new System.Windows.Forms.Panel { BackColor = Color.White, Dock = DockStyle.Fill };
            var lbl3 = new Label { Text = "Contact", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
            addin3.Controls.Add(lbl3);
            _container2.AddControl("Contact", new FakeAddin(addin3), ContainerTypeEnum.TabbedPanel);

            Controls.Add(_container2);
        }

        // Minimal fake addin wrapper that implements IDM_Addin
        class FakeAddin : TheTechIdea.Beep.Addin.IDM_Addin
        {
            public FakeAddin(Control control)
            {
                Control = control;
                Details = new TheTechIdea.Beep.Addin.AddinDetails { AddinName = control?.Name ?? "Fake" };
            }

            public Control Control { get; }
            public Control AddinControl => Control;

            public void ApplyTheme() { if (Control != null) Control.Invalidate(); }
            public void Dispose() { if (Control != null && !Control.IsDisposed) Control.Dispose(); }
            public void Initialize() { }
            public void Resume() { }
            public void Suspend() { }
            public string GuidID { get; set; } = Guid.NewGuid().ToString();
            public TheTechIdea.Beep.Addin.AddinDetails Details { get; set; } = new TheTechIdea.Beep.Addin.AddinDetails();
        }
    }
}
