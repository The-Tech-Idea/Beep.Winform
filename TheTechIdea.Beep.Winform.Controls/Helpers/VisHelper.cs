using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using TheTechIdea.Beep.Desktop.Common;

namespace TheTechIdea.Beep.Winform.Controls.Helpers
{
    public class VisHelper : IVisHelper
    {

        public VisHelper()
        {
            
        }
        public VisHelper(IDMEEditor editor, IAppManager vismanager)
        {
            DMEEditor = editor;
            Vismanager = vismanager;
        }
        public IDMEEditor DMEEditor { get  ; set  ; }
        public IAppManager Vismanager { get  ; set  ; }
        public List<ImageConfiguration> ImgAssemblies { get  ; set  ; }
        public object LogoBigImage { get  ; set  ; }
        public object LogoSmallImage { get  ; set  ; }

        public List<ImageConfiguration> GetGraphicFilesLocations(string path)
        {
             ImageListHelper.GetGraphicFilesLocations(path);
            ImgAssemblies = ImageListHelper.ImgAssemblies;

            return ImgAssemblies;
        }

        public List<ImageConfiguration> GetGraphicFilesLocationsFromEmbedded(string[] namespacestoinclude)
        {
             ImageListHelper.GetGraphicFilesLocationsFromEmbedded(namespacestoinclude);
            ImgAssemblies = ImageListHelper.ImgAssemblies;
            return ImgAssemblies;
        }

        public object GetImage(string imagename)
        {
            return ImageListHelper.GetImage(imagename);
        }

        public object GetImage(string imagename, int size)
        {
           return ImageListHelper.GetImage(imagename, size);
        }

        public object GetImageFromIndex(int index)
        {
            return ImageListHelper.GetImageFromIndex(index);
        }

        public object GetImageFromName(string name)
        {
            return  ImageListHelper.GetImageFromName(name);
        }

        public int GetImageIndex(string name)
        {
            return ImageListHelper.GetImageIndex(name);
        }

        public int GetImageIndexFromConnectioName(string Connectioname)
        {
           return ImageListHelper.GetImageIndexFromConnectioName(Connectioname);
        }

        public List<string> GetImageNames()
        {
            return ImageListHelper.GetImageNames();
        }

        public object GetResource(string resource)
        {
            return ImageListHelper.GetImageFromName(resource);
        }

        public void RefreshTreeView()
        {
           
        }
    }

    //   public class VisHelper : IVisHelper
    //   {
    //       public VisHelper(IDMEEditor pDMEEditor, IAppManager pVismanager)
    //       {
    //           DMEEditor = pDMEEditor;
    //           Vismanager = pVismanager;

    //           ImageList16 = new ImageList();
    //           ImageList16.ColorDepth = ColorDepth.Depth32Bit;
    //           ImageList16.RightButtonSize = new Size(24, 24);
    //           ImageList16.Images.Clear(); // Clear existing images if any


    //           ImageList32 = new ImageList();
    //           ImageList32.ColorDepth = ColorDepth.Depth32Bit;
    //           ImageList32.RightButtonSize = new Size(32, 32);
    //           ImageList32.Images.Clear(); // Clear existing images if any

    //           ImageList24 = new ImageList();
    //           ImageList24.ColorDepth = ColorDepth.Depth32Bit;
    //           ImageList24.RightButtonSize = new Size(24, 24);
    //           ImageList24.Images.Clear(); // Clear existing images if any

    //           ImageList64 = new ImageList();
    //           ImageList64.ColorDepth = ColorDepth.Depth32Bit;
    //           ImageList64.RightButtonSize = new Size(64, 64);
    //           ImageList64.Images.Clear(); // Clear existing images if any

    //           ImageList128 = new ImageList();
    //           ImageList128.ColorDepth = ColorDepth.Depth32Bit;
    //           ImageList128.RightButtonSize = new Size(128, 128);
    //           ImageList128.Images.Clear(); // Clear existing images if any

