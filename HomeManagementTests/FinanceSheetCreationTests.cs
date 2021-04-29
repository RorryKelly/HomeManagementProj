using HomeManagement.DAL;
using HomeManagement.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeManagementTests
{
    [TestClass]
    public class FinanceSheetCreationTests
    {
        [TestMethod]
        public void ValidFinanceSheetShouldCreateNewEntry()
        {
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(test).Result;
            int expectedResult = FinanceSheetDAL.GetSheets().Count;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m };
            sheet.Owners.Add(id);

            FinanceSheetDAL.Create(sheet);

            int actualResult = FinanceSheetDAL.GetSheets().Count;
            Assert.AreEqual(expectedResult + 1, actualResult);
        }

        [TestMethod]
        public void ValidFinanceSheetShouldInsertExpendituresOnCreation()
        {
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m };
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int userId = UserDAL.CreateUser(test).Result;
            sheet.Owners.Add(userId);
            Expenditure expenditure = new Expenditure() { Name = $"testexpenditure-{Guid.NewGuid()}", Type = 1, Amount = 2000, Reoccurance = "One Time" };
            sheet.Expenditures.Add(expenditure);

            int sheetId = FinanceSheetDAL.Create(sheet);
            int actualResult = FinanceSheetDAL.GetExpenditures(sheetId).Count;

            Assert.AreEqual(1, actualResult);
        }

        [TestMethod]
        public void ValidFinanceSheetShouldInsertIncomesOnCreation()
        {
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m };
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int userId = UserDAL.CreateUser(test).Result;
            sheet.Owners.Add(userId);
            Income income = new Income() { Name = $"testincome-{Guid.NewGuid()}",Type = 1, Amount = 2000, Reoccurance = "One Time" };
            sheet.Incomes.Add(income);


            int sheetId = FinanceSheetDAL.Create(sheet);
            int actualResult = FinanceSheetDAL.GetIncomes(sheetId).Count;

            Assert.AreEqual(1, actualResult);
        }

        [TestMethod]
        public void ValidFinanceSheetShouldInsertSavingsOnCreation()
        {
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m };
            User test = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int userId = UserDAL.CreateUser(test).Result;
            sheet.Owners.Add(userId);
            SavingGoal goal = new SavingGoal() { Name = $"testgoal-{Guid.NewGuid()}", Type = 1, Amount = 300000, EndDate = DateTime.Now.AddDays(30), Description = ""};
            sheet.SavingGoals.Add(goal);

            int sheetId = FinanceSheetDAL.Create(sheet);
            int actualResult = FinanceSheetDAL.GetGoals(sheetId).Count;

            Assert.AreEqual(1, actualResult);
        }
    }
}
