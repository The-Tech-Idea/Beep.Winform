using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Editor;
using System.Collections;
using System.Reflection;
using System.Drawing;
using System.IO;
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;
using TheTechIdea.Beep.DriversConfigurations;


namespace TheTechIdea.Beep.Vis.Logic
{
 //   public class VisHelper : IVisHelper
 //   {
 //       public VisHelper()
 //       {
 //               init();
 //       }
 //       public VisHelper(IDMEEditor pDMEEditor, IVisManager pVismanager)
 //       {
 //           DMEEditor = pDMEEditor;
 //           Vismanager = pVismanager;
 //           init();
 //       }
 //       private void init()
 //       {
 //           ImageList16 = new Dictionary<string, Image>();
 //           ImageList32 = new Dictionary<string, Image>();
 //           ImageList24 = new Dictionary<string, Image>();
 //           ImageList64 = new Dictionary<string, Image>();
 //           ImageList128 = new Dictionary<string, Image>();
 //       }
 //       public IDMEEditor DMEEditor { get; set; }
 //       public List<ImageConfiguration> ImgAssemblies { get; set; } = new List<ImageConfiguration>();
 //       public List<MenuList> Menus { get; set; } = new List<MenuList>();
 //       public List<string> Images { get; set; } = new List<string>();
 //       public IVisManager Vismanager { get; set; }
 //       public Dictionary<string,Image> ImageList16 { get; set; }
 //       public Dictionary<string, Image> ImageList32 { get; set; }
 //       public Dictionary<string, Image> ImageList24 { get; set; }
 //       public Dictionary<string, Image> ImageList64 { get; set; }
 //       public Dictionary<string, Image> ImageList128 { get; set; }
 //       public object LogoBigImage { get; set; }
 //       public object LogoSmallImage { get; set; }
 //       public List<Icon> Icons { get; set; } = new List<Icon>();
 //       int index = -1; // Explicit index for the files
 //       public void FillImageList(List<ImageConfiguration> ls)
 //       {
 //           //foreach (var file in ls)
 //           //{
 //           //    Image image = null;
 //           //    if (file.Assembly != null)
 //           //    {
 //           //        using (Stream stream = file.Assembly.GetManifestResourceStream(file.Path))
 //           //        {
 //           //            if (stream != null)
 //           //            {
 //           //                if (Path.GetExtension(file.Path).ToLower() == ".svg")
 //           //                {
 //           //                    using (StreamReader reader = new StreamReader(stream))
 //           //                    {
 //           //                        //string svgContent = reader.ReadToEnd();
 //           //                        //var svgDocument = SvgDocument.FromSvg<Svg.SvgDocument>(svgContent);
 //           //                        //image = svgDocument.Draw();

 //           //                    }
 //           //                }
 //           //                else
 //           //                {
 //           //                    // Your existing code for non-SVG images
 //           //                    using (MemoryStream memoryStream = new MemoryStream())
 //           //                    {
 //           //                        stream.CopyTo(memoryStream);
 //           //                        memoryStream.Position = 0;
 //           //                        image = Image.FromStream(memoryStream);
 //           //                    }
 //           //                }
 //           //            }
 //           //        }
 //           //    }
 //           //    else
 //           //    {
 //           //        // Read the file from the path
 //           //        //image = Image.FromFile(Path.Combine(file.Path, file.Name));
 //           //        string fullPath = Path.Combine(file.Path, file.Name);

 //           //        using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
 //           //        {
 //           //            using (MemoryStream memoryStream = new MemoryStream())
 //           //            {
 //           //                fileStream.CopyTo(memoryStream);
 //           //                memoryStream.Position = 0;
 //           //                image = Image.FromStream(memoryStream);
 //           //            }
 //           //        }
 //           //    }
 //           //    if (image != null)
 //           //    {
 //           //        int sz = image.Width;
 //           //        switch (sz)
 //           //        {
 //           //            case 16:
 //           //                if (ImageList16.ContainsKey(file.Name) == false)
 //           //                {
 //           //                    ImageList16.Add(file.Name, image); // Add image to the ImageList
 //           //                }
                           
