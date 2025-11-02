using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal static class DesignRegistration
    {
        [ModuleInitializer]
        internal static void Register()
        {
            RegisterControl(typeof(BeepButton), typeof(BeepButtonDesigner));
            RegisterControl(typeof(BeepPanel), typeof(BeepPanelDesigner));
            RegisterControl(typeof(BeepLabel), typeof(BeepLabelDesigner));
            RegisterControl(typeof(BeepImage), typeof(BeepImageDesigner));

            AddImagePathEditor(typeof(BaseControl));
            AddImagePathEditor(typeof(BeepControl));
        }

        private static void RegisterControl(Type controlType, Type designerType)
        {
            TypeDescriptor.AddAttributes(controlType, new DesignerAttribute(designerType));
        }

        private static void AddImagePathEditor(Type controlType)
        {
            var baseProvider = TypeDescriptor.GetProvider(controlType);
            TypeDescriptor.AddProviderTransparent(new ImagePathEditorTypeDescriptionProvider(baseProvider), controlType);
        }
    }
}
