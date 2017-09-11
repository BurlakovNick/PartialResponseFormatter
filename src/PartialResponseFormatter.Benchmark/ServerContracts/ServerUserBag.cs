using System;
using System.Collections.Generic;

namespace PartialResponseFormatter.Benchmark.ServerContracts
{
    public class ServerUserBag
    {
        public DateTime ValidUntil { get; set; }
        public Dictionary<Guid, ServerUserBagItem> Items { get; set; }
        public string PromoCode { get; set; }
    }
}