    //       }
    //       public IDMEEditor DMEEditor { get; set; }
    //       public List<ImageConfiguration> ImgAssemblies { get; set; } = new List<ImageConfiguration>();
    //       public List<MenuList> Menus { get; set; } = new List<MenuList>();
    //       public List<string> Images { get; set; } = new List<string>();
    //       public IAppManager Vismanager { get; set; }
    //       public ImageList ImageList16 { get; set; }
    //       public ImageList ImageList32 { get; set; }
    //       public ImageList ImageList24 { get; set; }
    //       public ImageList ImageList64 { get; set; }
    //       public ImageList ImageList128 { get; set; }
    //       public List<Icon> Icons { get; set; } = new List<Icon>();
    //       public object LogoBigImage { get; set; }
    //       public object LogoSmallImage { get; set; }

    //       int index = -1; // Explicit index for the files
    //       public void FillImageList(List<ImageConfiguration> ls)
    //       {
    //           foreach (var file in ls)
    //           {
    //               Image image = null;
    //               if (file.AssemblyFullName != null && file.AssemblyLocation != null)
    //               {
    //                   Assembly assembly = Assembly.LoadFrom(file.AssemblyLocation);
    //                   using (Stream stream = assembly.GetManifestResourceStream(file.Path))
    //                   {
    //                       if (stream != null)
    //                       {
    //                           if (Path.GetExtension(file.Path).ToLower() == ".svg")
    //                           {
    //                               using (StreamReader reader = new StreamReader(stream))
    //                               {
    //                                   //string svgContent = reader.ReadToEnd();
    //                                   //var svgDocument = SvgDocument.FromSvg<Svg.SvgDocument>(svgContent);
    //                                   //image = svgDocument.Draw();

    //                               }
    //                           }
    //                           else
    //                           {
    //                               // Your existing code for non-SVG images
    //                               using (MemoryStream memoryStream = new MemoryStream())
    //                               {
    //                                   stream.CopyTo(memoryStream);
    //                                   memoryStream.Position = 0;
    //                                   image = Image.FromStream(memoryStream);
    //                               }
    //                           }
    //                       }
    //                   }
    //               }
    //               else
    //               {
    //                   // Read the file from the path
    //                   //image = ImagePath.FromFile(Path.Combine(file.Path, file.Name));
    //                   string fullPath = Path.Combine(file.Path, file.Name);

    //                   using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
    //                   {
    //                       using (MemoryStream memoryStream = new MemoryStream())
    //                       {
    //                           fileStream.CopyTo(memoryStream);
    //                           memoryStream.Position = 0;
    //                           image = Image.FromStream(memoryStream);
    //                       }
    //                   }
    //               }
    //               if (image != null)
    //               {
    //                   int sz = image.Width;
    //                   switch (sz)
    //                   {
    //                       case 16:
    //                           ImageList16.Images.Add(file.Name, image); // Add image to the ImageList
    //                           break;
    //                       case 24:
    //                           ImageList24.Images.Add(file.Name, image); // Add image to the ImageList
    //                           break;
    //                       case 32:
    //                           ImageList32.Images.Add(file.Name, image); // Add image to the ImageList
    //                           break;
    //                       case 64:
    //                           ImageList64.Images.Add(file.Name, image); // Add image to the ImageList
    //                           break;
    //                       case 128:
    //                           ImageList128.Images.Add(file.Name, image); // Add image to the ImageList
    //                           break;
    //                       default:
    //                           // ImageList32.Images.Add(file.Name, image); // Add image to the ImageList
    //                           break;
    //                   }
    //               }
    //           }
    //       }
    //       public List<string> GetImageNames()
    //       {
    //           return ImgAssemblies.Select(a => a.Name).ToList();
    //       }
    //       public void RefreshTreeView()
    //       {

