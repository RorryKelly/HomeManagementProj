using HomeManagement.Models;
using System;
using System.Collections.Generic;
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
                var query = new SqlCommand($"SELECT id FROM Users WHERE Username = '{username}' AND Password = '{password}';", conn);
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
                                           $"Values('{user.Username}', '{user.Password}', '{user.Email}', '{user.PhoneNumber}', {user.Age}); "
                                           , conn);
                var reader = query.ExecuteReader();
                reader.Read();
                var result = int.Parse(reader[0].ToString());
                conn.Close();

                return result;
            }
        }
    }
}