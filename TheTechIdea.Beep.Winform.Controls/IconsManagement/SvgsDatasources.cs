using Svg;
using System.IO;
using System.Reflection;
 
using TheTechIdea.Beep.Winform.Controls;

namespace TheTechIdea.Beep.Icons
{
    /// <summary>
    /// Static class providing easy access to all embedded SVG image paths in the Beep.Winform.Controls assembly.
    /// All paths are formatted as embedded resource names for use with Assembly.GetManifestResourceStream().
    /// Datasource Icons for databases, APIs, cloud services, and data management platforms.
    /// </summary>
    public static class SvgsDatasources
    {
        private const string BaseNamespace = "TheTechIdea.Beep.Winform.Controls.GFX.Icons.Datasources";

        /// <summary>
        /// Gets the assembly containing the embedded SVG resources.
        /// </summary>
        public static Assembly ResourceAssembly => Assembly.GetExecutingAssembly();

        #region "Relational Databases"
        public static readonly string Db2 = $"{BaseNamespace}.db2.svg";
        public static readonly string H2Database = $"{BaseNamespace}.h2database.svg";
        public static readonly string Mariadb = $"{BaseNamespace}.mariadb.svg";
        public static readonly string Mysql = $"{BaseNamespace}.mysql.svg";
        public static readonly string Oracle = $"{BaseNamespace}.oracle.svg";
        public static readonly string Postgre = $"{BaseNamespace}.postgre.svg";
        public static readonly string Postgresql = $"{BaseNamespace}.postgresql.svg";
        public static readonly string Sqlserver = $"{BaseNamespace}.sqlserver.svg";
        public static readonly string SqlLite = $"{BaseNamespace}.sqllite.svg";
        public static readonly string Sqlite = $"{BaseNamespace}.sql.svg";
        public static readonly string Vertica = $"{BaseNamespace}.vertica.svg";
        public static readonly string VistaDb = $"{BaseNamespace}.vistadb.svg";
        #endregion

        #region "NoSQL Databases"
        public static readonly string Arangodb = $"{BaseNamespace}.arangodb.svg";
        public static readonly string Cassandra = $"{BaseNamespace}.cassandra.svg";
        public static readonly string Chromadb = $"{BaseNamespace}.chromadb.svg";
        public static readonly string Cockroach = $"{BaseNamespace}.cockroach.svg";
        public static readonly string Cockroachdb = $"{BaseNamespace}.cockroachdb.svg";
        public static readonly string Couchbase = $"{BaseNamespace}.couchbase.svg";
        public static readonly string Couchdb = $"{BaseNamespace}.couchdb.svg";
        public static readonly string Documentdb = $"{BaseNamespace}.documentdb.svg";
        public static readonly string Dynamodb = $"{BaseNamespace}.awsrds.svg";
        public static readonly string Elasticsearch = $"{BaseNamespace}.elasticsearch.svg";
        public static readonly string Graphdb = $"{BaseNamespace}.graphdb.svg";
        public static readonly string Hbase = $"{BaseNamespace}.hbase.svg";
        public static readonly string Janusgraph = $"{BaseNamespace}.janusgraph.svg";
        public static readonly string Litedb = $"{BaseNamespace}.litedb.svg";
        public static readonly string Mongodb = $"{BaseNamespace}.mongodb.svg";
        public static readonly string Milvus = $"{BaseNamespace}.milvus.svg";
        public static readonly string Nosql = $"{BaseNamespace}.nosql.svg";
        public static readonly string Orientdb = $"{BaseNamespace}.orientdb.svg";
        public static readonly string Pinecone = $"{BaseNamespace}.pinecone.svg";
        public static readonly string Qdrant = $"{BaseNamespace}.qdrant.svg";
        public static readonly string Realm = $"{BaseNamespace}.realm.svg";
        public static readonly string Redis = $"{BaseNamespace}.redis.svg";
        public static readonly string Redisvector = $"{BaseNamespace}.redisvector.svg";
        public static readonly string Solr = $"{BaseNamespace}.solr.svg";
        public static readonly string Tigergraph = $"{BaseNamespace}.tigergraph.svg";
        public static readonly string Vectordb = $"{BaseNamespace}.vectordb.svg";
        public static readonly string Zilliz = $"{BaseNamespace}.zilliz.svg";
        #endregion

