using HomeManagement.DAL;
using HomeManagement.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeManagementTests
{
    [TestClass]
    public class ReportTests
    {
        [TestMethod]
        public void ValidReportShouldCreateNewEntry()
        {
            User testUser = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(testUser).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 20.00m, Owners = { id } };
            int sheetId = FinanceSheetDAL.Create(sheet);
            sheet.Id = sheetId;
            int expectedResult = ReportDAL.GetReports(sheetId).Count;

            ReportDAL.CreateReport(sheet);

            int actualResult = ReportDAL.GetReports(sheetId).Count;
            Assert.AreEqual(expectedResult + 1, actualResult);
        }

        [TestMethod]
        public void ValidReportShouldEvaluateBasedOnIncomeExpenditure()
        {
            User testUser = new User() { Username = $"testuser-{Guid.NewGuid()}", Password = "pa223", Age = 20, Email = "fakeemail@gmail.com", PhoneNumber = "07779593201" };
            int id = UserDAL.CreateUser(testUser).Result;
            FinanceSheet sheet = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 0, Owners = { id } };
            sheet.Incomes.Add(new Income() { Name = $"testincome-{Guid.NewGuid()}", Type = 1, Amount = 100000, Reoccurance = "One Time" });
            sheet.Expenditures.Add(new Expenditure() { Name = $"testexpenditure-{Guid.NewGuid()}", Type = 1, Amount = 20000, Reoccurance = "One Time" });
            sheet.Id = FinanceSheetDAL.Create(sheet);
            FinanceSheet sheet2 = new FinanceSheet() { Name = $"Test-{Guid.NewGuid()}", Balance = 0, Owners = { id } };
            sheet2.Incomes.Add(new Income() { Name = $"testincome-{Guid.NewGuid()}", Type = 1, Amount = 100000, Reoccurance = "One Time" });
            sheet2.Expenditures.Add(new Expenditure() { Name = $"testexpenditure-{Guid.NewGuid()}", Type = 1, Amount = 60000, Reoccurance = "One Time" });
            sheet2.Id = FinanceSheetDAL.Create(sheet2);

            int reportId1 = ReportDAL.CreateReport(sheet);
            int reportId2 = ReportDAL.CreateReport(sheet2);

            Report report1 = ReportDAL.GetReport(reportId1);
            Report report2 = ReportDAL.GetReport(reportId2);
            Assert.AreEqual(5, report1.Rating);
            Assert.AreNotEqual(5, report2.Rating);
        }
    }
}
