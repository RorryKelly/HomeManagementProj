using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeManagement.Models
{
    public class SavingGoal
    {
        public int Id { get; set; }
        public int SheetId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public decimal Amount { get; set; }
        public decimal Deposited { get; set; }
        public DateTime EndDate { get; set; }
    }
}