        #region "Time-Series & Analytics Databases"
        public static readonly string Clickhouse = $"{BaseNamespace}.clickhouse.svg";
        public static readonly string Druid = $"{BaseNamespace}.druid.svg";
        public static readonly string Duckdb = $"{BaseNamespace}.duckdb.svg";
        public static readonly string Hadoop = $"{BaseNamespace}.hadoop.svg";
        public static readonly string Influxdb = $"{BaseNamespace}.influxdb.svg";
        public static readonly string Presto = $"{BaseNamespace}.presto.svg";
        public static readonly string Timescale = $"{BaseNamespace}.timescale.svg";
        public static readonly string Timeseriesdb = $"{BaseNamespace}.timeseriesdb.svg";
        public static readonly string Trino = $"{BaseNamespace}.trino.svg";
        #endregion

        #region "Search & Indexing"
        public static readonly string Kafka = $"{BaseNamespace}.kafka.svg";
        public static readonly string Searchengine = $"{BaseNamespace}.searchengine.svg";
        public static readonly string Solr2 = $"{BaseNamespace}.solr.svg";
        #endregion

        #region "Big Data & Cloud Analytics"
        public static readonly string BigData = $"{BaseNamespace}.bigdata.svg";
        public static readonly string Databricks = $"{BaseNamespace}.awsglue.svg";
        public static readonly string Googleanalytics = $"{BaseNamespace}.googleanalytics.svg";
        public static readonly string Googlebigquery = $"{BaseNamespace}.googlebigquery.svg";
        public static readonly string Snowflake = $"{BaseNamespace}.snowflake.svg";
        public static readonly string Spanner = $"{BaseNamespace}.spanner.svg";
        #endregion

        #region "AWS Services"
        public static readonly string Awsathena = $"{BaseNamespace}.awsathena.svg";
        public static readonly string Awsglue = $"{BaseNamespace}.awsglue.svg";
        public static readonly string Awskinesis = $"{BaseNamespace}.awskinesis.svg";
        public static readonly string Awsrds = $"{BaseNamespace}.awsrds.svg";
        public static readonly string Awssns = $"{BaseNamespace}.awssns.svg";
        public static readonly string Awssqs = $"{BaseNamespace}.awssqs.svg";
        #endregion

        #region "Azure Services"
        public static readonly string Azureiothub = $"{BaseNamespace}.azureiothub.svg";
        public static readonly string Azureservicebus = $"{BaseNamespace}.azureservicebus.svg";
        public static readonly string Azuresql = $"{BaseNamespace}.azuresql.svg";
        #endregion

        #region "Google Cloud Services"
        public static readonly string Googlecloudstorage = $"{BaseNamespace}.googlecloudstorage.svg";
        public static readonly string Googledocs = $"{BaseNamespace}.googledocs.svg";
        public static readonly string Googledrive = $"{BaseNamespace}.googledrive.svg";
        public static readonly string Googlesheets = $"{BaseNamespace}.googlesheets.svg";
        public static readonly string Googleslides = $"{BaseNamespace}.googleslides.svg";
        public static readonly string Googlechat = $"{BaseNamespace}.googlechat.svg";
        public static readonly string Googleads = $"{BaseNamespace}.googleads.svg";
        #endregion

        #region "Data Warehousing & Lakes"
        public static readonly string Datalake = $"{BaseNamespace}.datalake.svg";
        public static readonly string Datamart = $"{BaseNamespace}.datamart.svg";
        public static readonly string Datamesh = $"{BaseNamespace}.datamesh.svg";
        public static readonly string Datawarehouse = $"{BaseNamespace}.datawarehouse.svg";
        public static readonly string Datafabric = $"{BaseNamespace}.datafabric.svg";
        #endregion

        #region "Data Processing & Pipelines"
        public static readonly string Datapipeline = $"{BaseNamespace}.datapipeline.svg";
        public static readonly string Streamprocessing = $"{BaseNamespace}.streamprocessing.svg";
        public static readonly string Airflow = $"{BaseNamespace}.apacheignite.svg";
        public static readonly string Apacheignite = $"{BaseNamespace}.apacheignite.svg";
        public static readonly string Apachestorm = $"{BaseNamespace}.apachestorm.svg";
        public static readonly string Pulsar = $"{BaseNamespace}.pulsar.svg";
        #endregion

