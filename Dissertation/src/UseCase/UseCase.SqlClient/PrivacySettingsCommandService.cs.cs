using UseCase.Contracts;
using UseCase.Contracts.Interfaces;
using UseCase.SqlClient.Commands;

namespace UseCase.SqlClient
{
    public class PrivacySettingsCommandService : IPrivacySettingsCommandService
    {
        public void InsertPrivacySettings(int userId)
        {
            InsertPrivacySettingsCommand.Execute(new
            {
                UserId = userId
            });
        }

        public void UpdatePrivacySettings(PrivacySettings privacySettings)
        {
            UpdatePrivacySettingsCommand.Execute(new
            {
                privacySettings.UserId,
                privacySettings.VisibleProfile,
                privacySettings.VisiblePosts
            });
        }
    }
}