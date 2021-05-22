using HomeManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HomeManagement.DAL
{
    public class FinanceSheetDAL
    {
        public static List<string> GetSheets()
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand("SELECT Name FROM FinanceSheets;", conn);
                var reader = query.ExecuteReader();
                var result = new List<string>();
                while (reader.Read())
                {
                    result.Add(reader[0].ToString());
                }
                conn.Close();

                return result;
            }
        }

        public static FinanceSheet GetSheetData(int userId, int sheetId)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand("SELECT F.Id, F.Name, F.Balance " +
                                           "FROM FinanceSheets as F " +
                                           "INNER JOIN UserFinanceSheet as U " +
                                           "ON F.Id = U.FinanceSheets_Id " +
                                           $"WHERE U.Users_Id = {userId} AND F.Id = {sheetId}", conn);
                var reader = query.ExecuteReader();
                var result = new FinanceSheet();
                if (reader.Read())
                {
                    result.Id = int.Parse(reader[0].ToString());
                    result.Owners.Add(userId);
                    result.Name = reader[1].ToString();
                    result.Balance = decimal.Parse(reader[2].ToString());
                    result.Incomes = GetIncomes(sheetId);
                    result.Expenditures = GetExpenditures(sheetId);
                    result.SavingGoals = GetGoals(sheetId);
                }
                conn.Close();
                return result;
            }
        }

        public static List<FinanceSheet> GetUsersSheets(int id)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand("SELECT F.Id, F.Name, F.Balance " +
                                           "FROM FinanceSheets as F " +
                                           "INNER JOIN UserFinanceSheet as U " +
                                           "ON F.Id = U.FinanceSheets_Id " +
                                           $"WHERE U.Users_Id = {id}", conn);
                var reader = query.ExecuteReader();
                var result = new List<FinanceSheet>();
                while (reader.Read())
                {
                    result.Add(new FinanceSheet() { Id = int.Parse(reader[0].ToString()), Name = reader[1].ToString(), Balance = decimal.Parse(reader[2].ToString()) });
                }
                conn.Close();
                return result;
            }
        }

        public static List<Expenditure> GetExpenditures(int sheetId)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand("SELECT Id, Name, FinanceSheetId, Type, Amount, Reoccurance " +
                                           "FROM Expenditures " +
                                           $"WHERE FinanceSheetId = {sheetId}", conn);
                var reader = query.ExecuteReader();
                var result = new List<Expenditure>();
                while (reader.Read())
                {
                    result.Add(new Expenditure { Id = int.Parse(reader[0].ToString()), Name = reader[1].ToString(), SheetId = int.Parse(reader[2].ToString()), Type = int.Parse(reader[3].ToString()), Amount = decimal.Parse(reader[4].ToString()), Reoccurance = reader[5].ToString() });
                }
                conn.Close();
                return result;
            }
        }

        public static List<Income> GetIncomes(int sheetId)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand("SELECT Id, Name, FinanceSheetId, Type, Amount, Reoccurance " +
                                           "FROM Incomes " +
                                           $"WHERE FinanceSheetId = {sheetId}", conn);
                var reader = query.ExecuteReader();
                var result = new List<Income>();
                while (reader.Read())
                {
                    result.Add(new Income { Id = int.Parse(reader[0].ToString()), Name = reader[1].ToString(), SheetId = int.Parse(reader[2].ToString()), Type = int.Parse(reader[3].ToString()), Amount = decimal.Parse(reader[4].ToString()), Reoccurance = reader[5].ToString() });
                }
                conn.Close();
                return result;
            }
        }

        public static List<SavingGoal> GetGoals(int sheetId)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand("SELECT Id, FinanceSheetId, Name, Type, Amount, EndDate, Description, Deposited " +
                                           "FROM SavingGoals " +
                                           $"WHERE FinanceSheetId = {sheetId}", conn);
                var reader = query.ExecuteReader();
                var result = new List<SavingGoal>();
                while (reader.Read())
                {
                    result.Add(new SavingGoal() { Id = int.Parse(reader[0].ToString()), SheetId = int.Parse(reader[1].ToString()), Name = reader[2].ToString(), Type = int.Parse(reader[3].ToString()), Amount = decimal.Parse(reader[4].ToString()), EndDate = DateTime.Parse(reader[5].ToString()), Description = reader[6].ToString(), Deposited = reader[7].ToString() == "" ? 0 : decimal.Parse(reader[7].ToString()) });
                }
                conn.Close();
                return result;
            }
        }

        public static int Create(FinanceSheet sheet)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand("INSERT INTO FinanceSheets(Name, Balance) " +
                                           "OUTPUT Inserted.ID " +
                                           $"Values(@name, {sheet.Balance}); "
                                           , conn);
                query.Parameters.Add("@name", SqlDbType.VarChar);
                query.Parameters["@name"].Value = sheet.Name;

                var reader = query.ExecuteReader();
                reader.Read();
                var result = int.Parse(reader[0].ToString());
                reader.Close();
                var query2 = new SqlCommand("INSERT INTO [dbo].[UserFinanceSheet]([Users_Id], [FinanceSheets_Id])"
                                            + $"VALUES({sheet.Owners[0]}, {result});", conn);
                query2.ExecuteNonQuery();
                CreateExpenditures(sheet.Expenditures, result, conn);
                CreateIncomes(sheet.Incomes, result, conn);
                CreateGoals(sheet.SavingGoals, result, conn);
                conn.Close();
                return result;
            }
        }

        public static void AddOwner(int sheetId, int userId)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query2 = new SqlCommand("INSERT INTO [dbo].[UserFinanceSheet]([Users_Id], [FinanceSheets_Id])"
                                            + $"VALUES({userId}, {sheetId});", conn);
                query2.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static List<int> CreateExpenditures(List<Expenditure> expenditures, int sheetId, SqlConnection conn)
        {
            List<int> result = new List<int>();
            foreach(Expenditure expenditure in expenditures)
            {
                var query = new SqlCommand("INSERT INTO Expenditures(Name, FinanceSheetId, Type, Amount, Reoccurance) " +
                                           "OUTPUT Inserted.ID " +
                                           $"Values(@name, {sheetId}, {expenditure.Type}, {expenditure.Amount}, '{expenditure.Reoccurance}'); "
                                           , conn);
                query.Parameters.Add("@name", SqlDbType.VarChar);
                query.Parameters["@name"].Value = expenditure.Name;

                var reader = query.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(int.Parse(reader[0].ToString()));
                    expenditure.Id = int.Parse(reader[0].ToString());
                }
                expenditure.SheetId = sheetId;
                ReoccuranceDAL.AddReoccurance(expenditure);
                reader.Close();

                if (expenditure.PayPreemptively)
                    DecreaseBalance(sheetId, expenditure.Amount);
            }
            return result;
        }

        public static List<int> CreateIncomes(List<Income> incomes, int sheetId, SqlConnection conn)
        {
            List<int> result = new List<int>();
            foreach (Income income in incomes)
            {
                var query = new SqlCommand("INSERT INTO Incomes(Name, FinanceSheetId, Type, Amount, Reoccurance) " +
                                           "OUTPUT Inserted.ID " +
                                           $"Values(@name, {sheetId}, {income.Type}, {income.Amount}, '{income.Reoccurance}'); "
                                           , conn);
                query.Parameters.Add("@name", SqlDbType.VarChar);
                query.Parameters["@name"].Value = income.Name;
                var reader = query.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(int.Parse(reader[0].ToString()));
                    income.Id = int.Parse(reader[0].ToString());
                }
                reader.Close();

                if (income.PayPreemptively)
                    IncreaseBalance(sheetId, income.Amount);

                income.SheetId = sheetId;
                ReoccuranceDAL.AddReoccurance(income);
            }
            return result;
        }

        public static void Delete(int sheetId)
        {
            var incomes = GetIncomes(sheetId);
            var expenditures = GetExpenditures(sheetId);
            var goals = GetGoals(sheetId);

            foreach (Income income in incomes)
                DeleteIncome(new List<int>() { income.Id });

            foreach (Expenditure expenditure in expenditures)
                DeleteExpenditure(new List<int>() { expenditure.Id });

            foreach (SavingGoal goal in goals)
                DeleteGoal(new List<int>() { goal.Id });

            DeleteOwners(sheetId);

            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"DELETE FROM FinanceSheets WHERE Id = {sheetId};", conn);
                var reader = query.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static void DeleteOwners(int sheetId)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"DELETE FROM UserFinanceSheet WHERE FinanceSheets_Id = {sheetId};", conn);
                var reader = query.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static void DeleteIncome(List<int> incomeIds)
        {
            foreach (int incomeId in incomeIds)
            {
                DeleteReoccurance(incomeId, 1);
                using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
                {
                    conn.Open();
                    var query = new SqlCommand($"DELETE FROM Incomes WHERE Id = {incomeId};", conn);
                    var reader = query.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static void DeleteExpenditure(List<int> expenditureIds)
        {
            foreach (int expenditureId in expenditureIds)
            {
                DeleteReoccurance(expenditureId, 2);
                using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
                {
                    conn.Open();
                    var query = new SqlCommand($"DELETE FROM Expenditures WHERE Id = {expenditureId};", conn);
                    var reader = query.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static void DeleteGoal(List<int> goalIds)
        {
            foreach (int goalId in goalIds)
            {
                DeleteReoccurance(goalId, 3);
                using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
                {
                    conn.Open();
                    var query = new SqlCommand($"DELETE FROM SavingGoals WHERE Id = {goalId};", conn);
                    var reader = query.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static void DeleteReoccurance(int paymentId, int paymentType)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"DELETE FROM Reoccurances WHERE PaymentId = {paymentId} AND PaymentType = {paymentType};", conn);
                var reader = query.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static List<int> CreateGoals(List<SavingGoal> goals, int sheetId, SqlConnection conn)
        {
            List<int> result = new List<int>();
            foreach (SavingGoal goal in goals)
            {
                var query = new SqlCommand("INSERT INTO SavingGoals( Name, Description, FinanceSheetId, Type, Amount, EndDate) " +
                                           "OUTPUT Inserted.ID " +
                                           $"Values(@name, @description, {sheetId}, {goal.Type}, {goal.Amount}, CONVERT(datetime, '{goal.EndDate}', 103)); "
                                           , conn);
                query.Parameters.Add("@name", SqlDbType.VarChar);
                query.Parameters["@name"].Value = goal.Name;
                query.Parameters.Add("@description", SqlDbType.VarChar);
                query.Parameters["@description"].Value = goal.Description;
                var reader = query.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(int.Parse(reader[0].ToString()));
                    goal.Id = int.Parse(reader[0].ToString());
                }
                goal.SheetId = sheetId;
                ReoccuranceDAL.AddReoccurance(goal);
                reader.Close();
            }
            return result;
        }

        public static void IncreaseBalance(int sheetId, decimal amount)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"SELECT Balance FROM FinanceSheets WHERE Id = {sheetId}", conn);
                var reader = query.ExecuteReader();
                reader.Read();
                decimal Balance = decimal.Parse(reader[0].ToString());
                reader.Close();

                decimal newBalance = Balance + amount;

                query = new SqlCommand($"UPDATE FinanceSheets SET Balance={newBalance} WHERE Id = {sheetId}", conn);
                query.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static void DecreaseBalance(int sheetId, decimal amount)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"SELECT Balance FROM FinanceSheets WHERE Id = {sheetId}", conn);
                var reader = query.ExecuteReader();
                reader.Read();
                decimal Balance = decimal.Parse(reader[0].ToString());
                reader.Close();

                decimal newBalance = Balance - amount;

                query = new SqlCommand($"UPDATE FinanceSheets SET Balance={newBalance} WHERE Id = {sheetId}", conn);
                query.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static void AddDeposit(int sheetId, int goalId, decimal amount)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"SELECT Balance FROM FinanceSheets WHERE Id={sheetId}", conn);
                var reader = query.ExecuteReader();
                reader.Read();
                var balance = reader[0].ToString() == "" ? 0 : decimal.Parse(reader[0].ToString());
                reader.Close();

                var query2 = new SqlCommand($"SELECT Deposited FROM SavingGoals WHERE Id={goalId}", conn);
                var reader2 = query2.ExecuteReader();
                reader2.Read();
                var amountDeposited = reader2[0].ToString() == "" ? 0 : decimal.Parse(reader2[0].ToString());
                reader2.Close();

                balance -= amount;
                amountDeposited += amount; 

                var query3 = new SqlCommand("UPDATE SavingGoals " +
                                            $"SET Deposited = {amountDeposited} " +
                                            $"WHERE Id = {goalId}; " +
                                            "UPDATE FinanceSheets " +
                                            $"SET Balance = {balance} " +
                                            $"WHERE Id = {sheetId}; ", conn);
                query3.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static bool SheetExists(string sheetName, int userId)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"SELECT 1 " +
                                            "FROM FinanceSheets " +
                                            "INNER JOIN UserFinanceSheet ON FinanceSheets.Id = UserFinanceSheet.FinanceSheets_Id " +
                                           $"WHERE FinanceSheets.Name = @name " +
                                           $"AND UserFinanceSheet.Users_Id = { userId }", conn);
                query.Parameters.Add("@name", SqlDbType.VarChar);
                query.Parameters["@name"].Value = sheetName;

                var reader = query.ExecuteReader();
                if (reader.Read())
                    return true;
                else
                    return false;
            }
        }

        public static bool IncomeExists(string name, int sheetId)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"SELECT 1 "+
                                            "FROM Incomes "+
                                            "INNER JOIN UserFinanceSheet ON Incomes.FinanceSheetId = UserFinanceSheet.FinanceSheets_Id "+
                                           $"WHERE Incomes.Name = @name " +
                                           $"AND Incomes.FinanceSheetId = {sheetId}", conn);
                query.Parameters.Add("@name", SqlDbType.VarChar);
                query.Parameters["@name"].Value = name;
                var reader = query.ExecuteReader();
                if (reader.Read())
                    return true;
                else
                    return false;
            }
        }

        public static bool ExpenditureExists(string name, int sheetId)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"SELECT 1 " +
                                            "FROM Expenditures " +
                                           $"WHERE Expenditures.Name = @name " +
                                           $"AND Expenditures.FinanceSheetId = {sheetId}", conn);
                query.Parameters.Add("@name", SqlDbType.VarChar);
                query.Parameters["@name"].Value = name;
                var reader = query.ExecuteReader();
                if (reader.Read())
                    return true;
                else
                    return false;
            }
        }

        public static bool GoalExists(string name, int sheetId)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"SELECT 1 " +
                                            "FROM SavingGoals " +
                                           $"WHERE SavingGoals.Name = @name " +
                                           $"AND SavingGoals.FinanceSheetId = {sheetId}", conn);
                query.Parameters.Add("@name", SqlDbType.VarChar);
                query.Parameters["@name"].Value = name;
                var reader = query.ExecuteReader();
                if (reader.Read())
                    return true;
                else
                    return false;
            }
        }

        public static bool ReoccuranceExists(int paymentId, int paymentType)
        {
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand($"SELECT 1 " +
                                            "FROM Reoccurances " +
                                           $"WHERE PaymentId = {paymentId} " +
                                           $"AND PaymentType = {paymentType}; ", conn);
                var reader = query.ExecuteReader();
                if (reader.Read())
                    return true;
                else
                    return false;
            }
        }
    }
}
