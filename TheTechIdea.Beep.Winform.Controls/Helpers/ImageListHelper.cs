﻿
using Microsoft.Extensions.DependencyModel;
using Svg;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Xml.Linq;

using TheTechIdea.Beep.DriversConfigurations;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    public static class ImageListHelper
    {
        // Static references if needed (based on your snippet)
        public static IDMEEditor DMEEditor { get; set; }
        public static IAppManager Vismanager { get; set; }

        // This tracks all discovered images (local or embedded).
        public static List<ImageConfiguration> ImgAssemblies { get; set; } = new List<ImageConfiguration>();

        // An optional list of icons if .ico files are found
        public static List<Icon> Icons { get; set; } = new List<Icon>();

        // Optional: store a "big" or "small" logo if matched
        public static object LogoBigImage { get; set; }
        public static ImageType LogoBigImageType { get; set; } // can be any type
        public static object LogoSmallImage { get; set; }
        public static ImageType LogoSmallImageType { get; set; }

        // A simple incremental index
        private static int index = -1;

        #region "Local File Discovery"
        /// <summary>
        /// Scans the given folder (path) for .png or .ico images,
        /// creates ImageConfiguration for each, adds them to ImgAssemblies,
        /// and optionally sets LogoBigImage or LogoSmallImage if matched.
        /// </summary>
        /// <param name="path">The folder to scan.</param>
        /// <returns>A list of newly discovered ImageConfiguration objects.</returns>
        public static List<ImageConfiguration> GetGraphicFilesLocations(string path)
        {
            var result = new List<ImageConfiguration>();
            if (string.IsNullOrEmpty(path))
            {
                return result;
            }
            if (!Directory.Exists(path))
            {
                return result;
            }

            // 1) Iterate files in the folder
            foreach (string file in Directory.GetFiles(path))
            {
                string filename = Path.GetFileName(file);
                string extension = Path.GetExtension(filename)?.ToLowerInvariant();
                if (string.IsNullOrEmpty(extension))
                    continue;

                // 2) Convert extension to ImageType (null if not recognized)
                ImageType? type = ImageTypeExtensions.FromExtension(extension);
                if (type == null)
                    continue; // skip unrecognized

                // 3) Check if already in ImgAssemblies
                bool alreadyInList = ImgAssemblies.Any(
                    cfg => cfg.Name.Equals(filename, StringComparison.OrdinalIgnoreCase));

                if (!alreadyInList)
                {
                    // 4) Create a new ImageConfiguration
                    var config = new ImageConfiguration
                    {
                        Index = index++,
                        Name = filename,
                        Ext = extension,
                        Path = path,
                        FileName = filename,
                        ImageType = type.ToString() // store the enum as a string, or rename to an enum property
                    };
                    result.Add(config);

                    // 5) If it's an icon => add to Icons
                    if (type == ImageType.Ico || type == ImageType.Icon)
                    {
                        using (var icon = new Icon(file))
                        {
                            Icons.Add((Icon)icon.Clone());
                        }
                    }

                    // 6) Check for LogoBigImage
                    if (!string.IsNullOrEmpty(Vismanager?.LogoUrl))
                    {
                        // if the path contains the LogoUrl => set big logo
                        if (file.IndexOf(Vismanager.LogoUrl, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            // We load the file as an object 
                            // (maybe an Image or Icon, depending on the type)
                            (object obj, ImageType it) = ImageLoader.LoadLogoObjectFromFile(file, type.Value);
                            LogoBigImage = obj;
                            LogoBigImageType = it;
                        }
                    }

                    // 7) Check for LogoSmallImage
                    if (!string.IsNullOrEmpty(Vismanager?.IconUrl))
                    {
                        // if the filename is in the IconUrl => set small logo
                        if (Vismanager.IconUrl.IndexOf(filename, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            string iconPath = Vismanager.IconUrl;
                            if (File.Exists(iconPath))
                            {
                                string ext2 = Path.GetExtension(iconPath).ToLowerInvariant();
                                ImageType? smallType = ImageTypeExtensions.FromExtension(ext2);
                                if (smallType == ImageType.Ico || smallType == ImageType.Icon)
                                {
                                    using (var icon = new Icon(iconPath))
                                    {
                                        LogoSmallImage = icon.Clone();
                                        LogoSmallImageType = ImageType.Ico;
                                    }
                                }
                                else
                                {
                                    // Convert from image => icon or store as Image
                                    (object obj2, ImageType it2) = ImageLoader.LoadLogoObjectFromFile(iconPath, smallType ?? ImageType.Png);
                                    LogoSmallImage = obj2;
                                    LogoSmallImageType = it2;
                                }
                            }
                            else
                            {
                                Console.WriteLine($"File not found: {iconPath}");
                            }
                        }
                    }
                }
            }

            // 8) Add newly discovered items to global list, fill the ImageList
            if (result.Count > 0)
            {
                ImgAssemblies.AddRange(result);
               
            }

            return result;
        }
        #endregion
        #region "Embedded Resource Discovery"

        /// <summary>
        /// Scans multiple assemblies for embedded .png or .ico resources,
        /// creates ImageConfiguration, sets logos if matched, 
        /// adds them to ImgAssemblies, then calls FillImageList.
        /// </summary>
        /// <param name="namespaces">
        ///   Optional array of namespace strings to filter resources. 
        ///   If not null, resource must contain one of them to be considered.
        /// </param>
        /// <returns>A list of newly discovered ImageConfiguration objects.</returns>
        public static List<ImageConfiguration> GetGraphicFilesLocationsFromEmbedded(string[] namespaces)
        {
            var result = new List<ImageConfiguration>();

            // Gather assemblies from multiple sources
            var assemblies = new List<Assembly>
    {
        Assembly.GetExecutingAssembly(),
        Assembly.GetCallingAssembly(),
        Assembly.GetEntryAssembly()!
    };

            if (DMEEditor?.ConfigEditor?.LoadedAssemblies != null)
            {
                assemblies.AddRange(DMEEditor.ConfigEditor.LoadedAssemblies);
            }

            var loadedFromContext = DependencyContext.Default.RuntimeLibraries
                .SelectMany(lib => lib.GetDefaultAssemblyNames(DependencyContext.Default))
                .Select(Assembly.Load)
                .ToList();
            assemblies.AddRange(loadedFromContext);

            assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.FullName.StartsWith("System", StringComparison.OrdinalIgnoreCase)
                         && !a.FullName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase)));

            assemblies = assemblies.Distinct().ToList(); // remove duplicates

            foreach (Assembly assembly in assemblies)
            {
                // get resource names
                string[] resources = assembly.GetManifestResourceNames();

                foreach (string resource in resources)
                {
                    // if namespaces specified, ensure resource has one
                    if (namespaces != null && namespaces.Length > 0)
                    {
                        if (!namespaces.Any(ns => resource.IndexOf(ns, StringComparison.OrdinalIgnoreCase) >= 0))
                            continue;
                    }

                    // parse extension from resource
                    string extension = Path.GetExtension(resource)?.ToLowerInvariant();
                    if (string.IsNullOrEmpty(extension))
                        continue;

                    // map extension => ImageType
                    ImageType? type = ImageTypeExtensions.FromExtension(extension);
                    if (type == null)
                        continue; // skip if not recognized

                    // parse the "filename" portion
                    int lastDot = resource.LastIndexOf('.');
                    int secondToLastDot = resource.LastIndexOf('.', lastDot - 1);
                    if (secondToLastDot < 0 || lastDot < 0)
                        continue; // parse safety

                    string fileName = resource.Substring(secondToLastDot + 1, lastDot - secondToLastDot - 1)
                                             .ToLowerInvariant();

                    // check if we already have it
                    bool already = ImgAssemblies.Any(
                        cfg => cfg.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                    if (!already)
                    {
                        // create config
                        var config = new ImageConfiguration
                        {
                            Index = index++,
                            Name = fileName + extension,
                            Ext = extension,
                            Path = resource, // the embedded resource name
                            FileName = fileName + extension,
                            ImageType = type.ToString(),
                            AssemblyFullName = assembly.FullName,
                            AssemblyLocation = assembly.Location
                        };
                        result.Add(config);

                        // now check if .ico => add to icons
                        if (type == ImageType.Ico || type == ImageType.Icon)
                        {
                            using (Stream st = assembly.GetManifestResourceStream(resource))
                            {
                                if (st != null)
                                {
                                    using var ico = new Icon(st);
                                    Icons.Add(ico);
                                }
                            }
                        }

                        // check if resource name matches LogoUrl => set big logo
                        if (!string.IsNullOrEmpty(Vismanager?.LogoUrl) &&
                            Vismanager.LogoUrl.IndexOf(fileName, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            using Stream st = assembly.GetManifestResourceStream(resource);
                            if (st != null)
                            {
                                (object bigObj, ImageType bigType) = ImageLoader.LoadLogoObjectFromStream(st, type.Value);
                                LogoBigImage = bigObj;
                                LogoBigImageType = bigType;
                            }
                        }

                        // check if resource name matches IconUrl => set small logo
                        if (!string.IsNullOrEmpty(Vismanager?.IconUrl) &&
                            Vismanager.IconUrl.IndexOf(fileName, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            using Stream st = assembly.GetManifestResourceStream(resource);
                            if (st != null)
                            {
                                (object smallObj, ImageType smallType) = ImageLoader.LoadLogoObjectFromStream(st, type.Value);
                                LogoSmallImage = smallObj;
                                LogoSmallImageType = smallType;
                            }
                        }
                    }
                }
            }

            // if found new items, add them, fill image list
            if (result.Count > 0)
            {
                ImgAssemblies.AddRange(result);
               
            }

            return result;
        }
        #endregion
        #region "Image configuration helpers"
        public static List<ImageConfiguration> GetEmbeddedGraphicsInProj(string projectDirectory)
        {
            var results = new List<ImageConfiguration>();

            // 1) Locate the .csproj file
            string csprojFilePath = Directory.GetFiles(projectDirectory, "*.csproj")
                .FirstOrDefault();

            if (csprojFilePath == null)
            {
                // If not found, just return empty list
                return results;
            }

            // 2) Load the .csproj
            XDocument xmlDocument = XDocument.Load(csprojFilePath);

            // 3) Look for <ItemGroup><EmbeddedResource Include="..."/></ItemGroup>
            var embeddedResourceElements = xmlDocument
                .Descendants("ItemGroup")
                .SelectMany(ig => ig.Elements("EmbeddedResource"));

            foreach (var er in embeddedResourceElements)
            {
                var includeAttr = er.Attribute("Include");
                if (includeAttr == null) continue;

                // 4) Convert the relative path to absolute
                string relativePath = includeAttr.Value;
                string fullPath = Path.GetFullPath(
                    Path.Combine(projectDirectory, relativePath));

                // 5) Check extension => see if it's an image type
                string ext = Path.GetExtension(fullPath)?.ToLowerInvariant();
                ImageType? type = ImageTypeExtensions.FromExtension(ext);

                // If not recognized, skip
                if (type == null) continue;

                // 6) Check if we already have it in our global list (optional)
                bool already = ImgAssemblies.Any(cfg =>
                    cfg.Path?.Equals(Path.GetDirectoryName(fullPath),
                                     StringComparison.OrdinalIgnoreCase) == true
                    &&
                    cfg.FileName?.Equals(Path.GetFileName(fullPath),
                                         StringComparison.OrdinalIgnoreCase) == true);

                if (!already)
                {
                    // 7) Build a new ImageConfiguration
                    var cfg = new ImageConfiguration
                    {
                        Index = index++,
                        GuidID = Guid.NewGuid().ToString(), // unique ID
                        Name = Path.GetFileName(fullPath),  // e.g. "icon.png"
                        Description = "Discovered via .csproj EmbeddedResource",
                        Ext = ext,                         // e.g. ".png"
                        Path = Path.GetDirectoryName(fullPath),
                        FileName = Path.GetFileName(fullPath),
                        ImageType = type.ToString(),

                        // The file is physically on disk, but it is also an "EmbeddedResource"
                        // in the project. 
                        IsProjResource = true,
                        IsResxEmbedded = false,
                        IsFile = true,
                        IsUrl = false,
                        IsBase64 = false,
                        IsMemoryStream = false,
                        IsStream = false,

                        // We'll set these booleans based on the type
                        IsIcon = (type == ImageType.Icon || type == ImageType.Ico),
                        IsSVG = (type == ImageType.Svg),
                        IsImage = !(type == ImageType.Icon || type == ImageType.Ico || type == ImageType.Svg),

                        // We don't know actual pixel size until we load it, so either skip
                        // or set it to (0,0). 
                        Size = Size.Empty,

                        AssemblyFullName = null,
                        AssemblyLocation = null
                    };

                    results.Add(cfg);
                }
            }

            return results;
        }

        /// <summary>
        /// Scans a .resx file for any resources whose value is an image (Bitmap, Icon, etc.).
        /// Returns a list of newly discovered ImageConfiguration objects.
        /// </summary>
        /// <param name="resxFilePath">Full path to the .resx file.</param>
        public static List<ImageConfiguration> GetEmbeddedGraphicsInResx(string resxFilePath)
        {
            var results = new List<ImageConfiguration>();

            if (string.IsNullOrEmpty(resxFilePath))
                return results;
            if (!File.Exists(resxFilePath))
                return results;

            try
            {
                using (ResXResourceReader reader = new ResXResourceReader(resxFilePath))
                {
                    foreach (DictionaryEntry entry in reader)
                    {
                        // Each entry has a Key (string) and a Value (object)
                        string key = entry.Key?.ToString();
                        object val = entry.Value;

                        // 1) Check if it's a Bitmap or Icon or some other image
                        if (val is Bitmap || val is Icon || val is Image)
                        {
                            // 2) Let's guess the extension from the key if possible,
                            //    otherwise default to .png for bitmaps, etc.
                            string extensionGuess = ".png";
                            if (val is Icon) extensionGuess = ".ico";

                            // If the resource key itself ends with, say, ".jpg", we can parse it:
                            var possibleExt = Path.GetExtension(key ?? "");
                            if (!string.IsNullOrEmpty(possibleExt))
                            {
                                extensionGuess = possibleExt;
                            }

                            // 3) Convert to our ImageType if recognized
                            ImageType? type = ImageTypeExtensions.FromExtension(extensionGuess);

                            // 4) Build a new ImageConfiguration
                            //    For .resx-embedded items, you might store Path as the .resx filename,
                            //    or a combination of .resx file + resource key, etc.
                            var cfg = new ImageConfiguration
                            {
                                Index = index++,
                                GuidID = Guid.NewGuid().ToString(),
                                Name = key ?? $"ResxResource_{index}",
                                Description = "Discovered via .resx",
                                Ext = extensionGuess.ToLowerInvariant(),
                                Path = Path.GetDirectoryName(resxFilePath),
                                FileName = key,    // resource key
                                ImageType = (type?.ToString()) ?? "Png",

                                // This is from a .resx
                                IsResxEmbedded = true,
                                IsProjResource = false,
                                IsFile = false, // It's stored in the .resx, not as a direct file
                                IsUrl = false,
                                IsBase64 = false,
                                IsMemoryStream = false,
                                IsStream = false,

                                // If it's an Icon, set IsIcon; if it's a Bitmap, set IsImage
                                IsIcon = val is Icon,
                                IsSVG = false, // typically .resx won't store raw .svg as an object
                                IsImage = val is Bitmap || val is Image,

                                // We can attempt to get the actual pixel size if it's a Bitmap:
                                Size = (val is Image bmp) ? bmp.Size : Size.Empty,

                                AssemblyFullName = null,
                                AssemblyLocation = null
                            };
                            results.Add(cfg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading .resx: {ex.Message}");
            }

            return results;
        }

        /// <summary>
        /// Creates an ImageList of a given width and height from a list of ImageConfiguration.
        /// Each image is loaded on-demand from file/assembly, then optionally resized.
        /// </summary>
        /// <param name="configs">List of image metadata (ImageConfiguration).</param>
        /// <param name="desiredWidth">Width for the returned ImageList icons.</param>
        /// <param name="desiredHeight">Height for the returned ImageList icons.</param>
        /// <returns>A new ImageList containing loaded (and possibly resized) images.</returns>
        public static ImageList CreateImageList(
      List<ImageConfiguration> configs,
      int desiredWidth,
      int desiredHeight,
      bool getOnlyRightSize = true,
      ImageScaleMode scaleMode = ImageScaleMode.KeepAspectRatio)
        {
            // 1) Filter configurations upfront if getOnlyRightSize is true
            var filteredConfigs = getOnlyRightSize
                ? configs.Where(cfg => cfg.Size.Width == desiredWidth && cfg.Size.Height == desiredHeight).ToList()
                : configs;

            // 2) Initialize a fresh ImageList
            ImageList imgList = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new Size(desiredWidth, desiredHeight)
            };

            // 3) Iterate over the filtered configurations
            foreach (var cfg in filteredConfigs)
            {
                try
                {
                    // Attempt to load the image (from embedded resource, local file, or other source)
                    Image originalImage = ImageLoader.LoadImageFromConfig(cfg);
                    if (originalImage == null)
                        continue;

                    Image scaledImage = originalImage;

                    // 4) Resize the image if necessary (when getOnlyRightSize is false)
                    if (!getOnlyRightSize &&
                        (originalImage.Width != desiredWidth || originalImage.Height != desiredHeight))
                    {
                        if (cfg.IsSVG)
                        {
                            // Scale SVG using ScaleSvgImage logic
                            var svgDocument = SvgDocument.Open<SvgDocument>(new MemoryStream(File.ReadAllBytes(cfg.Path)));
                            Bitmap bmp = new Bitmap(desiredWidth, desiredHeight);
                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                g.Clear(Color.Transparent);
                                ImageConverters.ScaleSvgImage(g, svgDocument, new Rectangle(0, 0, desiredWidth, desiredHeight), 0, scaleMode);
                            }
                            scaledImage = bmp;
                        }
                        else
                        {
                            // Scale raster images using ScaleRegularImage logic
                            Bitmap bmp = new Bitmap(desiredWidth, desiredHeight);
                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                g.Clear(Color.Transparent);
                                ImageConverters.ScaleRegularImage(g, originalImage, new Rectangle(0, 0, desiredWidth, desiredHeight), 0, scaleMode);
                            }
                            scaledImage = bmp;
                        }

                        // Dispose the original image if resized
                        originalImage.Dispose();
                    }

                    // 5) Add the scaled image to the ImageList with a unique key
                    imgList.Images.Add(cfg.Name, scaledImage);

                    // Optionally dispose of the scaled image after adding it to the ImageList
                    scaledImage.Dispose();
                }
                catch (Exception ex)
                {
                    // Log or handle exceptions (e.g., file not found, invalid format, etc.)
                    Console.WriteLine($"Failed to load or process image for {cfg.Name}: {ex.Message}");
                }
            }

            return imgList;
        }
       

        #endregion "Image configuration helpers"
        #region "Get Images"
        // Updated method: 
        public static int GetImageIndexFromConnectioName(string ConnectionName)
        {
            try
            {
                string drname = null;
                string iconname = null;
                ConnectionDriversConfig connectionDrivers;

                var conn = DMEEditor.ConfigEditor.DataConnections
                    .FirstOrDefault(c => c.ConnectionName == ConnectionName);
                if (conn != null)
                {
                    drname = conn.DriverName;
                }

                if (drname == null)
                    return -1;

                string drversion = conn.DriverVersion;
                connectionDrivers = DMEEditor.ConfigEditor.DataDriversClasses
                    .FirstOrDefault(c => c.version == drversion && c.DriverClass == drname);
                if (connectionDrivers == null)
                {
                    connectionDrivers = DMEEditor.ConfigEditor.DataDriversClasses
                        .FirstOrDefault(c => c.DriverClass == drname);
                }

                if (connectionDrivers != null)
                {
                    iconname = connectionDrivers.iconname;
                }

                int imgindx = GetImageIndex(iconname);
                return imgindx;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        // Remove references to ImageList, just search in ImgAssemblies by name
        public static int GetImageIndex(string pimagename)
        {
            if (string.IsNullOrEmpty(pimagename))
                return -1;

            string imagename = pimagename.ToLowerInvariant();
            var cfg = ImgAssemblies
                .FirstOrDefault(x => x.Name.Equals(imagename, StringComparison.OrdinalIgnoreCase));
            if (cfg != null)
            {
                return cfg.Index;
            }
            return -1;
        }

        // If found in ImgAssemblies => load it, else null
        public static object GetImage(string pimagename)
        {
            if (string.IsNullOrEmpty(pimagename))
                return null;

            try
            {
                int idx = GetImageIndex(pimagename);
                if (idx > -1)
                {
                    return GetImageFromIndex(idx);
                }
                else
                {
                    // fallback => maybe do something else
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        // If size > 0 => do a quick resize, otherwise load original
        public static object GetImage(string pimagename, int size)
        {
            if (string.IsNullOrEmpty(pimagename))
                return null;

            try
            {
                int idx = GetImageIndex(pimagename);
                if (idx == -1)
                    return null;

                var original = GetImageFromIndex(idx);
                if (size <= 0 || original == null)
                    return original;

                // if you want to do a quick resizing for images
                if (original is Image bmp)
                {
                    // resize to new (size, size)
                    return new Bitmap(bmp, new Size(size, size));
                }
                // if it's an Icon, you might handle differently or skip
                return original;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Load by index from ImgAssemblies => call our new load method
        public static object GetImageFromIndex(int index)
        {
            if (index < 0 || index >= ImgAssemblies.Count)
                return null;

            var cfg = ImgAssemblies[index];
            return  ImageLoader.LoadImageFromConfig(cfg);
        }

        // If you still want these older methods, unify them
        public static object GetImageFromName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var cfg = ImgAssemblies
                .FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                                  || (c.Path?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false));
            if (cfg != null)
            {
                return ImageLoader.LoadImageFromConfig(cfg);
            }
            else
            {
                // fallback => if 'name' is actually a file path
                if (File.Exists(name))
                {
                    return Image.FromFile(name);
                }
                return null;
            }
        }

        // If you want a separate method for local file
        public static object GetImageFromFile(string fullname)
        {
            // 1) Validate input
            if (string.IsNullOrEmpty(fullname))
                return null;
            if (!File.Exists(fullname))
                return null;

            // 2) Determine extension
            string extension = Path.GetExtension(fullname)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(extension))
            {
                // If no extension, fallback => standard approach or null
                return null;
            }

            // 3) Switch logic based on extension
            //    If you prefer a single approach with "LoadObjectFromStream", 
            //    you can do that as well; but here's a simpler direct approach:
            try
            {
                // For .ico => load an Icon
                if (extension == ".ico")
                {
                    // Create an Icon from the file
                    using var icon = new Icon(fullname);
                    return icon.Clone(); // return a separate copy so the using icon is disposed
                }
                // For .svg => parse with Svg.NET
                else if (extension == ".svg")
                {
                    // Use SvgDocument from the Svg.NET library
                    var svgDoc = Svg.SvgDocument.Open<Svg.SvgDocument>(fullname);
                    // doc.Draw() => returns a Bitmap
                    return svgDoc.Draw();
                }
                else
                {
                    // standard e.g. .png, .jpg, .bmp, .gif, .tiff, etc.
                    return Image.FromFile(fullname);
                }
            }
            catch
            {
                // If something goes wrong (e.g., parsing fails), return null
                return null;
            }
        }
        // If you want a quick approach for "fullName" embedded
        public static object GetImageFromFullName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return null;

            // 1) Look up the ImageConfiguration by name or path in ImgAssemblies.
            var cfg = ImgAssemblies
                .FirstOrDefault(c =>
                    c.Name.Equals(fullName, StringComparison.OrdinalIgnoreCase)
                    || (!string.IsNullOrEmpty(c.Path)
                        && c.Path.Equals(fullName, StringComparison.OrdinalIgnoreCase)));

            // 2) If found, load from config. Otherwise, return null.
            if (cfg != null)
            {
                return ImageLoader.LoadImageFromConfig(cfg);
            }
            else
            {
                return null;
            }
        }
        public static string GetImagePathFromName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            var cfg = ImgAssemblies
                .FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                                  || (c.Path?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false));
            if (cfg != null)
            {
                return cfg.Path;
            }
            return null;
        }
        public static List<string> GetImageNames()
        {
            return ImgAssemblies.Select(x => x.Name).ToList();
        }
        #endregion "Get Images"
        #region "Async Methods"
        public static async Task<Image> LoadImageFromConfigAsync(ImageConfiguration cfg)
        {
            try
            {
                // 1) If it's embedded in a .resx
                if (cfg.IsResxEmbedded)
                {
                    var imageFromResx = LoadFromResx(cfg);
                    if (imageFromResx != null)
                        return imageFromResx;
                }

                // 2) If we have an assembly full name => embedded resource
                if (!string.IsNullOrEmpty(cfg.AssemblyFullName))
                {
                    var embedded = LoadFromEmbeddedAssembly(cfg);
                    if (embedded != null)
                        return embedded;
                }

                // 3) If it's physically on disk (IsFile == true)
                if (cfg.IsFile)
                {
                    // We'll do an async read of the file into memory, then parse.
                    return await LoadFromFileAsync(cfg);
                }

                // 4) If it's a URL (IsUrl), we do an async download
                if (cfg.IsUrl && !string.IsNullOrEmpty(cfg.Path))
                {
                    return await LoadFromUrlAsync(cfg);
                }

                // 5) If it's base64 (IsBase64), decode and load from memory
                if (cfg.IsBase64 && !string.IsNullOrEmpty(cfg.Path))
                {
                    return LoadFromBase64(cfg);
                }

                // 6) If it's some sort of in-memory stream (not shown in your class), 
                //    you could do an async approach reading from that stream.

                // If none match, return null
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadImageFromConfigAsync error: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Loads an image from a .resx file synchronously (since ResXResourceReader is sync-only).
        /// </summary>
        private static Image LoadFromResx(ImageConfiguration cfg)
        {
            if (string.IsNullOrEmpty(cfg.Path) || string.IsNullOrEmpty(cfg.FileName))
                return null;

            if (!File.Exists(cfg.Path))
                return null;

            using var reader = new ResXResourceReader(cfg.Path);
            foreach (DictionaryEntry entry in reader)
            {
                if (string.Equals(entry.Key?.ToString(), cfg.FileName, StringComparison.OrdinalIgnoreCase))
                {
                    if (entry.Value is Icon iconVal)
                        return iconVal.ToBitmap();

                    if (entry.Value is Bitmap bmpVal)
                        return bmpVal;

                    if (entry.Value is Image imgVal)
                        return imgVal;

                    // if it's stored as base64 string or something else, handle accordingly
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Loads an image from an embedded resource in the specified assembly (synchronous).
        /// </summary>
        private static Image LoadFromEmbeddedAssembly(ImageConfiguration cfg)
        {
            var asm = MiscFunctions.LoadAssembly(cfg.AssemblyFullName);
            if (asm == null)
                return null;

            using var stream = asm.GetManifestResourceStream(cfg.Path);
            if (stream == null)
                return null;

            // We can do a small helper for extension detection
            string ext = GetExtension(cfg).ToLowerInvariant();
            return LoadImageFromStream(stream, ext);
        }

        /// <summary>
        /// Async method to load from a file. Reads the file into memory, then interprets it.
        /// </summary>
        private static async Task<Image> LoadFromFileAsync(ImageConfiguration cfg)
        {
            // Combine the path + file
            var fullFilePath = Path.Combine(cfg.Path ?? "", cfg.FileName ?? cfg.Name);
            if (!File.Exists(fullFilePath))
                return null;

            string extension = Path.GetExtension(fullFilePath).ToLowerInvariant();

            // Asynchronously read the file contents
            byte[] fileData = await File.ReadAllBytesAsync(fullFilePath);

            // Now parse the data
            using var ms = new MemoryStream(fileData);
            if (extension == ".svg")
            {
                // Svg.NET is currently sync-only, so do it here:
                var svgDoc = SvgDocument.Open<SvgDocument>(ms);
                return svgDoc.Draw();
            }
            else
            {
                // This call is sync, but we're loading from memory, so it's usually fast
                return Image.FromStream(ms);
            }
        }

        /// <summary>
        /// Async method to load from a URL using HttpClient.
        /// </summary>
        private static async Task<Image> LoadFromUrlAsync(ImageConfiguration cfg)
        {
            using var httpClient = new HttpClient();
            byte[] data = await httpClient.GetByteArrayAsync(cfg.Path);

            // Decide if it might be an SVG from the URL extension or content-type
            // For simplicity, we just do a naive check by extension:
            string ext = GetExtension(cfg).ToLowerInvariant();

            using var ms = new MemoryStream(data);
            if (ext == ".svg")
            {
                var svgDoc = SvgDocument.Open<SvgDocument>(ms);
                return svgDoc.Draw();
            }
            else
            {
                return Image.FromStream(ms);
            }
        }

        /// <summary>
        /// Loads from a base64 string stored in cfg.Path (synchronous decode).
        /// </summary>
        private static Image LoadFromBase64(ImageConfiguration cfg)
        {
            byte[] bytes = Convert.FromBase64String(cfg.Path);
            using var ms = new MemoryStream(bytes);

            // If you want to handle .svg base64, you’d need a clue that it's SVG. 
            // For example, if cfg.Ext == ".svg"
            string ext = GetExtension(cfg).ToLowerInvariant();
            if (ext == ".svg")
            {
                var svgDoc = SvgDocument.Open<SvgDocument>(ms);
                return svgDoc.Draw();
            }
            else
            {
                return Image.FromStream(ms);
            }
        }

        /// <summary>
        /// Synchronous helper to load an image from a stream,
        /// properly handling .svg vs. other types.
        /// </summary>
        private static Image LoadImageFromStream(Stream st, string extension)
        {
            extension = extension?.ToLowerInvariant();
            if (extension == ".svg")
            {
                // Convert the stream to an SvgDocument
                var svgDoc = SvgDocument.Open<SvgDocument>(st);
                return svgDoc.Draw();
            }
            else
            {
                // standard approach for PNG, JPG, BMP, ICO, etc.
                using var mem = new MemoryStream();
                st.CopyTo(mem);
                mem.Position = 0;
                return Image.FromStream(mem);
            }
        }

        /// <summary>
        /// Determine the file/resource extension from ImageConfiguration.
        /// If cfg.Ext is not empty, return that; otherwise parse from FileName or Path.
        /// </summary>
        private static string GetExtension(ImageConfiguration cfg)
        {
            if (!string.IsNullOrEmpty(cfg.Ext))
                return cfg.Ext.ToLowerInvariant();

            string candidate = cfg.FileName ?? cfg.Name ?? cfg.Path;
            if (!string.IsNullOrEmpty(candidate))
                return Path.GetExtension(candidate).ToLowerInvariant();

            return string.Empty;
        }
        #endregion "Async Methods"
        #region "Stream Methods for Loading Images"
        public static async Task<Image> LoadImageFromUrlAsync(string url)
        {
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Error fetching {url}: {response.StatusCode}");

            // Get the HTTP stream
            using var httpStream = await response.Content.ReadAsStreamAsync();
            using var ms = new MemoryStream();

            // Asynchronously copy the stream in chunks
            await httpStream.CopyToAsync(ms);

            ms.Position = 0; // Reset stream
                             // Parse with GDI+ (still sync):
            return Image.FromStream(ms);
        }
        public static async Task<Image> LoadSvgFromUrlAsync(string url)
        {
            using var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();
            using var httpStream = await response.Content.ReadAsStreamAsync();

            using var ms = new MemoryStream();
            await httpStream.CopyToAsync(ms);

            // parse .svg after fully reading
            ms.Position = 0;
            var svgDoc = SvgDocument.Open<SvgDocument>(ms);
            return svgDoc.Draw();
        }
        public static async Task<Image> LoadImageAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            // Open a FileStream in async mode.
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
                                          bufferSize: 4096, useAsync: true);

            // Option A: Stream all bytes into MemoryStream, then decode
            using var ms = new MemoryStream();

            // Asynchronously copy file data to MemoryStream in chunks
            await fs.CopyToAsync(ms);

            ms.Position = 0; // Reset position

            // Now parse with GDI+ (sync call):
            Image image = Image.FromStream(ms);
            return image;
        }
        public static async Task<Image> LoadSvgAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");
            // Open a FileStream in async mode.
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
                                          bufferSize: 4096, useAsync: true);
            // Option A: Stream all bytes into MemoryStream, then decode
            using var ms = new MemoryStream();
            // Asynchronously copy file data to MemoryStream in chunks
            await fs.CopyToAsync(ms);
            ms.Position = 0; // Reset position
            // Now parse with Svg.NET (sync call):
            var svgDoc = SvgDocument.Open<SvgDocument>(ms);
            return svgDoc.Draw();
        }
        public static async Task<Image> LoadImageFromBase64Async(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            using var ms = new MemoryStream(bytes);
            ms.Position = 0; // Reset position
            // Now parse with GDI+ (sync call):
            Image image = Image.FromStream(ms);
            return image;
        }
        public static async Task<Image> LoadSvgFromBase64Async(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            using var ms = new MemoryStream(bytes);
            ms.Position = 0; // Reset position
            // Now parse with Svg.NET (sync call):
            var svgDoc = SvgDocument.Open<SvgDocument>(ms);
            return svgDoc.Draw();
        }
        public static async Task<Image> LoadImageFromStreamAsync(Stream stream)
        {
            // Option A: Stream all bytes into MemoryStream, then decode
            using var ms = new MemoryStream();
            // Asynchronously copy stream data to MemoryStream in chunks
            await stream.CopyToAsync(ms);
            ms.Position = 0; // Reset position
            // Now parse with GDI+ (sync call):
            Image image = Image.FromStream(ms);
            return image;
        }
        public static async Task<Image> LoadSvgFromStreamAsync(Stream stream)
        {
            // Option A: Stream all bytes into MemoryStream, then decode
            using var ms = new MemoryStream();
            // Asynchronously copy stream data to MemoryStream in chunks
            await stream.CopyToAsync(ms);
            ms.Position = 0; // Reset position
            // Now parse with Svg.NET (sync call):
            var svgDoc = SvgDocument.Open<SvgDocument>(ms);
            return svgDoc.Draw();
        }
        

        #endregion "Stream Methods for Loading Images"
    }
}
