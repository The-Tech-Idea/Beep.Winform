using Microsoft.DotNet.DesignTools.Designers.Actions;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Provides design-time verbs for managing the BeepButton image content.
    /// </summary>
    internal sealed class BeepButtonDesigner : BaseBeepControlDesigner, IImagePathDesignerHost
    {
        private DesignerVerbCollection? _verbs;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
            => new DesignerActionListCollection
            {
                new ImagePathDesignerActionList(this)
            };

        public override DesignerVerbCollection Verbs
            => _verbs ??= new DesignerVerbCollection
            {
                new DesignerVerb("Select Image...", OnSelectImage),
                new DesignerVerb("Clear Image", OnClearImage)
            };

        private void OnSelectImage(object sender, EventArgs e)
            => SelectImage();

        private void OnClearImage(object sender, EventArgs e)
            => ClearImage();

        public void SelectImage()
        {
            try
            {
                if (Component == null)
                {
                    return;
                }

                var property = TypeDescriptor.GetProperties(Component)["ImagePath"];
                var serviceProvider = Component.Site ?? (IServiceProvider)GetService(typeof(IServiceProvider));

                using var dialog = new BeepImagePickerDialog(null, embed: false, serviceProvider, Component.GetType().Assembly);
                
                // Don't use IUIService - just show the dialog directly
                var result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    var newValue = dialog.SelectedResourcePath ?? dialog.SelectedFilePath;
                    if (!string.IsNullOrEmpty(newValue))
                    {
                        SetImagePath(newValue);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting image: {ex.Message}\n\n{ex.StackTrace}", "Designer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ClearImage()
        {
            if (Component == null)
            {
                return;
            }

            SetImagePath(string.Empty);
        }

        public string GetImagePath()
            => GetProperty<string>("ImagePath") ?? string.Empty;

        public void SetImagePath(string value)
            => SetProperty("ImagePath", value ?? string.Empty);
    }
}
