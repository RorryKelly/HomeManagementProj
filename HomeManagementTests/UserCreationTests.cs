using HomeManagement.DAL;
using HomeManagement.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HomeManagementTests
{
    [TestClass]
    public class UserCreationTests
    {
        [TestMethod]
        public void ValidAccountShouldCreateNewEntry()
        {
            var expectedResult = UserDAL.GetAllUsernames().Result;
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };

            UserDAL.CreateUser(test);
            var actualResult = UserDAL.GetAllUsernames().Result;

            Assert.AreEqual(expectedResult.Count + 1, actualResult.Count);
        }

        [TestMethod]
        public void ReplicaUserNamesShouldNotCreateNewEntry()
        {
            string username = $"testuser-{Guid.NewGuid()}";
            var expectedResult = UserDAL.GetAllUsernames().Result;
            User test = new User() { Username = username, Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            User test2 = new User() { Username = username, Password = "rwe", Age = 20, Email = "fakerrremail@gmail.com", PhoneNumber = "077795252201" };

            UserDAL.CreateUser(test);
            UserDAL.CreateUser(test2);
            var actualResult = UserDAL.GetAllUsernames().Result;

            Assert.AreEqual(expectedResult.Count + 1, actualResult.Count);
        }

        [TestMethod]
        public void ReplicaDataExceptForUsernamesShouldCreateNewEntry()
        {
            var expectedResult = UserDAL.GetAllUsernames().Result;
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            User test2 = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };

            UserDAL.CreateUser(test);
            UserDAL.CreateUser(test2);
            var actualResult = UserDAL.GetAllUsernames().Result;

            Assert.AreEqual(expectedResult.Count + 2, actualResult.Count);
        }
    }
}
