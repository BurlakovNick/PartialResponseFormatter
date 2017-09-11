using System;

namespace PartialResponseFormatter.Benchmark.ServerContracts
{
    public class ServerUserBagItem
    {
        public Guid GoodId { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public decimal VatRate { get; set; }
        public decimal Discount { get; set; }
    }
}