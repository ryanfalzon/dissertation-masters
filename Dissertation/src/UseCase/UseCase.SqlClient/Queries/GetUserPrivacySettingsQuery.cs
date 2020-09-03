using System.Collections.Generic;

namespace UseCase.SqlClient.Queries
{
    public class GetUserPrivacySettingsQuery<T>
    {
        public static string Sql = @"
SELECT TOP 1
	PS.Id,
    PS.VisibleProfile,
    PS.VisiblePosts,
    PS.UserId
FROM PrivacySettings PS
WHERE PS.UserId = @UserId;";

        public static T Execute(object param = null)
        {
            return DatabaseHelper.ExecuteQuerySingle<T>(Sql, param);
        }
    }
}