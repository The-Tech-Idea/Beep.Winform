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
using TheTechIdea.Beep.Winform.Controls.TextFields;
using TheTechIdea.Beep.Winform.Controls.Cards;
using TheTechIdea.Beep.Winform.Controls.ProgressBars;
using TheTechIdea.Beep.Winform.Controls.Layouts;
using TheTechIdea.Beep.Winform.Controls.GridX;
using TheTechIdea.Beep.Winform.Controls.BottomNavBars;
using TheTechIdea.Beep.Winform.Controls.Docks;
using TheTechIdea.Beep.Winform.Controls.Menus;
using TheTechIdea.Beep.Winform.Controls.RadioGroup;
using TheTechIdea.Beep.Winform.Controls.AccordionMenus;
using TheTechIdea.Beep.Winform.Controls.RadioGroup.Renderers;
using TheTechIdea.Beep.Winform.Controls.CheckBoxes;
using TheTechIdea.Beep.Winform.Controls.Images;
using TheTechIdea.Beep.Winform.Controls.VerticalTables;
using TheTechIdea.Beep.Winform.Controls.Widgets;

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
            // Image Controls
            RegisterControl(typeof(BeepImage), typeof(BeepImageDesigner));

            // ===== NEW DESIGNERS =====
            
            // Toggle Controls
            RegisterControl(typeof(BeepSwitch), typeof(BeepSwitchDesigner));
            RegisterControl(typeof(BeepToggle), typeof(BeepToggleDesigner));
            // CheckBox Controls
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
            RegisterControl(typeof(BeepVerticalTable), typeof(BeepVerticalTableDesigner));
            RegisterControl(typeof(BeepRadioGroup), typeof(BeepRadioGroupDesigner));
            RegisterControl(typeof(BeepTabs), typeof(BeepTabsDesigner));
            
            // Combined Controls
            RegisterControl(typeof(BeepExtendedButton), typeof(BeepExtendedButtonDesigner));

            // Data Block Controls
            RegisterControl(typeof(BeepDataBlock), typeof(BeepDataBlockDesigner));
            RegisterControl(typeof(BeepDataConnection), typeof(BeepDataConnectionDesigner));

            // Text Input Controls
            RegisterControl(typeof(BeepTextBox), typeof(BeepTextBoxDesigner));

            // Container Controls
            RegisterControl(typeof(BeepCard), typeof(BeepCardDesigner));
            RegisterControl(typeof(BeepLayoutControl), typeof(BeepLayoutControlDesigner));

            // Progress Controls
            RegisterControl(typeof(BeepProgressBar), typeof(BeepProgressBarDesigner));

            // Grid Controls
            RegisterControl(typeof(BeepGridPro), typeof(BeepGridProDesigner));

            // Navigation Controls
            RegisterControl(typeof(BottomBar), typeof(BottomBarDesigner));
            RegisterControl(typeof(BeepDock), typeof(BeepDockDesigner));

            // Menu Controls
            RegisterControl(typeof(BeepMenuBar), typeof(BeepMenuBarDesigner));
            RegisterControl(typeof(BeepAccordionMenu), typeof(BeepAccordionMenuDesigner));

            // Display Controls
            RegisterControl(typeof(BeepMarquee), typeof(BeepMarqueeDesigner));

            // Selection Controls
            RegisterControl(typeof(BeepRadioGroup), typeof(BeepRadioGroupDesigner));

            // Table Controls
            RegisterControl(typeof(BeepVerticalTable), typeof(BeepVerticalTableDesigner));

            // Widget Controls
            RegisterControl(typeof(BeepMetricWidget), typeof(BeepMetricWidgetDesigner));
            RegisterControl(typeof(BeepChartWidget), typeof(BeepChartWidgetDesigner));
            RegisterControl(typeof(BeepListWidget), typeof(BeepListWidgetDesigner));
            RegisterControl(typeof(BeepDashboardWidget), typeof(BeepDashboardWidgetDesigner));
            RegisterControl(typeof(BeepControlWidget), typeof(BeepControlWidgetDesigner));
            RegisterControl(typeof(BeepNotificationWidget), typeof(BeepNotificationWidgetDesigner));
            RegisterControl(typeof(BeepNavigationWidget), typeof(BeepNavigationWidgetDesigner));
            RegisterControl(typeof(BeepMediaWidget), typeof(BeepMediaWidgetDesigner));
            RegisterControl(typeof(BeepMapWidget), typeof(BeepMapWidgetDesigner));
            RegisterControl(typeof(BeepFinanceWidget), typeof(BeepFinanceWidgetDesigner));
            RegisterControl(typeof(BeepFormWidget), typeof(BeepFormWidgetDesigner));
            RegisterControl(typeof(BeepCalendarWidget), typeof(BeepCalendarWidgetDesigner));
            RegisterControl(typeof(BeepSocialWidget), typeof(BeepSocialWidgetDesigner));

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