    //       }
    //       public List<ImageConfiguration> GetGraphicFilesLocations(string path)
    //       {
    //           var result = new List<ImageConfiguration>();
    //           // Add extensions to look for
    //           string[] extensions = { ".png", ".ico" };
    //           if (string.IsNullOrEmpty(path))
    //           {
    //               return result;
    //           }
    //           if (Directory.Exists(path))
    //           {
    //               // Iterate through the files in the folder
    //               foreach (string file in Directory.GetFiles(path))
    //               {
    //                   string filename = Path.GetFileName(file);
    //                   string extension = Path.GetExtension(filename);
    //                   // Check if the file has one of the specified extensions
    //                   if (Array.Exists(extensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase)))
    //                   {
    //                       if (!ImgAssemblies.Any(ext => ext.Name.Equals(filename, StringComparison.OrdinalIgnoreCase)))
    //                       {
    //                           result.Add(new ImageConfiguration
    //                           {
    //                               Index = index++,
    //                               Name = filename,
    //                               Ext = extension,
    //                               Path = path
    //                           });
    //                           if (extension == ".ico")
    //                           {
    //                               using (Icon icon = new Icon(file))
    //                               {
    //                                   Icons.Add(icon);
    //                               }
    //                           }
    //                           if (!string.IsNullOrEmpty(Vismanager.LogoUrl))
    //                           {
    //                               if (file.Contains(Vismanager.LogoUrl))
    //                               {
    //                                   LogoBigImage = Image.FromFile(file);
    //                               }
    //                           }
    //                           if (!string.IsNullOrEmpty(Vismanager.IconUrl))
    //                           {
    //                               if (Vismanager.IconUrl.ToLower().Contains(filename.ToLower()))
    //                               {
    //                                   string iconPath = Vismanager.IconUrl;
    //                                   if (File.Exists(iconPath))
    //                                   {
    //                                       if (iconPath.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
    //                                       {

    //                                           // Load the icon directly from the file path
    //                                           using (Icon icon = new Icon(iconPath))
    //                                           {
    //                                               LogoSmallImage = (Icon)icon.Clone();
    //                                           }
    //                                       }
    //                                       else
    //                                       {
    //                                           // Load the image from the file path and convert it to an icon
    //                                           using (Image iconImage = Image.FromFile(iconPath))
    //                                           {
    //                                               using (Bitmap bitmap = new Bitmap(iconImage))
    //                                               {
    //                                                   using (Icon newIcon = Icon.FromHandle(bitmap.GetHicon()))
    //                                                   {
    //                                                       LogoSmallImage = (Icon)newIcon.Clone();
    //                                                   }
    //                                               }
    //                                           }
    //                                       }
    //                                   }
    //                                   else
    //                                   {
    //                                       // Handle file not found scenario
    //                                       Console.WriteLine($"File not found: {iconPath}");
    //                                   }
    //                               }

    //                           }
    //                       }
    //                   }
    //               }
    //           }
    //           if (result.Count > 0)
    //           {
    //               ImgAssemblies.AddRange(result);
    //               FillImageList(result);
    //           }

    //           return result;
    //       }
    //       public List<ImageConfiguration> GetGraphicFilesLocationsFromEmbedded(string[] namesspaces)
    //       {
    //           var result = new List<ImageConfiguration>();
    //           // Add extensions to look for
    //           string[] extensions = { ".png", ".ico" };
    //           // namesspaces= { "BeepEnterprize","Koc","DHUB","TheTechIdea","Beep" };
    //           // Get current, executing, and calling assemblies
    //           List<Assembly> assemblies = new Assembly[]{
    //               Assembly.GetExecutingAssembly(),
    //               Assembly.GetCallingAssembly(),
    //               Assembly.GetEntryAssembly()!       }.ToList();
    //           assemblies.AddRange(DMEEditor.ConfigEditor.LoadedAssemblies);
    //           List<Assembly> LoadedAssemblies = DependencyContext.Default.RuntimeLibraries
    //.SelectMany(library => library.GetDefaultAssemblyNames(DependencyContext.Default))
    //.Select(Assembly.Load)
    //.ToList();
    //           assemblies.AddRange(LoadedAssemblies);
    //           // Load all assemblies from the current domain to ensure referenced projects are included
    //           assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies()
    //               .Where(assembly => !assembly.FullName.StartsWith("System") && !assembly.FullName.StartsWith("Microsoft")));
    //           foreach (Assembly assembly in assemblies)
    //           {
    //               if (assembly.FullName.ToUpper().Contains("HALALLIB"))
    //               {
    //                   Debug.WriteLine(assembly.FullName);
    //               }
    //               // Get all embedded resources
    //               string[] resources = assembly.GetManifestResourceNames();

