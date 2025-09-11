using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheTechIdea.Beep.Vis.Modules;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server
{
    /// <summary>
    /// Main class for embedding resources into projects using modular assistant classes
    /// Provides both synchronous and asynchronous methods for resource management
    /// </summary>
    public static class ProjectResourceEmbedder
    {
        #region Events

        /// <summary>
        /// Event raised when a resource operation completes successfully
        /// </summary>
        public static event EventHandler<ResourceOperationEventArgs> OperationCompleted;

        /// <summary>
        /// Event raised when an operation fails
        /// </summary>
        public static event EventHandler<ResourceOperationEventArgs> OperationFailed;

        /// <summary>
        /// Event raised to report progress of operations
        /// </summary>
        public static event EventHandler<ProgressEventArgs> ProgressReported;

        #endregion

        #region Properties

        /// <summary>
        /// Dictionary containing all embedded images with their configurations
        /// </summary>
        public static Dictionary<string, ImageConfiguration> EmbeddedImages { get; } = new Dictionary<string, ImageConfiguration>();

        /// <summary>
        /// Default resource folder names
        /// </summary>
        public static readonly string[] DefaultResourceFolders = { "Resources", "Properties\\Resources", "Assets", "Images" };

        #endregion

        #region Async Methods - .resx Embedding

        /// <summary>
        /// Asynchronously embeds an image into .resx resources
        /// </summary>
        public static async Task<ResourceOperationResult> EmbedImageInResourcesAsync(
            Dictionary<string, SimpleItem> imageResources,
            string resxFile,
            string previewFilePath,
            string projectDirectory,
            IProgress<ProgressEventArgs> progress = null,
            string customResourceName = null)
        {
            var result = new ResourceOperationResult();

            try
            {
                // Step 1: Validate inputs
                if (string.IsNullOrEmpty(previewFilePath))
                {
                    result.AddError("Preview file path is required");
                    return result;
                }

                if (!ResourceValidationHelper.IsValidImageFile(previewFilePath))
                {
                    result.AddError($"Invalid or unsupported image file: {previewFilePath}");
                    return result;
                }

                progress?.Report(new ProgressEventArgs { Message = "Validating inputs...", PercentComplete = 10 });

                // Step 2: Generate resource name
                var resourceName = customResourceName ?? Path.GetFileNameWithoutExtension(previewFilePath);
                if (!ResourceValidationHelper.IsValidResourceName(resourceName))
                {
                    result.AddError($"Invalid resource name: {resourceName}");
                    return result;
                }

                progress?.Report(new ProgressEventArgs { Message = "Creating resource directory...", PercentComplete = 20 });

                // Step 3: Create resources directory and copy file
                var resourcesPath = Path.Combine(projectDirectory, "Properties", "Resources");
                await FileOperationHelper.EnsureDirectoryExistsAsync(resourcesPath);

                progress?.Report(new ProgressEventArgs { Message = "Copying file...", PercentComplete = 40 });

                var copyResult = await FileOperationHelper.CopyFileAsync(previewFilePath, resourcesPath);
                if (!copyResult.IsSuccess)
                {
                    result.Errors.AddRange(copyResult.Errors);
                    return result;
                }

                progress?.Report(new ProgressEventArgs { Message = "Adding to .resx file...", PercentComplete = 70 });

                // Step 4: Add to .resx file
                using (var bitmap = new Bitmap(copyResult.DestinationPath))
                {
                    var resxResult = await ResxResourceHelper.AddOrUpdateResourceAsync(
                        resxFile, resourceName, bitmap.Clone(), imageResources);
                    
                    if (!resxResult.IsSuccess)
                    {
                        result.Errors.AddRange(resxResult.Errors);
                        return result;
                    }
                }

                progress?.Report(new ProgressEventArgs { Message = "Updating collections...", PercentComplete = 90 });

                // Step 5: Update embedded images collection
                EmbeddedImages[resourceName] = new ImageConfiguration
                {
                    Name = resourceName,
                    Path = copyResult.DestinationPath,
                    Ext = Path.GetExtension(copyResult.DestinationPath),
                    IsResxEmbedded = true,
                    GuidID = Guid.NewGuid().ToString()
                };

                progress?.Report(new ProgressEventArgs { Message = "Operation completed", PercentComplete = 100 });

                result.IsSuccess = true;
                result.ResourceName = resourceName;
                result.FilePath = copyResult.DestinationPath;
                result.Message = "Image successfully embedded in .resx resources";

                OperationCompleted?.Invoke(null, new ResourceOperationEventArgs
                {
                    OperationType = ResourceOperationType.ResxEmbed,
                    ResourceName = resourceName,
                    FilePath = copyResult.DestinationPath,
                    IsSuccess = true
                });

                return result;
            }
            catch (Exception ex)
            {
                result.AddError($"Failed to embed image: {ex.Message}");
                result.Exception = ex;

                OperationFailed?.Invoke(null, new ResourceOperationEventArgs
                {
                    OperationType = ResourceOperationType.ResxEmbed,
                    Error = ex.Message,
                    IsSuccess = false
                });

                return result;
            }
        }

        #endregion

        #region Async Methods - Project Embedding

        /// <summary>
        /// Asynchronously copies and embeds a file as project resource
        /// </summary>
        public static async Task<ResourceOperationResult> CopyAndEmbedFileToProjectResourcesAsync(
            Dictionary<string, SimpleItem> projectResources,
            string previewFilePath,
            string projectDirectory,
            IProgress<ProgressEventArgs> progress = null,
            string customResourceName = null,
            string targetFolder = "Resources")
        {
            var result = new ResourceOperationResult();

            try
            {
                // Step 1: Validate inputs
                if (string.IsNullOrEmpty(previewFilePath))
                {
                    result.AddError("Preview file path is required");
                    return result;
                }

                var projectValidation = ResourceValidationHelper.ValidateProjectDirectory(projectDirectory);
                if (!projectValidation.IsValid)
                {
                    result.Errors.AddRange(projectValidation.Errors);
                    return result;
                }

                if (!ResourceValidationHelper.IsValidImageFile(previewFilePath))
                {
                    result.AddError($"Invalid or unsupported image file: {previewFilePath}");
                    return result;
                }

                progress?.Report(new ProgressEventArgs { Message = "Starting operation...", PercentComplete = 5 });

                // Step 2: Copy file to project
                var resourcesFolder = Path.Combine(projectDirectory, targetFolder);
                await FileOperationHelper.EnsureDirectoryExistsAsync(resourcesFolder);

                progress?.Report(new ProgressEventArgs { Message = "Copying file to project...", PercentComplete = 20 });

                var copyResult = await FileOperationHelper.CopyFileAsync(previewFilePath, resourcesFolder, null, false, progress);
                if (!copyResult.IsSuccess)
                {
                    result.Errors.AddRange(copyResult.Errors);
                    return result;
                }

                progress?.Report(new ProgressEventArgs { Message = "Updating project file...", PercentComplete = 60 });

                // Step 3: Add to .csproj as embedded resource
                var embedResult = await ProjectFileHelper.AddEmbeddedResourceAsync(
                    copyResult.DestinationPath, projectValidation.CsprojPath, projectDirectory);
                
                if (!embedResult.IsSuccess)
                {
                    result.Errors.AddRange(embedResult.Errors);
                    return result;
                }

                progress?.Report(new ProgressEventArgs { Message = "Updating collections...", PercentComplete = 80 });

                // Step 4: Update resource collections
                var resourceName = customResourceName ?? Path.GetFileNameWithoutExtension(copyResult.DestinationPath);
                
                projectResources[resourceName] = new SimpleItem
                {
                    Name = resourceName,
                    ImagePath = copyResult.DestinationPath,
                    GuidId = Guid.NewGuid().ToString()
                };

                EmbeddedImages[resourceName] = new ImageConfiguration
                {
                    Name = resourceName,
                    Path = copyResult.DestinationPath,
                    Ext = Path.GetExtension(copyResult.DestinationPath),
                    IsProjResource = true,
                    GuidID = Guid.NewGuid().ToString()
                };

                progress?.Report(new ProgressEventArgs { Message = "Operation completed", PercentComplete = 100 });

                result.IsSuccess = true;
                result.ResourceName = resourceName;
                result.FilePath = copyResult.DestinationPath;
                result.Message = "File successfully copied and embedded as project resource";

                OperationCompleted?.Invoke(null, new ResourceOperationEventArgs
                {
                    OperationType = ResourceOperationType.ProjectEmbed,
                    ResourceName = resourceName,
                    FilePath = copyResult.DestinationPath,
                    IsSuccess = true
                });

                return result;
            }
            catch (Exception ex)
            {
                result.AddError($"Failed to copy and embed file: {ex.Message}");
                result.Exception = ex;

                OperationFailed?.Invoke(null, new ResourceOperationEventArgs
                {
                    OperationType = ResourceOperationType.ProjectEmbed,
                    Error = ex.Message,
                    IsSuccess = false
                });

                return result;
            }
        }

        #endregion

        #region Batch Operations

        /// <summary>
        /// Processes multiple files in batch operations
        /// </summary>
        public static async Task<BatchOperationResult> ProcessMultipleFilesAsync(
            Dictionary<string, SimpleItem> targetDictionary,
            string[] filePaths,
            string projectDirectory,
            ResourceOperationType operationType,
            IProgress<ProgressEventArgs> progress = null,
            string targetFolder = "Resources")
        {
            var batchResult = new BatchOperationResult();
            
            if (filePaths == null || filePaths.Length == 0)
            {
                batchResult.AddError("No files provided for processing");
                return batchResult;
            }

            var totalFiles = filePaths.Length;
            var processedFiles = 0;

            foreach (var filePath in filePaths)
            {
                try
                {
                    progress?.Report(new ProgressEventArgs
                    {
                        Message = $"Processing {Path.GetFileName(filePath)}...",
                        PercentComplete = (processedFiles * 100) / totalFiles,
                        CurrentItem = processedFiles + 1,
                        TotalItems = totalFiles
                    });

                    ResourceOperationResult operationResult;

                    switch (operationType)
                    {
                        case ResourceOperationType.ProjectEmbed:
                            operationResult = await CopyAndEmbedFileToProjectResourcesAsync(
                                targetDictionary, filePath, projectDirectory, null, null, targetFolder);
                            break;
                        case ResourceOperationType.ResxEmbed:
                            var resxFile = Path.Combine(projectDirectory, "Properties", "Resources.resx");
                            operationResult = await EmbedImageInResourcesAsync(
                                targetDictionary, resxFile, filePath, projectDirectory);
                            break;
                        default:
                            operationResult = new ResourceOperationResult();
                            operationResult.AddError($"Unsupported operation type: {operationType}");
                            break;
                    }

                    if (operationResult.IsSuccess)
                    {
                        batchResult.SuccessfulOperations.Add(filePath, operationResult);
                    }
                    else
                    {
                        batchResult.FailedOperations.Add(filePath, operationResult);
                    }
                }
                catch (Exception ex)
                {
                    var failedResult = new ResourceOperationResult();
                    failedResult.AddError($"Exception processing {filePath}: {ex.Message}");
                    batchResult.FailedOperations.Add(filePath, failedResult);
                }

                processedFiles++;
            }

            batchResult.TotalProcessed = processedFiles;
            return batchResult;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets existing embedded resources from project
        /// </summary>
        public static async Task<List<string>> GetExistingEmbeddedResourcesAsync(string projectDirectory)
        {
            var projectValidation = ResourceValidationHelper.ValidateProjectDirectory(projectDirectory);
            if (!projectValidation.IsValid)
                return new List<string>();

            return await ProjectFileHelper.GetEmbeddedResourcesAsync(projectValidation.CsprojPath);
        }

        /// <summary>
        /// Removes an embedded resource from the project
        /// </summary>
        public static async Task<ResourceOperationResult> RemoveEmbeddedResourceAsync(
            string resourcePath, 
            string projectDirectory,
            bool deletePhysicalFile = true)
        {
            var projectValidation = ResourceValidationHelper.ValidateProjectDirectory(projectDirectory);
            if (!projectValidation.IsValid)
            {
                var result = new ResourceOperationResult();
                result.Errors.AddRange(projectValidation.Errors);
                return result;
            }

            return await ProjectFileHelper.RemoveEmbeddedResourceAsync(
                resourcePath, projectValidation.CsprojPath, projectDirectory, deletePhysicalFile);
        }

        /// <summary>
        /// Loads embedded resources into a dictionary
        /// </summary>
        public static async Task LoadEmbeddedResourcesToDictionaryAsync(
            Dictionary<string, SimpleItem> resourceDictionary,
            string projectDirectory)
        {
            await ProjectFileHelper.LoadEmbeddedResourcesToDictionaryAsync(resourceDictionary, projectDirectory);
        }

        /// <summary>
        /// Creates a backup of project files before operations
        /// </summary>
        public static async Task<BackupResult> CreateBackupAsync(string projectDirectory)
        {
            var result = new BackupResult();

            try
            {
                var projectValidation = ResourceValidationHelper.ValidateProjectDirectory(projectDirectory);
                if (!projectValidation.IsValid)
                {
                    result.Errors.AddRange(projectValidation.Errors);
                    return result;
                }

                // Backup .csproj
                result.CsprojBackupPath = await ProjectFileHelper.CreateBackupAsync(projectValidation.CsprojPath);

                // Backup .resx files if they exist
                var resxFile = Path.Combine(projectDirectory, "Properties", "Resources.resx");
                if (File.Exists(resxFile))
                {
                    result.ResxBackupPath = await ResxResourceHelper.CreateBackupAsync(resxFile);
                }

                result.IsSuccess = true;
                result.Message = "Backup created successfully";

                return result;
            }
            catch (Exception ex)
            {
                result.AddError($"Failed to create backup: {ex.Message}");
                result.Exception = ex;
                return result;
            }
        }

        #endregion

        #region Legacy Synchronous Methods (for backward compatibility)

        /// <summary>
        /// Legacy synchronous method - use EmbedImageInResourcesAsync instead
        /// </summary>
        [Obsolete("Use EmbedImageInResourcesAsync instead for better performance and error handling")]
        public static void EmbedImageInResources(
            Dictionary<string, SimpleItem> imageResources,
            string resxFile,
            string previewFilePath,
            string projectDirectory)
        {
            try
            {
                var task = EmbedImageInResourcesAsync(imageResources, resxFile, previewFilePath, projectDirectory);
                task.Wait();

                if (!task.Result.IsSuccess)
                {
                    var errors = string.Join(Environment.NewLine, task.Result.Errors);
                    MessageBox.Show(errors, "Embedding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(task.Result.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Embedding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Legacy synchronous method - use CopyAndEmbedFileToProjectResourcesAsync instead
        /// </summary>
        [Obsolete("Use CopyAndEmbedFileToProjectResourcesAsync instead for better performance and error handling")]
        public static string CopyAndEmbedFileToProjectResources(
            Dictionary<string, SimpleItem> projectResources,
            string previewFilePath,
            string projectDirectory)
        {
            try
            {
                var task = CopyAndEmbedFileToProjectResourcesAsync(projectResources, previewFilePath, projectDirectory);
                task.Wait();

                if (!task.Result.IsSuccess)
                {
                    var errors = string.Join(Environment.NewLine, task.Result.Errors);
                    MessageBox.Show(errors, "Copy and Embed Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                MessageBox.Show(task.Result.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return task.Result.FilePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Copy and Embed Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Legacy method - redirects to CopyAndEmbedFileToProjectResources
        /// </summary>
        [Obsolete("Use CopyAndEmbedFileToProjectResourcesAsync instead")]
        public static string CopyFileToProjectResources(
            Dictionary<string, SimpleItem> projectResources,
            string previewFilePath,
            string projectDirectory)
        {
            return CopyAndEmbedFileToProjectResources(projectResources, previewFilePath, projectDirectory);
        }

        /// <summary>
        /// Legacy method for file embedding
        /// </summary>
        [Obsolete("Use ProjectFileHelper.AddEmbeddedResourceAsync instead")]
        public static void EmbedFileAsEmbeddedResource(
            string filePath,
            string previewFilePath,
            string projectDirectory)
        {
            try
            {
                var projectValidation = ResourceValidationHelper.ValidateProjectDirectory(projectDirectory);
                if (!projectValidation.IsValid)
                {
                    var errors = string.Join(Environment.NewLine, projectValidation.Errors);
                    MessageBox.Show(errors, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var task = ProjectFileHelper.AddEmbeddedResourceAsync(filePath, projectValidation.CsprojPath, projectDirectory);
                task.Wait();

                if (!task.Result.IsSuccess)
                {
                    var errors = string.Join(Environment.NewLine, task.Result.Errors);
                    MessageBox.Show(errors, "Embedding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(task.Result.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Embedding Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Legacy method for moving files
        /// </summary>
        [Obsolete("Use FileOperationHelper.MoveFileAsync instead")]
        public static void MoveFileToProjectResources(
            Dictionary<string, SimpleItem> localImages,
            string sourceFilePath,
            string destinationFolder)
        {
            try
            {
                var task = FileOperationHelper.MoveFileAsync(sourceFilePath, destinationFolder);
                task.Wait();

                if (!task.Result.IsSuccess)
                {
                    var errors = string.Join(Environment.NewLine, task.Result.Errors);
                    MessageBox.Show(errors, "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Update dictionary
                    var fileName = task.Result.FileName;
                    localImages[fileName] = new SimpleItem
                    {
                        Name = fileName,
                        ImagePath = task.Result.DestinationPath
                    };

                    MessageBox.Show(task.Result.Message, "File Moved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}