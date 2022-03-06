using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHABS.COMMON;
using System.Data.OleDb;
using System.Data;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SHABS.DB
{
    public class OleDBHelper : IDBHelper
    {
        private string _connectionString;
        private OleDbCommand _command;
        private OleDbDataAdapter _adapter;
        private OleDbConnection _connection;

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        public void GetConnectionString()
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                ConnectionString = System.Configuration.ConfigurationManager.AppSettings["connectionString"];
            }
        }

        public System.Data.DataTable GetData(string query)
        {
            DataTable data = new DataTable();
            try
            {
                _command = new OleDbCommand();
                _adapter = new OleDbDataAdapter();
                GetConnectionString();
                OpenConnection();
                _command.Connection = _connection;
                _command.CommandType = CommandType.Text;
                _command.CommandText = query;
                _adapter.SelectCommand = _command;
                _adapter.Fill(data);

            }
            catch (Exception ex)
            {
                // throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return data;
        }
        public async Task<Dictionary<string, object>> GetMsgByIdAsync(string ID)
        {
            DataTable data = new DataTable();
            try
            {
                GetConnectionString();
                OpenConnection();
                var table = new DataTable();
                var dt = new OleDbDataAdapter($"SELECT * FROM dbo.Messages WHERE ID={ID}", ConnectionString);

                dt.Fill(table);
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

                Dictionary<string, object> row;
                foreach (DataRow dr in table.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in table.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }
                return rows.Last();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                CloseConnection();
            }
            return null;
        }
        public string GetUserData(int userID)
        {
            DataTable data = new DataTable();
            try
            {
                GetConnectionString();
                OpenConnection();
                var table = new DataTable();
                var dt = new OleDbDataAdapter($"SELECT * FROM dbo.Messages WHERE createdBy={userID}", ConnectionString);

                dt.Fill(table);
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

                Dictionary<string, object> row;
                foreach (DataRow dr in table.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in table.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }
                return JsonConvert.SerializeObject(rows);
            }
            catch (Exception ex)
            {
                // throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return null;
        }

        public async Task<Dictionary<string, object>> GetUserDataBtwAsync(string me, string to, string subject, string guid)
        {
            DataTable data = new DataTable();
            try
            {
                GetConnectionString();
                OpenConnection();
                var table = new DataTable();
                var dt = new OleDbDataAdapter($"SELECT * FROM dbo.Messages WHERE SentFrom='{me}' AND SendTo='{to}' AND Subject='{subject}'", ConnectionString);

                dt.Fill(table);
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

                Dictionary<string, object> row;
                foreach (DataRow dr in table.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in table.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
                }
                return rows.Last();
            }
            catch (Exception ex)
            {
                FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
                await firebaseDBHelper.WriteLog(guid, "SQL Error", ex.ToString());
            }
            finally
            {
                CloseConnection();
            }
            return null;
        }

        public System.Data.DataTable GetData(string procName, Dictionary<string, string> parameters)
        {
            DataTable data = new DataTable();
            try
            {
                _command = new OleDbCommand();
                _adapter = new OleDbDataAdapter();
                GetConnectionString();
                OpenConnection();
                _command.Connection = _connection;
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = procName;
                Addparams(parameters);
                _adapter.SelectCommand = _command;
                _adapter.Fill(data);
            }
            catch (Exception ex)
            {

                ///  throw;
            }
            finally
            {
                CloseConnection();
            }
            return data;
        }

        public System.Data.DataTable GetData(string procName, Dictionary<string, object> parameters)
        {
            DataTable data = new DataTable();
            try
            {
                _command = new OleDbCommand();
                _adapter = new OleDbDataAdapter();
                GetConnectionString();
                OpenConnection();
                _command.Connection = _connection;
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = procName;
                Addparams(parameters);
                _adapter.SelectCommand = _command;
                _adapter.Fill(data);
            }
            catch (Exception ex)
            {

                ///  throw;
            }
            finally
            {
                CloseConnection();
            }
            return data;
        }

        public int InsertUpdateData(string query)
        {
            int resl = 0;
            try
            {
                _command = new OleDbCommand();
                GetConnectionString();
                OpenConnection();
                _command.Connection = _connection;
                _command.CommandType = CommandType.Text;
                _command.CommandText = query;
                resl = _command.ExecuteNonQuery();

            }
            catch (Exception)
            {
                //throw;
            }
            finally
            {
                CloseConnection();
            }
            return resl;
        }
        public async System.Threading.Tasks.Task<int> InsertUpdateDataAsync(string procName, Dictionary<string, string> parameters, string guid)
        {
            int resl = 0;
            try
            {
                _command = new OleDbCommand();
                GetConnectionString();
                OpenConnection();
                _command.Connection = _connection;
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = procName;
                Addparams(parameters);
                resl = _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                FirebaseDBHelper firebaseDBHelper = new FirebaseDBHelper();
                await firebaseDBHelper.WriteLog(guid, "SQL Error", ex.ToString());
            }
            finally
            {
                CloseConnection();
            }

            return resl;
        }
        public int InsertUpdateData(string procName, Dictionary<string, string> parameters)
        {
            int resl = 0;
            try
            {
                _command = new OleDbCommand();
                GetConnectionString();
                OpenConnection();
                _command.Connection = _connection;
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = procName;
                Addparams(parameters);
                resl = _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                CloseConnection();
            }

            return resl;
        }


        public object ExecuteScalar(string procName, Dictionary<string, string> parameters)
        {
            object resl = null;
            try
            {
                _command = new OleDbCommand();
                GetConnectionString();
                OpenConnection();
                _command.Connection = _connection;
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = procName;
                Addparams(parameters);
                resl = _command.ExecuteScalar();
            }
            catch (Exception)
            {
            }
            finally
            {
                CloseConnection();
            }
            return resl;
        }

        public object ExecuteScalar(string procName, Dictionary<string, object> parameters)
        {
            object resl = null;
            try
            {
                _command = new OleDbCommand();
                GetConnectionString();
                OpenConnection();
                _command.Connection = _connection;
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = procName;
                Addparams(parameters);
                resl = _command.ExecuteScalar();
            }
            catch (Exception)
            {
            }
            finally
            {
                CloseConnection();
            }
            return resl;
        }



        public void OpenConnection()
        {
            _connection = new OleDbConnection();
            _connection.ConnectionString = _connectionString;
            try
            {
                //if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }
            }
            catch (Exception ex)
            {
                // throw ex;
            }
        }

        public void CloseConnection()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                // if (System.Configuration.ConfigurationManager.AppSettings["close"] == "1")
                {
                    _connection.Close();
                }
            }
        }

        private void Addparams(Dictionary<string, string> parameters)
        {
            foreach (var item in parameters)
            {
                _command.Parameters.AddWithValue(item.Key, item.Value);
            }
        }

        private void Addparams(Dictionary<string, object> parameters)
        {
            foreach (var item in parameters)
            {
                _command.Parameters.AddWithValue(item.Key, item.Value);
            }
        }






        public int InsertUpdateData(string procName, Dictionary<string, object> parameters)
        // public int InsertUpdateData(string _procName, Dictionary<string, object> parameters)
        {
            int resl = 0;
            try
            {
                _command = new OleDbCommand();
                GetConnectionString();
                OpenConnection();
                _command.Connection = _connection;
                _command.CommandType = CommandType.StoredProcedure;
                _command.CommandText = procName;
                Addparams(parameters);
                resl = _command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                //throw;
            }
            finally
            {
                CloseConnection();
            }

            return resl;
        }

        public void Alter()
        {
           
                GetConnectionString();
                OpenConnection();
            string script = @"
ALTER procedure[dbo].[GetMsgById]--'24951,24950,24949,24948'
(
--@Me varchar(500),
@MsgID varchar(500)
)
as
Set nocount on
BEGIN
 select m.*

	,p1.Name as FromName,p1.ImageID as FromImageID,  
p2.Name as ToName,p2.ImageID as ToImageID,u1.userType as [FromUserType],u1.lastOnlineTime as [FromLastOnlineTime],u2.lastOnlineTime as [ToLastOnlineTime] from messages m
join users u1 on u1.username = m.SentFrom
join users u2 on u2.username = m.SendTo
join userProfile p1 on p1.userID = u1.userID
join userProfile p2 on p2.userID = u2.userID
where m.ID = @MsgID
end
";
            OleDbCommand command = new OleDbCommand(script, _connection);
            command.ExecuteNonQuery();
            CloseConnection();
        }

        //get message model
        //get from user data
        // get to user data 
        public Dictionary<string, object>  GetUserDetailsByUsername(string username)
        {
           
                DataTable data = new DataTable();
                try
                {
                    GetConnectionString();
                    OpenConnection();
                    var table = new DataTable();
                    var dt = new OleDbDataAdapter($"SELECT * FROM dbo.users WHERE username='{username}'", ConnectionString);

                    dt.Fill(table);
                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

                    Dictionary<string, object> row;
                    foreach (DataRow dr in table.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in table.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                    return rows.First();
                }
                catch (Exception ex)
                {
                    // throw ex;
                }
                finally
                {
                    CloseConnection();
                }
                return null;            
        }
    }
    public static class ObjectToDictionaryHelper
    {
        public static IDictionary<string, string> ToDictionary(this object source)
        {
            return source.ToDictionary<string>();
        }

        public static IDictionary<string, T> ToDictionary<T>(this object source)
        {
            if (source == null)
                ThrowExceptionWhenSourceArgumentIsNull();

            var dictionary = new Dictionary<string, T>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                AddPropertyToDictionary<T>(property, source, dictionary);
            return dictionary;
        }

        private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, T> dictionary)
        {
            object value = property.GetValue(source);
            if (IsOfType<T>(value))
                dictionary.Add(property.Name, (T)value);
        }

        private static bool IsOfType<T>(object value)
        {
            return value is T;
        }

        private static void ThrowExceptionWhenSourceArgumentIsNull()
        {
            throw new ArgumentNullException("source", "Unable to convert object to a dictionary. The source object is null.");
        }
    }

}

