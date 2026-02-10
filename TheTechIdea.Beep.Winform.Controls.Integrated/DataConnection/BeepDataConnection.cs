
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using TheTechIdea.Beep.ConfigUtil;
using TheTechIdea.Beep.Container.Services;
using TheTechIdea.Beep.Services;
using TheTechIdea.Beep.Utilities;
using TheTechIdea.Beep.Winform.Controls.Converters;

namespace TheTechIdea.Beep.Winform.Controls
{
    public class BeepDataConnection : Component
    {
        private IBeepService? _beepService;
        private BeepConnectionRepository? _connectionRepository;

        public IBeepService? BeepService => _beepService;
        public event EventHandler? ConnectionsChanged;

        public BeepDataConnection()
        {
            InitializeBeepService();
            DataConnections = new List<ConnectionProperties>();
            InitializeRepository();
            ReloadConnections();
        }

        [Browsable(true)]
        [Category("Connections")]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        public List<ConnectionProperties> DataConnections { get; set; }

        [Browsable(true)]
        [Category("Current Connection")]
        [TypeConverter(typeof(DataConnectionConverter))]
        public ConnectionProperties? CurrentConnection { get; set; }

        /// <summary>
        /// Reloads connections from shared config when available; otherwise keeps local list.
        /// </summary>
        public IReadOnlyList<ConnectionProperties> ReloadConnections()
        {
            if (_connectionRepository != null)
            {
                SetLocalConnections(_connectionRepository.LoadConnections());
            }
            else if (IsInDesignTime())
            {
                LoadDesignTimeConnections();
            }

            return DataConnections.AsReadOnly();
        }

        /// <summary>
        /// Adds or updates a connection and persists it to DataConnections.json when service is available.
        /// </summary>
        public bool AddOrUpdateConnection(ConnectionProperties connection, bool persist = true)
        {
            if (connection == null || string.IsNullOrWhiteSpace(connection.ConnectionName))
            {
                return false;
            }

            bool changed;
            if (_connectionRepository != null)
            {
                changed = _connectionRepository.AddOrUpdate(connection, persist);
                if (changed)
                {
                    SetLocalConnections(_connectionRepository.LoadConnections());
                }
            }
            else
            {
                changed = AddOrUpdateLocalConnection(connection);
            }

            if (changed)
            {
                ConnectionsChanged?.Invoke(this, EventArgs.Empty);
            }

            return changed;
        }

        /// <summary>
        /// Removes a connection by name and persists changes when service is available.
        /// </summary>
        public bool RemoveConnection(string connectionName, bool persist = true)
        {
            if (string.IsNullOrWhiteSpace(connectionName))
            {
                return false;
            }

            bool removed;
            if (_connectionRepository != null)
            {
                removed = _connectionRepository.Remove(connectionName, persist);
                if (removed)
                {
                    SetLocalConnections(_connectionRepository.LoadConnections());
                }
            }
            else
            {
                removed = RemoveLocalConnection(connectionName);
            }

            if (removed)
            {
                ConnectionsChanged?.Invoke(this, EventArgs.Empty);
            }

            return removed;
        }

        /// <summary>
        /// Persists the current local connection list to shared configuration.
        /// </summary>
        public bool SaveConnections()
        {
            if (_connectionRepository == null)
            {
                return false;
            }

            var saved = _connectionRepository.Save(new List<ConnectionProperties>(DataConnections));
            if (saved)
            {
                SetLocalConnections(_connectionRepository.LoadConnections());
                ConnectionsChanged?.Invoke(this, EventArgs.Empty);
            }

            return saved;
        }

        /// <summary>
        /// Initializes the BeepService for design-time or runtime use.
        /// </summary>
        private void InitializeBeepService()
        {
            try
            {
                var service = new BeepService();

                if (IsInDesignTime())
                {
                    // BeepService default constructor configures design-time automatically.
                    _beepService = service;
                }
                else
                {
                    service.Configure(AppContext.BaseDirectory, "RuntimeContainer", BeepConfigType.DataConnector, false);
                    _beepService = service;
                }
            }
            catch
            {
                _beepService = null;
            }
        }

        private void InitializeRepository()
        {
            if (_beepService == null)
            {
                return;
            }

            _connectionRepository = new BeepConnectionRepository(_beepService);
            _connectionRepository.ConnectionsChanged += (_, _) =>
            {
                SetLocalConnections(_connectionRepository.LoadConnections());
                ConnectionsChanged?.Invoke(this, EventArgs.Empty);
            };
        }

        /// <summary>
        /// Loads design-time connections from the BeepService configuration.
        /// </summary>
        private void LoadDesignTimeConnections()
        {
            var configEditor = _beepService?.Config_editor;
            if (configEditor == null)
            {
                return;
            }

            var loaded = configEditor.LoadDataConnectionsValues() ?? configEditor.DataConnections;
            SetLocalConnections(loaded);
        }

        private void SetLocalConnections(IEnumerable<ConnectionProperties>? connections)
        {
            DataConnections.Clear();
            if (connections != null)
            {
                DataConnections.AddRange(connections);
            }

            if (CurrentConnection == null ||
                !DataConnections.Any(c =>
                    !string.IsNullOrWhiteSpace(c.GuidID) &&
                    !string.IsNullOrWhiteSpace(CurrentConnection.GuidID) &&
                    string.Equals(c.GuidID, CurrentConnection.GuidID, StringComparison.OrdinalIgnoreCase)))
            {
                CurrentConnection = DataConnections.FirstOrDefault();
            }
        }

        private bool AddOrUpdateLocalConnection(ConnectionProperties connection)
        {
            if (string.IsNullOrWhiteSpace(connection.GuidID))
            {
                connection.GuidID = Guid.NewGuid().ToString("D");
            }

            var existing = DataConnections.FirstOrDefault(c =>
                (!string.IsNullOrWhiteSpace(c.GuidID) &&
                 string.Equals(c.GuidID, connection.GuidID, StringComparison.OrdinalIgnoreCase)) ||
                string.Equals(c.ConnectionName, connection.ConnectionName, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                DataConnections.Add(connection);
                if (CurrentConnection == null)
                {
                    CurrentConnection = connection;
                }

                return true;
            }

            var index = DataConnections.IndexOf(existing);
            DataConnections[index] = connection;
            CurrentConnection = connection;
            return true;
        }

        private bool RemoveLocalConnection(string connectionName)
        {
            var existing = DataConnections.FirstOrDefault(c =>
                !string.IsNullOrWhiteSpace(c.ConnectionName) &&
                string.Equals(c.ConnectionName, connectionName, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                return false;
            }

            DataConnections.Remove(existing);
            if (CurrentConnection == existing)
            {
                CurrentConnection = DataConnections.FirstOrDefault();
            }

            return true;
        }

        /// <summary>
        /// Determines whether the current context is design-time.
        /// </summary>
        private static bool IsInDesignTime()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }
    }
}
