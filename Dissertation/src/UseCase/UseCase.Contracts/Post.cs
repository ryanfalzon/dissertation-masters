namespace UseCase.Contracts
{
    public class Post
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Hash { get; set; }

        public string Content { get; set; }

        public long Timestamp { get; set; }

        public int Likes { get; set; }
    }
}