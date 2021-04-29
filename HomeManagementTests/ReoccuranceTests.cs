using HomeManagement.DAL;
using HomeManagement.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HomeManagementTests
{
    [TestClass]
    public class ReoccuranceTests
    {
        [TestMethod]
        public void ReoccurancesShouldHaveEachInstanceAsUniqueEntry()
        {
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(test).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m, Owners = { id } };
            Income income = new Income() { Name = $"testincome-{Guid.NewGuid()}", Type = 1, Amount = 2000, Reoccurance = "Test" };
            sheet.Incomes.Add(income);
            Expenditure expenditure = new Expenditure() { Name = $"testincome-{Guid.NewGuid()}", Type = 1, Amount = 2000, Reoccurance = "Test" };
            sheet.Expenditures.Add(expenditure);
            SavingGoal goal = new SavingGoal() { Name = $"testgoal-{Guid.NewGuid()}", Type = 1, Amount = 300000, EndDate = DateTime.Now.AddDays(30), Description = ""};
            sheet.SavingGoals.Add(goal);
            var expectedResult = ReoccuranceDAL.GetReoccurances();

            int sheetId = FinanceSheetDAL.Create(sheet);

            var actualResult = ReoccuranceDAL.GetReoccurances();
            Assert.AreEqual(expectedResult.Count + 3, actualResult.Count);
        }

        [TestMethod]
        public void ReoccurancesShouldBeCheckedAndActedOnWhenDateHasPassed()
        {
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(test).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m, Owners = { id } };
            Income income = new Income() { Name = $"testincome-{Guid.NewGuid()}", Type = 1, Amount = 2000, Reoccurance = "Test" };
            sheet.Incomes.Add(income);
            int sheetId = FinanceSheetDAL.Create(sheet);
            var expectedResult = ReoccuranceDAL.GetSheetReoccurances(sheetId);
            Thread.Sleep(1000);

            ReoccuranceDAL.CheckReoccurances();

            var actualResult = ReoccuranceDAL.GetSheetReoccurances(sheetId);
            Assert.AreNotEqual(expectedResult[0].Date, actualResult[0].Date);
        }

        [TestMethod]
        public void ReoccurancesShouldEffectTheFinanceSheetItBelongsTo_Income()
        {
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(test).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 2000.00m, Owners = { id } };
            Income income = new Income() { Name = $"testincome-{Guid.NewGuid()}", Type = 1, Amount = 2000, Reoccurance = "Test" };
            sheet.Incomes.Add(income);
            int sheetId = FinanceSheetDAL.Create(sheet);
            Thread.Sleep(1000);

            ReoccuranceDAL.CheckReoccurances();

            var result = FinanceSheetDAL.GetSheetData(id, sheetId);
            Assert.AreEqual(sheet.Balance + income.Amount, result.Balance);
        }

        [TestMethod]
        public void ReoccurancesShouldEffectTheFinanceSheetItBelongsTo_Expenditure()
        {
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(test).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 2000.00m, Owners = { id } };
            Expenditure expenditure = new Expenditure() { Name = $"testincome-{Guid.NewGuid()}", Type = 1, Amount = 2000, Reoccurance = "Test" };
            sheet.Expenditures.Add(expenditure);
            int sheetId = FinanceSheetDAL.Create(sheet);
            Thread.Sleep(1000);

            ReoccuranceDAL.CheckReoccurances();

            var result = FinanceSheetDAL.GetSheetData(id, sheetId);
            Assert.AreEqual(sheet.Balance - expenditure.Amount, result.Balance);
        }
    }
}
