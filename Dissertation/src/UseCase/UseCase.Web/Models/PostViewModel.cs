using Newtonsoft.Json;

namespace UseCase.Web.Models
{
    public class PostViewModel
    {
        [JsonIgnore]
        public int Id { get; set; }

        public int UserId { get; set; }

        [JsonIgnore]
        public string Hash { get; set; }

        public string Content { get; set; }

        public long Timestamp { get; set; }

        [JsonIgnore]
        public int Likes { get; set; }
    }
}