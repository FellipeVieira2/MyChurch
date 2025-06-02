using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mychurch.Common.WebClients.Models.Requests
{
    public class AsaasSubscriptionRequestDto
    {
        public string Customer { get; set; }
        public decimal Value { get; set; }
        public string BillingType { get; set; }
        public string Cycle { get; set; } // "MONTHLY" ou "ANNUAL"
        public string Description { get; set; }
        public DateTime FirstDueDate { get; set; }
        public string ExternalReference { get; set; }
    }
}
