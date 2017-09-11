using System;
using System.Collections.Generic;

namespace PartialResponseFormatter.Benchmark.ClientContracts
{
    public class ClientUserBag
    {
        public DateTime ValidUntil { get; set; }
        public Dictionary<Guid, ClientUserBagItem> Items { get; set; }
    }
}