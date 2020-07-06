using Telerik.OpenAccess;
using Telerik.OpenAccess.Metadata;
namespace MobilityScm.Utilerias
{
    public partial class EntitiesModel : OpenAccessContext
    {
        private const string ConnectionStringName = @"StringConnection";

        private static readonly BackendConfiguration Backend = GetBackendConfiguration();

        private static readonly MetadataSource MetadataSource = AttributesMetadataSource.FromContext(typeof(EntitiesModel));

        static partial void CustomizeBackendConfiguration(ref BackendConfiguration config);

        public static BackendConfiguration GetBackendConfiguration()
        {
            var configuration = new BackendConfiguration
            {
                Backend = "MsSql",
                ProviderName = "System.Data.SqlClient",
                Runtime = { CommandTimeout = 60000}
            };

            CustomizeBackendConfiguration(ref configuration);

            return configuration;
        }

        public EntitiesModel()
            : base(ConnectionStringName, Backend, MetadataSource)
        { }


        public EntitiesModel(string connection)
            : base(connection, Backend, MetadataSource)
        { }


        public EntitiesModel(BackendConfiguration backendConfiguration)
            : base(ConnectionStringName, backendConfiguration, MetadataSource)
        { }


        public EntitiesModel(string connection, MetadataSource metadataSource)
            : base(connection, Backend, metadataSource)
        { }

        public EntitiesModel(string connection, BackendConfiguration backendConfiguration, MetadataSource metadataSource)
            : base(connection, backendConfiguration, metadataSource)
        { }

    }
}
