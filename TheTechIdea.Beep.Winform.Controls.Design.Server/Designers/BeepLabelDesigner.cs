using System;
using System.ComponentModel;
using System.Windows.Forms;
using TheTechIdea.Beep.Winform.Controls;
using TheTechIdea.Beep.Winform.Controls.Design.Server.ActionLists;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server.Designers
{
    /// <summary>
    /// Designer verbs for BeepLabel to streamline image selection.
    /// </summary>
    internal sealed class BeepLabelDesigner : BaseBeepControlDesigner, IImagePathDesignerHost
    {
        private DesignerVerbCollection? _verbs;

        protected override DesignerActionListCollection GetControlSpecificActionLists()
            => new DesignerActionListCollection
            {
                new ImagePathDesignerActionList(this),
                new BeepLabelActionList(this)
            };

        public override DesignerVerbCollection Verbs
            => _verbs ??= new DesignerVerbCollection
            {
                new DesignerVerb("Select Image...", OnSelectImage),
                new DesignerVerb("Clear Image", OnClearImage),
                new DesignerVerb("Preset: Text Only", (s, e) => ApplyTextOnlyPreset()),
                new DesignerVerb("Preset: Image Left", (s, e) => ApplyImageLeftPreset()),
                new DesignerVerb("Preset: Image Top", (s, e) => ApplyImageTopPreset()),
                new DesignerVerb("Preset: Title + Subheader", (s, e) => ApplyTitleCardPreset()),
                new DesignerVerb("Toggle Theme Image Tint", (s, e) => ToggleApplyThemeOnImage())
            };

        private void OnSelectImage(object sender, EventArgs e)
            => SelectImage();

        private void OnClearImage(object sender, EventArgs e)
            => ClearImage();

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

        public string GetImagePath()
            => GetProperty<string>("ImagePath") ?? string.Empty;

        public void SetImagePath(string value)
            => SetProperty("ImagePath", value ?? string.Empty);

        public void ApplyTextOnlyPreset()
            => ApplyPreset(textOnly: true, imageTop: false, titleCard: false);

        public void ApplyImageLeftPreset()
            => ApplyPreset(textOnly: false, imageTop: false, titleCard: false);

        public void ApplyImageTopPreset()
            => ApplyPreset(textOnly: false, imageTop: true, titleCard: false);

        public void ApplyTitleCardPreset()
            => ApplyPreset(textOnly: true, imageTop: false, titleCard: true);

        private void ApplyPreset(bool textOnly, bool imageTop, bool titleCard)
        {
            if (Component == null)
            {
                return;
            }

            SetProperty("HideText", false);
            SetProperty("TextImageRelation", textOnly
                ? TextImageRelation.Overlay
                : (imageTop ? TextImageRelation.ImageAboveText : TextImageRelation.ImageBeforeText));
            SetProperty("TextAlign", titleCard ? ContentAlignment.TopLeft : ContentAlignment.MiddleLeft);
            SetProperty("ImageAlign", imageTop ? ContentAlignment.TopCenter : ContentAlignment.MiddleLeft);
            SetProperty("HeaderSubheaderSpacing", titleCard ? 4 : 2);
            SetProperty("Multiline", titleCard);
        }

        public void ToggleApplyThemeOnImage()
        {
            if (Component == null)
            {
                return;
            }

            bool current = GetProperty<bool>("ApplyThemeOnImage");
            SetProperty("ApplyThemeOnImage", !current);
        }
    }

    internal sealed class BeepLabelActionList : DesignerActionList
    {
        private readonly BeepLabelDesigner _designer;

        public BeepLabelActionList(BeepLabelDesigner designer)
            : base(designer.Component)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        [Category("Content")]
        [Description("Hide the label text while keeping image or subheader content.")]
        public bool HideText
        {
            get => _designer.GetProperty<bool>("HideText");
            set => _designer.SetProperty("HideText", value);
        }

        [Category("Content")]
        [Description("Relationship between image and text.")]
        public TextImageRelation TextImageRelation
        {
            get => _designer.GetProperty<TextImageRelation>("TextImageRelation");
            set => _designer.SetProperty("TextImageRelation", value);
        }

        [Category("Content")]
        [Description("Text alignment within the label surface.")]
        public ContentAlignment TextAlign
        {
            get => _designer.GetProperty<ContentAlignment>("TextAlign");
            set => _designer.SetProperty("TextAlign", value);
        }

        [Category("Content")]
        [Description("Image alignment within the label surface.")]
        public ContentAlignment ImageAlign
        {
            get => _designer.GetProperty<ContentAlignment>("ImageAlign");
            set => _designer.SetProperty("ImageAlign", value);
        }

        [Category("Content")]
        [Description("Spacing between header and subheader text blocks.")]
        public int HeaderSubheaderSpacing
        {
            get => _designer.GetProperty<int>("HeaderSubheaderSpacing");
            set => _designer.SetProperty("HeaderSubheaderSpacing", value);
        }

        [Category("Content")]
        [Description("Allow the label to render as multiple lines.")]
        public bool Multiline
        {
            get => _designer.GetProperty<bool>("Multiline");
            set => _designer.SetProperty("Multiline", value);
        }

        [Category("Image")]
        [Description("Tint the selected image with the active theme colors.")]
        public bool ApplyThemeOnImage
        {
            get => _designer.GetProperty<bool>("ApplyThemeOnImage");
            set => _designer.SetProperty("ApplyThemeOnImage", value);
        }

        public void UseTextOnlyPreset() => _designer.ApplyTextOnlyPreset();
        public void UseImageLeftPreset() => _designer.ApplyImageLeftPreset();
        public void UseImageTopPreset() => _designer.ApplyImageTopPreset();
        public void UseTitleCardPreset() => _designer.ApplyTitleCardPreset();

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            var items = new DesignerActionItemCollection();

            items.Add(new DesignerActionHeaderItem("Label Presets"));
            items.Add(new DesignerActionMethodItem(this, nameof(UseTextOnlyPreset), "Text Only", "Label Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseImageLeftPreset), "Image Left", "Label Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseImageTopPreset), "Image Top", "Label Presets", true));
            items.Add(new DesignerActionMethodItem(this, nameof(UseTitleCardPreset), "Title + Subheader", "Label Presets", true));

            items.Add(new DesignerActionHeaderItem("Content"));
            items.Add(new DesignerActionPropertyItem(nameof(HideText), "Hide Text", "Content"));
            items.Add(new DesignerActionPropertyItem(nameof(TextImageRelation), "Text / Image Relation", "Content"));
            items.Add(new DesignerActionPropertyItem(nameof(TextAlign), "Text Align", "Content"));
            items.Add(new DesignerActionPropertyItem(nameof(ImageAlign), "Image Align", "Content"));
            items.Add(new DesignerActionPropertyItem(nameof(HeaderSubheaderSpacing), "Header/Subheader Spacing", "Content"));
            items.Add(new DesignerActionPropertyItem(nameof(Multiline), "Multiline", "Content"));

            items.Add(new DesignerActionHeaderItem("Image"));
            items.Add(new DesignerActionPropertyItem(nameof(ApplyThemeOnImage), "Apply Theme On Image", "Image"));

            return items;
        }
    }
}
