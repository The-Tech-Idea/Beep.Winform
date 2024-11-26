using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Svg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using System.Xml.Linq;
using Svg;


namespace TheTechIdea.Beep.Winform.Controls.Design.Helper
{
    public static class DesignResourceHelper
    {

        public static List<string> ResourcesImages = new List<string>();
        // populate ImageList from Resrources.resx
        public static void PopulateImageListFromResx(ImageList imageList)
        {
            imageList.Images.Clear(); // Clear existing images
            ResourcesImages = GetImageResourcesFromResx(); // Get resources

            foreach (string resourceName in ResourcesImages)
            {
                try
                {
                    object resourceValue = GetResource(resourceName);
                    if (resourceValue is Bitmap bitmap)
                    {
                        // Add bitmap to the ImageList
                        imageList.Images.Add(resourceName, bitmap);
                    }
                    else if (resourceValue is string svgPath && svgPath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
                    {
                        // Convert SVG to Bitmap and add it to the ImageList
                        var svgBitmap = GetBitmapFromSvg(svgPath);
                        if (svgBitmap != null)
                        {
                            imageList.Images.Add(resourceName, svgBitmap);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading image resource '{resourceName}': {ex.Message}");
                }
            }
        }


        /// <summary>
        /// Reads all image resources (bitmaps and SVGs) from the user's Resources.resx file.
        /// </summary>
        public static List<string> GetImageResourcesFromResx()
        {
            List<string> resourceNames = new List<string>();

            try
            {
                string resxFilePath = GetResxFilePath();
                if (!File.Exists(resxFilePath))
                {
                    throw new FileNotFoundException("No Resources.resx file found in the user's project.");
                }

                using (var resxReader = new ResXResourceReader(resxFilePath))
                {
                    foreach (DictionaryEntry entry in resxReader)
                    {
                        if (entry.Value is Bitmap || (entry.Value is string svgPath && svgPath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)))
                        {
                            resourceNames.Add(entry.Key.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading image resources: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return resourceNames;
        }

        /// <summary>
        /// Adds or updates a resource in the user's Resources.resx file.
        /// </summary>
        public static void AddOrUpdateResource(string resourceName, object resourceValue)
        {
            try
            {
                string resxFilePath = GetResxFilePath();

                Dictionary<string, object> resources = new Dictionary<string, object>();
                using (var resxReader = new ResXResourceReader(resxFilePath))
                {
                    foreach (DictionaryEntry entry in resxReader)
                    {
                        resources[entry.Key.ToString()] = entry.Value;
                    }
                }

                resources[resourceName] = resourceValue;

                using (var resxWriter = new ResXResourceWriter(resxFilePath))
                {
                    foreach (var entry in resources)
                    {
                        resxWriter.AddResource(entry.Key, entry.Value);
                    }
                }

                MessageBox.Show($"Resource '{resourceName}' has been successfully added to Resources.resx.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding or updating resource: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// Retrieves the path to the Resources.resx file in the user's project.
        /// </summary>
        private static string GetResxFilePath()
        {
            string projectPath = ProjectPathHelper.GetProjectPath();
            return Path.Combine(projectPath, "Properties", "Resources.resx");
        }

        /// <summary>
        /// Copies an image file to the user's Resources folder and returns the destination path.
        /// </summary>
        public static string CopyFileToResources(string filePath)
        {
            try
            {
                string projectPath = ProjectPathHelper.GetProjectPath();
                string resourcesPath = Path.Combine(projectPath, "Resources");
                Directory.CreateDirectory(resourcesPath); // Ensure the Resources folder exists

                string destPath = Path.Combine(resourcesPath, Path.GetFileName(filePath));
                File.Copy(filePath, destPath, true); // Overwrite if exists

                MessageBox.Show($"File copied to project resources folder: {destPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return destPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying file to resources: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Embeds an image file as an EmbeddedResource in the user's project file (.csproj).
        /// </summary>
        public static void EmbedFileAsResource(string filePath)
        {
            try
            {
                string projectPath = ProjectPathHelper.GetProjectPath();
                string csprojFilePath = Directory.GetFiles(projectPath, "*.csproj").FirstOrDefault();

                if (csprojFilePath == null)
                {
                    throw new FileNotFoundException("No .csproj file found in the user's project.");
                }

                // Get the relative path to the file
                string relativeFilePath = Path.GetRelativePath(projectPath, filePath).Replace("\\", "/");

                // Load the .csproj file
                var xmlDocument = XDocument.Load(csprojFilePath);

                // Find or create the <ItemGroup> for EmbeddedResource
                var itemGroup = xmlDocument.Descendants("ItemGroup")
                    .FirstOrDefault(ig => ig.Elements("EmbeddedResource").Any()) ?? new XElement("ItemGroup");

                // Check if the file is already included
                bool alreadyExists = itemGroup.Elements("EmbeddedResource")
                    .Any(er => er.Attribute("Include")?.Value == relativeFilePath);

                if (!alreadyExists)
                {
                    itemGroup.Add(new XElement("EmbeddedResource", new XAttribute("Include", relativeFilePath)));
                    xmlDocument.Root.Add(itemGroup);
                    xmlDocument.Save(csprojFilePath);

                    MessageBox.Show($"File '{filePath}' embedded successfully as a resource. Reload the project to apply changes.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"File '{filePath}' is already embedded as a resource.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error embedding file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #region "Support Methods"
    


        public static Bitmap GetBitmapFromSvg(string svgPath)
        {
            if (string.IsNullOrEmpty(svgPath) || !File.Exists(svgPath))
            {
                return null;
            }

            try
            {
                var svgDoc = SvgDocument.Open(svgPath);
                return svgDoc.Draw(); // Render the SVG as a Bitmap
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error converting SVG to Bitmap: {ex.Message}");
                return null;
            }
        }


        public static object GetResource(string resourceName)
        {
            try
            {
                string resxFilePath = GetResxFilePath();
                if (!File.Exists(resxFilePath))
                {
                    throw new FileNotFoundException("Resources.resx file not found.");
                }

                using (var resxReader = new ResXResourceReader(resxFilePath))
                {
                    foreach (DictionaryEntry entry in resxReader)
                    {
                        if (entry.Key.ToString() == resourceName)
                        {
                            return entry.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching resource '{resourceName}': {ex.Message}");
            }

            return null;
        }

        #endregion "Support Methods"

    }
}