        #region "File Formats & Serialization"
        public static readonly string Avro = $"{BaseNamespace}.avro.svg";
        public static readonly string Csv = $"{BaseNamespace}.csv.svg";
        public static readonly string Doc = $"{BaseNamespace}.doc.svg";
        public static readonly string Docx = $"{BaseNamespace}.docx.svg";
        public static readonly string Dicom = $"{BaseNamespace}.dicom.svg";
        public static readonly string Graphfile = $"{BaseNamespace}.graphfile.svg";
        public static readonly string Graphml = $"{BaseNamespace}.graphml.svg";
        public static readonly string Html = $"{BaseNamespace}.html.svg";
        public static readonly string Ini = $"{BaseNamespace}.ini.svg";
        public static readonly string Json = $"{BaseNamespace}.json.svg";
        public static readonly string Jsonrpc = $"{BaseNamespace}.jsonrpc.svg";
        public static readonly string Las = $"{BaseNamespace}.las.svg";
        public static readonly string Log = $"{BaseNamespace}.log.svg";
        public static readonly string Markdown = $"{BaseNamespace}.markdown.svg";
        public static readonly string Orc = $"{BaseNamespace}.orc.svg";
        public static readonly string Parquet = $"{BaseNamespace}.parquet.svg";
        public static readonly string Ppt = $"{BaseNamespace}.ppt.svg";
        public static readonly string Pptx = $"{BaseNamespace}.pptx.svg";
        public static readonly string Recordio = $"{BaseNamespace}.recordio.svg";
        public static readonly string Text = $"{BaseNamespace}.text.svg";
        public static readonly string Tfrecord = $"{BaseNamespace}.tfrecord.svg";
        public static readonly string Tsv = $"{BaseNamespace}.tsv.svg";
        public static readonly string Xml = $"{BaseNamespace}.xml.svg";
        public static readonly string Xmlrpc = $"{BaseNamespace}.xmlrpc.svg";
        public static readonly string Xls = $"{BaseNamespace}.xls.svg";
        public static readonly string Xlsx = $"{BaseNamespace}.xlsx.svg";
        public static readonly string Yaml = $"{BaseNamespace}.yaml.svg";
        #endregion

        #region "APIs & Web Services"
        public static readonly string Graphql = $"{BaseNamespace}.graphql.svg";
        public static readonly string Rest = $"{BaseNamespace}.webapi.svg";
        public static readonly string Soap = $"{BaseNamespace}.soap.svg";
        public static readonly string Webapi = $"{BaseNamespace}.webapi.svg";
        public static readonly string Websocket = $"{BaseNamespace}.websocket.svg";
        #endregion

        #region "Network & Protocols"
        public static readonly string Ftp = $"{BaseNamespace}.ftp.svg";
        public static readonly string Sftp = $"{BaseNamespace}.sftp.svg";
        public static readonly string Imap = $"{BaseNamespace}.imap.svg";
        public static readonly string Pop3 = $"{BaseNamespace}.pop3.svg";
        public static readonly string Smtp = $"{BaseNamespace}.smtp.svg";
        public static readonly string Mqtt = $"{BaseNamespace}.iot.svg";
        public static readonly string Zeromq = $"{BaseNamespace}.zeromq.svg";
        public static readonly string Nats = $"{BaseNamespace}.nats.svg";
        public static readonly string Sse = $"{BaseNamespace}.sse.svg";
        #endregion

        #region "CRM & Business Applications"
        public static readonly string Salesforce = $"{BaseNamespace}.salesforce.svg";
        public static readonly string Sapcrm = $"{BaseNamespace}.sapcrm.svg";
        public static readonly string Sugarcrm = $"{BaseNamespace}.sugarcrm.svg";
        public static readonly string Hubspot = $"{BaseNamespace}.hubspot.svg";
        public static readonly string Pipedrive = $"{BaseNamespace}.pipedrive.svg";
        public static readonly string Freshsales = $"{BaseNamespace}.freshsales.svg";
        public static readonly string Zoho = $"{BaseNamespace}.zoho.svg";
        public static readonly string Zohodesk = $"{BaseNamespace}.zohodesk.svg";
        public static readonly string Zohobooks = $"{BaseNamespace}.zohobooks.svg";
        public static readonly string Dynamicsgp = $"{BaseNamespace}.microsoft.svg";
        public static readonly string Myob = $"{BaseNamespace}.myob.svg";
        public static readonly string Nutshell = $"{BaseNamespace}.nutshell.svg";
        public static readonly string Copper = $"{BaseNamespace}.copper.svg";
        #endregion

