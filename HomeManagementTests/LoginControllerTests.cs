using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HomeManagement;
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
    public class LoginControllerTest
    {
        [TestMethod]
        public void ValidLoginShouldReturn200()
        {
            var username = $"testuser-{Guid.NewGuid()}";
            var pass = Guid.NewGuid().ToString();
            var controller = new LoginController();
            Login credintials = new Login() { username = username, password = pass };
            User test = new User() { Username = username, Password = pass, Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            var _ = UserDAL.CreateUser(test).Result;

            var response = controller.Post(JsonConvert.SerializeObject(credintials)) as ObjectResult;

            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public void ValidLoginShouldReturnJWT()
        {
            var username = $"testuser-{Guid.NewGuid()}";
            var pass = Guid.NewGuid().ToString();
            var controller = new LoginController();
            Login credintials = new Login() { username = username, password = pass };
            User test = new User() { Username = username, Password = pass, Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            var Id = UserDAL.CreateUser(test).Result;
            var token = JWTAuthenticationManager.Authenticate(Id, username, pass);

            var response = controller.Post(JsonConvert.SerializeObject(credintials)) as ObjectResult;

            Assert.AreEqual(token.Length, response.Value.ToString().Length);
        }

        [TestMethod]
        public void InvalidLoginShouldReturn404()
        {
            var username = $"testuser-{Guid.NewGuid()}";
            var pass = Guid.NewGuid().ToString();
            var controller = new LoginController();
            Login credintials = new Login() { username = username, password = pass };
            User test = new User() { Username = username, Password = pass + "123", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            var _ = UserDAL.CreateUser(test).Result;

            var response = controller.Post(JsonConvert.SerializeObject(credintials)) as ObjectResult;

            Assert.AreEqual(404, response.StatusCode);
        }

        [TestMethod]
        public void InvalidUsernameShouldReturnRelevantMessage()
        {
            var username = $"testuser-{Guid.NewGuid()}";
            var pass = Guid.NewGuid().ToString();
            var controller = new LoginController();
            Login credintials = new Login() { username = "fake_user", password = pass };
            User test = new User() { Username = username, Password = pass, Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            var _ = UserDAL.CreateUser(test).Result;

            var response = controller.Post(JsonConvert.SerializeObject(credintials)) as ObjectResult;

            Assert.AreEqual("The username you entered is invalid. Please check again!", response.Value);
        }

        [TestMethod]
        public void InvalidPasswordShouldReturnRelevantMessage()
        {
            var username = $"testuser-{Guid.NewGuid()}";
            var pass = Guid.NewGuid().ToString();
            var controller = new LoginController();
            Login credintials = new Login() { username = username, password = "fakepass" };
            User test = new User() { Username = username, Password = pass, Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            var _ = UserDAL.CreateUser(test).Result;

            var response = controller.Post(JsonConvert.SerializeObject(credintials)) as ObjectResult;

            Assert.AreEqual("The password you entered is invalid. Please check again!", response.Value);
        }

        [TestMethod]
        public void SearchQueryShouldReturnAccurateInformation()
        {
            var username = $"testuser-{Guid.NewGuid()}";
            User test = new User() { Username = username, Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            _ = UserDAL.CreateUser(test).Result;

            var result = UserDAL.GetUserByName(username.Substring(0, username.Length - 3));

            var isPresent = false;
            foreach(var user in result)
            {
                if (user.Username == username)
                    isPresent = true;
            }

            Assert.AreEqual(true, isPresent);
        }
    }
}
