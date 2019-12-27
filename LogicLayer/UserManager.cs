using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DataObjects;
using System.Security.Cryptography; // Used for Hashing the password

namespace LogicLayer
{
    public class UserManager : IUserManager
    {
        private IUserAccessor _userAccessor;

        public UserManager()
        {
            _userAccessor = new UserAccessor();
        }

        public UserManager(IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
        }

        public bool AddUser(User user)
        {
            bool result = true;
            try
            {
                result = _userAccessor.InsertUser(user) > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("User not added", ex);
            }
            return result;
        }

        public bool AddUserRole(int userID, string role)
        {
            bool result = false;
            try
            {
                result = (1 == _userAccessor.InsertOrDeleteUserRole(userID, role));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Role not added!", ex);
            }
            return result;
        }

        public User AuthenticateUser(string email, string password)
        {
            User user = null;
            try
            {
                var passwordHash = hashPassword(password);
                password = null;

                user = _userAccessor.AuthenticateUser(email, passwordHash);
            }
            catch(Exception ex)
            {
                throw new ApplicationException("Bad email address or password!", ex);
            }
            return user;
        }

        public bool DeleteUserRole(int userID, string role)
        {
            bool result = false;
            try
            {
                result = (1 == _userAccessor.InsertOrDeleteUserRole(userID, role, delete: true));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Role not removed!", ex);
            }
            return result;
        }

        public bool EditUser(User oldUser, User newUser)
        {
            bool result = false;

            try
            {
                result = _userAccessor.UpdateUser(oldUser, newUser) == 1;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Update failed", ex);
            }

            return result;
        }

        public List<User> RetrieveUserListByActive(bool active = true)
        {
            try
            {
                return _userAccessor.SelectUsersByActive(active);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Data not found.", ex);
            }
        }

        public List<string> RetrieveUserRoles(int userID)
        {
            List<string> roles = null;

            try
            {
                roles = _userAccessor.SelectRolesByUserID(userID);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Roles not found", ex);
            }

            return roles;
        }

        public List<string> RetrieveUserRoles()
        {
            List<string> roles = null;

            try
            {
                roles = _userAccessor.SelectAllRoles();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Roles not found", ex);
            }

            return roles;
        }

        public bool SetUserActiveState(bool active, int userID)
        {
            bool result = false;
            try
            {
                if (active)
                {
                    result = 1 == _userAccessor.ActivateUser(userID);
                }
                else
                {
                    result = 1 == _userAccessor.DeactivateUser(userID);
                }
                if (result == false)
                {
                    throw new ApplicationException("User record not updated.");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Update failed!", ex);
            }
            return result;
        }

        public bool UpdatePassword(int userID, string newPassword, string oldPassword)
        {
            bool isUpdated = false;
            string newPasswordHash = hashPassword(newPassword);
            string oldPasswordHash = hashPassword(oldPassword);

            try
            {
                isUpdated = _userAccessor.UpdatePasswordHash(userID,
                   oldPasswordHash, newPasswordHash);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Update Failed", ex);
            }
            return isUpdated;
        }

        private string hashPassword(string source)
        {
            string result = "";

            // Create a byte array
            byte[] data;

            // Resource Intensive
            using(SHA256 sha256hash = SHA256.Create())
            {
                data = sha256hash.ComputeHash(Encoding.UTF8.GetBytes(source));
                var s = new StringBuilder();
                // Loop through the bytes and build the string
                for (int i = 0; i < data.Length; i++)
                {
                    s.Append(data[i].ToString("x2"));
                }
                result = s.ToString().ToUpper();
            }

            return result;
        }

    }
}
