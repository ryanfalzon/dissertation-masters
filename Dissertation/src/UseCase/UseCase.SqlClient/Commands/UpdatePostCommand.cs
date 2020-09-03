namespace UseCase.SqlClient.Commands
{
    public class UpdatePostCommand
    {
        public static string Sql = @"
Update P
SET
    P.Content = @Content
FROM Posts P
WHERE P.Id = @Id;";

        public static void Execute(object param = null)
        {
            DatabaseHelper.ExecuteCommand(Sql, param);
        }
    }
}