    //               foreach (string resource in resources)
    //               {
    //                   // Check if the resource name contains any of the specified namespaces
    //                   if (namesspaces != null)
    //                   {
    //                       if (!namesspaces.Any(ns => resource.Contains(ns, StringComparison.OrdinalIgnoreCase)))
    //                       {
    //                           continue; // Skip this resource as it doesn't match the namespace criteria
    //                       }

    //                   }

    //                   foreach (string extension in extensions)
    //                   {
    //                       if (resource.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
    //                       {
    //                           int lastDot = resource.LastIndexOf('.');
    //                           int secondToLastDot = resource.LastIndexOf('.', lastDot - 1);

    //                           string fileName = (resource.Substring(secondToLastDot + 1, lastDot - secondToLastDot - 1)).ToLower();

    //                           if (!ImgAssemblies.Any(ext => ext.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase)))
    //                           {
    //                               result.Add(new ImageConfiguration
    //                               {
    //                                   Index = index++,
    //                                   Name = fileName + extension,
    //                                   Ext = extension,
    //                                   Path = resource,
    //                                   AssemblyLocation = assembly.Location,
    //                                   AssemblyFullName = assembly.FullName
    //                               });
    //                               if (extension == ".ico")
    //                               {
    //                                   using (Stream stream = assembly.GetManifestResourceStream(resource))
    //                                   {
    //                                       if (stream != null)
    //                                       {
    //                                           Icons.Add(new Icon(stream));
    //                                       }
    //                                   }
    //                               }
    //                               // Check for LogoBigImage based on LogoUrl
    //                               if (!string.IsNullOrEmpty(Vismanager.LogoUrl) && Vismanager.LogoUrl.Contains(fileName, StringComparison.OrdinalIgnoreCase))
    //                               {
    //                                   using (Stream stream = assembly.GetManifestResourceStream(resource))
    //                                   {
    //                                       if (stream != null)
    //                                       {
    //                                           LogoBigImage = Image.FromStream(stream);
    //                                       }
    //                                   }
    //                               }

    //                               // Check for LogoSmallImage based on IconUrl
    //                               if (!string.IsNullOrEmpty(Vismanager.IconUrl) && Vismanager.IconUrl.Contains(fileName, StringComparison.OrdinalIgnoreCase))
    //                               {
    //                                   using (Stream stream = assembly.GetManifestResourceStream(resource))
    //                                   {
    //                                       if (stream != null)
    //                                       {
    //                                           LogoSmallImage = new Icon(stream);
    //                                       }
    //                                   }
    //                               }
    //                           }

    //                           break;
    //                       }
    //                   }
    //               }
    //           }

    //           ImgAssemblies.AddRange(result);
    //           FillImageList(result);
    //           return result;
    //       }
    //       private ImageList GetImageList()
    //       {

    //           return ImageList32;
    //       }
    //       public int GetImageIndexFromConnectioName(string Connectioname)
    //       {
    //           try
    //           {
    //               string drname = null;
    //               string iconname = null;
    //               ConnectionDriversConfig connectionDrivers;
    //               if (DMEEditor.ConfigEditor.DataConnections.Where(c => c.ConnectionName == Connectioname).Any())
    //               {
    //                   drname = DMEEditor.ConfigEditor.DataConnections.Where(c => c.ConnectionName == Connectioname).FirstOrDefault().DriverName;
    //               }

