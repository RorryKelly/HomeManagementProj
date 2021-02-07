using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HomeManagement.DAL;
using HomeManagement.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HomeManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public ActionResult Post([FromBody] string json)
        {
            var credintials = JsonConvert.DeserializeObject<Login>(json);
            var Id = UserDAL.GetUser(credintials.username, credintials.password).Result;

            if (Id == -1)
                return NotFound();

            return Ok(Id);
        }
    }
}
