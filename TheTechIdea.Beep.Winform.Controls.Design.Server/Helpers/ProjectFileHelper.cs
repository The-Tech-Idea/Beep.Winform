using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server
{
    /// <summary>
    /// Assistant class for managing .csproj file operations and embedded resources
    /// </summary>
    public static class ProjectFileHelper
    {
        /// <summary>
        /// Asynchronously adds a file as an embedded resource to the .csproj file
        /// </summary>
        public static async Task<ResourceOperationResult> AddEmbeddedResourceAsync(
            string filePath,
            string csprojPath,
            string projectDirectory)
        {
            var result = new ResourceOperationResult();

            try
            {
                if (!File.Exists(csprojPath))
                {
                    result.AddError($".csproj file not found: {csprojPath}");
                    return result;
                }

                if (!File.Exists(filePath))
                {
                    result.AddError($"File not found: {filePath}");
                    return result;
                }

                await Task.Run(() =>
                {
                    var relativeFilePath = Path.GetRelativePath(projectDirectory, filePath).Replace("\\", "/");
                    var xmlDocument = XDocument.Load(csprojPath);

                    var itemGroup = xmlDocument.Descendants("ItemGroup")
                        .FirstOrDefault(ig => ig.Elements("EmbeddedResource").Any())
                        ?? new XElement("ItemGroup");

                    if (itemGroup.Parent == null)
                    {
                        xmlDocument.Root?.Add(itemGroup);
                    }

                    var alreadyExists = itemGroup.Elements("EmbeddedResource")
                        .Any(er => er.Attribute("Include")?.Value == relativeFilePath);

                    if (!alreadyExists)
                    {
                        itemGroup.Add(new XElement("EmbeddedResource", new XAttribute("Include", relativeFilePath)));
                        xmlDocument.Save(csprojPath);
                        
                        result.IsSuccess = true;
                        result.ResourceName = Path.GetFileNameWithoutExtension(filePath);
                        result.FilePath = filePath;
                        result.Message = "File successfully marked as embedded resource";
                    }
                    else
                    {
                        result.IsSuccess = true;
                        result.ResourceName = Path.GetFileNameWithoutExtension(filePath);
                        result.FilePath = filePath;
                        result.Message = "File is already embedded as resource";
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                result.AddError($"Failed to embed file as resource: {ex.Message}");
                result.Exception = ex;
                return result;
            }
        }

        /// <summary>
        /// Asynchronously removes an embedded resource from the .csproj file
        /// </summary>
        public static async Task<ResourceOperationResult> RemoveEmbeddedResourceAsync(
            string resourcePath,
            string csprojPath,
            string projectDirectory,
            bool deletePhysicalFile = true)
        {
            var result = new ResourceOperationResult();

            try
            {
                if (!File.Exists(csprojPath))
                {
                    result.AddError($".csproj file not found: {csprojPath}");
                    return result;
                }

                await Task.Run(() =>
                {
                    var xmlDocument = XDocument.Load(csprojPath);
                    var resourceElement = xmlDocument.Descendants("EmbeddedResource")
                        .FirstOrDefault(er => er.Attribute("Include")?.Value == resourcePath);

                    if (resourceElement != null)
                    {
                        resourceElement.Remove();
                        xmlDocument.Save(csprojPath);
                        
                        // Also remove physical file if requested
                        if (deletePhysicalFile)
                        {
                            var fullPath = Path.Combine(projectDirectory, resourcePath.Replace("/", "\\"));
                            if (File.Exists(fullPath))
                            {
                                File.Delete(fullPath);
                            }
                        }

                        result.IsSuccess = true;
                        result.ResourceName = Path.GetFileNameWithoutExtension(resourcePath);
                        result.FilePath = resourcePath;
                        result.Message = "Resource successfully removed";
                    }
                    else
                    {
                        result.AddError("Resource not found in project file");
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                result.AddError($"Failed to remove resource: {ex.Message}");
                result.Exception = ex;
                return result;
            }
        }

        /// <summary>
        /// Gets all embedded resources from the .csproj file
        /// </summary>
        public static async Task<List<string>> GetEmbeddedResourcesAsync(string csprojPath)
        {
            if (!File.Exists(csprojPath))
                return new List<string>();

            return await Task.Run(() =>
            {
                try
                {
                    var xmlDocument = XDocument.Load(csprojPath);
                    return xmlDocument.Descendants("EmbeddedResource")
                        .Select(er => er.Attribute("Include")?.Value)
                        .Where(path => !string.IsNullOrEmpty(path))
                        .ToList();
                }
                catch
                {
                    return new List<string>();
                }
            });
        }

        /// <summary>
        /// Checks if a file is already embedded as a resource
        /// </summary>
        public static async Task<bool> IsEmbeddedResourceAsync(string filePath, string csprojPath, string projectDirectory)
        {
            if (!File.Exists(csprojPath))
                return false;

            var relativeFilePath = Path.GetRelativePath(projectDirectory, filePath).Replace("\\", "/");
            var embeddedResources = await GetEmbeddedResourcesAsync(csprojPath);
            
            return embeddedResources.Contains(relativeFilePath, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates a backup of the .csproj file before modifications
        /// </summary>
        public static async Task<string> CreateBackupAsync(string csprojPath)
        {
            if (!File.Exists(csprojPath))
                throw new FileNotFoundException($".csproj file not found: {csprojPath}");

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var backupFileName = $"{Path.GetFileNameWithoutExtension(csprojPath)}_backup_{timestamp}.csproj";
            var backupPath = Path.Combine(Path.GetDirectoryName(csprojPath), backupFileName);

            await Task.Run(() => File.Copy(csprojPath, backupPath, false));
            
            return backupPath;
        }

        /// <summary>
        /// Updates resource dictionaries with embedded resource information
        /// </summary>
        public static async Task LoadEmbeddedResourcesToDictionaryAsync(
            Dictionary<string, SimpleItem> resourceDictionary,
            string projectDirectory)
        {
            resourceDictionary.Clear();

            var validation = ResourceValidationHelper.ValidateProjectDirectory(projectDirectory);
            if (!validation.IsValid)
                return;

            try
            {
                var embeddedResources = await GetEmbeddedResourcesAsync(validation.CsprojPath);
                
                var imageResources = embeddedResources.Where(path =>
                {
                    var extension = Path.GetExtension(path).ToLowerInvariant();
                    return ResourceValidationHelper.SupportedImageExtensions.Contains(extension);
                });

                foreach (var resourcePath in imageResources)
                {
                    var resourceName = Path.GetFileNameWithoutExtension(resourcePath);
                    
                    resourceDictionary[resourceName] = new SimpleItem
                    {
                        Name = resourceName,
                        ImagePath = resourcePath,
                        GuidId = Guid.NewGuid().ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load embedded resources: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets project properties like target framework, output type, etc.
        /// </summary>
        public static async Task<Dictionary<string, string>> GetProjectPropertiesAsync(string csprojPath)
        {
            var properties = new Dictionary<string, string>();

            if (!File.Exists(csprojPath))
                return properties;

            return await Task.Run(() =>
            {
                try
                {
                    var xmlDocument = XDocument.Load(csprojPath);
                    
                    // Get common properties from PropertyGroup
                    var propertyGroups = xmlDocument.Descendants("PropertyGroup");
                    
                    foreach (var group in propertyGroups)
                    {
                        foreach (var element in group.Elements())
                        {
                            if (!properties.ContainsKey(element.Name.LocalName))
                            {
                                properties[element.Name.LocalName] = element.Value;
                            }
                        }
                    }

                    return properties;
                }
                catch
                {
                    return new Dictionary<string, string>();
                }
            });
        }
    }
}