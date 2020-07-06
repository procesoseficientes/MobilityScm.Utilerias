using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Telerik.OpenAccess;

namespace MobilityScm.Utilerias
{
    public partial class BaseDeDatosServicio : OpenAccessDomainService<EntitiesModel>, IBaseDeDatosServicio
    {
        private EntitiesModel _dataContext;
        public IConfiguracionDeConexion ConfiguracionDeConexion { get; set; }

        // ReSharper disable once EmptyConstructor
        public BaseDeDatosServicio()
            // ReSharper disable once RedundantBaseConstructorCall
            : base()
        {

        }

        public BaseDeDatosServicio(IConfiguracionDeConexion configuracionDeConeccion):base()
        {
            ConfiguracionDeConexion = configuracionDeConeccion;
        }

        protected override EntitiesModel DataContext => _dataContext ?? (_dataContext = new EntitiesModel(ConfiguracionDeConexion.StringConnection));


        public IList<T> ExecuteQuery<T>(string commandText, CommandType commandType, params DbParameter[] parameters)
        {    
            var ls =  DataContext.ExecuteQuery<T>(commandText, commandType, parameters);
            DataContext.SaveChanges();
            return ls;
        }

        public IList<T> ExecuteQuery<T>(string commandText, params DbParameter[] parameters)
        {
            
            var ls = DataContext.ExecuteQuery<T>(commandText, parameters);
            DataContext.SaveChanges();
            return ls;
        }
        public IList<T> ExecuteQuery<T>(string commandText, CommandType commandType, bool autoCommit, params DbParameter[] parameters)
        {
            var ls = DataContext.ExecuteQuery<T>(commandText, commandType, parameters);
            if (autoCommit)
            {
                DataContext.SaveChanges();
            }
            return ls;
        }

        public void BeginTransaction()
        {

        }
        public void Commit()
        {
            DataContext.SaveChanges();
        }

        public void Rollback()
        {
            DataContext.ClearChanges();
        }

        public string Esquema => ConfiguracionDeConexion.Esquema;

        public int ExecuteNonQuery(string commandText, CommandType commandType, params DbParameter[] parameters)
        {
            return DataContext.ExecuteNonQuery(commandText, commandType, parameters);
        }

        public int ExecuteNonQuery(string commandText, params DbParameter[] parameters)
        {
            return DataContext.ExecuteNonQuery(commandText, parameters);
        }


        public T ExecuteScalar<T>(string commandText, params DbParameter[] parameters)
        {
            return DataContext.ExecuteScalar<T>(commandText, parameters);
        }

        public T ExecuteScalar<T>(string commandText, CommandType commandType, params DbParameter[] parameters)
        {
            return DataContext.ExecuteScalar<T>(commandText, commandType, parameters);
        }


        
    }

    public interface IConfiguracionDeConexion
    {
        string StringConnection { get; }
        string Esquema { get; }
    }
}
