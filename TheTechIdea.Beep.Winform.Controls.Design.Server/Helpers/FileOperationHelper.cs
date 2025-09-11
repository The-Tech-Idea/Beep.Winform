using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server
{
    /// <summary>
    /// Assistant class for file operations like copying, moving, and managing files
    /// </summary>
    public static class FileOperationHelper
    {
        /// <summary>
        /// Asynchronously copies a file to a destination with conflict resolution
        /// </summary>
        public static async Task<FileOperationResult> CopyFileAsync(
            string sourceFilePath,
            string destinationFolder,
            string customFileName = null,
            bool overwrite = false,
            IProgress<ProgressEventArgs> progress = null)
        {
            var result = new FileOperationResult();

            try
            {
                if (!File.Exists(sourceFilePath))
                {
                    result.AddError($"Source file not found: {sourceFilePath}");
                    return result;
                }

                progress?.Report(new ProgressEventArgs { Message = "Validating destination...", PercentComplete = 10 });

                // Ensure destination directory exists
                await Task.Run(() => Directory.CreateDirectory(destinationFolder));

                progress?.Report(new ProgressEventArgs { Message = "Preparing file copy...", PercentComplete = 20 });

                // Determine final file name
                var fileName = customFileName ?? Path.GetFileName(sourceFilePath);
                
                if (!overwrite)
                {
                    fileName = ResourceValidationHelper.GenerateUniqueFileName(destinationFolder, fileName);
                }

                var destinationPath = Path.Combine(destinationFolder, fileName);

                progress?.Report(new ProgressEventArgs { Message = "Copying file...", PercentComplete = 50 });

                // Perform the actual copy operation
                await CopyFileWithProgressAsync(sourceFilePath, destinationPath, progress);

                progress?.Report(new ProgressEventArgs { Message = "Copy completed", PercentComplete = 100 });

                result.IsSuccess = true;
                result.SourcePath = sourceFilePath;
                result.DestinationPath = destinationPath;
                result.FileName = fileName;
                result.Message = $"File successfully copied to {destinationPath}";

                return result;
            }
            catch (Exception ex)
            {
                result.AddError($"Failed to copy file: {ex.Message}");
                result.Exception = ex;
                return result;
            }
        }

        /// <summary>
        /// Asynchronously moves a file to a destination
        /// </summary>
        public static async Task<FileOperationResult> MoveFileAsync(
            string sourceFilePath,
            string destinationFolder,
            string customFileName = null,
            bool overwrite = false)
        {
            var result = new FileOperationResult();

            try
            {
                if (!File.Exists(sourceFilePath))
                {
                    result.AddError($"Source file not found: {sourceFilePath}");
                    return result;
                }

                // First copy the file
                var copyResult = await CopyFileAsync(sourceFilePath, destinationFolder, customFileName, overwrite);
                
                if (!copyResult.IsSuccess)
                {
                    result.Errors.AddRange(copyResult.Errors);
                    return result;
                }

                // Then delete the original
                await Task.Run(() => File.Delete(sourceFilePath));

                result.IsSuccess = true;
                result.SourcePath = sourceFilePath;
                result.DestinationPath = copyResult.DestinationPath;
                result.FileName = copyResult.FileName;
                result.Message = $"File successfully moved to {copyResult.DestinationPath}";

                return result;
            }
            catch (Exception ex)
            {
                result.AddError($"Failed to move file: {ex.Message}");
                result.Exception = ex;
                return result;
            }
        }

        /// <summary>
        /// Copies file with progress reporting for large files
        /// </summary>
        private static async Task CopyFileWithProgressAsync(
            string sourceFilePath,
            string destinationPath,
            IProgress<ProgressEventArgs> progress = null)
        {
            const int bufferSize = 8192; // 8KB buffer
            var fileInfo = new FileInfo(sourceFilePath);
            var totalBytes = fileInfo.Length;
            var copiedBytes = 0L;

            using (var sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
            using (var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
            {
                var buffer = new byte[bufferSize];
                int bytesRead;

                while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await destinationStream.WriteAsync(buffer, 0, bytesRead);
                    copiedBytes += bytesRead;

                    if (progress != null && totalBytes > 0)
                    {
                        var percentComplete = (int)((copiedBytes * 100) / totalBytes);
                        progress.Report(new ProgressEventArgs
                        {
                            Message = $"Copying... {copiedBytes:N0} of {totalBytes:N0} bytes",
                            PercentComplete = Math.Min(percentComplete, 99) // Cap at 99% until complete
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Creates a directory structure if it doesn't exist
        /// </summary>
        public static async Task<bool> EnsureDirectoryExistsAsync(string directoryPath)
        {
            if (string.IsNullOrEmpty(directoryPath))
                return false;

            try
            {
                await Task.Run(() =>
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a file safely with error handling
        /// </summary>
        public static async Task<FileOperationResult> DeleteFileAsync(string filePath, bool moveToRecycleBin = false)
        {
            var result = new FileOperationResult();

            try
            {
                if (!File.Exists(filePath))
                {
                    result.AddWarning($"File not found: {filePath}");
                    result.IsSuccess = true; // Not an error if file doesn't exist
                    return result;
                }

                await Task.Run(() =>
                {
                    if (moveToRecycleBin)
                    {
                        // Move to recycle bin (requires additional reference)
                        // Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(filePath, 
                        //     Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, 
                        //     Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                        
                        // For now, just delete directly
                        File.Delete(filePath);
                    }
                    else
                    {
                        File.Delete(filePath);
                    }
                });

                result.IsSuccess = true;
                result.SourcePath = filePath;
                result.Message = $"File successfully deleted: {filePath}";

                return result;
            }
            catch (Exception ex)
            {
                result.AddError($"Failed to delete file: {ex.Message}");
                result.Exception = ex;
                return result;
            }
        }

        /// <summary>
        /// Gets file information and metadata
        /// </summary>
        public static async Task<FileMetadata> GetFileMetadataAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            return await Task.Run(() =>
            {
                try
                {
                    var fileInfo = new FileInfo(filePath);
                    
                    return new FileMetadata
                    {
                        FullPath = filePath,
                        FileName = fileInfo.Name,
                        FileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath),
                        Extension = fileInfo.Extension,
                        DirectoryName = fileInfo.DirectoryName,
                        SizeInBytes = fileInfo.Length,
                        CreatedDate = fileInfo.CreationTime,
                        ModifiedDate = fileInfo.LastWriteTime,
                        IsReadOnly = fileInfo.IsReadOnly,
                        IsValidImage = ResourceValidationHelper.IsValidImageFile(filePath)
                    };
                }
                catch
                {
                    return null;
                }
            });
        }

        /// <summary>
        /// Batch operation for processing multiple files
        /// </summary>
        public static async Task<BatchFileOperationResult> ProcessMultipleFilesAsync(
            string[] filePaths,
            string destinationFolder,
            FileOperationType operationType,
            IProgress<ProgressEventArgs> progress = null)
        {
            var batchResult = new BatchFileOperationResult();

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

                    FileOperationResult operationResult;

                    switch (operationType)
                    {
                        case FileOperationType.Copy:
                            operationResult = await CopyFileAsync(filePath, destinationFolder);
                            break;
                        case FileOperationType.Move:
                            operationResult = await MoveFileAsync(filePath, destinationFolder);
                            break;
                        default:
                            operationResult = new FileOperationResult();
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
                    var failedResult = new FileOperationResult();
                    failedResult.AddError($"Exception processing {filePath}: {ex.Message}");
                    batchResult.FailedOperations.Add(filePath, failedResult);
                }

                processedFiles++;
            }

            batchResult.TotalProcessed = processedFiles;
            return batchResult;
        }
    }

    #region Supporting Classes

    /// <summary>
    /// Result of file operations
    /// </summary>
    public class FileOperationResult
    {
        public bool IsSuccess { get; set; }
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public string FileName { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();
        public Exception Exception { get; set; }

        public void AddError(string error) => Errors.Add(error);
        public void AddWarning(string warning) => Warnings.Add(warning);
    }

    /// <summary>
    /// File metadata information
    /// </summary>
    public class FileMetadata
    {
        public string FullPath { get; set; }
        public string FileName { get; set; }
        public string FileNameWithoutExtension { get; set; }
        public string Extension { get; set; }
        public string DirectoryName { get; set; }
        public long SizeInBytes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsValidImage { get; set; }

        public string FormattedSize
        {
            get
            {
                string[] sizes = { "B", "KB", "MB", "GB" };
                double len = SizeInBytes;
                int order = 0;
                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len = len / 1024;
                }
                return $"{len:0.##} {sizes[order]}";
            }
        }
    }

    /// <summary>
    /// Result of batch file operations
    /// </summary>
    public class BatchFileOperationResult
    {
        public int TotalProcessed { get; set; }
        public Dictionary<string, FileOperationResult> SuccessfulOperations { get; } = new Dictionary<string, FileOperationResult>();
        public Dictionary<string, FileOperationResult> FailedOperations { get; } = new Dictionary<string, FileOperationResult>();
        public List<string> Errors { get; } = new List<string>();

        public void AddError(string error) => Errors.Add(error);
        public bool HasFailures => FailedOperations.Count > 0;
        public int SuccessCount => SuccessfulOperations.Count;
        public int FailureCount => FailedOperations.Count;
    }

    /// <summary>
    /// Types of file operations
    /// </summary>
    public enum FileOperationType
    {
        Copy,
        Move,
        Delete
    }

    #endregion
}