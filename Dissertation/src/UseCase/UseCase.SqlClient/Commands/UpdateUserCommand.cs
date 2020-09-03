namespace UseCase.SqlClient.Commands
{
    public class UpdateUserCommand
    {
        public static string Sql = @"
UPDATE U
SET
    U.PublicKey = @PublicKey,
    U.FirstName = @FirstName,
    U.LastName = @LastName,
    U.Email = @Email,
    U.Mobile = @Mobile,
    U.Description = @Description
FROM Users U
WHERE U.Id = @Id;";

        public static void Execute(object param = null)
        {
            DatabaseHelper.ExecuteCommand(Sql, param);
        }
    }
}