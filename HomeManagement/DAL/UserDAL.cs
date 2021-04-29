using HomeManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HomeManagement.DAL
{
    public class UserDAL
    {
        public async static Task<List<string>> GetAllUsernames()
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand("SELECT username FROM Users;", conn);
                var reader = query.ExecuteReader();
                var result = new List<string>();
                while (reader.Read())
                {
                    result.Add(reader[0].ToString());
                }
                conn.Close();

                return result;
            }
        }

        public async static Task<int> GetUser(string username, string password)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"SELECT id FROM Users WHERE Username = @username AND Password = @password;", conn);
                query.Parameters.Add("@username", SqlDbType.NVarChar);
                query.Parameters.Add("@password", SqlDbType.NVarChar);
                query.Parameters["@username"].Value = username;
                query.Parameters["@password"].Value = password;
                var reader = query.ExecuteReader();
                reader.Read();

                if (reader.HasRows == false)
                    return -1;

                var result = int.Parse(reader[0].ToString());
                conn.Close();

                return result;
            }
        }

        public async static Task<int> GetIdByUsername(string username)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"SELECT id FROM Users WHERE Username = @username;", conn);
                query.Parameters.Add("@username", SqlDbType.NVarChar);
                query.Parameters["@username"].Value = username;

                var reader = query.ExecuteReader();
                reader.Read();

                if (reader.HasRows == false)
                    return -1;

                var result = int.Parse(reader[0].ToString());
                conn.Close();

                return result;
            }
        }

        public async static Task<int> CreateUser(User user)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand("INSERT INTO Users(Username, Password, Email, PhoneNumber, Age) " +
                                           "OUTPUT Inserted.ID " +
                                           $"Values(@username, @password, @email, @phonenumber, @age); "
                                           , conn);
                query.Parameters.Add("@username", SqlDbType.NVarChar);
                query.Parameters["@username"].Value = user.Username;
                query.Parameters.Add("@password", SqlDbType.NVarChar);
                query.Parameters["@password"].Value = user.Password;
                query.Parameters.Add("@email", SqlDbType.NVarChar);
                query.Parameters["@email"].Value = user.Email;
                query.Parameters.Add("@phonenumber", SqlDbType.NVarChar);
                query.Parameters["@phonenumber"].Value = user.PhoneNumber;
                query.Parameters.Add("@age", SqlDbType.Int);
                query.Parameters["@age"].Value = user.Age;
                var reader = query.ExecuteReader();
                reader.Read();
                var result = int.Parse(reader[0].ToString());
                conn.Close();

                return result;
            }
        }

        public static List<User> GetUserByName(string q)
        {
            List<User> result = new List<User>();
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand("SELECT Id, Username, Email, PhoneNumber, Age "+
                                           "FROM Users " +
                                           $"WHERE Username LIKE '%{q}%'; ", conn);

                var reader = query.ExecuteReader();
                while(reader.Read())
                {
                    result.Add(new User { Id = int.Parse(reader[0].ToString()), Username = reader[1].ToString(), Email = reader[2].ToString(), PhoneNumber = reader[3].ToString(), Age = int.Parse(reader[4].ToString()) });
                }
                conn.Close();
            }
            return result;
        }

        public static User GetUserById(int id)
        {
            User result = new User();
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand("SELECT Id, Username, Email, PhoneNumber, Age " +
                                           "FROM Users " +
                                          $"WHERE Id=@id; ", conn);
                query.Parameters.Add("@id", SqlDbType.Int);
                query.Parameters["@id"].Value = id;

                var reader = query.ExecuteReader();
                while (reader.Read())
                {
                    result = new User { Id = int.Parse(reader[0].ToString()), Username = reader[1].ToString(), Email = reader[2].ToString(), PhoneNumber = reader[3].ToString(), Age = int.Parse(reader[4].ToString()) };
                }
                conn.Close();
            }
            return result;
        }
    }
}