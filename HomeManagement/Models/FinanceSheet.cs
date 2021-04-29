using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeManagement.Models
{
    public class FinanceSheet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public List<int> Owners = new List<int>();
        public List<Income> Incomes = new List<Income>();
        public List<Expenditure> Expenditures = new List<Expenditure>();
        public List<SavingGoal> SavingGoals = new List<SavingGoal>();
    }
}
