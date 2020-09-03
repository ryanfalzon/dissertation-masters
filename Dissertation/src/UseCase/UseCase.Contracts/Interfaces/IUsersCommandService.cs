namespace UseCase.Contracts.Interfaces
{
    public interface IUsersCommandService
    {
        void InsertUser(User user);

        void UpdateUser(User user);
    }
}