using BenchmarkDotNet.Running;

namespace PartialResponseFormatter.Benchmark
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<FormatterBenchmark>();
        }
    }
}