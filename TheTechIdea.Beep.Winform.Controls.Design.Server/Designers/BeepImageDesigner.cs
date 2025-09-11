using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Design.Server.Designers;

namespace TheTechIdea.Beep.Winform.Controls.MDI.Designers
{
    public class BeepImageDesigner : ControlDesigner
    {
        private DesignerActionListCollection _lists;
        public override DesignerActionListCollection ActionLists => _lists ??= new DesignerActionListCollection { new BeepImageActionList(this) };
        internal BeepImage ImageControl => Control as BeepImage;
    }

    internal class BeepImageActionList : DesignerActionList
    {
        private readonly BeepImageDesigner _designer;
        private readonly IComponentChangeService _change;
        public BeepImageActionList(BeepImageDesigner designer) : base(designer.Component)
        {
            _designer = designer;
            _change = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
        }

        private BeepImage C => _designer.ImageControl;

        public string ImagePath
        {
            get => C?.ImagePath;
            set
            {
                if (C == null) return;
                var p = TypeDescriptor.GetProperties(C)[nameof(C.ImagePath)];
                var old = p.GetValue(C);
                if (Equals(old, value)) return;
                _change?.OnComponentChanging(C, p);
                p.SetValue(C, value);
                _change?.OnComponentChanged(C, p, old, value);
            }
        }

        public void SelectImage() => ShowPicker(false);
        public void EmbedImage() => ShowPicker(true);

        private void ShowPicker(bool embed)
        {
            if (C == null) return;
            using var dlg = new BeepImagePickerDialog(C, embed, (IServiceProvider)C.Site);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                ImagePath = dlg.SelectedResourcePath ?? dlg.SelectedFilePath;
            }
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            return new DesignerActionItemCollection
            {
                new DesignerActionHeaderItem("Image"),
                new DesignerActionPropertyItem(nameof(ImagePath),"Image Path"),
                new DesignerActionMethodItem(this,nameof(SelectImage),"Select / Link Image"),
                new DesignerActionMethodItem(this,nameof(EmbedImage),"Add & Embed Image")
            };
        }
    }

    
}
