using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeManagement.Models
{
    public class Report
    {
        public int Id { get; set; }
        public int FinanceSheetId { get; set; }
        public int Rating { get; set; }
        public List<Income> Incomes { get; set; }
        public List<Expenditure> Expenditures { get; set; }
        public List<SavingGoal> SavingGoals { get; set; }
        public decimal Balance { get; set; }
        public string IncomeString { get; set; }
        public string ExpenditureString { get; set; }
        public string GoalString { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
