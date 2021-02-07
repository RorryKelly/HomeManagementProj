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

namespace HomeManagement.Tests
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
            var id = UserDAL.CreateUser(test).Result;

            var response = controller.Post(JsonConvert.SerializeObject(credintials)) as ObjectResult;

            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public void ValidLoginShouldReturnId()
        {
            var username = $"testuser-{Guid.NewGuid()}";
            var pass = Guid.NewGuid().ToString();
            var controller = new LoginController();
            Login credintials = new Login() { username = username, password = pass };
            User test = new User() { Username = username, Password = pass, Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            var id = UserDAL.CreateUser(test).Result;

            var response = controller.Post(JsonConvert.SerializeObject(credintials)) as ObjectResult;

            Assert.AreEqual(id, response.Value);
        }

        [TestMethod]
        public void InvalidLoginShouldReturn404()
        {
            var username = $"testuser-{Guid.NewGuid()}";
            var pass = Guid.NewGuid().ToString();
            var controller = new LoginController();
            Login credintials = new Login() { username = username, password = pass };
            User test = new User() { Username = username, Password = pass + "123", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            var id = UserDAL.CreateUser(test).Result;

            var response = controller.Post(JsonConvert.SerializeObject(credintials)) as StatusCodeResult;

            Assert.AreEqual(404, response.StatusCode);
        }
    }
}
