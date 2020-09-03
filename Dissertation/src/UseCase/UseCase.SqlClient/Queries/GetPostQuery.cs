namespace UseCase.SqlClient.Queries
{
    public class GetPostQuery<T>
    {
        public static string Sql = @"
SELECT TOP 1
	P.Id,
    P.UserId,
	P.Hash,
    P.Content,
    P.Timestamp,
    P.Likes
FROM Posts P
WHERE P.Id = @Id;";

        public static T Execute(object param = null)
        {
            return DatabaseHelper.ExecuteQuerySingle<T>(Sql, param);
        }
    }
}