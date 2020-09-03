namespace UseCase.Contracts
{
    public class PrivacySettings
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public bool VisibleProfile { get; set; }

        public bool VisiblePosts { get; set; }
    }
}