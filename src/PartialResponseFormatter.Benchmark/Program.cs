using BenchmarkDotNet.Running;

namespace PartialResponseFormatter.Benchmark
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<FormatterBenchmark>();
        }
    }
}