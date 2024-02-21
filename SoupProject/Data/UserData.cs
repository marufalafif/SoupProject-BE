using MySql.Data.MySqlClient;
using SoupProject.Models;

namespace SoupProject.Data
{
    public class UserData
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public UserData(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        #region  Single SQL Command
        //Previous statement will be overridden by the last statement
        //public bool CreateUserAccount(User user, UserRole userRole)
        //{
        //    bool result = false;

        //    using (MySqlConnection connection = new MySqlConnection(_connectionString))
        //    {
        //        using (MySqlCommand command = new MySqlCommand())
        //        {
        //            command.Connection = connection;
        //            command.Parameters.Clear();

        //            command.CommandText = "INSERT INTO Users (Id, Username, Password) VALUES (@id, @username, @password)";

        //            command.Parameters.AddWithValue("@id", user.Id);
        //            command.Parameters.AddWithValue("@username", user.Username);
        //            command.Parameters.AddWithValue("@password", user.Password);


        //            command.CommandText = "INSERT INTO UserRoles (UserId, Role) VALUES (@userId, @role)";

        //            command.Parameters.AddWithValue("@userId", userRole.UserId);
        //            command.Parameters.AddWithValue("@role", userRole.Role);

        //            try
        //            {
        //                connection.Open();

        //                int execResult = command.ExecuteNonQuery();

        //                result = execResult > 0 ? true : false;
        //            }
        //            catch
        //            {
        //                throw;
        //            }
        //            finally
        //            {
        //                connection.Close();
        //            }
        //        }
        //    }

        //    return result;
        //}
        #endregion

        #region Multiple Sql command (without transaction)
        // If first statement No OK, then next statment will be No OK
        // If first statment OK, then next statement No Ok, only first statement will be inserted

        //public bool CreateUserAccount(User user, UserRole userRole)
        //{
        //    bool result = false;

        //    using (MySqlConnection connection = new MySqlConnection(_connectionString))
        //    {
        //        MySqlCommand command1 = new MySqlCommand();
        //        command1.Connection = connection;
        //        command1.Parameters.Clear();

        //        command1.CommandText = "INSERT INTO Users (Id, Username, Password) VALUES (@id, @username, @password)";
        //        command1.Parameters.AddWithValue("@id", user.Id);
        //        command1.Parameters.AddWithValue("@username", user.Username);
        //        command1.Parameters.AddWithValue("@password", user.Password);


        //        MySqlCommand command2 = new MySqlCommand();
        //        command2.Connection = connection;
        //        command2.Parameters.Clear();

        //        command2.CommandText = "INSERT INTO UserRoles (UserId, Role) VALUES (@userId, @role)";
        //        command2.Parameters.AddWithValue("@userId", userRole.UserId);
        //        command2.Parameters.AddWithValue("@role", userRole.Role);

        //        try
        //        {
        //            connection.Open();
        //            var result1 = command1.ExecuteNonQuery();
        //            var result2 = command2.ExecuteNonQuery();

        //            if (result1 > 0 && result2 > 0)
        //                result = true;
        //        }
        //        catch(Exception ex)
        //        {
        //            throw;
        //        }
        //        finally
        //        {
        //            connection.Close();
        //        }
        //    }

        //    return result;
        //}
        #endregion

        public List<User> GetAll()
        {
            List<User> users = new List<User>();

            string query = "SELECT * FROM user";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                User user = new User
                                {
                                    userId = Guid.Parse(reader["userId"].ToString() ?? string.Empty),
                                    username = reader["username"].ToString() ?? string.Empty,
                                    email = reader["email"].ToString() ?? string.Empty,
                                    password = reader["password"].ToString() ?? string.Empty,
                                    role = reader["role"].ToString() ?? string.Empty,
                                    isActivated = Convert.ToBoolean(reader["isActivated"])
                                };

                                users.Add(user);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return users;
        }

        public User GetUserById(Guid userId)
        {
            User user = null;

            string query = "SELECT * FROM user WHERE userId = @userId";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);

                    try
                    {
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new User
                                {
                                    userId = Guid.Parse(reader["userId"].ToString()),
                                    username = reader["username"].ToString(),
                                    email = reader["email"].ToString(),
                                    password = reader["password"].ToString(),
                                    role = reader["role"].ToString(),
                                    isActivated = Convert.ToBoolean(reader["isActivated"])
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return user;
        }

        public bool UpdateUserAccount(Guid userId, User user)
        {
            bool result = false;

            string query = "UPDATE user SET username = @username, email = @email, role = @role, isActivated = @isActivated " +
                "WHERE userId = @userId";


            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.Parameters.Clear();

                    command.CommandText = query;

                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@username", user.username);
                    command.Parameters.AddWithValue("@email", user.email);
                    command.Parameters.AddWithValue("@role", user.role);
                    command.Parameters.AddWithValue("@isActivated", user.isActivated);

                    try
                    {
                        connection.Open();

                        result = command.ExecuteNonQuery() > 0 ? true : false;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    { connection.Close(); }
                }

            }

            return result;
        }

        #region Multiple Sql command (with transaction)
        public bool CreateUserAccount(User user)
        {
            bool result = false;

            string query = "INSERT INTO user (userId, username, email, password, role, isActivated) " +
                "VALUES (@userId, @username, @email, @password, @role, @isActivated)";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.Parameters.Clear();

                    command.CommandText = query;

                    command.Parameters.AddWithValue("@userId", user.userId);
                    command.Parameters.AddWithValue("@username", user.username);
                    command.Parameters.AddWithValue("@email", user.email);
                    command.Parameters.AddWithValue("@password", user.password);
                    command.Parameters.AddWithValue("@role", user.role);
                    command.Parameters.AddWithValue("@isActivated", user.isActivated);

                    try
                    {
                        connection.Open();

                        result = command.ExecuteNonQuery() > 0 ? true : false;
                    }
                    catch
                    {

                    }
                    finally
                    { connection.Close(); }
                }

            }

