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
    /// Designer verbs for BeepLabel to streamline image selection.
    /// </summary>
    internal sealed class BeepLabelDesigner : ControlDesigner, IImagePathDesignerHost
    {
        private IComponentChangeService _changeService;
        private DesignerVerbCollection _verbs;
        private DesignerActionListCollection _actionLists;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            _changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
        }

        public override DesignerVerbCollection Verbs
            => _verbs ??= new DesignerVerbCollection
            {
                new DesignerVerb("Select Image...", OnSelectImage),
                new DesignerVerb("Clear Image", OnClearImage),
                new DesignerVerb("Preset: Text Only", (s, e) => ApplyPreset(textOnly: true, imageTop: false, titleCard: false)),
                new DesignerVerb("Preset: Image Left", (s, e) => ApplyPreset(textOnly: false, imageTop: false, titleCard: false)),
                new DesignerVerb("Preset: Image Top", (s, e) => ApplyPreset(textOnly: false, imageTop: true, titleCard: false)),
                new DesignerVerb("Preset: Title + Subheader", (s, e) => ApplyPreset(textOnly: true, imageTop: false, titleCard: true)),
                new DesignerVerb("Toggle Theme Image Tint", (s, e) => ToggleApplyThemeOnImage())
            };

        public override DesignerActionListCollection ActionLists
            => _actionLists ??= new DesignerActionListCollection
            {
                new ImagePathDesignerActionList(this)
            };

        private void OnSelectImage(object sender, EventArgs e)
            => SelectImage();

        private void OnClearImage(object sender, EventArgs e)
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

        private void ApplyPreset(bool textOnly, bool imageTop, bool titleCard)
        {
            if (Component == null)
            {
                return;
            }

            var properties = TypeDescriptor.GetProperties(Component);
            SetProperty(properties, "HideText", false);
            SetProperty(properties, "TextImageRelation", textOnly
                ? TextImageRelation.Overlay
                : (imageTop ? TextImageRelation.ImageAboveText : TextImageRelation.ImageBeforeText));
            SetProperty(properties, "TextAlign", titleCard ? ContentAlignment.TopLeft : ContentAlignment.MiddleLeft);
            SetProperty(properties, "ImageAlign", imageTop ? ContentAlignment.TopCenter : ContentAlignment.MiddleLeft);
            SetProperty(properties, "HeaderSubheaderSpacing", titleCard ? 4 : 2);
            SetProperty(properties, "Multiline", titleCard);
        }

        private void ToggleApplyThemeOnImage()
        {
            if (Component == null)
            {
                return;
            }

            var properties = TypeDescriptor.GetProperties(Component);
            var property = properties["ApplyThemeOnImage"];
            if (property == null)
            {
                return;
            }

            bool current = property.GetValue(Component) is bool value && value;
            SetProperty(properties, "ApplyThemeOnImage", !current);
        }

        private void SetProperty(PropertyDescriptorCollection properties, string propertyName, object value)
        {
            var property = properties[propertyName];
            if (property == null || property.IsReadOnly)
            {
                return;
            }

            var oldValue = property.GetValue(Component);
            if (Equals(oldValue, value))
            {
                return;
            }

            _changeService?.OnComponentChanging(Component, property);
            property.SetValue(Component, value);
            _changeService?.OnComponentChanged(Component, property, oldValue, value);
        }
    }
}
