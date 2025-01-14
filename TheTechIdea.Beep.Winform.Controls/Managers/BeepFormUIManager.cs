﻿using TheTechIdea.Beep.Vis.Modules;
using System.ComponentModel;
using System.ComponentModel.Design;
using TheTechIdea.Beep.Desktop.Common;


namespace TheTechIdea.Beep.Winform.Controls.Managers
{
    [DesignerCategory("Component")]
    [ToolboxItem(true)]
    [Category("Beep Controls")]
    [Description("A manager for BeepForm controls.")]
    [DisplayName("Beep Form UI Manager")]
    public class BeepFormUIManager : Component
    {
        #region "Properties"
        private bool _applyBeepFormStyle = false;
        private EnumBeepThemes _theme = EnumBeepThemes.DefaultTheme;
        private Form _form;
        private bool _showborder = true;
        private BeepImage beepimage = new BeepImage();
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
                if (value != null)
                {
                    beepimage.ImagePath = value;
                }
                if (BeepAppBar != null)
                {
                    BeepAppBar.LogoImage = _logoImage;
                }
                if (BeepSideMenu != null)
                {
                    BeepSideMenu.LogoImage = _logoImage;
                }
                if (BeepiForm != null)
                {
                    if (beepimage != null)
                    {
                        if (beepimage.svgDocument != null)
                        {
                            BeepiForm.Icon = ImageConverters.ConvertSvgToIcon(beepimage.svgDocument, 64);
                        }
                    }

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
                if (BeepSideMenu != null)
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
        private BeepAppBar _beepappbar;
        [Browsable(true)]
        [Category("Appearance")]
        public BeepAppBar BeepAppBar
        {
            get => _beepappbar;
            set
            {

                if (value != null)
                {
                    _beepappbar = value;

                    if (BeepSideMenu != null)
                    {
                        BeepSideMenu.BeepForm = BeepiForm;
                        BeepSideMenu.BeepAppBar = _beepappbar;
                    }
                }

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

                if (value != null)
                {
                    _beepiForm = value;

                    if (_beepSideMenu != null)
                    {
                        _beepSideMenu.BeepForm = BeepiForm;

                    }
                    if (beepimage != null)
                    {
                        if (beepimage.svgDocument != null)
                        {
                            _beepiForm.Icon = ImageConverters.ConvertSvgToIcon(beepimage.svgDocument, 64);
                        }
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
            set
            {
                _beepSideMenu = value;
                if (BeepiForm != null)
                {
                    _beepSideMenu.BeepForm = BeepiForm;

                }
                if (_beepSideMenu != null)
                {
                    _beepSideMenu.Title = Title;
                    _beepSideMenu.LogoImage = LogoImage;
                    _beepSideMenu.OnMenuCollapseExpand -= _beepSideMenu_OnMenuCollapseExpand;
                    _beepSideMenu.OnMenuCollapseExpand += _beepSideMenu_OnMenuCollapseExpand;
                }


            }
        }
        private BeepMenuBar _beepMenuBar;
        [Browsable(true)]
        [Category("Appearance")]
        public BeepMenuBar BeepMenuBar
        {
            get => _beepMenuBar;
            set
            {
                _beepMenuBar = value;
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
        #endregion "Properties"
        #region "Constructors"
        public BeepFormUIManager(IContainer container)
        {
            container.Add(this);
            // OnThemeChanged += theme => Theme = theme;
            // Set form load event to apply settings at runtime

        }
        #endregion
        #region "Design-time support"
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
        public bool GetPropertyFromControl(Control control, string PropertyName)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)[PropertyName];
            if (themeProperty != null)
            {
                return (bool)themeProperty.GetValue(control);
            }
            else
            {
                return false;
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
                    if (control is BeepFunctionsPanel)
                    {
                        beepFunctionsPanel1 = control as BeepFunctionsPanel;
                    }
                    if (control is BeepiForm)
                    {
                        BeepiForm = control as BeepiForm;
                        BeepAppBar.LogoImage = LogoImage;
                        BeepAppBar.Title = Title;
                    }
                }
            }
        }
        public void ShowTitle(bool show)
        {
            if (BeepAppBar != null)
            {
                BeepAppBar.ShowTitle = show;
            }
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
            if (Container is ISite site && site.Container != null)
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
                BeepAppBar.Title = Title;
                BeepSideMenu.Title = Title;
                _beepSideMenu.OnMenuCollapseExpand -= _beepSideMenu_OnMenuCollapseExpand;
                _beepSideMenu.OnMenuCollapseExpand += _beepSideMenu_OnMenuCollapseExpand;
            }
            if (e.Control is BeepAppBar beepappbar)
            {
                _beepappbar = beepappbar;
                BeepAppBar.Title = Title;
            }
            if (e.Control is BeepFunctionsPanel beepFunctionsPanel)
            {
                beepFunctionsPanel1 = beepFunctionsPanel;
            }
            if (e.Control is BeepiForm beepiForm)
            {
                _beepiForm = beepiForm;
                BeepSideMenu.BeepForm = BeepiForm;
                BeepAppBar.Title = Title;
                BeepSideMenu.Title = Title;
            }
        }
        private void _beepSideMenu_OnMenuCollapseExpand(bool obj)
        {
            if (BeepAppBar != null)
            {
                BeepAppBar.Title = Title;
                BeepSideMenu.Title = Title;
                BeepAppBar.ShowTitle = obj;
                BeepAppBar.ShowLogoIcon = false;
            }

        }
        #endregion "Design-time support"
        #region "Theme Management"
        // Optional method to revert to default form styling
        private void RevertToDefaultFormStyle()
        {
            if (_form != null)
            {
                BeepFormGenerator.RemoveBeepForm(_form); // Remove BeepForm styling and reset form appearance
            }
        }
        public void ApplyRoundedToControl(Control control, bool isrounded)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["IsRounded"];
            if (themeProperty != null)
            {
                themeProperty.SetValue(control, isrounded);
            }
            if (control.Controls.Count > 0 && !HasThemeProperty(control))
            {
                foreach (Control child in control.Controls)
                {
                    ApplyRoundedToControl(child, isrounded);
                }
            }

        }
        public void ApplyBorderToControl(Control control, bool showborder)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["ShowAllBorders"];
            if (themeProperty != null)
            {
                themeProperty.SetValue(control, showborder);
                BeepControl beepControl = (BeepControl)control;
                beepControl.Invalidate();
            }
            if(control.Controls.Count > 0 && !HasThemeProperty(control))
            {
                foreach (Control child in control.Controls)
                {
                    ApplyBorderToControl(child, showborder);
                }
            }

        }
        public void ApplyShadowToControl(Control control, bool showshadow)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["ShowShadow"];
            if (themeProperty != null)
            {
                themeProperty.SetValue(control, showshadow);
            }
            if (control.Controls.Count > 0 && !HasThemeProperty(control))
            {
                foreach (Control child in control.Controls)
                {
                    ApplyBorderToControl(child, showshadow);
                }
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
        // Apply BeepForm styling to the form
        private void ApplyBeepFormTheme()
        {
            if (_form != null)
            {
                BeepFormGenerator.ApplyBeepForm(_form, BeepThemesManager.GetTheme(_theme)); // Apply BeepForm properties
            }
        }

        public void ApplyThemeToControl(Control control, EnumBeepThemes _theme, bool applytoimage)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["Theme"];
            if (themeProperty != null && themeProperty.PropertyType == typeof(EnumBeepThemes))
            {
                themeProperty.SetValue(control, _theme);
            }
            if (BeepiForm != null)
            {
                BeepAppBar.LogoImage = LogoImage;
                BeepAppBar.Title = Title;
            }
            if (BeepSideMenu != null)
            {
                BeepSideMenu.LogoImage = LogoImage;
                BeepSideMenu.Title = Title;
            }

            if (GetPropertyFromControl(control, "IsBorderAffectedByTheme"))
            {
                ApplyThemeOnImageControl(control, _showborder);
            }
            //IsShadowAffectedByTheme
            if (GetPropertyFromControl(control, "IsShadowAffectedByTheme"))
            {
                ApplyShadowToControl(control, _showShadow);
            }
            if (GetPropertyFromControl(control, "IsRoundedAffectedByTheme"))
            {
                ApplyRoundedToControl(control, _isrounded);
            }


            ApplyThemeOnImageControl(control, _applyThemeOnImage);
        }
     
