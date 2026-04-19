using System;
using System.ComponentModel;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Design-time support for container panels, exposing verbs for image and layout helpers.
    /// </summary>
    internal sealed class BeepPanelDesigner : BaseBeepParentControlDesigner, IImagePathDesignerHost
    {
        private DesignerVerbCollection? _verbs;

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
        }

        protected override DesignerActionListCollection GetControlSpecificActionLists()
            => new DesignerActionListCollection
            {
                new ImagePathDesignerActionList(this),
                new ContainerControlActionList(this),
                new BeepPanelActionList(this)
            };

        public override DesignerVerbCollection Verbs
            => _verbs ??= new DesignerVerbCollection
            {
                new DesignerVerb("Select Background Image...", OnSelectBackgroundImage),
                new DesignerVerb("Clear Background Image", OnClearBackgroundImage),
                new DesignerVerb("Select Header Icon...", OnSelectHeaderIcon),
                new DesignerVerb("Clear Header Icon", OnClearHeaderIcon),
                new DesignerVerb("Preset: GroupBox Header", (s, e) => ApplyPanelPreset("groupbox")),
                new DesignerVerb("Preset: Card Header", (s, e) => ApplyPanelPreset("card")),
                new DesignerVerb("Preset: Flat Panel", (s, e) => ApplyPanelPreset("flat")),
                new DesignerVerb("Preset: Header Hidden", (s, e) => ApplyPanelPreset("hidden")),
                new DesignerVerb("Fix Header Placement", (s, e) => FixHeaderPlacement())
            };

        private void OnSelectBackgroundImage(object sender, EventArgs e)
            => SelectImage();

        private void OnClearBackgroundImage(object sender, EventArgs e)
            => ClearImage();

        private void OnSelectHeaderIcon(object sender, EventArgs e)
            => SelectHeaderIcon();

        private void OnClearHeaderIcon(object sender, EventArgs e)
            => ClearHeaderIcon();

        public void SelectImage()
        {
            if (Component == null)
            {
                return;
            }

            var property = TypeDescriptor.GetProperties(Component)["ImagePath"];
            var serviceProvider = Component.Site ?? (IServiceProvider)GetService(typeof(IServiceProvider));

            using var dialog = new BeepImagePickerDialog(null, embed: false, serviceProvider, Component.GetType().Assembly);
            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                var newValue = dialog.SelectedResourcePath ?? dialog.SelectedFilePath;
                if (!string.IsNullOrEmpty(newValue))
                {
                    SetImagePath(newValue);
                }
            }
        }

        public void ClearImage()
        {
            if (Component == null)
            {
                return;
            }

            SetImagePath(string.Empty);
        }

        internal void SelectHeaderIcon()
        {
            if (Component == null)
            {
                return;
            }

            var serviceProvider = Component.Site ?? (IServiceProvider)GetService(typeof(IServiceProvider));
            using var dialog = new BeepImagePickerDialog(null, embed: false, serviceProvider, Component.GetType().Assembly);
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            var newValue = dialog.SelectedResourcePath ?? dialog.SelectedFilePath;
            if (!string.IsNullOrEmpty(newValue))
            {
                SetHeaderIconPath(newValue);
            }
        }

        internal void ClearHeaderIcon()
        {
            if (Component == null)
            {
                return;
            }

            SetHeaderIconPath(string.Empty);
        }

        public string GetImagePath()
            => GetProperty<string>("ImagePath") ?? string.Empty;

        public void SetImagePath(string value)
            => SetProperty("ImagePath", value ?? string.Empty);

        public string GetHeaderIconPath() => GetProperty<string>("IconPath") ?? string.Empty;

        public void SetHeaderIconPath(string value)
        {
            string next = value ?? string.Empty;
            SetProperty("IconPath", next);
            if (!string.IsNullOrEmpty(next))
            {
                SetProperty("ShowTitleIcon", true);
            }
        }

        public void ApplyPanelPreset(string preset)
        {
            if (Component == null)
            {
                return;
            }

            switch (preset)
            {
                case "groupbox":
                    SetProperty("ShowTitle", true);
                    SetProperty("TitleStyle", PanelTitleStyle.GroupBox);
                    SetProperty("ShowTitleLine", false);
                    SetProperty("TitleAlignment", ContentAlignment.TopLeft);
                    SetProperty("TitleGap", 8);
                    break;
                case "card":
                    SetProperty("ShowTitle", true);
                    SetProperty("TitleStyle", PanelTitleStyle.TopHeader);
                    SetProperty("ShowTitleLine", true);
                    SetProperty("ShowTitleLineinFullWidth", true);
                    SetProperty("TitleAlignment", ContentAlignment.TopLeft);
                    SetProperty("ShowTitleIcon", true);
                    break;
                case "flat":
                    SetProperty("ShowTitle", true);
                    SetProperty("TitleStyle", PanelTitleStyle.Above);
                    SetProperty("ShowTitleLine", true);
                    SetProperty("ShowTitleLineinFullWidth", true);
                    SetProperty("ShowTitleIcon", true);
                    break;
                case "hidden":
                    SetProperty("ShowTitle", false);
                    break;
            }
        }

        public void FixHeaderPlacement()
        {
            if (Component == null)
            {
                return;
            }

            SetProperty("TitleGap", 8);
            SetProperty("TitleLineThickness", 2);
            SetProperty("TitleAlignment", ContentAlignment.TopLeft);
            SetProperty("ShowTitleLineinFullWidth", true);
        }
    }

    internal sealed class BeepPanelActionList : DesignerActionList
    {
        private readonly BeepPanelDesigner _designer;

        public BeepPanelActionList(BeepPanelDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        [Category("Header")]
        [Description("Show or hide the panel title area.")]
        public bool ShowTitle
        {
            get => _designer.GetProperty<bool>("ShowTitle");
            set => _designer.SetProperty("ShowTitle", value);
        }

        [Category("Header")]
        [Description("The panel title text.")]
        public string TitleText
        {
            get => _designer.GetProperty<string>("TitleText") ?? string.Empty;
            set => _designer.SetProperty("TitleText", value);
        }

        [Category("Header")]
        [Description("Header visual style.")]
        public PanelTitleStyle TitleStyle
        {
            get => _designer.GetProperty<PanelTitleStyle>("TitleStyle");
            set => _designer.SetProperty("TitleStyle", value);
        }

        [Category("Header")]
        [Description("Show a decorative line under the title.")]
        public bool ShowTitleLine
        {
            get => _designer.GetProperty<bool>("ShowTitleLine");
            set => _designer.SetProperty("ShowTitleLine", value);
        }

        [Category("Header")]
        [Description("Stretch the title line across the full width.")]
        public bool ShowTitleLineinFullWidth
        {
            get => _designer.GetProperty<bool>("ShowTitleLineinFullWidth");
            set => _designer.SetProperty("ShowTitleLineinFullWidth", value);
        }

        [Category("Header")]
        [Description("Alignment used for the panel title.")]
        public ContentAlignment TitleAlignment
        {
            get => _designer.GetProperty<ContentAlignment>("TitleAlignment");
            set => _designer.SetProperty("TitleAlignment", value);
        }

        [Category("Header")]
        [Description("Spacing between the title and body.")]
        public int TitleGap
        {
            get => _designer.GetProperty<int>("TitleGap");
            set => _designer.SetProperty("TitleGap", value);
        }

        [Category("Header")]
        [Description("Show an icon beside the title text.")]
        public bool ShowTitleIcon
        {
            get => _designer.GetProperty<bool>("ShowTitleIcon");
            set => _designer.SetProperty("ShowTitleIcon", value);
        }

        [Category("Header")]
        [Description("Resource or file path for the panel header icon.")]
        public string HeaderIconPath
        {
            get => _designer.GetHeaderIconPath();
            set => _designer.SetHeaderIconPath(value);
        }

        public void SelectHeaderIcon() => _designer.SelectHeaderIcon();
        public void ClearHeaderIcon() => _designer.ClearHeaderIcon();
        public void ApplyGroupBoxPreset() => _designer.ApplyPanelPreset("groupbox");
        public void ApplyCardPreset() => _designer.ApplyPanelPreset("card");
        public void ApplyFlatPreset() => _designer.ApplyPanelPreset("flat");
        public void HideHeader() => _designer.ApplyPanelPreset("hidden");
        public void FixHeaderPlacement() => _designer.FixHeaderPlacement();

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Header"));
            items.Add(new DesignerActionPropertyItem(nameof(ShowTitle), "Show Title", "Header"));
            items.Add(new DesignerActionPropertyItem(nameof(TitleText), "Title Text", "Header"));
            items.Add(new DesignerActionPropertyItem(nameof(TitleStyle), "Title Style", "Header"));
            items.Add(new DesignerActionPropertyItem(nameof(ShowTitleLine), "Show Title Line", "Header"));
            items.Add(new DesignerActionPropertyItem(nameof(ShowTitleLineinFullWidth), "Full Width Title Line", "Header"));
            items.Add(new DesignerActionPropertyItem(nameof(TitleAlignment), "Title Alignment", "Header"));
            items.Add(new DesignerActionPropertyItem(nameof(TitleGap), "Title Gap", "Header"));
            items.Add(new DesignerActionPropertyItem(nameof(ShowTitleIcon), "Show Title Icon", "Header"));
            items.Add(new DesignerActionPropertyItem(nameof(HeaderIconPath), "Header Icon Path", "Header"));
            items.Add(new DesignerActionMethodItem(this, nameof(SelectHeaderIcon), "Select Header Icon", "Header", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ClearHeaderIcon), "Clear Header Icon", "Header", true));

            items.Add(new DesignerActionHeaderItem("Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(ApplyGroupBoxPreset), "GroupBox Header", "Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ApplyCardPreset), "Card Header", "Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(ApplyFlatPreset), "Flat Panel", "Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(HideHeader), "Header Hidden", "Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(FixHeaderPlacement), "Fix Header Placement", "Presets", true));

            return items;
        }
    }
}