    //               if (drname != null)
    //               {
    //                   string drversion = DMEEditor.ConfigEditor.DataConnections.Where(c => c.ConnectionName == Connectioname).FirstOrDefault().DriverVersion;
    //                   if (DMEEditor.ConfigEditor.DataDriversClasses.Where(c => c.version == drversion && c.DriverClass == drname).Any())
    //                   {

    //                       connectionDrivers = DMEEditor.ConfigEditor.DataDriversClasses.Where(c => c.version == drversion && c.DriverClass == drname).FirstOrDefault();
    //                       if (connectionDrivers != null)
    //                       {
    //                           iconname = connectionDrivers.iconname;
    //                       }
    //                   }
    //                   else
    //                   {
    //                       connectionDrivers = DMEEditor.ConfigEditor.DataDriversClasses.Where(c => c.DriverClass == drname).FirstOrDefault();
    //                       if (connectionDrivers != null)
    //                       {
    //                           iconname = connectionDrivers.iconname;
    //                       }
    //                   }

    //                   int imgindx = GetImageIndex(iconname);

    //                   return imgindx;
    //               }
    //               else
    //                   return -1;
    //           }
    //           catch (Exception)
    //           {

    //               return -1;
    //           }

    //       }
    //       public int GetImageIndex(string pimagename)
    //       {
    //           if (pimagename == null)
    //           {
    //               return -1;
    //           }
    //           string imagename = pimagename.ToLower();
    //           if (string.IsNullOrEmpty(imagename))
    //           {
    //               return -1;
    //           }
    //           int imgindx = ImageList32.Images.IndexOfKey(imagename);

    //           if (imgindx == -1)
    //           {
    //               imgindx = ImageList32.Images.IndexOfKey(imagename);
    //           }
    //           if (imgindx == -1)
    //           {
    //               if (ImageList32.Images.ContainsKey(imagename))
    //               {
    //                   imgindx = ImageList32.Images.IndexOfKey(imagename);
    //               }
    //           }
    //           if (imgindx == -1)
    //           {
    //               ImageConfiguration img = ImgAssemblies.FirstOrDefault(p => p.Name.ToLower().Equals(imagename, StringComparison.InvariantCultureIgnoreCase));
    //               if (img != null)
    //               {
    //                   imgindx = img.Index;
    //               }

    //           }

    //           return imgindx;


    //       }
    //       public object GetImage(string pimagename)
    //       {
    //           try
    //           {
    //               string imagename = pimagename.ToLower();
    //               object img;
    //               int idx = GetImageIndex(imagename);
    //               if (idx > -1)
    //               {

    //                   img = ImageList32.Images[idx];
    //               }
    //               else
    //               {
    //                   ImageConfiguration cfg = ImgAssemblies.FirstOrDefault(ImgAssemblies => ImgAssemblies.Name.Equals(imagename, StringComparison.InvariantCultureIgnoreCase));
    //                   if (cfg != null)
    //                   {
    //                       img = GetImageFromFullName(LoadAssembly(cfg), cfg.Path);
    //                   }
    //                   else
    //                   {
    //                       img = null;
    //                   }

    //               }

    //               return img;
    //               // Tree.SelectedImageIndex = GetImageIndex("select.ico");
    //           }
    //           catch (Exception)
    //           {
    //               return null;
    //           }
    //       }
    //       private Assembly LoadAssembly(ImageConfiguration imageConfiguration)
    //       {
    //           Assembly assembly = null;
    //           if (imageConfiguration.AssemblyLocation != null)
    //           {
    //               assembly = Assembly.LoadFrom(imageConfiguration.AssemblyLocation);
    //           }
    //           else
    //           {
    //               assembly = Assembly.GetExecutingAssembly();
    //           }
    //           return assembly;
    //       }
    //       public object GetImage(string pimagename, int size)
    //       {
    //           try
    //           {
    //               object img;