        // Recursively apply the theme to all controls on the form and child containers
        private void ApplyThemeToAllBeepControls(Control container)
        {
            if (container == null) return;
            container.BackColor = BeepThemesManager.GetTheme(_theme).BackgroundColor;
            // Apply theme to the container itself
            ApplyThemeToControl(container);
            if (HasApplyThemeToChildsProperty(container))
            {
                if (GetPropertyFromControl(container, "ApplyThemeToChilds"))
                {
                    // Recursively apply theme to all child controls
                    foreach (Control child in container.Controls)
                    {

                        ApplyThemeToAllBeepControls(child);
                    }
                }
            }
            else
            {
                // Recursively apply theme to all child controls
                foreach (Control child in container.Controls)
                {
                    ApplyThemeToAllBeepControls(child);
                }

            }
        }
        private bool HasApplyThemeToChildsProperty(Control control)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["ApplyThemeToChilds"];
            if (themeProperty != null && themeProperty.PropertyType == typeof(bool))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool HasThemeProperty(Control control)
        {
            var themeProperty = TypeDescriptor.GetProperties(control)["Theme"];
            if (themeProperty != null && themeProperty.PropertyType == typeof(EnumBeepThemes))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // Apply the t
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
        #endregion "Theme Management"
        #region "Dispose"
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
        #endregion "Dispose"


    }
}
