using TheTechIdea.Beep.Vis.Modules;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;


namespace TheTechIdea.Beep.Winform.Controls
{
    [DesignerCategory("Component")]
    public class BeepUIManager : Component
    {
        private bool _applyBeepFormStyle = false;
        private EnumBeepThemes _theme = EnumBeepThemes.DefaultTheme;
        private Form _form;
        private bool _showborder = true;

        public event Action<EnumBeepThemes> OnThemeChanged;
        //private EnumBeepThemes _globalTheme = EnumBeepThemes.DefaultTheme;
        // LogoImage property to set the logo image of the form
        private string _logoImage = "";
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set the logo image of the form.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Editor(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string LogoImage
        {
            get => _logoImage;
            set
            {
                _logoImage = value;
                if (BeepiForm != null)
                {
                    BeepiForm.LogoImage = _logoImage;
                }
                if(BeepSideMenu != null)
                {
                    BeepSideMenu.LogoImage = _logoImage;
                }
            }
            
        }

        // title property to set the title of the form
        private string _title = "Beep Form";
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set the title of the form.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                if (BeepiForm != null)
                {
                    BeepiForm.Text = _title;
                 
                }
                if(BeepSideMenu != null)
                {
                    BeepSideMenu.Title = _title;
                }
            }
        }
        bool _applyThemeOnImage = false;
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Set the title of the form.")]
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ApplyThemeOnImage
        {
            get => _applyThemeOnImage;
            set
            {
                _applyThemeOnImage = value;
                ApplyImage();

            }
        }
        private BeepiForm _beepiForm;
        [Browsable(true)]
        [Category("Appearance")]
        public BeepiForm BeepiForm
        {
            get => _beepiForm;
            set
            {
               
                if (value != null) {
                    _beepiForm = value;
                    _beepiForm.ShowTitle(false);
                    if (BeepSideMenu != null)
                    {
                        BeepSideMenu.BeepForm = BeepiForm;
                    }
                }
                
            }
        }
        private BeepSideMenu _beepSideMenu;
        [Browsable(true)]
        [Category("Appearance")]
        public BeepSideMenu BeepSideMenu
        {
            get => _beepSideMenu;
            set { _beepSideMenu = value; _beepSideMenu = value;
                if (BeepiForm != null)
                {
                    BeepSideMenu.BeepForm = BeepiForm;
                    _beepSideMenu.OnMenuCollapseExpand -= _beepSideMenu_OnMenuCollapseExpand;
                    _beepSideMenu.OnMenuCollapseExpand += _beepSideMenu_OnMenuCollapseExpand;
                }
               
                
            }
        }
        [Browsable(true)]
        [Category("Appearance")]
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowBorder
        {
            get { return _showborder; }
            set { _showborder = value; ApplyBorder(); }
        }
        private bool _showShadow = false;
        [Browsable(true)]
        [Category("Appearance")]
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowShadow
        {
            get { return _showShadow; }
            set { _showShadow = value; ApplyShadow(); }
        }
        private bool _isrounded = false;
        [Browsable(true)]
        [Category("Appearance")]
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsRounded
        {
            get { return _isrounded; }
            set { _isrounded = value; ApplyRounded(); }
        }
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Enable or disable BeepForm styling for the form.")]
        
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
                OnThemeChanged?.Invoke(_theme);
                ApplyThemeToAllBeepControls(_form); // Apply to all existing controls recursively
            }
        }
        private BeepFunctionsPanel beepFunctionsPanel1;
        [Browsable(true)]
        [Category("Appearance")]
        public BeepFunctionsPanel BeepFunctionsPanel
        {
            get => beepFunctionsPanel1;
            set => beepFunctionsPanel1 = value;
        }
        public BeepUIManager(IContainer container)
        {
            container.Add(this);
            // OnThemeChanged += theme => Theme = theme;
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

           // InitForm((BeepiForm)sender);
        }

        public void InitForm(BeepiForm form)
        {
            _form = form;
        //    ApplyBeepFormTheme(); // Apply BeepForm styling at runtime
            ApplyThemeToAllBeepControls(_form); // Apply theme to all controls
            FindBeepSideMenu();
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
                ApplyBorderToControl(containerControl, _showborder);
                ApplyShadowToControl(containerControl, _showShadow);
                ApplyRoundedToControl(containerControl, _isrounded);
            }
            if (e.Control is BeepSideMenu beepSideMenu)
            {
                _beepSideMenu = beepSideMenu;
                 BeepSideMenu.BeepForm = BeepiForm;
                BeepiForm.Title = Title;
                BeepSideMenu.Title = Title;
                _beepSideMenu.OnMenuCollapseExpand -= _beepSideMenu_OnMenuCollapseExpand;
                _beepSideMenu.OnMenuCollapseExpand += _beepSideMenu_OnMenuCollapseExpand;
            }
        }

        private void _beepSideMenu_OnMenuCollapseExpand(bool obj)
        {
            if (BeepiForm != null)
            {
                BeepiForm.Title= Title;
                BeepSideMenu.Title = Title;
                BeepiForm.ShowTitle(obj);
            }
        
        }

        // Recursively apply the theme to all controls on the form and child containers
        private void ApplyThemeToAllBeepControls(Control container)
        {
            if (container == null) return;
            container.BackColor = BeepThemesManager.GetTheme(_theme).BackgroundColor;
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
            ApplyThemeToControl(control, _theme, ApplyThemeOnImage);
        }
        private void ApplyShadow()
        {
            if (_form != null)
            {
                //     ApplyThemeToControl(_form); // Apply to the control itself

                // Recursively apply to child controls if the control is a container
                foreach (Control child in _form.Controls)
                {
                    ApplyShadowToControl(child, _showShadow);
                }
            }

        }
        private void ApplyRounded()
        {
            if (_form != null)
            {
                //     ApplyThemeToControl(_form); // Apply to the control itself

                // Recursively apply to child controls if the control is a container
                foreach (Control child in _form.Controls)
                {
                    ApplyRoundedToControl(child, _isrounded);
                }
            }

        }
        private void ApplyBorder()
        {
            if (_form != null)
            {
                //   ApplyThemeToControl(_form); // Apply to the control itself

                // Recursively apply to child controls if the control is a container
                foreach (Control child in _form.Controls)
                {
                    ApplyBorderToControl(child, _showborder);
                }
            }
        }
        private void ApplyImage()
        {
            if (_form != null)
            {
                // ApplyThemeToControl(_form); // Apply to the control itself

                // Recursively apply to child controls if the control is a container
                foreach (Control child in _form.Controls)
                {
                    ApplyThemeOnImageControl(child, _applyThemeOnImage);
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DetachControlAddedEvent(_form);
                OnThemeChanged -= theme => Theme = theme;

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
                BeepFormGenerator.ApplyBeepForm(_form, BeepThemesManager.GetTheme(_theme)); // Apply BeepForm properties
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

        public void ApplyThemeToControl(Control control, EnumBeepThemes _theme, bool applytoimage)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["Theme"];
            if (themeProperty != null && themeProperty.PropertyType == typeof(EnumBeepThemes))
            {
                themeProperty.SetValue(control, _theme);
            }
            BeepiForm.Title = Title;
            BeepSideMenu.Title = Title;
            ApplyShadowToControl(control, _showShadow);
            ApplyRoundedToControl(control, _isrounded);
            ApplyBorderToControl(control, _showborder);
            ApplyThemeOnImageControl(control, _applyThemeOnImage);
        }
        public void ApplyShadowToControl(Control control, bool showshadow)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["ShowShadow"];
            if (themeProperty != null)
            {
                themeProperty.SetValue(control, showshadow);
            }

        }
        public void ApplyRoundedToControl(Control control, bool isrounded)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["IsRounded"];
            if (themeProperty != null)
            {
                themeProperty.SetValue(control, isrounded);
            }

        }
        public void ApplyBorderToControl(Control control, bool showborder)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["ShowAllBorders"];
            if (themeProperty != null)
            {
                themeProperty.SetValue(control, showborder);
            }

        }
        public void ApplyThemeOnImageControl(Control control, bool _applyonimage)
        {
            var ImageProperty = TypeDescriptor.GetProperties(control)["LogoImage"];
            if (ImageProperty != null && ImageProperty.PropertyType == typeof(string))
            {
                var ApplyThemeOnImage = TypeDescriptor.GetProperties(control)["ApplyThemeOnImage"];
                if (ApplyThemeOnImage != null && ApplyThemeOnImage.PropertyType == typeof(bool))
                {
                    ApplyThemeOnImage.SetValue(control, _applyonimage);
                }
            }
        }
        public void FindBeepSideMenu()
        {
            if (_form != null)
            {
                foreach (Control control in _form.Controls)
                {
                    if (control is BeepSideMenu)
                    {

                        _beepSideMenu.Title = Title;
                        _beepSideMenu.BeepForm = BeepiForm;
                        _beepSideMenu.LogoImage = LogoImage;
                        _beepSideMenu = control as BeepSideMenu;
                        _beepSideMenu.BeepForm = BeepiForm;
                        _beepSideMenu.OnMenuCollapseExpand -= _beepSideMenu_OnMenuCollapseExpand;
                        _beepSideMenu.OnMenuCollapseExpand += _beepSideMenu_OnMenuCollapseExpand;
                    }
                    if(control is BeepFunctionsPanel)
                    {
                        beepFunctionsPanel1 = control as BeepFunctionsPanel;
                    }
                    if(control is BeepiForm)
                    {
                        BeepiForm = control as BeepiForm;
                        BeepiForm.LogoImage = LogoImage;
                        BeepiForm.Title = Title;
                    }
                }
            }
        }
    }
}
