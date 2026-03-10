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
                new DesignerVerb("Clear Background Image", OnClearBackgroundImage),
                new DesignerVerb("Select Header Icon...", OnSelectHeaderIcon),
                new DesignerVerb("Clear Header Icon", OnClearHeaderIcon),
                new DesignerVerb("Preset: GroupBox Header", (s, e) => ApplyPanelPreset("groupbox")),
                new DesignerVerb("Preset: Card Header", (s, e) => ApplyPanelPreset("card")),
                new DesignerVerb("Preset: Flat Panel", (s, e) => ApplyPanelPreset("flat")),
                new DesignerVerb("Preset: Header Hidden", (s, e) => ApplyPanelPreset("hidden")),
                new DesignerVerb("Fix Header Placement", (s, e) => FixHeaderPlacement())
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

        private void OnSelectHeaderIcon(object sender, EventArgs e)
            => SelectHeaderIcon();

        private void OnClearHeaderIcon(object sender, EventArgs e)
            => ClearHeaderIcon();

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

        private void SelectHeaderIcon()
        {
            if (Component == null)
            {
                return;
            }

            var serviceProvider = Component.Site ?? (IServiceProvider)GetService(typeof(IServiceProvider));
            using var dialog = new BeepImagePickerDialog(null, embed: false, serviceProvider, Component.GetType().Assembly);
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            var newValue = dialog.SelectedResourcePath ?? dialog.SelectedFilePath;
            if (!string.IsNullOrEmpty(newValue))
            {
                SetHeaderIconPath(newValue);
            }
        }

        private void ClearHeaderIcon()
        {
            if (Component == null)
            {
                return;
            }

            SetHeaderIconPath(string.Empty);
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

        private void SetHeaderIconPath(string value)
        {
            if (Component == null)
            {
                return;
            }

            var properties = TypeDescriptor.GetProperties(Component);
            var iconProperty = properties["IconPath"];
            if (iconProperty == null || iconProperty.IsReadOnly)
            {
                return;
            }

            var current = iconProperty.GetValue(Component) as string ?? string.Empty;
            var next = value ?? string.Empty;
            if (string.Equals(current, next, StringComparison.Ordinal))
            {
                return;
            }

            _changeService?.OnComponentChanging(Component, iconProperty);
            iconProperty.SetValue(Component, next);
            _changeService?.OnComponentChanged(Component, iconProperty, current, next);

            var showIconProperty = properties["ShowTitleIcon"];
            if (showIconProperty != null && !showIconProperty.IsReadOnly)
            {
                var currentShow = showIconProperty.GetValue(Component) is bool b && b;
                if (!currentShow && !string.IsNullOrEmpty(next))
                {
                    _changeService?.OnComponentChanging(Component, showIconProperty);
                    showIconProperty.SetValue(Component, true);
                    _changeService?.OnComponentChanged(Component, showIconProperty, false, true);
                }
            }
        }

        private void ApplyPanelPreset(string preset)
        {
            if (Component == null)
            {
                return;
            }

            var properties = TypeDescriptor.GetProperties(Component);
            switch (preset)
            {
                case "groupbox":
                    SetProperty(properties, "ShowTitle", true);
                    SetProperty(properties, "TitleStyle", PanelTitleStyle.GroupBox);
                    SetProperty(properties, "ShowTitleLine", false);
                    SetProperty(properties, "TitleAlignment", ContentAlignment.TopLeft);
                    SetProperty(properties, "TitleGap", 8);
                    break;
                case "card":
                    SetProperty(properties, "ShowTitle", true);
                    SetProperty(properties, "TitleStyle", PanelTitleStyle.TopHeader);
                    SetProperty(properties, "ShowTitleLine", true);
                    SetProperty(properties, "ShowTitleLineinFullWidth", true);
                    SetProperty(properties, "TitleAlignment", ContentAlignment.TopLeft);
                    SetProperty(properties, "ShowTitleIcon", true);
                    break;
                case "flat":
                    SetProperty(properties, "ShowTitle", true);
                    SetProperty(properties, "TitleStyle", PanelTitleStyle.Above);
                    SetProperty(properties, "ShowTitleLine", true);
                    SetProperty(properties, "ShowTitleLineinFullWidth", true);
                    SetProperty(properties, "ShowTitleIcon", true);
                    break;
                case "hidden":
                    SetProperty(properties, "ShowTitle", false);
                    break;
            }
        }

        private void FixHeaderPlacement()
        {
            if (Component == null)
            {
                return;
            }

            var properties = TypeDescriptor.GetProperties(Component);
            SetProperty(properties, "TitleGap", 8);
            SetProperty(properties, "TitleLineThickness", 2);
            SetProperty(properties, "TitleAlignment", ContentAlignment.TopLeft);
            SetProperty(properties, "ShowTitleLineinFullWidth", true);
        }

        private void SetProperty(PropertyDescriptorCollection properties, string propertyName, object value)
        {
            var property = properties[propertyName];
            if (property == null || property.IsReadOnly)
            {
                return;
            }

            var current = property.GetValue(Component);
            if (Equals(current, value))
            {
                return;
            }

            _changeService?.OnComponentChanging(Component, property);
            property.SetValue(Component, value);
            _changeService?.OnComponentChanged(Component, property, current, value);
        }
    }
}