        #region "Marketing & Analytics Platforms"
        public static readonly string Mailchimp = $"{BaseNamespace}.mailchimp.svg";
        public static readonly string Hubspot2 = $"{BaseNamespace}.hubspot.svg";
        public static readonly string Marketo = $"{BaseNamespace}.awsglue.svg";
        public static readonly string Klaviyo = $"{BaseNamespace}.awsglue.svg";
        public static readonly string Googleads2 = $"{BaseNamespace}.googleads.svg";
        public static readonly string Googleanalytics2 = $"{BaseNamespace}.googleanalytics.svg";
        public static readonly string Hootsuitemarketing = $"{BaseNamespace}.hootsuitemarketing.svg";
        public static readonly string Loomly = $"{BaseNamespace}.loomly.svg";
        public static readonly string Sendinblue = $"{BaseNamespace}.sendinblue.svg";
        public static readonly string Campaignmonitor = $"{BaseNamespace}.campaignmonitor.svg";
        public static readonly string Constantcontact = $"{BaseNamespace}.constantcontact.svg";
        #endregion

        #region "E-Commerce Platforms"
        public static readonly string Shopify = $"{BaseNamespace}.shopify.svg";
        public static readonly string Woocommerce = $"{BaseNamespace}.woocommerce.svg";
        public static readonly string Bigcommerce = $"{BaseNamespace}.bigcommerce.svg";
        public static readonly string Bigcartel = $"{BaseNamespace}.bigcartel.svg";
        public static readonly string Prestashop = $"{BaseNamespace}.prestashop.svg";
        public static readonly string Squarespace = $"{BaseNamespace}.squarespace.svg";
        public static readonly string Wix = $"{BaseNamespace}.wix.svg";
        public static readonly string Etsy = $"{BaseNamespace}.etsy.svg";
        #endregion

        #region "Project Management & Collaboration"
        public static readonly string Asana = $"{BaseNamespace}.asana.svg";
        public static readonly string Jira = $"{BaseNamespace}.jira.svg";
        public static readonly string Trello = $"{BaseNamespace}.trello.svg";
        public static readonly string Slack = $"{BaseNamespace}.slack.svg";
        public static readonly string Discord = $"{BaseNamespace}.discord.svg";
        public static readonly string Rocketchat = $"{BaseNamespace}.rocketchat.svg";
        public static readonly string Telegram = $"{BaseNamespace}.telegram.svg";
        public static readonly string Threads = $"{BaseNamespace}.threads.svg";
        public static readonly string Bluesky = $"{BaseNamespace}.bluesky.svg";
        public static readonly string Teamwork = $"{BaseNamespace}.teamwork.svg";
        public static readonly string Wrike = $"{BaseNamespace}.wrike.svg";
        public static readonly string Smartsheet = $"{BaseNamespace}.smartsheet.svg";
        public static readonly string Smartsheetpm = $"{BaseNamespace}.smartsheetpm.svg";
        public static readonly string Clickup = $"{BaseNamespace}.clickup.svg";
        public static readonly string Trayio = $"{BaseNamespace}.trayio.svg";
        public static readonly string Integromat = $"{BaseNamespace}.integromat.svg";
        public static readonly string Make = $"{BaseNamespace}.make.svg";
        public static readonly string Anydo = $"{BaseNamespace}.anydo.svg";
        public static readonly string Doodle = $"{BaseNamespace}.doodle.svg";
        public static readonly string Chanty = $"{BaseNamespace}.chanty.svg";
        public static readonly string Flock = $"{BaseNamespace}.flock.svg";
        #endregion

