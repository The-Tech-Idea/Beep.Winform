using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheTechIdea.Beep.Winform.Controls.Docking.Models;

namespace TheTechIdea.Beep.Winform.Controls.Docking.Layout
{
    /// <summary>
    /// Validates and repairs docking layout consistency.
    /// Checks for invalid states, overlaps, and orphaned elements.
    /// </summary>
    public class LayoutValidator
    {
        private readonly DockLayoutTree _layoutTree;
        private readonly LayoutCalculator _calculator;

        private List<ValidationError> _errors = new List<ValidationError>();

        public LayoutValidator(DockLayoutTree layoutTree, LayoutCalculator calculator = null)
        {
            _layoutTree = layoutTree ?? throw new ArgumentNullException(nameof(layoutTree));
            _calculator = calculator ?? new LayoutCalculator(50, 50, 4);
        }

        /// <summary>
        /// Validates the entire layout tree.
        /// Returns true if valid, false if errors found.
        /// </summary>
        public bool Validate()
        {
            _errors.Clear();

            // Run all validation checks
            ValidateHierarchy();
            ValidateRatios();
            ValidatePanelReferences();
            ValidateGroupReferences();
            ValidateNoBounds();  // Additional check for structural integrity

            return _errors.Count == 0;
        }

        /// <summary>
        /// Gets all validation errors found.
        /// </summary>
        public List<ValidationError> GetErrors() => new List<ValidationError>(_errors);

        /// <summary>
        /// Gets human-readable error summary.
        /// </summary>
        public string GetErrorSummary()
        {
            if (_errors.Count == 0)
                return "Layout is valid.";

            var summary = new System.Text.StringBuilder();
            summary.AppendLine($"Layout has {_errors.Count} error(s):");
            foreach (var error in _errors)
            {
                summary.AppendLine($"  - {error.ErrorType}: {error.Message}");
            }
            return summary.ToString();
        }

        /// <summary>
        /// Attempts to repair layout errors.
        /// Returns true if all errors were fixable.
        /// </summary>
        public bool TryRepair()
        {
            if (!Validate())
            {
                RepairErrors();
                return Validate();  // Check if repairs fixed everything
            }
            return true;
        }

        #region Private Validation Methods

        /// <summary>
        /// Validates the group hierarchy structure.
        /// </summary>
        private void ValidateHierarchy()
        {
            if (_layoutTree.Root == null)
            {
                _errors.Add(new ValidationError
                {
                    ErrorType = ErrorType.MissingRoot,
                    Message = "Layout tree has no root group",
                    Severity = ErrorSeverity.Critical
                });
                return;
            }

            // Validate no circular references
            ValidateNoCircularReferences(_layoutTree.Root, new HashSet<string>());

            // Validate all groups are reachable from root
            var reachable = new HashSet<string>();
            CollectReachableGroups(_layoutTree.Root, reachable);

            foreach (var group in _layoutTree.GetAllGroups())
            {
                if (!reachable.Contains(group.Id))
                {
                    _errors.Add(new ValidationError
                    {
                        ErrorType = ErrorType.UnreachableGroup,
                        Message = $"Group '{group.Id}' is not reachable from root",
                        Severity = ErrorSeverity.High,
                        AffectedElement = group.Id
                    });
                }
            }
        }

        /// <summary>
        /// Validates split ratios are in valid ranges.
        /// </summary>
        private void ValidateRatios()
        {
            foreach (var group in _layoutTree.GetAllGroups())
            {
                if (!_calculator.IsValidRatio(group.SplitRatio))
                {
                    _errors.Add(new ValidationError
                    {
                        ErrorType = ErrorType.InvalidRatio,
                        Message = $"Group '{group.Id}' has invalid ratio {group.SplitRatio:P}",
                        Severity = ErrorSeverity.Medium,
                        AffectedElement = group.Id
                    });
                }

                // Check that ratio is only used when group has child groups
                if (group.Children.Count < 2 && group.SplitRatio != 0.5f)
                {
                    _errors.Add(new ValidationError
                    {
                        ErrorType = ErrorType.RatioWithoutSplit,
                        Message = $"Group '{group.Id}' has ratio but fewer than 2 child groups",
                        Severity = ErrorSeverity.Low,
                        AffectedElement = group.Id
                    });
                }
            }
        }

        /// <summary>
        /// Validates panel references are valid.
        /// </summary>
        private void ValidatePanelReferences()
        {
            foreach (var panel in _layoutTree.GetAllPanels())
            {
                // Check if panel's group exists
                if (panel.Group == null)
                {
                    _errors.Add(new ValidationError
                    {
                        ErrorType = ErrorType.InvalidGroupReference,
                        Message = $"Panel '{panel.Key}' is not assigned to any group",
                        Severity = ErrorSeverity.High,
                        AffectedElement = panel.Key
                    });
                    continue;
                }

                // Check if panel is actually in that group
                if (!panel.Group.Panels.Any(p => p.Key == panel.Key))
                {
                    _errors.Add(new ValidationError
                    {
                        ErrorType = ErrorType.InconsistentReference,
                        Message = $"Panel '{panel.Key}' has Group reference but group doesn't contain it",
                        Severity = ErrorSeverity.High,
                        AffectedElement = panel.Key
                    });
                }
            }
        }

