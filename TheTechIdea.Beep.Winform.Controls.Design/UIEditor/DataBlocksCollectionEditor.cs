using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Linq;
using System.ComponentModel;
using TheTechIdea.Beep.Winform.Controls.Design.Forms;

namespace TheTechIdea.Beep.Winform.Controls.Design.UIEditor
{
    public class DataBlocksCollectionEditor : CollectionEditor
    {
        private readonly IServiceProvider _serviceProvider;

        public DataBlocksCollectionEditor(Type type, IServiceProvider serviceProvider) : base(type)
        {
            _serviceProvider = serviceProvider;
        }

        protected override object CreateInstance(Type itemType)
        {
            return new BeepDataBlock();
        }

        protected override CollectionForm CreateCollectionForm()
        {
            var form = base.CreateCollectionForm();

            // Custom form logic
            form.Load += (s, e) =>
            {
                if (form is CollectionForm collectionForm)
                {
                    var editorSite = _serviceProvider.GetService(typeof(IContainer)) as IContainer;

                    if (editorSite != null)
                    {
                        var blocksOnForm = editorSite.Components
                            .OfType<BeepDataBlock>()
                            .Where(b => !this.Context.Instance.Equals(b)) // Exclude the current block
                            .ToList();

                        var customForm = new DataBlocksCollectionForm(this, blocksOnForm);
                        if (customForm.ShowDialog() == DialogResult.OK)
                        {
                            // Use selected blocks to update collection
                            var selectedBlocks = customForm.GetSelectedBlocks();
                            foreach (var block in selectedBlocks)
                            {
                                this.Context.PropertyDescriptor.SetValue(this.Context.Instance, block);
                            }
                        }
                    }
                }
            };

            return form;
        }
    }
}
