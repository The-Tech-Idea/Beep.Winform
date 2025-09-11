using System;
using System.Collections.Generic;

namespace TheTechIdea.Beep.Winform.Controls.Design.Server
{
    /// <summary>
    /// Event arguments for progress reporting
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        public string Message { get; set; }
        public int PercentComplete { get; set; }
        public int CurrentItem { get; set; }
        public int TotalItems { get; set; }
    }

    /// <summary>
    /// Event arguments for resource operations
    /// </summary>
    public class ResourceOperationEventArgs : EventArgs
    {
        public ResourceOperationType OperationType { get; set; }
        public string ResourceName { get; set; }
        public string FilePath { get; set; }
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
    }

    /// <summary>
    /// Types of resource operations
    /// </summary>
    public enum ResourceOperationType
    {
        ResxEmbed,
        ProjectEmbed,
        Copy,
        Move,
        Remove
    }

    /// <summary>
    /// Result of a resource operation
    /// </summary>
    public class ResourceOperationResult
    {
        public bool IsSuccess { get; set; }
        public string ResourceName { get; set; }
        public string FilePath { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();
        public Exception Exception { get; set; }

        public void AddError(string error) => Errors.Add(error);
        public void AddWarning(string warning) => Warnings.Add(warning);
    }

    /// <summary>
    /// Result of batch operations
    /// </summary>
    public class BatchOperationResult
    {
        public int TotalProcessed { get; set; }
        public Dictionary<string, ResourceOperationResult> SuccessfulOperations { get; } = new Dictionary<string, ResourceOperationResult>();
        public Dictionary<string, ResourceOperationResult> FailedOperations { get; } = new Dictionary<string, ResourceOperationResult>();
        public List<string> Errors { get; } = new List<string>();

        public void AddError(string error) => Errors.Add(error);
        public bool HasFailures => FailedOperations.Count > 0;
        public int SuccessCount => SuccessfulOperations.Count;
        public int FailureCount => FailedOperations.Count;
    }

    /// <summary>
    /// Result of backup operations
    /// </summary>
    public class BackupResult
    {
        public bool IsSuccess { get; set; }
        public string CsprojBackupPath { get; set; }
        public string ResxBackupPath { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; } = new List<string>();
        public Exception Exception { get; set; }

        public void AddError(string error) => Errors.Add(error);
    }
}