using System;
using System.Collections;
using System.Data;
using System.Resources;
using System.ComponentModel;
using Svg;
using System.Reflection;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using TheTechIdea.Beep.Winform.Controls.Models;


namespace TheTechIdea.Beep.Winform.Controls.Design.UIEditor
{

    // Form use to  return the selected image Path either from local or project resources
    // Or just Import Images wether is local or project resources and supply the path
    // can import image types bmp,jpg,jpeg,png,svg
    public partial class ImageSelectorImporterDialog : Form, IImageSelector
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
        private bool _isinPreview=false;
        private string previewFilePath = string.Empty; // Store the file path of the selected image for later embedding

        private Dictionary<string, string> _imageResources = new Dictionary<string, string>();
        private Dictionary<string, string> _localImages = new Dictionary<string, string>();
        private Dictionary<string, string> _projectImages = new Dictionary<string, string>();
        private Dictionary<string, string> _embeddedImages = new Dictionary<string, string>();


        private Dictionary<string, string> _selectedLocalImages = new Dictionary<string, string>();
        private Dictionary<string, string> _selectedProjectImages = new Dictionary<string, string>();
        private Dictionary<string, string> _selectedEmbeddedImages = new Dictionary<string, string>();
        private Dictionary<string, string> _selectedImagesResources = new Dictionary<string, string>();

        
        public string CurrentResourceType { get; set; }
        public string SelectedImagePath { get;  set; }
        public ImageSelectorImporterDialog(string imagePath)
        {
            InitializeComponent();
            _isinPreview = true;
            Init(imagePath);
            ShowDialog();
        }
        
        public ImageSelectorImporterDialog()
        {
            InitializeComponent();

            // Load all resource dictionaries
            LoadLocalImagesToDictionary();
            LoadProjectImagesToDictionary();
            LoadEmbeddedResourcesToDictionary();

            // Set default radio button and initialize ImagelistBox
            LocalResoucesradioButton.Checked = true;
            CurrentResourceType = "Local";
            PopulateImagelistBoxFromDictionary();

            // Set up event handlers
            setupEventsHandlers();
        }

        private void Init(string imagePath)
        {
            // Set default radio button and initialize ImagelistBox
            LocalResoucesradioButton.Checked = true;
            CurrentResourceType = "Local";

            // Load all resource dictionaries
            LoadLocalImagesToDictionary();
            LoadProjectImagesToDictionary();
            LoadEmbeddedResourcesToDictionary();

            // Populate ImagelistBox and display the specified image
            PopulateImagelistBoxFromDictionary();
            PreviewImage(imagePath);

            // Set up event handlers
            setupEventsHandlers();
        }

