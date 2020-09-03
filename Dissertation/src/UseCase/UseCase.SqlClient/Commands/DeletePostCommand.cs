namespace UseCase.SqlClient.Commands
{
    public class DeletePostCommand
    {
        public static string Sql = @"
DELETE FROM POSTS
WHERE Id = @Id;";

        public static void Execute(object param = null)
        {
            DatabaseHelper.ExecuteCommand(Sql, param);
        }
    }
}