namespace UseCase.SqlClient.Commands
{
    public class InsertPrivacySettingsCommand
    {
        public static string Sql = @"
INSERT INTO PrivacySettings(UserId)
VALUES(@UserId);";

        public static void Execute(object param = null)
        {
            DatabaseHelper.ExecuteCommand(Sql, param);
        }
    }
}