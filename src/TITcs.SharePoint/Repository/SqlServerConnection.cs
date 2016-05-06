using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TITcs.SharePoint.Repository
{
    public abstract class SqlServerConnection
    {
        private SqlConnection _sqlConnection;
        private static string _connectionString;

        protected SqlServerConnection(string connectionString)
        {
            if (_connectionString == null)
                _connectionString = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
        }

        private void OpenConnection()
        {
            _sqlConnection = new SqlConnection(_connectionString);
            _sqlConnection.Open();
        }

        private void CloseConnection()
        {
            if (_sqlConnection != null && _sqlConnection.State == ConnectionState.Open)
            {
                _sqlConnection = new SqlConnection(_connectionString);
                _sqlConnection.Close();
                _sqlConnection.Dispose();
            }
        }

        protected SqlParameter CreateParameter(string name, object value)
        {
            var parameter = new SqlParameter(name, value ?? DBNull.Value)
            {
                Value = value
            };

            return parameter;
        }

        protected DataSet Execute(string storedProcedureName, params SqlParameter[] parameters)
        {
            OpenConnection();
            var sqlCommand = new SqlCommand(storedProcedureName, _sqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };


            foreach (var parameter in parameters)
            {
                sqlCommand.Parameters.Add(parameter);
            }

            var dataSet = new DataSet();
            var dataAdapter = new SqlDataAdapter(sqlCommand);

            dataAdapter.Fill(dataSet);

            return dataSet;
        }

        protected bool ExecuteNoReader(string storedProcedureName, params SqlParameter[] parameters)
        {
            OpenConnection();
            var sqlCommand = new SqlCommand(storedProcedureName, _sqlConnection)
            {
                CommandType = CommandType.StoredProcedure
            };


            foreach (var parameter in parameters)
            {
                sqlCommand.Parameters.Add(parameter);
            }


            var rowsAffected = sqlCommand.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        protected bool ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            OpenConnection();
            var sqlCommand = new SqlCommand(query, _sqlConnection)
            {
                CommandType = CommandType.Text
            };


            foreach (var parameter in parameters)
            {
                sqlCommand.Parameters.Add(parameter);
            }


            var rowsAffected = sqlCommand.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        ~SqlServerConnection()
        {
            CloseConnection();
        }

        protected string GetString(object value)
        {
            return value == DBNull.Value ? "" : value.ToString();
        }

        protected int GetInt32(object value)
        {
            return value == DBNull.Value ? 0 : Convert.ToInt32(value.ToString());
        }

        protected double GetDouble(object value)
        {
            return value == DBNull.Value ? 0 : Convert.ToDouble(value.ToString());
        }

        protected DateTime GetDate(object value)
        {
            return value == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(value.ToString());
        }

        protected Guid GetGuid(object value)
        {
            return value == DBNull.Value ? Guid.Empty : new Guid(value.ToString());
        }
    }
}
