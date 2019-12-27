using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataObjects;

namespace LogicLayer
{
    public interface IUserManager
    {
        User AuthenticateUser(string email, string password);
        bool UpdatePassword(int userID, string newPassword, string oldPassword);
        List<User> RetrieveUserListByActive(bool active = true);
        bool EditUser(User oldUser, User newUser);
        bool AddUser(User user);
        bool SetUserActiveState(bool active, int employeeID);
        List<string> RetrieveUserRoles(int userID);
        List<string> RetrieveUserRoles();
        bool DeleteUserRole(int userID, string role);
        bool AddUserRole(int userID, string role);
    }
}
