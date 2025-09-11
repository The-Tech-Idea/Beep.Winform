using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Models;
using TheTechIdea.Beep.Vis.Modules;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server
{
    /// <summary>
    /// Assistant class for managing .resx file operations
    /// </summary>
    public static class ResxResourceHelper
    {
        /// <summary>
        /// Asynchronously loads resources from a .resx file
        /// </summary>
        public static async Task<Dictionary<string, object>> LoadResxResourcesAsync(string resxFile)
        {
            return await Task.Run(() => LoadResxResources(resxFile));
        }

        /// <summary>
        /// Synchronously loads resources from a .resx file
        /// </summary>
        public static Dictionary<string, object> LoadResxResources(string resxFile)
        {
            var resources = new Dictionary<string, object>();
            
            if (!File.Exists(resxFile))
                return resources;

            try
            {
                using (var reader = new ResXResourceReader(resxFile))
                {
                    foreach (DictionaryEntry entry in reader)
                    {
                        resources[entry.Key.ToString()] = entry.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load .resx file '{resxFile}': {ex.Message}", ex);
            }

            return resources;
        }

        /// <summary>
        /// Asynchronously saves resources to a .resx file
        /// </summary>
        public static async Task SaveResxResourcesAsync(string resxFile, Dictionary<string, object> resources)
        {
            await Task.Run(() => SaveResxResources(resxFile, resources));
        }

        /// <summary>
        /// Synchronously saves resources to a .resx file
        /// </summary>
        public static void SaveResxResources(string resxFile, Dictionary<string, object> resources)
        {
            try
            {
                // Ensure directory exists
                var directory = Path.GetDirectoryName(resxFile);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var writer = new ResXResourceWriter(resxFile))
                {
                    foreach (var entry in resources)
                    {
                        writer.AddResource(entry.Key, entry.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save .resx file '{resxFile}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Adds or updates a resource in a .resx file
        /// </summary>
        public static async Task<ResourceOperationResult> AddOrUpdateResourceAsync(
            string resxFile,
            string resourceName,
            object resourceValue,
            Dictionary<string, SimpleItem> resourceDictionary = null)
        {
            var result = new ResourceOperationResult();

            try
            {
                // Load existing resources
                var existingResources = await LoadResxResourcesAsync(resxFile);
                
                // Add or update the resource
                existingResources[resourceName] = resourceValue;
                
                // Save back to file
                await SaveResxResourcesAsync(resxFile, existingResources);

                // Update dictionary if provided
                if (resourceDictionary != null)
                {
                    resourceDictionary[resourceName] = new SimpleItem
                    {
                        Name = resourceName,
                        ImagePath = resxFile,
                        GuidId = Guid.NewGuid().ToString()
                    };
                }

                result.IsSuccess = true;
                result.ResourceName = resourceName;
                result.FilePath = resxFile;
                result.Message = $"Resource '{resourceName}' successfully added to .resx file";

                return result;
            }
            catch (Exception ex)
            {
                result.AddError($"Failed to add resource to .resx: {ex.Message}");
                result.Exception = ex;
                return result;
            }
        }

        /// <summary>
        /// Removes a resource from a .resx file
        /// </summary>
        public static async Task<ResourceOperationResult> RemoveResourceAsync(
            string resxFile,
            string resourceName,
            Dictionary<string, SimpleItem> resourceDictionary = null)
        {
            var result = new ResourceOperationResult();

            try
            {
                if (!File.Exists(resxFile))
                {
                    result.AddError($".resx file not found: {resxFile}");
                    return result;
                }

                // Load existing resources
                var existingResources = await LoadResxResourcesAsync(resxFile);
                
                // Remove the resource if it exists
                if (existingResources.Remove(resourceName))
                {
                    // Save back to file
                    await SaveResxResourcesAsync(resxFile, existingResources);

                    // Update dictionary if provided
                    resourceDictionary?.Remove(resourceName);

                    result.IsSuccess = true;
                    result.ResourceName = resourceName;
                    result.FilePath = resxFile;
                    result.Message = $"Resource '{resourceName}' successfully removed from .resx file";
                }
                else
                {
                    result.AddWarning($"Resource '{resourceName}' was not found in .resx file");
                    result.IsSuccess = true; // Not an error if resource doesn't exist
                }

                return result;
            }
            catch (Exception ex)
            {
                result.AddError($"Failed to remove resource from .resx: {ex.Message}");
                result.Exception = ex;
                return result;
            }
        }

        /// <summary>
        /// Gets all resource names from a .resx file
        /// </summary>
        public static async Task<List<string>> GetResourceNamesAsync(string resxFile)
        {
            if (!File.Exists(resxFile))
                return new List<string>();

            try
            {
                var resources = await LoadResxResourcesAsync(resxFile);
                return resources.Keys.ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Checks if a resource exists in a .resx file
        /// </summary>
        public static async Task<bool> ResourceExistsAsync(string resxFile, string resourceName)
        {
            if (!File.Exists(resxFile) || string.IsNullOrEmpty(resourceName))
                return false;

            try
            {
                var resources = await LoadResxResourcesAsync(resxFile);
                return resources.ContainsKey(resourceName);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a backup of a .resx file before modifications
        /// </summary>
        public static async Task<string> CreateBackupAsync(string resxFile)
        {
            if (!File.Exists(resxFile))
                throw new FileNotFoundException($".resx file not found: {resxFile}");

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var backupFileName = $"{Path.GetFileNameWithoutExtension(resxFile)}_backup_{timestamp}.resx";
            var backupPath = Path.Combine(Path.GetDirectoryName(resxFile), backupFileName);

            await Task.Run(() => File.Copy(resxFile, backupPath, false));
            
            return backupPath;
        }
    }
}