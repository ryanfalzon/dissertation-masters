namespace UseCase.Contracts.Interfaces
{
    public interface IPrivacySettingsCommandService
    {
        void InsertPrivacySettings(int userId);

        void UpdatePrivacySettings(PrivacySettings privacySettings);
    }
}