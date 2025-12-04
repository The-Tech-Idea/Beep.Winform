using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Base;
using TheTechIdea.Beep.Winform.Controls.Toggle;
using TheTechIdea.Beep.Winform.Controls.Numerics;
using TheTechIdea.Beep.Winform.Controls.Ratings;
using TheTechIdea.Beep.Winform.Controls.ListBoxs;
using TheTechIdea.Beep.Winform.Controls.ComboBoxes;
using TheTechIdea.Beep.Winform.Controls.Chips;
using TheTechIdea.Beep.Winform.Controls.Charts;
using TheTechIdea.Beep.Winform.Controls.Calendar;
using TheTechIdea.Beep.Winform.Controls.BreadCrumbs;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    internal static class DesignRegistration
    {
        [ModuleInitializer]
        internal static void Register()
        {
            // Original designers
            RegisterControl(typeof(BeepButton), typeof(BeepButtonDesigner));
            RegisterControl(typeof(BeepPanel), typeof(BeepPanelDesigner));
            RegisterControl(typeof(BeepLabel), typeof(BeepLabelDesigner));
            RegisterControl(typeof(BeepImage), typeof(BeepImageDesigner));

            // ===== NEW DESIGNERS =====
            
            // Toggle Controls
            RegisterControl(typeof(BeepSwitch), typeof(BeepSwitchDesigner));
            RegisterControl(typeof(BeepToggle), typeof(BeepToggleDesigner));
            RegisterControl(typeof(BeepCheckBoxBool), typeof(BeepCheckBoxDesigner));
            RegisterControl(typeof(BeepCheckBoxChar), typeof(BeepCheckBoxDesigner));
            RegisterControl(typeof(BeepCheckBoxString), typeof(BeepCheckBoxDesigner));
            
            // Data Entry Controls
            RegisterControl(typeof(BeepNumericUpDown), typeof(BeepNumericUpDownDesigner));
            RegisterControl(typeof(BeepDatePicker), typeof(BeepDatePickerDesigner));
            RegisterControl(typeof(BeepTimePicker), typeof(BeepTimePickerDesigner));
            
            // Selection Controls
            RegisterControl(typeof(BeepListBox), typeof(BeepListBoxDesigner));
            RegisterControl(typeof(BeepComboBox), typeof(BeepComboBoxDesigner));
            RegisterControl(typeof(BeepMultiChipGroup), typeof(BeepMultiChipGroupDesigner));
            
            // Display Controls
            RegisterControl(typeof(BeepChart), typeof(BeepChartDesigner));
            RegisterControl(typeof(BeepCalendar), typeof(BeepCalendarDesigner));
            RegisterControl(typeof(BeepStarRating), typeof(BeepStarRatingDesigner));
            RegisterControl(typeof(BeepBreadcrump), typeof(BeepBreadcrumpDesigner));
            
            // Combined Controls
            RegisterControl(typeof(BeepExtendedButton), typeof(BeepExtendedButtonDesigner));

            // Data Block Controls
            RegisterControl(typeof(BeepDataBlock), typeof(BeepDataBlockDesigner));

            // Image path editors
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
