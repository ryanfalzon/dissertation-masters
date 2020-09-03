namespace UseCase.Contracts.Interfaces
{
    public interface IPrivacySettingsQueryService
    {
        PrivacySettings GetPrivacySettings(int userId);
    }
}