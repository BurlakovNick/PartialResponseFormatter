using System;

namespace PartialResponseFormatter.Benchmark.ClientContracts
{
    public class ClientUserAccount
    {
        public string UserName { get; set; }
        public DateTime LastVisitTime { get; set; }
        public ClientUserBag UserBag { get; set; }
        public ClientPreviousPurchase[] PreviousPurchases { get; set; }
        public ClientCreditCard[] CreditCards { get; set; }
    }
}