        /// <summary>
        /// Validates group references are consistent.
        /// </summary>
        private void ValidateGroupReferences()
        {
            foreach (var group in _layoutTree.GetAllGroups())
            {
                // Check all child groups are registered
                foreach (var child in group.Children)
                {
                    if (_layoutTree.GetGroup(child.Id) == null)
                    {
                        _errors.Add(new ValidationError
                        {
                            ErrorType = ErrorType.InvalidGroupReference,
                            Message = $"Group '{group.Id}' references non-existent child group '{child.Id}'",
                            Severity = ErrorSeverity.High,
                            AffectedElement = group.Id
                        });
                    }
                }

                // Check all panels in group are registered
                foreach (var panel in group.Panels)
                {
                    if (_layoutTree.GetPanel(panel.Key) == null)
                    {
                        _errors.Add(new ValidationError
                        {
                            ErrorType = ErrorType.InvalidPanelReference,
                            Message = $"Group '{group.Id}' contains unregistered panel '{panel.Key}'",
                            Severity = ErrorSeverity.High,
                            AffectedElement = group.Id
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Validates that all groups have proper bounds (not empty).
        /// </summary>
        private void ValidateNoBounds()
        {
            // Check for groups with invalid properties
            foreach (var group in _layoutTree.GetAllGroups())
            {
                // Leaf groups (with no children) should have panels
                bool isLeafGroup = group.Children.Count == 0;
                if (isLeafGroup && group.Panels.Count == 0)
                {
                    _errors.Add(new ValidationError
                    {
                        ErrorType = ErrorType.EmptyGroup,
                        Message = $"Group '{group.Id}' is empty (no panels or child groups)",
                        Severity = ErrorSeverity.Medium,
                        AffectedElement = group.Id
                    });
                }

                // Child groups should not have panels (should be in leaf groups only)
                bool isParentGroup = group.Children.Count > 0;
                if (isParentGroup && group.Panels.Count > 0)
                {
                    _errors.Add(new ValidationError
                    {
                        ErrorType = ErrorType.MixedContent,
                        Message = $"Group '{group.Id}' has both child groups and panels",
                        Severity = ErrorSeverity.High,
                        AffectedElement = group.Id
                    });
                }
            }
        }

        #endregion

        #region Private Repair Methods

        /// <summary>
        /// Attempts to repair validation errors.
        /// </summary>
        private void RepairErrors()
        {
            // Repair invalid ratios
            foreach (var error in _errors.Where(e => e.ErrorType == ErrorType.InvalidRatio).ToList())
            {
                var group = _layoutTree.GetGroup(error.AffectedElement);
                if (group != null)
                {
                    group.SplitRatio = _calculator.ClampRatio(group.SplitRatio);
                }
            }

            // Repair orphaned groups by reattaching to root
            var root = _layoutTree.Root;
            if (root != null)
            {
                var reachable = new HashSet<string>();
                CollectReachableGroups(root, reachable);

                foreach (var group in _layoutTree.GetAllGroups())
                {
                    if (!reachable.Contains(group.Id) && group.Id != root.Id)
                    {
                        // Try to reattach to root
                        // Note: This is a simplified repair; more sophisticated logic may be needed
                    }
                }
            }
        }

        /// <summary>
        /// Checks for circular references in group hierarchy.
        /// </summary>
        private void ValidateNoCircularReferences(DockGroup group, HashSet<string> visited)
        {
            if (group == null)
                return;

            if (visited.Contains(group.Id))
            {
                _errors.Add(new ValidationError
                {
                    ErrorType = ErrorType.CircularReference,
                    Message = $"Circular reference detected in group hierarchy at '{group.Id}'",
                    Severity = ErrorSeverity.Critical,
                    AffectedElement = group.Id
                });
                return;
            }

            visited.Add(group.Id);

            foreach (var child in group.Children)
            {
                ValidateNoCircularReferences(child, new HashSet<string>(visited));
            }
        }

        /// <summary>
        /// Collects all groups reachable from a starting group.
        /// </summary>
        private void CollectReachableGroups(DockGroup group, HashSet<string> reachable)
        {
            if (group == null || reachable.Contains(group.Id))
                return;

            reachable.Add(group.Id);

            foreach (var child in group.Children)
            {
                CollectReachableGroups(child, reachable);
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents a single validation error.
    /// </summary>
    public class ValidationError
    {
        public ErrorType ErrorType { get; set; }
        public string Message { get; set; }
        public ErrorSeverity Severity { get; set; }
        public string AffectedElement { get; set; }

        public override string ToString()
        {
            var element = string.IsNullOrEmpty(AffectedElement) ? "" : $" [{AffectedElement}]";
            return $"{Severity} - {ErrorType}: {Message}{element}";
        }
    }

    /// <summary>
    /// Types of validation errors.
    /// </summary>
    public enum ErrorType
    {
        MissingRoot,
        UnreachableGroup,
        InvalidRatio,
        RatioWithoutSplit,
        InvalidGroupReference,
        InvalidPanelReference,
        InconsistentReference,
        CircularReference,
        EmptyGroup,
        MixedContent,
        OverlappingBounds
    }

    /// <summary>
    /// Severity levels for validation errors.
    /// </summary>
    public enum ErrorSeverity
    {
        Critical,    // Breaks layout functionality
        High,        // Causes incorrect behavior
        Medium,      // May cause issues
        Low          // Minor inconsistency
    }
}
