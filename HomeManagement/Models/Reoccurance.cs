using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeManagement.Models
{
    public class Reoccurance
    {
        public int Id { get; set; }
        public int FinanceSheetId { get; set; }
        public int PaymentId {get; set;}
        public int PaymentType { get; set; }
        public string ReoccuranceType { get; set; }
        public DateTime Date { get; set; }
    }
}
