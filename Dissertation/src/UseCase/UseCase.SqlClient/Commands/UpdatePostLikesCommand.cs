namespace UseCase.SqlClient.Commands
{
    public class UpdatePostLikesCommand
    {
        public static string Sql = @"
Update P
SET
    P.Likes = P.Likes + 1
FROM Posts P
WHERE P.Id = @Id;";

        public static void Execute(object param = null)
        {
            DatabaseHelper.ExecuteCommand(Sql, param);
        }
    }
}