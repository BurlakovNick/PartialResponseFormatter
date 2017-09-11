using System;

namespace PartialResponseFormatter.Benchmark.ClientContracts
{
    public class ClientCreditCard
    {
        public DateTime ValidUntil { get; set; }
        public string CardHolder { get; set; }
    }
}