        #region "Payment & Finance"
        public static readonly string Paypal = $"{BaseNamespace}.paypal.svg";
        public static readonly string Stripe = $"{BaseNamespace}.stripe.svg";
        public static readonly string Square = $"{BaseNamespace}.square.svg";
        public static readonly string Braintree = $"{BaseNamespace}.braintree.svg";
        public static readonly string Razorpay = $"{BaseNamespace}.razorpay.svg";
        public static readonly string Adyen = $"{BaseNamespace}.adyen.svg";
        public static readonly string Authorizenet = $"{BaseNamespace}.authorizenet.svg";
        public static readonly string Twocheckout = $"{BaseNamespace}.twocheckout.svg";
        public static readonly string Payoneer = $"{BaseNamespace}.payoneer.svg";
        public static readonly string Wise = $"{BaseNamespace}.wise.svg";
        #endregion

        #region "File Storage & Cloud"
        public static readonly string Box = $"{BaseNamespace}.box.svg";
        public static readonly string Dropbox = $"{BaseNamespace}.dropbox.svg";
        public static readonly string Icloud = $"{BaseNamespace}.icloud.svg";
        public static readonly string Mediafire = $"{BaseNamespace}.mediafire.svg";
        public static readonly string Mega = $"{BaseNamespace}.mega.svg";
        public static readonly string Backblaze = $"{BaseNamespace}.backblaze.svg";
        public static readonly string Citrixsharefile = $"{BaseNamespace}.citrixsharefile.svg";
        public static readonly string Egnyte = $"{BaseNamespace}.egnyte.svg";
        #endregion

        #region "Support & Ticketing"
        public static readonly string Freshdesk = $"{BaseNamespace}.freshsales.svg";
        public static readonly string Zendesk = $"{BaseNamespace}.zendesk.svg";
        public static readonly string Kayako = $"{BaseNamespace}.kayako.svg";
        public static readonly string Liveagent = $"{BaseNamespace}.liveagent.svg";
        public static readonly string Helpscout = $"{BaseNamespace}.helpscout.svg";
        #endregion

        #region "Communication & Messaging"
        public static readonly string Gmail = $"{BaseNamespace}.gmail.svg";
        public static readonly string Outlook = $"{BaseNamespace}.outlook.svg";
        public static readonly string Whatsappbusiness = $"{BaseNamespace}.whatsappbusiness.svg";
        public static readonly string Philipshue = $"{BaseNamespace}.philipshue.svg";
        #endregion

        #region "Machine Learning & AI Models"
        public static readonly string Mlmodel = $"{BaseNamespace}.mlmodel.svg";
        public static readonly string Mimodel = $"{BaseNamespace}.mimodel.svg";
        public static readonly string Onnx = $"{BaseNamespace}.onnx.svg";
        public static readonly string Libsvm = $"{BaseNamespace}.libsvm.svg";
        public static readonly string Petastorm = $"{BaseNamespace}.petastorm.svg";
        #endregion

        #region "Message Queuing"
        public static readonly string Messagequeue = $"{BaseNamespace}.messagequeue.svg";
        public static readonly string Activemq = $"{BaseNamespace}.activemq.svg";
        public static readonly string Kafka2 = $"{BaseNamespace}.kafka.svg";
        public static readonly string Queue = $"{BaseNamespace}.queue.svg";
        #endregion

        #region "Database Utilities & Tools"
        public static readonly string Cachedmemory = $"{BaseNamespace}.cachedmemory.svg";
        public static readonly string Columnardb = $"{BaseNamespace}.columnardb.svg";
        public static readonly string Connector = $"{BaseNamespace}.connector.svg";
        public static readonly string Entity = $"{BaseNamespace}.entity.svg";
        public static readonly string Gridgain = $"{BaseNamespace}.gridgain.svg";
        public static readonly string Hazelcast = $"{BaseNamespace}.hazelcast.svg";
        public static readonly string Inmemory = $"{BaseNamespace}.inmemory.svg";
        public static readonly string Inmemorycache = $"{BaseNamespace}.inmemorycache.svg";
        public static readonly string Keyvaluedb = $"{BaseNamespace}.keyvaluedb.svg";
        public static readonly string Kudu = $"{BaseNamespace}.kudu.svg";
        public static readonly string Memcached = $"{BaseNamespace}.memcached.svg";
        public static readonly string Rdbms = $"{BaseNamespace}.rdbms.svg";
        public static readonly string Realim = $"{BaseNamespace}.realim.svg";
        #endregion

        #region "IoT & Hardware"
        public static readonly string Arduinocloud = $"{BaseNamespace}.arduinocloud.svg";
        public static readonly string Iot = $"{BaseNamespace}.iot.svg";
        public static readonly string Smartthings = $"{BaseNamespace}.smartthings.svg";
        #endregion

