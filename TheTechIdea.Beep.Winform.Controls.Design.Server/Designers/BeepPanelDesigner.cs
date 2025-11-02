using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Designers.Actions;
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for container panels, exposing verbs for image and layout helpers.
    /// </summary>
    internal sealed class BeepPanelDesigner : ParentControlDesigner, IImagePathDesignerHost
    {
        private IComponentChangeService _changeService;
        private DesignerVerbCollection _verbs;
        private DesignerActionListCollection _actionLists;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            EnableDragDrop(true);
        }

        public override DesignerVerbCollection Verbs
            => _verbs ??= new DesignerVerbCollection
            {
                new DesignerVerb("Select Background Image...", OnSelectBackgroundImage),
                new DesignerVerb("Clear Background Image", OnClearBackgroundImage)
            };

        public override DesignerActionListCollection ActionLists
            => _actionLists ??= new DesignerActionListCollection
            {
                new ImagePathDesignerActionList(this)
            };

        private void OnSelectBackgroundImage(object sender, EventArgs e)
            => SelectImage();

        private void OnClearBackgroundImage(object sender, EventArgs e)
            => ClearImage();

        public void SelectImage()
        {
            if (Component == null)
            {
                return;
            }

            var property = TypeDescriptor.GetProperties(Component)["ImagePath"];
            var serviceProvider = Component.Site ?? (IServiceProvider)GetService(typeof(IServiceProvider));

            using var dialog = new BeepImagePickerDialog(null, embed: false, serviceProvider, Component.GetType().Assembly);
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

        public void ClearImage()
        {
            if (Component == null)
            {
                return;
            }

            SetImagePath(string.Empty);
        }

        public string GetImagePath()
        {
            if (Component == null)
            {
                return string.Empty;
            }

            var property = TypeDescriptor.GetProperties(Component)["ImagePath"];
            return property?.GetValue(Component) as string ?? string.Empty;
        }

        public void SetImagePath(string value)
        {
            if (Component == null)
            {
                return;
            }

            var property = TypeDescriptor.GetProperties(Component)["ImagePath"];
            if (property == null)
            {
                return;
            }

            var current = property.GetValue(Component) as string;
            if (string.Equals(current ?? string.Empty, value ?? string.Empty, StringComparison.Ordinal))
            {
                return;
            }

            _changeService?.OnComponentChanging(Component, property);
            property.SetValue(Component, value);
            _changeService?.OnComponentChanged(Component, property, current, value);
        }
    }
}
