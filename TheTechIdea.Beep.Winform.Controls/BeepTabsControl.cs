using System;
using System.ComponentModel;
using System.Diagnostics;
using TheTechIdea.Beep.Desktop.Common;

namespace TheTechIdea.Beep.Winform.Controls
{
   
    public class BeepTabsControl : BeepControl
    {
        // The TabControlWithoutHeader is expected to be provided via designer serialization.
        [Category("Beep Controls")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabControlWithoutHeader TabControl { get; set; }

        // The header control is created in code.
        private BeepTabHeaderControl _headerControl;
        private int spacing=5;

        // A default header height
        private const int DefaultHeaderHeight = 30;

        public BeepTabsControl()
        {
            //Debug.WriteLine("BeepTabsControl: Constructor called.");

            // Create the header control with anchor on Top | Left | Right
            // so it stays at the top and resizes horizontally with the parent.
            _headerControl = new BeepTabHeaderControl
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Height = DefaultHeaderHeight,
                BackColor = Color.LightGray, // for visibility during design
                Left = 0,
                Top = 0
            };
            Padding = new Padding(5);
            // Add the header control to the Controls collection.
            this.Controls.Add(_headerControl);
            //Debug.WriteLine("BeepTabsControl: Header control created and added.");
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            //Debug.WriteLine("BeepTabsControl: OnCreateControl called.");

            // Instead of creating a fallback when TabControl is null,
            // assume the designer provides it.
            if (TabControl == null)
            {
                //Debug.WriteLine("BeepTabsControl: TabControl property is null. Check designer serialization!");
                // Optionally, you could throw an exception or log a warning here.
                // For debugging, you might create a fallback temporarily:
                // TabControl = new TabControlWithoutHeader { Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
                // this.Controls.Add(TabControl);
            }
            else
            {
                //Debug.WriteLine("BeepTabsControl: Using designer-provided TabControl.");
                TabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            }

            // Wire up the header control with the TabControl.
            _headerControl.TargetTabControl = TabControl;
            //Debug.WriteLine("BeepTabsControl: Header control wired to TabControl.");

            // Bring the header control to front to ensure visibility.
            _headerControl.BringToFront();
            //Debug.WriteLine("BeepTabsControl: Header control brought to front.");
        }


        /// <summary>
        /// Called when the control or its children need to layout. 
        /// We position the tab control right below the header.
        /// </summary>
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            // Position header at top-left, anchored horizontally
            _headerControl.Left = 0;
            _headerControl.Top = 0;
            _headerControl.Width = this.ClientSize.Width;  // anchor will keep it resizing

            // Position tab control below the header
            if (TabControl != null)
            {
                TabControl.Left = 0;
                TabControl.Top = _headerControl.Bottom+spacing;
                TabControl.Width = this.ClientSize.Width;
                // Height = total height - header height
                TabControl.Height = this.ClientSize.Height - _headerControl.Height;
            }
        }

        /// <summary>
        /// Exposes the header’s Tabs property for design-time editing.
        /// </summary>
        [Browsable(true)]
        [Localizable(true)]
        [MergableProperty(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingList<SimpleItem> Tabs
        {
            get
            {
                //Debug.WriteLine("BeepTabsControl: Tabs getter called.");
                return _headerControl.Tabs;
            }
            set
            {
                //Debug.WriteLine("BeepTabsControl: Tabs setter called.");
                _headerControl.Tabs = value;
            }
        }
        /// <summary>
        /// Exposes the SelectedTab property from the header so you can select a tab at design time or runtime.
        /// </summary>
        [Category("Beep Controls")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets or sets the currently selected tab.")]
        public TabPage SelectedTab
        {
            get
            {
                //Debug.WriteLine("BeepTabsControl: SelectedTab getter called.");
                return _headerControl.SelectedTab;
            }
            set
            {
                //Debug.WriteLine("BeepTabsControl: SelectedTab setter called.");
                _headerControl.SelectedTab = value;
            }
        }

        /// <summary>
        /// Hides the underlying TabPages property from the property grid.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new TabControl.TabPageCollection TabPages
        {
            get
            {
                //Debug.WriteLine("BeepTabsControl: TabPages getter called.");
                return TabControl?.TabPages;
            }
        }
    }
}
