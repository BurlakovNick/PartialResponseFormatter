using System;

namespace PartialResponseFormatter.Benchmark.ServerContracts
{
    public class ServerPreviousPurchase
    {
        public DateTime PurchaseDate { get; set; }
        public bool IsPaid { get; set; }
        public bool IsDelivered { get; set; }
        public ServerPreviousPurchaseItem[] Items { get; set; }
    }
}