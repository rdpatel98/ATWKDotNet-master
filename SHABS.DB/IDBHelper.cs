using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SHABS.DB
{
    public interface IDBHelper
    {
         DataTable GetData(string query);
         DataTable GetData(string procName,Dictionary<string,string> parameters);
         DataTable GetData(string procName, Dictionary<string, object> parameters);
         int InsertUpdateData(string query);
         int InsertUpdateData(string procName, Dictionary<string, string> parameters);
         int InsertUpdateData(string procName, Dictionary<string, object> parameters);
         object ExecuteScalar(string procName, Dictionary<string, string> parameters);
         object ExecuteScalar(string procName, Dictionary<string, object> parameters);
         string ConnectionString { get; set; }
         void OpenConnection();
         void CloseConnection();
    }
}
