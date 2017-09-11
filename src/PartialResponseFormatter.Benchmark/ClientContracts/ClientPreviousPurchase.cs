using System;

namespace PartialResponseFormatter.Benchmark.ClientContracts
{
    public class ClientPreviousPurchase
    {
        public DateTime PurchaseDate { get; set; }
        public ClientPreviousPurchaseItem[] Items { get; set; }
    }
}