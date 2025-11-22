using System;
using System.Drawing;
using TheTechIdea.Beep.Winform.Controls.DisplayContainers;

namespace TheTechIdea.Beep.Winform.Controls.Styling
{
    public static class TabStylePresets
    {
        /// <summary>
        /// Apply sensible defaults to a classic-style tabs control
        /// </summary>
        public static void ApplyPreset(TheTechIdea.Beep.Winform.Controls.BeepTabs tabs, TheTechIdea.Beep.Winform.Controls.TabStyle style)
        {
            if (tabs == null) return;

            switch (style)
            {
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Classic:
                    tabs.TabStyle = TheTechIdea.Beep.Winform.Controls.TabStyle.Classic;
                    tabs.ShowCloseButtons = true;
                    tabs.HeaderHeight = 36;
                    tabs.Padding = new Point(8, 6);
                    tabs.Theme = "Default";
                    break;
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Capsule:
                    tabs.TabStyle = TheTechIdea.Beep.Winform.Controls.TabStyle.Capsule;
                    tabs.ShowCloseButtons = false;
                    tabs.HeaderHeight = 36;
                    tabs.Padding = new Point(12, 8);
                    tabs.Theme = "Light";
                    break;
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Underline:
                    tabs.TabStyle = TheTechIdea.Beep.Winform.Controls.TabStyle.Underline;
                    tabs.ShowCloseButtons = false;
                    tabs.HeaderHeight = 28;
                    tabs.Padding = new Point(10, 5);
                    tabs.Theme = "Light";
                    break;
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Minimal:
                    tabs.TabStyle = TheTechIdea.Beep.Winform.Controls.TabStyle.Minimal;
                    tabs.ShowCloseButtons = false;
                    tabs.HeaderHeight = 26;
                    tabs.Padding = new Point(10, 4);
                    tabs.Theme = "Light";
                    break;
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Segmented:
                    tabs.TabStyle = TheTechIdea.Beep.Winform.Controls.TabStyle.Segmented;
                    tabs.ShowCloseButtons = false;
                    tabs.HeaderHeight = 40;
                    tabs.Padding = new Point(10, 8);
                    tabs.Theme = "Light";
                    break;
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Card:
                    tabs.TabStyle = TheTechIdea.Beep.Winform.Controls.TabStyle.Card;
                    tabs.ShowCloseButtons = true;
                    tabs.HeaderHeight = 32;
                    tabs.Padding = new Point(10, 6);
                    tabs.Theme = "Default";
                    break;
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Button:
                    tabs.TabStyle = TheTechIdea.Beep.Winform.Controls.TabStyle.Button;
                    tabs.ShowCloseButtons = false;
                    tabs.HeaderHeight = 36;
                    tabs.Padding = new Point(12, 8);
                    tabs.Theme = "Default";
                    break;
            }
            tabs.Invalidate();
        }

        /// <summary>
        /// Apply presets to BeepDisplayContainer (wraps internal BeepTabs)
        /// </summary>
        public static void ApplyPreset(BeepDisplayContainer container, TheTechIdea.Beep.Winform.Controls.TabStyle style)
        {
            if (container == null) return;
            container.TabStyle = style;
            if (container.ContainerType == TheTechIdea.Beep.Vis.Modules.ContainerTypeEnum.TabbedPanel && container.Controls != null)
            {
                if (container.Controls.Count > 0)
                {
                    foreach (Control c in container.Controls)
                    {
                        if (c is TheTechIdea.Beep.Winform.Controls.BeepTabs tabs)
                        {
                            ApplyPreset(tabs, style);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Apply presets for BeepDisplayContainer2 (native painting)
        /// </summary>
        public static void ApplyPreset(TheTechIdea.Beep.Winform.Controls.DisplayContainers.BeepDisplayContainer2 container, TheTechIdea.Beep.Winform.Controls.TabStyle style)
        {
            if (container == null) return;

            container.TabStyle = style;
            // Map visual styles to control-level properties
            switch (style)
            {
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Classic:
                    container.TabHeight = 36;
                    container.ShowCloseButtons = true;
                    container.AllowTabReordering = true;
                    break;
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Capsule:
                    container.TabHeight = 40;
                    container.ShowCloseButtons = false;
                    container.AllowTabReordering = false;
                    break;
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Underline:
                    container.TabHeight = 28;
                    container.ShowCloseButtons = false;
                    container.AllowTabReordering = false;
                    break;
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Minimal:
                    container.TabHeight = 26;
                    container.ShowCloseButtons = false;
                    container.AllowTabReordering = false;
                    break;
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Segmented:
                    container.TabHeight = 42;
                    container.ShowCloseButtons = false;
                    container.AllowTabReordering = true;
                    break;
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Card:
                    container.TabHeight = 32;
                    container.ShowCloseButtons = true;
                    container.AllowTabReordering = true;
                    break;
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Button:
                    container.TabHeight = 36;
                    container.ShowCloseButtons = false;
                    container.AllowTabReordering = true;
                    break;
            }

            // Set suggested ControlStyle for visual coherence
            switch (style)
            {
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Capsule:
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Segmented:
                    container.ControlStyle = BeepControlStyle.PillRail;
                    break;
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Minimal:
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Underline:
                    container.ControlStyle =BeepControlStyle.Minimal;
                    break;
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Card:
                    container.ControlStyle = BeepControlStyle.FigmaCard;
                    break;
                case TheTechIdea.Beep.Winform.Controls.TabStyle.Button:
                    container.ControlStyle = BeepControlStyle.FigmaCard;
                    break;
                default:
                    container.ControlStyle = BeepControlStyle.FigmaCard;
                    break;
            }

            // Theme: for capsule/segmented choose a light accent; for minimal/underline choose subtle theme
            if (style == TheTechIdea.Beep.Winform.Controls.TabStyle.Capsule || style == TheTechIdea.Beep.Winform.Controls.TabStyle.Segmented)
            {
                container.Theme = "Light";
            }
            else
            {
                container.Theme = "Default";
            }

            // Force repaint
            container.RecalculateLayout();
            container.Invalidate();
        }
    }
}