            return result;
        }
        #endregion

        public User? CheckUserAuth(string email)
        {
            User? user = null;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * From user WHERE email = @email";

                    command.Parameters.Clear();

                    command.Parameters.AddWithValue("@email", email);

                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = new User
                            {
                                userId = Guid.Parse(reader["userId"].ToString() ?? string.Empty),
                                username = reader["username"].ToString() ?? string.Empty,
                                email = reader["email"].ToString() ?? string.Empty,
                                password = reader["password"].ToString() ?? string.Empty,
                                role = reader["role"].ToString() ?? string.Empty,
                                isActivated = Convert.ToBoolean(reader["isActivated"])
                            };
                        }
                    }

                    connection.Close();

                }
            }

            return user;
        }

        public User? GetUserRole(Guid userId)
        {
            User? userRole = null;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.Parameters.Clear();

                    command.CommandText = "SELECT * FROM user WHERE userId = @userId";
                    command.Parameters.AddWithValue("@userId", userId);


                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userRole = new User
                            {
                                userId = Guid.Parse(reader["userId"].ToString() ?? string.Empty),
                                username = reader["username"].ToString() ?? string.Empty,
                                //email = reader["email"].ToString() ?? string.Empty,
                                //password = reader["password"].ToString() ?? string.Empty,
                                role = reader["role"].ToString() ?? string.Empty
                            };

                        }
                    }

                    connection.Close();

                }
            }

            return userRole;
        }
        public bool ActivateUser(Guid userId)
        {
            bool result = false;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                MySqlCommand command = new MySqlCommand();
                command.Connection = connection;
                command.Parameters.Clear();

                command.CommandText = "UPDATE user SET isActivated = 1 WHERE userId = @userId";
                command.Parameters.AddWithValue("@userId", userId);

                connection.Open();
                result = command.ExecuteNonQuery() > 0 ? true : false;

                connection.Close();

            }

            return result;
        }

        public bool ResetPassword(string email, string password)
        {
            bool result = false;

            string query = "UPDATE user SET password = @password WHERE email = @email";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.Parameters.Clear();

                    command.CommandText = query;

                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@password", password);

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0 ? true : false;

                    connection.Close();
                }
            }

            return result;
        }
    }
}
