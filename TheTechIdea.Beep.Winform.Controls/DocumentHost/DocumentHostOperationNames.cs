using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Canonical coordinator operation names used for telemetry and
    /// post-mutation tree validation keys.
    /// </summary>
    internal static class DocumentHostOperationNames
    {
        [DocumentHostOperationName]
        internal const string AutoHideDocument = "auto-hide-document";
        [DocumentHostOperationName]
        internal const string RestoreAutoHideDocument = "restore-auto-hide-document";
        [DocumentHostOperationName]
        internal const string BatchMoveDocument = "batch-move-document";
        [DocumentHostOperationName]
        internal const string CloseDocument = "close-document";
        [DocumentHostOperationName]
        internal const string FloatDocument = "float-document";
        [DocumentHostOperationName]
        internal const string DockBackDocument = "dock-back-document";

        [DocumentHostOperationName]
        internal const string DropDockCentre = "drop-dock-centre";
        [DocumentHostOperationName]
        internal const string DropDockRight = "drop-dock-right";
        [DocumentHostOperationName]
        internal const string DropDockBottom = "drop-dock-bottom";
        [DocumentHostOperationName]
        internal const string DropDockLeft = "drop-dock-left";
        [DocumentHostOperationName]
        internal const string DropDockTop = "drop-dock-top";

        [DocumentHostOperationName]
        internal const string ExternalDropRight = "external-drop-right";
        [DocumentHostOperationName]
        internal const string ExternalDropBottom = "external-drop-bottom";
        [DocumentHostOperationName]
        internal const string ExternalDropLeft = "external-drop-left";
        [DocumentHostOperationName]
        internal const string ExternalDropTop = "external-drop-top";

        [DocumentHostOperationName]
        internal const string DetachExternalDocument = "detach-external-document";
        [DocumentHostOperationName]
        internal const string AttachExternalDocument = "attach-external-document";
        [DocumentHostOperationName]
        internal const string SplitDocument = "split-document";
        [DocumentHostOperationName]
        internal const string MoveDocumentToGroup = "move-document-to-group";
        [DocumentHostOperationName]
        internal const string CollapseEmptyGroup = "collapse-empty-group";
        [DocumentHostOperationName]
        internal const string MergeAllGroups = "merge-all-groups";

        private static readonly HashSet<string> KnownOperationNames = BuildKnownOperationNames();
        private static readonly ReadOnlyCollection<string> KnownOperationNamesOrdered = Array.AsReadOnly(KnownOperationNames
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray());

        private static HashSet<string> BuildKnownOperationNames()
        {
            var values = new HashSet<string>(StringComparer.Ordinal);
            var ownerByValue = new Dictionary<string, string>(StringComparer.Ordinal);
            var fields = typeof(DocumentHostOperationNames).GetFields(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            foreach (var field in fields)
            {
                if (!field.IsLiteral || field.IsInitOnly || field.FieldType != typeof(string))
                    continue;
                if (field.GetCustomAttribute<DocumentHostOperationNameAttribute>() == null)
                    continue;

                string? value = field.GetRawConstantValue() as string;
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                if (ownerByValue.TryGetValue(value, out string? existingField))
                {
                    throw new InvalidOperationException(
                        $"Duplicate operation-name value '{value}' in constants '{existingField}' and '{field.Name}'.");
                }

                ownerByValue[value] = field.Name;
                values.Add(value);
            }

            if (values.Count == 0)
                throw new InvalidOperationException("No document-host operation-name constants were discovered.");

            return values;
        }

        internal static void EnsureKnown(string operationName)
        {
            ArgumentNullException.ThrowIfNull(operationName);
            if (string.IsNullOrWhiteSpace(operationName))
                throw new ArgumentException("Operation name cannot be empty or whitespace.", nameof(operationName));
            if (!KnownOperationNames.Contains(operationName))
            {
                throw new ArgumentException(
                    $"Unknown document-host operation name '{operationName}'. Add it to {nameof(DocumentHostOperationNames)} first.",
                    nameof(operationName));
            }
        }

        internal static IReadOnlyList<string> GetAll()
            => KnownOperationNamesOrdered;
    }
}
