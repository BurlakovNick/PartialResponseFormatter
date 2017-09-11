using System;

namespace PartialResponseFormatter.Benchmark.ServerContracts
{
    public class ServerUserAccount
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public DateTime LastVisitTime { get; set; }
        public string UserEmail { get; set; }
        public ServerUserBag UserBag { get; set; }
        public ServerPreviousPurchase[] PreviousPurchases { get; set; }
        public ServerContactPhone[] ContactPhone { get; set; }
        public ServerCreditCard[] CreditCards { get; set; }
    }
}