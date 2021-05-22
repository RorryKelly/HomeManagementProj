using Microsoft.AspNetCore.Mvc;
using HomeManagement.DAL;
using HomeManagement.Models;
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
