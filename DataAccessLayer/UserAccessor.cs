using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using DataObjects;

namespace DataAccessLayer
{
    public class UserAccessor : IUserAccessor
    {
        public User AuthenticateUser(string email, string passwordHash)
        {
            User user = null;
            int result = 0;

            // Start the Connection
            var conn = DBConnection.GetConnection();

            // Get a command object
            var cmd = new SqlCommand("sp_authenticate_user");
            cmd.Connection = conn;

            // Set the command type
            cmd.CommandType = CommandType.StoredProcedure;

            // Add the parameters
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 250);
            cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 100);

            // Provide values for the parameters
            cmd.Parameters["@Email"].Value = email;
            cmd.Parameters["@PasswordHash"].Value = passwordHash;

            // Try to open the connection, execute and process results
            try
            {
                conn.Open();
                result = Convert.ToInt32(cmd.ExecuteScalar());
                if(result == 1)
                {
                    user = SelectUserByEmail(email);
                }
                else
                {
                    throw new ApplicationException("User not found!");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return user;
        }

        private User SelectUserByEmail(string email)
        {
            User user = null;

            // Get connection Object
            var conn = DBConnection.GetConnection();

            // Get command objects
            var cmd1 = new SqlCommand("sp_select_user_by_email", conn);
            var cmd2 = new SqlCommand("sp_select_roles_by_userid", conn);

            // Set command types
            cmd1.CommandType = CommandType.StoredProcedure;
            cmd2.CommandType = CommandType.StoredProcedure;

            // Set Command parameters
            cmd1.Parameters.Add("@Email", SqlDbType.NVarChar, 250);
            cmd2.Parameters.Add("@UserID", SqlDbType.Int);

            // Values for first command, cmd2 relies on this to set its parameters
            cmd1.Parameters["@Email"].Value = email;

            // Try to open connection, execute and process results
            try
            {
                conn.Open();

                var reader1 = cmd1.ExecuteReader();

                user = new User();

                user.Email = email;
                if (reader1.Read())
                {
                    user.UserID = reader1.GetInt32(0);
                    user.FirstName = reader1.GetString(1);
                    user.LastName = reader1.GetString(2);
                }
                else
                {
                    throw new ApplicationException("User not found!");
                }
                reader1.Close();

                // cmd2 parameter value can now be set
                cmd2.Parameters["@UserID"].Value = user.UserID;

                var reader2 = cmd2.ExecuteReader();
                List<string> roles = new List<string>();
                while (reader2.Read())
                {
                    roles.Add(reader2.GetString(0));
                }
                reader2.Close();
                user.Roles = roles;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return user;
        }

        public bool UpdatePasswordHash(int userID, string oldPassHash, string newPassHash)
        {
            bool result = false;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_update_password");
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@UserID", SqlDbType.Int);
            cmd.Parameters.Add("@OldPasswordHash", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@NewPasswordHash", SqlDbType.NVarChar, 100);

            cmd.Parameters["@UserID"].Value = userID;
            cmd.Parameters["@OldPasswordHash"].Value = oldPassHash;
            cmd.Parameters["@NewPasswordHash"].Value = newPassHash;

            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                result = (rowsAffected == 1);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public List<User> SelectUsersByActive(bool active = true)
        {
            List<User> users = new List<User>();

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_select_users_by_active");
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Active", SqlDbType.Bit);
            cmd.Parameters["@Active"].Value = active;

            try
            {
                conn.Open();
                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var user = new User();
                        user.UserID = reader.GetInt32(0);
                        user.FirstName = reader.GetString(1);
                        user.LastName = reader.GetString(2);
                        user.Email = reader.GetString(3);
                        user.Active = reader.GetBoolean(4);
                        users.Add(user);
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return users;
        }

        public int UpdateUser(User oldUser, User newUser)
        {
            int rows = 0;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_update_user", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EmployeeID", oldUser.UserID);

            cmd.Parameters.AddWithValue("@NewFirstName", newUser.FirstName);
            cmd.Parameters.AddWithValue("@NewLastName", newUser.LastName);
            cmd.Parameters.AddWithValue("@NewEmail", newUser.Email);

            cmd.Parameters.AddWithValue("@OldFirstName", oldUser.FirstName);
            cmd.Parameters.AddWithValue("@OldLastName", oldUser.LastName);
            cmd.Parameters.AddWithValue("@OldEmail", oldUser.Email);

            try
            {
                conn.Open();
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return rows;
        }

        public int InsertUser(User user)
        {
            int userID = 0;

            var conn = DBConnection.GetConnection();

            var cmd = new SqlCommand("sp_insert_user", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
            cmd.Parameters.AddWithValue("@LastName", user.LastName);
            cmd.Parameters.AddWithValue("@Email", user.Email);

            try
            {
                conn.Open();
                userID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return userID;
        }

        public int ActivateUser(int userID)
        {
            int rows = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_reactivate_user", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userID);

            try
            {
                conn.Open();
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return rows;
        }

        public int DeactivateUser(int userID)
        {
            int rows = 0;

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand("sp_deactivate_user", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userID);

            try
            {
                conn.Open();
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
            return rows;
        }

        public List<string> SelectAllRoles()
        {
            List<string> roles = new List<string>();

            // connection
            var conn = DBConnection.GetConnection();

            // command objects
            var cmd = new SqlCommand("sp_select_all_roles");
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                // open connection
                conn.Open();

                // execute the first command

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string role = reader.GetString(0);
                    roles.Add(role);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return roles;
        }

        public List<string> SelectRolesByUserID(int userID)
        {
            List<string> roles = new List<string>();

            // connection
            var conn = DBConnection.GetConnection();

            // command objects
            var cmd = new SqlCommand("sp_select_roles_by_userID");
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;

            // parameters
            cmd.Parameters.Add("@UserID", SqlDbType.Int);
            cmd.Parameters["@UserID"].Value = userID;

            try
            {
                // open connection
                conn.Open();

                // execute the first command

                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string role = reader.GetString(0);
                    roles.Add(role);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return roles;
        }

        public int InsertOrDeleteUserRole(int userID, string role, bool delete = false)
        {
            int rows = 0;

            string cmdText = delete ? "sp_delete_user_role" : "sp_insert_user_role";

            var conn = DBConnection.GetConnection();
            var cmd = new SqlCommand(cmdText, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserID", userID);
            cmd.Parameters.AddWithValue("@RoleID", role);

            try
            {
                conn.Open();
                rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return rows;
        }
    }
}
