using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Base;

namespace TheTechIdea.Beep.Winform.Controls.Widgets.Helpers.Sample
{
    /// <summary>
    /// Sample implementations for BeepControlWidget with all control styles
    /// </summary>
    public static class BeepControlWidgetSamples
    {
        /// <summary>
        /// Creates a toggle switch control widget
        /// Uses ToggleSwitchPainter.cs
        /// </summary>
        public static BeepControlWidget CreateToggleSwitchWidget()
        {
            return new BeepControlWidget
            {
                Style = ControlWidgetStyle.ToggleSwitch,
                Title = "Enable Notifications",
                Value = true,
                Size = new Size(200, 60),
                AccentColor = Color.FromArgb(76, 175, 80)
            };
        }

        /// <summary>
        /// Creates a slider control widget
        /// Uses SliderPainter.cs
        /// </summary>
        public static BeepControlWidget CreateSliderWidget()
        {
            return new BeepControlWidget
            {
                Style = ControlWidgetStyle.Slider,
                Title = "Volume",
                Value = 75,
                MinValue = 0,
                MaxValue = 100,
                Size = new Size(250, 80),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Creates a dropdown filter control widget
        /// Uses DropdownFilterPainter.cs
        /// </summary>
        public static BeepControlWidget CreateDropdownWidget()
        {
            return new BeepControlWidget
            {
                Style = ControlWidgetStyle.DropdownFilter,
                Title = "Category Filter",
                Value = "All Categories",
                Options = new List<string> { "All Categories", "Electronics", "Clothing", "Books", "Home & Garden" },
                Size = new Size(200, 80),
                AccentColor = Color.FromArgb(156, 39, 176)
            };
        }

        /// <summary>
        /// Creates a date picker control widget
        /// Uses DatePickerPainter.cs
        /// </summary>
        public static BeepControlWidget CreateDatePickerWidget()
        {
            return new BeepControlWidget
            {
                Style = ControlWidgetStyle.DatePicker,
                Title = "Select Date",
                Value = DateTime.Now.ToString("MM/dd/yyyy"),
                Size = new Size(200, 80),
                AccentColor = Color.FromArgb(255, 193, 7)
            };
        }

        /// <summary>
        /// Creates a search box control widget
        /// Uses SearchBoxPainter.cs
        /// </summary>
        public static BeepControlWidget CreateSearchBoxWidget()
        {
            return new BeepControlWidget
            {
                Style = ControlWidgetStyle.SearchBox,
                Title = "Search",
                Value = "",
                Size = new Size(300, 80),
                AccentColor = Color.FromArgb(33, 150, 243)
            };
        }

        /// <summary>
        /// Gets all control widget samples
        /// </summary>
        public static BeepControlWidget[] GetAllSamples()
        {
            return new BeepControlWidget[]
            {
                CreateToggleSwitchWidget(),
                CreateSliderWidget(),
                CreateDropdownWidget(),
                CreateDatePickerWidget(),
                CreateSearchBoxWidget()
            };
        }
    }
}