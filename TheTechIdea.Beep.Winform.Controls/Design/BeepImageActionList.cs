using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Design.UIEditor;

namespace TheTechIdea.Beep.Winform.Controls.Design
{
    internal class BeepImageActionList : DesignerActionList
    {
        private BeepImage _beepImage;

        public BeepImageActionList(IComponent component) : base(component)
        {
            _beepImage = (BeepImage)component;
        }

        public void EditImagePath()
        {
            // Here you can open the same dialog you used in your UITypeEditor
            using (var form = new ImageSelectorImporterDialog(_beepImage.ImagePath))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    PropertyDescriptor pd = TypeDescriptor.GetProperties(_beepImage)["ImagePath"];
                    pd.SetValue(_beepImage, form.SelectedImagePath);
                }
            }
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            items.Add(new DesignerActionHeaderItem("ImagePath Options"));
            items.Add(new DesignerActionMethodItem(this, "EditImagePath", "Edit ImagePath...", "ImagePath Options", "Open image selector dialog", true));
            return items;
        }
    }
}