 //           //                break;
 //           //            case 24:
 //           //                if (ImageList24.ContainsKey(file.Name) == false)
 //           //                {
 //           //                    ImageList24.Add(file.Name, image); // Add image to the ImageList
 //           //                }
                         
 //           //                break;
 //           //            case 32:
 //           //                if (ImageList32.ContainsKey(file.Name) == false)
 //           //                {
 //           //                    ImageList32.Add(file.Name, image); // Add image to the ImageList
 //           //                }
                         
 //           //                break;
 //           //            case 64:
 //           //                if (ImageList64.ContainsKey(file.Name) == false)
 //           //                {
 //           //                    ImageList64.Add(file.Name, image); // Add image to the ImageList
 //           //                }
                        
 //           //                break;
 //           //            case 128:
 //           //                if (ImageList128.ContainsKey(file.Name) == false)
 //           //                {
 //           //                    ImageList128.Add(file.Name, image); // Add image to the ImageList
 //           //                }
                          
 //           //                break;
 //           //            default:
 //           //               // ImageList32.Images.Add(file.Name, image); // Add image to the ImageList
 //           //                break;
 //           //        }
 //           //    }
 //           //}
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
 //                                   Assembly = assembly
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
 //           string imagename = pimagename.ToLower();
 //           if (string.IsNullOrEmpty(imagename))
 //           {
 //               return -1;
 //           }
 //           int imgindx = -1;
        
            
 //               ImageConfiguration img = ImgAssemblies.FirstOrDefault(p => p.Name.ToLower().Equals(imagename, StringComparison.InvariantCultureIgnoreCase));
 //           var imglist16 = ImageList16.Keys.ToList();
 //           var imglist32 = ImageList32.Keys.ToList();
 //           var imglist64 = ImageList64.Keys.ToList();
 //           var imglist128 = ImageList128.Keys.ToList();
 //           if(imglist16.Count>0)
 //           {
 //               imgindx= imglist16.IndexOf(imagename);
 //           }
 //           if (imglist32.Count > 0 && imgindx==-1)
 //           {
 //               imgindx = imglist32.IndexOf(imagename);
 //           }
 //           if (imglist64.Count > 0 && imgindx == -1)
 //           {
 //               imgindx = imglist64.IndexOf(imagename);
 //           }
 //           if (imglist128.Count > 0 && imgindx == -1)
 //           {
 //               imgindx = imglist128.IndexOf(imagename);
 //           }
 //           if (img != null)
 //               {
 //                   imgindx =img.Index;
 //               }
                
           
         
 //           return imgindx;


 //       }
 //       public object GetImage(string pimagename)
 //       {
 //           try
 //           {
 //               return GetImageBase64(pimagename);
 //               //string imagename = pimagename.ToLower();
 //               //object img;
 //               ////  int idx = GetImageIndex(imagename);
 //               //img = ImageList32.FirstOrDefault(p => p.Key == imagename).Value;
 //               //if (img==null)
 //               //{
 //               //    ImageConfiguration cfg = ImgAssemblies.FirstOrDefault(ImgAssemblies => ImgAssemblies.Name.Equals(imagename, StringComparison.InvariantCultureIgnoreCase));
 //               //    if (cfg != null)
 //               //    {
 //               //        img = GetImageFromFullName(cfg.Assembly, cfg.Path);
 //               //    }
 //               //    else
 //               //    {
 //               //        img = null;
 //               //    }

 //               //}

