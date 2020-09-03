namespace UseCase.SqlClient.Commands
{
    public class UpdatePrivacySettingsCommand
    {
        public static string Sql = @"
UPDATE PS
SET
    PS.VisibleProfile = @VisibleProfile,
    PS.VisiblePosts = @VisiblePosts
FROM PrivacySettings PS
WHERE PS.UserId = @UserId;";

        public static void Execute(object param = null)
        {
            DatabaseHelper.ExecuteCommand(Sql, param);
        }
    }
}