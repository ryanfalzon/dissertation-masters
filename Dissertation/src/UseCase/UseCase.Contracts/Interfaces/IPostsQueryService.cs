using System.Collections.Generic;

namespace UseCase.Contracts.Interfaces
{
    public interface IPostsQueryService
    {
        Post GetPost(int id);

        IEnumerable<Post> GetPosts(int offset);
    }
}