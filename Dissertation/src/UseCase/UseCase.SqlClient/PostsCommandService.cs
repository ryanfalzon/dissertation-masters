using UseCase.Contracts;
using UseCase.Contracts.Interfaces;
using UseCase.SqlClient.Commands;

namespace UseCase.SqlClient
{
    public class PostsCommandService : IPostsCommandService
    {
        public void DeletePost(int id)
        {
            DeletePostCommand.Execute(new
            {
                Id = id
            });
        }

        public void LikePost(int id)
        {
            UpdatePostLikesCommand.Execute(new
            {
                Id = id
            });
        }

        public void InsertPost(Post post)
        {
            InsertPostCommand.Execute(new
            {
                post.UserId,
                post.Hash,
                post.Content,
                post.Timestamp,
                post.Likes
            });
        }

        public void UpdatePost(Post post)
        {
            UpdatePostCommand.Execute(new
            {
                post.Id,
                post.Content
            });
        }
    }
}