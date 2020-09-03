namespace UseCase.Contracts.Interfaces
{
    public interface IPostsCommandService
    {
        void InsertPost(Post post);

        void UpdatePost(Post post);

        void LikePost(int id);

        void DeletePost(int id);
    }
}