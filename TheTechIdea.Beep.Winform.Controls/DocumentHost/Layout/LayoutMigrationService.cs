// LayoutMigrationService.cs
// Upgrades saved layout JSON from older schema versions to the current v2 format.
// Designed as a stateless service — all methods are static.
// ─────────────────────────────────────────────────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TheTechIdea.Beep.Winform.Controls.DocumentHost.Layout
{
    /// <summary>
    /// Migrates saved layout JSON payloads produced by older schema versions
    /// to the current <b>v2</b> format that includes split groups, floating
    /// window geometry, and auto-hide state.
    /// </summary>
    public static class LayoutMigrationService
    {
        // ─────────────────────────────────────────────────────────────────────
        // Public entry point
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Inspects the <c>schemaVersion</c> field in <paramref name="json"/> and
        /// applies any required upgrade transformations in order, returning a
        /// payload that conforms to schema v2.
        /// </summary>
        /// <param name="json">Raw JSON string from disk or elsewhere.</param>
        /// <returns>A v2-compatible JSON string, or the original string if it is
        /// already at v2 (or newer).</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="json"/> cannot be parsed as JSON.
        /// </exception>
        public static string MigrateToLatest(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON must not be null or empty.", nameof(json));

            JsonNode? root;
            try
            {
                root = JsonNode.Parse(json);
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("Invalid JSON payload.", nameof(json), ex);
            }

            if (root is not JsonObject obj)
                throw new ArgumentException("JSON root must be an object.", nameof(json));

            int version = obj["schemaVersion"]?.GetValue<int>() ?? 0;

            if (version < 1)
                obj = UpgradeFromV0(obj);

            if (version < 2)
                obj = UpgradeFromV1ToV2(obj);

            // Future: add UpgradeFromV2ToV3 here as needed.

            return obj.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Returns <c>true</c> if the given JSON string is already at schema v2.
        /// </summary>
        public static bool IsCurrentVersion(string json)
        {
            try
            {
                var root = JsonNode.Parse(json);
                return (root?["schemaVersion"]?.GetValue<int>() ?? 0) >= 2;
            }
            catch { return false; }
        }

        // ─────────────────────────────────────────────────────────────────────
        // v0 → v1  (pre-release / malformed — just add required fields)
        // ─────────────────────────────────────────────────────────────────────

        private static JsonObject UpgradeFromV0(JsonObject v0)
        {
            var v1 = new JsonObject();
            v1["schemaVersion"] = 1;
            v1["activeId"]      = v0["activeId"]     ?? v0["ActiveId"];
            v1["scrollOffset"]  = v0["scrollOffset"] ?? v0["ScrollOffset"] ?? 0;

            // Normalise array name (old code used PascalCase)
            var tabs = v0["tabs"] ?? v0["Tabs"];
            v1["tabs"] = tabs ?? new JsonArray();

            return v1;
        }

        // ─────────────────────────────────────────────────────────────────────
        // v1 → v2  (tabs-only → full layout tree + floats + auto-hide)
        // ─────────────────────────────────────────────────────────────────────

        private static JsonObject UpgradeFromV1ToV2(JsonObject v1)
        {
            // Grab the flat tab list from v1
            var tabs = v1["tabs"] as JsonArray ?? new JsonArray();
            string activeId = v1["activeId"]?.GetValue<string>() ?? string.Empty;

            // Build a single-group layout tree that contains all the tabs
            var groupId   = Guid.NewGuid().ToString();
            var docArray  = new JsonArray();

            foreach (var tab in tabs)
            {
                if (tab is not JsonObject t) continue;

                var doc = new JsonObject
                {
                    ["id"]         = t["id"]        ?? t["Id"],
                    ["title"]      = t["title"]     ?? t["Title"],
                    ["iconPath"]   = t["iconPath"]  ?? t["IconPath"],
                    ["isPinned"]   = t["isPinned"]  ?? t["IsPinned"]   ?? false,
                    ["isModified"] = t["isModified"]?? t["IsModified"] ?? false,
                    ["customData"] = new JsonObject()
                };
                docArray.Add(doc);
            }

            var groupNode = new JsonObject
            {
                ["type"]               = "tabGroup",
                ["groupId"]            = groupId,
                ["selectedDocumentId"] = activeId,
                ["documents"]          = docArray
            };

            // v2 root object
            var v2 = new JsonObject
            {
                ["schemaVersion"]      = 2,
                ["activeDocumentId"]   = activeId,
                ["layoutTree"]         = groupNode,
                ["floatingWindows"]    = new JsonArray(),
                ["autoHideEntries"]    = new JsonArray(),
                ["mruSnapshot"]        = new JsonArray()
            };

            return v2;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // LayoutRestoreReport
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Diagnostic report returned by <see cref="BeepDocumentHost.TryRestoreLayout"/>.
    /// Describes which documents were restored, which were skipped, and any failures.
    /// </summary>
    public sealed class LayoutRestoreReport
    {
        /// <summary>IDs of documents that were successfully restored.</summary>
        public List<string> Restored { get; } = new List<string>();

        /// <summary>
        /// IDs of documents that were skipped — either because a
        /// <see cref="BeepDocumentHost.LayoutRestoring"/> handler cancelled them,
        /// or because they were already open.
        /// </summary>
        public List<string> Skipped { get; } = new List<string>();

        /// <summary>
        /// IDs of documents that failed to restore, paired with a short reason message.
        /// </summary>
        public List<(string Id, string Reason)> Failed { get; } = new List<(string, string)>();

        /// <summary>
        /// <c>true</c> when <see cref="Failed"/> is empty.  Skipped entries are not failures.
        /// </summary>
        public bool IsSuccess => Failed.Count == 0;

        /// <summary>The schema version of the JSON that was restored.</summary>
        public int SchemaVersion { get; internal set; }

        /// <summary>Whether a migration was applied before restoring (v1 → v2).</summary>
        public bool WasMigrated { get; internal set; }

        /// <summary>
        /// Human-readable summary for logging.
        /// </summary>
        public override string ToString()
            => $"LayoutRestoreReport: {Restored.Count} restored, {Skipped.Count} skipped, " +
               $"{Failed.Count} failed (schema v{SchemaVersion}{(WasMigrated ? ", migrated" : "")}).";
    }
}
