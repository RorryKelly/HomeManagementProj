using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HomeManagement.Controllers;
using HomeManagement.DAL;
using HomeManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace HomeManagementTests
{
    [TestClass]
    public class RegisterControllerTests
    {
        [TestMethod]
        public void ValidRegistrationShouldReturn200()
        {
            var controller = new RegisterController();
            var user = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "fakepass", Email = "fakeemail@gmail.com", PhoneNumber = "07779593201", Age = 20 };

            var response = controller.Post(JsonConvert.SerializeObject(user)) as ObjectResult;

            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public void ValidRegistrationShouldCreateNewEntry()
        {
            var expectedResult = UserDAL.GetAllUsernames().Result;
            var user = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "fakepass", Email = "fakeemail@gmail.com", PhoneNumber = "07779593201", Age = 20 };
            var controller = new RegisterController();

            controller.Post(JsonConvert.SerializeObject(user));

            var actualResult = UserDAL.GetAllUsernames().Result;
            Assert.AreEqual(expectedResult.Count + 1, actualResult.Count);
        }

        [TestMethod]
        public void ValidRegistrationShouldReturnId()
        {
            var user = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "fakepass", Email = "fakeemail@gmail.com", PhoneNumber = "07779593201", Age = 20 };
            var controller = new RegisterController();

            var result = controller.Post(JsonConvert.SerializeObject(user)) as ObjectResult;

            var expectedResult = UserDAL.GetUser(user.Username, user.Password).Result;
            Assert.AreEqual(expectedResult, result.Value);
        }

        [TestMethod]
        public void DuplicateUserNamesShouldReturn401()
        {
            var username = $"testuser-{Guid.NewGuid()}";
            var user1 = new User() { Username = username, Password = "fakepass", Email = "fakeemail@gmail.com", PhoneNumber = "07779593201", Age = 20 };
            var user2 = new User() { Username = username, Password = "fakerpass123", Email = "fakeremail321@gmail.com", PhoneNumber = "07779595234", Age = 38 };
            var _ = UserDAL.CreateUser(user1).Result;
            var controller = new RegisterController();

            var result = controller.Post(JsonConvert.SerializeObject(user2)) as ObjectResult;

            Assert.AreEqual(401, result.StatusCode);
        }
    }
}
