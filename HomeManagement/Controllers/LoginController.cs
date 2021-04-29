using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HomeManagement.DAL;
using HomeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HomeManagement.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        [Route("api/[controller]")]
        [HttpPost]
        public ActionResult Post([FromBody] string json)
        {
            var credintials = JsonConvert.DeserializeObject<Login>(json);
            var Id = UserDAL.GetUser(credintials.username, credintials.password).Result;

            if (Id == -1)
            {
                if (UserDAL.GetIdByUsername(credintials.username).Result == -1)
                    return NotFound("The username you entered is invalid. Please check again!");
                else
                    return NotFound("The password you entered is invalid. Please check again!");
            }

            var token = JWTAuthenticationManager.Authenticate(Id, credintials.username, credintials.password);
            return Ok(token);
        }

        [Route("api/getUsers/{query}")]
        [HttpGet]
        public ActionResult GetUsers(string query)
        {
            var result = UserDAL.GetUserByName(query);
            return Ok(result);
        }
    }
}
