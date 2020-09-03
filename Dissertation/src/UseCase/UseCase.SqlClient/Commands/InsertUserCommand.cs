namespace UseCase.SqlClient.Commands
{
    public class InsertUserCommand
    {
        public static string Sql = @"
INSERT INTO Users(PublicKey, FirstName, LastName, Email, Mobile, Description)
VALUES(@PublicKey, @FirstName, @LastName, @Email, @Mobile, @Description);";

        public static void Execute(object param = null)
        {
            DatabaseHelper.ExecuteCommand(Sql, param);
        }
    }
}