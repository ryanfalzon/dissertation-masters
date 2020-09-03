using System.Collections.Generic;

namespace UseCase.SqlClient.Queries
{
    public class GetUserProfileQuery<T>
    {
        public static string Sql = @"
SELECT TOP 1
	U.Id,
    U.PublicKey,
    U.FirstName,
    U.LastName,
    U.Email,
    U.Mobile,
    U.Description
FROM Users U
WHERE U.PublicKey = @PublicKey;";

        public static T Execute(object param = null)
        {
            return DatabaseHelper.ExecuteQuerySingle<T>(Sql, param);
        }
    }
}