using System;

namespace PartialResponseFormatter.Benchmark.ServerContracts
{
    public class ServerCreditCard
    {
        public DateTime ValidUntil { get; set; }
        public string CardHolder { get; set; }
        public string Number { get; set; }
    }
}