        #region "Accounting & Financial"
        public static readonly string Benchaccounting = $"{BaseNamespace}.benchaccounting.svg";
        public static readonly string Freshbooks = $"{BaseNamespace}.freshbooks.svg";
        public static readonly string Sagebusinesscloud = $"{BaseNamespace}.sagebusinesscloud.svg";
        public static readonly string Sageintacct = $"{BaseNamespace}.sageintacct.svg";
        public static readonly string Waveapps = $"{BaseNamespace}.waveapps.svg";
        #endregion

        #region "Lead & Customer Management"
        public static readonly string Insightly = $"{BaseNamespace}.insightly.svg";
        public static readonly string Kudosity = $"{BaseNamespace}.kudosity.svg";
        #endregion

        #region "Data Analytics & Visualization"
        public static readonly string Cyfe = $"{BaseNamespace}.cyfe.svg";
        public static readonly string Databox = $"{BaseNamespace}.databox.svg";
        public static readonly string Fathom = $"{BaseNamespace}.fathom.svg";
        public static readonly string Geckoboard = $"{BaseNamespace}.geckoboard.svg";
        public static readonly string Heap = $"{BaseNamespace}.heap.svg";
        public static readonly string Tldv = $"{BaseNamespace}.tldv.svg";
        #endregion

        #region "Other Services & Platforms"
        public static readonly string Ado = $"{BaseNamespace}.ado.svg";
        public static readonly string Cloud = $"{BaseNamespace}.cloud.svg";
        public static readonly string Feather = $"{BaseNamespace}.feather.svg";
        public static readonly string File = $"{BaseNamespace}.file.svg";
        public static readonly string Firebase = $"{BaseNamespace}.firebase.svg";
        public static readonly string Geospatial = $"{BaseNamespace}.geospatial.svg";
        public static readonly string Hana = $"{BaseNamespace}.hana.svg";
        public static readonly string Hologres = $"{BaseNamespace}.hologres.svg";
        public static readonly string Microsoftaccess = $"{BaseNamespace}.microsoftaccess.svg";
        public static readonly string Microsoft = $"{BaseNamespace}.microsoft.svg";
        public static readonly string None = $"{BaseNamespace}.none.svg";
        public static readonly string Notion = $"{BaseNamespace}.notion.svg";
        public static readonly string Opc = $"{BaseNamespace}.opc.svg";
        public static readonly string Other = $"{BaseNamespace}.other.svg";
        public static readonly string Odbc = $"{BaseNamespace}.odbc.svg";
        public static readonly string Oledb = $"{BaseNamespace}.oledb.svg";
        public static readonly string Rocketset = $"{BaseNamespace}.rocketset.svg";
        public static readonly string Shapevector = $"{BaseNamespace}.shapevector.svg";
        public static readonly string Stream = $"{BaseNamespace}.stream.svg";
        public static readonly string Swaggerhub = $"{BaseNamespace}.swaggerhub.svg";
        public static readonly string Tuya = $"{BaseNamespace}.tuya.svg";
        public static readonly string Volusion = $"{BaseNamespace}.volusion.svg";
        public static readonly string Workflow = $"{BaseNamespace}.workflow.svg";
        public static readonly string Worldpay = $"{BaseNamespace}.worldpay.svg";
        public static readonly string Nest = $"{BaseNamespace}.nest.svg";
        public static readonly string Firebolt = $"{BaseNamespace}.firebolt.svg";
        public static readonly string Email = $"{BaseNamespace}.email.svg";
        public static readonly string Drip = $"{BaseNamespace}.drip.svg";
        public static readonly string Criteo = $"{BaseNamespace}.criteo.svg";
        public static readonly string Clicksend = $"{BaseNamespace}.clicksend.svg";
        public static readonly string Front = $"{BaseNamespace}.front.svg";
        public static readonly string Ecwid = $"{BaseNamespace}.ecwid.svg";
        #endregion

