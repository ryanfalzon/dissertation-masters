namespace UseCase.Web.Models
{
    public class PrivacySettingsViewModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public bool VisibleProfile { get; set; }

        public bool VisiblePosts { get; set; }
    }
}