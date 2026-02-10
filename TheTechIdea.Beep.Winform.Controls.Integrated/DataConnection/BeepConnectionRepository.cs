using System;
using System.Collections.Generic;
using System.Linq;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Services;

namespace TheTechIdea.Beep.Winform.Controls
{
    /// <summary>
    /// Centralized connection persistence helper backed by IBeepService.Config_editor.
    /// Keeps connection CRUD and save/reload behavior consistent across forms.
    /// </summary>
    public sealed class BeepConnectionRepository
    {
        private readonly IBeepService _beepService;
        private readonly object _syncRoot = new();

        public event EventHandler? ConnectionsChanged;

        public BeepConnectionRepository(IBeepService beepService)
        {
            _beepService = beepService ?? throw new ArgumentNullException(nameof(beepService));
        }

        public IReadOnlyList<ConnectionProperties> LoadConnections()
        {
            lock (_syncRoot)
            {
                var configEditor = _beepService.Config_editor;
                if (configEditor == null)
                {
                    return Array.Empty<ConnectionProperties>();
                }

                var connections = configEditor.LoadDataConnectionsValues() ?? new List<ConnectionProperties>();
                configEditor.DataConnections = connections;
                return new List<ConnectionProperties>(connections);
            }
        }

        public bool AddOrUpdate(ConnectionProperties connection, bool persist = true)
        {
            if (connection == null || string.IsNullOrWhiteSpace(connection.ConnectionName))
            {
                return false;
            }

            lock (_syncRoot)
            {
                var configEditor = _beepService.Config_editor;
                if (configEditor == null)
                {
                    return false;
                }

                EnsureConnectionDefaults(connection);

                var existing = FindExisting(configEditor.DataConnections, connection);
                bool changed;

                if (existing == null)
                {
                    changed = configEditor.AddDataConnection(connection);
                }
                else if (!string.IsNullOrWhiteSpace(existing.GuidID))
                {
                    changed = configEditor.UpdateDataConnection(connection, existing.GuidID);
                }
                else
                {
                    // Fallback path for older entries with empty GuidID.
                    configEditor.RemoveConnByName(existing.ConnectionName);
                    changed = configEditor.AddDataConnection(connection);
                }

                if (!changed)
                {
                    return false;
                }

                if (persist)
                {
                    configEditor.SaveDataconnectionsValues();
                    configEditor.LoadDataConnectionsValues();
                }

                ConnectionsChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
        }

        public bool Remove(string connectionName, bool persist = true)
        {
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                return false;
            }

            lock (_syncRoot)
            {
                var configEditor = _beepService.Config_editor;
                if (configEditor == null)
                {
                    return false;
                }

                var removed = configEditor.RemoveDataConnection(connectionName);
                if (!removed)
                {
                    return false;
                }

                if (persist)
                {
                    configEditor.SaveDataconnectionsValues();
                    configEditor.LoadDataConnectionsValues();
                }

                ConnectionsChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
        }

        public bool Save(List<ConnectionProperties> connections)
        {
            lock (_syncRoot)
            {
                var configEditor = _beepService.Config_editor;
                if (configEditor == null)
                {
                    return false;
                }

                configEditor.DataConnections = connections ?? new List<ConnectionProperties>();
                configEditor.SaveDataconnectionsValues();
                configEditor.LoadDataConnectionsValues();

                ConnectionsChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
        }

        private static ConnectionProperties? FindExisting(IEnumerable<ConnectionProperties>? connections, ConnectionProperties candidate)
        {
            if (connections == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(candidate.GuidID))
            {
                var byGuid = connections.FirstOrDefault(c =>
                    !string.IsNullOrWhiteSpace(c.GuidID) &&
                    string.Equals(c.GuidID, candidate.GuidID, StringComparison.OrdinalIgnoreCase));
                if (byGuid != null)
                {
                    return byGuid;
                }
            }

            return connections.FirstOrDefault(c =>
                !string.IsNullOrWhiteSpace(c.ConnectionName) &&
                string.Equals(c.ConnectionName, candidate.ConnectionName, StringComparison.OrdinalIgnoreCase));
        }

        private static void EnsureConnectionDefaults(ConnectionProperties connection)
        {
            if (string.IsNullOrWhiteSpace(connection.GuidID))
            {
                connection.GuidID = Guid.NewGuid().ToString("D");
            }
        }
    }
}
