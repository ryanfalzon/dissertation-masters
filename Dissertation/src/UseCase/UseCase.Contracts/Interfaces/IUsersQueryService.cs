namespace UseCase.Contracts.Interfaces
{
    public interface IUsersQueryService
    {
        User GetUser(string publicKey);
    }
}