
using System.Runtime.CompilerServices;
using TheTechIdea.Beep.Desktop.Common;
using TheTechIdea.Beep.Vis.Logic;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Helpers;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.UIEditor
{
    public partial class BeepImageSelectorDialog : Form, IImageSelector
    {
        private string _imagePath;
        private bool _isEmbedded;
        private bool _isLocal;
        private bool _isProject;
        private bool _isResource;
        private bool _isSVG;
        private bool _isImage;
        private bool _isBitmap;
        private bool _isJPG;
        private bool _isPNG;
        private bool _isBMP;
        private bool _isJPEG;
        private bool _isGIF;
        private bool _isTIFF;
        private bool _isICO;
        private bool _isEMF;
        private bool _isWMF;
        private bool _isinPreview = false;
        private string previewFilePath = string.Empty; // Store the file path of the selected image for later embedding

        private Dictionary<string, ImageConfiguration> _embeddedImages = new Dictionary<string, ImageConfiguration>();

       private Dictionary<string, ImageConfiguration> _selectedEmbeddedImages = new Dictionary<string, ImageConfiguration>();
        private bool _isImageFromlistSelected;

        public string CurrentResourceType { get; set; }
        public string SelectedImagePath { get; set; }
        public SimpleItem SelectedImage { get; set; }

        public BeepImageSelectorDialog()
        {
            InitializeComponent();
            setupEventsHandlers();
            CurrentResourceType = "Local";

            // Load all resource dictionaries
            //ProjectResourceEmbedder.LoadLocalImagesToDictionary(_embeddedImages);


            // Populate ImagelistBox and display the specified image
            PopulateImagelistBoxFromDictionary();
        }
      
        public void SetEmbeddedImages(Dictionary<string, ImageConfiguration> embeddedImages)
        {
            _embeddedImages = embeddedImages;
        }
        public void SetSelectedEmbeddedImages(Dictionary<string, ImageConfiguration> selectedEmbeddedImages)
        {
            _selectedEmbeddedImages = selectedEmbeddedImages;
        }
        public void SetPreviewFilePath(string previewFilePath)
        {
            this.previewFilePath = previewFilePath;
        }
        private void Init(string imagePath)
        {
            // Set default radio button and initialize ImagelistBox

            ImagePreviewer.PreviewImage(PreviewpictureBox, imagePath);

            
        }
        private void setupEventsHandlers()
        {
           
            ImportLocalResourcesbutton.Click += (s, e) => ImportImageandEmbeddIt();
           
            ImagelistBox.SelectedIndexChanged += (s, e) => PreviewResourceFromImageList();
            Viewbutton.Click += (s, e) => SelectImage();
            // Set up radio button event handlers
            
        }
        private void SelectImage()
        {
            if(ImagelistBox.SelectedItems.Count > 0)
            {
                SelectedImage = (SimpleItem)ImagelistBox.SelectedItem;
                ImagePreviewer.PreviewImage(PreviewpictureBox,SelectedImage.ImagePath);
                SelectedImagePath = SelectedImage.ImagePath;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select an image to continue", "No image selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void PreviewResourceFromImageList()
        {
            _isImageFromlistSelected = true;
            if (ImagelistBox.SelectedItem != null)
            {
                // get the path of the selected image
                string selectedImage = ImagelistBox.SelectedItem.ToString();

                _isinPreview = true;
                ImagePreviewer.PreviewImage(PreviewpictureBox, ImagelistBox.SelectedItem.ToString());
            }

        }
        private void ImportImageandEmbeddIt()
        {
          //  ProjectResourceEmbedder.CopyAndEmbedFileToProjectResources(_embeddedImages,previewFilePath, ProjectPathHelper.GetMyPath());
        }
        private void PopulateImagelistBoxFromDictionary()
        {
           // ImageListHelper.LoadEmbeddedImagesToDictionary(_embeddedImages, ProjectPathHelper.GetMyPath());
            ImagelistBox.Items.Clear();
            _isinPreview = false;
            ImagelistBox.DisplayMember = "Name";
            ImagelistBox.ValueMember = "GuidId";
            foreach (var item in _embeddedImages)
            {
                ImageConfiguration x = item.Value;
                ImagelistBox.Items.Add(x); // Add embedded resource names
            }
        }
        public string PreviewImage(string initialPath)
        {
            _isImageFromlistSelected = false;
            SelectedImagePath = initialPath;
            ImagePreviewer.PreviewImage(PreviewpictureBox, SelectedImagePath);
            return SelectedImagePath;

        }
    }
}
