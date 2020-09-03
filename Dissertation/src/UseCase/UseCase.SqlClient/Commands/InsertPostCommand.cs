namespace UseCase.SqlClient.Commands
{
    public class InsertPostCommand
    {
        public static string Sql = @"
INSERT INTO Posts(UserId, Hash, Content, Timestamp, Likes)
VALUES(@UserId, @Hash, @Content, @Timestamp, @Likes);";

        public static void Execute(object param = null)
        {
            DatabaseHelper.ExecuteCommand(Sql, param);
        }
    }
}