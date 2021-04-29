using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeManagement.DAL;
using HomeManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HomeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        [HttpPost]
        public ActionResult Post([FromBody] string json)
        {
            var credintials = JsonConvert.DeserializeObject<User>(json);
            bool usernameExist = UserDAL.GetIdByUsername(credintials.Username).Result != -1;
            if (usernameExist)
                return Unauthorized(-1);

            var id = UserDAL.CreateUser(credintials).Result;
            return Ok(id);
        }
    }
}