    //               string imagename = pimagename.ToLower();
    //               if (size > 0)
    //               {

    //                   switch (size)
    //                   {
    //                       case 16:
    //                           img = ImageList16.Images[imagename];
    //                           break;
    //                       case 24:
    //                           img = ImageList24.Images[imagename];
    //                           break;
    //                       case 32:
    //                           img = ImageList32.Images[imagename];
    //                           break;
    //                       case 64:
    //                           img = ImageList64.Images[imagename];
    //                           break;
    //                       case 128:
    //                           img = ImageList128.Images[imagename];
    //                           break;
    //                       default:
    //                           img = ImageList32.Images[imagename];
    //                           break;
    //                   }
    //               }
    //               else
    //               {
    //                   ImageConfiguration cfg = ImgAssemblies.FirstOrDefault(ImgAssemblies => ImgAssemblies.Name.Equals(imagename, StringComparison.InvariantCultureIgnoreCase));
    //                   if (cfg != null)
    //                   {
    //                       img = GetImageFromFullName(LoadAssembly(cfg), cfg.Path);
    //                   }
    //                   else
    //                   {
    //                       img = null;
    //                   }

    //               }

    //               return img;
    //               // Tree.SelectedImageIndex = GetImageIndex("select.ico");
    //           }
    //           catch (Exception)
    //           {
    //               return null;
    //           }
    //       }
    //       public object GetImageFromIndex(int index)
    //       {
    //           if (index >= 0 && index < ImgAssemblies.Count)
    //           {
    //               var file = ImgAssemblies[index];

    //               if (file.AssemblyLocation != null)
    //               {
    //                   Assembly assembly = LoadAssembly(file);
    //                   // Get the resource stream from the assembly
    //                   using (Stream stream = assembly.GetManifestResourceStream(file.Path))
    //                   {
    //                       if (stream != null)
    //                       {
    //                           // Create and return the ImagePath from the stream
    //                           return Image.FromStream(stream);
    //                       }
    //                   }
    //               }
    //               else
    //               {
    //                   // Read the file from the path
    //                   using (Stream stream = File.OpenRead(Path.Combine(file.Path, file.Name)))
    //                   {
    //                       // Create and return the ImagePath from the stream
    //                       return Image.FromStream(stream);
    //                   }
    //               }
    //           }

    //           return null; // Return null if the index is out of range or the image is not found
    //       }
    //       public Image GetImageFromFullName(Assembly assembly, string fullName)
    //       {
    //           Bitmap image = null;
    //           Stream stream;

    //           // Is this just a single (ie. one-time) image?
    //           stream = assembly.GetManifestResourceStream(fullName);
    //           if (stream != null)
    //           {

    //               image = new Bitmap(stream);
    //               stream.Close();
    //               return image;
    //           }
    //           else
    //           {
    //               assembly = Assembly.GetCallingAssembly();
    //               stream = assembly.GetManifestResourceStream(fullName);
    //               if (stream != null)
    //               {
    //                   image = new Bitmap(stream);
    //                   stream.Close();
    //                   return image;
    //               }
    //           }
    //           return null;
    //       }
    //       public object GetImageFromName(string name)
    //       {
    //           if (name == null)
    //           {
    //               return null;
    //           }
    //           if (name.Length == 0)
    //           {
    //               return null;
    //           }
    //           ImageConfiguration imageConfiguration = null;//= ImgAssemblies.FirstOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    //           if (imageConfiguration == null)
    //           {
    //               imageConfiguration = ImgAssemblies.FirstOrDefault(p => p.Path.ToLower().Equals(name.ToLower(), StringComparison.InvariantCultureIgnoreCase));
    //           }
    //           if (imageConfiguration != null)
    //           {
    //               if (imageConfiguration.AssemblyFullName != null)
    //               {