        private void setupEventsHandlers()
        {
            LocalResoucesradioButton.CheckedChanged += (s, e) => RefreshResources();
            ProjectResoucesradioButton.CheckedChanged += (s, e) => RefreshResources();
            ImportLocalResourcesbutton.Click += (s, e) => PreviewImage();
            Embeddbutton.Click += (s, e) => CopyFileToProjectResources();
            ProjectResourceImportbutton.Click += (s, e) => EmbedImageInResources();
            ImagelistBox.SelectedIndexChanged += (s, e) => PreviewResource();
            SelectImagebutton.Click += (s, e) => SelectImage();
            // Set up radio button event handlers
            LocalResoucesradioButton.CheckedChanged += RadioButton_CheckedChanged;
            ProjectResoucesradioButton.CheckedChanged += RadioButton_CheckedChanged;
            EmbeddedradioButton.CheckedChanged += RadioButton_CheckedChanged;
        }
        #region "Resource Type Handling"
        // Populate ImagelistBox with local resources
        private void PopulateLocalResources()
        {
            // Add local files (e.g., from a directory)
            string localImagePath = Path.Combine(GetProjectPath(), "LocalImages");
            if (Directory.Exists(localImagePath))
            {
                var images = Directory.GetFiles(localImagePath, "*.*").Where(file =>
                    file.EndsWith(".bmp") || file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".png") || file.EndsWith(".svg"));
                ImagelistBox.Items.AddRange(images.Select(Path.GetFileName).ToArray());
            }
        }

        // Populate ImagelistBox with resources from a .resx file
        private void PopulateProjectResxResources()
        {
            var resourceSet = Properties.Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                if (entry.Value is Bitmap || entry.Value is string svgPath && svgPath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                {
                    ImagelistBox.Items.Add(entry.Key.ToString());
                }
            }
        }

        // Populate ImagelistBox with embedded resources
        private void PopulateEmbeddedResources()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();

            foreach (string resourceName in resourceNames)
            {
                if (resourceName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    resourceName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    resourceName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    resourceName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                    resourceName.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                {
                    ImagelistBox.Items.Add(resourceName);
                }
            }
        }
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Determine which radio button is selected and set CurrentResourceType accordingly
            if (LocalResoucesradioButton.Checked)
            {
                SetResourceTypeAndPopulateList("Local");
            }
            else if (ProjectResoucesradioButton.Checked)
            {
                SetResourceTypeAndPopulateList("ProjectResx");
            }
            else if (EmbeddedradioButton.Checked)
            {
                SetResourceTypeAndPopulateList("Embedded");
            }
        }
        private void SetResourceTypeAndPopulateList(string resourceType)
        {
            // Set CurrentResourceType based on selected radio button
            CurrentResourceType = resourceType;

            // Clear and repopulate ImagelistBox based on CurrentResourceType
            ImagelistBox.Items.Clear();
            switch (CurrentResourceType)
            {
                case "Local":
                    PopulateLocalResources();
                    break;
                case "ProjectResx":
                    PopulateProjectResxResources();
                    break;
                case "Embedded":
                    PopulateEmbeddedResources();
                    break;
            }
        }
        #endregion "Resource Type Handling"
        private void PreviewResource()
        {
            _isinPreview = false;
            if (ImagelistBox.SelectedItem == null)
            {
                MessageBox.Show("No resource selected for preview.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            switch (CurrentResourceType)
            {
                case "Local":
                    PreviewLocalResource();
                    break;

                case "ProjectResx":
                    PreviewProjectResxResource();
                    break;

                case "Embedded":
                    PreviewEmbeddedResource();
                    break;

                default:
                    MessageBox.Show("Unknown resource type selected.", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }
        public string PreviewImage(string initialPath)
        {
            _isinPreview = true;
            previewFilePath= initialPath; 
            SelectedImagePath = initialPath;
            ImagePath = initialPath;
            return SelectedImagePath;

        }
       
        private string SelectImage()
        {
            try
            {
                if (LocalResoucesradioButton.Checked && ImagelistBox.SelectedItem != null)
                {
                    SelectedImagePath = ImagelistBox.SelectedItem.ToString();
                }
                else if (ProjectResoucesradioButton.Checked && ImagelistBox.SelectedItem != null)
                {
                    if (ResourcesPathcomboBox.SelectedItem == null)
                    {
                        throw new Exception("No project resource path selected.");
                    }

                    // Construct the path based on selected project resource
                    SelectedImagePath = Path.Combine(
                        GetProjectPath(),
                        ResourcesPathcomboBox.SelectedItem.ToString()
                    );
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting image: {ex.Message}", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return SelectedImagePath;
        }
        private void RemoveResource(string resourceName)
        {
            string resxFile = Path.Combine(GetProjectPath(), ResourcesPathcomboBox.SelectedItem.ToString());
            string tempResxFile = Path.Combine(GetProjectPath(), "temp.resx");

            using (ResXResourceReader reader = new ResXResourceReader(resxFile))
            using (ResXResourceWriter writer = new ResXResourceWriter(tempResxFile))
            {
                foreach (DictionaryEntry entry in reader)
                {
                    if ((string)entry.Key != resourceName) // Exclude the resource to be removed
                    {
                        writer.AddResource(entry.Key.ToString(), entry.Value);
                    }
                }
            }

            // Replace original resx file with updated one
            File.Delete(resxFile);
            File.Move(tempResxFile, resxFile);

            RefreshResources(); // Refresh the resources list
        }
        private void RefreshResources()
        {
            // Clear the ImagelistBox
            ImagelistBox.Items.Clear();

            // Check which radio button is selected and set the CurrentResourceType
            if (LocalResoucesradioButton.Checked)
            {
                CurrentResourceType = "Local";
                LoadLocalImagesToDictionary(); // Load images into the dictionary
            }
            else if (ProjectResoucesradioButton.Checked)
            {
                CurrentResourceType = "ProjectResx";
                LoadResourceFilesToDictionary(); // Load images from .resx files into the dictionary
            }
            else if (EmbeddedradioButton.Checked)
            {
                CurrentResourceType = "Embedded";
                LoadEmbeddedResourcesToDictionary(); // Load embedded images into the dictionary
            }

            // Populate the ImagelistBox using the current dictionary
            PopulateImagelistBoxFromDictionary();
        }
        string GetMyPath([CallerFilePath] string from = null)
        {
            return from;
        }
        public string GetProjectPath()
        {
            string projectPath = null;
            string fullPath =Path.GetDirectoryName(Path.GetFullPath(GetMyPath()));
            
            DirectoryInfo directory = new DirectoryInfo(fullPath);
            // Runtime path handling
            projectPath = directory.Parent?.FullName;
            MessageBox.Show(projectPath);

            return projectPath;
        }

        #region "Previewing Images"
        

        private void PreviewImage()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "ImagePath Files|*.bmp;*.jpg;*.jpeg;*.png;*.svg";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    previewFilePath = ofd.FileName;

                    try
                    {
                        // Display the image in PreviewpictureBox
                        string extension = Path.GetExtension(previewFilePath).ToLower();

                        if (extension == ".svg")
                        {
                            // Load SVG and render as Bitmap for preview
                            var svgDocument = SvgDocument.Open(previewFilePath);
                            PreviewpictureBox.Image = svgDocument.Draw();
                        }
                        else
                        {
                            // Load other image types
                            PreviewpictureBox.Image = new Bitmap(previewFilePath);
                        }
                        _isinPreview=true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error previewing image: {ex.Message}", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void PreviewResourceFromLocal()
        {
            if (ImagelistBox.SelectedItem == null) return;

            string filePath = ImagelistBox.SelectedItem.ToString();
            ImagePath = filePath;
        }
        private void PreviewLocalResource()
        {
            string filePath = ImagelistBox.SelectedItem.ToString();
            if (File.Exists(filePath))
            {
                try
                {
                    PreviewpictureBox.Image?.Dispose();
                    string extension = Path.GetExtension(filePath).ToLower();

                    if (extension == ".svg")
                    {
                        // Load SVG image and render as Bitmap
                        var svgDocument = SvgDocument.Open(filePath);
                        PreviewpictureBox.Image = svgDocument.Draw();
                    }
                    else
                    {
                        // Load other image formats
                        PreviewpictureBox.Image = new Bitmap(filePath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading local image: {ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("File not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PreviewProjectResxResource()
        {
            string resourceName = ImagelistBox.SelectedItem.ToString();
            string selectedResxFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ResourcesPathcomboBox.SelectedItem?.ToString() ?? "");

            try
            {
                using (var reader = new ResXResourceReader(selectedResxFile))
                {
                    foreach (DictionaryEntry entry in reader)
                    {
                        if (entry.Key.ToString() == resourceName)
                        {
                            PreviewpictureBox.Image?.Dispose();
                            if (entry.Value is Bitmap bitmap)
                            {
                                PreviewpictureBox.Image = bitmap;
                            }
                            else if (entry.Value is string svgPath && Path.GetExtension(svgPath).Equals(".svg", StringComparison.OrdinalIgnoreCase))
                            {
                                var svgDocument = SvgDocument.Open(svgPath);
                                PreviewpictureBox.Image = svgDocument.Draw();
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading .resx resource: {ex.Message}", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PreviewEmbeddedResource()
        {
            if (ResourcesPathcomboBox.SelectedItem == null) return;

            string resourceName = ResourcesPathcomboBox.SelectedItem.ToString();
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Format resource name to match fully qualified embedded resource names
            string formattedResourceName = assembly.GetName().Name + "." + resourceName.Replace(" ", "_").Replace("\\", ".").Replace("/", ".");

            using (Stream stream = assembly.GetManifestResourceStream(formattedResourceName))
            {
                if (stream != null)
                {
                    try
                    {
                        // Dispose of any previous image in the picture box
                        PreviewpictureBox.Image?.Dispose();

                        if (formattedResourceName.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                        {
                            // Handle SVG images
                            var svgDocument = SvgDocument.Open<SvgDocument>(stream);
                            PreviewpictureBox.Image = svgDocument.Draw(); // Convert SVG to Bitmap for display
                        }
                        else
                        {
                            // Handle non-SVG images
                            PreviewpictureBox.Image = new Bitmap(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading embedded resource: {ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"Embedded resource '{formattedResourceName}' not found. Please ensure the resource name is correct and properly formatted.",
                        "Resource Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        #endregion
        #region "Moving Images"
        private void EmbedImageInResources()
        {
            if (string.IsNullOrEmpty(previewFilePath))
            {
                MessageBox.Show("Please preview an image before embedding it.", "No ImagePath Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Set the destination path in the Resources folder
                string fileName = Path.GetFileNameWithoutExtension(previewFilePath);
                string projectDirectory =GetProjectPath();
                string resourcesPath = Path.Combine(projectDirectory, "Properties", "Resources");
                Directory.CreateDirectory(resourcesPath);

                string destPath = Path.Combine(resourcesPath, Path.GetFileName(previewFilePath));

                // Copy the file to the resources directory
                File.Copy(previewFilePath, destPath, true);

                // Update the .resx file if a resource path is selected in ResourcesPathcomboBox
                if (ResourcesPathcomboBox.SelectedItem != null)
                {
                    string resxFile = Path.Combine(projectDirectory, ResourcesPathcomboBox.SelectedItem.ToString());

                    // Load existing resources
                    Dictionary<string, object> existingResources = new Dictionary<string, object>();
                    using (ResXResourceReader resxReader = new ResXResourceReader(resxFile))
                    {
                        foreach (DictionaryEntry entry in resxReader)
                        {
                            existingResources[entry.Key.ToString()] = entry.Value;
                        }
                    }

                    // Add or update the new resource
                    existingResources[fileName] = new Bitmap(destPath);

                    // Write all resources back to the .resx file
                    using (ResXResourceWriter resxWriter = new ResXResourceWriter(resxFile))
                    {
                        foreach (var entry in existingResources)
                        {
                            resxWriter.AddResource(entry.Key, entry.Value);
                        }
                        resxWriter.Generate();
                    }
                }

               // MessageBox.Config("ImagePath embedded in resources successfully.", "Embedding Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SelectedImagePath = destPath;
                ImagePath = destPath;
                // Refresh the resources list
                RefreshResources();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error embedding image: {ex.Message}", "Embedding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string CopyFileToProjectResources()
        {
            if (string.IsNullOrEmpty(previewFilePath))
            {
                MessageBox.Show("Please preview an image before embedding it.", "No ImagePath Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            try
            {
                // Define the target folder within the project directory
                string projectDirectory = GetProjectPath();
                string resourcesFolder = Path.Combine(projectDirectory, "Resources");
                Directory.CreateDirectory(resourcesFolder); // Ensure the folder exists

                // Copy the previewed file to the Resources folder
                string destPath = Path.Combine(resourcesFolder, Path.GetFileName(previewFilePath));
                File.Copy(previewFilePath, destPath, true); // Overwrite if exists
                EmbedFileAsEmbeddedResource(filePath: destPath); // Embed the file in the .csproj file
              //  MessageBox.Config("File copied to project resources folder successfully.", "Copy Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SelectedImagePath = destPath;
                ImagePath = destPath;
                return destPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying file: {ex.Message}", "Copy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        private void EmbedFileAsEmbeddedResource(string filePath)
        {
            // Get the .csproj file path
            string projectDirectory = GetProjectPath();
            string csprojFilePath = Directory.GetFiles(projectDirectory, "*.csproj").FirstOrDefault();

            if (csprojFilePath == null)
            {
                MessageBox.Show("Could not find the .csproj file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the relative path to the file
            string relativeFilePath = Path.GetRelativePath(projectDirectory, filePath).Replace("\\", "/");

            try
            {
                // Load the .csproj file content
                var xmlDocument = XDocument.Load(csprojFilePath);

                // Find or create the <ItemGroup> for EmbeddedResource
                var itemGroup = xmlDocument.Descendants("ItemGroup")
                    .FirstOrDefault(ig => ig.Elements("EmbeddedResource").Any());

                if (itemGroup == null)
                {
                    itemGroup = new XElement("ItemGroup");
                    xmlDocument.Root.Add(itemGroup);
                }

                // Check if the file is already included as an embedded resource
                bool alreadyExists = itemGroup.Elements("EmbeddedResource")
                    .Any(er => er.Attribute("Include")?.Value == relativeFilePath);

                if (!alreadyExists)
                {
                    // Add the new EmbeddedResource element
                    itemGroup.Add(new XElement("EmbeddedResource", new XAttribute("Include", relativeFilePath)));

                    // Save the .csproj file
                    xmlDocument.Save(csprojFilePath);

                    MessageBox.Show("File marked as embedded resource in .csproj successfully. Please reload the project in Visual Studio to apply changes.", "Embedding Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("The file is already embedded as a resource in the project.", "Already Embedded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error embedding file as resource: {ex.Message}", "Embedding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MoveFileToProjectResources(string sourceFilePath, string destinationFolder)
        {
            try
            {
                // Ensure the destination folder exists within the project
                string projectResourceFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, destinationFolder);
                Directory.CreateDirectory(projectResourceFolder);

                // Copy the file to the project resource folder
                string fileName = Path.GetFileName(sourceFilePath);
                string destinationPath = Path.Combine(projectResourceFolder, fileName);

                File.Copy(sourceFilePath, destinationPath, true);

                MessageBox.Show($"File moved to project resource folder: {destinationPath}", "File Moved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error moving file: {ex.Message}", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion "Moving Images"
        #region "Loading Images"
        private void LoadResourceFilesToDictionary()
        {
            _imageResources.Clear();
            string[] possibleFolders = {
        GetProjectPath(),
        Path.Combine(GetProjectPath(), "Properties"),
        Path.Combine(GetProjectPath(), "Resources")
    };

            foreach (string folder in possibleFolders)
            {
                if (Directory.Exists(folder))
                {
                    string[] resxFiles = Directory.GetFiles(folder, "*.resx", SearchOption.AllDirectories);

                    foreach (var resxFile in resxFiles)
                    {
                        using (ResXResourceReader reader = new ResXResourceReader(resxFile))
                        {
                            foreach (DictionaryEntry entry in reader)
                            {
                                if (entry.Value is Bitmap) // Only add image resources
                                {
                                    string resourceKey = entry.Key.ToString();
                                    _imageResources[resourceKey] = resxFile; // Store resource key and .resx file path
                                }
                            }
                        }
                    }
                }
            }
        }
        private void LoadEmbeddedResourcesToDictionary()
        {
            _embeddedImages.Clear();
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();

            foreach (string resourceName in resourceNames)
            {
                if (resourceName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    resourceName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    resourceName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    resourceName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                    resourceName.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                {
                    _embeddedImages[resourceName] = resourceName; // Store the resource name as key and path
                }
            }
        }
        private void LoadProjectImagesToDictionary()
        {
            _projectImages.Clear();
            var resourceSet = Properties.Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);

            foreach (DictionaryEntry entry in resourceSet)
            {
                if (entry.Value is Bitmap) // Only add image resources
                {
                    _projectImages[entry.Key.ToString()] = entry.Key.ToString(); // Store resource key
                }
            }
        }
        private void LoadLocalImagesToDictionary()
        {
            _localImages.Clear();
            string localImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LocalImages");

            if (Directory.Exists(localImagePath))
            {
                var images = Directory.GetFiles(localImagePath, "*.*").Where(file =>
                    file.EndsWith(".bmp") || file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".png") || file.EndsWith(".svg"));

                foreach (var imagePath in images)
                {
                    string fileName = Path.GetFileName(imagePath);
                    _localImages[fileName] = imagePath; // Store file name and path
                }
            }
        }
        private void PopulateImagelistBoxFromDictionary()
        {
            ImagelistBox.Items.Clear();
            _isinPreview = false;
            switch (CurrentResourceType)
            {
                case "Local":
                    foreach (var item in _localImages)
                    {
                        ImagelistBox.Items.Add(item.Key); // Add file names
                    }
                    break;

                case "ProjectResx":
                    foreach (var item in _imageResources)
                    {
                        ImagelistBox.Items.Add(item.Key); // Add resource keys from .resx files
                    }
                    break;

                case "Embedded":
                    foreach (var item in _embeddedImages)
                    {
                        ImagelistBox.Items.Add(item.Key); // Add embedded resource names
                    }
                    break;

                case "Project":
                    foreach (var item in _projectImages)
                    {
                        ImagelistBox.Items.Add(item.Key); // Add project resource keys
                    }
                    break;

                default:
                    MessageBox.Show("Unknown resource type selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        #endregion "Loading Images"
        // Property to set and load image from path
        [Category("Appearance")]
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                LoadImageToPictureBox(_imagePath);
            }
        }
        private void LoadImageToPictureBox(string path)
        {
            // Clear previous image
            if (PreviewpictureBox.Image != null)
            {
                PreviewpictureBox.Image.Dispose();
                PreviewpictureBox.Image = null;
            }

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return;
            }

            try
            {
                // Check the file extension to determine how to load the image
                string extension = Path.GetExtension(path).ToLower();

                if (extension == ".svg")
                {
                    // Load SVG and render it as a Bitmap
                    SvgDocument svgDocument = SvgDocument.Open(path);
                    PreviewpictureBox.Image = svgDocument.Draw();  // Renders SVG as Bitmap
                }
                else if (extension == ".bmp" || extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                {
                    // Load regular image types directly
                    PreviewpictureBox.Image = Image.FromFile(path);
                }
                else
                {
                    MessageBox.Show("Unsupported file format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "ImagePath Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Allows external setting of the image path
    }
}

