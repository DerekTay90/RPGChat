using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace DataAccessLayer
{
    public interface IUserAccessor
    {
        User AuthenticateUser(string email, string passwordHash);
        bool UpdatePasswordHash(int userID, string oldPassHash, string newPassHash);
        List<User> SelectUsersByActive(bool active = true);
        int UpdateUser(User oldUser, User newUser);
        int InsertUser(User user);
        int ActivateUser(int userID);
        int DeactivateUser(int userID);
        List<string> SelectAllRoles();
        List<string> SelectRolesByUserID(int userID);
        int InsertOrDeleteUserRole(int userID, string role, bool delete = false);
    }
}
