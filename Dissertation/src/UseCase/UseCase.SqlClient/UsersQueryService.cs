using UseCase.Contracts;
using UseCase.Contracts.Interfaces;
using UseCase.SqlClient.Queries;

namespace UseCase.SqlClient
{
    public class UsersQueryService : IUsersQueryService
    {
        public User GetUser(string publicKey)
        {
            return GetUserProfileQuery<User>.Execute(new
            {
                PublicKey = publicKey
            });
        }
    }
}