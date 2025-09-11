using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheTechIdea.Beep.Winform.Controls.Models;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server
{
    /// <summary>
    /// Assistant class for validating file paths and project structures
    /// </summary>
    public static class ResourceValidationHelper
    {
        /// <summary>
        /// Supported image file extensions
        /// </summary>
        public static readonly string[] SupportedImageExtensions = 
        {
            ".png", ".jpg", ".jpeg", ".bmp", ".svg", ".ico", ".gif", ".tiff", ".webp"
        };

        /// <summary>
        /// Validates if the file path is a supported image format
        /// </summary>
        public static bool IsValidImageFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return false;

            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return SupportedImageExtensions.Contains(extension);
        }

        /// <summary>
        /// Validates project directory and ensures it contains a .csproj file
        /// </summary>
        public static ProjectValidationResult ValidateProjectDirectory(string projectDirectory)
        {
            var result = new ProjectValidationResult();

            if (string.IsNullOrWhiteSpace(projectDirectory))
            {
                result.AddError("Project directory cannot be null or empty");
                return result;
            }

            if (!Directory.Exists(projectDirectory))
            {
                result.AddError($"Project directory does not exist: {projectDirectory}");
                return result;
            }

            var csprojFiles = Directory.GetFiles(projectDirectory, "*.csproj");
            if (csprojFiles.Length == 0)
            {
                result.AddError("No .csproj file found in the project directory");
                return result;
            }

            if (csprojFiles.Length > 1)
            {
                result.AddWarning($"Multiple .csproj files found. Using: {csprojFiles[0]}");
            }

            result.IsValid = true;
            result.CsprojPath = csprojFiles[0];
            return result;
        }

        /// <summary>
        /// Validates resource name for conflicts and naming conventions
        /// </summary>
        public static bool IsValidResourceName(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
                return false;

            // Check for invalid characters
            var invalidChars = Path.GetInvalidFileNameChars().Concat(new[] { ' ', '-' }).ToArray();
            return !resourceName.Any(c => invalidChars.Contains(c));
        }

        /// <summary>
        /// Checks if a file already exists and generates a unique name if needed
        /// </summary>
        public static string GenerateUniqueFileName(string destinationFolder, string originalFileName)
        {
            var destPath = Path.Combine(destinationFolder, originalFileName);
            
            if (!File.Exists(destPath))
                return originalFileName;

            var nameWithoutExt = Path.GetFileNameWithoutExtension(originalFileName);
            var extension = Path.GetExtension(originalFileName);
            var counter = 1;

            string uniqueFileName;
            do
            {
                uniqueFileName = $"{nameWithoutExt}_{counter}{extension}";
                destPath = Path.Combine(destinationFolder, uniqueFileName);
                counter++;
            } while (File.Exists(destPath));

            return uniqueFileName;
        }

        /// <summary>
        /// Validates file size constraints
        /// </summary>
        public static bool IsFileSizeValid(string filePath, long maxSizeInMB = 50)
        {
            if (!File.Exists(filePath))
                return false;

            var fileInfo = new FileInfo(filePath);
            var maxSizeInBytes = maxSizeInMB * 1024 * 1024;
            
            return fileInfo.Length <= maxSizeInBytes;
        }
    }

    /// <summary>
    /// Result of project validation operations
    /// </summary>
    public class ProjectValidationResult
    {
        public bool IsValid { get; set; }
        public string CsprojPath { get; set; }
        public List<string> Errors { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();

        public void AddError(string error) => Errors.Add(error);
        public void AddWarning(string warning) => Warnings.Add(warning);
    }
}