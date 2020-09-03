using System.Collections.Generic;
using UseCase.Contracts;
using UseCase.Contracts.Interfaces;
using UseCase.SqlClient.Queries;

namespace UseCase.SqlClient
{
    public class PostsQueryService : IPostsQueryService
    {
        public Post GetPost(int id)
        {
            return GetPostQuery<Post>.Execute(new
            {
                Id = id
            });
        }

        public IEnumerable<Post> GetPosts(int offset)
        {
            return GetPostsPaginatedQuery<Post>.Execute(new
            {
                Offset = offset
            });
        }
    }
}