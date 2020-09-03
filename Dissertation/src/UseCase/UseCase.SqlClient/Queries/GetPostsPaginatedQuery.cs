using System.Collections.Generic;

namespace UseCase.SqlClient.Queries
{
    public class GetPostsPaginatedQuery<T>
    {
        public static string Sql = @"
SELECT
	P.Id,
    P.UserId,
	P.Hash,
    P.Content,
    P.Timestamp,
    P.Likes
FROM Posts P
INNER JOIN PrivacySettings PS
    ON PS.UserId = P.UserId
WHERE PS.VisiblePosts = 1
ORDER BY P.Id DESC
OFFSET @Offset ROW
FETCH FIRST 10 ROWS ONLY;";

        public static IEnumerable<T> Execute(object param = null)
        {
            return DatabaseHelper.ExecuteQuery<T>(Sql, param);
        }
    }
}