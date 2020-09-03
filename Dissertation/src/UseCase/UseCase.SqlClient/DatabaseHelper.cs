using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace UseCase.SqlClient
{
    public class DatabaseHelper
    {
        public static string ConnectionString = "Server=dlt5900.database.windows.net,1433;Database=SocialNetworkDApp;User ID=ryanfalzon;Password=DltMasters2020;Trusted_Connection=False;Encrypt=True;";

        public static T ExecuteQuerySingle<T>(string sql, object param = null)
        {
            using SqlConnection connection = new SqlConnection(ConnectionString);
            return connection.QuerySingle<T>(sql, param);
        }

        public static IEnumerable<T> ExecuteQuery<T>(string sql, object param = null)
        {
            using SqlConnection connection = new SqlConnection(ConnectionString);
            return connection.Query<T>(sql, param);
        }

        public static void ExecuteCommand(string sql, object param = null)
        {
            using SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Execute(sql, param);
        }
    }
}