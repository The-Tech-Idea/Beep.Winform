using System;
using System.Collections;
using System.Resources;

using System.Xml.Linq;
using TheTechIdea.Beep.Vis.Modules;



namespace TheTechIdea.Beep.Desktop.Common
{
    /// <summary>
    /// Provides methods for copying image files into a project's resources folder,
    /// embedding them as resources (in .csproj), or updating .resx files.
    /// </summary>
    public static class ProjectResourceEmbedder
    {
        public static  Dictionary<string, ImageConfiguration> EmbeddedImages = new Dictionary<string, ImageConfiguration>();
        #region "Embed into .resx"
        /// <summary>
        /// Embeds an image into a .resx file by copying it to a local folder
        /// and updating or creating a resource entry in the .resx file.
        /// Also stores the result in _imageResources dictionary.
        /// </summary>
        public static void EmbedImageInResources(
            Dictionary<string, SimpleItem> _imageResources,
            string resxFile,
            string previewFilePath,
            string projectDirectory)
        {
            if (string.IsNullOrEmpty(previewFilePath))
            {
                MessageBox.Show("Please preview an image before embedding it.",
                                "No ImagePath Selected",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Copy the image into /Properties/Resources folder in the project
                string fileName = Path.GetFileNameWithoutExtension(previewFilePath);
                string resourcesPath = Path.Combine(projectDirectory, "Properties", "Resources");
                Directory.CreateDirectory(resourcesPath);

                string destPath = Path.Combine(resourcesPath, Path.GetFileName(previewFilePath));
                File.Copy(previewFilePath, destPath, true);

                // Load existing resources from the .resx file
                Dictionary<string, object> existingResources = new Dictionary<string, object>();
                using (ResXResourceReader resxReader = new ResXResourceReader(resxFile))
                {
                    foreach (DictionaryEntry entry in resxReader)
                    {
                        existingResources[entry.Key.ToString()] = entry.Value;
                    }
                }

                // Add or update the new resource in the dictionary (as a Bitmap)
                existingResources[fileName] = new Bitmap(destPath);

                // Write all resources back to the .resx
                using (ResXResourceWriter resxWriter = new ResXResourceWriter(resxFile))
                {
                    foreach (var entry in existingResources)
                    {
                        resxWriter.AddResource(entry.Key, entry.Value);
                    }
                }

                // Add to _imageResources dictionary for your in-app usage
                _imageResources[fileName] = new SimpleItem
                {
                    Name = fileName,
                    ImagePath = destPath
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error embedding image: {ex.Message}",
                                "Embedding Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        #endregion
        #region "Copy & Embed in .csproj"

        /// <summary>
        /// Copies a file to the project's /Resources folder and then
        /// calls EmbedFileAsEmbeddedResource to mark it in the .csproj.
        /// Also updates the _projectResources dictionary with a SimpleItem.
        /// </summary>
        public static string CopyFileToProjectResources(
            Dictionary<string, SimpleItem> _projectResources,
            string previewFilePath,
            string projectDirectory)
        {
            if (string.IsNullOrEmpty(previewFilePath))
            {
                MessageBox.Show("Please preview an image before embedding it.",
                                "No ImagePath Selected",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return null;
            }

            try
            {
                string resourcesFolder = Path.Combine(projectDirectory, "Resources");
                Directory.CreateDirectory(resourcesFolder);

                string destPath = Path.Combine(resourcesFolder, Path.GetFileName(previewFilePath));
                File.Copy(previewFilePath, destPath, true);

                // Now mark this file as an embedded resource in the .csproj
                EmbedFileAsEmbeddedResource(previewFilePath, destPath, projectDirectory);

                // Store in dictionary
                _projectResources[Path.GetFileNameWithoutExtension(previewFilePath)] =
                    new SimpleItem
                    {
                        Name = Path.GetFileNameWithoutExtension(previewFilePath),
                        ImagePath = destPath
                    };

                return destPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying file: {ex.Message}",
                                "Copy Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Marks 'filePath' as an EmbeddedResource in the .csproj 
        /// so it is compiled into the assembly.
        /// </summary>
        public static void EmbedFileAsEmbeddedResource(
            string filePath,
            string previewFilePath,
            string projectDirectory)
        {
            // Find the .csproj file
            string csprojFilePath = Directory.GetFiles(projectDirectory, "*.csproj").FirstOrDefault();
            if (csprojFilePath == null)
            {
                MessageBox.Show("Could not find the .csproj file.",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }

            // Convert to relative path for <EmbeddedResource Include="relativePath"/>
            string relativeFilePath = Path.GetRelativePath(projectDirectory, filePath).Replace("\\", "/");

            try
            {
                var xmlDocument = XDocument.Load(csprojFilePath);
                var itemGroup = xmlDocument.Descendants("ItemGroup")
                    .FirstOrDefault(ig => ig.Elements("EmbeddedResource").Any());

                if (itemGroup == null)
                {
                    itemGroup = new XElement("ItemGroup");
                    xmlDocument.Root.Add(itemGroup);
                }

                bool alreadyExists = itemGroup.Elements("EmbeddedResource")
                    .Any(er => er.Attribute("Include")?.Value == relativeFilePath);

                if (!alreadyExists)
                {
                    itemGroup.Add(new XElement("EmbeddedResource", new XAttribute("Include", relativeFilePath)));
                    xmlDocument.Save(csprojFilePath);

                    MessageBox.Show("File marked as embedded resource in .csproj successfully. " +
                                    "Please reload the project in Visual Studio to apply changes.",
                                    "Embedding Success",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("The file is already embedded as a resource in the project.",
                                    "Already Embedded",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error embedding file as resource: {ex.Message}",
                                "Embedding Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Combines the copying and embedding steps into a single call. 
        ///  1) Copy to /Resources
        ///  2) Mark .csproj as <EmbeddedResource Include=...>
        ///  3) Add to projectResources dictionary
        /// </summary>
        public static string CopyAndEmbedFileToProjectResources(
            Dictionary<string, SimpleItem> projectResources,
            string previewFilePath,
            string projectDirectory)
        {
            if (string.IsNullOrEmpty(previewFilePath))
            {
                MessageBox.Show("Please preview an image before embedding it.",
                                "No ImagePath Selected",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return null;
            }

            try
            {
                // 1. Copy file into /Resources
                string resourcesFolder = Path.Combine(projectDirectory, "Resources");
                Directory.CreateDirectory(resourcesFolder);

                string fileName = Path.GetFileName(previewFilePath);
                string destPath = Path.Combine(resourcesFolder, fileName);
                File.Copy(previewFilePath, destPath, true);

                // 2. Now embed it as an embedded resource in the .csproj file
                string csprojFilePath = Directory.GetFiles(projectDirectory, "*.csproj").FirstOrDefault();
                if (csprojFilePath == null)
                {
                    MessageBox.Show("Could not find the .csproj file.",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return null;
                }

                string relativeFilePath = Path.GetRelativePath(projectDirectory, destPath).Replace("\\", "/");
                var xmlDocument = XDocument.Load(csprojFilePath);
                var itemGroup = xmlDocument.Descendants("ItemGroup")
                    .FirstOrDefault(ig => ig.Elements("EmbeddedResource").Any());

                if (itemGroup == null)
                {
                    itemGroup = new XElement("ItemGroup");
                    xmlDocument.Root.Add(itemGroup);
                }

                bool alreadyExists = itemGroup.Elements("EmbeddedResource")
                    .Any(er => er.Attribute("Include")?.Value == relativeFilePath);

                if (!alreadyExists)
                {
                    itemGroup.Add(new XElement("EmbeddedResource", new XAttribute("Include", relativeFilePath)));
                    xmlDocument.Save(csprojFilePath);

                    MessageBox.Show("File marked as embedded resource in .csproj successfully. " +
                                    "Please reload the project in Visual Studio to apply changes.",
                                    "Embedding Success",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("The file is already embedded as a resource in the project.",
                                    "Already Embedded",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }

                // 3. Add to dictionary
                var simpleMenuItem = new SimpleItem
                {
                    Name = Path.GetFileNameWithoutExtension(fileName),
                    ImagePath = destPath
                };
                projectResources[Path.GetFileNameWithoutExtension(fileName)] = simpleMenuItem;

                return destPath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying and embedding file: {ex.Message}",
                                "Copy and Embed Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Copies a file to the specified folder inside the current app domain base directory.
        /// Then updates _localImages with the new location.
        /// </summary>
        public static void MoveFileToProjectResources(
            Dictionary<string, SimpleItem> _localImages,
            string sourceFilePath,
            string destinationFolder)
        {
            try
            {
                string projectResourceFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, destinationFolder);
                Directory.CreateDirectory(projectResourceFolder);

                string fileName = Path.GetFileName(sourceFilePath);
                string destinationPath = Path.Combine(projectResourceFolder, fileName);

                File.Copy(sourceFilePath, destinationPath, true);

                _localImages[fileName] = new SimpleItem
                {
                    Name = fileName,
                    ImagePath = destinationPath
                };

                MessageBox.Show($"File moved to project resource folder: {destinationPath}",
                                "File Moved",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error moving file: {ex.Message}",
                                "Move Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}