 //               //return img;
 //               // Tree.SelectedImageIndex = GetImageIndex("select.ico");
 //           }
 //           catch (Exception)
 //           {
 //               return null;
 //           }
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
 //                           img = ImageList16[imagename];
 //                           break;
 //                       case 24:
 //                           img = ImageList24[imagename];
 //                           break;
 //                       case 32:
 //                           img = ImageList32[imagename];
 //                           break;
 //                       case 64:
 //                           img = ImageList64[imagename];
 //                           break;
 //                       case 128:
 //                           img = ImageList128[imagename];
 //                           break;
 //                       default:
 //                           img = ImageList32[imagename];
 //                           break;
 //                   }
 //               }
 //               else
 //               {
 //                   ImageConfiguration cfg = ImgAssemblies.FirstOrDefault(ImgAssemblies => ImgAssemblies.Name.Equals(imagename, StringComparison.InvariantCultureIgnoreCase));
 //                   if (cfg != null)
 //                   {
 //                       img = ImageListHelper. GetImageFromFullName(cfg.Assembly, cfg.Path);
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

 //               if (file.Assembly != null)
 //               {
 //                   // Get the resource stream from the assembly
 //                   using (Stream stream = file.Assembly.GetManifestResourceStream(file.Path))
 //                   {
 //                       if (stream != null)
 //                       {
 //                           // Create and return the Image from the stream
 //                           return Image.FromStream(stream);
 //                       }
 //                   }
 //               }
 //               else
 //               {
 //                   // Read the file from the path
 //                   using (Stream stream = File.OpenRead(Path.Combine(file.Path, file.Name)))
 //                   {
 //                       // Create and return the Image from the stream
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
 //           ImageConfiguration imageConfiguration=null;//= ImgAssemblies.FirstOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
 //           if(imageConfiguration==null)
 //           {
 //               imageConfiguration = ImgAssemblies.FirstOrDefault(p => p.Path.ToLower().Equals(name.ToLower(), StringComparison.InvariantCultureIgnoreCase));
 //           }
 //           if (imageConfiguration != null)
 //           {
 //               if (imageConfiguration.Assembly != null)
 //               {
 //                   return GetImageFromFullName(imageConfiguration.Assembly, imageConfiguration.Path);
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
 //           //                    // Create and return the Image from the stream
 //           //                    return Image.FromStream(stream);
 //           //                }
 //           //            }
 //           //        }
 //           //        else
 //           //        {
 //           //            // Read the file from the path
 //           //            using (Stream stream = File.OpenRead(Path.Combine(file.Path, file.Name)))
 //           //            {
 //           //                // Create and return the Image from the stream
 //           //                return Image.FromStream(stream);
 //           //            }
 //           //        }
 //           //    }
 //           //}

 //          // return null; // Return null if the name is not found
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
 //           if(File.Exists(fullname)==false)
 //           {
 //               return null;
 //           }
 //           // Read the file from the path
 //           using (Stream stream = File.OpenRead(fullname))
 //           {
 //               // Create and return the Image from the stream
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
 //       public string GetImageBase64(string imageName)
 //       {
 //           try
 //           {
 //               var imageObj = ImageList32.FirstOrDefault(p => p.Key.Equals(imageName, StringComparison.OrdinalIgnoreCase)).Value;
 //               if (imageObj == null)
 //               {
 //                   var cfg = ImgAssemblies.FirstOrDefault(a => a.Name.Equals(imageName, StringComparison.OrdinalIgnoreCase));
 //                   if (cfg != null)
 //                   {
 //                       imageObj = GetImageFromFullName(cfg.Assembly, cfg.Path);
 //                   }
 //               }

 //               if (imageObj is System.Drawing.Image image)
 //               {
 //                   using (var ms = new MemoryStream())
 //                   {
 //                       // Save the image to the stream, specifying the format
 //                       image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

 //                       // Convert the stream's byte array to a Base64 string
 //                       var base64 = Convert.ToBase64String(ms.ToArray());
 //                       return $"data:image/png;base64,{base64}";
 //                   }
 //               }
 //               return null;
 //           }
 //           catch (Exception)
 //           {
 //               return null;
 //           }
 //       }

 //       public object GetResource(string resource)
 //       {
 //           throw new NotImplementedException();
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
