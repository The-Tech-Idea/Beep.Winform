using TheTechIdea.Beep.Vis.Modules;
using Microsoft.Extensions.DependencyModel;
using Svg;
using System.Reflection;
using System.Collections;
using System.Resources;
using System.Xml.Linq;
using TheTechIdea.Beep.Winform.Controls.Template;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    public static class ImageLoader
    {
        private static int index;
        private static string[] extensions = { ".png", ".ico", ".svg", ".bmp", ".jpg" };
        public static List<ImageConfiguration> ImgAssemblies { get; set; } = new List<ImageConfiguration>();
        public static List<Icon> Icons { get; set; } = new List<Icon>();

        public static List<ImageConfiguration> GetGraphicFilesLocations(string path)
        {
            var result = new List<ImageConfiguration>();
            // Add extensions to look for
            
            if (string.IsNullOrEmpty(path))
            {
                return result;
            }
            if (Directory.Exists(path))
            {
                // Iterate through the files in the folder
                foreach (string file in Directory.GetFiles(path))
                {
                    string filename = Path.GetFileName(file);
                    string extension = Path.GetExtension(filename);
                    // Check if the file has one of the specified extensions
                    if (Array.Exists(extensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (!ImgAssemblies.Any(ext => ext.Name.Equals(filename, StringComparison.OrdinalIgnoreCase)))
                        {
                            result.Add(new ImageConfiguration
                            {
                                Index = index++,
                                Name = filename,
                                Ext = extension,
                                Path = path
                            });
                            if (extension == ".ico")
                            {
                                using (Icon icon = new Icon(file))
                                {
                                    Icons.Add(icon);
                                }
                            }
                         
                        }
                    }
                }
            }
            if (result.Count > 0)
            {
                ImgAssemblies.AddRange(result);
                //FillImageList(result);
            }

            return result;
        }
        public static List<ImageConfiguration> GetGraphicFilesLocationsFromEmbedded(string[] namesspaces)
        {
            var result = new List<ImageConfiguration>();
            // Add extensions to look for
         
            // namesspaces= { "BeepEnterprize","Koc","DHUB","TheTechIdea","Beep" };
            // Get current, executing, and calling assemblies
            List<Assembly> assemblies = new Assembly[]{
                Assembly.GetExecutingAssembly(),
                Assembly.GetCallingAssembly(),
                Assembly.GetEntryAssembly()!       }.ToList();
            List<Assembly> LoadedAssemblies = DependencyContext.Default.RuntimeLibraries
 .SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
 .Select(Assembly.Load)
 .ToList();
            assemblies.AddRange(LoadedAssemblies);
            // Load all assemblies from the current domain to ensure referenced projects are included
            assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.FullName.StartsWith("System") && !assembly.FullName.StartsWith("Microsoft")));
            foreach (Assembly assembly in assemblies)
            {
               
                // Get all embedded resources
                string[] resources = assembly.GetManifestResourceNames();

                foreach (string resource in resources)
                {
                    // Check if the resource name contains any of the specified namespaces
                    if (namesspaces != null)
                    {
                        if (!namesspaces.Any(ns => resource.Contains(ns, StringComparison.OrdinalIgnoreCase)))
                        {
                            continue; // Skip this resource as it doesn't match the namespace criteria
                        }

                    }
                    foreach (string extension in extensions)
                    {
                        if (resource.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                        {
                            int lastDot = resource.LastIndexOf('.');
                            int secondToLastDot = resource.LastIndexOf('.', lastDot - 1);

                            string fileName = (resource.Substring(secondToLastDot + 1, lastDot - secondToLastDot - 1)).ToLower();

                            if (!ImgAssemblies.Any(ext => ext.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase)))
                            {
                                result.Add(new ImageConfiguration
                                {
                                    Index = index++,
                                    Name = fileName + extension,
                                    Ext = extension,
                                    Path = resource,
                                    Assembly = assembly
                                });
                                if (extension == ".ico")
                                {
                                    using (Stream stream = assembly.GetManifestResourceStream(resource))
                                    {
                                        if (stream != null)
                                        {
                                            Icons.Add(new Icon(stream));
                                        }
                                    }
                                }
                                // Check for LogoBigImage based on LogoUrl
                            }

                            break;
                        }
                    }
                }
            }

            ImgAssemblies.AddRange(result);
            //FillImageList(result);
            return result;
        }
        public static bool IsEmbeddedResource(string path)
        {
            // check if path has more than one dot
            if (path.Split('.').Length > 2)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public static void LoadImageFromFile(string path)
        {
            string extension = Path.GetExtension(path).ToLower();
            switch (extension)
            {
                case ".svg":
                    LoadSvg(path);
                    break;
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                    LoadRegularImage(path);
                    break;
                default:
                    throw new ArgumentException("Unsupported image format. Supported formats are: SVG, PNG, JPG, BMP.");
            }
        }
        public static Tuple<bool,object> LoadImageFromEmbeddedResource(string resourcePath)
        {
            bool isSvg = false;
            object image = null;
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream(resourcePath))
                {
                    if (stream != null)
                    {
                        // Check file extension to determine the type
                        string extension = Path.GetExtension(resourcePath).ToLower();
                        if (extension == ".svg")
                        {
                            SvgDocument svgDocument = SvgDocument.Open<SvgDocument>(stream);
                            isSvg = true;
                        }
                        else
                        {
                            Image regularImage = Image.FromStream(stream);
                            isSvg = false;
                        }
                        return new Tuple<bool, object>(isSvg, image);
                    }
                    else
                    {
                        throw new FileNotFoundException($"Embedded resource not found: {resourcePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading embedded resource: {ex.Message}", "Embedded Resource Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new Tuple<bool, object>(isSvg, null);
            }
        }
        public static SvgDocument LoadSvg(string svgPath)
        {
            try
            {
                SvgDocument svgDocument = SvgDocument.Open(svgPath);
                return svgDocument;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading SVG: {ex.Message}", "SVG Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        public static Image LoadRegularImage(string imagePath)
        {
            try
            {
                Image regularImage = Image.FromFile(imagePath);
                regularImage.Tag = imagePath; // Store path for future reference
               return regularImage;
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"Error loading image: {ex.Message}", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        #region "Previewing Images"


        public static void PreviewImage(PictureBox PreviewpictureBox, string resourceName)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.svg";

                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    resourceName = ofd.FileName;

                    try
                    {
                        // Display the image in PreviewpictureBox
                        string extension = Path.GetExtension(resourceName).ToLower();

                        if (extension == ".svg")
                        {
                            // Load SVG and render as Bitmap for preview
                            var svgDocument = SvgDocument.Open(resourceName);
                            PreviewpictureBox.Image = svgDocument.Draw();
                        }
                        else
                        {
                            // Load other image types
                            PreviewpictureBox.Image = new Bitmap(resourceName);
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error previewing image: {ex.Message}", "Preview Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        public static void PreviewImageFromFile(PictureBox previewPictureBox, SimpleMenuItem menuItem)
        {
            if (menuItem == null || string.IsNullOrEmpty(menuItem.Image))
            {
                MessageBox.Show("No valid image path provided.", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                string imagePath = menuItem.Image;

                // Dispose any previously loaded image to free resources
                previewPictureBox.Image?.Dispose();

                // Check if the image is SVG or a standard format and load accordingly
                if (imagePath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                {
                    // Load and render SVG image
                    using (var svgStream = File.OpenRead(imagePath))
                    {
                        var svgDocument = Svg.SvgDocument.Open<Svg.SvgDocument>(svgStream);
                        previewPictureBox.Image = svgDocument.Draw();
                    }
                }
                else
                {
                    // Load and render standard bitmap formats (e.g., .png, .jpg)
                    previewPictureBox.Image = Image.FromFile(imagePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image from file: {ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void PreviewLocalResource(PictureBox PreviewpictureBox, string resourceName)
        {
          //  string filePath = ImagelistBox.SelectedItem.ToString();
            if (File.Exists(resourceName))
            {
                try
                {
                    PreviewpictureBox.Image?.Dispose();
                    string extension = Path.GetExtension(resourceName).ToLower();

                    if (extension == ".svg")
                    {
                        // Load SVG image and render as Bitmap
                        var svgDocument = SvgDocument.Open(resourceName);
                        PreviewpictureBox.Image = svgDocument.Draw();
                    }
                    else
                    {
                        // Load other image formats
                        PreviewpictureBox.Image = new Bitmap(resourceName);
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
        public static void PreviewProjectResxResource(PictureBox PreviewpictureBox, string resourceName)
        {
          //  string resourceName = ImagelistBox.SelectedItem.ToString();
            string selectedResxFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, resourceName ?? "");

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
        public static void PreviewEmbeddedResource(PictureBox PreviewpictureBox,string resourceName)
        {
            //if (ResourcesPathcomboBox.SelectedItem == null) return;

            //string resourceName = ResourcesPathcomboBox.SelectedItem.ToString();
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
        public static void EmbedImageInResources(Dictionary<string, SimpleMenuItem> _imageResources, string resxFile, string previewFilePath, string projectDirectory)
        {
            if (string.IsNullOrEmpty(previewFilePath))
            {
                MessageBox.Show("Please preview an image before embedding it.", "No Image Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Set the destination path in the Resources folder
                string fileName = Path.GetFileNameWithoutExtension(previewFilePath);
                string resourcesPath = Path.Combine(projectDirectory, "Properties", "Resources");
                Directory.CreateDirectory(resourcesPath);

                string destPath = Path.Combine(resourcesPath, Path.GetFileName(previewFilePath));
                File.Copy(previewFilePath, destPath, true); // Copy the file

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
                using (ResXResourceWriter resxWriter = new ResXResourceWriter(resxFile))
                {
                    foreach (var entry in existingResources)
                    {
                        resxWriter.AddResource(entry.Key, entry.Value);
                    }
                }

                // Add to _imageResources dictionary
                _imageResources[fileName] = new SimpleMenuItem { Name = fileName, Image = destPath };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error embedding image: {ex.Message}", "Embedding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static string CopyFileToProjectResources(Dictionary<string, SimpleMenuItem> _projectResources, string previewFilePath, string projectDirectory)
        {
            if (string.IsNullOrEmpty(previewFilePath))
            {
                MessageBox.Show("Please preview an image before embedding it.", "No Image Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            try
            {
                string resourcesFolder = Path.Combine(projectDirectory, "Resources");
                Directory.CreateDirectory(resourcesFolder); // Ensure the folder exists

                string destPath = Path.Combine(resourcesFolder, Path.GetFileName(previewFilePath));
                File.Copy(previewFilePath, destPath, true); // Copy file
                EmbedFileAsEmbeddedResource(previewFilePath, destPath, projectDirectory);

                _projectResources[Path.GetFileNameWithoutExtension(previewFilePath)] = new SimpleMenuItem { Name = Path.GetFileNameWithoutExtension(previewFilePath), Image = destPath };
                return destPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying file: {ex.Message}", "Copy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        public static void EmbedFileAsEmbeddedResource(string filePath, string previewFilePath, string projectDirectory)
        {
            string csprojFilePath = Directory.GetFiles(projectDirectory, "*.csproj").FirstOrDefault();
            if (csprojFilePath == null)
            {
                MessageBox.Show("Could not find the .csproj file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string relativeFilePath = Path.GetRelativePath(projectDirectory, filePath).Replace("\\", "/");

            try
            {
                var xmlDocument = XDocument.Load(csprojFilePath);
                var itemGroup = xmlDocument.Descendants("ItemGroup").FirstOrDefault(ig => ig.Elements("EmbeddedResource").Any());
                if (itemGroup == null)
                {
                    itemGroup = new XElement("ItemGroup");
                    xmlDocument.Root.Add(itemGroup);
                }

                bool alreadyExists = itemGroup.Elements("EmbeddedResource").Any(er => er.Attribute("Include")?.Value == relativeFilePath);
                if (!alreadyExists)
                {
                    itemGroup.Add(new XElement("EmbeddedResource", new XAttribute("Include", relativeFilePath)));
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
        public static string CopyAndEmbedFileToProjectResources(
    Dictionary<string, SimpleMenuItem> projectResources,
    string previewFilePath,
    string projectDirectory)
        {
            if (string.IsNullOrEmpty(previewFilePath))
            {
                MessageBox.Show("Please preview an image before embedding it.", "No Image Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            try
            {
                // Define the target folder within the project directory
                string resourcesFolder = Path.Combine(projectDirectory, "Resources");
                Directory.CreateDirectory(resourcesFolder); // Ensure the folder exists

                // Copy the previewed file to the Resources folder
                string fileName = Path.GetFileName(previewFilePath);
                string destPath = Path.Combine(resourcesFolder, fileName);
                File.Copy(previewFilePath, destPath, true); // Copy and overwrite if exists

                // Now embed it as an embedded resource in the .csproj file
                string csprojFilePath = Directory.GetFiles(projectDirectory, "*.csproj").FirstOrDefault();
                if (csprojFilePath == null)
                {
                    MessageBox.Show("Could not find the .csproj file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                string relativeFilePath = Path.GetRelativePath(projectDirectory, destPath).Replace("\\", "/");
                var xmlDocument = XDocument.Load(csprojFilePath);
                var itemGroup = xmlDocument.Descendants("ItemGroup").FirstOrDefault(ig => ig.Elements("EmbeddedResource").Any());

                if (itemGroup == null)
                {
                    itemGroup = new XElement("ItemGroup");
                    xmlDocument.Root.Add(itemGroup);
                }

                bool alreadyExists = itemGroup.Elements("EmbeddedResource").Any(er => er.Attribute("Include")?.Value == relativeFilePath);
                if (!alreadyExists)
                {
                    itemGroup.Add(new XElement("EmbeddedResource", new XAttribute("Include", relativeFilePath)));
                    xmlDocument.Save(csprojFilePath);
                    MessageBox.Show("File marked as embedded resource in .csproj successfully. Please reload the project in Visual Studio to apply changes.", "Embedding Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("The file is already embedded as a resource in the project.", "Already Embedded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Add to projectResources dictionary as SimpleMenuItem
                var simpleMenuItem = new SimpleMenuItem { Name = Path.GetFileNameWithoutExtension(fileName), Image = destPath };
                projectResources[Path.GetFileNameWithoutExtension(fileName)] = simpleMenuItem;

                return destPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying and embedding file: {ex.Message}", "Copy and Embed Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static void MoveFileToProjectResources(Dictionary<string, SimpleMenuItem> _localImages, string sourceFilePath, string destinationFolder)
        {
            try
            {
                string projectResourceFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, destinationFolder);
                Directory.CreateDirectory(projectResourceFolder);

                string fileName = Path.GetFileName(sourceFilePath);
                string destinationPath = Path.Combine(projectResourceFolder, fileName);

                File.Copy(sourceFilePath, destinationPath, true);

                _localImages[fileName] = new SimpleMenuItem { Name = fileName, Image = destinationPath };
                MessageBox.Show($"File moved to project resource folder: {destinationPath}", "File Moved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error moving file: {ex.Message}", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #endregion "Moving Images"
        #region "Loading Images"
        public static void LoadResourceFilesToDictionary(Dictionary<string, SimpleMenuItem> _imageResources, string[] possibleFolders)
        {
            _imageResources.Clear();
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
                                    SimpleMenuItem item = new SimpleMenuItem
                                    {
                                        Name = resourceKey,
                                        Image = resxFile // Path to the .resx file
                                    };
                                    _imageResources[resourceKey] = item;
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void LoadEmbeddedImagesToDictionary(Dictionary<string, SimpleMenuItem> _embeddedImages, string projectDirectory)
        {
            _embeddedImages.Clear();

            // Get the .csproj file path
            string csprojFilePath = Directory.GetFiles(projectDirectory, "*.csproj").FirstOrDefault();
            if (csprojFilePath == null)
            {
                MessageBox.Show("Could not find the .csproj file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Load the .csproj file to identify embedded resources
                var xmlDocument = XDocument.Load(csprojFilePath);

                // Find all <EmbeddedResource> items in the .csproj file
                var embeddedResources = xmlDocument.Descendants("EmbeddedResource")
                    .Select(er => er.Attribute("Include")?.Value)
                    .Where(path => path != null &&
                                  (path.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                   path.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                   path.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                   path.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                                   path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                // Add each embedded resource to the dictionary
                foreach (var resourcePath in embeddedResources)
                {
                    // Construct the resource name as it would be in the assembly
                    string resourceName = Path.GetFileNameWithoutExtension(resourcePath);

                    // Add to dictionary with SimpleMenuItem structure
                    SimpleMenuItem item = new SimpleMenuItem
                    {
                        Name = resourceName,
                        Image = resourcePath // Store the relative path
                    };
                    _embeddedImages[resourceName] = item;
                }

                MessageBox.Show("Embedded images loaded successfully.", "Load Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading embedded images: {ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void LoadEmbeddedResourcesToDictionary(Dictionary<string, SimpleMenuItem> _embeddedImages)
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
                    SimpleMenuItem item = new SimpleMenuItem
                    {
                        Name = resourceName,
                        Image = resourceName // Store the resource name for embedded resources
                    };
                    _embeddedImages[resourceName] = item;
                }
            }
        }

        public static void LoadProjectImagesToDictionary(Dictionary<string, SimpleMenuItem> _projectImages)
        {
            _projectImages.Clear();
            var resourceSet = Properties.Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);

            foreach (DictionaryEntry entry in resourceSet)
            {
                if (entry.Value is Bitmap) // Only add image resources
                {
                    string resourceKey = entry.Key.ToString();
                    SimpleMenuItem item = new SimpleMenuItem
                    {
                        Name = resourceKey,
                        Image = resourceKey // Store the resource key for project resources
                    };
                    _projectImages[resourceKey] = item;
                }
            }
        }

        public static void LoadLocalImagesToDictionary(Dictionary<string, SimpleMenuItem> _localImages)
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
                    SimpleMenuItem item = new SimpleMenuItem
                    {
                        Name = fileName,
                        Text = fileName,
                        Image = imagePath // Store the file path for local images
                    };
                    _localImages[fileName] = item;
                }
            }
        }

        #endregion "Loading Images"
        public static string GetProjectPath(string GetMyPath)
        {

            string projectPath = null;
            string fullPath = Path.GetDirectoryName(Path.GetFullPath(GetMyPath));

            DirectoryInfo directory = new DirectoryInfo(fullPath);
            // Runtime path handling
            projectPath = directory.Parent?.FullName;
            MessageBox.Show(projectPath);

            return projectPath;
        }
        public static void LoadImageToPictureBox(PictureBox PreviewpictureBox,string path)
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
                MessageBox.Show($"Error loading image: {ex.Message}", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
