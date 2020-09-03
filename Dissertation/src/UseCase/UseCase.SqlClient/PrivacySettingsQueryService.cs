using UseCase.Contracts;
using UseCase.Contracts.Interfaces;
using UseCase.SqlClient.Queries;

namespace UseCase.SqlClient
{
    public class PrivacySettingsQueryService : IPrivacySettingsQueryService
    {
        public PrivacySettings GetPrivacySettings(int userId)
        {
            return GetUserPrivacySettingsQuery<PrivacySettings>.Execute(new
            {
                UserId = userId
            });
        }
    }
}