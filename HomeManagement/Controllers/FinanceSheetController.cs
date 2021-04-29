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
    public class FinanceSheetController : ControllerBase
    {
        [Route("api/[controller]")]
        [HttpPost]
        public ActionResult Create([FromBody] string json)
        {
            var sheet = JsonConvert.DeserializeObject<FinanceSheet>(json);
            foreach(int owner in sheet.Owners)
            {
                if (FinanceSheetDAL.SheetExists(sheet.Name, owner))
                {
                    var user = UserDAL.GetUserById(owner);
                    return Conflict($"The finance name entered already exists under {user.Username}'s account");
                }
            }

            int id = FinanceSheetDAL.Create(sheet);
            return Ok(id);
        }

        [Route("api/sheet/{sheetId}/income")]
        [HttpPost]
        public ActionResult CreateIncome(int sheetId, [FromBody] string json)
        {
            var income = JsonConvert.DeserializeObject<Income>(json);
            if (FinanceSheetDAL.IncomeExists(income.Name, sheetId))
                return Conflict($"{income.Name} has already been taken in this sheet, please try another name");

            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                FinanceSheetDAL.CreateIncomes(new List<Income>() { income }, sheetId, conn);
                conn.Close();
            }
            return Ok();
        }

        [Route("api/sheet/{sheetId}/expenditure")]
        [HttpPost]
        public ActionResult CreateExpenditure(int sheetId, [FromBody] string json)
        {
            var expenditure = JsonConvert.DeserializeObject<Expenditure>(json);
            if (FinanceSheetDAL.ExpenditureExists(expenditure.Name, sheetId))
                return Conflict($"{expenditure.Name} has already been taken in this sheet, please try another name");

            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                FinanceSheetDAL.CreateExpenditures(new List<Expenditure>() { expenditure }, sheetId, conn);
                conn.Close();
            }
            return Ok();
        }

        [Route("api/sheet/{sheetId}/goal")]
        [HttpPost]
        public ActionResult CreateSavingsGoal(int sheetId, [FromBody] string json)
        {
            var goal = JsonConvert.DeserializeObject<SavingGoal>(json);
            if (FinanceSheetDAL.GoalExists(goal.Name, sheetId))
                return Conflict($"{goal.Name} has already been taken in this sheet, please try another name");

            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                FinanceSheetDAL.CreateGoals(new List<SavingGoal>() { goal }, sheetId, conn);
                conn.Close();
            }
            return Ok();
        }

        [Route("api/sheet/{sheetId}/goal/{goalId}/deposit")]
        [HttpPost]
        public ActionResult AddGoalDeposit(int sheetId, int goalId, [FromBody] string amount)
        {
            decimal dAmount = decimal.Parse(amount);
            FinanceSheetDAL.AddDeposit(sheetId, goalId, dAmount);
            return Ok();
        }

        [Route("api/sheet/{sheetId}/owners/add/{ownerId}")]
        [HttpPost]
        public ActionResult AddOwner(int sheetId, int ownerId)
        {
            FinanceSheetDAL.AddOwner(sheetId, ownerId);
            return Ok();
        }

        [Route("api/sheet/{sheetId}/delete")]
        [HttpDelete]
        public ActionResult DeleteSheet(int sheetId)
        {
            FinanceSheetDAL.Delete(sheetId);
            return Ok();
        }

        [Route("api/sheet/{sheetId}/income/{incomeId}/delete")]
        [HttpDelete]
        public ActionResult DeleteIncome(int sheetId, int incomeId)
        {
            FinanceSheetDAL.DeleteIncome(new List<int>() { incomeId });
            return Ok();
        }

        [Route("api/sheet/{sheetId}/expenditure/{expenditureId}/delete")]
        [HttpDelete]
        public ActionResult DeleteExpenditure(int sheetId, int expenditureId)
        {
            FinanceSheetDAL.DeleteExpenditure(new List<int>() { expenditureId });
            return Ok();
        }

        [Route("api/sheet/{sheetId}/goal/{goalId}/delete")]
        [HttpDelete]
        public ActionResult DeleteGoal(int sheetId, int goalId)
        {
            FinanceSheetDAL.DeleteGoal(new List<int>() { goalId });
            return Ok();
        }

        [Route("api/UsersFinanceSheets/{id}")]
        [HttpGet]
        public ActionResult GetUsersSheets(int id)
        {
            var result = FinanceSheetDAL.GetUsersSheets(id);
            return Ok(result);
        }

        [Route("api/user/{userId}/sheet/{sheetId}")]
        [HttpGet]
        public ActionResult GetUserSheetData(int userId, int sheetId)
        {
            var result = FinanceSheetDAL.GetSheetData(userId, sheetId);
            if (result.Id == 0)
                return NotFound();
            return Ok(JsonConvert.SerializeObject(result));
        }
    }
}