        /// <summary>
        /// Gets all SVG resource paths as a dictionary for easy enumeration.
        /// </summary>
        public static Dictionary<string, string> GetAllPaths()
        {
            var paths = new Dictionary<string, string>();
            var type = typeof(SvgsDatasources);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                if (field.FieldType == typeof(string) && field.IsLiteral == false && field.IsInitOnly)
                {
                    var value = field.GetValue(null) as string;
                    if (!string.IsNullOrEmpty(value) && value.EndsWith(".svg"))
                    {
                        paths[field.Name] = value;
                    }
                }
            }

            return paths;
        }

        /// <summary>
        /// Checks if a resource path exists in the assembly.
        /// </summary>
        /// <param name="resourcePath">The full resource path</param>
        /// <returns>True if the resource exists</returns>
        public static bool ResourceExists(string resourcePath)
        {
            var resourceNames = ResourceAssembly.GetManifestResourceNames();
            return resourceNames.Contains(resourcePath);
        }

        /// <summary>
        /// Gets all available SVG resource names from the assembly.
        /// </summary>
        /// <returns>Array of resource names</returns>
        public static string[] GetAvailableResources()
        {
            return ResourceAssembly.GetManifestResourceNames()
                .Where(name => name.StartsWith(BaseNamespace) && name.EndsWith(".svg"))
                .ToArray();
        }

        /// <summary>
        /// Helper method to get the full file system path (useful for development/debugging).
        /// This assumes the standard project structure.
        /// </summary>
        /// <param name="svgFileName">Just the SVG filename (e.g., "mongodb.svg")</param>
        /// <returns>Full file system path</returns>
        public static string GetFileSystemPath(string svgFileName)
        {
            // This is for development use - gets the actual file path
            string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(baseDirectory, "..", "..", "..", "GFX", "Icons", "datasources", svgFileName);
        }
    }

    /// <summary>
    /// Extension methods for easier use of SvgsDatasources with BeepImage controls.
    /// </summary>
    public static class BeepSvgDatasourcesExtensions
    {
        /// <summary>
        /// Sets the image path for a BeepImage control using SvgsDatasources
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <param name="svgPath">The SVG path from SvgsDatasources</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetDatasourceSvgPath(this BeepImage beepImage, string svgPath)
        {
            if (beepImage != null)
            {
                beepImage.ImagePath = svgPath;
            }
            return beepImage;
        }

        /// <summary>
        /// Sets a database icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <param name="dbType">The database type from SvgsDatasources</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetDatabaseIcon(this BeepImage beepImage, string dbType)
        {
            return beepImage.SetDatasourceSvgPath(dbType);
        }

        /// <summary>
        /// Sets a MongoDB icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetMongoDbIcon(this BeepImage beepImage)
        {
            return beepImage.SetDatasourceSvgPath(SvgsDatasources.Mongodb);
        }

        /// <summary>
        /// Sets a PostgreSQL icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetPostgreSqlIcon(this BeepImage beepImage)
        {
            return beepImage.SetDatasourceSvgPath(SvgsDatasources.Postgresql);
        }

        /// <summary>
        /// Sets a MySQL icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetMySqlIcon(this BeepImage beepImage)
        {
            return beepImage.SetDatasourceSvgPath(SvgsDatasources.Mysql);
        }

        /// <summary>
        /// Sets a SQL Server icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetSqlServerIcon(this BeepImage beepImage)
        {
            return beepImage.SetDatasourceSvgPath(SvgsDatasources.Sqlserver);
        }

        /// <summary>
        /// Sets an Oracle icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetOracleIcon(this BeepImage beepImage)
        {
            return beepImage.SetDatasourceSvgPath(SvgsDatasources.Oracle);
        }

        /// <summary>
        /// Sets a Redis icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetRedisIcon(this BeepImage beepImage)
        {
            return beepImage.SetDatasourceSvgPath(SvgsDatasources.Redis);
        }

        /// <summary>
        /// Sets a Salesforce icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetSalesforceIcon(this BeepImage beepImage)
        {
            return beepImage.SetDatasourceSvgPath(SvgsDatasources.Salesforce);
        }

        /// <summary>
        /// Sets a Snowflake icon for the BeepImage
        /// </summary>
        /// <param name="beepImage">The BeepImage control</param>
        /// <returns>The BeepImage for method chaining</returns>
        public static BeepImage SetSnowflakeIcon(this BeepImage beepImage)
        {
            return beepImage.SetDatasourceSvgPath(SvgsDatasources.Snowflake);
        }
    }
}
