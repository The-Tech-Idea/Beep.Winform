﻿using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls.Converters;
using TheTechIdea.Beep.Winform.Controls.Design;

namespace TheTechIdea.Beep.Winform.Controls
{
    [ToolboxItem(true)]
    [DefaultProperty("TabPages")]
    [DisplayName("Beep Tabs WithoutHeader")]
    [Category("Beep Controls")]
   // [Designer(typeof(BeepTabsDesigner))] // Use ControlDesigner or a custom designer
    public class TabControlWithoutHeader : TabControl
    {
        // Constructor to initialize the control
        public TabControlWithoutHeader() : base()
        {
            AllowDrop = true; // Enable drag-and-drop by default

            // Set the size of the control to match the first TabPage
            //if (TabPages.Count > 0)
            //{
            //    Size = TabPages[0].Size;
            //}
        }

        // Event triggered when TabPages are added or removed
        public event EventHandler TabPagesChanged;

        // Struct for P/Invoke to hide the tab headers
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left; public int Top; public int Right; public int Bottom; }

        /// <summary>
        /// Overrides the default window procedure to hide tab headers.
        /// </summary>
        /// <param name="m">The Windows Message being processed.</param>
        protected override void WndProc(ref Message m)
        {
            const int TCM_ADJUSTRECT = 0x1328;

            if (m.Msg == TCM_ADJUSTRECT )
            {
                // Return zero rect to hide the tabs
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Called when a control (TabPage) is added to the TabControl.
        /// </summary>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

          
        }

        /// <summary>
        /// Called when a control (TabPage) is removed from the TabControl.
        /// </summary>
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            // Trigger the TabPagesChanged event
            TabPagesChanged?.Invoke(this, EventArgs.Empty);
        }



        public event EventHandler<TabPage> TabPageLoading;

        

    }
}
