using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeManagement.Models
{
    public class Expenditure
    {
        public int Id { get; set; }
        public int SheetId { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public decimal Amount { get; set; }
        public string Reoccurance { get; set; }
        public bool PayPreemptively { get; set; }
        public decimal annualAmount { get; set; }
    }
}