    //                   return GetImageFromFullName(LoadAssembly(imageConfiguration), imageConfiguration.Path);
    //               }
    //               else
    //               {
    //                   return GetImageFromFile(imageConfiguration.Path);
    //               }
    //           }
    //           else
    //           {
    //               return null;
    //           }
    //           //foreach (var file in ImgAssemblies)
    //           //{
    //           //    if (file.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
    //           //    {
    //           //        if (file.Assembly != null)
    //           //        {
    //           //            // Get the resource stream from the assembly
    //           //            using (Stream stream = file.Assembly.GetManifestResourceStream(file.Path))
    //           //            {
    //           //                if (stream != null)
    //           //                {
    //           //                    // Create and return the ImagePath from the stream
    //           //                    return ImagePath.FromStream(stream);
    //           //                }
    //           //            }
    //           //        }
    //           //        else
    //           //        {
    //           //            // Read the file from the path
    //           //            using (Stream stream = File.OpenRead(Path.Combine(file.Path, file.Name)))
    //           //            {
    //           //                // Create and return the ImagePath from the stream
    //           //                return ImagePath.FromStream(stream);
    //           //            }
    //           //        }
    //           //    }
    //           //}

    //           // return null; // Return null if the name is not found
    //       }
    //       public Image GetImageFromFile(string fullname)
    //       {
    //           if (fullname == null)
    //           {
    //               return null;
    //           }
    //           if (fullname.Length == 0)
    //           {
    //               return null;
    //           }
    //           if (File.Exists(fullname) == false)
    //           {
    //               return null;
    //           }
    //           // Read the file from the path
    //           using (Stream stream = File.OpenRead(fullname))
    //           {
    //               // Create and return the ImagePath from the stream
    //               return Image.FromStream(stream);
    //           }
    //       }
    //       public Image GetImageFromFullName(string fullName)
    //       {
    //           Bitmap image = null;
    //           Stream stream;
    //           Assembly assembly = Assembly.GetExecutingAssembly();
    //           // Is this just a single (ie. one-time) image?
    //           stream = assembly.GetManifestResourceStream(fullName);
    //           if (stream != null)
    //           {

    //               image = new Bitmap(stream);
    //               stream.Close();
    //               return image;
    //           }
    //           else
    //           {
    //               assembly = Assembly.GetCallingAssembly();
    //               stream = assembly.GetManifestResourceStream(fullName);
    //               if (stream != null)
    //               {
    //                   image = new Bitmap(stream);
    //                   stream.Close();
    //                   return image;
    //               }
    //           }
    //           return null;
    //       }
    //       public List<string> GetImageList(Assembly assembly)
    //       {
    //           System.Globalization.CultureInfo culture = System.Threading.Thread.CurrentThread.CurrentCulture;
    //           string resourceName = assembly.GetName().Name;
    //           System.Resources.ResourceManager rm = new System.Resources.ResourceManager(resourceName, assembly);
    //           System.Resources.ResourceSet resourceSet = rm.GetResourceSet(culture, true, true);
    //           List<string> resources = new List<string>();
    //           foreach (DictionaryEntry resource in resourceSet)
    //           {
    //               resources.Add((string)resource.Key);
    //           }
    //           rm.ReleaseAllResources();
    //           return resources;
    //       }
    //       public object GetResource(string resource)
    //       {
    //           return null;
    //       }
    //       //private bool IsMethodApplicabletoNode(AssemblyClassDefinition cls, IBranch br)
    //       //{
    //       //    if (cls.classProperties == null)
    //       //    {
    //       //        return true;
    //       //    }
    //       //    if (cls.classProperties.ObjectType != null)
    //       //    {
    //       //        if (!cls.classProperties.ObjectType.Equals(br.ObjectType, StringComparison.InvariantCultureIgnoreCase))
    //       //        {
    //       //            return false;
    //       //        }
    //       //    }
    //       //    return true;


    //       //}

    //   }
}
