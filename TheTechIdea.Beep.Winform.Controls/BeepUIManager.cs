using TheTechIdea.Beep.Vis.Modules;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls
{
    [DesignerCategory("Component")]
    public class BeepUIManager : Component
    {
        private bool _applyBeepFormStyle = false;
        private EnumBeepThemes _theme = EnumBeepThemes.DefaultTheme;
        private Form _form;
        private bool _showborder = true;
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowBorder
        {
            get { return _showborder; }
            set { _showborder = value; ApplyBorder(); }
        }

     

        private bool _showShadow = false;
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowShadow
        {
            get { return _showShadow; }
            set { _showShadow = value; ApplyShadow(); }
        }
        private bool _isrounded = false;
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ISRounded
        {
            get { return _isrounded; }
            set { _isrounded = value; ApplyRounded(); }
        }

      

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Enable or disable BeepForm styling for the form.")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
      

        bool ApplyBeepFormStyle
        {
            get => _applyBeepFormStyle;
            set
            {
                _applyBeepFormStyle = value;
                if (_applyBeepFormStyle)
                {
                    ApplyBeepFormTheme(); // Apply BeepForm styling if enabled
                }
                else
                {
                    RevertToDefaultFormStyle(); // Revert to default form style if disabled
                }
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Select the theme to apply to all Beep controls.")]
        [DefaultValue(EnumBeepThemes.DefaultTheme)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public EnumBeepThemes Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                ApplyThemeToAllBeepControls(_form); // Apply to all existing controls recursively
            }
        }
        private BeepFunctionsPanel beepFunctionsPanel1;
        [Browsable(true)]
        [Category("Appearance")]
        public BeepFunctionsPanel BeepFunctionsPanel
        { get => beepFunctionsPanel1; 
         set => beepFunctionsPanel1 = value; 
        }
        public BeepUIManager(IContainer container)
        {
            container.Add(this);
            BeepGlobalThemeManager.OnThemeChanged += theme => Theme = theme;
            // Set form load event to apply settings at runtime
            
        }

        public override ISite Site
        {
            get => base.Site;
            set
            {
                base.Site = value;
                if (value?.DesignMode == true)
                {
                    IDesignerHost host = value.GetService(typeof(IDesignerHost)) as IDesignerHost;
                    if (host != null)
                    {
                        _form = host.RootComponent as Form;
                        AttachControlAddedEvent(_form); // Attach to control added events

                       
                    }
                }
                else
                {
                    // Attach the ParentChanged event for runtime detection
                   // this.ParentChanged += BeepUIManager_ParentChanged;
                }
            }
        }

        private void BeepUIManager_ParentChanged(object sender, EventArgs e)
        {
            FindFormAtRuntime();
        }

        private void FindFormAtRuntime()
        {
            if (_form == null)
            {
                _form = FindParentForm();
                if (_form != null)
                {
                    _form.Load += Form_Load;
                }
            }
        }

        private Form FindParentForm()
        {
            if (this.Container is ISite site && site.Container != null)
            {
                // Traverse up the container hierarchy to find a form
                IContainer container = site.Container;
                while (container != null)
                {
                    Form form = container.Components.OfType<Form>().FirstOrDefault();
                    if (form != null)
                    {
                        return form;
                    }

                    if (container is Component component && component.Site != null)
                    {
                        container = component.Site.Container;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return null;
        }

        private void Form_Load(object sender, EventArgs e)
        {
           // Console.WriteLine("Form Load event 1");

          //      Console.WriteLine("Form Load event 2");
                ApplyBeepFormTheme(); // Apply BeepForm styling at runtime

         //   Console.WriteLine("Form Load event 3");
            ApplyThemeToAllBeepControls(_form); // Apply theme to all controls
        }

    

        // Attach to ControlAdded event for the form and all child containers
        private void AttachControlAddedEvent(Control container)
        {
            if (container != null)
            {
              //  Console.WriteLine("Attaching to control added event 1");
                container.ControlAdded += OnControlAdded;
               // container.HandleCreated += (s, e) => ApplyBeepFormTheme(); // Apply theme to controls on handle creation
               // _form.Load += Form_Load; // Apply theme to controls on form load
               // Console.WriteLine("Attaching to control added event 2");
                // Recursively attach to existing child containers
                foreach (Control child in container.Controls)
                {
                    if (child is ContainerControl)
                    {
                        try
                        {
                            AttachControlAddedEvent(child); // Attach event to child containers
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($" {child.Name} -  {ex.Message}");
                        }
                        
                    }
                }
            }
        }

        // Event handler for when a control is added to the form or a container
        private void OnControlAdded(object sender, ControlEventArgs e)
        {
            ApplyThemeToControlAndChildren(e.Control); // Apply theme to the new control and its children

            // If the control is a container, recursively attach the ControlAdded event
            if (e.Control is ContainerControl containerControl)
            {
                AttachControlAddedEvent(containerControl);
            }
        }

        // Recursively apply the theme to all controls on the form and child containers
        private void ApplyThemeToAllBeepControls(Control container)
        {
            if (container == null) return;
            Console.WriteLine(" apply1 ");
            container.BackColor = BeepThemesManager.GetTheme(_theme).BackgroundColor;
            Console.WriteLine(" apply2 ");
            // Apply theme to the container itself

            ApplyThemeToControl(container);

            // Recursively apply theme to all child controls
            foreach (Control child in container.Controls)
            {
                ApplyThemeToAllBeepControls(child);
            }
        }

        // Apply the theme to a single control and all its children recursively
        private void ApplyThemeToControlAndChildren(Control control)
        {
            ApplyThemeToControl(control); // Apply to the control itself

            // Recursively apply to child controls if the control is a container
            foreach (Control child in control.Controls)
            {
                ApplyThemeToControlAndChildren(child);
            }
        }

        // Apply the selected theme to a single control
        private void ApplyThemeToControl(Control control)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["Theme"];
            if (themeProperty != null && themeProperty.PropertyType == typeof(EnumBeepThemes))
            {
                themeProperty.SetValue(control, _theme);
            }
        }
         private void ApplyShadow()
        {
            if (_form != null)
            {
                ApplyThemeToControl(_form); // Apply to the control itself

                // Recursively apply to child controls if the control is a container
                foreach (Control child in _form.Controls)
                {
                    BeepGlobalThemeManager.ApplyShadowToControl(child, _showShadow);
                }
            }
            
        }
        private void ApplyRounded()
        {
            if (_form != null)
            {
                ApplyThemeToControl(_form); // Apply to the control itself

                // Recursively apply to child controls if the control is a container
                foreach (Control child in _form.Controls)
                {
                    BeepGlobalThemeManager.ApplyRoundedToControl(child, _isrounded);
                }
            }
           
        }
        private void ApplyBorder()
        {
            if (_form != null)
            {
                ApplyThemeToControl(_form); // Apply to the control itself

                // Recursively apply to child controls if the control is a container
                foreach (Control child in _form.Controls)
                {
                    BeepGlobalThemeManager.ApplyBorderToControl(child, _showborder);
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DetachControlAddedEvent(_form);
                BeepGlobalThemeManager.OnThemeChanged -= theme => Theme = theme;

                if (_form != null)
                {
                    _form.Load -= Form_Load;
                }
            }
            base.Dispose(disposing);
        }

        private void DetachControlAddedEvent(Control container)
        {
            if (container != null)
            {
                container.ControlAdded -= OnControlAdded;
                foreach (Control child in container.Controls)
                {
                    if (child is ContainerControl)
                    {
                        DetachControlAddedEvent(child);
                    }
                }
            }
        }

        // Apply BeepForm styling to the form
        private void ApplyBeepFormTheme()
        {
            if (_form != null)
            {
                BeepFormGenerator.ApplyBeepForm(_form,  BeepThemesManager.GetTheme(_theme)); // Apply BeepForm properties
            }
        }

        // Optional method to revert to default form styling
        private void RevertToDefaultFormStyle()
        {
            if (_form != null)
            {
                BeepFormGenerator.RemoveBeepForm(_form); // Remove BeepForm styling and reset form appearance
            }
        }
    }
}
