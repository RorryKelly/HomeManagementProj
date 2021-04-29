using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using HomeManagement.DAL;
using HomeManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HomeManagement.Controllers
{
    [ApiController]
    public class ReportController : ControllerBase
    {
        [Route("api/report")]
        [HttpPost]
        public ActionResult Create([FromBody] string json)
        {
            var sheet = JsonConvert.DeserializeObject<FinanceSheet>(json);
            sheet = FinanceSheetDAL.GetSheetData(sheet.Owners[0], sheet.Id);
            int Id = ReportDAL.CreateReport(sheet);
            return Ok(Id);
        }

        [Route("api/report/{reportId}")]
        [HttpGet]
        public ActionResult GetReport(int reportId)
        {
            var report = ReportDAL.GetReport(reportId);
            return Ok(report);
        }
    }
}
