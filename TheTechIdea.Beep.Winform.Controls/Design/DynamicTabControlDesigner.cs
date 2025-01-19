using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheTechIdea.Beep.Desktop.Common;

namespace TheTechIdea.Beep.Winform.Controls.Design
{
    public class DynamicTabControlDesigner : ParentControlDesigner
    {
        private BeepDynamicTabControl _control;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            _control = component as BeepDynamicTabControl;
            if (_control != null)
            {
                // Register the _contentPanel with the designer
                _control.RegisterControlAsChildForParentControl(_control, _control._contentPanel);

                // Register existing panels in TabPanels
                foreach (Control panel in _control.TabPanels)
                {
                    _control.RegisterControlAsChildForParentControl(_control._contentPanel, panel);
                }
            }
        }

        private void ContentPanel_ControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control is Panel panel)
            {
                panel.AllowDrop = true;
                panel.DragEnter += Panel_DragEnter;
                panel.DragDrop += Panel_DragDrop;
                Console.WriteLine($"[Designer] Subscribed drag-and-drop events to panel: {panel.Name}");
            }
        }

        private void ContentPanel_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (e.Control is Panel panel)
            {
                panel.DragEnter -= Panel_DragEnter;
                panel.DragDrop -= Panel_DragDrop;
                Console.WriteLine($"[Designer] Unsubscribed drag-and-drop events from panel: {panel.Name}");
            }
        }

        protected override bool GetHitTest(System.Drawing.Point point)
        {
            // Allow click-through on the header and child controls
            var relativePoint = _control.PointToClient(point);
            return _control._contentPanel.ClientRectangle.Contains(relativePoint);
        }

        private void EnableDragDropForPanels()
        {
            if (_control == null) return;

            foreach (var panel in _control._contentPanel.Controls.OfType<Panel>())
            {
                panel.AllowDrop = true;
                panel.DragEnter += Panel_DragEnter;
                panel.DragDrop += Panel_DragDrop;
                Console.WriteLine($"[Designer] Subscribed drag-and-drop events to panel: {panel.Name}");
            }
        }

        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Control)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Panel_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(Control)) is Control draggedControl)
            {
                if (sender is Panel targetPanel)
                {
                    var dropPoint = targetPanel.PointToClient(new System.Drawing.Point(e.X, e.Y));
                    draggedControl.Location = dropPoint;
                    targetPanel.Controls.Add(draggedControl);
                    Console.WriteLine($"[Designer] Dropped control '{draggedControl.Name}' into panel '{targetPanel.Name}' at {dropPoint}");
                }
            }
        }

        public override void OnSetComponentDefaults()
        {
            base.OnSetComponentDefaults();

            if (_control != null && _control.Tabs.Count == 0)
            {
                var defaultTab = new SimpleItem { Text = "Tab 1" };
                _control.Tabs.Add(defaultTab);
                Console.WriteLine("[Designer] Set default tab 'Tab 1'");
            }
        }
    }
}
