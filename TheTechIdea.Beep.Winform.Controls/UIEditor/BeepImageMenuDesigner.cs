using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.Diagnostics;

namespace TheTechIdea.Beep.Winform.Controls.UIEditor
{
    public class BeepImageMenuDesigner : ControlDesigner
    {
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            Debugger.Launch();  // This will prompt you to attach a debugger when the designer loads
            // Add custom context menu item for setting ImagePath
            var menuItem = new DesignerVerb("Set Image Path", OnSetImagePath);
            Verbs.Add(menuItem);
        }

        private void OnSetImagePath(object sender, EventArgs e)
        { // Debug message to check if triggered
            MessageBox.Show("Custom context menu clicked!", "Debug");
            // Cast the component to BeepImage
            var beepImageControl = (BeepImage)Component;

            // Display the ImageSelectorImporterForm
            using (var form = new ImageSelectorImporterForm(beepImageControl.ImagePath))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Set the ImagePath property if the user selects an image
                    PropertyDescriptor property = TypeDescriptor.GetProperties(beepImageControl)["ImagePath"];
                    property.SetValue(beepImageControl, form.SelectedImagePath);
                }
            }
        }
    }
    public class BeepImageDesigner : ControlDesigner
    {
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                var actionLists = new DesignerActionListCollection
                {
                    new ImagePathActionList((BeepImage)Control)
                };
                return actionLists;
            }
        }
    }
}
