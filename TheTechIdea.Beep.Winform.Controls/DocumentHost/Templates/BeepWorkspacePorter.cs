// BeepWorkspacePorter.cs
// Static helpers for exporting and importing workspace definitions.
// Supports: JSON string, ZIP file (System.IO.Compression), clipboard (Base64).
// No external NuGet packages required — System.IO.Compression ships with .NET 8+.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost
{
    /// <summary>
    /// Result of a workspace import operation.
    /// </summary>
    public sealed class WorkspaceImportResult
    {
        /// <summary>Successfully imported workspaces.</summary>
        public IReadOnlyList<WorkspaceDefinition> Workspaces { get; init; } =
            Array.Empty<WorkspaceDefinition>();

        /// <summary>Validation warnings (non-fatal) encountered during import.</summary>
        public IReadOnlyList<string> Warnings { get; init; } = Array.Empty<string>();

        /// <summary>Whether any workspaces were successfully imported.</summary>
        public bool Success => Workspaces.Count > 0;
    }

    /// <summary>
    /// Static utility for exporting and importing <see cref="WorkspaceDefinition"/> objects
    /// as JSON strings, ZIP archives, or clipboard Base64 payloads.
    /// </summary>
    public static class BeepWorkspacePorter
    {
        private const string ManifestEntryName = "workspaces.json";
        private const string ClipboardPrefix   = "BEEP-WS-V1:";

        private static readonly JsonSerializerOptions _json = new()
        {
            WriteIndented            = true,
            PropertyNameCaseInsensitive = true
        };

        // ── Export ────────────────────────────────────────────────────────────

        /// <summary>Serialises a single workspace to a JSON string.</summary>
        public static string ExportToJson(WorkspaceDefinition workspace)
        {
            ArgumentNullException.ThrowIfNull(workspace);
            return JsonSerializer.Serialize(workspace, _json);
        }

        /// <summary>Serialises a collection of workspaces to a JSON array string.</summary>
        public static string ExportToJson(IEnumerable<WorkspaceDefinition> workspaces)
        {
            ArgumentNullException.ThrowIfNull(workspaces);
            return JsonSerializer.Serialize(workspaces.ToList(), _json);
        }

        /// <summary>
        /// Writes one or more workspaces to a ZIP file at <paramref name="filePath"/>.
        /// The archive contains a single <c>workspaces.json</c> manifest.
        /// </summary>
        public static void ExportToZip(
            IEnumerable<WorkspaceDefinition> workspaces, string filePath)
        {
            ArgumentNullException.ThrowIfNull(workspaces);
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path must not be empty.", nameof(filePath));

            var list = workspaces.ToList();
            var manifest = JsonSerializer.Serialize(list, _json);

            using var archive = ZipFile.Open(filePath, ZipArchiveMode.Create);
            var entry         = archive.CreateEntry(ManifestEntryName, CompressionLevel.Optimal);
            using var writer  = new StreamWriter(entry.Open(), Encoding.UTF8);
            writer.Write(manifest);
        }

        /// <summary>
        /// Encodes a workspace as a Base64 string suitable for clipboard sharing.
        /// Returns a string prefixed with <c>BEEP-WS-V1:</c>.
        /// </summary>
        public static string ExportToClipboardString(WorkspaceDefinition workspace)
        {
            ArgumentNullException.ThrowIfNull(workspace);
            var json    = ExportToJson(workspace);
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            return ClipboardPrefix + encoded;
        }

        /// <summary>Copies a workspace to the system clipboard as a Base64 string.</summary>
        public static void CopyToClipboard(WorkspaceDefinition workspace)
        {
            var text = ExportToClipboardString(workspace);
            Clipboard.SetText(text);
        }

        // ── Import ────────────────────────────────────────────────────────────

        /// <summary>
        /// Deserialises one or more workspaces from a JSON string.
        /// Accepts both a single-object JSON and a JSON array.
        /// </summary>
        public static WorkspaceImportResult ImportFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return Fail("JSON string is empty.");

            try
            {
                var warnings = new List<string>();
                var list     = new List<WorkspaceDefinition>();

                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var el in doc.RootElement.EnumerateArray())
                    {
                        var ws = ParseWorkspace(el.GetRawText(), warnings);
                        if (ws != null) list.Add(ws);
                    }
                }
                else
                {
                    var ws = ParseWorkspace(json, warnings);
                    if (ws != null) list.Add(ws);
                }

                return new WorkspaceImportResult { Workspaces = list, Warnings = warnings };
            }
            catch (Exception ex)
            {
                return Fail($"JSON parse error: {ex.Message}");
            }
        }

        /// <summary>
        /// Reads workspaces from a ZIP file previously created by
        /// <see cref="ExportToZip"/>.
        /// </summary>
        public static WorkspaceImportResult ImportFromZip(string filePath)
        {
            if (!File.Exists(filePath))
                return Fail($"File not found: {filePath}");

            try
            {
                using var archive = ZipFile.OpenRead(filePath);
                var entry         = archive.GetEntry(ManifestEntryName);
                if (entry == null)
                    return Fail($"Archive does not contain '{ManifestEntryName}'.");

                using var reader = new StreamReader(entry.Open(), Encoding.UTF8);
                var json         = reader.ReadToEnd();
                return ImportFromJson(json);
            }
            catch (Exception ex)
            {
                return Fail($"ZIP read error: {ex.Message}");
            }
        }

        /// <summary>
        /// Decodes a workspace from a Base64 clipboard string created by
        /// <see cref="ExportToClipboardString"/>.
        /// </summary>
        public static WorkspaceImportResult ImportFromClipboardString(string clipboardText)
        {
            if (string.IsNullOrWhiteSpace(clipboardText))
                return Fail("Clipboard string is empty.");

            if (!clipboardText.StartsWith(ClipboardPrefix, StringComparison.Ordinal))
                return Fail($"String does not start with expected prefix '{ClipboardPrefix}'.");

            try
            {
                var encoded = clipboardText[ClipboardPrefix.Length..];
                var json    = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                return ImportFromJson(json);
            }
            catch (Exception ex)
            {
                return Fail($"Clipboard decode error: {ex.Message}");
            }
        }

        /// <summary>Reads a workspace from the system clipboard (if it contains a valid payload).</summary>
        public static WorkspaceImportResult PasteFromClipboard()
        {
            if (!Clipboard.ContainsText())
                return Fail("Clipboard does not contain text.");
            return ImportFromClipboardString(Clipboard.GetText());
        }

        // ── Merge helper ──────────────────────────────────────────────────────

        /// <summary>
        /// Merges imported workspaces into <paramref name="manager"/>.
        /// Existing workspaces with the same id are overwritten if
        /// <paramref name="overwriteExisting"/> is <c>true</c>.
        /// </summary>
        public static void MergeInto(
            WorkspaceImportResult result,
            WorkspaceManager      manager,
            bool                  overwriteExisting = false)
        {
            ArgumentNullException.ThrowIfNull(result);
            ArgumentNullException.ThrowIfNull(manager);

            foreach (var ws in result.Workspaces)
            {
                var existing = manager.FindById(ws.Id);
                if (existing == null || overwriteExisting)
                    manager.Save(ws.Name, ws.LayoutJson);
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static WorkspaceDefinition? ParseWorkspace(string json, List<string> warnings)
        {
            try
            {
                var ws = JsonSerializer.Deserialize<WorkspaceDefinition>(json, _json);
                if (ws == null) { warnings.Add("Skipped a null workspace entry."); return null; }
                if (string.IsNullOrWhiteSpace(ws.Name))  { warnings.Add($"Workspace {ws.Id} has no name."); ws.Name = "Imported"; }
                if (string.IsNullOrWhiteSpace(ws.LayoutJson)) warnings.Add($"Workspace '{ws.Name}' has no layout JSON.");
                return ws;
            }
            catch (Exception ex)
            {
                warnings.Add($"Skipped malformed workspace entry: {ex.Message}");
                return null;
            }
        }

        private static WorkspaceImportResult Fail(string message) =>
            new() { Workspaces = Array.Empty<WorkspaceDefinition>(), Warnings = new[] { message } };
    }
}
