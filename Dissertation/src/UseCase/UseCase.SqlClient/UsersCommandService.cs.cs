using UseCase.Contracts;
using UseCase.Contracts.Interfaces;
using UseCase.SqlClient.Commands;

namespace UseCase.SqlClient
{
    public class UsersCommandService : IUsersCommandService
    {
        public void InsertUser(User user)
        {
            InsertUserCommand.Execute(new
            {
                user.PublicKey,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Mobile,
                user.Description
            });
        }

        public void UpdateUser(User user)
        {
            UpdateUserCommand.Execute(new
            {
                user.Id,
                user.PublicKey,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Mobile,
                user.Description
            });
        }
    }
}