using HomeManagement.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HomeManagement.DAL
{
    public class ReportDAL
    {
        public static List<Report> GetReports(int id)
        {
            List<Report> result = new List<Report>();
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand("SELECT Id, FinanceSheetId, Rating, Incomes, Expenditures, Goal, Balance, IncomeString, ExpenditureString, GoalString, DateCreated " +
                                           "FROM FinanceReports " +
                                          $"WHERE FinanceSheetId = {id}", conn);
                var reader = query.ExecuteReader();
                while (reader.Read())
                {
                    List<Income> incomes = JsonConvert.DeserializeObject<List<Income>>(reader[3].ToString());
                    List<Expenditure> expenditures = JsonConvert.DeserializeObject<List<Expenditure>>(reader[4].ToString());
                    List<SavingGoal> goals = JsonConvert.DeserializeObject<List<SavingGoal>>(reader[5].ToString());
                    result.Add(new Report() { Id = int.Parse(reader[0].ToString()), FinanceSheetId = int.Parse(reader[1].ToString()), Rating = int.Parse(reader[2].ToString()), Incomes = incomes, Expenditures = expenditures, SavingGoals = goals, Balance = decimal.Parse(reader[6].ToString()), IncomeString = reader[7].ToString(), ExpenditureString = reader[8].ToString(), GoalString = reader[9].ToString(), DateCreated = DateTime.Parse(reader[10].ToString()) });
                }
                conn.Close();
            }
            return result;
        }

        public static Report GetReport(int reportId)
        {
            Report result = new Report();
            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand("SELECT Id, FinanceSheetId, Rating, Incomes, Expenditures, Goal, Balance, IncomeString, ExpenditureString, GoalString, DateCreated " +
                                           "FROM FinanceReports " +
                                          $"WHERE Id = {reportId}", conn);
                var reader = query.ExecuteReader();
                if (reader.Read())
                {
                    List<Income> incomes = JsonConvert.DeserializeObject<List<Income>>(reader[3].ToString());
                    List<Expenditure> expenditures = JsonConvert.DeserializeObject<List<Expenditure>>(reader[4].ToString());
                    List<SavingGoal> goals = JsonConvert.DeserializeObject<List<SavingGoal>>(reader[5].ToString());
                    result = new Report() { Id = int.Parse(reader[0].ToString()), FinanceSheetId = int.Parse(reader[1].ToString()), Rating = int.Parse(reader[2].ToString()), Incomes = incomes, Expenditures = expenditures, SavingGoals = goals, Balance = decimal.Parse(reader[6].ToString()), IncomeString = reader[7].ToString(), ExpenditureString = reader[8].ToString(), GoalString = reader[9].ToString(), DateCreated = DateTime.Parse(reader[10].ToString()) };
                }
                conn.Close();
            }
            return result;
        }

        public static int CreateReport(FinanceSheet sheet)
        {
            int Id;
            Report report = CompileReport(sheet);

            using (var conn = new SqlConnection("Data Source=(localDB)\\MSSQLLOCALDB;Initial Catalog=HomeManagementDB;"))
            {
                conn.Open();
                var query = new SqlCommand("INSERT INTO [dbo].[FinanceReports] ([FinanceSheetId], [Rating], [Incomes], [Expenditures], [Goal], [Balance], [IncomeString], [ExpenditureString], [GoalString], [DateCreated]) " +
                                            "OUTPUT Inserted.ID " +
                                          $"VALUES({report.FinanceSheetId}, {report.Rating}, '{JsonConvert.SerializeObject(report.Incomes)}', '{JsonConvert.SerializeObject(report.Expenditures)}', '{JsonConvert.SerializeObject(report.SavingGoals)}', {report.Balance}, '{report.IncomeString}', '{report.ExpenditureString}', '{report.GoalString}', CONVERT(datetime, '{report.DateCreated}', 103) )", conn);

                var reader = query.ExecuteReader();
                reader.Read();
                Id = int.Parse(reader[0].ToString());
                conn.Close();
            }
            return Id;
        }

        private static Report CompileReport(FinanceSheet sheet)
        {
            Report report = new Report();

            report.FinanceSheetId = sheet.Id;
            report.Rating = 0;
            report.Incomes = GetAnnualIncomeAmounts(sheet);
            report.Expenditures = GetAnnualExpenditureAmounts(sheet);
            report.SavingGoals = sheet.SavingGoals;
            report.Balance = sheet.Balance;


            decimal annualIncome = 0;
            decimal annualNeedExpenditures = 0;
            decimal annualWantExpenditures = 0;
            if (sheet.Incomes.Count >= 1)
            {
                annualIncome = getSheetIncomeTotal(report);
                report.IncomeString = IncomeStringBuilder(report, annualIncome);
            }

            if(sheet.Expenditures.Count >= 1)
                (annualNeedExpenditures, annualWantExpenditures) = getSheetExpendituresTotal(report);

            if (sheet.Incomes.Count >= 1 && sheet.Expenditures.Count >= 1)
            {
                report.Rating = GetRating(annualIncome, annualNeedExpenditures, annualWantExpenditures);
                report.ExpenditureString = ExpenditureStringBuilder(report, annualIncome, annualNeedExpenditures,
                    annualWantExpenditures);
            }

            if (sheet.SavingGoals.Count >= 1)
                report.GoalString = GoalStringBuilder(report);

            report.DateCreated = DateTime.Now;

            return report;
        }

        private static int GetRating(decimal annualIncome, decimal annualNeedExpenditures, decimal annualWantExpenditures)
        {
            int rating = 5;

            if (annualNeedExpenditures / annualIncome > (decimal) 0.5)
                rating--;
            if (annualWantExpenditures / annualIncome > (decimal) 0.3)
                rating--;
            if ((annualNeedExpenditures + annualWantExpenditures) / annualIncome > (decimal) 0.7)
                rating--;
            if (annualIncome - (annualNeedExpenditures + annualWantExpenditures) < 0)
                rating = 0;
            return rating;
        }

        private static string IncomeStringBuilder(Report report, decimal annualIncome)
        {
            int percentile = GetIncomePercentile(annualIncome);
            string percentileFeedback = percentile > 50
                ? "Your current total household income is above average! While there are always ways you could improve your finances, your household has an adequate "
                : "You are below average, meaning that you may want to consider diversifying your income streams or try to increase your gains from your current incomes ";
            
            int dividendCount = 0;
            foreach (var income in report.Incomes)
            {
                if (income.Type == 2)
                    dividendCount++;
            }

            string dividendFeedback = dividendCount > 20
                ? "You have currently more than 20 dividends incoming, this is an average amount. Recommendations are to have around 20-60 dividends. "
                : $"You currently only have {dividendCount} dividends, this is below the recommended amount of 20-60. If you are planning on becoming more financially independent, it is a good idea to place your money into passive income streams! ";

            return $"Your expected annual household income for the year of {DateTime.UtcNow.Year} is £{String.Format("{0:n}", annualIncome)}. You are in the {percentile} percentile. " + percentileFeedback + dividendFeedback;
        }

        private static string ExpenditureStringBuilder(Report report, decimal annualIncome, decimal annualNeedExpenditures, decimal annualWantExpenditures)
        {
            string result = $"Your current total annual expenditures for the year are £{String.Format("{0:n}", annualNeedExpenditures + annualWantExpenditures)}. ";
            if ((annualNeedExpenditures + annualWantExpenditures) / annualIncome > (decimal) 0.7)
                result += "The amount of money you are spending exceeds the recommended amount! Try to cut back on your spending and allow for enough excess money so that if one or more of your income streams dry up you have something to fall back on. ";

            result += annualWantExpenditures / annualIncome > (decimal) 0.3
                ? "You seem to be spending money quite recklessly, try cutting back a little on your unnecessary spending. You shouldn't be spending more than 30% of your income on unneeded products. "
                : "You are spending your money wisely! Good job! Try to keep non essential spending down below 30% of your total income.";
            return result;
        }

        private static string GoalStringBuilder(Report report)
        {
            int soonestGoal = 0;
            for (int i = 0; i <= report.SavingGoals.Count - 1; i++)
            {
                if (report.SavingGoals[i].EndDate.CompareTo(report.SavingGoals[soonestGoal].EndDate) > 0)
                    soonestGoal = i;
            }

            return
                $"Your closest goal is {report.SavingGoals[soonestGoal].Name}. You currently have deposited £{String.Format("{0:n}", report.SavingGoals[soonestGoal].Deposited) } out of £{String.Format("{0:n}", report.SavingGoals[soonestGoal].Amount) }. You have {report.SavingGoals[soonestGoal].EndDate.Subtract(DateTime.UtcNow).Days} days left before your end date has been met!";
        }

        private static int GetIncomePercentile(decimal annualIncome)
        {
            int result = 0;
            if (annualIncome > 116000)
                return 99;
            if (annualIncome > 84600)
                return 98;
            if (annualIncome > 73700)
                return 97;
            if (annualIncome > 66100)
                return 96;
            if (annualIncome > 60500)
                return 95;
            if (annualIncome > 56300)
                return 94;
            if (annualIncome > 53000)
                return 93;
            if (annualIncome > 50400)
                return 92;
            if (annualIncome > 48300)
                return 91;
            if (annualIncome > 46500)
                return 90;
            if (annualIncome > 45100)
                return 89;
            if (annualIncome > 43900)
                return 88;
            if (annualIncome > 43000)
                return 87;
            if (annualIncome > 42000)
                return 86;
            if (annualIncome > 41000)
                return 85;
            if (annualIncome > 40100)
                return 84;
            if (annualIncome > 39200)
                return 83;
            if (annualIncome > 38300)
                return 82;
            if (annualIncome > 37400)
                return 81;
            if (annualIncome > 36600)
                return 80;
            if (annualIncome > 35800)
                return 79;
            if (annualIncome > 35100)
                return 78;
            if (annualIncome > 34400)
                return 77;
            if (annualIncome > 33700)
                return 76;
            if (annualIncome > 33000)
                return 75;
            if (annualIncome > 32400)
                return 74;
            if (annualIncome > 31800)
                return 73;
            if (annualIncome > 31200)
                return 72;
            if (annualIncome > 30700)
                return 71;
            if (annualIncome > 30200)
                return 70;
            if (annualIncome > 29700)
                return 69;
            if (annualIncome > 29200)
                return 68;
            if (annualIncome > 28700)
                return 67;
            if (annualIncome > 28300)
                return 66;
            if (annualIncome > 27800)
                return 65;
            if (annualIncome > 27400)
                return 64;
            if (annualIncome > 27000)
                return 63;
            if (annualIncome > 26600)
                return 62;
            if (annualIncome > 26200)
                return 61;
            if (annualIncome > 25900)
                return 60;
            if (annualIncome > 25500)
                return 59;
            if (annualIncome > 25100)
                return 58;
            if (annualIncome > 24800)
                return 57;
            if (annualIncome > 24400)
                return 56;
            if (annualIncome > 24100)
                return 55;
            if (annualIncome > 23800)
                return 54;
            if (annualIncome > 23500)
                return 53;
            if (annualIncome > 23200)
                return 52;
            if (annualIncome > 22900)
                return 51;
            if (annualIncome > 22600)
                return 50;
            if (annualIncome > 22300)
                return 49;
            if (annualIncome > 22000)
                return 48;
            if (annualIncome > 21800)
                return 47;
            if (annualIncome > 21500)
                return 46;
            if (annualIncome > 21200)
                return 45;
            if (annualIncome > 21000)
                return 44;
            if (annualIncome > 20700)
                return 43;
            if (annualIncome > 20500)
                return 42;
            if (annualIncome > 20200)
                return 41;
            if (annualIncome > 20000)
                return 40;
            if (annualIncome > 19700)
                return 39;
            if (annualIncome > 19500)
                return 38;
            if (annualIncome > 19200)
                return 37;
            if (annualIncome > 19000)
                return 36;
            if (annualIncome > 18800)
                return 35;
            if (annualIncome > 18600)
                return 34;
            if (annualIncome > 18400)
                return 33;
            if (annualIncome > 18100)
                return 32;
            if (annualIncome > 17900)
                return 31;
            if (annualIncome > 17700)
                return 30;
            if (annualIncome > 17500)
                return 29;
            if (annualIncome > 17300)
                return 28;
            if (annualIncome > 17100)
                return 27;
            if (annualIncome > 16900)
                return 26;
            if (annualIncome > 16700)
                return 25;
            if (annualIncome > 16500)
                return 24;
            if (annualIncome > 16300)
                return 23;
            if (annualIncome > 16100)
                return 22;
            if (annualIncome > 15900)
                return 21;
            if (annualIncome > 15800)
                return 20;
            if (annualIncome > 15600)
                return 19;
            if (annualIncome > 15400)
                return 18;
            if (annualIncome > 15200)
                return 17;
            if (annualIncome > 15000)
                return 16;
            if (annualIncome > 14800)
                return 15;
            if (annualIncome > 14600)
                return 14;
            if (annualIncome > 14400)
                return 13;
            if (annualIncome > 14200)
                return 12;
            if (annualIncome > 14000)
                return 11;
            if (annualIncome > 13800)
                return 10;
            if (annualIncome > 13600)
                return 9;
            if (annualIncome > 13400)
                return 8;
            if (annualIncome > 13200)
                return 7;
            if (annualIncome > 13100)
                return 6;
            if (annualIncome > 12900)
                return 5;
            if (annualIncome > 12700)
                return 4;
            if (annualIncome > 12500)
                return 3;
            if (annualIncome > 12200)
                return 2;
            if (annualIncome > 12000)
                return 1;

            return result;
        }

        private static decimal getSheetIncomeTotal(Report report)
        {
            decimal annualIncome = 0;

            foreach (Income income in report.Incomes)
                annualIncome += income.annualAmount;

            return annualIncome;
        }

        private static (decimal, decimal) getSheetExpendituresTotal(Report report)
        {
            decimal annualNeedExpenditures = 0;
            decimal annualWantExpenditures = 0;

            foreach (Expenditure expenditure in report.Expenditures)
            {
                if (expenditure.Type == 1)
                    annualNeedExpenditures += expenditure.annualAmount;
                else
                    annualWantExpenditures += expenditure.annualAmount;
            }

            return (annualNeedExpenditures, annualWantExpenditures);
        }

        private static List<Expenditure> GetAnnualExpenditureAmounts(FinanceSheet sheet)
        {
            List<Expenditure> incomes = new List<Expenditure>();

            foreach (Expenditure expenditure in sheet.Expenditures)
            {
                decimal expenditureAmount = expenditure.Amount;
                switch (expenditure.Reoccurance)
                {
                    case "Hourly":
                        expenditureAmount *= 8760;
                        break;
                    case "Bi-Daily":
                        expenditureAmount *= 730;
                        break;
                    case "Daily":
                        expenditureAmount *= 365;
                        break;
                    case "Bi-Weekly":
                        expenditureAmount *= 104;
                        break;
                    case "Weekly":
                        expenditureAmount *= 52;
                        break;
                    case "Bi-Monthly":
                        expenditureAmount *= 24;
                        break;
                    case "Monthly":
                        expenditureAmount *= 12;
                        break;
                    case "Bi-Anually":
                        expenditureAmount *= 2;
                        break;
                    case "Anually":
                        expenditureAmount *= 1;
                        break;
                }

                expenditure.annualAmount = expenditureAmount;
                incomes.Add(expenditure);
            }

            return incomes;
        }

        private static List<Income> GetAnnualIncomeAmounts(FinanceSheet sheet)
        {
            List<Income> incomes = new List<Income>();

            foreach (Income income in sheet.Incomes)
            {
                decimal incomeAmount = income.Amount;
                switch (income.Reoccurance)
                {
                    case "Hourly":
                        incomeAmount *= 8760;
                        break;
                    case "Bi-Daily":
                        incomeAmount *= 730;
                        break;
                    case "Daily":
                        incomeAmount *= 365;
                        break;
                    case "Bi-Weekly":
                        incomeAmount *= 104;
                        break;
                    case "Weekly":
                        incomeAmount *= 52;
                        break;
                    case "Bi-Monthly":
                        incomeAmount *= 24;
                        break;
                    case "Monthly":
                        incomeAmount *= 12;
                        break;
                    case "Bi-Anually":
                        incomeAmount *= 2;
                        break;
                    case "Anually":
                        incomeAmount *= 1;
                        break;
                }

                income.annualAmount = incomeAmount;
                incomes.Add(income);
            }

            return incomes;
        }
    }
}
