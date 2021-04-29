using HomeManagement.Controllers;
using HomeManagement.DAL;
using HomeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace HomeManagementTests
{
    [TestClass]
    public class FinanceSheetControllerTests
    {
        [TestMethod]
        public void ValidFinanceSheetShouldCreateANewSheetAndReturn200()
        {
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(test).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m, Owners = { id } };
            var controller = new FinanceSheetController();
            var expectedResult = FinanceSheetDAL.GetSheets().Count;

            var response = controller.Create(JsonConvert.SerializeObject(sheet)) as ObjectResult;

            var actualResult = FinanceSheetDAL.GetSheets().Count;
            Assert.AreEqual(expectedResult + 1, actualResult);
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public void ValidGetRequestShouldReturnAListOfSheets()
        {
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(test).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m, Owners = { id } };
            FinanceSheet sheet2 = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m, Owners = { id } };
            FinanceSheet sheet3 = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m, Owners = { id } };
            FinanceSheetDAL.Create(sheet);
            FinanceSheetDAL.Create(sheet2);
            FinanceSheetDAL.Create(sheet3);
            var controller = new FinanceSheetController();

            var response = controller.GetUsersSheets(id) as ObjectResult;

            var result = response.Value as List<FinanceSheet>;
            var count = result.Count;
            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public void ValidGetRequestShouldReturnSheetData()
        {
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int userId = UserDAL.CreateUser(test).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m, Owners = { userId } };
            Income income = new Income() { Name = $"testincome-{Guid.NewGuid()}", Type = 1, Amount = 2000, Reoccurance = "One Time" };
            sheet.Incomes.Add(income);
            Expenditure expenditure = new Expenditure() { Name = $"testexpenditure-{Guid.NewGuid()}", Type = 1, Amount = 2000, Reoccurance = "One Time" };
            sheet.Expenditures.Add(expenditure);
            SavingGoal goal = new SavingGoal() { Name = $"testgoal-{Guid.NewGuid()}", Type = 1, Amount = 300000, EndDate = DateTime.Now.AddDays(30), Description = "" };
            sheet.SavingGoals.Add(goal);
            int sheetId = FinanceSheetDAL.Create(sheet);
            var controller = new FinanceSheetController();

            var response = controller.GetUserSheetData(userId, sheetId) as ObjectResult;
            var str = response.Value as string;
            var result = JsonConvert.DeserializeObject<FinanceSheet>(str);

            Assert.AreEqual(sheetId, result.Id);
            Assert.AreEqual(userId, result.Owners[0]);
            Assert.AreEqual(sheet.Name, result.Name);
            Assert.AreEqual(goal.Name, result.SavingGoals[0].Name);
            Assert.AreEqual(income.Name, result.Incomes[0].Name);
            Assert.AreEqual(expenditure.Name, result.Expenditures[0].Name);
        }

        [TestMethod]
        public void ValidIncomePostShouldCreateNewEntry()
        {
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(test).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m, Owners = { id } };
            int sheetId = FinanceSheetDAL.Create(sheet);
            Income income = new Income() { Name = $"testincome-{Guid.NewGuid()}", Type = 1, Amount = 2000, Reoccurance = "One Time", SheetId = sheetId };
            var controller = new FinanceSheetController();
            var expectedResult = FinanceSheetDAL.GetIncomes(sheetId).Count;

            var response = controller.CreateIncome(sheetId, JsonConvert.SerializeObject(income)) as ObjectResult;

            var actualResult = FinanceSheetDAL.GetIncomes(sheetId).Count;
            Assert.AreEqual(expectedResult + 1, actualResult);
        }

        [TestMethod]
        public void ValidExpenditurePostShouldCreateNewEntry()
        {
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(test).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m, Owners = { id } };
            int sheetId = FinanceSheetDAL.Create(sheet);
            Expenditure expenditure = new Expenditure() { Name = $"testincome-{Guid.NewGuid()}", Type = 1, Amount = 2000, Reoccurance = "One Time", SheetId = sheetId };
            var controller = new FinanceSheetController();
            var expectedResult = FinanceSheetDAL.GetExpenditures(sheetId).Count;

            var response = controller.CreateExpenditure(sheetId, JsonConvert.SerializeObject(expenditure)) as ObjectResult;

            var actualResult = FinanceSheetDAL.GetExpenditures(sheetId).Count;
            Assert.AreEqual(expectedResult + 1, actualResult);
        }

        [TestMethod]
        public void ValidGoalPostShouldCreateNewEntry()
        {
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(test).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m, Owners = { id } };
            int sheetId = FinanceSheetDAL.Create(sheet);
            SavingGoal goal = new SavingGoal() { Name = $"testgoal-{Guid.NewGuid()}", Type = 1, Amount = 300000, EndDate = DateTime.Now.AddDays(30), Description = "" };
            var controller = new FinanceSheetController();
            var expectedResult = FinanceSheetDAL.GetGoals(sheetId).Count;

            var response = controller.CreateSavingsGoal(sheetId, JsonConvert.SerializeObject(goal)) as ObjectResult;

            var actualResult = FinanceSheetDAL.GetGoals(sheetId).Count;
            Assert.AreEqual(expectedResult + 1, actualResult);
        }

        [TestMethod]
        public void GoalDepositShouldSubtractFromSheetAmount()
        {
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(test).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 2000.00m, Owners = { id } };
            int sheetId = FinanceSheetDAL.Create(sheet);
            SavingGoal goal = new SavingGoal() { Name = $"testgoal-{Guid.NewGuid()}", Type = 1, Amount = 30, EndDate = DateTime.Now.AddDays(30), Description = "" };
            int goalId;
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                goalId = FinanceSheetDAL.CreateGoals(new List<SavingGoal>() { goal }, sheetId, conn)[0];
                conn.Close();
            }
            var controller = new FinanceSheetController();

            var response = controller.AddGoalDeposit(sheetId, goalId, goal.Amount.ToString()) as ObjectResult;

            var actualResult = FinanceSheetDAL.GetSheetData(id, sheetId);
            Assert.AreEqual(sheet.Balance - goal.Amount, actualResult.Balance);
            Assert.AreEqual(goal.Amount, actualResult.SavingGoals[0].Amount);
        }

        [TestMethod]
        public void DeletionOfIncomeShouldRemoveAllReferences()
        {
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(test).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m, Owners = { id } };
            int sheetId = FinanceSheetDAL.Create(sheet);
            Income income = new Income() { Name = $"testincome-{Guid.NewGuid()}", Type = 1, Amount = 2000, Reoccurance = "Test", SheetId = sheetId };
            using var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;");
            conn.Open();
            var incomeId = FinanceSheetDAL.CreateIncomes(new List<Income>() { income }, sheetId, conn);
            conn.Close();
            var controller = new FinanceSheetController();

            controller.DeleteIncome(sheetId, incomeId[0]);

            Assert.AreEqual(false, FinanceSheetDAL.IncomeExists(income.Name, sheetId));
            Assert.AreEqual(false, FinanceSheetDAL.ReoccuranceExists(incomeId[0], 1));
        }

        [TestMethod]
        public void DeletionOfExpendituresShouldRemoveAllRefferences()
        {
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(test).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m, Owners = { id } };
            int sheetId = FinanceSheetDAL.Create(sheet);
            Expenditure expenditure = new Expenditure() { Name = $"testincome-{Guid.NewGuid()}", Type = 1, Amount = 2000, Reoccurance = "Test", SheetId = sheetId };
            using var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;");
            conn.Open();
            var expenditureId = FinanceSheetDAL.CreateExpenditures(new List<Expenditure>() { expenditure }, sheetId, conn);
            conn.Close();
            var controller = new FinanceSheetController();

            controller.DeleteExpenditure(sheetId, expenditureId[0]);

            Assert.AreEqual(false, FinanceSheetDAL.ExpenditureExists(expenditure.Name, sheetId));
            Assert.AreEqual(false, FinanceSheetDAL.ReoccuranceExists(expenditureId[0], 2));
        }

        [TestMethod]
        public void DeletionOfGoalsShouldRemoveAllRefferences()
        {
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(test).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m, Owners = { id } };
            int sheetId = FinanceSheetDAL.Create(sheet);
            SavingGoal goal = new SavingGoal() { Name = $"testgoal-{Guid.NewGuid()}", Type = 1, Amount = 30, EndDate = DateTime.Now.AddDays(30), Description = "" };
            using var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;");
            conn.Open();
            var goalId = FinanceSheetDAL.CreateGoals(new List<SavingGoal>() { goal }, sheetId, conn);
            conn.Close();
            var controller = new FinanceSheetController();

            controller.DeleteGoal(sheetId, goalId[0]);

            Assert.AreEqual(false, FinanceSheetDAL.GoalExists(goal.Name, sheetId));
            Assert.AreEqual(false, FinanceSheetDAL.ReoccuranceExists(goalId[0], 3));
        }

        [TestMethod]
        public void DeletionOfFinanceSheetsShouldRemoveAllRefferences()
        {
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(test).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m, Owners = { id } };
            SavingGoal goal = new SavingGoal() { Name = $"testgoal-{Guid.NewGuid()}", Type = 1, Amount = 30, EndDate = DateTime.Now.AddDays(30), Description = ""};
            Expenditure expenditure = new Expenditure() { Name = $"testincome-{Guid.NewGuid()}", Type = 1, Amount = 2000, Reoccurance = "Test" };
            Income income = new Income() { Name = $"testincome-{Guid.NewGuid()}", Type = 1, Amount = 2000, Reoccurance = "Test" };
            sheet.Incomes.Add(income);
            sheet.SavingGoals.Add(goal);
            sheet.Expenditures.Add(expenditure);
            var sheetId = FinanceSheetDAL.Create(sheet);
            var controller = new FinanceSheetController();

            controller.DeleteSheet(sheetId);

            Assert.AreEqual(false, FinanceSheetDAL.SheetExists(sheet.Name, id));
            Assert.AreEqual(false, FinanceSheetDAL.GoalExists(goal.Name, sheetId));
            Assert.AreEqual(false, FinanceSheetDAL.IncomeExists(income.Name, sheetId));
            Assert.AreEqual(false, FinanceSheetDAL.ExpenditureExists(expenditure.Name, sheetId));
        }
    }
}
