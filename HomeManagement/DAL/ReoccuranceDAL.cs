using HomeManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HomeManagement.DAL
{
    public class ReoccuranceDAL
    {
        public static List<Reoccurance> GetReoccurances()
        {
            List<Reoccurance> result = new List<Reoccurance>();
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"SELECT [Id], [FinanceSheetId], [PaymentId], [PaymentType], [ReoccuranceType], [Date] FROM [HomeManagementDB].[dbo].[Reoccurances]", conn);
                var reader = query.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Reoccurance() { Id = int.Parse(reader[0].ToString()), FinanceSheetId = int.Parse(reader[1].ToString()), PaymentId = int.Parse(reader[2].ToString()), PaymentType = int.Parse(reader[3].ToString()), ReoccuranceType = reader[4].ToString(), Date = DateTime.Parse(reader[5].ToString()) });
                }
                conn.Close();
            }

            return result;
        }

        public static void AddReoccurance(Reoccurance reoccurance)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"INSERT INTO [dbo].[Reoccurances]([FinanceSheetId], [PaymentId], [PaymentType], [ReoccuranceType], [Date]) " +
                                           $"VALUES({reoccurance.FinanceSheetId}, {reoccurance.PaymentId}, {reoccurance.PaymentType}, '{reoccurance.ReoccuranceType}', CONVERT(datetime, '{reoccurance.Date}', 103))", conn);
                query.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static void AddReoccurance(Income income)
        {
            DateTime date = new DateTime();
            switch(income.Reoccurance)
            {
                case "Test":
                    date = DateTime.Now.AddSeconds(1);
                    break;
                case "Hourly":
                    date = DateTime.Now.AddHours(1);
                    break;
                case "Bi-Daily":
                    date = DateTime.Now.AddHours(12);
                    break;
                case "Daily":
                    date = DateTime.Now.AddDays(1);
                    break;
                case "Bi-Weekly":
                    date = DateTime.Now.AddDays(3);
                    break;
                case "Weekly":
                    date = DateTime.Now.AddDays(7);
                    break;
                case "Bi-Monthly":
                    date = DateTime.Now.AddDays(14);
                    break;
                case "Monthly":
                    date = DateTime.Now.AddMonths(1);
                    break;
                case "Bi-Anually":
                    date = DateTime.Now.AddMonths(6);
                    break;
                case "Anually":
                    date = DateTime.Now.AddYears(1);
                    break;
            }
            var reoccurance = new Reoccurance()
            {
                FinanceSheetId = income.SheetId,
                PaymentId = income.Id,
                PaymentType = 1,
                ReoccuranceType = income.Reoccurance,
                Date = date
            };

            if(income.Reoccurance != "One Time")
                AddReoccurance(reoccurance);
        }

        public static void AddReoccurance(Expenditure expenditure)
        {
            DateTime date = new DateTime();
            switch (expenditure.Reoccurance)
            {
                case "Test":
                    date = DateTime.Now.AddSeconds(1);
                    break;
                case "Hourly":
                    date = DateTime.Now.AddHours(1);
                    break;
                case "Bi-Daily":
                    date = DateTime.Now.AddHours(12);
                    break;
                case "Daily":
                    date = DateTime.Now.AddDays(1);
                    break;
                case "Bi-Weekly":
                    date = DateTime.Now.AddDays(3);
                    break;
                case "Weekly":
                    date = DateTime.Now.AddDays(7);
                    break;
                case "Bi-Monthly":
                    date = DateTime.Now.AddDays(14);
                    break;
                case "Monthly":
                    date = DateTime.Now.AddMonths(1);
                    break;
                case "Bi-Anually":
                    date = DateTime.Now.AddMonths(6);
                    break;
                case "Anually":
                    date = DateTime.Now.AddYears(1);
                    break;
            }
            var reoccurance = new Reoccurance()
            {
                FinanceSheetId = expenditure.SheetId,
                PaymentId = expenditure.Id,
                PaymentType = 2,
                ReoccuranceType = expenditure.Reoccurance,
                Date = date
            };

            if (expenditure.Reoccurance != "One Time")
                AddReoccurance(reoccurance);
        }

        public static void AddReoccurance(SavingGoal goal)
        {
            var reoccurance = new Reoccurance()
            {
                FinanceSheetId = goal.SheetId,
                PaymentId = goal.Id,
                PaymentType = 3,
                ReoccuranceType = "One Time",
                Date = goal.EndDate
            };

            AddReoccurance(reoccurance);
        }

        public static void CheckReoccurances()
        {
            List<Reoccurance> result = new List<Reoccurance>();
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"SELECT Id, FinanceSheetId, PaymentId, PaymentType, ReoccuranceType, Date FROM Reoccurances WHERE Date <= getDate()", conn);
                var reader = query.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Reoccurance() { Id = int.Parse(reader[0].ToString()), FinanceSheetId = int.Parse(reader[1].ToString()), PaymentId = int.Parse(reader[2].ToString()), PaymentType = int.Parse(reader[3].ToString()), ReoccuranceType = reader[4].ToString(), Date = DateTime.Parse(reader[5].ToString()) });
                }
                conn.Close();
            }

            foreach(Reoccurance reoccurance in result)
            {
                ActOnReoccurance(reoccurance);
            }
        }

        public static void ActOnReoccurance(Reoccurance reoccurance)
        {
            Reoccurance newReoccurance = reoccurance;
            switch (reoccurance.ReoccuranceType)
            {
                case "Test":
                    newReoccurance.Date = reoccurance.Date.AddSeconds(1);
                    break;
                case "Hourly":
                    newReoccurance.Date = reoccurance.Date.AddHours(1);
                    break;
                case "Bi-Daily":
                    newReoccurance.Date = reoccurance.Date.AddHours(12);
                    break;
                case "Daily":
                    newReoccurance.Date = reoccurance.Date.AddDays(1);
                    break;
                case "Bi-Weekly":
                    newReoccurance.Date = reoccurance.Date.AddDays(3);
                    break;
                case "Weekly":
                    newReoccurance.Date = reoccurance.Date.AddDays(7);
                    break;
                case "Bi-Monthly":
                    newReoccurance.Date = reoccurance.Date.AddDays(14);
                    break;
                case "Monthly":
                    newReoccurance.Date = reoccurance.Date.AddMonths(1);
                    break;
                case "Bi-Anually":
                    newReoccurance.Date = reoccurance.Date.AddMonths(6);
                    break;
                case "Anually":
                    newReoccurance.Date = reoccurance.Date.AddYears(1);
                    break;
            }
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"DELETE FROM Reoccurances WHERE Id = {reoccurance.Id}", conn);
                query.ExecuteNonQuery();
                conn.Close();
            }

            switch(reoccurance.PaymentType)
            {
                case 1:
                    ActOnIncomeReoccurance(newReoccurance);
                    break;
                case 2:
                    ActOnExpenditureReoccurance(newReoccurance);
                    break;
            }

            AddReoccurance(newReoccurance);
        }

        public static void ActOnIncomeReoccurance(Reoccurance reoccurance)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();

                var query = new SqlCommand($"SELECT Amount FROM Incomes WHERE Id = {reoccurance.PaymentId}", conn);
                var reader = query.ExecuteReader();
                reader.Read();
                decimal incomeAmount = decimal.Parse(reader[0].ToString());
                reader.Close();

                FinanceSheetDAL.IncreaseBalance(reoccurance.FinanceSheetId, incomeAmount);

                conn.Close();

            }
        }

        public static void ActOnExpenditureReoccurance(Reoccurance reoccurance)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();

                var query = new SqlCommand($"SELECT Amount FROM Expenditures WHERE Id = {reoccurance.PaymentId}", conn);
                var reader = query.ExecuteReader();
                reader.Read();
                decimal expenditureAmount = decimal.Parse(reader[0].ToString());
                reader.Close();

                FinanceSheetDAL.DecreaseBalance(reoccurance.FinanceSheetId, expenditureAmount);
                conn.Close();
            }
        }

        public static List<Reoccurance> GetSheetReoccurances(int sheetId)
        {
            List<Reoccurance> result = new List<Reoccurance>();
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"SELECT Id, FinanceSheetId, PaymentId, PaymentType, ReoccuranceType, Date FROM Reoccurances WHERE FinanceSheetId = {sheetId}", conn);
                var reader = query.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Reoccurance() { Id = int.Parse(reader[0].ToString()), FinanceSheetId = int.Parse(reader[1].ToString()), PaymentId = int.Parse(reader[2].ToString()), PaymentType = int.Parse(reader[3].ToString()), ReoccuranceType = reader[4].ToString(), Date = DateTime.Parse(reader[5].ToString()) });
                }
                conn.Close();
            }

            return result;
        }

        public static void ReoccuranceChecker()
        {
            while(true)
            {
                CheckReoccurances();
                Thread.Sleep(60000);
            }
        }
    }
}
