using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace MobilityScm.Utilerias
{
    public interface IBaseDeDatosServicio
    {
        IList<T> ExecuteQuery<T>(string commandText, CommandType commandType, params DbParameter[] parameters);
        IList<T> ExecuteQuery<T>(string commandText, params DbParameter[] parameters);
        IList<T> ExecuteQuery<T>(string commandText, CommandType commandType, bool autoCommit, params DbParameter[] parameters);
        int ExecuteNonQuery(string commandText, CommandType commandType, params DbParameter[] parameters);
        int ExecuteNonQuery(string commandText, params DbParameter[] parameters);
        T ExecuteScalar<T>(string commandText, params DbParameter[] parameters);
        T ExecuteScalar<T>(string commandText, CommandType commandType, params DbParameter[] parameters);
       
        void BeginTransaction();

        void Commit();

        void Rollback();


        string Esquema { get; }
    }
}
